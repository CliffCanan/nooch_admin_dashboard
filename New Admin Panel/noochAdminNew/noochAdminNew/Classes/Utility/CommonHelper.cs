using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using noochAdminNew.Classes.Crypto;
using noochAdminNew.Models;

namespace noochAdminNew.Classes.Utility
{
    public static class CommonHelper
    {
        public static string GetEncryptedData(string sourceData)
        {
            try
            {
                var aesAlgorithm = new AES();
                string encryptedData = aesAlgorithm.Encrypt(sourceData, string.Empty);
                return encryptedData.Replace(" ", "+");
            }
            catch (Exception ex)
            {
                Logger.Info("Admin Dash -> GetEncryptedData FAILED - [Source Data: " + sourceData + "]. Exception: [" + ex + "]");
            }
            return string.Empty;
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string GetDecryptedData(string sourceData)
        {
            if (!String.IsNullOrEmpty(sourceData))
            {
                try
                {
                    var aesAlgorithm = new AES();
                    string decryptedData = aesAlgorithm.Decrypt(sourceData.Replace(" ", "+"), string.Empty);
                    return decryptedData;
                }
                catch (Exception ex)
                {
                    Logger.Info("Admin Dash -> GetDecryptedData FAILED - [Source Data: " + sourceData + "]. Exception: [" + ex + "]");
                }
            }
            return string.Empty;
        }

        public static MemberNotification GetMemberNotificationSettings(string memberId)
        {
            using (var noochConnection = new NOOCHEntities())
            {
                Guid memId = Utility.ConvertToGuid(memberId);

                var memberNotifications = (from c in noochConnection.MemberNotifications where c.MemberId==memId select c).SingleOrDefault();

                return memberNotifications;
            }
        }


        public static string GetMemberNameFromMemberId(string memberId)
        {
            using (var noochConnection = new NOOCHEntities())
            {
                Guid memId = Utility.ConvertToGuid(memberId);

                var memberNotifications = (from c in noochConnection.Members where c.MemberId == memId select c).SingleOrDefault();

                if (memberNotifications!=null)
                {
                    return UppercaseFirst(GetDecryptedData(memberNotifications.FirstName)) + " "+
                    UppercaseFirst(GetDecryptedData(memberNotifications.LastName));
                }
                else
                {
                    return "";
                }
            }
        }

        public static string FormatPhoneNumber(string sourcePhone)
        {
            if (String.IsNullOrEmpty(sourcePhone) || sourcePhone.ToString().Length != 10)
            {
                return sourcePhone;
            }

            sourcePhone = "(" + sourcePhone;
            sourcePhone = sourcePhone.Insert(4, ")");
            sourcePhone = sourcePhone.Insert(5, " ");
            sourcePhone = sourcePhone.Insert(9, "-");

            return sourcePhone;
        }

        public static string RemovePhoneNumberFormatting(string sourceNum)
        {
            if (!String.IsNullOrEmpty(sourceNum))
            {
                // removing extra stuff from phone number
                sourceNum = sourceNum.Replace("(", "");
                sourceNum = sourceNum.Replace(")", "");
                sourceNum = sourceNum.Replace(" ", "");
                sourceNum = sourceNum.Replace("-", "");
                sourceNum = sourceNum.Replace("+", "");
            }
            return sourceNum;
        }
  
    }
}