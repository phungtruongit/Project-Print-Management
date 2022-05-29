using PrintManagement.EmailHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.MailHub.EmailProvider
{
    public interface IEmailSender
    {
        void SendEmail(Message message, EmailBody emailBody);
        Task SendEmailAsync(Message message, EmailBody emailBody);
    }
}
