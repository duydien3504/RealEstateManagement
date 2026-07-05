using RabbitMQ.Client;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.EmailWorker.Workers;
using RealEstateSystem.Infrastructure.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var rabbitHost = context.Configuration["RabbitMq:Host"] ?? "localhost";
        var rabbitPort = int.Parse(context.Configuration["RabbitMq:Port"] ?? "5672");
        var rabbitUsername = context.Configuration["RabbitMq:Username"] ?? "guest";
        var rabbitPassword = context.Configuration["RabbitMq:Password"] ?? "guest";

        services.AddSingleton<IConnection>(serviceProvider =>
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitHost,
                Port = rabbitPort,
                UserName = rabbitUsername,
                Password = rabbitPassword
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddTransient<IMailService, SmtpMailService>();
        services.AddHostedService<OtpEmailConsumerWorker>();
    })
    .Build();

await host.RunAsync();
