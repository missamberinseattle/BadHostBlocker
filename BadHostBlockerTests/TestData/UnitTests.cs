using Argosoft.Plugins;
using BadHostBlocker;
using NUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BadHostBlockerTests
{
    [TestFixture]
    public class UnitTests : AssertionHelper
    {
        public static readonly string[] GoodIps = { "192.168.2.8", "94.236.240.140" };
        public static readonly string[] BadIps = { "100.43.131.55", "173.44.252.10", "198.23.213.7", "23.239.133.177", "67.209.149.85", "84.200.228.122", "98.126.156.73", "98.143.148.95" };
        public static readonly string TestIp = "12.34.567.89";

        //[Test]
        //public void EndToEnd()
        //{
        //    var result = PostProcessResult.Accept;
        //    var sender = new object();
        //    var ip = GoodIps[0];
        //    var mailFrom = "";
        //    var rcptTo = new List<string>();
        //    var msgData = new StringBuilder();
        //    string reply = null;

        //    BadHostPlugin.PostProcessHandler(sender, ip, ref mailFrom, ref rcptTo, ref msgData, ref result, ref reply);
        //}

        [Test]
        public void LoadLocalSenders()
        {
            foreach (var adGroup in BadHostPlugin.LocalSenders)
            {
                Logger.Log(adGroup.ToString());
            }
        }

        [Test]
        public void CheckSenderWhiteList()
        {
            var sender="support@boldtypetickets.com";
            var result = PostProcessResult.Accept;

            BadHostPlugin.CheckMailFromWhiteList(sender, "0.0.0.0", ref result);
            
        }
        [Test]
        public void DecodeBase64_LineBreaks()
        {
            var body = "PCFET0NUWVBFIEhUTUwgUFVCTElDICItLy9XM0MvL0RURCBIVE1MIDQuMCBUcmFuc2l0aW9uYWwv\r\nL0VOIj4NCjxIVE1MPjxIRUFEPg0KPE1FVEEgY29udGVudD0idGV4dC9odG1sOyBjaGFyc2V0PWdi\r\nMjMxMiIgaHR0cC1lcXVpdj1Db250ZW50LVR5cGU+DQo8TUVUQSBuYW1lPUdFTkVSQVRPUiBjb250\r\nZW50PSJNU0hUTUwgOC4wMC42MDAxLjE4NzAyIj48L0hFQUQ+DQo8Qk9EWT4NCjxQIGFsaWduPWNl\r\nbnRlcj48Rk9OVCBzaXplPTYgDQpmYWNlPURlZmF1bHQ+PFNUUk9ORz7R18jVK8nZuL49ye3QxL3i\r\nt8U8L1NUUk9ORz48L0ZPTlQ+PC9QPg0KPFA+PEEgaHJlZj0iaHR0cDovLzAzNzV0dWFuLmNvbS8i\r\nPjxTVFJPTkc+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjogZGFya2N5YW4iIGNvbG9y\r\nPXB1cnBsZSANCnNpemU9Nj5odHRwOi8vMDM3NXR1YW4uY29tLzwvRk9OVD48L1NUUk9ORz48L0E+\r\nPC9QPg0KPFA+Jm5ic3A7PC9QPg0KPFA+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjog\r\neWVsbG93Ij43NjbR18jVK8nZuL49ye3QxL3it8XPws7nIDA4OjM2OjU0PC9GT05UPjwvUD48L0JP\r\nRFk+PC9IVE1MPg0K";

            var result = Utilities.Base64Decode(body);

            Expect(body != result, "body == result, whoops!");
        }

        [Test]
        public void DecodeBase64_OneLine()
        {
            var body = "PCFET0NUWVBFIEhUTUwgUFVCTElDICItLy9XM0MvL0RURCBIVE1MIDQuMCBUcmFuc2l0aW9uYWwvL0VOIj4NCjxIVE1MPjxIRUFEPg0KPE1FVEEgY29udGVudD0idGV4dC9odG1sOyBjaGFyc2V0PWdiMjMxMiIgaHR0cC1lcXVpdj1Db250ZW50LVR5cGU+DQo8TUVUQSBuYW1lPUdFTkVSQVRPUiBjb250ZW50PSJNU0hUTUwgOC4wMC42MDAxLjE4NzAyIj48L0hFQUQ+DQo8Qk9EWT4NCjxQIGFsaWduPWNlbnRlcj48Rk9OVCBzaXplPTYgDQpmYWNlPURlZmF1bHQ+PFNUUk9ORz7R18jVK8nZuL49ye3QxL3it8U8L1NUUk9ORz48L0ZPTlQ+PC9QPg0KPFA+PEEgaHJlZj0iaHR0cDovLzAzNzV0dWFuLmNvbS8iPjxTVFJPTkc+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjogZGFya2N5YW4iIGNvbG9yPXB1cnBsZSANCnNpemU9Nj5odHRwOi8vMDM3NXR1YW4uY29tLzwvRk9OVD48L1NUUk9ORz48L0E+PC9QPg0KPFA+Jm5ic3A7PC9QPg0KPFA+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjogeWVsbG93Ij43NjbR18jVK8nZuL49ye3QxL3it8XPws7nIDA4OjM2OjU0PC9GT05UPjwvUD48L0JPRFk+PC9IVE1MPg0K";

            var result = Utilities.Base64Decode(body);

            Expect(body != result, "body == result, whoops!");
        }

        [Test]
        public void DecodeBase64_NotEncoded()
        {
            var body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n<HTML><HEAD>\r\n<META content=\"text/html; charset=gb2312\" http-equiv=Content-Type>\r\n<META name=GENERATOR content=\"MSHTML 8.00.6001.18702\"></HEAD>\r\n<BODY>\r\n<P align=center><FONT size=6 \r\nface=Default><STRONG>+ٸ=Ľ</STRONG></FONT></P>\r\n<P><A href=\"http://0375tuan.com/\"><STRONG><FONT \r\nstyle=\"BACKGROUND-COLOR: darkcyan\" color=purple \r\nsize=6>http://0375tuan.com/</FONT></STRONG></A></P>\r\n<P>&nbsp;</P>\r\n<P><FONT \r\nstyle=\"BACKGROUND-COLOR: yellow\">766+ٸ=Ľ 08:36:54</FONT></P></BODY></HTML>\r\n";

            var result = Utilities.Base64Decode(body);

            Expect(body == result, "body != result, whoops!");
        }

        [Test]
        public void IsBase64String_LineBreaks()
        {
            var body = "PCFET0NUWVBFIEhUTUwgUFVCTElDICItLy9XM0MvL0RURCBIVE1MIDQuMCBUcmFuc2l0aW9uYWwv\r\nL0VOIj4NCjxIVE1MPjxIRUFEPg0KPE1FVEEgY29udGVudD0idGV4dC9odG1sOyBjaGFyc2V0PWdi\r\nMjMxMiIgaHR0cC1lcXVpdj1Db250ZW50LVR5cGU+DQo8TUVUQSBuYW1lPUdFTkVSQVRPUiBjb250\r\nZW50PSJNU0hUTUwgOC4wMC42MDAxLjE4NzAyIj48L0hFQUQ+DQo8Qk9EWT4NCjxQIGFsaWduPWNl\r\nbnRlcj48Rk9OVCBzaXplPTYgDQpmYWNlPURlZmF1bHQ+PFNUUk9ORz7R18jVK8nZuL49ye3QxL3i\r\nt8U8L1NUUk9ORz48L0ZPTlQ+PC9QPg0KPFA+PEEgaHJlZj0iaHR0cDovLzAzNzV0dWFuLmNvbS8i\r\nPjxTVFJPTkc+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjogZGFya2N5YW4iIGNvbG9y\r\nPXB1cnBsZSANCnNpemU9Nj5odHRwOi8vMDM3NXR1YW4uY29tLzwvRk9OVD48L1NUUk9ORz48L0E+\r\nPC9QPg0KPFA+Jm5ic3A7PC9QPg0KPFA+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjog\r\neWVsbG93Ij43NjbR18jVK8nZuL49ye3QxL3it8XPws7nIDA4OjM2OjU0PC9GT05UPjwvUD48L0JP\r\nRFk+PC9IVE1MPg0K";

            var isB64 = Utilities.IsBase64String(body);
            Expect(isB64, "body detection failed");
        }

        [Test]
        public void IsBase64String_Html()
        {
            var body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n<HTML><HEAD>\r\n<META content=\"text/html; charset=gb2312\" http-equiv=Content-Type>\r\n<META name=GENERATOR content=\"MSHTML 8.00.6001.18702\"></HEAD>\r\n<BODY>\r\n<P align=center><FONT size=6 \r\nface=Default><STRONG>+ٸ=Ľ</STRONG></FONT></P>\r\n<P><A href=\"http://0375tuan.com/\"><STRONG><FONT \r\nstyle=\"BACKGROUND-COLOR: darkcyan\" color=purple \r\nsize=6>http://0375tuan.com/</FONT></STRONG></A></P>\r\n<P>&nbsp;</P>\r\n<P><FONT \r\nstyle=\"BACKGROUND-COLOR: yellow\">766+ٸ=Ľ 08:36:54</FONT></P></BODY></HTML>\r\n";

            var isB64 = Utilities.IsBase64String(body);
            Expect(!isB64, "body false positive");
        }

        [Test]
        public void IsBase64String_OneLine()
        {
            var body = "PCFET0NUWVBFIEhUTUwgUFVCTElDICItLy9XM0MvL0RURCBIVE1MIDQuMCBUcmFuc2l0aW9uYWwvL0VOIj4NCjxIVE1MPjxIRUFEPg0KPE1FVEEgY29udGVudD0idGV4dC9odG1sOyBjaGFyc2V0PWdiMjMxMiIgaHR0cC1lcXVpdj1Db250ZW50LVR5cGU+DQo8TUVUQSBuYW1lPUdFTkVSQVRPUiBjb250ZW50PSJNU0hUTUwgOC4wMC42MDAxLjE4NzAyIj48L0hFQUQ+DQo8Qk9EWT4NCjxQIGFsaWduPWNlbnRlcj48Rk9OVCBzaXplPTYgDQpmYWNlPURlZmF1bHQ+PFNUUk9ORz7R18jVK8nZuL49ye3QxL3it8U8L1NUUk9ORz48L0ZPTlQ+PC9QPg0KPFA+PEEgaHJlZj0iaHR0cDovLzAzNzV0dWFuLmNvbS8iPjxTVFJPTkc+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjogZGFya2N5YW4iIGNvbG9yPXB1cnBsZSANCnNpemU9Nj5odHRwOi8vMDM3NXR1YW4uY29tLzwvRk9OVD48L1NUUk9ORz48L0E+PC9QPg0KPFA+Jm5ic3A7PC9QPg0KPFA+PEZPTlQgDQpzdHlsZT0iQkFDS0dST1VORC1DT0xPUjogeWVsbG93Ij43NjbR18jVK8nZuL49ye3QxL3it8XPws7nIDA4OjM2OjU0PC9GT05UPjwvUD48L0JPRFk+PC9IVE1MPg0K";

            var isB64 = Utilities.IsBase64String(body);
            Expect(isB64, "body detection failed");
        }

        [Test]
        public void IsHostCapturable()
        {
            #region Test Data
            var data = new[] { new { hostName = "216.107.9.12.nni.net", capturable = true }, 
                new { hostName = "35.Red-88-6-218.staticIP.rima-tde.net", capturable = true }, 
                new { hostName = "66-220-144-141.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-143.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-146.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-148.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-149.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-151.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-152.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-152.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-153.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-154.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-156.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-158.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-158.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-164.outcampmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-144-173.outappmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-135.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-135.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-136.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-136.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-137.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-140.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-143.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-143.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-144.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-144.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-144.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-144.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-147.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-147.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-147.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-148.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-150.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-151.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-151.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-154.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-154.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-155.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-155.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-155.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-156.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-156.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-158.outmail.facebook.com", capturable = false }, 
                new { hostName = "66-220-155-158.outmail.facebook.com", capturable = false }, 
                new { hostName = "BSN-77-80-25.static.siol.net", capturable = true }, 
                new { hostName = "ccm235.constantcontact.com", capturable = false }, 
                new { hostName = "h1684609.stratoserver.net", capturable = true }, 
                new { hostName = "joystickrequired.com", capturable = true }, 
                new { hostName = "mail05.fetlifemail.com", capturable = false }, 
                new { hostName = "mail05.fetlifemail.com", capturable = false }, 
                new { hostName = "mail05.fetlifemail.com", capturable = false }, 
                new { hostName = "mail05.fetlifemail.com", capturable = false }, 
                new { hostName = "mail05.fetlifemail.com", capturable = false }, 
                new { hostName = "mail06.fetlifemail.com", capturable = false }, 
                new { hostName = "mail07.fetlifemail.com", capturable = false }, 
                new { hostName = "mail07.fetlifemail.com", capturable = false }, 
                new { hostName = "mail07.fetlifemail.com", capturable = false }, 
                new { hostName = "mail10.fetlifemail.com", capturable = false }, 
                new { hostName = "mail10.fetlifemail.com", capturable = false }, 
                new { hostName = "mail128-18.atl41.mandrillapp.com", capturable = false }, 
                new { hostName = "mail17-14.srv2.de", capturable = true }, 
                new { hostName = "mail45.wdc01.mcdlv.net", capturable = false }, 
                new { hostName = "mail67.suw11.mcdlv.net", capturable = false }, 
                new { hostName = "mail-bn1on0135.outbound.protection.outlook.com", capturable = false }, 
                new { hostName = "mail-ig0-f170.google.com", capturable = false }, 
                new { hostName = "mail-io0-f197.google.com", capturable = false }, 
                new { hostName = "mail-oi0-f74.google.com", capturable = false }, 
                new { hostName = "mailout03.etsy.com", capturable = false }, 
                new { hostName = "mail-yk0-f202.google.com", capturable = false }, 
                new { hostName = "mdc.com.co", capturable = true }, 
                new { hostName = "msbadger0202.apple.com", capturable = false }, 
                new { hostName = "mta.e.atlanticrecords.com", capturable = false }, 
                new { hostName = "mta0209.fbmta.com", capturable = true }, 
                new { hostName = "mta11.em.redbox.com", capturable = false }, 
                new { hostName = "mta192s1.r.groupon.com", capturable = false }, 
                new { hostName = "mta3.email.amctheatres.com", capturable = false }, 
                new { hostName = "mta4.email.safeway.com", capturable = false }, 
                new { hostName = "p1-100165.deals.michaels.com", capturable = false }, 
                new { hostName = "p1-182169.e.email.overnightprints.com", capturable = false }, 
                new { hostName = "pop.brownpapertickets.com", capturable = false }, 
                new { hostName = "r46.mail.womanwithin.com", capturable = false }, 
                new { hostName = "r46.mail.womanwithin.com", capturable = false }, 
                new { hostName = "remote.rainbow.be", capturable = true }, 
                new { hostName = "smartcorporateloans.com", capturable = true }, 
                new { hostName = "smtp3.dca.wordpress.com", capturable = false }, 
                new { hostName = "tdcmkt8.email-tickets.com", capturable = false } 
            };
            #endregion

            var failures = new StringBuilder();

            foreach (var datum in data)
            {
                var result = BadHostPlugin.IsHostCapturable("0.0.0.0", datum.hostName);

                if (result != datum.capturable)
                {
                    if (failures.Length == 0) failures.AppendLine("The following failed:");

                    failures.AppendLine(string.Format("\"{0}\"; Expected: {1}; Actual: {2}", datum.hostName, datum.capturable, result));
                }
            }
            Expect(failures.Length == 0, failures.ToString());
        }

        [Test]
        public void AcceptNoImitations_RealVmZooMail()
        {
            PostProcessResult result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.AcceptNoImitations("70.89.127.76", "vmzoomail", "someone@somewhere.org", ref result, ref reply);

            Expect(result == PostProcessResult.Accept, "result == " + result);
        }

        [Test]
        public void AcceptNoImitations_FakeVmZooMail()
        {
            PostProcessResult result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.AcceptNoImitations(TestIp, "vmzoomail", "someone@clarkzoo.org", ref result, ref reply);

            Expect(result == PostProcessResult.Reject, "result == " + result);
        }

        [Test]
        public void AcceptNoImitations_NullHost()
        {
            PostProcessResult result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.AcceptNoImitations(TestIp, null, "\"amber@clarkzoo.org\" <someone@somewhere.org>", ref result, ref reply);

            Expect(result == PostProcessResult.Reject, "result == " + result);
        }

        [Test]
        public void AcceptNoImitations_NullFrom()
        {
            PostProcessResult result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.AcceptNoImitations(TestIp, "host.com", null, ref result, ref reply);

            Expect(result == PostProcessResult.Reject, "result == " + result);
        }

        [Test]
        public void AcceptNoImitations_DoubleSenderDifferent()
        {
            PostProcessResult result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.AcceptNoImitations(TestIp, "somehost", "\"amber@clarkzoo.org\" <someone@somewhere.org>", ref result, ref reply);

            Expect(result == PostProcessResult.Reject, "result == " + result);
        }

        [Test]
        public void AcceptNoImitations_DoubleSenderSame()
        {
            PostProcessResult result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.AcceptNoImitations(TestIp, "somehost", "\"someone@somewhere.org\" <someone@somewhere.org>", ref result, ref reply);

            Expect(result == PostProcessResult.Accept, "result == " + result);
        }

        [Test]
        public void AcceptNoImitations_SingleSender()
        {
            PostProcessResult result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.AcceptNoImitations(TestIp, "somehost", "\"Someone Else\" <someone@somewhere.org>", ref result, ref reply);

            Expect(result == PostProcessResult.Accept, "result == " + result);
        }


        [Test]
        public void WhiteListCheck_NotIgnore()
        {
            var postProcessResult = PostProcessResult.Accept;
            BadHostPlugin.CheckMailFromWhiteList("amber@clarkzoo.org", "1.2.3.4", ref postProcessResult);

            Expect(postProcessResult == PostProcessResult.Ignore, "postProcessResult (" + postProcessResult + ") != Ignore");
        }

        [Test]
        public void WhiteListCheck_NotAccept()
        {
            var postProcessResult = PostProcessResult.Accept;
            BadHostPlugin.CheckMailFromWhiteList("frank@clarkzoo.org", "1.2.3.4", ref postProcessResult);

            Expect(postProcessResult == PostProcessResult.Accept, "postProcessResult (" + postProcessResult + ") != Accept");
        }

        [Test]
        public void IsFileNewer_nullList()
        {
            var path = @"f:\Dropbox\MailData\spam-ip-catalog.xml";

            List<string> nullList = null;

            var isNewer = BadHostPlugin.IsNewerFile(path, nullList);

            Expect(isNewer, path + " is not newer");
        }

        [Test]
        public void IsFileNewer_notNull()
        {
            var path = @"f:\Dropbox\MailData\spam-ip-catalog.xml";

            List<string> notNullList = new List<string>();

            // Call it once to initialize it's present in the collection
            BadHostPlugin.IsNewerFile(path, notNullList);

            // Call it again to check
            var isNewer = BadHostPlugin.IsNewerFile(path, notNullList);

            Expect(!isNewer, path + " is not newer");
        }

        [Test]
        public void PropertiesLoaded()
        {
            foreach (var entry in BadHostPlugin.BannedTLD)
            {
                Logger.Log("BannedTLD", entry);
            }
        }

        [Test]
        public void RejectImposters()
        {
            #region Test Data (testItems)
            var testItems = new[] {
                new { from = "\"CNN Exclusive Report:\" <amber@clarkzoo.org>", smtpFrom = "contact@discovery.com", 
                    expected = PostProcessResult.Reject },
                new { from = "\"amber@clarkzoo.org\"", smtpFrom = "" , expected = PostProcessResult.Reject},
                new { from = "\"amber@clarkzoo.org\" <amber@clarkzoo.org>", smtpFrom = "amber@stopped-motion.com", 
                    expected = PostProcessResult.Reject},
                new { from = "<amber@stopped-motion.com>", smtpFrom = "amber@stopped-motion.com", 
                    expected = PostProcessResult.Reject},
                new { from = "\"amber@clarkzoo.org\" <b00869cf9@datco.es>", smtpFrom = "" , 
                    expected = PostProcessResult.Reject},
                new { from = "<AMBER@STOPPED-MOTION.COM>", smtpFrom = "AMBER@STOPPED-MOTION.COM" , 
                    expected = PostProcessResult.Reject},
                new { from = "amber <amber@clarkzoo.org>", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Reject},
                new { from = "\"Antivirus Software\" <amber@clarkzoo.org>", smtpFrom = "contact@discovery.com" , 
                    expected = PostProcessResult.Reject},
                new { from = "\"Oil Change Discount	 <_>", smtpFrom = "abilao.classroomclips.net" , 
                    expected = PostProcessResult.Reject},
                new { from = "\"Amber Clark\" <amber@clarkzoo.org>", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Accept},
                new { from = "\"Amber Clark\" <amber@stopped-motion.com>", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Accept},
                new { from = "Subviewer3@yahoogroups.com", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Accept},
                new { from = "marketing@eventbee.com", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Accept},
                new { from = "monitor@clarkzoo.org", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Accept},
                new { from = "serviceawards@spihq.com", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Accept},
                new { from = "\"Miss Violet DeVille\" <violet@violetdeville.com>", smtpFrom = "amber@clarkzoo.org" , 
                    expected = PostProcessResult.Accept}
            };

            #endregion

            string reply = null;
            var output = new StringBuilder();

            foreach (var item in testItems)
            {
                var postProcessResult = PostProcessResult.Accept;

                BadHostPlugin.RejectImposters("0.0.0.0", item.from, item.smtpFrom, ref postProcessResult, ref reply);

                if (postProcessResult != item.expected)
                {
                    if (output.Length == 0)
                    {
                        output.AppendLine("The following from values failed:");
                    }

                    var log = string.Format("Test Value: {0}; Expected Result: {1}; Actual: {2}",
                        item.from, item.expected, postProcessResult);
                    Logger.Log("Fail", log);
                    output.AppendLine(log);
                }
            }

            Expect(output.Length == 0, output.ToString());
        }

        [Test]
        public void NoQuestionableBodies()
        {
            var msg = GetTestMessage();
            var result = PostProcessResult.Accept;
            string reply = null;

            BadHostPlugin.NoQuestionableBodies(TestIp, msg, ref result, ref reply);
        }
        [Test]
        public void BadDsiplayNameFilter()
        {
            #region Test Emails (testEmailAddresses)
            var testEmailAddresses = new[] {new { Email = "<jen.a.mcinerney@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "<TimeAndExpense@allegisgroup.com>", Result = PostProcessResult.Accept},
                new { Email = "=?US-ASCII?Q?Good_To_Go?= <wsdot@service.govdelivery.com>", Result = PostProcessResult.Accept},
                new { Email = "=?UTF-8?B?QmF5b3UgTXlzdMOocmU=?= <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "=?UTF-8?B?QsO8YmVzIFJhZGxleQ==?= <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "=?UTF-8?B?QWltw6llIEJydW5lYXU=?= <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "=?UTF-8?B?U3lsdmlhIE/igJlTdGF5Zm9ybW9yZQ==?= <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Bombsheller?= <badass@bombsheller.com>", Result = PostProcessResult.Accept},
                new { Email = "=?UTF-8?Q?Elspeth_G=C3=B6tz?= <swordsinger@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Jamberry=20Nails?= <support@jamberrynails.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?King=20County=20|=20Girl=20Scouts=20of=20Western=20Washington?= <KingCountyEvents@GirlScoutsWW.org>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Kristie?= <support@swakdesigns.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Miss=20Kitty=20Baby?= <misskittybaby@msn.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Red=20Hot=20Annie?= <annie@redhotannie.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Seattle=20Pipeline?= <seattlepipeline@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Sinner=20Saint=20Burlesque?= <info@sinnersaintburlesque.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Stan=20Winston=20School?= <info@stanwinstonschool.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Time=20For=20National=20Wedding=20Month=21?= <contact@snapknot.com>", Result = PostProcessResult.Accept},
                new { Email = "=?utf-8?Q?Woot?= <no-reply@woot.com>", Result = PostProcessResult.Accept},
                new { Email = "2015 Refi Options <Shaw1@doingitanddone.com>", Result = PostProcessResult.Reject},
                new { Email = "24Hour Lift <Ward1@beautycardforus.com>", Result = PostProcessResult.Reject},
                new { Email = "3 Day Blinds <3DayBlinds@ablenus.website>", Result = PostProcessResult.Reject},
                new { Email = "4 Sizes by Valentine's Day <4SizesbyValentines@visual043.spreadawaybodyfats.org>", Result = PostProcessResult.Reject},
                new { Email = "401K Plan <KPlan@timenflplz.com>", Result = PostProcessResult.Reject},
                new { Email = "5th Avenue Theatre <news@5thavenue.org>", Result = PostProcessResult.Accept},
                new { Email = "Aaliyah D. <eyes@linencilia.com>", Result = PostProcessResult.Accept},
                new { Email = "Aaron H. R. Allshouse <aallshouse@msn.com>", Result = PostProcessResult.Accept},
                new { Email = "Aaron Joshua Shay <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Abby Fulkerson <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Abby Wegener <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Abigail Michell <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Abigail Pishaw <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "ABS trading software <ABStradingsoftware@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "accounts@livejournal.com", Result = PostProcessResult.Accept},
                new { Email = "Admin <no-replay@bbbl.org>", Result = PostProcessResult.Accept},
                new { Email = "Alan Tozier <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Alaska Airlines <deals@ifly.alaskaair.com>", Result = PostProcessResult.Accept},
                new { Email = "Alex Duff <obese@yawlspave.com>", Result = PostProcessResult.Accept},
                new { Email = "Alexa Perplexa Anastas <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Allura Fette <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Amanda Mayven <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Amazon <katherine@portshappenings.com>", Result = PostProcessResult.Accept},
                new { Email = "Amazon Claim-Code <Greer1@retailgalortime.com>", Result = PostProcessResult.Reject},
                new { Email = "Amazon Coupon Rewards <Reid1@southwestssurveying.com>", Result = PostProcessResult.Reject},
                new { Email = "Amazon Instant Video <store-news@amazon.com>", Result = PostProcessResult.Accept},
                new { Email = "Amazon Local <LocalDeals@amazon.com>", Result = PostProcessResult.Accept},
                new { Email = "Amazon.com <store_news@amazon.com>", Result = PostProcessResult.Accept},
                new { Email = "Amazon.com <store-news@amazon.com>", Result = PostProcessResult.Accept},
                new { Email = "Amazon.com <vfe-campaign-response@amazon.com>", Result = PostProcessResult.Accept},
                new { Email = "AMBarksdale <AMBarksdale@hotmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Amber Clark <amber@clarkzoo.org>", Result = PostProcessResult.Accept},
                new { Email = "Amber Clark <amber@stopped-motion.com>", Result = PostProcessResult.Accept},
                new { Email = "amber@clarkzoo.org", Result = PostProcessResult.Accept},
                new { Email = "AMC Theatres <noreply@email.amctheatres.com>", Result = PostProcessResult.Accept},
                new { Email = "America's Economic Disaster <AmericasEconomicDisaster@equal198.crashstockmarketover.org>", Result = PostProcessResult.Reject},
                new { Email = "Amy McHaahr Mahon <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "AnastasiaSingle Team <AnastasiaSingleTeam@vietnamtravel360.com>", Result = PostProcessResult.Reject},
                new { Email = "Andrea Westaway <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Andrew Watson <andreww@cadlink.com>", Result = PostProcessResult.Accept},
                new { Email = "Angela Petite Mort <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Angelica Barksdale <ambarksdale@hotmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Angie Colleen Swanson <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ann Mcdonough Mallory <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Anne Honeycutt-Joseph <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Another angry woman <comment-reply@wordpress.com>", Result = PostProcessResult.Accept},
                new { Email = "Anya Ngo <Jessica_Dill@tutorrink.com>", Result = PostProcessResult.Accept},
                new { Email = "App Store <AppStore@new.itunes.com>", Result = PostProcessResult.Accept},
                new { Email = "Ariel Amoure <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Artemis Lark <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "artStar Charlatan <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ashley Sheridan <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ashton <webmaster@unbonprof.com>", Result = PostProcessResult.Accept},
                new { Email = "Backdrop Outlet <info@backdropoutlet.com>", Result = PostProcessResult.Accept},
                new { Email = "Badass Jewelry <mail@badassjewelry.com>", Result = PostProcessResult.Accept},
                new { Email = "Bailey Asher James <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Bana Banal <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Banfield Pet Hospital <email@mailer.banfield.com>", Result = PostProcessResult.Accept},
                new { Email = "BECU eStatements <noreply@becuestatements.becu.org>", Result = PostProcessResult.Accept},
                new { Email = "Bella La Blanc <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Bella Sin Delgado <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Benny Pedersen <me@junc.eu>", Result = PostProcessResult.Accept},
                new { Email = "Bertrand Caplet <bertrand.caplet@chunkz.net>", Result = PostProcessResult.Accept},
                new { Email = "Better than a facelift [Ellen] <Schmidt1@clinicalupdating.com>", Result = PostProcessResult.Reject},
                new { Email = "Binary Options <BinaryOptions@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Blizzard Entertainment <Newsletter@email.blizzard.com>", Result = PostProcessResult.Accept},
                new { Email = "Blizzard Entertainment <noreply@blizzard.com>", Result = PostProcessResult.Accept},
                new { Email = "Block Carbs Oprah-Way <Oprah_Skips_Carbs@vanilla062.haveskinnybody.org>", Result = PostProcessResult.Reject},
                new { Email = "Blood Pressure Solution <BloodPressureSolution@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Blood Sugar Control <bloodsugarcontrol@vortex176.figuresdiabetesvalue.org>", Result = PostProcessResult.Reject},
                new { Email = "Blu Reine <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Bob Comer <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Bob Kohl <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Bob Zilla <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "BorrowLenses.com <borrowlenses@e.borrowlenses.com>", Result = PostProcessResult.Accept},
                new { Email = "Brain Ammo <BrainAmmo@babettemedia.com>", Result = PostProcessResult.Reject},
                new { Email = "Brain Stimulator <BrainStimulator@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Brain Stimulator Method <BrainStimulatorMethod@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Brain-Stimulator <Brain-Stimulator@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Brain-Stimulator <Brain-Stimulator@rlmautosales.com>", Result = PostProcessResult.Reject},
                new { Email = "Brandi Benson <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Brian Macky McReynolds <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Bridgette Messick <nufosy@surfer.at>", Result = PostProcessResult.Accept},
                new { Email = "BronZe Bettina <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Brown Girls Burlesque <browngirlsburlesque@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Brown Paper Tickets <noreply-bpt@brownpapertickets.com>", Result = PostProcessResult.Accept},
                new { Email = "Buca di Beppo <BucadiBeppo@buca.fbmta.com>", Result = PostProcessResult.Accept},
                new { Email = "Bunny Bedford <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Bunny Vish'us <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "BurlyCon Admin Director <ad@burlycon.org>", Result = PostProcessResult.Accept},
                new { Email = "Burn FAT Quicker <BurnFATQuicker@ruiykmninny.com>", Result = PostProcessResult.Reject},
                new { Email = "C Rockie Mountains <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "CableService <news@1timenewsrun.com>", Result = PostProcessResult.Reject},
                new { Email = "CableService <news@fiftysevenbustlingukoffers.com>", Result = PostProcessResult.Reject},
                new { Email = "CableService <news@thenewsdoctorisin.com>", Result = PostProcessResult.Reject},
                new { Email = "Cam Riebe <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Cameron XLB <camxlb@hotmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Captain Robert <robert@abneypark.com>", Result = PostProcessResult.Accept},
                new { Email = "Carina Henry <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Caringforaparent.com <Caringforaparent.com@golfgps-units.com>", Result = PostProcessResult.Reject},
                new { Email = "Carlos Sirombo <royotto@bergenscd.org>", Result = PostProcessResult.Accept},
                new { Email = "Carlotta Shakin <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Casey Davis Jones <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Cassandra Moselle <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Cat Litter Coupons <6944@cazaregurahumorului.com>", Result = PostProcessResult.Reject},
                new { Email = "Catalina Mystique <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Cate R Siguenza <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Celebrity Carb Inhibitor <CelebrityCarbInhibitor@equal195.qualifysmallbody.org>", Result = PostProcessResult.Reject},
                new { Email = "Cellulite <Cellulite@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "CertifiedNursingAssistantEducation <news@greatexpectionsfornews.com>", Result = PostProcessResult.Reject},
                new { Email = "CertifiedNursingAssistantEducation <news@waferthinnews.com>", Result = PostProcessResult.Reject},
                new { Email = "Charlotte Lin <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Chaz R Oyal <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Chelsea Nicole Rabelas <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Christi Michelle <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Christopher Campbell <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Christopher Todd <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Claracoquette Burlyq <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Consolidate Debt <consolidatedebt@wdfmd.com>", Result = PostProcessResult.Reject},
                new { Email = "ConsolidateDebt <news@newsofthedragon.com>", Result = PostProcessResult.Reject},
                new { Email = "Constance T. Nople <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Coralyn Martin <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Courtney Connor <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Courtney Shrumm <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "CreativeLive <learn@email.creativelive.com>", Result = PostProcessResult.Accept},
                new { Email = "Cristina Solmaz <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "CustomerSupport@wahbexchange.org", Result = PostProcessResult.Accept},
                new { Email = "Dan Knight, LowEndMac.com <lowenddan@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Dangerous GMOs <DangerousGMOs@vortex174.nationaleconomyissue.org>", Result = PostProcessResult.Reject},
                new { Email = "D'Arcy Harrison <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Dark Regions <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "David Nail <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "David Spada <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Dayton Allen <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "dd-pipeline@microsoft.com", Result = PostProcessResult.Accept},
                new { Email = "Deanna Lee <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Debug Mail (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Deirdre McCollom Hadlock <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Delicia Pastiche <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Denise Nicole <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Diabetes-Miracle <diabetesm@zhaocaibet.com>", Result = PostProcessResult.Reject},
                new { Email = "Diana M Dotter <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Dice with Buddies <info@dice-with-buddies.scopely.com>", Result = PostProcessResult.Reject},
                new { Email = "Digg <team@email.digg.com>", Result = PostProcessResult.Accept},
                new { Email = "Discreet Affair <Affair@by.cothasanswers.com>", Result = PostProcessResult.Reject},
                new { Email = "DIY-Woodworking For Everyone <Turner1@suchdib.com>", Result = PostProcessResult.Reject},
                new { Email = "Don Young <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Doodle <mailer@doodle.com>", Result = PostProcessResult.Accept},
                new { Email = "Dorothy Modlin <dorothy.modlin@traducerieng.me>", Result = PostProcessResult.Accept},
                new { Email = "Dottie Lux <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "DrOz Unusual Belly Blaster <drozunusualbellyblaster@result061.stillweightloss.org>", Result = PostProcessResult.Reject},
                new { Email = "Early Childhood Education <8875@jesiha.com>", Result = PostProcessResult.Reject},
                new { Email = "Early Childhood Education <8875@stylegyani.com>", Result = PostProcessResult.Reject},
                new { Email = "Early Valentines Savings <EarlyValentinesSavings@vortex171.specialflowerforgift.org>", Result = PostProcessResult.Reject},
                new { Email = "Eczema <Eczema@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Ed Corrigan <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "EgyptBlaque Knyle <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Elektronikai cikkek <elektronikaih6h7jx5cikkek@sdhdrnovice.com>", Result = PostProcessResult.Reject},
                new { Email = "Elizabeth Lottman <pickacardburlesque@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ellen Degeneres Beauty Lesson <Greer1@chartskinlook.com>", Result = PostProcessResult.Reject},
                new { Email = "Elspeth Gotz <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Elspeth Gotz <notification+zj4ocfytj996@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Elspeth Gotz <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Erase Tinnitus <EraseTinnitus@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Erectile Dysfunction Protocol <ErectileDysfunctionProtocol@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "e-Rewards <erewards@e-rewards.net>", Result = PostProcessResult.Accept},
                new { Email = "Eric Starker <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Erin Kelly Brown <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Erin Renae Burdick <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Essence Walker <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Esther Chilcutt <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Esther Chilcutt <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "eSurface Protector  <esurface@visagroup.net>", Result = PostProcessResult.Reject},
                new { Email = "Etsy <emails@mail.etsy.com>", Result = PostProcessResult.Accept},
                new { Email = "Etsy Finds <emails@mail.etsy.com>", Result = PostProcessResult.Accept},
                new { Email = "Eva D Be <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Eventful <events@eventful.com>", Result = PostProcessResult.Accept},
                new { Email = "Evernote Team <team@email.evernote.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <noreply@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <notification+iv161p1z@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <notification+oazacfj6@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <notification+zf9toytc@pages.facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <notification+zj4ocfytj996@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <notification+zrdodv6cvfe1@pages.facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <update+iv161p1z@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <update+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <update+zj4o6stsyoy9@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <update+zj4ocfytj996@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook <update+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Facebook Ads Team <advertise-noreply@support.facebook.com>", Result = PostProcessResult.Accept},
                new { Email = "Facelift in a bottle <Vanessa@bakersonskinclinic.com>", Result = PostProcessResult.Reject},
                new { Email = "Fallon Grey <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Fashion Fabrics Club <customerservice@fashionfabricsclub.com>", Result = PostProcessResult.Accept},
                new { Email = "FatChanceBellyDance, Inc. <fcbdoffice@fcbd.com>", Result = PostProcessResult.Accept},
                new { Email = "FDAFoodScandal <FDAFoodScandal@result063.relativefoxnews.org>", Result = PostProcessResult.Reject},
                new { Email = "Federal Grants Status", Result = PostProcessResult.Accept},
                new { Email = "Federal Student Aid-Distributions <Reyes@flynnstreetfinance.com>", Result = PostProcessResult.Reject},
                new { Email = "FetLife <donotreply@fetlifemail.com>", Result = PostProcessResult.Accept},
                new { Email = "Fira Alexandra <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Fiserv Secure Notification <secure.notification@fiserv.com>", Result = PostProcessResult.Accept},
                new { Email = "Fitness - Oprah-Style <FitnessOprahStyle@visual046.spotcigarettefilters.org>", Result = PostProcessResult.Reject},
                new { Email = "Flag <Flag@appleaueen.com>", Result = PostProcessResult.Accept},
                new { Email = "Flag <Flag@findaguynows.com>", Result = PostProcessResult.Accept},
                new { Email = "Flag <Flag@sleepisforu.com>", Result = PostProcessResult.Accept},
                new { Email = "Flag <Flag@tiebuyscars.com>", Result = PostProcessResult.Accept},
                new { Email = "FoxConnect.com <20thCenturyFox@newsletter.foxhome.com>", Result = PostProcessResult.Accept},
                new { Email = "Frank Capezzuto <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "FreeCellPhone <news@coresitenews.com>", Result = PostProcessResult.Reject},
                new { Email = "FreeCellPhone <news@fiftytwoflawlessukoffers.com>", Result = PostProcessResult.Reject},
                new { Email = "FreeCellPhone <news@warriornewsforyou.com>", Result = PostProcessResult.Reject},
                new { Email = "Frenchie Renard <messages-noreply@linkedin.com>", Result = PostProcessResult.Accept},
                new { Email = "From the Desk of Allen Watson <FromtheDeskofAllenWatson@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Gabriella Maze <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Gaia Online <bounces@mailer.gaiaonline.com>", Result = PostProcessResult.Accept},
                new { Email = "Gala Delish <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "GarageCoatings <GarageCoatings@castle-residence.com>", Result = PostProcessResult.Reject},
                new { Email = "Gavour George <runatamba1@yahoo.com>", Result = PostProcessResult.Accept},
                new { Email = "George Sadak <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ginger Rockafella <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Girl Scout Cookies <noreply@girlscouts.org>", Result = PostProcessResult.Accept},
                new { Email = "Glamour modeling and photography <info@meetup.com>", Result = PostProcessResult.Accept},
                new { Email = "Global Who's--Who <global@zjdfryp.com>", Result = PostProcessResult.Reject},
                new { Email = "Glucose Control 7x <drab@appsheretime.biz> ", Result = PostProcessResult.Reject},
                new { Email = "Gmail <araceli.rogers26@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "GMO Danger Exposed <GMODangerExposed@equal193.realeconomicnews.org>", Result = PostProcessResult.Reject},
                new { Email = "GoDaddy <offers@godaddy.com>", Result = PostProcessResult.Accept},
                new { Email = "GoFreebies <newsletter@gofreebies.com>", Result = PostProcessResult.Accept},
                new { Email = "GoFundMe <news@gofundme.com>", Result = PostProcessResult.Accept},
                new { Email = "GolfersPainRelief <GolfersPainRelief@sum222.feelreliefjointpain.org>", Result = PostProcessResult.Reject},
                new { Email = "Google Alerts <googlealerts-noreply@google.com>", Result = PostProcessResult.Accept},
                new { Email = "Google Calendar <calendar-notification@google.com>", Result = PostProcessResult.Accept},
                new { Email = "Google+ (Elf Boy) <noreply-63bd92f7@plus.google.com>", Result = PostProcessResult.Accept},
                new { Email = "Google+ (KamenRiderGumo) <noreply-63bd92f7@plus.google.com>", Result = PostProcessResult.Accept},
                new { Email = "Google+ <noreply-2c4b41ab@plus.google.com>", Result = PostProcessResult.Accept},
                new { Email = "Great Plains Lending <admin@greatplainslending.com>", Result = PostProcessResult.Accept},
                new { Email = "Gregory, Kim <Kim.Gregory@adeccona.com>", Result = PostProcessResult.Accept},
                new { Email = "Groupon <noreply@r.groupon.com>", Result = PostProcessResult.Accept},
                new { Email = "Groupon Getaways <noreply@r.groupon.com>", Result = PostProcessResult.Accept},
                new { Email = "Groupon Goods <noreply@r.groupon.com>", Result = PostProcessResult.Accept},
                new { Email = "Hair Loss Protocol <HairLossProtocol@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Hans Iverson <hans@hansiverson.com>", Result = PostProcessResult.Accept},
                new { Email = "HARP_2.0 Verification Changes <Cunningham@carbrown.com>", Result = PostProcessResult.Reject},
                new { Email = "HeartAttackFighter@invertirenvino.net", Result = PostProcessResult.Reject},
                new { Email = "Heather Carper <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Heather Love <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Hedwig Robinson <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Himanthi Karunathilaka <himanthi_k@hotmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Hlaf Vmaprie (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Holly Amber <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Holly Dai <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Home Security <HomeSecurity@preistbeatpally.com>", Result = PostProcessResult.Reject},
                new { Email = "Home Security <HomeSecurity@tankwowforu.com>", Result = PostProcessResult.Reject},
                new { Email = "Home Security <HomeSecurity@tiebuyscars.com>", Result = PostProcessResult.Reject},
                new { Email = "Home_Goods_Outlet <8995@loanauctionmarkets.com>", Result = PostProcessResult.Reject},
                new { Email = "HypnosisTechniqueExchange@yahoogroups.com", Result = PostProcessResult.Accept},
                new { Email = "Ilise S. Carter <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "ImogenQuest Miranda Warner <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Indiegogo <newsletter@indiegogo.com>", Result = PostProcessResult.Accept},
                new { Email = "INTERNAL FAX <fax@clarkzoo.org>", Result = PostProcessResult.Reject},
                new { Email = "iPhone Tracking Chip <iPhoneTrackingChip@result067.trackinggeneral.org>", Result = PostProcessResult.Reject},
                new { Email = "iTunes Store <do_not_reply@itunes.com>", Result = PostProcessResult.Accept},
                new { Email = "IvaFiero Productions <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jackie Latendresse <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jacqueline Hyde <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "James Dorian Gaynes <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "James Haas <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "James Lyle <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "James Rice <jrice@nsd.org>", Result = PostProcessResult.Accept},
                new { Email = "Jared Polin <fro@froknowsphoto.com>", Result = PostProcessResult.Accept},
                new { Email = "Jason <roger@liferstimeds.biz>", Result = PostProcessResult.Accept},
                new { Email = "Jason Meininger <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jay T. Conrad <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "JD Beggs <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jeff Meaders <solar.eclipse6@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jeff Ramsey <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jeff Stwart <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jen Gapay <jen@thirstygirlproductions.com>", Result = PostProcessResult.Accept},
                new { Email = "Jenn Baker <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jennifer Ewing <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jennifer Louise Lopez <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jennifer Toby <fetishlady43@aol.com>", Result = PostProcessResult.Accept},
                new { Email = "Jennifer Toby <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jennifer Wold <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jesse-Linn Masters <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jessica Rosa <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jesus la Pinga (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Jezabelle von Jane <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jim D. Dean <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jimmy Berg <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "JL Barnett <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jo Jo Stiletto <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jo Weldon (BurlyCon) <application@teamwork.com>", Result = PostProcessResult.Accept},
                new { Email = "Joce Weird <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "John <tomliucontact@sina.com>", Result = PostProcessResult.Accept},
                new { Email = "John Hardin <jhardin@impsec.org>", Result = PostProcessResult.Accept},
                new { Email = "John Kijhn Morrison <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "John Metz IV <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "John.DAgostino@promo45.net", Result = PostProcessResult.Accept},
                new { Email = "Johnson <tberger@autosnapa.com>", Result = PostProcessResult.Accept},
                new { Email = "JoJoStiletto <jojostiletto@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jon Lutyens <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jonathan Smith <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Jones, Alisa <aljones@teksystems.com>", Result = PostProcessResult.Accept},
                new { Email = "Joshua Jlp Hancock <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Josie Redmond <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Joy Faith Daley <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Joy Faith Daley <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Joy Gracey <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Julia from Hootsuite <julia@hootsuite.com>", Result = PostProcessResult.Accept},
                new { Email = "Just Great Software Newsletter <newsletter@jgsoft.com>", Result = PostProcessResult.Accept},
                new { Email = "Justin F. (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Kaitlyn Tigerlily Weldon <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kara Tucker <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Karen Iva Handfull <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Karen Junker <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kasondra Gilbert <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "kat DeLac <katdelacburlesque@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Katharine Bond <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kathy Hsieh <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kay C. Sunshine <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kelsi Jensen <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ken Krahl <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kerry Rodda <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kevin A. McGrail <KMcGrail@PCCC.com>", Result = PostProcessResult.Accept},
                new { Email = "Kevin Brennan <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kickstarter <no-reply@kickstarter.com>", Result = PostProcessResult.Accept},
                new { Email = "Kickstarter HQ <no-reply@kickstarter.com>", Result = PostProcessResult.Accept},
                new { Email = "Kidney Function <KidneyFunction@zeeiuyhgthy.com>", Result = PostProcessResult.Reject},
                new { Email = "Kim Archer <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kitty Ashton <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Kitty Serena <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "KnitPicks.com <customerservice@knitpicks.com>", Result = PostProcessResult.Accept},
                new { Email = "Kolea Hara <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "KPlan <news@yourfastnewsnow.com>", Result = PostProcessResult.Accept},
                new { Email = "Kylie Hubbard <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lady Monster <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Laika Fox <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Language_Secrets <lang@flavorwall.com>", Result = PostProcessResult.Reject},
                new { Email = "Laura Tempest Zakroff <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Laurel R. Dodge <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Laurie Rector <lauriemf@msn.com>", Result = PostProcessResult.Accept},
                new { Email = "Len Tischner <len@msfundingpartners.com>", Result = PostProcessResult.Accept},
                new { Email = "Let iPhone Find It <LetiPhoneFindIt@equal199.wiremobiletracker.org>", Result = PostProcessResult.Reject},
                new { Email = "Lewd N Lucky <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lili VonSchtupp <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lilith von Fraumench <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lillian Cohen-Moore (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Lilyana Fey <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Linda <Avery@hpowerforce.com>", Result = PostProcessResult.Accept},
                new { Email = "Lindsay Luna Rouge <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "LinkedIn <jobs-listings@linkedin.com>", Result = PostProcessResult.Accept},
                new { Email = "LinkedIn Updates <messages-noreply@linkedin.com>", Result = PostProcessResult.Accept},
                new { Email = "Little MissRollerhoops <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "LiveJournal <accounts@livejournal.com>", Result = PostProcessResult.Accept},
                new { Email = "LiveJournal <lj_notify@livejournal.com>", Result = PostProcessResult.Accept},
                new { Email = "Liz Cruz <liz.cruz@lilydivine.com>", Result = PostProcessResult.Accept},
                new { Email = "Lock-In with Discover <LockinwithDiscover@visual045.probablyrefinance.org>", Result = PostProcessResult.Reject},
                new { Email = "Lola Demure <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lola Hart <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lola L Elspeth <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lola Love <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Loree Parker <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lottie LaCroix <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lovers <toymistress@list.loverspackage.com>", Result = PostProcessResult.Accept},
                new { Email = "Lucy Morals <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Lydia Swartz <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mad Marquis <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Madeline Sinclaire <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Maella Cai Vane <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Maggie Missile <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "mailman-owner@lists.nsd.org", Result = PostProcessResult.Accept},
                new { Email = "Mama Rogers <araceli.rogers26@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "MamaCane AndThe GlitterpussyRiot <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mandy Flame <mandy.flame.burlesque@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mandy Flame <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mandy McGee <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "ManicPixieDreamFox (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Manuela Cocchi <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Marc Perkel <support@junkemailfilter.com>", Result = PostProcessResult.Accept},
                new { Email = "Marcus DeBois <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Marcus Gorman (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Margarette Osornio <inihsuza@myvzw.com>", Result = PostProcessResult.Accept},
                new { Email = "Mari Adams <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Maria Delrio <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mariah Black <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mariah TrixiePaprika Pepper <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Marianna de Fazio <marianna@tpsonline.org>", Result = PostProcessResult.Accept},
                new { Email = "Marie Brownfield <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Marissa Patrick <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mark Crain, MoveOn.org Political Action <moveon-help@list.moveon.org>", Result = PostProcessResult.Accept},
                new { Email = "Mark Henderson <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mark_Cubam <desk@iradebeep.com>", Result = PostProcessResult.Accept},
                new { Email = "Marly Leigh Hauber <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Marry <naughtilyrda0@supernatuaralworks.com>", Result = PostProcessResult.Accept},
                new { Email = "Matilda <now@dgbuzz.com>", Result = PostProcessResult.Accept},
                new { Email = "Matus UHLAR - fantomas <uhlar@fantomas.sk>", Result = PostProcessResult.Accept},
                new { Email = "Maura Hubbell (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "May Hemmer <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Meetup <info@meetup.com>", Result = PostProcessResult.Accept},
                new { Email = "Melissa Veronica Marx <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "MemberService@FirstTechFed.com <memberservice@firsttechfed.com>", Result = PostProcessResult.Accept},
                new { Email = "Mens.Renewal <Mens.Renewal@holumi.com>", Result = PostProcessResult.Reject},
                new { Email = "Mesh-Claim-Deadline <MeshClaimDeadline@result064.servedmedicalhelp.org>", Result = PostProcessResult.Reject},
                new { Email = "Mia Mutch <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Mia PiaCherrie <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Michael <ad@arenabonus.com>", Result = PostProcessResult.Accept},
                new { Email = "Michael Cepress <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Michael Hanscom <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Michael Hanscom <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Michael Renney <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Michaels <Michaels@deals.michaels.com>", Result = PostProcessResult.Accept},
                new { Email = "Michele Dainiak (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Miranda Tempest <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Miss Kitty Baby Fan Page <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Miss Violet DeVille (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Miz Melancholy <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Monthly Home Payment Lower", Result = PostProcessResult.Accept},
                new { Email = "Morgana Alba <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Motty Cruz <motty.cruz@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Moxie LaBouche <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "MR.JUAN PENA CARLOS.<mrcarlospena@undp.org>", Result = PostProcessResult.Accept},
                new { Email = "Mrs. Kim James<carrie@asecretadmirer.com>", Result = PostProcessResult.Accept},
                new { Email = "MRS.LISA CLARENCE<frederic.collet@fr.oleane.com>", Result = PostProcessResult.Accept},
                new { Email = "mto <mtheresao@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "MyLife <mylife@mail.mylife.com>", Result = PostProcessResult.Accept},
                new { Email = "Nail Fungus Remedies <8727@cazaresucevita.com>", Result = PostProcessResult.Reject},
                new { Email = "Nancy <sales2@hlmold.com>", Result = PostProcessResult.Accept},
                new { Email = "Natalie Porter <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "NatureBox <noreply@naturebox.com>", Result = PostProcessResult.Accept},
                new { Email = "NervePain Relief <NervePainRelief@todogualeguaychu.com>", Result = PostProcessResult.Reject},
                new { Email = "NeuropathyMiracle <NeuropathyMiracle@gilbergco.com>", Result = PostProcessResult.Reject},
                new { Email = "NeuropathyMiracle <NeuropathyMiracle@tahoz.com>", Result = PostProcessResult.Reject},
                new { Email = "New Mort-Lows <refi.dept@thebluechimp.com>", Result = PostProcessResult.Reject},
                new { Email = "Niamh Holding <niamh@fullbore.co.uk>", Result = PostProcessResult.Accept},
                new { Email = "Nicholas John Pozega <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Nichole Nadkarni <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Nicole Murphy via PayPal <member@paypal.com>", Result = PostProcessResult.Accept},
                new { Email = "Nikki Lorraine Fox <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Nintendo <nintendo@em-news.nintendo.com>", Result = PostProcessResult.Accept},
                new { Email = "no-reply <no-reply@modelmayhem.com>", Result = PostProcessResult.Accept},
                new { Email = "NOTIFICATION <Delilah@fleecewae.com>", Result = PostProcessResult.Accept},
                new { Email = "NSD Communications <communications@nsd.org>", Result = PostProcessResult.Accept},
                new { Email = "NSD Community Mailing List <nsdcommunity@lists.nsd.org>", Result = PostProcessResult.Accept},
                new { Email = "Obsession Phrases <ObsessionPhrases@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Office Depot <rewards@e.officedepot.com>", Result = PostProcessResult.Accept},
                new { Email = "Okashii Fireclaw <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "OneStopPlus.com <OneStopPlus@mail.onestopplus.com>", Result = PostProcessResult.Accept},
                new { Email = "Online Doctorate <OnlineDoctorate@appleaueen.com>", Result = PostProcessResult.Reject},
                new { Email = "Online Doctorate <OnlineDoctorate@brumaa.com>", Result = PostProcessResult.Reject},
                new { Email = "Online Doctorate <onlinedoctorate@wdfmd.com>", Result = PostProcessResult.Reject},
                new { Email = "OnlineDoctorate <news@ontothenews.com>", Result = PostProcessResult.Reject},
                new { Email = "Oprah Lowers Blood Sugar <oprahlowersbloodsugar@result065.popularslimbodies.org>", Result = PostProcessResult.Reject},
                new { Email = "Oprah Secretly Eats This <oprahsecretlyeatsthis@complex020.todayskinnybodytype.org>", Result = PostProcessResult.Reject},
                new { Email = "Oprah Secretly Eats This <oprahsecretlyeatsthis@result062.withmodelbody.org>", Result = PostProcessResult.Reject},
                new { Email = "Oprah's Amazing Results <OprahsAmazingResults@vortex173.dropdresssizeover.org>", Result = PostProcessResult.Reject},
                new { Email = "Oprah's Carb-Interceptor <OprahsCarbIntercept@visual044.directlybodyfit.org>", Result = PostProcessResult.Reject},
                new { Email = "Oprah's Unique Fitness Routine <oprahsuniquefitnessroutine@complex018.simplybodydietplan.org>", Result = PostProcessResult.Reject},
                new { Email = "OvernightPrints <OvernightPrints@e.email.overnightprints.com>", Result = PostProcessResult.Accept},
                new { Email = "Pacific Fabrics & Crafts <annette@pacificfabrics.com>", Result = PostProcessResult.Accept},
                new { Email = "Paige Rustles <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Paizo.com <store@paizo.com>", Result = PostProcessResult.Accept},
                new { Email = "Paola <new.day@chottloin.com>", Result = PostProcessResult.Accept},
                new { Email = "Patrick McKinnion <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Payless ShoeSource <payless_customer_service@updates.payless.com>", Result = PostProcessResult.Accept},
                new { Email = "PayPAMS Customer Service <customerservice@PayPAMS.com>", Result = PostProcessResult.Accept},
                new { Email = "Pearl Klein <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Pearl Robledo <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Penelope Rose <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Penny stock sniper <Pennystocksniper@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Peter <anazawaakanetakta@live.com>", Result = PostProcessResult.Accept},
                new { Email = "Peter A Montgomery <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Peter Noble <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "PhotoShelter <noreply@photoshelter.com>", Result = PostProcessResult.Accept},
                new { Email = "Pinterest <editorial@pinterest.com>", Result = PostProcessResult.Accept},
                new { Email = "Pinterest <pinbot@pinterest.com>", Result = PostProcessResult.Accept},
                new { Email = "Pinterest Weekly <weekly@pinterest.com>", Result = PostProcessResult.Accept},
                new { Email = "Polyvore <postman@polyvore.com>", Result = PostProcessResult.Accept},
                new { Email = "Poppy Raucous <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Popular in your network <info@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Potential <info@aneardrew.com>", Result = PostProcessResult.Accept},
                new { Email = "Pre-Owned Cars <7167@sheenatabraham.com>", Result = PostProcessResult.Reject},
                new { Email = "Purple Devil Productions <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Queenie O'Hart <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Rachael Gets Skinny <rachaelgetsskinny@vanilla061.stopfatbelly.org>", Result = PostProcessResult.Reject},
                new { Email = "Rachael's Fat-Burning Secret <RachaelsfatburningSecret@equal196.destroyfatspowerlevel.org>", Result = PostProcessResult.Reject},
                new { Email = "Rachael's Loss Guarantee <RachaelsLossGuarantee@equal192.fewlossobesebody.org>", Result = PostProcessResult.Reject},
                new { Email = "RaeRae Sachs <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Rasa Vitalia <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Rate Increase Notice <RateIncreaseNotice@equal197.directlymortgagerates.org>", Result = PostProcessResult.Reject},
                new { Email = "Raven McCaw <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Rebates <systems@calcolivings.com>", Result = PostProcessResult.Accept},
                new { Email = "Rebecca Lowry <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Redbox <Redbox@em.redbox.com>", Result = PostProcessResult.Accept},
                new { Email = "Regal Crown Club News <news@crownclub.regmovies.com>", Result = PostProcessResult.Accept},
                new { Email = "Regus <info@contact.regus.com>", Result = PostProcessResult.Accept},
                new { Email = "Reindl Harald <h.reindl@thelounge.net>", Result = PostProcessResult.Accept},
                new { Email = "Remy Dee <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Restore.Hearing.Loss <Restore.Hearing.Loss@wonderfulislands.com>", Result = PostProcessResult.Reject},
                new { Email = "Restore.My.Blood.Sugar <Restore.My.Blood.Sugar@ayyildiz50.com>", Result = PostProcessResult.Reject},
                new { Email = "Restore.Vision <Restore.Vision@tugasapanhadas.com>", Result = PostProcessResult.Reject},
                new { Email = "RestoreYourHearing <restoration@restorehearing9.co.uk>", Result = PostProcessResult.Reject},
                new { Email = "Reverse my Tinnitus <ReversemyTinnitus@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Reverse Tinnitus <ReverseTinnitus@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "Reverse.Hearing.Loss <Reverse.Hearing.Loss@lookatprivateprofiles.com>", Result = PostProcessResult.Reject},
                new { Email = "Reverse.Hearing.Loss <Reverse.Hearing.Loss@travanda.com>", Result = PostProcessResult.Reject},
                new { Email = "Reverse-my-Tinnitus <Reverse-my-Tinnitus@honglinhjsc.com>", Result = PostProcessResult.Reject},
                new { Email = "RICHARD WILLIAMS<richardwilliams2037@careceo.com>", Result = PostProcessResult.Accept},
                new { Email = "Ricky German <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Roaman's <Roamans@mail.roamans.com>", Result = PostProcessResult.Accept},
                new { Email = "Robert Reich via MoveOn.org Civic Action <moveon-help@list.moveon.org>", Result = PostProcessResult.Accept},
                new { Email = "Rosewe <newsletter@rosewe.net> ", Result = PostProcessResult.Accept},
                new { Email = "Roth IRA <4700@naenlinea.com>", Result = PostProcessResult.Reject},
                new { Email = "Roxy Reckless-Burlyq <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Rue Lovett <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Runtastic Newsletter <info@runtastic.com>", Result = PostProcessResult.Accept},
                new { Email = "RW <rwmaillists@googlemail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ryan Anderson <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ryn Weston <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "SÃ¡ndor XÃ©nia <sandorcaeybq9xenia@ecbranding.com>", Result = PostProcessResult.Accept},
                new { Email = "Safeway just for U <safeway@email.safeway.com>", Result = PostProcessResult.Accept},
                new { Email = "Sailor St. Claire <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Sailor St. Claire <sailorstclaire@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Salem Octo <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Sara Dipity <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Sarah Charron via LinkedIn <notifications-noreply@linkedin.com>", Result = PostProcessResult.Accept},
                new { Email = "Sasha Summer Cousineau <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "SatelliteInternet <news@therocketnewsnow.com>", Result = PostProcessResult.Reject},
                new { Email = "SatelliteInternet <news@warriornewsforyou.com>", Result = PostProcessResult.Reject},
                new { Email = "Save with Toilet Paper Coupons <8803@bangladeshseller.com>", Result = PostProcessResult.Reject},
                new { Email = "Sawyer Moe <sawyermmoeburlycon@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Scarlet Celeste Tatro <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Scarlett O'Hairdye Storm <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Scarlette Revolver <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Scott B Randall <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Scott Madin (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Scott Ryan Bingham <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "SEAF 2015 <info@thefspc.org>", Result = PostProcessResult.Accept},
                new { Email = "Seamus McQualters <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Secret Alternate Banking System <AlternativeBankingSystem@result060.opennewbanking.org>", Result = PostProcessResult.Reject},
                new { Email = "Section 8 Housing <8736@gospelforindia.com>", Result = PostProcessResult.Reject},
                new { Email = "Section 8 Housing <8736@u2bdaili.com>", Result = PostProcessResult.Reject},
                new { Email = "Seraphina Fiero <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "service@paypal.com <service@paypal.com>", Result = PostProcessResult.Accept},
                new { Email = "Shana Deon <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Shana Graham <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Shanghai Pearl <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Shanna Marie Waite <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Shannon Hillinger <shannon@hillinger.org>", Result = PostProcessResult.Accept},
                new { Email = "Shantay <alicia_thomas@kythekufi.com>", Result = PostProcessResult.Accept},
                new { Email = "Sharon Kay <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Shelby Lynn Gerrath <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Siouxzie Hinton <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Siren SaintSin <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "SirMark Bruback <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Sita Mansour <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Skype <hello2@emails.skype.com>", Result = PostProcessResult.Accept},
                new { Email = "Sleep Apnea <SleepApnea@sleepisforu.com>", Result = PostProcessResult.Reject},
                new { Email = "Sleep Apnea <SleepApnea@tankwowforu.com>", Result = PostProcessResult.Reject},
                new { Email = "Sleep Apnea <SleepApnea@tightnightright.com>", Result = PostProcessResult.Reject},
                new { Email = "SleepApnea <news@hitthegymnews.com>", Result = PostProcessResult.Reject},
                new { Email = "SleepApnea <news@yourfastnewsnow.com>", Result = PostProcessResult.Reject},
                new { Email = "SmallBusinessLoan <news@centerforupdates.com>", Result = PostProcessResult.Reject},
                new { Email = "Smoothie Recipes <8192@acceleratedloansales.com>", Result = PostProcessResult.Reject},
                new { Email = "Smoothie Recipes <8192@syriainout.com>", Result = PostProcessResult.Reject},
                new { Email = "Snap by Groupon <email@e.snapsaves.com>", Result = PostProcessResult.Accept},
                new { Email = "Solar made easy <Solarmadeeasy@hilers.com>", Result = PostProcessResult.Reject},
                new { Email = "Solar Panels <solarpanels@calvaryhp.com>", Result = PostProcessResult.Reject},
                new { Email = "Sophie Maltease <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Southwest Airlines Rapid Rewards <RapidRewards@luv.southwest.com>", Result = PostProcessResult.Accept},
                new { Email = "Southwest Click 'n Save <SouthwestAirlines@luv.southwest.com>", Result = PostProcessResult.Accept},
                new { Email = "South-West SuperDays <Barton1@surveysforcards.com>", Result = PostProcessResult.Reject},
                new { Email = "Sparkle Leigh <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "SpringleafFinancialServices@springleaf.com", Result = PostProcessResult.Accept},
                new { Email = "Square <noreply@messaging.squareup.com>", Result = PostProcessResult.Accept},
                new { Email = "Stacy Bamer <hit-reply@linkedin.com>", Result = PostProcessResult.Accept},
                new { Email = "Star Stable <noreply@starstable.com>", Result = PostProcessResult.Accept},
                new { Email = "StartupBusiness <news@mistabustanews.com>", Result = PostProcessResult.Reject},
                new { Email = "StartupBusiness <news@poppernewstime.com>", Result = PostProcessResult.Reject},
                new { Email = "Steven Hasenbuhler <notification+iv161p1z@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Still A Problem (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Stormi Cliburn <mbyleuu@dalacom.kz>", Result = PostProcessResult.Accept},
                new { Email = "Styles Checks <styleschecks@styleschecks.rsys2.com>", Result = PostProcessResult.Accept},
                new { Email = "Summer Schief-Compean <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Sylvia Novak <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Tamara Trapeze Dover <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Tasha Sawyer <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Ted Ma <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Terrie Kyle <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "The Brain Stimulator <TheBrainStimulator@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "The Brain Stimulator Method <TheBrainStimulatorMethod@personsite.com>", Result = PostProcessResult.Reject},
                new { Email = "The Center for Women's Health <centerforwomenshealth@patient-message.com>", Result = PostProcessResult.Accept},
                new { Email = "The Doctors Carb Eliminator <thedoctorscarbeliminator@equal194.introducedietfor.org>", Result = PostProcessResult.Reject},
                new { Email = "The Gracious Body <thegraciousbody@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "The Petango Store <info@thepetangostore.com>", Result = PostProcessResult.Accept},
                new { Email = "They Might Be Giants <newsletter@tmbg.com>", Result = PostProcessResult.Accept},
                new { Email = "Thick Hair Full Volume", Result = PostProcessResult.Reject},
                new { Email = "Thinning with Rachel <Rachael4SizeDrop@vortex175.enoughfatsbustertype.org>", Result = PostProcessResult.Reject},
                new { Email = "Thrifty Car Rental <thriftycarrental@email.thrifty.com>", Result = PostProcessResult.Accept},
                new { Email = "Tiki Oasis Girl <tikioasisgirl@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Tinnitus Association <Oliver1@ttinituslit.com>", Result = PostProcessResult.Reject},
                new { Email = "Titania Teaseblossom <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Tootsie Spangles <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Tophatter <help@tophatter.com>", Result = PostProcessResult.Accept},
                new { Email = "Touch_fire Case and Keyboard <case@ferransales.com>", Result = PostProcessResult.Reject},
                new { Email = "Touch_fire Case and Keyboard <case@seeshelley.com>", Result = PostProcessResult.Reject},
                new { Email = "Touchfire <touchfireabc@marinichols.com>", Result = PostProcessResult.Reject},
                new { Email = "TrackR Bravo", Result = PostProcessResult.Reject},
                new { Email = "Tracy <Tracy@simplek12.com>", Result = PostProcessResult.Accept},
                new { Email = "Transgender Vacations <jland@dynamictravel.com>", Result = PostProcessResult.Accept},
                new { Email = "Trish Kitiera Morehead <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Tumblr <no-reply@tumblr.com>", Result = PostProcessResult.Accept},
                new { Email = "Twitter <info@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Twitter for Business <info@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "Two Big Blondes Plus Size Consignment <tbbplussize@gmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Undoing Memory Loss", Result = PostProcessResult.Reject},
                new { Email = "USAA Home_Rate_Drop <AmeriSaveAffordableHomeRate@result066.lessrefinance.org>", Result = PostProcessResult.Reject},
                new { Email = "Used Cars For Sale <5479@kasih2u.com>", Result = PostProcessResult.Reject},
                new { Email = "USSOLARDEPT <gov.panels@liferstimeds.biz>", Result = PostProcessResult.Reject},
                new { Email = "Vanessa Vivienne <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Veronica Michelle <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Vescha Lahearse <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Victoria Lacroix <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "ViewBug <donotreply@viewbug.com>", Result = PostProcessResult.Accept},
                new { Email = "Vincent Kovar <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Vincent<vic@hotmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Vision-Restored <Vision-Restored@notesalemarkets.com>", Result = PostProcessResult.Reject},
                new { Email = "Vita DeVoid <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Viva Valezz <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "VMware <vmwareteam@connect.vmware.com>", Result = PostProcessResult.Accept},
                new { Email = "VMware Online Events <vmwareteam@connect.vmware.com>", Result = PostProcessResult.Accept},
                new { Email = "WalkinTub <news@newsrunningman.com>", Result = PostProcessResult.Reject},
                new { Email = "WalkinTub <news@whosdrivingthetrain.com>", Result = PostProcessResult.Reject},
                new { Email = "Walk-inTub <WalkinTub@soscorepl.com>", Result = PostProcessResult.Reject},
                new { Email = "Walk-inTub <WalkinTub@throwbreak.com>", Result = PostProcessResult.Reject},
                new { Email = "Walk-inTub <walkintub@wdfmd.com>", Result = PostProcessResult.Reject},
                new { Email = "WebM.D. (Suffering from ED) <Owen@medicalmanshelp.com>", Result = PostProcessResult.Reject},
                new { Email = "WebProNews <Newsletter@webpronews.com>", Result = PostProcessResult.Accept},
                new { Email = "Weight Loss Resorts <8212@mkccambodia.com>", Result = PostProcessResult.Reject},
                new { Email = "Whisper DeCorvo Tristen Warner <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Wikia <community@wikia.com>", Result = PostProcessResult.Accept},
                new { Email = "William Chase Callaway <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "William Crim (via Twitter) <notify@twitter.com>", Result = PostProcessResult.Accept},
                new { Email = "William Sadorus <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Window.Discounts <Window.Discounts@frooglejews.com>", Result = PostProcessResult.Reject},
                new { Email = "WirelessInternet <news@infonewswar.com>", Result = PostProcessResult.Reject},
                new { Email = "WirelessInternet <news@nosoupfornews.com>", Result = PostProcessResult.Reject},
                new { Email = "WirelessInternet <news@raiseupthenews.com>", Result = PostProcessResult.Reject},
                new { Email = "WirelessInternet <news@whitestartnewstime.com>", Result = PostProcessResult.Reject},
                new { Email = "Women in Photography <groups-noreply@linkedin.com>", Result = PostProcessResult.Accept},
                new { Email = "woodworking materials <woodworkingmaterials@jopliuytuns.com>", Result = PostProcessResult.Reject},
                new { Email = "Yelp <no-reply@yelp.com>", Result = PostProcessResult.Accept},
                new { Email = "Yizzien Olonrae <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "YouTube <noreply@youtube.com>", Result = PostProcessResult.Accept},
                new { Email = "Zoe Brown <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Zoe Summers <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Zoey Paige Kay <notification+zf9toytc@facebookmail.com>", Result = PostProcessResult.Accept},
                new { Email = "Zora Phoenix <notification+zrdodv6cvfe1@facebookmail.com>", Result = PostProcessResult.Accept}
            };
            #endregion

            string reply = null;
            var output = new StringBuilder();

            foreach (var testSet in testEmailAddresses)
            {
                var postProcessResult = PostProcessResult.Accept;
                var msgData = new StringBuilder();

                BadHostPlugin.NoPoorlyChosenNames(null, "0.0.0.0", testSet.Email, msgData, ref postProcessResult, ref reply);

                if (postProcessResult != testSet.Result)
                {
                    if (output.Length == 0)
                    {
                        output.AppendLine("The following address/rule combinations failed:");
                    }

                    var log = string.Format("Test Value: {0}; Expected Result: {1}; Actual: {2}",
                        testSet.Email, testSet.Result, postProcessResult);
                    Logger.Log("Fail", log);
                    output.AppendLine(log);
                }
            }

            Expect(output.Length == 0, output.ToString());
        }

        [Test]
        public void GetCatalogPath()
        {
            var path = BadHostPlugin.CatalogPath;

            Expect(File.Exists(path), "Could not find " + path);
        }

        [Test]
        public void TestAddHeader()
        {
            var testMessage = GetTestMessage();

            BadHostPlugin.AddHeader(testMessage, "X-Test", "TestValue");

        }

        [Test]
        public void TestIpCatalogLoad()
        {
            var plugin = new BadHostPlugin();
            var catalog = BadHostPlugin.LoadCatalogFromDisk();

            Expect(catalog != null, "catalog is null");
            Expect(catalog.Count > 0, "catalog is empty");
        }

        [Test]
        public void ParseHosts()
        {
            var ip = new IpCatalogItem();
            const string format = "Expected \"{0}\", actual \"{1}\"";

            Expect(ip.HostName != null, "ip.HostName == null");
            Expect(ip.HostAliases != null, "ip.HostAliases == null");

            ip.ParseHosts("");
            Expect(ip.HostName == string.Empty, string.Format(format, "", ip.HostName));

            ip.ParseHosts("192.168.2.2");
            Expect(ip.HostName == "192.168.2.2", string.Format(format, "192.168.2.2", ip.HostName));

            const string testDomainDotOrg = "testdomain.org";

            ip.ParseHosts(testDomainDotOrg);
            Expect(!string.IsNullOrEmpty(ip.HostName), string.Format(format, testDomainDotOrg, ip.HostName));
            Expect(ip.HostAliases.Length == 0, "HostAliases.Length (" + ip.HostAliases.Length + ") != 0");

            ip.ParseHosts("testdomain.org;testalias1.com;testalias2.com;testalias3.com");
            Expect(!string.IsNullOrEmpty(ip.HostName), string.Format(format, testDomainDotOrg, ip.HostName));
            Expect(ip.HostAliases.Length == 3, "HostAliases.Length (" + ip.HostAliases.Length + ") != 3");

            for (var xx = 0; xx < 3; xx++)
            {
                var domain = "testalias" + (xx + 1) + ".com";
                Expect(ip.HostAliases[xx] == domain, "ip.HostAliases[" + xx + "] (" + ip.HostAliases[xx] + ") != \"" + domain + "\"");
            }

            ip.ParseHosts("testdomain.org;testalias1.com");
            Expect(!string.IsNullOrEmpty(ip.HostName), string.Format(format, testDomainDotOrg, ip.HostName));
            Expect(ip.HostAliases.Length == 1, "HostAliases.Length (" + ip.HostAliases.Length + ") != 1");
            var testDomain = "testalias1.com";
            Expect(ip.HostAliases[0] == testDomain, "ip.HostAliases[0] (" + ip.HostAliases[0] + ") != \"" + testDomain + "\"");
        }

        [Test]
        public void CheckHosts_GoodIps()
        {
            var postProcessResult = PostProcessResult.Accept;
            string reply = null;
            var isWhiteList = false;

            foreach (var ip in GoodIps)
            {
                BadHostPlugin.CheckHost(null, ip, ref postProcessResult, ref reply, ref isWhiteList);
                Expect(postProcessResult == PostProcessResult.Accept, string.Format("{0} was flagged as {1}, it should have been {2}",
                    ip, postProcessResult, PostProcessResult.Accept));
            }
        }

        [Test]
        public void CheckHosts_BadIps()
        {
            var postProcessResult = PostProcessResult.Accept;
            var isWhiteList = false;

            string reply = null;

            foreach (var ip in BadIps)
            {
                BadHostPlugin.CheckHost(null, ip, ref postProcessResult, ref reply, ref isWhiteList);
                Expect(postProcessResult == PostProcessResult.Reject, string.Format("{0} was flagged as {1}, not flagged as a blocked IP",
                    ip, postProcessResult));
            }
        }

        [Test]
        public void AttachedTrojans_SingleLine()
        {
            AttachedTrojans("Mail.w.Attachment.SingleLine.msg", PostProcessResult.Reject);
        }

        [Test]
        public void AttachedTrojans_SplitLine()
        {
            AttachedTrojans("Mail.w.Attachment.SplitLine.msg", PostProcessResult.Reject);
        }

        private void AttachedTrojans(string filePath, PostProcessResult expectedResult)
        {
            var msgData = GetTestMessage(filePath);

            PostProcessResult actualResult = expectedResult;
            string reply = null;

            BadHostPlugin.AttachedTrojans("AttachedTrojans", msgData, ref actualResult, ref reply);

            Expect(actualResult == expectedResult, "Tested \"" + filePath + "\"; Expected: " + expectedResult + "; Actual: " + actualResult);
        }

        private static StringBuilder GetTestMessage(string fileName)
        {
            var sb = new StringBuilder();

            var path = Path.Combine("..\\..\\TestData", fileName);

            sb.Append(File.ReadAllText(path));

            return sb;
        }

        private static StringBuilder GetTestMessage()
        {
            var sb = new StringBuilder();
            #region Build message
            sb.AppendLine("Received: from [104.245.103.60] by mail.clarkzoo.org (ArGoSoft Mail Server .NET v.1.0.8.6) with ESMTP (EHLO givengiftcardfree.rocks)");
            sb.AppendLine("	for <amber@clarkzoo.org>; Tue, 13 Jan 2015 20:28:24 -0800");
            sb.AppendLine("Date: Tue, 13 Jan 2015 21:26:28 -0700");
            sb.AppendLine("From: Amazon Online Coupon <amazononlinecoupon@givengiftcardfree.rocks>");
            sb.AppendLine("Salt-Less: 9bc012329cf60581d4cf4218d7f7e5353433433");
            sb.AppendLine("Mime-Version: 1.0");
            sb.AppendLine("To: <amber@clarkzoo.org>");
            sb.AppendLine("Im-Postor: 101637673433433");
            sb.AppendLine("Message-ID: <9bc012329cf60581d4cf4218d7f7e535.3433433.10163767@givengiftcardfree.rocks>");
            sb.AppendLine("Infi-Xion: 9bc012329cf60581d4cf4218d7f7e535");
            sb.AppendLine("Content-Type: multipart/alternative; boundary=\"3433433\"");
            sb.AppendLine("Subject: Your Amazon $50 Online Coupon expires 1/13/15.");
            sb.AppendLine("SPF-Received: pass");
            sb.AppendLine("X-FromIP: 104.245.103.60");
            sb.AppendLine("");
            sb.AppendLine("Lorem ipsum dolor sit amet, consectetur adipiscing elit. In eleifend condimentum ex quis finibus. Praesent lacinia eget odio eu blandit. Vivamus sagittis sodales ipsum, quis euismod erat viverra faucibus. In mattis in ipsum eget tempus. In rhoncus ipsum eu mattis consectetur. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nullam lacinia libero sit amet sapien ultrices dignissim. Morbi eu pulvinar leo, vel aliquam ante. Cras suscipit a tortor vel mattis. Maecenas odio lectus, vulputate in commodo et, lobortis a sem. Vivamus interdum euismod lectus dictum tristique. Etiam mollis consectetur quam at consectetur. Duis fringilla magna nec elit auctor semper. Suspendisse et consequat lectus. Nunc tempor dolor lacus, id tristique nulla euismod eget. Curabitur eget mauris dignissim purus blandit bibendum.");
            sb.AppendLine("");
            sb.AppendLine("Donec bibendum, libero ac sollicitudin viverra, ipsum est rutrum massa, a pretium orci lorem eu leo. Sed malesuada lorem ac dignissim sagittis. Suspendisse erat odio, aliquet eu dapibus sed, dictum in orci. Nam eu egestas magna. Quisque elementum nunc non pulvinar cursus. Ut facilisis ipsum eu quam rutrum iaculis. Vestibulum ac ipsum at ex sodales finibus. Phasellus tempus ac justo quis finibus. Ut eu dui lorem. Curabitur non nibh id justo elementum pharetra. Sed velit nisl, molestie vel nibh ut, egestas tempus dui. Fusce sit amet enim mi. Vivamus tempor feugiat risus et rhoncus. Proin cursus nibh eu magna dignissim, a malesuada ipsum efficitur. Sed ut augue vitae libero euismod semper pretium vitae est. Vivamus rutrum sed neque in luctus.");
            sb.AppendLine("");
            sb.AppendLine("Donec et suscipit quam. Fusce ac tellus massa. Nam fermentum imperdiet efficitur. Phasellus sit amet tellus ut turpis tristique rutrum ut eget tortor. Sed eget vulputate justo. Sed et varius massa, eu hendrerit lorem. Integer ac accumsan est. Pellentesque in tortor elit. Nulla rhoncus, felis nec tristique pretium, est ipsum finibus tellus, ut aliquam felis elit sit amet mi. Etiam maximus, arcu et mollis facilisis, erat odio consectetur ex, in vulputate lectus nisi sit amet sem. Aliquam consequat purus nulla, at rhoncus diam blandit sed. Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
            #endregion
            return sb;
        }
    }
}
