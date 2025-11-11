using System.Threading.Tasks;

namespace DataTier_Prov.Repositories
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string message);
    }
}
