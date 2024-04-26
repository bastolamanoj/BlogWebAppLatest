using BlogWebApp.ViewModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            //using var message = new MailMessage();
            //message.From = new MailAddress(_emailSettings.MailUsername); // Set the From address
            //message.To.Add(email);
            //message.Subject = subject;
            //AlternateView htmlView =
            //AlternateView.CreateAlternateViewFromString(htmlMessage, Encoding.UTF8, "text/html");
            //message.AlternateViews.Add(htmlView); // And a html attachment to make sure.
            //message.Body = htmlMessage;
            //message.IsBodyHtml = true;

            //using var client = new SmtpClient(_emailSettings.MailServer, _emailSettings.MailPort)
            //{
            //    Credentials = new NetworkCredential(_emailSettings.MailUsername, _emailSettings.MailPassword),
            //    EnableSsl = true
            //};

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_emailSettings.MailUsername, _emailSettings.MailPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.MailUsername),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true, // Set to true if you're sending HTML content
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

            //await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Handle and log the exception
            // Example: logger.LogError(ex, "Failed to send email");
            throw; // Rethrow the exception if necessary
        }

        //await client.SendMailAsync(message);
    }
}
