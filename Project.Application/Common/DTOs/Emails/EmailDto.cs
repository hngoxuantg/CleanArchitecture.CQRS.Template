using Microsoft.AspNetCore.Http;

namespace Project.Application.Common.DTOs.Mails
{
    public class EmailDto
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string? TemplateName { get; set; }

        public IFormFile? Attachment { get; set; }

        public string? Body { get; set; }

        public Dictionary<string, string>? TemplateData { get; set; }
    }
}
