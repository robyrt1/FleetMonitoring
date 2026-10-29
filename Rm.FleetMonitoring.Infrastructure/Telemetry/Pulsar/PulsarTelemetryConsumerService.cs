namespace Rm.FleetMonitoring.Infrastructure.Telemetry.Pulsar;

using System.Text.Json;
using System.Threading.Channels;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rm.FleetMonitoring.Application.Common.Models;
using Rm.FleetMonitoring.Application.UseCases.ProcessTelemetry;

public class PulsarTelemetryConsumerService : BackgroundService
{
    private readonly IPulsarClient _pulsarClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PulsarTelemetryConsumerService> _logger;

    public PulsarTelemetryConsumerService(
        IPulsarClient pulsarClient,
        IServiceProvider serviceProvider,
        ILogger<PulsarTelemetryConsumerService> logger)
    {
        _pulsarClient = pulsarClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var consumer = _pulsarClient.NewConsumer(Schema.String)
            .Topic("persistent://public/default/rm-motor-telemetry")
            .SubscriptionName("fleet-monitoring-consumer-sub")
            .SubscriptionType(SubscriptionType.Shared)
            .MessagePrefetchCount(5000)
            .Create();

        _logger.LogInformation("[DOTPULSAR] Consumidor de alta taxa pronto (Prefetch: 5000).");

        var channel = Channel.CreateBounded<(IMessage<string> Message, string Payload)>(new BoundedChannelOptions(10_000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = false
        });

        const int workerConcurrency = 50;
        var processorTasks = Enumerable.Range(0, workerConcurrency)
            .Select(_ => ProcessChannelItemsAsync(consumer, channel.Reader, stoppingToken))
            .ToArray();

        try
        {
            await foreach (var message in consumer.Messages(stoppingToken))
            {
                var payloadString = message.Value()?.Trim();
                if (!string.IsNullOrEmpty(payloadString) && (payloadString.StartsWith('{') || payloadString.StartsWith('[')))
                {
                    await channel.Writer.WriteAsync((message, payloadString), stoppingToken);
                }
                else
                {
                    await consumer.Acknowledge(message.MessageId, stoppingToken);
                }
            }
        }
        finally
        {
            channel.Writer.Complete();
            await Task.WhenAll(processorTasks);
        }
    }

    private async Task ProcessChannelItemsAsync(
        IConsumer<string> consumer,
        ChannelReader<(IMessage<string> Message, string Payload)> reader,
        CancellationToken ct)
    {
        while (await reader.WaitToReadAsync(ct))
        {
            while (reader.TryRead(out var item))
            {
                try
                {
                    var dto = JsonSerializer.Deserialize<TelemetryDto>(item.Payload, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (dto is not null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<ProcessTelemetryHandler>();
                        await handler.HandleAsync(new ProcessTelemetryCommand(dto), ct);
                    }

                    await consumer.Acknowledge(item.Message.MessageId, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar telemetria ID {MessageId}", item.Message.MessageId);
                    await consumer.Acknowledge(item.Message.MessageId, ct);
                }
            }
        }
    }
}