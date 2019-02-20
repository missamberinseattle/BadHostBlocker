using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
using Argosoft.Plugins;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Web;
using System.Net.Mail;

namespace BadHostBlocker
{
    public class BadHostPlugin : ISmtpPlugin
    {
        #region Public Constants
        public const string AcceptedMailBase = "AcceptedMailSummary_";
        public const string RejectedMailBase = "RejectedMailSummary_";
        public const int MaxFileAge = 14;

        public const string OkayToReject = ", okay to reject?";
        #endregion

        #region Private Event Handlers
        private PostProcessDelegate postProcess = PostProcessHandler;
        #endregion

        #region Private Data Members
        private static object _lock = new object();
        private static string _catalogPath;
        private static string _rootPath;

        private static List<IpCatalogItem> _staticCatalog;

        private static List<string> _bannedTLD;
        private static List<string> _targetedAddresses;
        private static List<string> _chameleonHosts;
        private static List<string> _badNames;
        private static List<string> _senderWhiteList;
        private static List<string> _hostWhiteList;
        private static List<string> _captureHosttIgnore;
        private static List<string> _badAttachmentNames;
        private static List<string> _badHeaders;
        private static List<string> _tabooSubjects;
        private static List<string> _questionableBodies;

        private static Dictionary<string, AddressDisplayGrouping> _localSenders;

        private static object _padlock = new object();

        private static Dictionary<string, DateTime> _fileTimestamps = new Dictionary<string, DateTime>();
        private static Dictionary<string, int> _stats = new Dictionary<string, int>();
        private static Dictionary<string, CaptureFileInfo> _captureFiles = new Dictionary<string, CaptureFileInfo>();
        #endregion

        #region Public Properties

        #region Cached Lists
        public static Dictionary<string, AddressDisplayGrouping> LocalSenders
        {
            get
            {
                var path = Path.Combine(RootPath, "ApprovedFromHeaders.txt");

                if (IsNewerFile(path, _localSenders))
                {
                    _localSenders = new Dictionary<string, AddressDisplayGrouping>();
                    var list = LoadList(path);
                    foreach (var line in list)
                    {
                        try
                        {
                            AddressDisplayGrouping address;
                            string failReason;

                            if (!AddressDisplayGrouping.TryParse(line, out address, out failReason))
                            {
                                Logger.Log("", "Could not parse \"" + line + "\" " + failReason);
                            }

                            if (!_localSenders.ContainsKey(address.Address))
                            {
                                _localSenders.Add(address.Address, address);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("", "Could not parse \"" + line + "\" " + ex.Message);

                        }
                    }
                }

                return _localSenders;
            }
        }

        public static List<string> BadAttachmentNames
        {
            get
            {
                var path = Path.Combine(RootPath, "AttachiamentaeNonGratae.txt");

                if (IsNewerFile(path, _badAttachmentNames))
                {
                    _badAttachmentNames = LoadList(path);
                }

                return _badAttachmentNames;
            }
        }

        public static List<string> QuestionableBodyPatterns
        {
            get
            {
                var path = Path.Combine(RootPath, "QuestionableBodies.txt");

                if (IsNewerFile(path, _questionableBodies))
                {
                    _questionableBodies = LoadList(path);
                }

                return _questionableBodies;
            }
        }

        public static List<string> BadHeaders
        {
            get
            {
                var path = Path.Combine(RootPath, "BadHeaders.txt");

                if (IsNewerFile(path, _badHeaders))
                {
                    _badHeaders = LoadList(path);
                }

                return _badHeaders;
            }
        }

        public static List<string> TabooSubjectPatterns
        {
            get
            {
                var path = Path.Combine(RootPath, "TabooSubjects.txt");

                if (IsNewerFile(path, _tabooSubjects))
                {
                    _tabooSubjects = LoadList(path);
                }

                return _tabooSubjects;
            }
        }

        public static List<string> BadNamePatterns
        {
            get
            {
                var path = Path.Combine(RootPath, "BadFromHeaders.txt");

                if (IsNewerFile(path, _badNames))
                {
                    _badNames = LoadList(path);
                }

                return _badNames;
            }
        }

        public static List<string> BannedTLD
        {
            get
            {
                var path = Path.Combine(RootPath, "BannedTLD.txt");

                if (IsNewerFile(path, _bannedTLD))
                {
                    _bannedTLD = LoadList(path);
                }

                return _bannedTLD;
            }
        }

        public static List<string> CaptureHostIgnore
        {
            get
            {
                var path = Path.Combine(RootPath, "CaptureHostIgnore.txt");

                if (IsNewerFile(path, _captureHosttIgnore))
                {
                    _captureHosttIgnore = LoadList(path);
                }

                return _captureHosttIgnore;
            }
        }

        public static List<string> ChameleonHostPatterns
        {
            get
            {
                var path = Path.Combine(RootPath, "ChameleonHosts.txt");

                if (IsNewerFile(path, _chameleonHosts))
                {
                    _chameleonHosts = LoadList(path);
                }

                return _chameleonHosts;
            }
        }

        public static List<string> HostWhiteList
        {
            get
            {
                var path = Path.Combine(RootPath, "HostWhiteList.txt");

                if (IsNewerFile(path, _hostWhiteList))
                {
                    _hostWhiteList = LoadList(path);
                }

                return _hostWhiteList;
            }
        }

        public static List<string> SenderWhiteList
        {
            get
            {
                var path = Path.Combine(RootPath, "SenderWhiteList.txt");

                if (IsNewerFile(path, _senderWhiteList))
                {
                    _senderWhiteList = LoadList(path);
                }

                return _senderWhiteList;
            }
        }

        public static List<string> TargetedAddressPrefixes
        {
            get
            {
                var path = Path.Combine(RootPath, "TargetedPrefixes.txt");

                if (IsNewerFile(path, _targetedAddresses))
                {
                    _targetedAddresses = LoadList(path);
                }

                return _targetedAddresses;
            }
        }

        #endregion

        public static string RootPath
        {
            get
            {
                if (_rootPath == null)
                {
                    var root = Assembly.GetExecutingAssembly().Location;
                    root = Path.GetPathRoot(root);
                    root = Path.Combine(root, "Dropbox\\MailData");

                    if (!Directory.Exists(root))
                    {
                        root = Path.GetPathRoot(Environment.CurrentDirectory);
                        root = Path.Combine(root, "Dropbox\\MailData");
                    }
                    _rootPath = root;
                }

                return _rootPath;
            }
        }

        public static string CatalogPath
        {
            get
            {
                if (_catalogPath == null)
                {
                    _catalogPath = Path.Combine(RootPath, "spam-ip-catalog.xml");
                }

                return _catalogPath;
            }
        }

        #region Interface Implmentation
        public AfterConnectDelegate AfterConnect
        {
            get { return null; }
            set { }
        }

        public AfterMailFromDelegate AfterMailFrom
        {
            get { return null; }
            set { }
        }

        public AfterRcptToDelegate AfterRcptTo
        {
            get { return null; }
            set { }
        }

        public PostProcessDelegate PostProcess
        {
            get { return postProcess; }
            set { }
        }
        #endregion

        public string Description
        {
            get { return "Anti spam plug in that applies anumber of rules to reject spam."; }
        }
        #endregion

        #region PostProcessor Handler
        public static void PostProcessHandler(object sender, string ipAddress, ref string mailFrom, ref List<string> rcptTo, ref StringBuilder msgData, ref PostProcessResult postProcessResult, ref string reply)
        {
            string hostName = null;
            var isHostWhiteList = false;
            var isSenderWhiteList = false;

            var headerFrom = GetHeaderValue("From", msgData);
            var headerTo = GetHeaderValue("To", msgData);
            var headerCc = GetHeaderValue("CC", msgData);
            var headerSubject = GetHeaderValue("Subject", msgData);
            var headerReplyTo = GetHeaderValue("Reply-To", msgData);

            Logger.Log(ipAddress, string.Format("mailFrom: {0}; from: {1}; to: {2}; cc: {3}; Reply-To: {4};", mailFrom, headerFrom, headerTo, headerCc, headerReplyTo));
            AddHeader(msgData, "X-SmtpFrom", mailFrom);
            IncrementStat("All");

            try
            {
                Logger.Log(ipAddress, "Entering CheckHostWhiteList");
                CheckHostWhiteList(mailFrom, ipAddress, ref hostName, ref postProcessResult);
                AddHeader(msgData, "X-HostName", hostName);

                if (postProcessResult == PostProcessResult.Ignore)
                {
                    postProcessResult = PostProcessResult.Accept;
                    IncrementStat(Keys.CheckHostWhiteList);
                    IncrementStat(Keys.WhiteListed);
                    isHostWhiteList = true;
                    AddHeader(msgData, "X-WhiteList-CheckHostWhiteList", "true");
                }

                Logger.Log(ipAddress, "Entering AcceptNoImitations");
                AcceptNoImitations(ipAddress, hostName, headerFrom, ref postProcessResult, ref reply);

                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.AcceptNoImitations);
                    return;
                }

                Logger.Log(ipAddress, "Entering CheckMailFromWhiteList");
                CheckMailFromWhiteList(mailFrom, headerFrom, ipAddress, ref postProcessResult);

                if (postProcessResult == PostProcessResult.Ignore)
                {
                    isSenderWhiteList = true;
                    postProcessResult = PostProcessResult.Accept;
                    IncrementStat(Keys.CheckMailFromWhiteList);
                    AddHeader(msgData, "X-WhiteList-MailFrom", "true");
                    Logger.Log(ipAddress, "Sender " + mailFrom + " is whitelisted");
                }
                else if (postProcessResult == PostProcessResult.Reject)
                {
                    IncrementStat(Keys.MalformedFromHeader);
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.MalformedFromHeader);
                    return;
                }
                Logger.Log(ipAddress, "Entering CheckSmtpSender");
                CheckSmtpSender(ipAddress, mailFrom, ref postProcessResult, ref reply);
                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.CheckSmtpSender);
                    return;
                }

                if (!isHostWhiteList)
                {
                    Logger.Log(ipAddress, "Entering CheckHost");
                    CheckHost(sender, ipAddress, ref postProcessResult, ref reply, ref isHostWhiteList);
                    if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                    {
                        IncrementStat(Keys.CheckHost);
                        if (postProcessResult == PostProcessResult.Ignore)
                        {
                            IncrementStat(Keys.WhiteListed);
                            IncrementStat(Keys.IpWhiteListed);
                            AddHeader(msgData, "X-WhiteList-CheckHost", "true");
                            postProcessResult = PostProcessResult.Accept;
                        }
                        else
                        {
                            DumpMessage(ipAddress, msgData, true);
                            DumpStats();
                            return;
                        }
                    }

                }
                else
                {
                    Logger.Log(ipAddress, "Skipping CheckHost; isHostWhiteList == true");
                }

                if (!isHostWhiteList && !isSenderWhiteList)
                {
                    Logger.Log(ipAddress, "Entering NoNameHostReject");
                    NoNameHostRejector(sender, ipAddress, ref mailFrom, ref rcptTo, ref msgData, ref postProcessResult, ref reply, hostName);

                    if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                    {
                        RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.NoNameHostReject);
                        return;
                    }
                }
                else
                {
                    Logger.Log(ipAddress, "Skipping NoNameHostReject, is white listed");
                }

                Logger.Log(ipAddress, "Entering RejectNonResolvingFromDomains");
                RejectNonResolvingFromDomains(ipAddress, headerFrom, mailFrom, ref postProcessResult, ref reply);
                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.NoNameHostReject);
                    return;
                }

                if (!isSenderWhiteList)
                {
                    Logger.Log(ipAddress, "Entering BannedTldHosts");
                    BannedTldHosts(sender, ipAddress, ref mailFrom, ref rcptTo, ref msgData, ref postProcessResult, ref reply, hostName);

                    if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                    {
                        RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.BannedTldHosts);
                        return;
                    }
                }
                else
                {
                    Logger.Log(ipAddress, "Skipping BannedTldHosts, is white listed");
                }

                if (!isSenderWhiteList)
                {
                    Logger.Log(ipAddress, "Entering BannedTldSenders");
                    BannedTldSenders(sender, ipAddress, ref mailFrom, ref rcptTo, ref msgData, ref postProcessResult, ref reply, hostName);

                    if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                    {
                        RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.BannedTldSenders);
                        return;
                    }
                }
                else
                {
                    Logger.Log(ipAddress, "Skipping BannedTldSenders, is white listed");
                }
                Logger.Log(ipAddress, "Entering TargetedSendLock");
                TargetedSendLock(sender, ipAddress, ref mailFrom, ref rcptTo, ref msgData, ref postProcessResult, ref reply);

                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.TargetedSendLock);
                    return;
                }

                Logger.Log(ipAddress, "Entering ChameleonHostsForbidden");
                ChameleonHostsForbidden(ipAddress, hostName, mailFrom, headerFrom, ref postProcessResult, ref reply);

                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.ChameleonHostsForbidden);
                    return;
                }

                if (!isSenderWhiteList)
                {
                    Logger.Log(ipAddress, "Entering NoPoorlyChosenNames");
                    NoPoorlyChosenNames(sender, ipAddress, headerFrom, msgData, ref postProcessResult, ref reply);
                    if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                    {
                        RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.NoPoorlyChosenNames);
                        return;
                    }
                }
                else
                {
                    Logger.Log(ipAddress, "Skipping NoPoorlyChosenNames, sender is white listed");
                }

                Logger.Log(ipAddress, "Entering RejectSenderSmtpFromEncoding");
                RejectSenderSmtpFromEncoding(sender, ipAddress, headerTo, headerFrom, ref postProcessResult, ref reply, mailFrom);
                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.RejectSenderSmtpFromEncoding);
                    return;
                }

                if (!isSenderWhiteList)
                {
                    Logger.Log(ipAddress, "Entering AttachedTrojans");
                    AttachedTrojans(ipAddress, msgData, ref postProcessResult, ref reply);
                    if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                    {
                        RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.AttachedTrojans);
                        return;
                    }
                }
                else
                {
                    Logger.Log(ipAddress, "Skipping AttachedTrojans, sender it whitelisted");

                }
                Logger.Log(ipAddress, "Entering RejectBadHeaders");
                RejectBadHeaders(ipAddress, msgData, ref postProcessResult, ref reply);
                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.RejectBadHeaders);
                    return;
                }

                Logger.Log(ipAddress, "Entering RejectImposters");
                RejectImposters(ipAddress, headerFrom, mailFrom, ref postProcessResult, ref reply);
                if (postProcessResult == PostProcessResult.Reject)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.RejectImposters);
                    return;
                }

                Logger.Log(ipAddress, "Entering RejectTabooTopics");
                RejectTabooTopics(ipAddress, headerSubject, ref postProcessResult, ref reply);

                if (postProcessResult == PostProcessResult.Reject || postProcessResult == PostProcessResult.Ignore)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.RejectTabooTopics);
                    return;
                }

                Logger.Log(ipAddress, "Entering NoQuestionableBodies");
                NoQuestionableBodies(ipAddress, msgData, ref postProcessResult, ref reply);

                if (postProcessResult == PostProcessResult.Reject)
                {
                    RejectMessage(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData, Keys.NoQuestionableBodies);
                    return;
                }

                AcceptEmail(ipAddress, hostName, headerFrom, mailFrom, headerTo, headerCc, headerSubject, msgData);
            }
            catch (Exception ex)
            {
                Logger.Log(ipAddress, "Exception hit! " + ex.ToString());
                DumpMessage(ipAddress, msgData, true);
                IncrementStat(Keys.Exception);
                postProcessResult = PostProcessResult.Accept;
            }

            DumpStats();
        }



        #region Spam Rule Methods
        /// <summary>
        /// Reject e-mails that are from a suspicious email address and display name combination
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="from"></param>
        /// <param name="mailFrom"></param>
        /// <param name="postProcessResult"></param>
        /// <param name="reply"></param>
        public static void RejectImposters(string ipAddress, string from, string mailFrom, ref PostProcessResult postProcessResult, ref string reply)
        {

            var fromEmail = Utilities.ParseMailAddress(from);

            if (fromEmail == null)
            {
                Logger.Log(ipAddress, "RejectImposters::Could not parse address, \"" + from + "\", rejecting.");
                postProcessResult = PostProcessResult.Reject;
                reply = ReplyTemplates.SuspiciousSender;
                return;
            }

            var isProtectedAddress = LocalSenders.ContainsKey(fromEmail.Address.ToLower()); // If the address is in the dictionary, we care about it
            var isDeplayNameIsProtectedAddress = LocalSenders.ContainsKey(fromEmail.DisplayName.ToLower());
            var isDisplayNameEmpty = string.IsNullOrEmpty(fromEmail.DisplayName);

            if (!isProtectedAddress)
            {
                if (!isDeplayNameIsProtectedAddress)
                {
                    return;
                }

                if (isDisplayNameEmpty)
                {
                    return;
                }
            }

            // If it doesn't have a display name, reject it
            if (string.IsNullOrEmpty(fromEmail.DisplayName))
            {
                Logger.Log(ipAddress, "RejectImposters::Address, \"" + from + "\", doesn't have a display name.");
                postProcessResult = PostProcessResult.Reject;
                reply = ReplyTemplates.SuspiciousSender;
                return;
            }

            // If it has a display name that matches the e-mail address, reject it
            if (fromEmail.DisplayName.ToLower() == fromEmail.Address.ToLower())
            {
                Logger.Log(ipAddress, "RejectImposters::Address, \"" + from + "\", doesn't have a display name.");
                postProcessResult = PostProcessResult.Reject;
                reply = ReplyTemplates.SuspiciousSender;
                return;
            }

            // If the display name looks like an e-mail address we care about 
            // but the address doesn't match, reject it
            if (!LocalSenders.ContainsKey(fromEmail.Address.ToLower()))
            {
                Logger.Log(ipAddress, "RejectImposters::Address, \"" + from + "\", has a " +
                    "display name that matches a local sender's address " +
                    "but the e-mail address is not found");
                postProcessResult = PostProcessResult.Reject;
                reply = ReplyTemplates.SuspiciousSender;
                return;
            }

            var address = LocalSenders[fromEmail.Address];

            foreach (var displayName in address.DisplayNames)
            {
                if (displayName.ToLower() == fromEmail.DisplayName.ToLower())
                {
                    return;
                }

                Logger.Log(ipAddress, "RejectImposters::Doesn't match. " + fromEmail.DisplayName + " != " + displayName);
            }

            postProcessResult = PostProcessResult.Reject;
            reply = ReplyTemplates.SuspiciousSender;
        }

        /// <summary>
        /// Rejects mails from IPs that resolve to the local server name of VMZOOMAIL 
        /// or messages with two e-mail addresses in the From header (ie "amber@clarkzoo.org" &lt;spammer@fuckoff.net&gt;)
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="hostName"></param>
        /// <param name="msgData"></param>
        /// <param name="postProcessResult"></param>
        /// <param name="reply"></param>
        public static void AcceptNoImitations(string ipAddress, string hostName, string from, ref PostProcessResult postProcessResult, ref string reply)
        {

            if (string.IsNullOrEmpty(from))
            {
                postProcessResult = PostProcessResult.Reject;
                reply = "501 Your mail was rejected by the mail server because it lacks a from header and may be a source of spam e-mail.";
                Logger.Log(ipAddress, GenerateRejectionLog("AcceptNoImitations", "From header was null or empty"));
                return;
            }

            if (hostName != null && hostName.StartsWith("vmzoomail", StringComparison.InvariantCultureIgnoreCase) && !ipAddress.StartsWith("70.89.127."))
            {
                postProcessResult = PostProcessResult.Reject;
                reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", did not correctly resolve to a host name and may be a source of spam e-mail.";
                Logger.Log(ipAddress, GenerateRejectionLog("AcceptNoImitations", "IP address improperly resoved", null, ipAddress + " -> " + hostName));
                return;
            }

            // More e-mail patterns: 
            //    "amber@clarkzoo.org" <amber@clarkzoo.org>
            //    <amber@clarkzoo.org>


            var emailPattern = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";

            var matches = Regex.Matches(from, emailPattern);

            if (matches.Count == 2 && matches[0].Value != matches[1].Value)
            {
                postProcessResult = PostProcessResult.Reject;
                reply = "501 Your mail was rejected by the mail server because the From header, " + from + ", appears to contain two different e-mail addresses and may be an attempt to obfuscate the sender or make the e-mail appear as if it came from legettimate source.";
                Logger.Log(ipAddress, GenerateRejectionLog("AcceptNoImitations", "From header contains two e-mail addresses that do not match", null, from));
                return;
            }

            // nm11-vm1.bullet.mail.sg3.yahoo.com   Miss Noleta Voss <ramia1ritha@hotmail.com> 
            // c-50-167-160-44.hsd1.ga.comcast.net  HP Digital Device <HP_Printer@clarkzoo.org> 
        }

        private static string GenerateRejectionLog(string ruleGroup, string reason)
        {
            return GenerateRejectionLog(ruleGroup, reason, null, null);
        }
        private static string GenerateRejectionLog(string ruleGroup, string reason, string rulePattern, string sample)
        {
            var output = new StringBuilder();

            output.Append(ruleGroup);
            output.Append("::Rejected ");
            output.Append(reason);

            if (rulePattern != null)
            {
                output.Append("::Rule ");
                output.Append(rulePattern);
            }

            if (sample != null)
            {
                output.Append("::Sample ");
                output.Append(sample);
            }

            return output.ToString();
        }

        public static void AttachedTrojans(string ipAddress, StringBuilder msgData, ref PostProcessResult result, ref string reply)
        {
            // Content-Type: application/zip; name=LPOMAL504240-DDDFD-8176480.zip

            // Content-Type: application/x-compressed; name=Photos.zip;
            // Content-Disposition: inline;
            // Content-Type: application/x-zip;
            // Content-Disposition: inline;

            var attachmentKeys = new[] { "Content-Disposition: attachment", "Content-Disposition: inline;" };
            const string fileKey = "filename=";

            var msg = msgData.ToString();
            result = PostProcessResult.Accept;

            int keyPos = -10;

            for (var xx = 0; xx < attachmentKeys.Length; xx++)
            {
                keyPos = msg.IndexOf(attachmentKeys[xx]);
                if (keyPos > -1) break;
            }

            // Logger.Log(ipAddress, "AttachedTrojans::keyPos == " + keyPos);
            if (keyPos == -1)
            {
                if (msg.IndexOf(".zip") > -1)
                {
                    Logger.Log(ipAddress, "AttachedTrojans::Whoa! Hidden attachment.");
                    Logger.Log(ipAddress, msg);
                }
                // Logger.Log(ipAddress, "AttachedTrojans::No attachment found (" + attachmentKey + ")");
                return;
            }

            var filePos = msg.IndexOf(fileKey, keyPos);
            if (filePos == -1)
            {
                Logger.Log(ipAddress, "AttachedTrojans::Could not find file key");
                Logger.Log(ipAddress, msg);
                return;
            }

            var eol = msg.IndexOfAny(new[] { '\r', '\n' }, filePos);
            if (eol == -1)
            {
                Logger.Log(ipAddress, "AttachedTrojans::Could not find end of line");
                DumpMessage(ipAddress, msgData, true);
            }

            var start = filePos + fileKey.Length;
            var len = eol - start;

            var fileName = msg.Substring(start, len);
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
            {
                fileName = fileName.Substring(1, fileName.Length - 2);
            }

            foreach (var pattern in BadAttachmentNames)
            {
                var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    Logger.Log(ipAddress, GenerateRejectionLog("AttachedTrojans", "file attatchment matched pattern", pattern, fileName));
                    result = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the server because the attachment could be a possible trojan designed to retrieve data from the intended recipient's system.";
                    return;
                }
            }

            Logger.Log(ipAddress, "AttachedTrojans::Found attachment, \"" + fileName + "\", but failed to match any rules");

        }

        public static void BannedTldHosts(object sender, string ipAddress, ref string mailFrom, ref List<string> rcptTo, ref StringBuilder msgData, ref PostProcessResult postProcessResult, ref string reply, string hostName)
        {
            Logger.Log(ipAddress, "BannedTldHosts::" + ipAddress + " resolved to " + hostName);

            for (var xx = 0; xx < BannedTLD.Count; xx++)
            {
                var bannedTld = BannedTLD[xx];
                if (hostName.EndsWith(bannedTld, StringComparison.InvariantCultureIgnoreCase))
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", resolved to a host name that may be a source of spam e-mail.";
                    Logger.Log(ipAddress, GenerateRejectionLog("BannedTldHosts", "host name ending with banned TLD", bannedTld, hostName));
                    break;
                }
            }
        }

        public static void BannedTldSenders(object sender, string ipAddress, ref string mailFrom, ref List<string> rcptTo, ref StringBuilder msgData, ref PostProcessResult postProcessResult, ref string reply, string hostName)
        {
            Logger.Log(ipAddress, "BannedTldSenders::Check sender " + mailFrom);

            mailFrom = mailFrom.Trim().ToLower();

            var fromAddress = Utilities.ParseMailAddress(mailFrom);
            var replyToAddress = Utilities.ParseMailAddress(reply);

            for (var xx = 0; xx < BannedTLD.Count; xx++)
            {
                var bannedTld = BannedTLD[xx];

                if (mailFrom.EndsWith(bannedTld))
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", resolved to a host name that may be a source of spam e-mail.";
                    Logger.Log(ipAddress, GenerateRejectionLog("BannedTldSenders", "sender with e-mail ending with bannedTLD", bannedTld, mailFrom));
                    break;
                }

                if (fromAddress != null && fromAddress.Address.EndsWith(bannedTld))
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", resolved to a host name that may be a source of spam e-mail.";
                    Logger.Log(ipAddress, GenerateRejectionLog("BannedTldSenders", "sender with e-mail ending with banned TLD", bannedTld, fromAddress.Address));
                    break;
                }

                if (replyToAddress != null && replyToAddress.Address.EndsWith(bannedTld))
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", resolved to a host name that may be a source of spam e-mail.";
                    Logger.Log(ipAddress, GenerateRejectionLog("BannedTldSenders", "reply-to with e-mail ending with banned TLD", bannedTld, replyToAddress.Address));
                    break;
                }

                if (fromAddress != null && fromAddress.Address.EndsWith(bannedTld))
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", resolved to a host name that may be a source of spam e-mail.";
                    Logger.Log(ipAddress, GenerateRejectionLog("BannedTldSenders", "from address with e-mail ending with banned TLD", bannedTld, fromAddress.Address));
                    break;
                }
            }
        }
        #endregion


        public static void ChameleonHostsForbidden(string ipAddress, string hostName, string mailFrom, string from, ref PostProcessResult postProcessResult, ref string reply)
        {
            if (ChameleonHostPatterns.Count == 0)
            {
                Logger.Log(ipAddress, "ChameleonHostPatterns::No patterns are loaded. Well shit.");
            }

            var mailFromHostName = GetHostFromEmailAddress(mailFrom);
            var fromHost = GetHostFromEmailAddress(from);

            var hosts = new Dictionary<string, string>();
            SafeAdd(hosts, hostName, hostName);
            SafeAdd(hosts, mailFrom, mailFromHostName);
            SafeAdd(hosts, from, fromHost);

            var keys = hosts.Keys;

            // Step 1. Walk the list of ChameleonHostPatterns 
            foreach (var key in keys)
            {
                var host = hosts[key];

                if (host == null) continue;

                Logger.Log(ipAddress, "ChameleonHostsForbidden::Testing host (" + host + ") for matching chameleon host patterns");

                foreach (var pattern in ChameleonHostPatterns)
                {
                    var match = Regex.Match(host, pattern);

                    if (match.Success)
                    {
                        postProcessResult = PostProcessResult.Reject;
                        reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                        Logger.Log(ipAddress, GenerateRejectionLog("ChameleonHostsForbidden", "Rejected host for matching chameleon host pattern", pattern, host));
                        return;
                    }
                }
            }

            var output = new StringBuilder();
            var xx = 0;

            foreach (var key in keys)
            {
                xx++;
                if (xx > 1)
                {
                    output.Append(", ");
                }

                if (xx == keys.Count)
                {
                    output.Append("or ");
                }

                output.AppendFormat("\"{1}\" in {0}", key, hosts[key]);
            }

            Logger.Log(ipAddress, "ChameleonHostsForbidden::No match found in " + output.ToString());
        }

        public static void CheckHost(object sender, string ipAddress, ref PostProcessResult reject, ref string reply, ref bool isWhiteList)
        {
            var catalog = GetCatalog();
            var badIp = IpCatalogItem.Find(ipAddress, catalog);

            if (badIp != null && badIp.State != ListState.WhiteList)
            {
                Logger.Log(ipAddress, GenerateRejectionLog("CheckHost", "bad IP", badIp.ToString(), ipAddress));
                reject = PostProcessResult.Reject;
                reply = GetReply(badIp);
            }
            else if (badIp != null && badIp.State == ListState.WhiteList)
            {
                isWhiteList = true;
                Logger.Log(ipAddress, "CheckHost::Message is white listed");
                reject = PostProcessResult.Accept;
            }
            else if (badIp != null)
            {
                reject = PostProcessResult.Ignore;
                Logger.Log(ipAddress, "CheckHost::Allowing white listed address " + ipAddress + " " + badIp.ToString());
            }
            else
            {
                Logger.Log(ipAddress, "CheckHost::" + ipAddress + " was not found in the catalog");
                reject = PostProcessResult.Accept;
            }
        }

        public static void CheckHostWhiteList(string mailFrom, string ipAddress, ref string hostName, ref PostProcessResult postProcessResult)
        {
            try
            {
                var ipOctetText = ipAddress.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                var ipOctetList = new List<byte>();

                foreach (var octet in ipOctetText)
                {
                    ipOctetList.Add(byte.Parse(octet));
                }

                var ip = new IPAddress(ipOctetList.ToArray());

                IPHostEntry host;

                host = Dns.GetHostEntry(ip);
                hostName = host.HostName;

                if (hostName == null)
                {
                    hostName = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ipAddress, "CheckHostWhiteList::" + ex.Message);
                hostName = null;
            }

            if (hostName == null)
            {
                Logger.Log(ipAddress, "CheckHostWhiteList::hostName is null");
                postProcessResult = PostProcessResult.Accept;
                return;
            }

            Logger.Log(ipAddress, "CheckHostWhiteList::" + ipAddress + " resolved to " + hostName);

            foreach (var pattern in HostWhiteList)
            {
                var match = Regex.Match(hostName, pattern);

                if (match.Success)
                {
                    postProcessResult = PostProcessResult.Ignore;
                    Logger.Log(ipAddress, "CheckHostWhiteList::Ignoring hostName (" + hostName + ") for matching hostWhiteList pattern (" + pattern + ")");
                    return;
                }
            }

        }

        public static void CheckMailFromWhiteList(string mailFrom, string fromHeaderValue, string ipAddress, ref PostProcessResult postProcessResult)
        {
            // Step 1: Get list of  WhiteListMailFroms
            var senders = SenderWhiteList;

            // Step 2: Query list
            var from = Utilities.ParseMailAddress(fromHeaderValue);

            if (senders.Contains(mailFrom.ToLower()))
            {
                Logger.Log(ipAddress, "CheckMailFromWhiteList::Found " + mailFrom + " in white list");
                postProcessResult = PostProcessResult.Ignore;
            }
            else if (from != null && senders.Contains(from.Address.ToLower()))
            {
                Logger.Log(ipAddress, "CheckMailFromWhiteList::Found " + from.Address + " in white list");
                postProcessResult = PostProcessResult.Ignore;
            }
            else if (from == null)
            {
                Logger.Log(ipAddress, "CheckMailFromWhiteList::fromHeaderValue (" + fromHeaderValue + ") is not valid; rejecting");
                postProcessResult = PostProcessResult.Reject;
            }
            else
            {
                Logger.Log(ipAddress, "CheckMailFromWhiteList::Neither " + mailFrom + " nor " + from.Address + " is not white-listed");
            }
        }

        public static void CheckSmtpSender(string ipAddress, string mailFrom, ref PostProcessResult postProcessResult, ref string reply)
        {
            if (mailFrom == null)
            {
                Logger.Log(ipAddress, GenerateRejectionLog("CheckSmtpSender", "null mailFrom"));
                postProcessResult = PostProcessResult.Reject;
                reply = "501 Invalid SMTP Sender. Rejecting for possible spam.";
            }
            else if (mailFrom.Trim().Length == 0)
            {
                Logger.Log(ipAddress, GenerateRejectionLog("CheckSmtpSender", "empty mailFrom; length == 0"));
                postProcessResult = PostProcessResult.Reject;
                reply = "501 Invalid SMTP Sender. Rejecting for possible spam.";
            }
        }

        public static void NoNameHostRejector(object sender, string ipAddress, ref string mailFrom, ref List<string> rcptTo, ref StringBuilder msgData, ref PostProcessResult postProcessResult, ref string reply, string hostName)
        {
            postProcessResult = PostProcessResult.Accept;
            reply = null;
            var dumpMessage = false;

            if (ipAddress.StartsWith("10.1.10."))
            {
                Logger.Log(ipAddress, "NoNameHostRejector::White listed " + ipAddress);
                postProcessResult = PostProcessResult.Ignore;
                return;
            }

            try
            {
                if (hostName == null)
                {
                    Logger.Log(ipAddress, GenerateRejectionLog("NoNameHostRejector", "IP address resolves to null", null, ipAddress));
                    dumpMessage = true;
                    reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", did not resolve to a host name and may be a source of spam e-mail.";
                    postProcessResult = PostProcessResult.Reject;
                }
                else if (hostName == ipAddress)
                {
                    Logger.Log(ipAddress, GenerateRejectionLog("NoNameHostRejector", "IP address resolves to IP address", null, ipAddress + " -> " + hostName));
                    dumpMessage = true;
                    reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", resolved to an unnamed IP address and may be a source of spam e-mail.";
                    postProcessResult = PostProcessResult.Reject;
                }
                else
                {
                    Logger.Log(ipAddress, "NoNameHostRejector::Accepted");

                }
            }
            catch (SocketException ex)
            {
                Logger.Log(ipAddress, GenerateRejectionLog("NoNameHostRejector", "socket error resolving to a host name", null, ipAddress + " -> " + ex.Message));
                reply = "501 Your mail was rejected by the mail server because the server's IP address, " + ipAddress + ", failed to resolve to a host name and may be a source of spam e-mail.";
                postProcessResult = PostProcessResult.Reject;
                dumpMessage = true;
            }
            catch (Exception ex)
            {
                Logger.Log(ipAddress, "NoNameHostRejector::Error resolving " + ipAddress + " to a host name");
                Logger.Log(ipAddress, ex.ToString());
            }

            if (dumpMessage)
            {
                // Trying to clean up the log files
                //DumpMessage(ipAddress, msgData, true);
            }
        }

        public static void NoPoorlyChosenNames(object eventSender, string ipAddress, string from, StringBuilder msgData, ref PostProcessResult postProcessResult, ref string reply)
        {
            //Return-Path: <mm-bounce-amber=stopped-motion.com@mailminion.net>
            //From: =?utf-8?Q?HSBC_=C2=A9_intl=2Eebanking=40jpmorgan?= <jp@sequencemultipliervip.com>
            //Reply-To: =?utf-8?Q?HSBC_=C2=A9_intl=2Eebanking=40jpmorgan?= <jp@sequencemultipliervip.com>
            //Sender: mm-bounce-amber=stopped-motion.com@mailminion.net

            // Step 1. Get the list of bad name patterns
            var patterns = BadNamePatterns;

            // Step 2. Get other sender headers
            var header = "From";
            var returnPath = GetHeaderValue("Return-Path", msgData);
            var replyTo = GetHeaderValue("Reply-To", msgData);
            var sender = GetHeaderValue("Sender", msgData);
            Logger.Log(ipAddress, "NoPoorlyChosenNames::from=" + from + "; returnPath=" + returnPath + "; replyTo=" + replyTo + "; sender=" + sender + ";");

            // Step 3. Walk the patterns looking for matches
            foreach (var pattern in patterns)
            {
                try
                {
                    var match = Regex.Match(from, pattern, RegexOptions.IgnoreCase);
                    string headerValue = from;

                    if (!match.Success && returnPath != null)
                    {
                        header = "Return-Path";
                        match = Regex.Match(returnPath, pattern, RegexOptions.IgnoreCase);
                        headerValue = returnPath;
                    }

                    if (!match.Success && replyTo != null)
                    {
                        header = "Reply-To";
                        match = Regex.Match(replyTo, pattern, RegexOptions.IgnoreCase);
                        headerValue = replyTo;
                    }

                    if (!match.Success && sender != null)
                    {
                        header = "Sender";
                        match = Regex.Match(sender, pattern, RegexOptions.IgnoreCase);
                        headerValue = sender;
                    }

                    if (match.Success == true)
                    {
                        postProcessResult = PostProcessResult.Reject;
                        reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                        Logger.Log(ipAddress, GenerateRejectionLog("NoPoorlyChosenNames", "header value matches bad name pattern", pattern, header));
                        return;
                    }
                }
                catch (ArgumentException ex)
                {
                    Logger.Log(ipAddress, "Skipping! Could not parse pattern from BadFromHeaders.txt: " + pattern);
                    Logger.Log(ipAddress, ex.ToString());
                }
            }
        }

        public static void RejectBadHeaders(string ipAddress, StringBuilder msgData, ref PostProcessResult result, ref string reply)
        {
            const string actionTesting = "Testing";
            const string actionActive = "Active";

            // GetHeaderValue("From", msgData)
            foreach (var rule in BadHeaders)
            {
                var commaPos = rule.IndexOf(',');

                if (commaPos <= 0)
                {
                    Logger.Log(ipAddress, "Invalid Bad Header Rule (" + rule + "). Rule use following format: HEADERNAME,{Testing|Active}");
                    continue;
                }

                var header = rule.Substring(0, commaPos);
                var action = rule.Substring(commaPos + 1);

                if (action != actionActive && action != actionTesting)
                {
                    Logger.Log(ipAddress, "Invalid Bad Header Rule (" + rule + ") action ( + ");
                    continue;
                }

                var value = GetHeaderValue(header, msgData);

                if (value == null)
                {
                    continue; // Not found
                }

                if (value.Length == 0)
                {
                    AddHeader(msgData, "X-BadHeaderFound", header);
                    switch (action)
                    {
                        case actionTesting:
                            Logger.Log(ipAddress, "Found bad header (" + header + ") but with no value" + OkayToReject);
                            continue;

                        case actionActive:
                            result = PostProcessResult.Reject;
                            Logger.Log(ipAddress, "Rejected for bad header (" + header + ")");
                            reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress +
                                ") address has been identified as a possible source of spam.";
                            return;
                    }

                }

                AddHeader(msgData, "X-BadHeaderFound", header + " / " + value);
                switch (action)
                {
                    case actionTesting:
                        Logger.Log(ipAddress, "Found bad header (" + header + ") with value (" + value + ")" + OkayToReject);
                        continue;

                    case actionActive:
                        Logger.Log(ipAddress, "Rejected for bad header (" + header + ") with value (" + value + ")" + OkayToReject);
                        result = PostProcessResult.Reject;
                        reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                        return;
                }
            }
        }

        public static void RejectNonResolvingFromDomains(string ipAddress, string from, string mailFrom, ref PostProcessResult postProcessResult, ref string reply)
        {
            // from: "MasterClass" <kisakonusma@seminar-overviews.com>
            // mailFrom: kisakonusma@seminar-overviews.com
            string hostName;
            string hostIp;

            // Start with the mailFrom 
            if (mailFrom.Contains("@"))
            {
                var pos = mailFrom.IndexOf("@");
                if (pos == -1 || pos == mailFrom.Length - 1)
                {
                    Logger.Log(ipAddress, "RejectNonResolvingFromDomains::Cannot determine the hostName from the mailFrom (" + mailFrom + "). Is it okay to reject?");
                    // postProcessResult = PostProcessResult.Reject;
                    // reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                    return;
                }

                hostName = mailFrom.Substring(pos + 1);
                hostIp = GetAddressFromHost(ipAddress, hostName);

                if (hostIp == null)
                {
                    Logger.Log(ipAddress, "RejectNonResolvingFromDomains::Host name (" + hostName + ") from mailFrom (" + mailFrom + ") did not resolve. Is it okay to reject?");
                    // postProcessResult = PostProcessResult.Reject;
                    // reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                    return;
                }

                Logger.Log(ipAddress, "RejectNonResolvingFromDomains::" + hostName + " in " + mailFrom + " resolves to " + hostIp);
            }

            // Reset hostName
            hostName = string.Empty;

            try
            {
                // Move on to the from
                var emailAddress = Utilities.ParseMailAddress(from);

                hostName = emailAddress.Host;
                hostIp = GetAddressFromHost(ipAddress, hostName);

                if (hostIp == null)
                {
                    Logger.Log(ipAddress, "RejectNonResolvingFromDomains::Host name (" + hostName + ") did not resolve in from (" + from + "). Is it okay to reject?");
                    // postProcessResult = PostProcessResult.Reject;
                    // reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                    return;
                }

                Logger.Log(ipAddress, "RejectNonResolvingFromDomains::" + hostName + " in " + from + " resolves to " + hostIp);
            }
            catch (Exception ex)
            {
                Logger.Log(ipAddress, "RejectNonResolvingFromDomains::Resolving hostName (" + hostName + ") in from (" + from + ") failed. Is it okay to reject?");
                Logger.Log(ipAddress, "RejectNonResolvingFromDomains::" + ex.Message);
                // postProcessResult = PostProcessResult.Reject;
                // reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                return;
            }
        }

        public static void RejectSenderSmtpFromEncoding(object sender, string ipAddress, string to, string from, ref PostProcessResult postProcessResult, ref string reply, string mailFrom)
        {
            try
            {
                if (!Regex.IsMatch(mailFrom, ".*-.*=.*@"))
                {
                    Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::Skipping mailFrom " + mailFrom);
                    return;
                }

                //#amber@stopped-motion.com#, #"Fat Crusher System" <crush@ilifegames.com>#, #crush-amber=stopped-motion.com@ilifegames.com#

                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::to == #" + to + "#");
                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::from == #" + from + "#");
                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::mailFrom == #" + mailFrom + "#");

                var toAddress = Utilities.ParseMailAddress(to.ToLower());
                var fromAddress = Utilities.ParseMailAddress(from.ToLower());
                var mailFromAddress = Utilities.ParseMailAddress(mailFrom.ToLower());

                var targetFrom = fromAddress.User + "-" + toAddress.User + "=" + toAddress.Host;

                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::targetFrom == #" + targetFrom + "#");

                if (targetFrom == mailFromAddress.User)
                {
                    reply = "501 Your mail was rejected by the mail server because your mail server's IP (" + ipAddress + ") address has been identified as a possible source of spam.";
                    Logger.Log(ipAddress, GenerateRejectionLog("RejectSenderSmtpFromEncoding", "from found pattern", null, mailFrom));
                    postProcessResult = PostProcessResult.Reject;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::Exception thrown! " + ex.Message);
                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::    to == #" + to + "#");
                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::    from == #" + from + "#");
                Logger.Log(ipAddress, "RejectSenderSmtpFromEncoding::    mailFrom == #" + mailFrom + "#");
                Logger.Log(ipAddress, ex.ToString());

            }
        }

        public static void TargetedSendLock(object sender, string ipAddress, ref string mailFrom, ref List<string> rcptTo, ref StringBuilder msgData, ref PostProcessResult postProcessResult, ref string reply)
        {
            // Step 1. Does the mailFrom match the basic pattern?
            var match = Regex.Match(mailFrom, "-.*?=.*?@");

            if (match == null || !match.Success)
            {
                return;
            }

            // Step 2. Does the mailFrom start with any of these offenders?
            foreach (var prefix in TargetedAddressPrefixes)
            {
                if (mailFrom.StartsWith(prefix))
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because your SMTP sender data (" + mailFrom + ") is suspiciously constructed.";
                    Logger.Log(ipAddress, GenerateRejectionLog("TargetedSendLock", "SMTP Sender matches a suspicious construction", prefix, mailFrom));
                    return;
                }
            }
        }

        public static void RejectMisleadingSubjectLines(string ipAddress, string from, string subject, ref PostProcessResult postProcessResult, ref string reply)
        {
            throw new NotImplementedException();
        }

        public static void RejectTabooTopics(string ipAddress, string subject, ref PostProcessResult postProcessResult, ref string reply)
        {
            if (subject == null)
            {
                Logger.Log(ipAddress, "RejectTabooTopics::Subject is null, skipping");
                return;
            }

            foreach (var pattern in TabooSubjectPatterns)
            {
                // Logger.Log(ipAddress, "RejectTabooTopics::Testing against rule: \"" + pattern + "\"");
                var match = Regex.Match(subject, pattern, RegexOptions.IgnoreCase);

                if (match.Success == true)
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because your subject (" + subject + ") is suspicious.";
                    Logger.Log(ipAddress, GenerateRejectionLog("RejectTabooTopics", "mail with subject for matching pattern",pattern,  subject));
                    return;
                }
            }

            Logger.Log(ipAddress, "RejectTabooTopics::No Matching Rules::Subject \"" + subject + "\"");

        }

        public static void NoQuestionableBodies(string ipAddress, StringBuilder msgData, ref PostProcessResult postProcessResult, ref string reply)
        {
            var body = msgData.ToString();
            var firstEmptyLine = body.IndexOf("\r\n\r\n");
            var buffer = 4;

            if (firstEmptyLine < 0)
            {
                firstEmptyLine = body.IndexOf("\n\n");
                buffer = 2;
            }

            Logger.Log(ipAddress, "Found first empty line at " + firstEmptyLine + " with a buffer of " + buffer);
            body = body.Substring(firstEmptyLine + buffer);

            body = Utilities.Base64Decode(body);

            //QuestionableBodies
            foreach (var pattern in QuestionableBodyPatterns)
            {
                Logger.Log(ipAddress, "NoQuestionableBodies::Testing against rule: \"" + pattern + "\"");
                var match = Regex.Match(body, pattern, RegexOptions.IgnoreCase);

                if (match.Success == true)
                {
                    postProcessResult = PostProcessResult.Reject;
                    reply = "501 Your mail was rejected by the mail server because your message contains suspicious content.";
                    Logger.Log(ipAddress, GenerateRejectionLog("NoQuestionableBodies", "mail with body matching pattern rule", pattern, null));
                    return;
                }
            }

            Logger.Log(ipAddress, "NoQuestionableBodies::\t" + body);
        }
        #endregion

        #region Utility Methods
        public static void AcceptEmail(string ipAddress, string hostName, string from, string mailFrom, string to, string cc, string subject, StringBuilder msgData)
        {

            Logger.Log(ipAddress, "Email accepted");

            DumpMessage(ipAddress, msgData, false);

            Logger.Log(ipAddress, string.Format("TSV\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}", hostName, from, mailFrom, to, cc, subject));
            CaptureSummary(AcceptedMailBase, ipAddress, hostName, from, mailFrom, to, cc, subject, null);
        }

        public static void AddHeader(StringBuilder msgData, string header, string value)
        {
            int lineIndex;
            int charIndex;

            FindHeaderBreak(msgData, out lineIndex, out charIndex);

            var newLine = string.Format("{0}: {1}\r\n", header, value);

            msgData.Insert(charIndex, newLine);
        }

        public static void AppendElement(string name, string text, StringBuilder xml)
        {
            xml.AppendFormat("<{0}>{1}</{0}>", name, HttpUtility.HtmlEncode(text));
            xml.AppendLine();
        }

        public static void CaptureSummary(string outputBase, string ipAddress, string hostName, string from, string mailFrom, string to, string cc, string subject, string fileName)
        {
            if (outputBase == AcceptedMailBase && IsHostCapturable(ipAddress, hostName))
            {
                Logger.Log(ipAddress, "CaptureSummary::Skipping " + outputBase + " capture of " + hostName);
                return;
            }

            var outputPath = GetCaptureFilePath(outputBase);

            try
            {
                var message = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", ipAddress, hostName, from, mailFrom, to, cc, subject, fileName);

                lock (_padlock)
                {
                    using (TextWriter logWriter = new StreamWriter(outputPath, true))
                    {
                        logWriter.WriteLine(message);
                        logWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }


        }

        public static string GetCaptureFilePath(string outputBase)
        {
            if (!_captureFiles.ContainsKey(outputBase) || _captureFiles[outputBase].SetOn < DateTime.Today)
            {
                CaptureFileInfo captureInfo;

                if (!_captureFiles.ContainsKey(outputBase))
                {
                    captureInfo = new CaptureFileInfo();
                    _captureFiles.Add(outputBase, captureInfo);
                }
                else
                {
                    captureInfo = _captureFiles[outputBase];
                }

                captureInfo.SetOn = DateTime.Today;
                captureInfo.Path = Path.Combine(RootPath, outputBase + DateTime.Today.ToString("yyyyMMdd")) + ".tsv";

                Utilities.ClearOldFiles(RootPath, outputBase + "*.tsv", MaxFileAge);
            }

            return _captureFiles[outputBase].Path;
        }

        public static string DumpMessage(string ipAddress, StringBuilder msgData, bool archive)
        {
            var lines = msgData.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None); ;

            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    break;
                }

                Logger.Log(ipAddress, "DumpMessage::        " + line);
            }

            if (!archive)
            {
                return null;
            }

            var path = Path.Combine(Path.GetPathRoot(BadHostPlugin.RootPath), "MessageArchive");
            var fileName = Path.Combine(path, string.Format("{0}--{1}.msg",
                DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss.ffff"),
                ipAddress));

            try
            {
                File.WriteAllLines(fileName, lines);
            }
            catch (Exception ex)
            {
                Logger.Log("Archival", "Could not write message to archive " + fileName);
                Logger.Log("Archival", ex.Message);
            }
            var files = Directory.GetFiles(path, "*.msg");

            foreach (var file in files)
            {
                var info = new FileInfo(file);

                var age = DateTime.Now.Subtract(info.LastWriteTime);

                if (age.TotalDays > MaxFileAge)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Archival", "Could delete old message " + file);
                        Logger.Log("Archival", ex.Message);
                    }
                }
            }

            return fileName;
        }

        public static void DumpStats()
        {
            var totalRejected = 0;
            var totalWhitelisted = 0;

            var rejections = new StringBuilder();
            var total = _stats["All"];

            var allKeys = new List<string>();
            allKeys.AddRange(_stats.Keys);
            allKeys.Sort();

            foreach (var key in allKeys)
            {
                if (key != "All" && !Keys.IsWhiteList(key))
                {
                    totalRejected += _stats[key];

                    if (rejections.Length > 0)
                    {
                        rejections.Append(", ");
                    }

                    rejections.AppendFormat("{0}: {1}", key, _stats[key]);
                }
                else if (key == Keys.WhiteListed)
                {
                    totalWhitelisted += _stats[key];
                }
            }

            Logger.Log("Statistics", string.Format("    Total: {0}; Accepted: {1}; Rejected: {2} ({3})",
                total + totalWhitelisted, total - totalRejected, totalRejected,
                (totalRejected > 0 ? rejections.ToString() : "no rejections yet!")));
        }

        public static void FindHeaderBreak(StringBuilder msgData, out int lineIndex, out int charIndex)
        {
            var lines = ToLines(msgData);
            charIndex = 0;
            lineIndex = -1;

            for (var xx = 0; xx < lines.Length; xx++)
            {
                var line = lines[xx];

                if (line.Length == 0)
                {
                    lineIndex = xx;
                    break;
                }

                charIndex += line.Length + 2;
            }
        }

        public static MatchCollection GetAddresses(string text)
        {
            if (text == null)
            {
                return null;
            }

            const string emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}\b";
            var matches = Regex.Matches(text, emailPattern);

            return matches;
        }

        public static string GetAddressFromHost(string ipAddress, string host)
        {
            string ip = null;

            try
            {
                var ipAddresses = Dns.GetHostAddresses(host);

                if (ipAddresses != null && ipAddresses.Length > 0)
                {
                    ip = ipAddresses[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ipAddress, "Could not resolve " + host + "; " + ex.Message);
            }

            return ip;
        }

        public static List<IpCatalogItem> GetCatalog()
        {
            var fileInfo = new FileInfo(CatalogPath);

            if (_staticCatalog == null)
            {
                Logger.Log("", "_staticCatalog is null, loading from disk");
                _staticCatalog = LoadCatalogFromDisk();
            }
            else if (IsNewerFile(CatalogPath, _staticCatalog))
            {
                Logger.Log("", "_staticCatalog is newer, loading from disk");
                _staticCatalog = LoadCatalogFromDisk();
            }

            return _staticCatalog;
        }

        public static string GetHeaderValue(string header, StringBuilder msgData)
        {
            return GetHeaderValue(header, null, msgData);
        }

        public static string GetHeaderValue(string header, string defaultValue, StringBuilder msgData)
        {
            var lines = ToLines(msgData);
            var value = defaultValue;
            header += ":";

            for (var xx = 0; xx < lines.Length; xx++)
            {
                var line = lines[xx];

                if (line.StartsWith(header, StringComparison.InvariantCultureIgnoreCase))
                {
                    while (xx + 1 < lines.Length && (lines[xx + 1].StartsWith("\t")) || lines[xx + 1].StartsWith(" "))
                    {
                        line += lines[xx + 1];
                        xx++;
                    }

                    value = line.Substring(header.Length).TrimStart();
                    break;
                }

                if (line.Length == 0)
                {
                    break;
                }
            }

            return value;
        }

        public static string GetHostFromEmailAddress(string mailFrom)
        {
            var address = Utilities.ParseMailAddress(mailFrom);
            var result = string.Empty;

            if (address != null)
            {
                result = address.Host;
            }

            return result;
        }

        public static string GetReply(IpCatalogItem badIp)
        {
            return string.Format(ReplyTemplates.BannedIp, badIp.IP);
        }

        public static void IncrementStat(string key)
        {
            if (_stats.ContainsKey(key))
            {
                _stats[key]++;
            }
            else
            {
                _stats.Add(key, 1);
            }
        }

        public static bool IsHostCapturable(string ipAddress, string hostName)
        {
            if (hostName == null)
            {
                Logger.Log(ipAddress, "IsHostCapturable::Capture host, hostName == null");
                return true;
            }

            foreach (var pattern in CaptureHostIgnore)
            {
                try
                {
                    var match = Regex.Match(hostName, "\\A" + pattern + "\\z", RegexOptions.IgnoreCase);

                    if (match.Success == true)
                    {
                        Logger.Log(ipAddress, "IsHostCapturable::Do not capture; hostName (" + hostName + ") matches pattern (" + pattern + ")");
                        return false;
                    }
                }
                catch (ArgumentException ex)
                {
                    Logger.Log(ipAddress, "IsHostCapturable::Could not parse RegEx pattern: " + pattern);
                    Logger.Log(ipAddress, "IsHostCapturable::Exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.Log(ipAddress, "IsHostCapturable::Could not test value. " + ex.Message);
                }
            }

            return true;
        }

        public static bool IsNewerFile(string path, object collection)
        {
            var fileInfo = new FileInfo(path);

            lock (_lock)
            {

                if (collection == null)
                {
                    Logger.Log("IsNewerFile", "Collection is null, returning true");
                    if (!_fileTimestamps.ContainsKey(path))
                    {
                        _fileTimestamps.Add(path, fileInfo.LastWriteTime);
                    }
                    else
                    {
                        _fileTimestamps[path] = fileInfo.LastWriteTime;
                    }
                    return true;

                }

                if (!_fileTimestamps.ContainsKey(path))
                {
                    Logger.Log("IsNewerFile", "_fileTimeStamps does not contain path (" + path + "), returning true");
                    _fileTimestamps.Add(path, fileInfo.LastWriteTime);
                    return true;
                }
            }

            var compareTo = _fileTimestamps[path].CompareTo(fileInfo.LastWriteTime);
            var result = (compareTo < 0);

            if (result)
            {
                _fileTimestamps[path] = fileInfo.LastWriteTime;
                Logger.Log("IsFileNewer", string.Format("_fileTimestamps[path].CompareTo(fileInfo.LastWriteTime) " +
                    "(({0}).CompareTo({1}) == {2}, returning {3}", _fileTimestamps[path], fileInfo.LastWriteTime,
                    compareTo, result));
            }

            return result;
        }

        public static List<IpCatalogItem> LoadCatalogFromDisk()
        {
            _staticCatalog = IpCatalogItem.LoadCatalog(CatalogPath);
            return _staticCatalog;
        }

        public static List<string> LoadList(string path)
        {
            var caller = new StackTrace().GetFrame(1).GetMethod().Name;
            var logPrefix = "LoadList::" + caller;

            var list = new List<string>();
            Logger.Log(logPrefix, "Loading the list from " + Path.GetFileName(path));
            var lines = File.ReadAllLines(path);

            for (var xx = 0; xx < lines.Length; xx++)
            {
                var line = lines[xx];

                if (string.IsNullOrEmpty(line))
                {
                    continue; // Skip this line
                }

                line = line.Trim();
                var comment = string.Empty;

                // If the rule is commented out, skip it
                if (line.StartsWith("#"))
                {
                    Logger.Log(logPrefix, "Skipped disabled rule " + line);
                    continue;
                }

                // If the rule includes a comment, remove the comment
                if (line.Contains("#"))
                {
                    var pos = line.IndexOf('#');
                    if (pos < line.Length - 1)
                    {
                        comment = "    // " + line.Substring(pos + 1).Trim();
                    }
                    line = line.Substring(0, pos).Trim();
                }

                Logger.Log(logPrefix, "Added line " + line + comment);
                list.Add(line);
            }

            return list;
        }

        public static string MsgToXml(StringBuilder msgData)
        {
            var xml = new StringBuilder();

            //Received: from [67.229.234.233] by mail.clarkzoo.org (ArGoSoft Mail Server .NET v.1.0.8.6) with ESMTP (EHLO fircia.us)
            //    for <gina@clarkzoo.org>; Thu, 26 Feb 2015 00:34:57 -0800
            //Subject: Be a part of the 1% with private jet rental
            //From: "Private Jets" <webmaster@crossroads.fircia.us>
            //To: <gina@clarkzoo.org>
            //Date: Thu, 26 Feb 2015 00:09:33 -0800
            //List-Unsubscribe: <mailto:unsubscribe@fircia.us>
            //Content-Type: multipart/related; boundary="c7f10b86c051a1ee_c4758e4aafe1d437a"
            //MIME-Version: 1.0
            //Message-ID: <0.0.0.30.1D0519B92C0EF6C.117153@fircia.us>
            //SPF-Received: pass
            //X-FromIP: 67.229.234.233
            //X-HostName: mx233.fircia.us

            xml.Append("<message>");
            AppendElement("host", GetHeaderValue("X-HostName", string.Empty, msgData), xml);
            AppendElement("ip", GetHeaderValue("X-FromIP", string.Empty, msgData), xml);
            AppendElement("from", GetHeaderValue("From", string.Empty, msgData), xml);
            AppendElement("to", GetHeaderValue("To", string.Empty, msgData), xml);
            AppendElement("cc", GetHeaderValue("CC", string.Empty, msgData), xml);
            AppendElement("subject", GetHeaderValue("Subject", string.Empty, msgData), xml);
            AppendElement("file", "[FILEPATH]", xml);
            xml.Append("</message>");

            return xml.ToString();
        }

        public static void RejectMessage(string ipAddress, string hostName, string from, string mailFrom, string to, string cc, string subject, StringBuilder msgData, string incrementKey)
        {
            RejectMessage(ipAddress, hostName, from, mailFrom, to, cc, subject, msgData, new string[] { incrementKey });
        }

        public static void RejectMessage(string ipAddress, string hostName, string from, string mailFrom, string to, string cc, string subject, StringBuilder msgData, string[] incrementKeys)
        {
            var fileName = DumpMessage(ipAddress, msgData, true);
            foreach (var key in incrementKeys)
            {
                IncrementStat(key);
            }
            DumpStats();
            CaptureSummary(RejectedMailBase, ipAddress, hostName, from, mailFrom, to, cc, subject, fileName);
        }

        public static void SafeAdd(Dictionary<string, string> dictionary, string key, string value)
        {
            var keyIncrment = 0;
            var newKey = key;

            if (newKey == null)
            {
                newKey = string.Empty;
            }

            while (dictionary.ContainsKey(newKey))
            {
                keyIncrment++;
                newKey = key + keyIncrment.ToString();
            }

            dictionary.Add(newKey, value);
        }

        public static string[] ToLines(StringBuilder msgData)
        {
            return msgData.ToString().Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.None);
        }

        #endregion
    }
}
