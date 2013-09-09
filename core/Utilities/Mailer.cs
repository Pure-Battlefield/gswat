using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace core.Utilities
{
    public interface IMailer
    {
        void SendMail(string recipient, string subject, string body);
    }
    public class Mailer : IMailer
    {
        private readonly ICloudSettingsManager _settingsManager;

        public Mailer(ICloudSettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void SendMail(string recipient, string subject, string body)
        {
            var host = _settingsManager.GetConfigurationSettingValue("SmtpHost");
            var account = _settingsManager.GetConfigurationSettingValue("SmtpAccount");
            var password = _settingsManager.GetConfigurationSettingValue("SmtpPassword");
            var sender = _settingsManager.GetConfigurationSettingValue("SmtpSender");
            var port = int.Parse(_settingsManager.GetConfigurationSettingValue("SmtpPort"));
            var smtpUseSsl = bool.Parse(_settingsManager.GetConfigurationSettingValue("SmtpUseSSL"));

            var message = new MailMessage(sender, recipient)
                              {
                                  Subject = subject, 
                                  Body = body
                              };

            var smtpClient = new SmtpClient(host, port)
                                 {
                                     EnableSsl = smtpUseSsl,
                                     Credentials = new NetworkCredential(account, password)
                                 };

            smtpClient.Send(message);
        }
    }
}
