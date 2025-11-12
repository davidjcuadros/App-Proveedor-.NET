using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using DataTier_Prov.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataTier_Prov.Mail
{
    public class KafkaConsumer : BackgroundService, IKafkaConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly ILogger<KafkaConsumer> _logger;

        public KafkaConsumer(IConfiguration configuration, EmailService emailService, ILogger<KafkaConsumer> logger)
        {
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "email-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        var topic = _configuration["Kafka:Topic"];
        _logger.LogInformation("✅ KafkaConsumer inicializando. Suscrito al topic '{Topic}'.", topic);

        await Task.Run(() =>
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("📩 Mensaje recibido desde Kafka: {Value}", consumeResult.Message.Value);

                        try
                        {
                            var emailData = JsonSerializer.Deserialize<EmailData>(consumeResult.Message.Value);
                            var to = emailData?.To ?? _configuration["Email:NotificationTo"];
                            var subject = emailData?.Subject ?? "Notificación desde ProveedorApp";
                            var body = emailData?.Body ?? consumeResult.Message.Value;

                            _emailService.SendEmailAsync(to, subject, body).GetAwaiter().GetResult();
                            _logger.LogInformation("✅ Correo enviado a {To}", to);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ Error al procesar mensaje Kafka.");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("🛑 KafkaConsumer detenido correctamente.");
            }
            finally
            {
                consumer.Close();
            }
        }, stoppingToken);
    }

        public Task StartConsumingAsync(string topic, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private class EmailData
        {
            public string? To { get; set; }
            public string? Subject { get; set; }
            public string? Body { get; set; }
        }
    }
}
