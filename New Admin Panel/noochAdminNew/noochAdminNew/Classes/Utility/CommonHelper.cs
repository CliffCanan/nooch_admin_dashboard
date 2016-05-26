using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Util;
using noochAdminNew.Classes.Crypto;
using noochAdminNew.Classes.ModelClasses;
using noochAdminNew.Classes.PushNotification;
using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Models;
using noochAdminNew.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace noochAdminNew.Classes.Utility
{
    public static class CommonHelper
    {
        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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
                if (sourceData.Length > 10)
                {
                    try
                    {
                        var aesAlgorithm = new AES();
                        string decryptedData = aesAlgorithm.Decrypt(sourceData.Replace(" ", "+"), string.Empty);
                        return decryptedData;
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("GetDecryptedData FAILED - [Source Data: " + sourceData + "]. Exception: [" + ex.InnerException + "]");
                    }
                }
                else
                {
                    Logger.Error("GetDecryptedData FAILED -> SourceData was too short - [SourceData: " + sourceData + "]");
                }
            }
            return string.Empty;
        }

        public static MemberNotification GetMemberNotificationSettings(string memberId)
        {
            using (var noochConnection = new NOOCHEntities())
            {
                Guid memId = Utility.ConvertToGuid(memberId);

                var memberNotifications = (from c in noochConnection.MemberNotifications where c.MemberId == memId select c).SingleOrDefault();

                return memberNotifications;
            }
        }

        public static Member GetMemberUsingGivenMemberId(string memberId)
        {
            using (var noochConnection = new NOOCHEntities())
            {
                Guid memId = Utility.ConvertToGuid(memberId);

                var memberNotifications = (from c in noochConnection.Members where c.MemberId == memId select c).SingleOrDefault();

                return memberNotifications;
            }
        }


        public static string GetMemberNameFromMemberId(string memberId)
        {
            using (var noochConnection = new NOOCHEntities())
            {
                Guid memId = Utility.ConvertToGuid(memberId);

                var memberNotifications = (from c in noochConnection.Members where c.MemberId == memId select c).SingleOrDefault();

                if (memberNotifications != null)
                {
                    return UppercaseFirst(GetDecryptedData(memberNotifications.FirstName)) + " " +
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

        public static string GetRandomTransactionTrackingId()
        {
            var random = new Random();
            int j = 1;
            using (var noochConnection = new NOOCHEntities())
            {
                for (int i = 0; i <= j; i++)
                {
                    var randomId = new string(
                        Enumerable.Repeat(Chars, 9)
                                  .Select(s => s[random.Next(s.Length)])
                                  .ToArray());

                    var transactionEntity = (from c in noochConnection.Transactions where c.TransactionTrackingId == randomId select c).FirstOrDefault();
                    if (transactionEntity == null)
                    {
                        return randomId;
                    }

                    j += i + 1;
                }
            }

            return null;
        }


        public static string GetRecentOrDefaultIPOfMember(Guid MemberIdPassed)
        {
            string RecentIpOfUser = "";
            using (var noochConnection = new NOOCHEntities())
            {

                var memberIP = (from c in noochConnection.MembersIPAddresses
                                where c.MemberId == MemberIdPassed
                                select c).OrderByDescending(m => m.ModifiedOn).FirstOrDefault();

                RecentIpOfUser = memberIP != null ? memberIP.Ip.ToString() : "54.201.43.89";
            }

            return RecentIpOfUser;
        }


        public static SynapseDetailsClass GetSynapseBankAndUserDetailsforGivenMemberId(string memberId)
        {
            SynapseDetailsClass res = new SynapseDetailsClass();

            try
            {
                var id = Utility.ConvertToGuid(memberId);

                using (NOOCHEntities noochConnection = new NOOCHEntities())
                {
                    // checking user details for given id

                    var createSynapseUserObj = (from c in noochConnection.SynapseCreateUserResults
                                                where c.MemberId == id &&
                                                      c.IsDeleted == false &&
                                                      c.success != null
                                                select c).FirstOrDefault();

                    if (createSynapseUserObj != null)
                    {
                        // This MemberId was found in the SynapseCreateUserResults DB
                        res.wereUserDetailsFound = true;
                        res.UserDetails = createSynapseUserObj;
                        res.UserDetailsErrMessage = "OK";
                    }
                    else
                    {
                        res.wereUserDetailsFound = false;
                        res.UserDetails = null;
                        res.UserDetailsErrMessage = "User synapse details not found.";
                    }

                    // Now get the user's bank account details
                    var defaultBank = (from c in noochConnection.SynapseBanksOfMembers
                                       where c.MemberId == id && c.IsDefault == true
                                       select c).FirstOrDefault();

                    if (defaultBank != null)
                    {
                        // Found a Synapse bank account for this user
                        res.wereBankDetailsFound = true;
                        res.BankDetails = defaultBank;
                        res.AccountDetailsErrMessage = "OK";
                    }
                    else
                    {
                        res.wereBankDetailsFound = false;
                        res.BankDetails = null;
                        res.AccountDetailsErrMessage = "User synapse bank not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Landlords WEB API -> GetSynapseBankAndUserDetailsforGivenMemberId FAILED - [MemberID: " + memberId + "], [Exception: " + ex + "]");
            }

            return res;
        }

        public static synapseSearchUserResponse getUserPermissionsForSynapseV3(string userEmail)
        {
            synapseSearchUserResponse res = new synapseSearchUserResponse();
            res.success = false;

            try
            {
                synapseSearchUserInputClass input = new synapseSearchUserInputClass();

                synapseSearchUser_Client client = new synapseSearchUser_Client();
                client.client_id = Utility.GetValueFromConfig("SynapseClientId");
                client.client_secret = Utility.GetValueFromConfig("SynapseClientSecret");

                synapseSearchUser_Filter filter = new synapseSearchUser_Filter();
                filter.page = 1;
                filter.exact_match = true; // we might want to set this to false to prevent error due to capitalization mis-match... (or make sure we only send all lowercase email when creating a Synapse user)
                filter.query = userEmail;

                input.client = client;
                input.filter = filter;

                string UrlToHit = Utility.GetValueFromConfig("Synapse_Api_User_Search");

                var http = (HttpWebRequest)WebRequest.Create(new Uri(UrlToHit));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                string parsedContent = JsonConvert.SerializeObject(input);
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                try
                {
                    var response = http.GetResponse();
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();

                    JObject checkPermissionResponse = JObject.Parse(content);
                    var successProperty = checkPermissionResponse.Property("success");

                    if (successProperty != null &&
                        (bool)successProperty.Value == true)
                    {
                        res = JsonConvert.DeserializeObject<synapseSearchUserResponse>(content);
                    }
                    else
                    {
                        res.error_code = "Service error.";
                    }
                }
                catch (WebException we)
                {
                    #region Synapse V3 Get User Permissions Exception

                    var httpStatusCode = ((HttpWebResponse)we.Response).StatusCode;
                    res.http_code = httpStatusCode.ToString();

                    var response = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                    JObject errorJsonFromSynapse = JObject.Parse(response);


                    res.error_code = errorJsonFromSynapse["error_code"].ToString();
                    res.errorMsg = errorJsonFromSynapse["error"]["en"].ToString();

                    if (!String.IsNullOrEmpty(res.error_code))
                    {
                        Logger.Error("Landlords API -> Common Helper -> getUserPermissionsForSynapseV3 FAILED - [Synapse Error Code: " + res.error_code +
                                               "], [Error Msg: " + res.errorMsg + "], [User Email: " + userEmail + "]");
                    }
                    else
                    {
                        Logger.Error("Landlords API -> Common Helper -> getUserPermissionsForSynapseV3 FAILED: Synapse Error, but *error_code* was null for [User Email: " +
                                               userEmail + "], [Exception: " + we.InnerException + "]");
                    }

                    #endregion Synapse V3 Get User Permissions Exception
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Landlords API -> Common Helper -> getUserPermissionsForSynapseV3 FAILED: Outer Catch Error - [User Email: " + userEmail +
                                       "], [Exception: " + ex.InnerException + "]");

                res.error_code = "Nooch Server Error: Outer Exception.";
            }

            return res;
        }

        public static NodePermissionCheckResult IsNodeActiveInGivenSetOfNodes(synapseSearchUserResponse_Node[] allNodes, string nodeToMatch)
        {
            NodePermissionCheckResult res = new NodePermissionCheckResult();

            res.IsPermissionfound = false;

            foreach (synapseSearchUserResponse_Node node in allNodes)
            {
                if (node._id != null && node._id.oid == nodeToMatch)
                {
                    if (!String.IsNullOrEmpty(node.allowed))
                    {
                        res.IsPermissionfound = true;
                        res.PermissionType = node.allowed;
                        break;
                    }
                }
            }

            return res;
        }

        public static SynapseV3AddTrans_ReusableClass AddTransSynapseV3Reusable(string sender_oauth, string sender_fingerPrint,
        string sender_bank_node_id, string amount, string fee, string receiver_oauth, string receiver_fingerprint,
        string receiver_bank_node_id, string suppID_or_transID, string senderUserName, string receiverUserName, string iPForTransaction, string senderLastName, string recepientLastName)
        {
            Logger.Info("Admin Common Helper-> AddTransSynapseV3Reusable Initiated - [Sender Username: " + senderUserName + "], " +
                        "[Recipient Username: " + receiverUserName + "], [Amount: " + amount + "]");

            SynapseV3AddTrans_ReusableClass res = new SynapseV3AddTrans_ReusableClass();
            res.success = false;

            try
            {
                bool SenderSynapsePermissionOK = false;
                bool RecipientSynapsePermissionOK = false;

                senderUserName = senderUserName.ToLower();
                receiverUserName = receiverUserName.ToLower();

                #region Check Sender Synapse Permissions

                // 1. Check USER permissions for SENDER
                synapseSearchUserResponse senderPermissions = getUserPermissionsForSynapseV3(senderUserName);

                if (senderPermissions == null || !senderPermissions.success)
                {
                    Logger.Error("Admin Common Helper -> AddTransSynapseV3Reusable - SENDER's Synapse Permissions were NULL or not successful :-(");

                    res.ErrorMessage = "Problem with senders synapse user permission.";
                    return res;
                }

                // 2. Check BANK/NODE permission for SENDER
                if (senderPermissions.users != null && senderPermissions.users.Length > 0)
                {
                    foreach (synapseSearchUserResponse_User senderUser in senderPermissions.users)
                    {
                        // iterating each node inside
                        if (senderUser.nodes != null && senderUser.nodes.Length > 0)
                        {
                            NodePermissionCheckResult nodePermCheckRes = IsNodeActiveInGivenSetOfNodes(senderUser.nodes, sender_bank_node_id);

                            if (nodePermCheckRes.IsPermissionfound == true)
                            {
                                if (nodePermCheckRes.PermissionType == "CREDIT-AND-DEBIT" || nodePermCheckRes.PermissionType == "DEBIT")
                                {
                                    SenderSynapsePermissionOK = true;
                                }
                            }
                        }
                    }
                }
                #endregion Check Sender Synapse Permissions

                #region Check Recipient Synapse Permissions

                // 3. Check USER permissions for RECIPIENT
                synapseSearchUserResponse recepientPermissions = getUserPermissionsForSynapseV3(receiverUserName);

                if (recepientPermissions == null || !recepientPermissions.success)
                {
                    Logger.Error("Admin Common Helper -> AddTransSynapseV3Reusable - RECIPIENT's Synapse Permissions were NULL or not successful :-(");

                    res.ErrorMessage = "Problem with recepient bank account permission.";
                    return res;
                }

                // 4. Check BANK/NODE permission for RECIPIENT
                if (recepientPermissions.users != null && recepientPermissions.users.Length > 0)
                {
                    foreach (synapseSearchUserResponse_User recUser in recepientPermissions.users)
                    {
                        // iterating each node inside
                        if (recUser.nodes != null && recUser.nodes.Length > 0)
                        {
                            NodePermissionCheckResult nodePermCheckRes = IsNodeActiveInGivenSetOfNodes(recUser.nodes, receiver_bank_node_id);

                            if (nodePermCheckRes.IsPermissionfound == true)
                            {
                                if (nodePermCheckRes.PermissionType == "CREDIT-AND-DEBIT" || nodePermCheckRes.PermissionType == "DEBIT")
                                {
                                    RecipientSynapsePermissionOK = true;
                                }
                            }
                        }
                    }
                }
                #endregion Check Recipient Synapse Permissions

                if (!SenderSynapsePermissionOK)
                {
                    res.ErrorMessage = "Sender bank permission problem.";
                    return res;
                }
                if (!RecipientSynapsePermissionOK)
                {
                    res.ErrorMessage = "Recipient bank permission problem.";
                    return res;
                }

                // all set...time to move money between accounts
                try
                {
                    #region Setup Synapse V3 Order Details

                    SynapseV3AddTransInput transParamsForSynapse = new SynapseV3AddTransInput();

                    SynapseV3Input_login login = new SynapseV3Input_login() { oauth_key = sender_oauth };
                    SynapseV3Input_user user = new SynapseV3Input_user() { fingerprint = sender_fingerPrint };
                    transParamsForSynapse.login = login;
                    transParamsForSynapse.user = user;

                    SynapseV3AddTransInput_trans transMain = new SynapseV3AddTransInput_trans();

                    SynapseV3AddTransInput_trans_from from = new SynapseV3AddTransInput_trans_from()
                    {
                        id = sender_bank_node_id,
                        type = "ACH-US"
                    };
                    SynapseV3AddTransInput_trans_to to = new SynapseV3AddTransInput_trans_to()
                    {
                        id = receiver_bank_node_id,
                        type = "ACH-US"
                    };
                    transMain.to = to;
                    transMain.from = from;

                    SynapseV3AddTransInput_trans_amount amountMain = new SynapseV3AddTransInput_trans_amount()
                    {
                        amount = Convert.ToDouble(amount),
                        currency = "USD"
                    };
                    transMain.amount = amountMain;

                    SynapseV3AddTransInput_trans_extra extraMain = new SynapseV3AddTransInput_trans_extra()
                    {
                        supp_id = suppID_or_transID,
                        // This is where we put the ACH memo (customized for Landlords, but just the same template for regular P2P transfers: "Nooch Payment {LNAME SENDER} / {LNAME RECIPIENT})
                        // maybe we should set this in whichever function calls this function because we don't have the names here...
                        // yes modifying this method to add 3 new parameters....sender IP, sender last name, recepient last name... this would be helpfull in keeping this method clean.
                        note = "NOOCH PAYMENT // " + senderLastName + " / " + recepientLastName, // + moneySenderLastName + " / " + requestMakerLastName, 
                        webhook = "",
                        process_on = 0, // this should be greater then 0 I guess... CLIFF: I don't think so, it's an optional parameter, but we always want it to process immediately, so I guess it should always be 0
                        ip = iPForTransaction // CLIFF:  This is actually required.  It should be the most recent IP address of the SENDER, or if none found, then '54.148.37.21'
                    };
                    transMain.extra = extraMain;

                    SynapseV3AddTransInput_trans_fees feeMain = new SynapseV3AddTransInput_trans_fees();

                    if (Convert.ToDouble(amount) > 10)
                    {
                        feeMain.fee = "0.20"; // to offset the Synapse fee so the user doesn't pay it
                    }
                    else if (Convert.ToDouble(amount) <= 10)
                    {
                        feeMain.fee = "0.10"; // to offset the Synapse fee so the user doesn't pay it
                    }
                    feeMain.note = "Negative Nooch Fee";

                    SynapseV3AddTransInput_trans_fees_to tomain = new SynapseV3AddTransInput_trans_fees_to()
                    {
                        id = "5618028c86c27347a1b3aa0f" // Temporary: ID of Nooch's SYNAPSE account (not bank account)... using temp Sandbox account until we get Production credentials
                    };

                    feeMain.to = tomain;
                    transMain.fees = new SynapseV3AddTransInput_trans_fees[1];
                    transMain.fees[0] = feeMain;

                    transParamsForSynapse.trans = transMain;

                    #endregion Setup Synapse V3 Order Details

                    #region Calling Synapse V3 TRANSACTION ADD

                    string UrlToHitV3 = Utility.GetValueFromConfig("Synapse_Api_Order_Add_V3");

                    try
                    {
                        // Calling Add Trans API

                        var http = (HttpWebRequest)WebRequest.Create(new Uri(UrlToHitV3));
                        http.Accept = "application/json";
                        http.ContentType = "application/json";
                        http.Method = "POST";

                        string parsedContent = JsonConvert.SerializeObject(transParamsForSynapse);
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        Byte[] bytes = encoding.GetBytes(parsedContent);

                        Stream newStream = http.GetRequestStream();
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();

                        var response = http.GetResponse();
                        var stream = response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var content = sr.ReadToEnd();

                        var synapseResponse = JsonConvert.DeserializeObject<SynapseV3AddTrans_Resp>(content);

                        if (synapseResponse.success == true ||
                            synapseResponse.success.ToString().ToLower() == "true")
                        {
                            res.success = true;
                            res.ErrorMessage = "OK";
                        }
                        else
                        {
                            res.success = false;
                            res.ErrorMessage = "Check synapse error.";
                        }
                        res.responseFromSynapse = synapseResponse;

                    }
                    catch (WebException ex)
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        JObject jsonFromSynapse = JObject.Parse(resp);

                        Logger.Error("Admin Common Helper -> AddTransSynapseV3Reusable FAILED. [Exception: " + jsonFromSynapse.ToString() + "]");

                        JToken token = jsonFromSynapse["error"]["en"];

                        if (token != null)
                        {
                            res.ErrorMessage = token.ToString();
                        }
                        else
                        {
                            res.ErrorMessage = "Error occured in call money transfer service.";
                        }
                    }

                    #endregion Calling Synapse V3 TRANSACTION ADD

                }
                catch (Exception ex)
                {
                    Logger.Error("Admin Common Helper -> AddTransSynapseV3Reusable FAILED - Inner Exception: [Exception: " + ex + "]");
                    res.ErrorMessage = "Server Error - TDA Inner Exception";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Common Helper -> AddTransSynapseV3Reusable FAILED - Outer Exception: [Exception: " + ex + "]");
                res.ErrorMessage = "TDA Outer Exception";
            }

            return res;
        }

        public static String ConvertImageURLToBase64(String url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                Logger.Info("MDA -> ConvertImageURLToBase64 Initiated - Photo URL is: [" + url + "]");

                StringBuilder _sb = new StringBuilder();

                Byte[] _byte = GetImage(url);

                _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));

                return _sb.ToString();
            }

            return "";
        }

        public static byte[] GetImage(string url)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    int len = (int)(response.ContentLength);
                    buf = br.ReadBytes(len);
                    br.Close();
                }

                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("MDA -> GetImage FAILED - Photo URL was: [" + url + "]. Exception: [" + ex + "]");
                buf = null;
            }

            return (buf);
        }

        public static synapseV3checkUsersOauthKey refreshSynapseV3OautKey(string oauthKey)
        {
            Logger.Info("Admin Common Helper -> refreshSynapseV3OautKey Initiated - User's Original OAuth Key (enc): [" + oauthKey + "]");

            synapseV3checkUsersOauthKey res = new synapseV3checkUsersOauthKey();
            res.success = false;

            try
            {
                //string oauthKeyEnc = CommonHelper.GetEncryptedData(oauthKey);
                // Checking user details for given MemberID
                using (NOOCHEntities noochConnection = new NOOCHEntities())
                {

                    SynapseCreateUserResult synCreateUserObject = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.access_token == oauthKey && m.IsDeleted == false);

                    // check for this is not needed.
                    // Will be calling login/refresh access token service to confirm if saved oAtuh token matches with token coming in response, if not then will update the token.
                    if (synCreateUserObject != null)
                    {
                        var noochMemberObject = GetMemberDetails(synCreateUserObject.MemberId.ToString());
                        var synapseCreateUserResult = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == synCreateUserObject.MemberId && m.IsDeleted == false);

                        //refreshToken = GetDecryptedData(refreshToken);
                        #region Found Refresh Token
                        Logger.Info("Admin Common Helper -> refreshSynapseV3OautKey - Found Member By Original OAuth Key (enc): [" + oauthKey + "]");

                        SynapseV3RefreshOauthKeyAndSign_Input input = new SynapseV3RefreshOauthKeyAndSign_Input();

                        string SynapseClientId = Utility.GetValueFromConfig("SynapseClientId");
                        string SynapseClientSecret = Utility.GetValueFromConfig("SynapseClientSecret");

                        input.login = new createUser_login2()
                        {
                            //email = GetDecryptedData(synCreateUserObject.NonNoochUserEmail), //why cant we get login feilds from members table ?
                            email = GetDecryptedData(noochMemberObject.UserName),
                            refresh_token = GetDecryptedData(synapseCreateUserResult.refresh_token)
                        };

                        input.client = new createUser_client()
                        {
                            client_id = SynapseClientId,
                            client_secret = SynapseClientSecret
                        };

                        SynapseV3RefreshOAuthToken_User_Input user = new SynapseV3RefreshOAuthToken_User_Input();

                        user._id = new synapseSearchUserResponse_Id1()
                        {
                            oid = synCreateUserObject.user_id
                        };
                        user.fingerprint = noochMemberObject.UDID1;

                        user.ip = GetRecentOrDefaultIPOfMember(noochMemberObject.MemberId);
                        input.user = user;

                        string UrlToHit = "";
                        UrlToHit = Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox")) ? "https://sandbox.synapsepay.com/api/v3/user/signin" : "https://synapsepay.com/api/v3/user/signin";


                        if (Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox")))
                        {
                            Logger.Info("Admin Common Helper -> refreshSynapseV3OautKey - TEST USER DETECTED - useSynapseSandbox is: [" +
                                        Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox")) + "] - About to ping Synapse Sandbox /user/refresh...");
                        }

                        var http = (HttpWebRequest)WebRequest.Create(new Uri(UrlToHit));
                        http.Accept = "application/json";
                        http.ContentType = "application/json";
                        http.Method = "POST";

                        string parsedContent = JsonConvert.SerializeObject(input);
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        Byte[] bytes = encoding.GetBytes(parsedContent);

                        Stream newStream = http.GetRequestStream();
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();

                        try
                        {
                            var response = http.GetResponse();
                            var stream = response.GetResponseStream();
                            var sr = new StreamReader(stream);
                            var content = sr.ReadToEnd();

                            //Logger.LogDebugMessage("Common Helper -> refreshSynapseV3OautKey Checkpoint #1 - About to parse Synapse Response");

                            synapseCreateUserV3Result_int refreshResultFromSyn = new synapseCreateUserV3Result_int();

                            refreshResultFromSyn = JsonConvert.DeserializeObject<synapseCreateUserV3Result_int>(content);

                            JObject refreshResponse = JObject.Parse(content);

                            Logger.Info("Common Helper -> synapseV3checkUsersOauthKey - Just Parsed Synapse Response: [" +
                                        refreshResponse + "]");

                            if (refreshResultFromSyn.success.ToString() == "true" || refreshResponse["success"] != null)
                            {
                                // checking if token is same as saved in db
                                if (synCreateUserObject.access_token ==
                                    GetEncryptedData(refreshResultFromSyn.oauth.oauth_key))
                                {
                                    // same as earlier..no change
                                    synCreateUserObject.access_token = GetEncryptedData(refreshResultFromSyn.oauth.oauth_key);
                                    synCreateUserObject.refresh_token = GetEncryptedData(refreshResultFromSyn.oauth.refresh_token);
                                    synCreateUserObject.expires_in = refreshResultFromSyn.oauth.expires_in;
                                    synCreateUserObject.expires_at = refreshResultFromSyn.oauth.expires_at;
                                }
                                else
                                {
                                    // changed.. time to update
                                    synCreateUserObject.access_token = GetEncryptedData(refreshResultFromSyn.oauth.oauth_key);
                                    synCreateUserObject.refresh_token = GetEncryptedData(refreshResultFromSyn.oauth.refresh_token);
                                    synCreateUserObject.expires_in = refreshResultFromSyn.oauth.expires_in;
                                    synCreateUserObject.expires_at = refreshResultFromSyn.oauth.expires_at;
                                }

                                int a = noochConnection.SaveChanges();

                                if (a > 0)
                                {
                                    Logger.Info(
                                        "Admin Common Helper -> refreshSynapseV3OautKey - SUCCESS From Synapse and Successfully added to Nooch DB - " +
                                        "Original Oauth Key (encr): [" + oauthKey + "], " +
                                        "Value for new, refreshed OAuth Key (encr): [" +
                                        synCreateUserObject.access_token + "]");

                                    res.success = true;
                                    res.oauth_consumer_key = synCreateUserObject.access_token;
                                    res.oauth_refresh_token = synCreateUserObject.refresh_token;
                                    res.user_oid = synCreateUserObject.user_id;
                                    res.msg = "Oauth key refreshed successfully";
                                }
                                else
                                {
                                    Logger.Error(
                                        "Admin Common Helper -> refreshSynapseV3OautKey FAILED - Error saving new key in Nooch DB - " +
                                        "Original Oauth Key: [" + oauthKey + "], " +
                                        "Value for new, refreshed OAuth Key: [" + synCreateUserObject.access_token + "]");

                                    res.msg = "Failed to save new OAuth key in Nooch DB.";
                                }
                            }
                            else
                            {
                                Logger.Error(
                                    "Admin Common Helper -> refreshSynapseV3OautKey FAILED - Error from Synapse service, no 'success' key found - " +
                                    "Original Oauth Key: [" + oauthKey + "]");
                                res.msg = "Service error.";
                            }
                        }
                        catch (WebException we)
                        {
                            #region Synapse V3 Signin/refresh Exception

                            var httpStatusCode = ((HttpWebResponse)we.Response).StatusCode;
                            string http_code = httpStatusCode.ToString();

                            var response = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                            JObject errorJsonFromSynapse = JObject.Parse(response);

                            string errorMsg = errorJsonFromSynapse["error"]["en"].ToString();

                            if (!String.IsNullOrEmpty(errorMsg))
                            {
                                Logger.Error("Admin Common Helper -> refreshSynapseV3OautKey WEBEXCEPTION - HTTP Code: [" + http_code +
                                    "], Error Msg: [" + errorMsg + "], Original Oauth Key (enc): [" + oauthKey + "]");
                                res.msg = "Webexception on refresh attempt: [" + errorMsg + "]";
                            }
                            else
                            {
                                Logger.Error(
                                    "Admin Common Helper -> refreshSynapseV3OautKey FAILED: Synapse Error, but *reason* was null for [Original Oauth Key (enc): " +
                                    oauthKey + "], [Exception: " + we.InnerException + "]");
                            }

                            #endregion Synapse V3 Signin/ refresh Exception
                        }

                        #endregion
                    }
                    else
                    {
                        // no record found for given oAuth token in synapse createuser results table
                        Logger.Error("Admin Common Helper -> refreshSynapseV3OautKey FAILED -  no record found for given oAuth key found - " +
                                     "Original Oauth Key: (enc) [" + oauthKey + "]");
                        res.msg = "Service error.";
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Common Helper -> refreshSynapseV3OautKey FAILED: Outer Catch Error - Original OAuth Key (enc): [" + oauthKey +
                              "], [Exception: " + ex + "]");

                res.msg = "Nooch Server Error: Outer Exception.";
            }

            return res;
        }


        public static Member GetMemberDetails(string memberId)
        {
            try
            {
                using (NOOCHEntities noochConnection = new NOOCHEntities())
                {

                    var id = Utility.ConvertToGuid(memberId);

                    var noochMember = noochConnection.Members.FirstOrDefault(m => m.MemberId == id && m.IsDeleted == false);

                    if (noochMember != null)
                    {
                        return noochMember;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Common Helper -> GetMemberDetails FAILED - Member ID: [" + memberId + "], [Exception: " + ex.Message + "]");
            }

            return new Member();
        }

        public static Member GetMemberDetailsByUsername(string email) // Should NOT already be encrypted
        {
            try
            {
                email = GetEncryptedData(email);
                var emailLowerCase = GetEncryptedData(email.ToLower());

                using (NOOCHEntities noochConnection = new NOOCHEntities())
                {
                    var noochMember = noochConnection.Members.FirstOrDefault(m => (m.UserName == email || m.UserNameLowerCase == email || m.SecondaryEmail == email) && m.IsDeleted == false);

                    if (noochMember != null)
                    {
                        return noochMember;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Common Helper -> GetMemberDetailsByUsername FAILED - Email: [" + email + "], [Exception: " + ex.Message + "]");
            }

            return null;
        }

        public static Member GetMemberDetailsByPhone(string phone)
        {
            try
            {
                phone = RemovePhoneNumberFormatting(phone); // Should already be formatted correctly, but just making sure.

                using (NOOCHEntities noochConnection = new NOOCHEntities())
                {
                    var noochMember = noochConnection.Members.FirstOrDefault(m => (m.ContactNumber == phone) && m.IsDeleted == false);

                    if (noochMember != null)
                    {
                        return noochMember;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Common Helper -> GetMemberDetailsByPhone FAILED - Phone: [" + phone + "], [Exception: " + ex.Message + "]");
            }

            return null;
        }




        public static List<Member> GetAllMembers()
        {
            // Logger.LogDebugMessage("MDA -> GetAllMembers");

            using (var noochConnection = new NOOCHEntities())
            {
                try
                {



                    return noochConnection.Members.Where(m => m.IsDeleted == false).ToList(); ;
                }
                catch (Exception ex)
                {
                    Logger.Error("CommonHelper -> GetAllMembers FAILED - [Exception: " + ex + "]");
                    return null;
                }
            }
        }



        public static List<Transaction> GetMemberTransactions(string MemberId, string TransactionType)
        {
            //Logger.LogDebugMessage("GetMemberTransactions - GetMemberTransactions[].");
            var id = Utility.ConvertToGuid(MemberId);
            var tranType = CommonHelper.GetEncryptedData(TransactionType);

            using (var noochConnection = new NOOCHEntities())
            {
                try
                {
                    if (TransactionType == "Request")
                    {


                        List<Transaction> list = noochConnection.Transactions.Where(t => t.TransactionType == "Request" && t.Member1.MemberId == id && t.TransactionStatus != "Cancelled")
                                .Distinct()
                                .OrderBy(o => o.TransactionDate)
                                .ToList();
                        return list;
                    }
                    else if (TransactionType == "Invite")
                    {
                        List<Transaction> list = noochConnection.Transactions.Where(t => t.TransactionType == "Invite" && t.Member1.MemberId == id && t.TransactionStatus != "Cancelled")
                                .Distinct()
                                .OrderBy(o => o.TransactionDate)
                                .ToList();
                        return list;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("CommonHelper -> GetMemberTransactions FAILED - [Exception: " + ex + "]");
                    return null;
                }
            }
            return null;
        }



        public static string SendTransactionReminderEmail(string ReminderType, string TransactionId, string MemberId)
        {
            Logger.Info("Common Helper -> SendTransactionReminderEmail Initiated. MemberID: [" + MemberId + "], " +
                                   "TransactionId: [" + TransactionId + "], " +
                                   "ReminderType: [" + ReminderType + "]");

            try
            {

                var TransId = Utility.ConvertToGuid(TransactionId);
                var MemId = Utility.ConvertToGuid(MemberId);
                using (NOOCHEntities noochConnection = new NOOCHEntities())
                {
                    if (ReminderType == "RequestMoneyReminderToNewUser" ||
                  ReminderType == "RequestMoneyReminderToExistingUser")
                    {
                        #region Requests - Both Types


                        var trans =
                            noochConnection.Transactions.FirstOrDefault(
                                t =>
                                    t.Member1.MemberId == MemId && t.TransactionId == TransId &&
                                    t.TransactionStatus == "Pending" && t.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw==");
                            

                        if (trans != null)
                        {
                            #region Setup Common Variables

                            string fromAddress = Utility.GetValueFromConfig("transfersMail");

                            string senderFirstName = UppercaseFirst(GetDecryptedData(trans.Member.FirstName));
                            string senderLastName = UppercaseFirst(GetDecryptedData(trans.Member.LastName));

                            string payLink = String.Concat(Utility.GetValueFromConfig("ApplicationURL"),
                                                           "Nooch/PayRequest?TransactionId=" + trans.TransactionId);

                            string s22 = trans.Amount.ToString("n2");
                            string[] s32 = s22.Split('.');

                            string memo = "";
                            if (trans.Memo != null && trans.Memo != "")
                            {
                                if (trans.Memo.Length > 3)
                                {
                                    string firstThreeChars = trans.Memo.Substring(0, 3).ToLower();
                                    bool startWithFor = firstThreeChars.Equals("for");

                                    if (startWithFor)
                                    {
                                        memo = trans.Memo;
                                    }
                                    else
                                    {
                                        memo = "For " + trans.Memo;
                                    }
                                }
                                else
                                {
                                    memo = "For " + trans.Memo;
                                }
                            }


                            bool isForRentScene = false;

                            if (trans.Member.MemberId.ToString().ToLower() == "852987e8-d5fe-47e7-a00b-58a80dd15b49") // Rent Scene's account
                            {
                                isForRentScene = true;
                                senderFirstName = "Rent Scene";
                                senderLastName = "";

                                payLink = payLink + "&rs=1";
                            }

                            #endregion Setup Common Variables


                            #region RequestMoneyReminderToNewUser

                            // Now check if this transaction was sent via Email or Phone Number (SMS)
                            if (trans.InvitationSentTo != null) // 'InvitationSentTo' field only used if it's an Email Transaction
                            {
                                #region If invited by email

                                string rejectLink = String.Concat(Utility.GetValueFromConfig("ApplicationURL"),
                                                                  "Nooch/RejectMoney?TransactionId=" + trans.TransactionId +
                                                                  "&UserType=U6De3haw2r4mSgweNpdgXQ==" +
                                                                  "&LinkSource=75U7bZRpVVxLNbQuoMQEGQ==" +
                                                                  "&TransType=T3EMY1WWZ9IscHIj3dbcNw==");

                                var tokens2 = new Dictionary<string, string>
                                {
								    {Constants.PLACEHOLDER_FIRST_NAME, senderFirstName + " " + senderLastName},
									{Constants.PLACEHOLDER_NEWUSER, ""},
									{Constants.PLACEHOLDER_TRANSFER_AMOUNT, s32[0]},
									{Constants.PLACEHLODER_CENTS, s32[1]},
									{Constants.PLACEHOLDER_REJECT_LINK, rejectLink},
									{Constants.PLACEHOLDER_TRANSACTION_DATE, Convert.ToDateTime(trans.TransactionDate).ToString("MMM dd yyyy")},
									{Constants.MEMO, memo},
									{Constants.PLACEHOLDER_PAY_LINK, payLink}
								};

                                var templateToUse = isForRentScene ? "requestReminderToNewUser_RentScene"
                                                                   : "requestReminderToNewUser";

                                var toAddress = CommonHelper.GetDecryptedData(trans.InvitationSentTo);

                                // Sending Request reminder email to Non-Nooch user
                                try
                                {
                                    Utility.SendEmail(templateToUse,  fromAddress, toAddress, 
                                                                senderFirstName + " " + senderLastName + " requested " + "$" + s22.ToString() + " - Reminder",
                                                                null, tokens2, null, null, null);

                                    Logger.Info("Common Helper -> SendTransactionReminderEmail - [" + templateToUse + "] sent to [" + toAddress + "] successfully.");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Common Helper -> SendTransactionReminderEmail - [" + templateToUse + "] NOT sent to [" + toAddress + "], Exception: [" + ex + "]");
                                }

                                return "Reminder mail sent successfully.";

                                #endregion If invited by email
                            }
                            else if (trans.IsPhoneInvitation == true && trans.PhoneNumberInvited != null)
                            {
                                #region If Invited by SMS

                                string RejectShortLink = "";
                                string AcceptShortLink = "";

                                string rejectLink = String.Concat(Utility.GetValueFromConfig("ApplicationURL"),
                                                                  "Nooch/RejectMoney?TransactionId=" + trans.TransactionId +
                                                                  "&UserType=U6De3haw2r4mSgweNpdgXQ==" +
                                                                  "&LinkSource=Um3I3RNHEGWqKM9MLsQ1lg==" +
                                                                  "&TransType=T3EMY1WWZ9IscHIj3dbcNw==");


                                #region Shortening URLs for SMS

                                string googleUrlAPIKey = Utility.GetValueFromConfig("GoogleURLAPI");

                                // Shorten the 'Pay' link
                                var cli = new WebClient();
                                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                                string response = cli.UploadString("https://www.googleapis.com/urlshortener/v1/url?key=" + googleUrlAPIKey, "{longUrl:\"" + rejectLink + "\"}");
                                googleURLShortnerResponseClass shortRejectLinkFromGoogleResult = JsonConvert.DeserializeObject<googleURLShortnerResponseClass>(response);

                                if (shortRejectLinkFromGoogleResult != null)
                                {
                                    RejectShortLink = shortRejectLinkFromGoogleResult.id;
                                }
                                else
                                {
                                    // Google short URL API broke...
                                    Logger.Error("Common Helper -> SendTransactionReminderEmail -> requestReceivedToNewUser Google short Reject URL NOT generated. Long Reject URL: [" + rejectLink + "].");
                                }

                                // Now shorten the 'Pay' link
                                cli.Dispose();
                                try
                                {
                                    var cli2 = new WebClient();
                                    cli2.Headers[HttpRequestHeader.ContentType] = "application/json";
                                    string response2 = cli2.UploadString("https://www.googleapis.com/urlshortener/v1/url?key=" + googleUrlAPIKey, "{longUrl:\"" + payLink + "\"}");
                                    googleURLShortnerResponseClass googlerejectshortlinkresult2 = JsonConvert.DeserializeObject<googleURLShortnerResponseClass>(response2);

                                    if (googlerejectshortlinkresult2 != null)
                                    {
                                        AcceptShortLink = googlerejectshortlinkresult2.id;
                                    }
                                    else
                                    {
                                        // Google short URL API broke...
                                        Logger.Error("Common Helper -> SendTransactionReminderEmail -> requestReceivedToNewUser Google short Pay URL NOT generated. Long Pay URL: [" + payLink + "].");
                                    }
                                    cli2.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Common Helper -> SendTransactionReminderEmail -> requestReceivedToNewUser Google short PAY URL NOT generated. Long Pay URL: [" + payLink + "].");
                                    return ex.ToString();
                                }

                                #endregion Shortening URLs for SMS


                                #region Sending SMS

                                try
                                {
                                    string SMSContent = "Just a reminder, " + senderFirstName + " " + senderLastName + " requested $" +
                                                          s32[0].ToString() + "." + s32[1].ToString() +
                                                          " from you using Nooch, a free app. Tap here to pay: " + AcceptShortLink +
                                                          ". Tap here to reject: " + RejectShortLink;

                                    Utility.SendSMS(GetDecryptedData(trans.PhoneNumberInvited), SMSContent, "");

                                    Logger.Info("Common Helper -> SendTransactionReminderEmail -> Request Reminder SMS sent to [" + CommonHelper.GetDecryptedData(trans.PhoneNumberInvited) + "].");

                                    return "Reminder sms sent successfully.";
                                }
                                catch (Exception)
                                {
                                    Logger.Error("Common Helper -> SendTransactionReminderEmail -> Request Reminder SMS NOT sent to [" + CommonHelper.GetDecryptedData(trans.PhoneNumberInvited) + "]. Problem occured in sending SMS.");
                                    return "Unable to send sms reminder.";
                                }

                                #endregion Sending SMS

                                #endregion If Invited by SMS
                            }

                            #endregion RequestMoneyReminderToNewUser


                            #region RequestMoneyReminderToExistingUser

                            else if (trans.Member.MemberId != null)
                            {
                                string rejectLink = String.Concat(Utility.GetValueFromConfig("ApplicationURL"), "Nooch/RejectMoney?TransactionId=" + TransId + "&UserType=mx5bTcAYyiOf9I5Py9TiLw==&LinkSource=75U7bZRpVVxLNbQuoMQEGQ==&TransType=T3EMY1WWZ9IscHIj3dbcNw==");
                                string paylink = "nooch://";

                                senderFirstName = UppercaseFirst(GetDecryptedData(trans.Member1.FirstName));
                                senderLastName = UppercaseFirst(GetDecryptedData(trans.Member1.LastName));

                                #region Reminder EMAIL

                                string toAddress = CommonHelper.GetDecryptedData(trans.Member.UserName);

                                var tokens2 = new Dictionary<string, string>
                                {
								    {Constants.PLACEHOLDER_FIRST_NAME, senderFirstName},
									{Constants.PLACEHOLDER_NEWUSER, UppercaseFirst(GetDecryptedData(trans.Member.FirstName))},
									{Constants.PLACEHOLDER_TRANSFER_AMOUNT,s32[0].ToString()},
									{Constants.PLACEHLODER_CENTS,s32[1].ToString()},
									{Constants.PLACEHOLDER_REJECT_LINK,rejectLink},
									{Constants.PLACEHOLDER_TRANSACTION_DATE,Convert.ToDateTime(trans.TransactionDate).ToString("MMM dd yyyy")},
									{Constants.MEMO, memo},
									{Constants.PLACEHOLDER_PAY_LINK,paylink}
								};

                                var templateToUse = isForRentScene ? "requestReminderToExistingUser_RentScene"
                                                                   : "requestReminderToExistingUser";

                                try
                                {
                                    Utility.SendEmail(templateToUse,  fromAddress,
                                        toAddress,  senderFirstName + " " + senderLastName + " requested " + "$" + s22.ToString() + " with Nooch - Reminder",
                                        null, tokens2, null, null, null);

                                    Logger.Info("Common Helper -> SendTransactionReminderEmail - [" + templateToUse + "] sent to [" + toAddress + "] successfully.");
                                }
                                catch (Exception)
                                {
                                    Logger.Error("TDA -> SendTransactionReminderEmail - [" + templateToUse + "] NOT sent to [" + toAddress + "]. Problem occured in sending mail.");
                                }

                                #endregion Reminder EMAIL

                                #region Reminder PUSH NOTIFICATION

                                if (!String.IsNullOrEmpty(trans.Member.DeviceToken) &&
                                    trans.Member.DeviceToken != "(null)")
                                {
                                    try
                                    {
                                        string firstName = (!String.IsNullOrEmpty(trans.Member.FirstName)) ? " " + UppercaseFirst(GetDecryptedData(trans.Member.FirstName)) : "";

                                        string msg = "Hey" + firstName + "! Just a reminder that " + senderFirstName + " " + senderLastName +
                                                     " sent you a Nooch request for $" + trans.Amount.ToString("n2") + ". Might want to pay up!";

                                        ApplePushNotification.SendNotificationMessage(msg, 1, null, trans.Member.DeviceToken,
                                                                                      Utility.GetValueFromConfig("AppKey"),
                                                                                      Utility.GetValueFromConfig("MasterSecret"));

                                        Logger.Info("Common Helper -> SendTransactionReminderEmail - (B/t 2 Existing Nooch Users) - Push notification sent successfully - [Username: " +
                                                               toAddress + "], [Device Token: " + trans.Member.DeviceToken + "]");
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("Common Helper -> SendTransactionReminderEmail - (B/t 2 Existing Nooch Users) - Push notification NOT sent - [Username: " +
                                                                toAddress + "], [Device Token: " + trans.Member.DeviceToken + "], [Exception: " + ex.Message + "]");
                                    }
                                }

                                #endregion  Reminder PUSH NOTIFICATION

                                return "Reminder mail sent successfully.";
                            }

                            #endregion RequestMoneyReminderToExistingUser

                            else
                            {
                                return "No recipient MemberId found for this transaction.";
                            }
                        }
                        else
                        {
                            Logger.Error("Common Helper -> SendTransactionReminderEmail FAILED - Could not find the Transaction. MemberID: [" + MemberId + "]. TransactionId: [" + TransactionId + "]");
                            return "No transaction found";
                        }

                        #endregion Requests - Both Types
                    }

                    else if (ReminderType == "InvitationReminderToNewUser")
                    {
                        #region InvitationReminderToNewUser

                        
                        var trans =
                            noochConnection.Transactions.FirstOrDefault(
                                t =>
                                    t.Member1.MemberId == MemId && t.TransactionId == TransId &&
                                    t.TransactionStatus == "Pending" &&
                                    (t.TransactionType == "DrRr1tU1usk7nNibjtcZkA==" ||
                                     t.TransactionType == "5dt4HUwCue532sNmw3LKDQ=="));
                            
                            

                        if (trans != null)
                        {
                            #region Setup Variables

                            string senderFirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(trans.Member.FirstName));
                            string senderLastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(trans.Member.LastName));

                            string linkSource = !String.IsNullOrEmpty(trans.PhoneNumberInvited) ? "Um3I3RNHEGWqKM9MLsQ1lg=="  // "Phone"
                                                                                                : "75U7bZRpVVxLNbQuoMQEGQ=="; // "Email"

                            string rejectLink = String.Concat(Utility.GetValueFromConfig("ApplicationURL"),
                                                              "trans/rejectMoney.aspx?TransactionId=" + trans.TransactionId +
                                                              "&UserType=U6De3haw2r4mSgweNpdgXQ==" +
                                                              "&LinkSource=" + linkSource +
                                                              "&TransType=DrRr1tU1usk7nNibjtcZkA==");

                            string acceptLink = String.Concat(Utility.GetValueFromConfig("ApplicationURL"),
                                                              "trans/depositMoney.aspx?TransactionId=" + trans.TransactionId.ToString());

                            string s22 = trans.Amount.ToString("n2");
                            string[] s32 = s22.Split('.');

                            string memo = "";
                            if (trans.Memo != null && trans.Memo != "")
                            {
                                if (trans.Memo.Length > 3)
                                {
                                    string firstThreeChars = trans.Memo.Substring(0, 3).ToLower();
                                    bool startWithFor = firstThreeChars.Equals("for");

                                    if (startWithFor)
                                    {
                                        memo = trans.Memo.ToString();
                                    }
                                    else
                                    {
                                        memo = "For " + trans.Memo.ToString();
                                    }
                                }
                                else
                                {
                                    memo = "For " + trans.Memo.ToString();
                                }
                            }

                            #endregion Setup Variables

                            if (trans.InvitationSentTo != null)
                            {
                                #region Invitation Was Sent By Email

                                var recipientEmail = CommonHelper.GetDecryptedData(trans.InvitationSentTo);

                                var tokens2 = new Dictionary<string, string>
												 {
													{Constants.PLACEHOLDER_FIRST_NAME, senderFirstName},
													{Constants.PLACEHOLDER_SENDER_FULL_NAME, senderFirstName + " " + senderLastName},
													{Constants.PLACEHOLDER_TRANSFER_AMOUNT, s32[0].ToString()},
													{Constants.PLACEHLODER_CENTS, s32[1].ToString()},
													{Constants.PLACEHOLDER_TRANSFER_REJECT_LINK, rejectLink},
													{Constants.PLACEHOLDER_TRANSACTION_DATE, Convert.ToDateTime(trans.TransactionDate).ToString("MMM dd yyyy")},
													{Constants.MEMO, memo},
													{Constants.PLACEHOLDER_TRANSFER_ACCEPT_LINK, acceptLink}
												 };
                                try
                                {
                                    string fromAddress = Utility.GetValueFromConfig("transfersMail");

                                    Utility.SendEmail("transferReminderToRecipient",  fromAddress,
                                                                recipientEmail, 
                                                                senderFirstName + " " + senderLastName + " sent you " + "$" + s22.ToString() + " - Reminder",
                                                                null, tokens2, null, null, null);

                                    Logger.Info("Common Helper -> SendTransactionReminderEmail -> Reminder email (Invite, New user) sent to [" + recipientEmail + "] successfully.");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Common Helper -> SendTransactionReminderEmail -> Reminder email (Invite, New user) NOT sent to [" + recipientEmail +
                                                           "], Exception: [" + ex + "]");
                                }

                                return "Reminder mail sent successfully.";

                                #endregion Invitation Was Sent By Email
                            }
                            else if (trans.IsPhoneInvitation == true &&
                                      String.IsNullOrEmpty(trans.InvitationSentTo) &&
                                     !String.IsNullOrEmpty(trans.PhoneNumberInvited))
                            {
                                #region Invitation Was Sent By SMS

                                #region Shortening URLs for SMS

                                string RejectShortLink = "";
                                string AcceptShortLink = "";

                                string googleUrlAPIKey = Utility.GetValueFromConfig("GoogleURLAPI");

                                var cli = new WebClient();
                                cli.Headers[HttpRequestHeader.ContentType] = "application/json";
                                string response = cli.UploadString("https://www.googleapis.com/urlshortener/v1/url?key=" + googleUrlAPIKey, "{longUrl:\"" + rejectLink + "\"}");

                                googleURLShortnerResponseClass googleRejectShortLinkResult = JsonConvert.DeserializeObject<googleURLShortnerResponseClass>(response);

                                if (googleRejectShortLinkResult != null)
                                {
                                    RejectShortLink = googleRejectShortLinkResult.id;
                                }
                                else
                                {
                                    // Google short URL link broke...
                                }

                                // Now shorten 'Accept' link
                                cli.Dispose();

                                try
                                {
                                    var cli2 = new WebClient();
                                    cli2.Headers[HttpRequestHeader.ContentType] = "application/json";
                                    string response2 = cli2.UploadString("https://www.googleapis.com/urlshortener/v1/url?key=" + googleUrlAPIKey, "{longUrl:\"" + acceptLink + "\"}");
                                    googleURLShortnerResponseClass googlerejectshortlinkresult2 = JsonConvert.DeserializeObject<googleURLShortnerResponseClass>(response2);

                                    if (googlerejectshortlinkresult2 != null)
                                    {
                                        AcceptShortLink = googlerejectshortlinkresult2.id;
                                    }
                                    else
                                    {
                                        // google short url link broke...
                                    }
                                    cli2.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    return ex.ToString();
                                }

                                #endregion Shortening URLs for SMS


                                #region Sending SMS
                                try
                                {
                                    string SMSContent = "Just a reminder, " + senderFirstName + " " + senderLastName + " wants to send you $" +
                                                          s32[0].ToString() + "." + s32[1].ToString() +
                                                          " using Nooch, a free app. Tap here accept: " + AcceptShortLink +
                                                          ". Or here to reject: " + RejectShortLink;

                                    var tophone = CommonHelper.GetDecryptedData(trans.PhoneNumberInvited);
                                    Utility.SendSMS(tophone, SMSContent, "");

                                    Logger.Info("Common Helper -> SendTransactionReminderEmail -> Reminder SMS (Invite, New user) sent to [" + CommonHelper.GetDecryptedData(trans.PhoneNumberInvited) + "] successfully.");

                                    return "Reminder sms sent successfully.";
                                }
                                catch (Exception)
                                {
                                    Logger.Error("Common Helper -> SendTransactionReminderEmail -> Reminder SMS (Invite, New user) NOT sent to [" + CommonHelper.GetDecryptedData(trans.PhoneNumberInvited) + "] Problem occured in sending sms.");
                                    return "Unable to send sms reminder.";
                                }
                                #endregion Sending SMS

                                #endregion Invitation Was Sent By SMS
                            }
                            else
                            {
                                return "no email mentioned for invited user";
                            }
                        }
                        else
                        {
                            return "No transaction found";
                        }

                        #endregion InvitationReminderToNewUser
                    }

                    else
                    {
                        return "invalid transaction id or memberid";
                    }
                }





            }
            catch (Exception ex)
            {
                Logger.Error("Common Helper -> SendTransactionReminderEmail FAILED - Outer Exception - [" + ex + "]");
                return "Error";
            }
        }


        // CANCEL transaction after 15 days check
        public static string CancelTransactionAfter15DaysWait(string TransactionId)
        {
            Guid TransId = Utility.ConvertToGuid(TransactionId);


            using (NOOCHEntities noochConnection = new NOOCHEntities())
            {
                
                var transResult = noochConnection.Transactions.FirstOrDefault(t => t.TransactionId == TransId);
                

                if (transResult != null)
                {
                    transResult.TransactionStatus = "Cancelled";
                    noochConnection.SaveChanges();
                    
                    return "1";
                }
                else
                {
                    return "0";
                }    
            }
            
            
        }

    }
}