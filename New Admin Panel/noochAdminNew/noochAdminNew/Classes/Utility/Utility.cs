using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace noochAdminNew.Classes.Utility
{
    public static class Utility
    {

        public static string GetValueFromConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static ServerProperties GetServerProperties()
        {
            var serverProperties = new ServerProperties
            {
                MailServerName = Utility.GetValueFromConfig("SMTPAddress")
            };
            bool authenticationRequired = Convert.ToBoolean(Utility.GetValueFromConfig("AuthRequired"));
            if (authenticationRequired)
            {
                serverProperties.IsAuthRequired = true;
                serverProperties.LogOn = Utility.GetValueFromConfig("SMTPLogOn");
                serverProperties.Password = Utility.GetValueFromConfig("SMTPPassword");
            }
            return serverProperties;
        }

        public static string SendNotificationMessage(string alertText, int badge, string sound, string devicetokens, string username, string password)
        {
            // sample json input
            // string json = "{\"aps\":{\"badge\":356,\"alert\":\"this 4 rd post\"},\"device_tokens\":[\"DC59F629CBAF8D88418C9FCD813F240B72311C6EDF27FAED0F5CB4ADB9F4D3C9\"]}";
            string json = new JavaScriptSerializer().Serialize(new
            {
                app_id = username,
                isIos = true,
                include_ios_tokens = new string[] { devicetokens },
                contents = new GameThriveContents() { en = alertText }
            });

            var cli = new WebClient();
            cli.Headers[HttpRequestHeader.ContentType] = "application/json";

            string response = cli.UploadString("https://gamethrive.com/api/v1/notifications", json);
            GameThriveResponseClass gamethriveresponse = JsonConvert.DeserializeObject<GameThriveResponseClass>(response);
            return "1";
        }



        public static string SendSMS(string phoneto, string msg,  string memberId)
        {
            try
            {
                if (phoneto.Substring(0, 3) != "555")
                {
                    string AccountSid = GetValueFromConfig("AccountSid");
                    string AuthToken = GetValueFromConfig("AuthToken");
                    string from = GetValueFromConfig("AccountPhone");
                    string to = "";

                    if (!phoneto.Trim().Contains("+"))
                        to = GetValueFromConfig("SMSInternationalCode") + phoneto.Trim();
                    else
                        to = phoneto.Trim();

                    var client = new Twilio.TwilioRestClient(AccountSid, AuthToken);
                    var sms= client.SendMessage(from, to, msg);

                    return sms.Status;
                }
                else
                {
                    Logger.Info("Utility-> Send SMS Aborted - Test Phone # Detected: [" + phoneto + "]");
                    return "Test phone number detected - SMS not sent";
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Utility -> SEND SMS FAILED - [To #: " + phoneto + "], [MemberID: " +
                                       memberId + "], [Exception: " + ex.InnerException + "]");
            }
            return "Failure";
        }


        public class GameThriveContents
        {
            public string en;
        }

        public class GameThriveResponseClass
        {
            public string id;
            public int recipients;
        }

        public static Guid ConvertToGuid(string value)
        {
            var id = new Guid();
            try
            {
                if (!String.IsNullOrEmpty(value) && value != Guid.Empty.ToString())
                {
                    id = new Guid(value);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + "Unable to format string :" + value);
                throw;
            }
            return id;
        }

        public static string GetEmailTemplate(string physicalPath)
        {
            using (var sr = new StreamReader(physicalPath))
                return sr.ReadToEnd();
        }

        public static bool SendEmail(string templateName, string fromAddress, string toAddress, string subject, string referenceLink, IEnumerable<KeyValuePair<string, string>> replacements, string ccMailIds, string bccMailIds, string bodyText)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();

                string template;
                string subjectString = subject;
                string content = string.Empty;

                if (!String.IsNullOrEmpty(templateName))
                {
                    template = GetEmailTemplate(String.Concat(Utility.GetValueFromConfig("EmailTemplatesPath"), templateName, ".txt"));
                    content = template;

                    // Replace tokens in the message body and subject line
                    if (replacements != null)
                    {
                        foreach (var token in replacements)
                        {
                            content = content.Replace(token.Key, token.Value);
                            subjectString = subject.Replace(token.Key, token.Value);
                        }
                    }
                    mailMessage.Body = content;
                }
                else
                {
                    mailMessage.Body = bodyText;
                }
                switch (fromAddress)
                {
                    case "receipts@nooch.com":
                        mailMessage.From = new MailAddress(fromAddress, "Nooch Receipts");
                        break;
                    case "support@nooch.com":
                        mailMessage.From = new MailAddress(fromAddress, "Nooch Support");
                        break;
                    default:
                        mailMessage.From = new MailAddress(fromAddress, "Nooch Admin");
                        break;
                }
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = subjectString;
                mailMessage.To.Add(toAddress);
                mailMessage.Priority = MailPriority.High;

                //---------------------------------
                ServerProperties serverProperties = GetServerProperties();

                SmtpClient smtpClient = new SmtpClient();

                NetworkCredential nc = new NetworkCredential(serverProperties.LogOn, serverProperties.Password);
                smtpClient.Host = serverProperties.MailServerName;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = nc;
                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Dash - Utility.cs -> SendEmail FAILED - [Exception: " + ex + "]");

                return false;
            }
        }
    }


    public class ServerProperties
    {
        public static int ChunkSize { get; set; }
        public bool IsAuthRequired { get; set; }
        public string LogOn { get; set; }
        public string MailServerName { get; set; }
        public string Password { get; set; }
        public static int ReceiverIdLimit { get; set; }
    }

}
