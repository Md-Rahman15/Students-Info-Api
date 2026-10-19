
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.Service.Interfaces;

namespace Students_Info_Api.Service.EmailServiceFolder
{
    public class EmailService : IEmailService
    {
        // Stores SMTP settings from appsettings.json
        private readonly EmailSMTP emailSmtp;

        // Used to log email sending errors
        private readonly ILogger<EmailService> logger;

        public EmailService(
            IOptions<EmailSMTP> options,
            ILogger<EmailService> logger)
        {
            this.emailSmtp = options.Value;
            this.logger = logger;
        }

        // Sends the token to the user's email.
        // Used for password reset and email verification.
        public async Task<bool> SendTokenLink(
            User_DM user,
            string resetlink,
            string emailaddress)
        {
            // Sender details from SMTP settings
            MailboxAddress SenderMailBoxAddress =
                new MailboxAddress(
                    emailSmtp.SenderName,
                    emailSmtp.SenderEmail
                );

            // Receiver details
            MailboxAddress receiverMailboxAddress =
                new MailboxAddress(
                    user.UserName,
                    emailaddress
                );

            // Email content
            BodyBuilder bodyBuilder = new BodyBuilder()
            {
                TextBody = resetlink,
            };

            MimeMessage mimeMessage = new MimeMessage();

            mimeMessage.From.Add(SenderMailBoxAddress);
            mimeMessage.To.Add(receiverMailboxAddress);
            mimeMessage.Subject = "Token Reset Link";
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    // Connect to the SMTP server using TLS
                    await smtpClient.ConnectAsync(
                        emailSmtp.SMTP_Host,
                        emailSmtp.SMTP_Port,
                        SecureSocketOptions.StartTls
                    );

                    // Log in to the email account
                    await smtpClient.AuthenticateAsync(
                        emailSmtp.UserName,
                        emailSmtp.Password
                    );

                    // Send the email
                    await smtpClient.SendAsync(mimeMessage);

                    await smtpClient.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the error and let the caller handle the failure
                logger.LogError(ex, "Failed to send token email");
                return false;
            }
        }
    }
}