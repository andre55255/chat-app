using System.Net.Mail;

namespace Chat.Communication.ViewObjects.Email
{
    public class SendMailVO
    {
        public List<string> Recipients { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Body { get; set; }
        public AttachmentCollection Attachments { get; set; }
    }

    public class ConfigurationMailVO
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string EmailLogin { get; set; }
        public string EmailPass { get; set; }
    }
}
