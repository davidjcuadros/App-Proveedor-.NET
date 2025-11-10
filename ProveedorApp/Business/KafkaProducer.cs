using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using ProveedorApp.IBusiness;

namespace ProveedorApp.Business;

public class KafkaProducer : IKafkaProducer
{
    private readonly IConfiguration _configuration;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        _configuration = configuration;

        var config = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            Console.WriteLine($"Mensaje enviado a {deliveryResult.TopicPartitionOffset}");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Error al enviar mensaje: {e.Error.Reason}");
            throw;
        }
    }
}