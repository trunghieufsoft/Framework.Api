using Microsoft.Extensions.Configuration;
using Serilog;
using System.Net.Mail;
using Service.Services.Abstractions;

namespace Service.Services
{
    public class EmailService : BaseService, IEmailService
    {
        private readonly IConfiguration _config;
        private string resetTitle = "Password Reset";
        private string resetTemplate = "Dear {fullName} <br><br><br>Your password has been successfully reset. <br> The new password is: {password} <br>Please change your password after the first log in.<br><br><br>Sincerely,<br>Administrator<br><i>Note: This is an auto-generated email, please do not reply.</i>";
        private string resetEmployeeTitle = "{company}: Your password has been reset";
        private string resetEmployeeTemplate = "Dear {fullName} <br><br><br>Your password has been successfully reset.  <br> The new password is: {password}<br><br><br>Sincerely,<br>{company}<br><i>Note: This is an auto-generated email, please do not reply.</i>";
        private string newTitle = "Your account has been successfully created";
        private string newTemplate = "Dear {fullName} <br><br><br>Your account has been successfully created. Please use detailed information below to log in.<br>Your username is: {username} <br> Your password is: {password} <br><br><br>Sincerely,<br> {company} <br><i>Note: This is an auto-generated email, please do not reply.</i>";
        
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendForgotPassword(string email, string password, string fullname, bool isEmployee, string company = "FPT Company")
        {
            Log.Information("Send Mail reset password to email " + email);
            if (isEmployee)
            {
                SendMail(resetEmployeeTitle, resetEmployeeTemplate.Replace("{password}", password).Replace("{fullName}", fullname), email, null);
            }
            else
            {
                SendMail(resetTitle, resetTemplate.Replace("{password}", password).Replace("{fullName}", fullname), email, null);
            }
        }

        public void SendNewPassword(string email, string password, string fullname, string username, string company = "FPT Company")
        {
            SendMail(newTitle, newTemplate.Replace("{password}", password).Replace("{fullName}", fullname).Replace("{username}", username).Replace("{company}", company), email, null);
        }

        private void SendMail(string title, string content, string email, string fileUlr)
        {
            try
            {
                string sender = _config["mailServer:Email"];
                string password = _config["mailServer:Password"];
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_config["mailServer:Host"]);

                mail.From = new MailAddress(sender);
                mail.To.Add(email);
                mail.Subject = title;
                mail.Body = content;
                mail.IsBodyHtml = true;
                SmtpServer.UseDefaultCredentials = false;

                if (fileUlr != null)
                {
                    Attachment attachment;
                    attachment = new Attachment(fileUlr);
                    mail.Attachments.Add(attachment);
                }
                SmtpServer.Port = int.Parse(_config["mailServer:Post"]);
                SmtpServer.Credentials = new System.Net.NetworkCredential(sender, password);
                SmtpServer.EnableSsl = bool.Parse(_config["mailServer:EnableSsl"]);

                SmtpServer.Send(mail);
            }
            catch (SmtpException e)
            {
                Log.Error("Cant send mail {e}", e);
            }
        }
    }
}