using GezginTravel.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace GezginTravel.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public SmtpEmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendMailAsync(string to, string subject, string htmlBody, string? plainTextBody = null)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _emailSettings.SenderName,
                _emailSettings.SenderEmail
                ));

            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = plainTextBody ?? "Bu e-postayı görüntülemek için HTML destekleyen bir mail istemcisi kullanınız."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
               _emailSettings.SmtpHost,
               _emailSettings.SmtpPort,
               _emailSettings.EnableSsl
                   ? SecureSocketOptions.StartTls
                   : SecureSocketOptions.None
            );

            await smtpClient.AuthenticateAsync(
                _emailSettings.SenderEmail,
                _emailSettings.Password
            );
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
