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
        string emailFromName = "AlmightyMGA@yandex.ru"; // почта, с которой будет выслан код
        string emailFromPass = "1234Fr"; 

        public object SendConfirmCode(string sendToEmail)
        {
            string code = null; // код подтверждения, который будет выслан на указанный email
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
                client.Port = 587;
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

        /// <summary>
        /// Генерация кода подтверждения определённой длины
        /// </summary>
        /// <param name="codeLenght">Длина кода подтверждения</param>
        /// <returns></returns>
        private string GetRandomCode(int codeLenght=4)
        {
            string rand = ""+Guid.NewGuid().GetHashCode();
            return rand.ToString().Substring(rand.Length-codeLenght);
        }
    }
}
