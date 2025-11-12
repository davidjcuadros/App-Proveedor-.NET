using System;

namespace DataTier_Prov.Repositories;

public interface IKafkaConsumer
{
    Task StartConsumingAsync(string topic, CancellationToken cancellationToken);
}