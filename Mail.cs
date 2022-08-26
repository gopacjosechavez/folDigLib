using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;


namespace folDigLib
{
    public class Mail
    {
        public Mail() { }
        public bool sendMail(AlternateView avHtml = null)
        {
            bool result = false;
            try
            {
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(this.From);
                mailMessage.To.Add(this.To);
                mailMessage.Subject = this.Subject;
                mailMessage.Body = this.Body;
                mailMessage.IsBodyHtml = true;

                if (this.Bcc != null && this.Bcc.Count > 0)
                {
                    foreach (var item in this.Bcc)
                    {
                        mailMessage.Bcc.Add(item);
                    }

                }
                if (this.CC != null && this.CC.Count > 0)
                {
                    foreach (var item in this.CC)
                    {
                        mailMessage.CC.Add(item);
                    }

                }

                //DGG 20200220
                if (avHtml != null)
                    mailMessage.AlternateViews.Add(avHtml);

                SmtpClient smtpClient = new SmtpClient(this.SMTP);

                smtpClient.Port = int.Parse(this.Port);

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new System.Net.NetworkCredential(this.User, this.Password);
                smtpClient.EnableSsl = this.SSL;

                // Check for attachments
                if(Attachments.Count > 0)
                //if (HasAttachments)
                {
                    // The attachments array should point to a file location where the attachment resides
                    // and add the attachments to the message
                    foreach (string attach in Attachments)
                    {
                        Attachment attached = new Attachment(attach, MediaTypeNames.Application.Octet);
                        mailMessage.Attachments.Add(attached);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                smtpClient.Send(mailMessage);
                result = true;
            }
            catch (Exception ex)
            {
                this.StatusMessage = ex.ToString();
            }
            return result;
        }


        public void AddAttachment(string attachmentPath)
        {
            if (Attachments == null)
            {
                Attachments = new List<string>();
            }
            Attachments.Add(attachmentPath);
            HasAttachments = true;
        }

        public string getStatusMessage()
        {
            return this.StatusMessage;
        }

        public Mail(
            string mailTo,
            string subject,
            string SMTPfrom,
            string SMTPuser,
            string SMTPpassword,
            string SMTPport,
            string SMTP,
            string body = "",
            bool SMTPssl = false,
            List<string> Bcc = null,
            List<string> CC = null
        )
        {
            this.To = mailTo;
            this.Subject = subject;
            this.From = SMTPfrom;
            this.User = SMTPuser;
            this.Password = SMTPpassword;
            this.Port = SMTPport;
            this.SMTP = SMTP;

            this.Body = body;
            this.SSL = SMTPssl;

            if (Bcc != null)
                this.Bcc = Bcc;

            if (CC != null)
                this.CC = CC;
        }

        public static bool ValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return false;
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public string Port { get; set; }
        public string SMTP { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public List<string> Bcc { get; set; }
        public List<string> CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Attachments { get; set; }
        public bool SSL { get; set; }

        private bool HasAttachments = false;
        private string StatusMessage;

    }
}
