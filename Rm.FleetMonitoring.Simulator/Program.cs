using System.Diagnostics;
using System.Text.Json;
using DotPulsar;
using DotPulsar.Extensions;

await using var client = PulsarClient.Builder()
    .ServiceUrl(new Uri("pulsar://localhost:6650"))
    .Build();

await using var producer = client.NewProducer(Schema.String)
    .Topic("persistent://public/default/rm-motor-telemetry")
    .Create();

const int totalMessages = 50_000;
const int maxParallelism = 200;

Console.WriteLine($"Iniciando teste de estresse: {totalMessages} mensagens (Concorrência: {maxParallelism})...");

var stopwatch = Stopwatch.StartNew();
using var throttler = new SemaphoreSlim(maxParallelism);

var tasks = Enumerable.Range(1, totalMessages).Select(async i =>
{
    await throttler.WaitAsync();
    try
    {
        var isAnomaly = i % 50 == 0;
        var payload = new
        {
            MotorId = $"rm-W22-IE4-{i % 250:D3}",
            PlantId = "JARAGUA-FABRICA-02",
            RmsVibration = isAnomaly ? 8.2 : 2.1,
            TemperatureCelsius = isAnomaly ? 89.0 : 41.0,
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(payload);
        await producer.Send(json);
    }
    finally
    {
        throttler.Release();
    }
});

await Task.WhenAll(tasks);
stopwatch.Stop();

var throughput = totalMessages / stopwatch.Elapsed.TotalSeconds;
Console.WriteLine($"Concluído em: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2}s)");
Console.WriteLine($"Taxa média de envio: {throughput:F2} msg/s");