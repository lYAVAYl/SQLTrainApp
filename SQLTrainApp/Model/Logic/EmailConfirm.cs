using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SQLTrainApp.Model.Logic
{
    public class EmailConfirm
    {
        string emailFromName = "yavay@pain-nagato.ru";
        string emailFromPass = "1234Fr";

        public object SendConfirmCode(string sendToEmail)
        {
            string code = null;
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailFromName); // Адрес отправителя
                mail.To.Add(new MailAddress(sendToEmail)); // Адрес получателя
                mail.Subject = "Подтверждение Email";
                code = GetRandomCode(5);
                mail.Body = "Код подтверждения: " + code;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.yandex.ru";
                client.Port = 587; // Обратите внимание что порт 587
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(emailFromName, emailFromPass); // Ваши логин и пароль
                client.Send(mail);
            }
            catch(Exception ex)
            {
                return ex;
            }

            return code;
        }

        private string GetRandomCode(int codeLenght=4)
        {
            string rand = ""+Guid.NewGuid().GetHashCode();
            return rand.ToString().Substring(rand.Length-codeLenght);
        }
    }
}
