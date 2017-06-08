using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Mail;

namespace AutomationHelper.Mailing
{
    public class Mailer
    {
        private Imap4Client client;
        public Mailer(string server, string address, string password)
        {
            Client.ConnectSsl(server);
            Client.Login(address, password);
        }

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
            get { return client ?? (client = new Imap4Client()); }
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
    }
}
