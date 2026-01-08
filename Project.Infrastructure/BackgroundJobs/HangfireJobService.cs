using Hangfire;
using Project.Application.Common.DTOs.Mails;
using Project.Application.Common.Interfaces.IBackgroundJobs;
using Project.Application.Common.Interfaces.IExternalServices.IMailServices;

namespace Project.Infrastructure.BackgroundJobs
{
    public class HangfireJobService : IBackgroundJobService
    {
        public void EnqueueSendEmail(EmailDto email)
        {
            BackgroundJob.Enqueue<IMailService>(
                job => job.SendEmailAsync(email));
        }
    }
}
