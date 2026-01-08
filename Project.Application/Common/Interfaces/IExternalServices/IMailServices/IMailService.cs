using Project.Application.Common.DTOs.Mails;

namespace Project.Application.Common.Interfaces.IExternalServices.IMailServices
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailDto email, CancellationToken cancellation = default);
    }
}
