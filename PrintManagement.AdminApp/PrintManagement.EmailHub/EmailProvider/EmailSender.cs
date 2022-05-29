using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using System.Net;
using System.Net.Mime;
using System.ComponentModel;
using MailKit.Net.Smtp;
using System.IO;
using ContentType = MimeKit.ContentType;
using MimeKit.Utils;
using PrintManagement.EmailHub.Models;
using PrintManagement.EmailHub;

namespace PrintManagement.MailHub.EmailProvider
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message, EmailBody emailBody)
        {
            var emailMessage = CreateEmailMessage(message, emailBody);
            Send(emailMessage);
        }

        public async Task SendEmailAsync(Message message, EmailBody emailBody)
        {
            var mailMessage = CreateEmailMessage(message, emailBody);
            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message, EmailBody emailBody)
        {
            // Create emailMessage --> set From, To, Subject
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To.Select(x => new MailboxAddress(x)));
            emailMessage.Subject = message.Subject;

            // Get image --> create ContentID --> set emailBody.CID = ContentID
            var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var file = Path.Combine(parentDirectory, @"PrintManagement.EmailHub\EmailProvider\banner.png");

            var bodyBuilder = new BodyBuilder();
            var imageHead = bodyBuilder.LinkedResources.Add(file);
            imageHead.ContentId = MimeUtils.GenerateMessageId();

            // Set emailBody.ImageCID
            emailBody.BannerImageCID = imageHead.ContentId;

            // Get htmlBody
            var htmlBody = (new RazorTemplateEngine()).RenderTemplateAsync(emailBody).Result;
            bodyBuilder.HtmlBody = htmlBody;

            // Get attachments
            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            // Set emailMessage.Body --> return MimeMessage object
            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        //private MimeMessage CreateEmailMessage(Message message)
        //{
        //    var emailMessage = new MimeMessage();
        //    emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
        //    emailMessage.To.AddRange(message.To);
        //    emailMessage.Subject = message.Subject;

        //    var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<pre>{0}</pre>", message.Content) };


        //    if (message.Attachments != null && message.Attachments.Any())
        //    {
        //        byte[] fileBytes;
        //        foreach (var attachment in message.Attachments)
        //        {
        //            using (var ms = new MemoryStream())
        //            {
        //                attachment.CopyTo(ms);
        //                fileBytes = ms.ToArray();
        //            }
        //            bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
        //        }
        //    }
        //    emailMessage.Body = bodyBuilder.ToMessageBody();

        //    //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        //    //{ Text = string.Format("<pre>{0}</pre>", message.Content) };
        //    return emailMessage;
        //}

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.PassWord);
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.PassWord);
                    await client.SendAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
