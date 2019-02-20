using System;
using System.Collections.Generic;
using System.Text;

namespace BadHostBlocker
{
    public static class Keys
    {
        public static readonly string WhiteListed = "WhiteListed";
        public static readonly string Exception = "Exception";
        public static readonly string CheckHostWhiteList = "CheckHostWhiteList";
        public static readonly string CheckMailFromWhiteList = "CheckMailFromWhiteList";
        public static readonly string CheckHost = "CheckHost";
        public static readonly string NoNameHostReject = "NoNameHostReject";
        public static readonly string TargetedSendLock = "TargetedSendLock";
        public static readonly string BannedTldSenders = "BannedTldSenders";
        public static readonly string AcceptNoImitations = "AcceptNoImitations";
        public static readonly string ChameleonHostsForbidden = "ChameleonHostsForbidden";
        public static readonly string NoPoorlyChosenNames = "NoPoorlyChosenNames";
        public static readonly string IpWhiteListed = "IpWhiteListed";
        public static readonly string BannedTldHosts = "BannedTldHosts";
        public static readonly string RejectSenderSmtpFromEncoding = "RejectSenderSmtpFromEncoding";
        public static readonly string AttachedTrojans = "AttachedTrojans";
        public static readonly string CheckSmtpSender = "CheckSmtpSender";
        public static readonly string RejectBadHeaders = "RejectBadHeaders";
        public static readonly string RejectImposters = "RejectImposters";
        public static readonly string RejectTabooTopics = "RejectTabooTopics";
        public static readonly string NoQuestionableBodies = "NoQuestionableBodies";
        public static readonly string CheckBannedHosts = "CheckBannedHosts";
        public static readonly string MalformedFromHeader = "MalformedFromHeader";

        internal static bool IsWhiteList(string key)
        {
            if (key == null) return false;

            return key.Contains("WhiteList");
        }



    }
}
