using System;
using System.Collections.Generic;
using System.Text;

namespace BadHostBlocker
{
    public static class ReplyTemplates
    {
        public const string BannedIp = "501 The IP address of your mail server, {0}, has been banned by the server administrator.";
        public const string SuspiciousSender = "501 The message has been rejected because the sender appears suspicious.";
    }
}
