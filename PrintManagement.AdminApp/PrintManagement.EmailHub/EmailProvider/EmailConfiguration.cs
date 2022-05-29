using System;
using System.Collections.Generic;
using System.Text;

namespace PrintManagement.MailHub.EmailProvider
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
