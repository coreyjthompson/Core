using System.Net.Mail;

using Microsoft.Extensions.Options;

namespace MEI.SPDocuments
{
    public interface IEmailer
    {
        void SendEmail(string subject, string body, string[] toAddresses, bool isBodyHtml = false);
    }

    internal class Emailer
        : IEmailer
    {
        private readonly SPDocumentsOptions _options;

        public Emailer(IOptions<SPDocumentsOptions> options)
        {
            _options = options.Value;
        }

        public void SendEmail(string subject, string body, string[] toAddresses, bool isBodyHtml = false)
        {
            using (var message = new MailMessage())
            {
                foreach (string emailAddress in toAddresses)
                {
                    message.To.Add(emailAddress);
                }

                message.IsBodyHtml = isBodyHtml;
                message.Subject = subject;
                message.Body = body;
                message.From = new MailAddress("support@meintl.com", "MEI_SP_Documents");

                using (var client = new SmtpClient(_options.SmtpServer))
                {
                    client.Send(message);
                }
            }
        }
    }
}
