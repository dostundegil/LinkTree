using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LinkTree
{
    public class EmailSender:IEmailSender
    {
        //MESAJ YOLLAYICI
        public int SendEmailAsync(string receiverAddress, string subject, string htmlBody)
        {

			//AWS Credentials ATAMALARI
			string host = "your hostname";
            int port = 587;
            string smtpUsername = "your username";
            string smptpPassword = "your pw";

            //CLIENT'IN OLUŞTURULMASI
            SmtpClient client = new SmtpClient(host, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(smtpUsername, smptpPassword);
            client.EnableSsl = true;

            //MAİL ADRESİ ATAMALARI
            MailAddress fromAddress = new MailAddress("sender", "sender", System.Text.Encoding.UTF8);
            MailAddress toAddress = new MailAddress(receiverAddress, receiverAddress, System.Text.Encoding.UTF8);
            MailMessage msg= new MailMessage(fromAddress,toAddress);

            //MESAJ ATAMALARI
            msg.Body = htmlBody;
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.Subject = subject;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            client.Send(msg);
            return 1;
        }
    }
}
