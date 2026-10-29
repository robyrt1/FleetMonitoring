using Microsoft.Extensions.Hosting;
using Rm.FleetMonitoring.Infrastructure;
using Rm.FleetMonitoring.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Registra Pulsar, Temporal e toda a infraestrutura
builder.Services.AddInfrastructure(builder.Configuration);

// Mantém o heartbeat do Worker padrão se desejar
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();