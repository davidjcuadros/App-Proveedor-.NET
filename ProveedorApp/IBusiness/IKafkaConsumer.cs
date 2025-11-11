namespace ProveedorApp.IBusiness;

public interface IKafkaConsumer
{
    Task StartConsumingAsync(string topic, CancellationToken cancellationToken);
}