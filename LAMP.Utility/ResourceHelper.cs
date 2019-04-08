using System;
using System.Configuration;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace LAMP.Utility
{
    /// <summary>
    /// ResourceHelper Class
    /// </summary>
    public class ResourceHelper
    {
        /// <summary>
        /// Get resource message by code
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns>Resource message</returns>
        public static string GetStringResource(int code)
        {
            const string messagePrefix = "Err_";
            string retValue = LAMPMessages.ResourceManager.GetString(messagePrefix + code);
            return retValue;
        }

        /// <summary>
        /// Get resource message by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Resource message</returns>
        public static string GetStringResource(string key)
        {
            string retValue = LAMPMessages.ResourceManager.GetString(key);
            return retValue;
        }

        /// <summary>
        /// Generate unique alphanumeric key having length within the specified limit.
        /// </summary>
        /// <param name="maxSize">Maximum character length.</param>        
        /// <returns>UniqueAlphaNumericCode</returns>
        public static string GenerateUniqueAlphaNumericCode(int MaxSize)
        {
            char[] chars = new char[62];
            string a;
            a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size = MaxSize;
            byte[] data = new byte[size];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }
        

        /// <summary>
        /// Send Email Using SMTP
        /// </summary>
        /// <param name="mailTo">To Address</param>
        /// <param name="mailSubject">Subject</param>
        /// <param name="mailBody">Body</param>
        public static void SendEmailUsingSMTP(string mailTo, string mailSubject, string mailBody)
        {
            try
            {
                var mailFrom = ConfigurationManager.AppSettings["mailFrom"].ToString();
                var host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                MailMessage message = new MailMessage(mailFrom, mailTo);
                message.Subject = mailSubject;
                message.Body = mailBody;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;

                SmtpClient client = new SmtpClient(host);
                //client.EnableSsl = true;
                client.Send(message);
                // Clean up.
                message.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                LogUtil.Error(ex);
                throw;
            }
        }
    }
}
