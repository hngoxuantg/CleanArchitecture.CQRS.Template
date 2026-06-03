using Project.Application.Common.DTOs.Emails;

namespace Project.Application.Common.Interfaces.IExternalServices.IMailServices
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailDto email, CancellationToken cancellation = default);
    }
}
