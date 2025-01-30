using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Windows;

namespace CRM_App
{
    class EmailSender
    {
        public void SendEmail(string subject, string body,string destination)
        {

            string source = "dbonea04@gmail.com";
            string pass = "htqn jzvj vfwu bsqr";

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(source, pass),
                EnableSsl = true,
            };

           MailMessage message = new MailMessage()
           {

               From = new MailAddress(source),
               Subject = subject,
               Body = body,
               IsBodyHtml = true
           };

            message.To.Add(destination);

            var logoPath = @"C:\sigla-KOFALM-160x138.png"; 
            if (System.IO.File.Exists(logoPath)) 
            {
                var logoAttachment = new Attachment(logoPath);
                logoAttachment.ContentId = "sigla"; 
                logoAttachment.ContentDisposition.Inline = true;
                logoAttachment.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                message.Attachments.Add(logoAttachment);
            }
            else
            {
                MessageBox.Show("Imaginea nu a fost gasita! Verificati calea catre fisier.");
                return;
            }


            try
            {
               
                smtpClient.Send(message);
                //MessageBox.Show("Email-ul a fost trimis cu succes!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la trimiterea email-ului: {ex.Message}");
            }
        }
    }
}
