using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace WebBanHangOnline.Common
{
    public class Common
    {
        private static readonly string password = ConfigurationManager.AppSettings["PasswordEmail"];
        private static readonly string Email = ConfigurationManager.AppSettings["Email"];

        public static bool SendMail(string name, string subject, string content, string toMail)
        {
            bool rs = false;
            try
            {
                if (!IsValidEmail(toMail)) throw new ArgumentException("Invalid email address.");

                MailMessage message = new MailMessage
                {
                    From = new MailAddress(Email, name),
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                };
                message.To.Add(toMail);

                using (var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(Email, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                })
                {
                    smtp.Send(message);
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("ErrorLog.txt", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                rs = false;
            }
            return rs;
        }

        public static string FormatNumber(object value, int SoSauDauPhay = 2)
        {
            if (!decimal.TryParse(value.ToString(), out decimal GT)) return "0";
            return GT.ToString($"N{SoSauDauPhay}");
        }

        public static string HtmlRate(int Rate)
        {
            var str = "";
            for (int i = 1; i <= 5; i++)
            {
                str += i <= Rate
                    ? "<li><i class='fa fa-star' aria-hidden='true'></i></li>"
                    : "<li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            return str;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
