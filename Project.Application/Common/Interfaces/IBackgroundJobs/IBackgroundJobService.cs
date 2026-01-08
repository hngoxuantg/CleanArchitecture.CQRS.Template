using Project.Application.Common.DTOs.Mails;

namespace Project.Application.Common.Interfaces.IBackgroundJobs
{
    public interface IBackgroundJobService
    {
        void EnqueueSendEmail(EmailDto email);
    }
}
