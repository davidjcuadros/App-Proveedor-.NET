using System;
using System.Threading.Tasks;

namespace ProveedorApp.IBusiness;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string message);
}