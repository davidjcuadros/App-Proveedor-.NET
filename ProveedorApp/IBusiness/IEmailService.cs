using System.Threading.Tasks;

namespace ProveedorApp.IBusiness;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}