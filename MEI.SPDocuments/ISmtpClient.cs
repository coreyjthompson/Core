using System;
using System.Net.Mail;

namespace MEI.SPDocuments
{
    public interface ISmtpClient
        : IDisposable
    {
        void Send(MailMessage message);
    }

    internal class SmtpClientWrapper
        : ISmtpClient
    {
        private readonly SmtpClient _client;

        public SmtpClientWrapper(string host)
        {
            _client = new SmtpClient(host);
        }

        public void Send(MailMessage message)
        {
            _client.Send(message);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
