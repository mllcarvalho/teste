using Oficina.Common.Domain.Entities;

namespace Oficina.Common.Domain.IHelper
{
    public interface IEmailHelper
    {
        Task EnviarEmailAsync(EmailMessage emailMessage);
    }
}
