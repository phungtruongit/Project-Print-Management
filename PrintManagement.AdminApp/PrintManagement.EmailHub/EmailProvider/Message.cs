using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.MailHub.EmailProvider
{
    public class Message
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public IFormFileCollection Attachments { get; set; }
        public Message(IEnumerable<string> to, string subject, string content, IFormFileCollection attachments)
        {
            To = new List<string>(to);
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }

        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<string>(to);
            Subject = subject;
            Content = content;
            Attachments = null;
        }
    }
}
