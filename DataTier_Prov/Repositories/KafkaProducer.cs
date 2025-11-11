using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace DataTier_Prov.Repositories
{
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
                var result = await _producer.ProduceAsync(
                    topic, 
                    new Message<Null, string> { Value = message }
                );

                Console.WriteLine($"Mensaje enviado a {result.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"Error enviando mensaje: {ex.Error.Reason}");
                throw;
            }
        }
    }
}
