using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Common.Utilities.Interfaces
{
    public interface ISendMail
    {
        void SendEmail(string userEmail, string cc, string subject, string body, string attachmentBody);
        void SendEmail(string userEmail, string cc, string subject, string body, byte[] attachment);
    }
}
