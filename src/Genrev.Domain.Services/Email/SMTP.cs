using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Mail;

namespace Genrev.DomainServices.Email
{
    public class SMTP
    {



        public static void Send(Domain.Email.SMTPAccount account, string subject, string message, string[] recipients)
        {

            foreach(string recipient in recipients)
            {
                Send(account, subject, message, recipient);
            }

        }


        public static void Send(Domain.Email.SMTPAccount account, string subject, string message, string recipient)
        {

            MailMessage mail = new MailMessage(account.FromAddress, recipient);
            SmtpClient client = new SmtpClient();
            client.Port = account.Port;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = account.UseSSL;
            client.Host = account.Server;
            client.Credentials = new System.Net.NetworkCredential(account.Username, account.Password);

            mail.Subject = subject;
            mail.Body = message;

            client.Send(mail);

        }



    }
}
