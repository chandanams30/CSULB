using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;

namespace ThoughtFocus.Common.Utilities.Implementation
{
    public class SendMail:ISendMail
    {
        private readonly IConfiguration _configuration;
        public ILogger<SendMail> _logger;
        public SendMail(IConfiguration configuration, ILogger<SendMail> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        #region fields
        private const int DerivationIterations = 1000;

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;
        #endregion
        public void SendEmail(string userEmail, string cc, string subject, string body,string attachmentBody)
        {
            var fromUserName = _configuration["EmailNotifications:EmailUserName"];
            var fromUserPassword = _configuration["EmailNotifications:EmailPassword"];
            var fromEmail = _configuration["EmailNotifications:FromAddress"];
            var smtpAddress = _configuration["EmailNotifications:SMTPSever"];
            var portNumber = _configuration["EmailNotifications:PortNumber"];
            bool enableSSL = Convert.ToBoolean(_configuration["EmailNotifications:enableSSL"]);
            string emailTo = userEmail;

            using (MailMessage mail = new MailMessage())
            {
                mail.IsBodyHtml = true;
                //create Alrternative HTML view
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                string imagePath=Path.GetFullPath("SupportFiles/Img/logo.jpeg");
                //Add Image
                LinkedResource theEmailImage = new LinkedResource(imagePath, MediaTypeNames.Image.Jpeg);
                theEmailImage.ContentId = "myImageID";

                //Add the Image to the Alternate view
                htmlView.LinkedResources.Add(theEmailImage);

                //Add view to the Email Message
                mail.AlternateViews.Add(htmlView);

                mail.From = new MailAddress(fromEmail);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                //mail.Body = body;
                if (!String.IsNullOrEmpty(cc))
                {
                    mail.CC.Add(cc);
                }
                if (!String.IsNullOrEmpty(attachmentBody))
                {
                    byte[] file = getAttachmentContent(attachmentBody);
                    Stream stream = new MemoryStream(file);
                    mail.Attachments.Add(new Attachment(stream, "TB/CTC_Approval.pdf"));
                }


                // Can set to false, if you are sending pure text. 
                try
                {
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, Convert.ToInt32(portNumber)))
                    {
                        if (!String.IsNullOrEmpty(fromUserName) || !String.IsNullOrEmpty(fromUserPassword))
                        {
                            smtp.Credentials = new NetworkCredential(fromUserName, fromUserPassword);
                        }
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);

                    }
                }
                catch(Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex.Message + " " + userEmail + " " + subject);
                }
            }
        }
        public void SendEmail(string userEmail, string cc, string subject, string body, byte[] attachment)
        {
            var fromUserName = _configuration["EmailNotifications:EmailUserName"];
            var fromUserPassword = _configuration["EmailNotifications:EmailPassword"];
            var fromEmail = _configuration["EmailNotifications:FromAddress"];
            var smtpAddress = _configuration["EmailNotifications:SMTPSever"];
            var portNumber = _configuration["EmailNotifications:PortNumber"];
            bool enableSSL = Convert.ToBoolean(_configuration["EmailNotifications:enableSSL"]);
            string emailTo = userEmail;

            using (MailMessage mail = new MailMessage())
            {
                mail.IsBodyHtml = true;
                //create Alrternative HTML view
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                string imagePath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
                //Add Image
                LinkedResource theEmailImage = new LinkedResource(imagePath, MediaTypeNames.Image.Jpeg);
                theEmailImage.ContentId = "myImageID";

                //Add the Image to the Alternate view
                htmlView.LinkedResources.Add(theEmailImage);

                //Add view to the Email Message
                mail.AlternateViews.Add(htmlView);

                mail.From = new MailAddress(fromEmail);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                //mail.Body = body;
                if (!String.IsNullOrEmpty(cc))
                {
                    mail.CC.Add(cc);
                }
                if (attachment!=null)
                {
                   // byte[] file = getAttachmentContent(attachmentBody);
                    Stream stream = new MemoryStream(attachment);
                    mail.Attachments.Add(new Attachment(stream, "Recommendation_Template.pdf"));
                }


                // Can set to false, if you are sending pure text. 
                try
                {
                    using (SmtpClient smtp = new SmtpClient(smtpAddress, Convert.ToInt32(portNumber)))
                    {
                        if (!String.IsNullOrEmpty(fromUserName) || !String.IsNullOrEmpty(fromUserPassword))
                        {
                            smtp.Credentials = new NetworkCredential(fromUserName, fromUserPassword);
                        }
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);

                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex.Message + " " + userEmail + " " + subject);
                }
            }
        }
        private byte[] getAttachmentContent(string attachmentBody)
        {
            byte[] content = null;
            System.IO.MemoryStream returnStream = new System.IO.MemoryStream();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                var attachment = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(attachmentBody, PdfSharp.PageSize.A4);

                //int _emptyNum = 4;
                int _cnt = attachment.PageCount;
                for (int i = 0; i < _cnt; i++)
                {
                    if (attachment.Pages[i].Elements.Count == 5)
                    {
                        attachment.Pages.RemoveAt(i);
                        _cnt--;
                    }
                }
                attachment.Save(ms);
                content = ms.ToArray();
                
            }

            return content;
        }

        private string Decrypt(string cipherText, string securityKey)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(securityKey, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                var engine = new RijndaelEngine(256);
                var blockCipher = new CbcBlockCipher(engine);
                var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
                var keyParam = new KeyParameter(keyBytes);
                var keyParamWithIV = new ParametersWithIV(keyParam, ivStringBytes, 0, 32);

                cipher.Init(false, keyParamWithIV);
                var comparisonBytes = new byte[cipher.GetOutputSize(cipherTextBytes.Length)];
                var length = cipher.ProcessBytes(cipherTextBytes, comparisonBytes, 0);

                cipher.DoFinal(comparisonBytes, length);

                var nullIndex = comparisonBytes.Length - 1;
                while (comparisonBytes[nullIndex] == (byte)0)
                    nullIndex--;
                comparisonBytes = comparisonBytes.Take(nullIndex + 1).ToArray();


                var result = Encoding.UTF8.GetString(comparisonBytes, 0, comparisonBytes.Length);

                return result;
            }

        }
        public string Encrypt(string plainText, string securityKey)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(securityKey, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                var engine = new RijndaelEngine(256);
                var blockCipher = new CbcBlockCipher(engine);
                var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
                var keyParam = new KeyParameter(keyBytes);
                //KeyParameter keyParam = new KeyParameter(Convert.FromBase64String(keyBytes));
                var keyParamWithIV = new ParametersWithIV(keyParam, ivStringBytes, 0, 32);

                cipher.Init(true, keyParamWithIV);
                var comparisonBytes = new byte[cipher.GetOutputSize(plainTextBytes.Length)];
                var length = cipher.ProcessBytes(plainTextBytes, comparisonBytes, 0);

                cipher.DoFinal(comparisonBytes, length);

                var cipherTextBytes = saltStringBytes;
                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                cipherTextBytes = cipherTextBytes.Concat(comparisonBytes).ToArray();
                return Convert.ToBase64String(cipherTextBytes);


               
            }
        }
        private byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }
    }
}
