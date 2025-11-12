using System;
using System.Text.Json;
using Confluent.Kafka;
using DataTier_Prov.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataTier_Prov.Mail;

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
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_configuration["Kafka:Topic"]);

        _logger.LogInformation("Kafka consumer started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);

                if (consumeResult.Message != null)
                {
                    _logger.LogInformation($"Received message: {consumeResult.Message.Value}");

                    // Assuming the message is a JSON with email details
                    var emailData = JsonSerializer.Deserialize<EmailData>(consumeResult.Message.Value);

                    if (emailData != null)
                    {
                        await _emailService.SendEmailAsync(emailData.To, emailData.Subject, emailData.Body);
                        _logger.LogInformation("Email sent successfully.");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Kafka consumer.");
        }
    }

    public Task StartConsumingAsync(string topic, CancellationToken cancellationToken)
    {
        // This method is not needed since it's a background service, but kept for interface
        return Task.CompletedTask;
    }

    private class EmailData
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}