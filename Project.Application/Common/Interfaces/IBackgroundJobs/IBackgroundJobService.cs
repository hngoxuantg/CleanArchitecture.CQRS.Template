using Project.Application.Common.DTOs.Emails;

namespace Project.Application.Common.Interfaces.IBackgroundJobs
{
    public interface IBackgroundJobService
    {
        void EnqueueSendEmail(EmailDto email);
    }
}
