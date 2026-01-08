using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Project.Application.Common.DTOs.Mails;
using Project.Application.Common.Interfaces.IExternalServices.IMailServices;
using Project.Common.Options;

namespace Project.Infrastructure.ExternalServices.MailServices
{
    public class MailService : IMailService
    {
        private readonly EmailSettings _emailSettings;

        public MailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendEmailAsync(EmailDto email, CancellationToken cancellation = default)
        {
            string mailContent = string.Empty;

            if (email.TemplateName == "CategoryCreated")
            {
                mailContent = GetCategoryCreatedHtml(email.TemplateData);
            }
            else
            {
                mailContent = email.Body ?? string.Empty;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.From));
            message.To.Add(MailboxAddress.Parse(email.To));
            message.Subject = email.Subject;

            var builder = new BodyBuilder { HtmlBody = mailContent };

            if (email.Attachment != null)
            {
                using Stream stream = email.Attachment.OpenReadStream();
                builder.Attachments.Add(email.Attachment.FileName, stream);
            }

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls, cancellation);
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.Password, cancellation);
            await smtp.SendAsync(message, cancellation);
            await smtp.DisconnectAsync(true, cancellation);
        }

        private string GetCategoryCreatedHtml(Dictionary<string, string>? data)
        {
            string html = @"
            <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 20px auto; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05);'>
                <div style='background-color: #fb8500; padding: 30px; text-align: center;'>
                    <h1 style='color: white; margin: 0; font-size: 24px; text-transform: uppercase; letter-spacing: 1px;'>New Category Created</h1>
                </div>
                
                <div style='padding: 30px; background-color: #ffffff;'>
                    <p style='font-size: 16px; color: #333;'>Hello <strong>{{AdminName}}</strong>,</p>
                    <p style='font-size: 15px; color: #555; line-height: 1.6;'>
                        This is an automated notification from <strong>WebAPI_Project</strong>. A new category has been successfully added to our platform.
                    </p>
                    
                    <div style='margin: 25px 0; padding: 20px; background-color: #f8f9fa; border-left: 4px solid #fb8500; border-radius: 4px;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='padding: 8px 0; color: #777; font-size: 14px;'>Category Name:</td>
                                <td style='padding: 8px 0; color: #333; font-weight: bold; font-size: 15px;'>{{CategoryName}}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px 0; color: #777; font-size: 14px;'>Created Date:</td>
                                <td style='padding: 8px 0; color: #333; font-size: 15px;'>{{CreatedDate}}</td>
                            </tr>
                        </table>
                    </div>

                    <p style='font-size: 15px; color: #555;'>You can now start managing products within this category through the Admin Dashboard.</p>
                    
                    <div style='text-align: center; margin-top: 30px;'>
                        <a href='#' style='background-color: #023047; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 14px;'>Go to Dashboard</a>
                    </div>
                </div>

                <div style='background-color: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #888;'>
                    <p style='margin: 0;'>&copy; 2026 WebAPI_Project. Developed by Ngô Xuân Hải.</p>
                    <p style='margin: 5px 0 0 0;'>Ho Chi Minh City, Vietnam</p>
                </div>
            </div>";

            return ReplacePlaceholders(html, data);
        }

        private string ReplacePlaceholders(string html, Dictionary<string, string>? data)
        {
            if (data == null) return html;

            foreach (var item in data)
            {
                html = html.Replace($"{{{{{item.Key}}}}}", item.Value);
            }
            return html;
        }
    }
}