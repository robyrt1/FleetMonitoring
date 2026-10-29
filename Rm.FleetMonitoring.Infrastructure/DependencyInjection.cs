using DotPulsar.Abstractions;
namespace Rm.FleetMonitoring.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotPulsar;
using DotPulsar.Extensions;
using Temporalio.Client;
using Rm.FleetMonitoring.Application.Common.Interfaces;
using Rm.FleetMonitoring.Application.UseCases.ProcessTelemetry;
using Rm.FleetMonitoring.Infrastructure.Orchestration.Temporal;
using Rm.FleetMonitoring.Infrastructure.Telemetry.Pulsar;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var pulsarUrl = configuration["Pulsar:ServiceUrl"] ?? "pulsar://localhost:6650";
        var temporalHost = configuration["Temporal:Host"] ?? "localhost:7233";

        // DotPulsar Client
        services.AddSingleton<IPulsarClient>(_ => PulsarClient.Builder()
            .ServiceUrl(new Uri(pulsarUrl))
            .Build());

        // Temporal Client
        services.AddSingleton<ITemporalClient>(_ => 
            TemporalClient.ConnectAsync(new TemporalClientConnectOptions(temporalHost)).GetAwaiter().GetResult());

        // Handlers e Serviços de Domínio/Aplicação
        services.AddSingleton<IMaintenanceActivities, MaintenanceActivities>();        
        services.AddScoped<ProcessTelemetryHandler>();
        services.AddScoped<IMotorTelemetryWorkflowLauncher, TemporalWorkflowLauncher>();

        // Hosted Services em background
        services.AddHostedService<PulsarTelemetryConsumerService>();
        services.AddHostedService<TemporalWorkerHostedService>();

        return services;
    }
}