using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using WEB.Models;

namespace WEB
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string bodyText, string bodyHtml = null, bool isErrorEmail = false, List<Attachment> attachments = null, bool textOnly = false);
    }

    public class EmailSender : IEmailSender
    {
        private readonly Settings _settings;

        public EmailSender(Settings settings)
        {
            _settings = settings;
        }

        public Task SendEmailAsync(string toEmail, string toName, string subject, string bodyText, string bodyHtml = null, bool isErrorEmail = false, List<Attachment> attachments = null, bool textOnly = false)
        {
            if (!isErrorEmail && !_settings.EmailSettings.SendEmails)
                return Task.CompletedTask;
            if (isErrorEmail && !_settings.EmailSettings.SendErrorEmails)
                return Task.CompletedTask;

            var html = bodyText;
            if (!textOnly)
            {
                html = System.IO.File.ReadAllText(System.IO.Path.Join(_settings.RootPath, "wwwroot/templates/email.html"));
                html = html.Replace("{rootUrl}", _settings.RootUrl);
                html = html.Replace("{title}", subject);
                if (bodyHtml == null) bodyHtml = bodyText;
                while (bodyHtml.IndexOf(Environment.NewLine + Environment.NewLine) >= 0)
                    bodyHtml = bodyHtml.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                var lines = "<p>" + string.Join("</p><p>", bodyHtml.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)) + "</p>";
                html = html.Replace("{body}", lines);
            }


            using (var smtp = new SmtpClient(_settings.EmailSettings.SMTP, _settings.EmailSettings.SMTPPort))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_settings.EmailSettings.UserName, _settings.EmailSettings.Password);
                smtp.EnableSsl = _settings.EmailSettings.SSL;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_settings.EmailSettings.Sender, _settings.EmailSettings.SenderName);
                    var to = new MailAddress(string.IsNullOrWhiteSpace(_settings.EmailSettings.SubstitutionEmailAddress) ? toEmail : _settings.EmailSettings.SubstitutionEmailAddress, toName);
                    mailMessage.To.Add(to);
                    mailMessage.Subject = subject;
                    mailMessage.Body = bodyText;

                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, null, MediaTypeNames.Text.Plain));
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                    if (attachments != null)
                        foreach (var attachment in attachments)
                            mailMessage.Attachments.Add(attachment);

                    smtp.Send(mailMessage);
                }
            }

            return Task.CompletedTask;
        }
    }

}
