using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;
namespace DOCVIDEO.Utility
{
    public class MailUtility
    {
        #region Send Email

        public static void SendEmail(string fromMailId, string toMailIds, string ccMailIds, string bccMailIds,
           string subject, string body, bool isBodyHtml, string smtpServer, int smtpPort)
        {
            smtpServer = "smtp.emailsrvr.com";
            smtpPort = 25;

            using (MailMessage m = new MailMessage())
            {
                using (AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (body, null, MediaTypeNames.Text.Html))
                {
                    m.AlternateViews.Add(avHtml);

                    m.From = new MailAddress(fromMailId);
                    m.To.Add(new MailAddress(toMailIds));
                    if(!string.IsNullOrEmpty(ccMailIds))
                        m.CC.Add(new MailAddress(ccMailIds));
                     if(!string.IsNullOrEmpty(bccMailIds))
                        m.Bcc.Add(new MailAddress(bccMailIds));
                    m.Subject = subject;
                    m.IsBodyHtml = true;
                    using (SmtpClient client = new SmtpClient(host: smtpServer, port: smtpPort))
                    {
                        client.Credentials = new System.Net.NetworkCredential("anoopriyaranjan@gmail.com", "ranjan@123");
                        //client.EnableSsl = true;
                        try
                        {
                            client.Send(m);
                        }
                        catch (SmtpException)
                        {
                            throw;
                        }
                    }
                }
            }
        }


        #endregion Send Email
    }
}
