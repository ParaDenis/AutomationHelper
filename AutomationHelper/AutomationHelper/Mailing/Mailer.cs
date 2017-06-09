using ActiveUp.Net.Mail;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using Message = ActiveUp.Net.Mail.Message;

namespace AutomationHelper.Mailing
{
    public class Mailer
    {
        private Imap4Client client;

        public Mailer(string server, string address, string password)
        {
            _server = server;
            _address = address;
            _password = password;
        }

        private string _server { get; set; }
        private string _address { get; set; }
        private string _password { get; set; }
        public IEnumerable<Message> GetAllMails(string mailBox)
        {
            return GetMails(mailBox, "ALL").Cast<Message>();
        }

        public IEnumerable<Message> GetUnreadMails(string mailBox)
        {
            var messages = GetMails(mailBox, "UNSEEN");
            //if(messages.Count == 0) r
            return messages.Cast<Message>();
            //return GetUnreadMails(mailBox);
        }

        protected Imap4Client Client
        {
            get
            {
                if (client != null) return client;
                client = new Imap4Client();
                client.ConnectSsl(_server);
                client.Login(_address, _password);
                return client;
            }
        }

        public MessageCollection GetMails(string mailBox, string searchPhrase)
        {
            Mailbox mails = Client.SelectMailbox(mailBox);
            MessageCollection messages = mails.SearchParse(searchPhrase);
            return messages;
        }

        public Mailbox GetMailbox(string mailBox)
        {
            return Client.SelectMailbox(mailBox);
        }

        /// <summary>
        /// Method can be used to send an e-mail
        /// </summary>
        /// <param name="body">The contents of the message</param>
        /// <param name="address">the e-mail address of the sender</param>
        /// <param name="password">the password of the sender</param>
        /// <param name="server">sender's server</param>
        /// <param name="subject">The subject of the e-mail</param>
        /// <param name="attachment">Files to be attached</param>
        /// <param name="emailsSentTo">The emails of the recievers</param>
        public void SendMailSmtp(string body, string subject,
            Attachment attachment,
            string[] emailsSentTo)
        {
            var loginInfo = new NetworkCredential(_address, _password);
            var msg = new MailMessage { From = new MailAddress(_address) };
            foreach (var s in emailsSentTo)
                msg.To.Add(new MailAddress(s));
            msg.Subject = subject;
            msg.Body = body;

            msg.Attachments.Add(attachment);
            var smtpClient = new System.Net.Mail.SmtpClient(_server);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(msg);
            attachment.Dispose();
            msg.Dispose();
            smtpClient.Dispose();
        }
        /// <summary>
        /// Method captures the screenshot and stores it as png file
        /// </summary>
        /// <param name="screenshotFileName"></param>
        public void CaptureScreen(string screenshotFileName)
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var graph = Graphics.FromImage(bmp);
            graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            bmp.Save(screenshotFileName, ImageFormat.Png);
            bmp.Dispose();
            graph.Dispose();
        }
    }
}
