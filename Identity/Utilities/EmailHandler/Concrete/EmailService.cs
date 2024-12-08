using Identity.Utilities.EmailHandler.Abstract;
using Identity.Utilities.EmailHandler.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Identity.Utilities.EmailHandler.Concrete
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public void SendMessage(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfiguration.Username, _emailConfiguration.Username));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfiguration.Username, _emailConfiguration.Password);
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while sending the email.", ex);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }
    }
}
