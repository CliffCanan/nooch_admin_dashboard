using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin;
using noochAdminNew.Classes.Utility;
using Owin;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Models;
using noochAdminNew.Classes.ModelClasses;
using System.Net;
using System.IO;
using System.Web.Util;
using noochAdminNew.Classes.PushNotification;
using noochAdminNew.Resources;
using Newtonsoft.Json.Linq;

[assembly: OwinStartup(typeof(noochAdminNew.Startup))]

namespace noochAdminNew
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            // setting up hangfire

            bool isRunningOnSandbox = Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox"));
            string connString = "";
            if (isRunningOnSandbox)
            {
                connString = Utility.GetValueFromConfig("HangFireSandboxConnectionString");
            }
            else
            {
                connString = Utility.GetValueFromConfig("HangFireProductionConnectionString");
            }
            //Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage(connString);

            //app.UseHangfireDashboard();
            //app.UseHangfireServer();

            //RecurringJob.AddOrUpdate(() => Logger.Info("Auto Task Running"), "0 12 * */2");
           // RecurringJob.AddOrUpdate(() => Logger.Info("Auto Task Running"), Cron.Minutely);

            //RecurringJob.AddOrUpdate(() => updateTransactionStatusService(), Cron.Daily);
            //RecurringJob.AddOrUpdate(() => notifyAdminOfBanksAwaitingVerification(), Cron.Daily);
            //RecurringJob.AddOrUpdate(() => NoochChecks(), Cron.Daily);
        }


        public void updateTransactionStatusService()
        {
            Logger.Info("Background Task in Startups.cs -> updateTransactionStatusService ---------------- Job Initiated at [" + DateTime.Now + "] ----------------");
            SynapseV3ShowTransInput transInput = new SynapseV3ShowTransInput();

            // Get Transaction Details
            using (var db = new NOOCHEntities())
            {
                var transactions = (from tr in db.Transactions
                                    join sa in db.SynapseAddTransactionResults
                                    on tr.TransactionId equals sa.TransactionId
                                    join syn in db.SynapseCreateUserResults
                                    on tr.SenderId equals syn.MemberId
                                    where tr.TransactionStatus == "Success"
                                    select new
                                    {
                                        tr,
                                        sa,
                                        syn
                                    }).ToList();

                if (transactions != null)
                {
                    Logger.Info("**********  [" + transactions.Count + "] TRANSACTIONS FOUND w/ TransactionStatus of 'Success'  **********");
                }

                foreach (var objT in transactions)
                {
                    Logger.Info("Startups.cs -> updateTransactionStatusService -> Checking Synapse status for TransID: [" + objT.tr.TransactionId + "]");

                    Transaction tran = objT.tr;
                    var usersSynapseOauthKey = "";

                    #region Check If OAuth Key Still Valid

                    synapseV3checkUsersOauthKey checkTokenResult = CommonHelper.refreshSynapseV3OautKey(objT.syn.access_token);

                    if (checkTokenResult != null)
                    {
                        if (checkTokenResult.success == true)
                        {
                            usersSynapseOauthKey = CommonHelper.GetDecryptedData(checkTokenResult.oauth_consumer_key);
                        }
                        else
                        {
                            Logger.Error("Daily Task from Startups.cs -> updateTransactionStatusService -> GetSynapseBankAndUserDetailsforGivenMemberId FAILED on Checking User's Synapse OAuth Token - " +
                                         "CheckTokenResult.msg: [" + checkTokenResult.msg + "], MemberID: [" + tran.SenderId + "]");
                            continue;
                        }
                    }
                    else
                    {
                        Logger.Error("Daily Task from Startups.cs -> updateTransactionStatusService -> GetSynapseBankAndUserDetailsforGivenMemberId FAILED on Checking User's Synapse OAuth Token - " +
                                     "CheckTokenResult was NULL, MemberID: [" + tran.SenderId + "]");
                        continue;
                    }

                    #endregion Check If OAuth Key Still Valid


                    SynapseV3TransInput_login login = new SynapseV3TransInput_login() { oauth_key = usersSynapseOauthKey };
                    SynapseV3TransInput_user user = new SynapseV3TransInput_user() { fingerprint = tran.Member.UDID1.ToString() };

                    SynapseV3ShowTransInput_filter_Trans_id oid = new SynapseV3ShowTransInput_filter_Trans_id();
                    oid.oid = objT.sa.OidFromSynapse;

                    SynapseV3ShowTransInput_filter filter = new SynapseV3ShowTransInput_filter();
                    filter._id = oid;
                    filter.page = "1";

                    transInput.login = login;
                    transInput.user = user;
                    transInput.filter = filter;

                    string baseAddress = Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox")) ? "https://sandbox.synapsepay.com/api/v3/trans/show" : "https://synapsepay.com/api/v3/trans/show";

                    try
                    {
                        var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                        http.Accept = "application/json";
                        http.ContentType = "application/json";
                        http.Method = "POST";

                        string parsedContent = JsonConvert.SerializeObject(transInput);
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        Byte[] bytes = encoding.GetBytes(parsedContent);

                        Stream newStream = http.GetRequestStream();
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();

                        var response = http.GetResponse();
                        var stream = response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var content = sr.ReadToEnd();
                        JObject jsonFromSynapse = JObject.Parse(content);

                        if (jsonFromSynapse["success"].ToString().ToLower() == "true")
                        {
                            if (jsonFromSynapse["trans"].Count() > 0)
                            {
                                // Update transaction table in db

                                tran.SynapseStatus = jsonFromSynapse["trans"][0]["recent_status"]["status"].ToString();

                                db.SaveChanges();
                            }
                            else
                            {
                                tran.SynapseStatus = "";
                                db.SaveChanges();
                                Logger.Info("Daily Task from Startups.cs -> updateTransactionStatusService -> response from showTransactioFromSynapseV3 is false for transaction - [transactionID: " + tran.TransactionId + "]");
                            }
                        }
                        else
                        {
                            Logger.Info("Daily Task from Startups.cs -> updateTransactionStatusService -> response from showTransactioFromSynapseV3 is null for transaction - [transactionID: " + tran.TransactionId + "]");
                        }
                    }
                    catch (WebException ex)
                    {
                        Logger.Error("Daily Task from Startups.cs -> updateTransactionStatusService -> updateTransactionStatusService FAILED - [MemberID: " + tran.SenderId + "], Exception: [" + ex.Message + "]");
                    }
                }
            }
        }


        public void notifyAdminOfBanksAwaitingVerification()
        {
            Logger.Info("Daily Task from Startups.cs -> notifyAdminOfBanksAwaitingVerification -> Banks Awaiting Verification Check Initiated");

            using (var noochConnection = new NOOCHEntities())
            {
                try
                {
                    // Get All Active, Non-Verified Synapse Banks
                    var nonVerifiedBanks = (from c in noochConnection.SynapseBanksOfMembers
                                            where c.IsDefault == true &&
                                                 (c.Status == "Not Verified" || c.Status == "Pending Review") && // Bank status would be 'Pending Review' if they have submitted ID documentation inside the Nooch app.
                                                 (c.name_on_account != "VsubAZ/orNwSN1SiJ/TEPQ==" && // "Test User"
                                                  c.email != "test@synapsepay.com")
                                            select c).ToList();

                    if (nonVerifiedBanks != null)
                    {
                        if (nonVerifiedBanks.Count > 0)
                        {
                            Logger.Info("Daily Task from Startups.cs -> notifyAdminOfBanksAwaitingVerification -> DAILY SCAN -> Found [" + nonVerifiedBanks.Count + "] Non-Verified Banks");

                            StringBuilder st = new StringBuilder();

                            st.Append("<table cellpadding='3' border='1' style='border-collapse:collapse;text-align:center;'>" +
                                        "<tr><th>Name</th>" +
                                        "<th>Nooch_Id</th>" +
                                        "<th>Bank</th>" +
                                        "<th>Name From Bank</th>" +
                                        "<th>Bank ID</th>" +
                                        "<th>MFA Verified?</th>" +
                                        "<th>SSN Verified?</th>" +
                                        "<th>Date Added</th></tr>");

                            foreach (var bank in nonVerifiedBanks)
                            {
                                // For each Non-Verified bank found, get the user's Member Details (name, etc)
                                var memGuid = Utility.ConvertToGuid(bank.MemberId.ToString());

                                var memberFound = (from c in noochConnection.Members
                                                   where c.MemberId == memGuid &&
                                                         c.IsDeleted == false
                                                   select c).FirstOrDefault();

                                if (memberFound != null)
                                {
                                    var usersFullName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(memberFound.FirstName)) + " " +
                                                        CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(memberFound.LastName));
                                    var usersNoochId = memberFound.Nooch_ID;
                                    var usersType = memberFound.Type;
                                    var userSsnVerified = memberFound.IsVerifiedWithSynapse != null
                                                          ? memberFound.IsVerifiedWithSynapse.ToString()
                                                          : "<small>null</small>";

                                    var memberDetailsUrl = "https://noochme.com/noochnewadmin/Member/Detail?NoochId=" + usersNoochId;

                                    st.Append("<tr>");
                                    st.Append("<td>" + usersFullName + "<br/><small style='text-transform:uppercase;'>" + usersType + "</small></td>");
                                    st.Append("<td><small>" + usersNoochId + "</small></td>");
                                    st.Append("<td><strong>" + CommonHelper.GetDecryptedData(bank.bank_name) + "</strong><br/>" +
                                              "<small>" + CommonHelper.GetDecryptedData(bank.nickname) + "</small></td>");
                                    st.Append("<td>" + CommonHelper.GetDecryptedData(bank.name_on_account) + "</td>");
                                    st.Append("<td>" + bank.bankid + "<br/><small><em>" + bank.Status + "</em></small></td>");
                                    st.Append("<td>" + bank.mfa_verifed + "</td>");
                                    st.Append("<td>" + userSsnVerified + "</td>");
                                    st.Append("<td>" + Convert.ToDateTime(bank.AddedOn).ToString("MM/dd/yyyy") + "</td>");
                                    st.Append("</tr>");
                                }
                            }
                            st.Append("</table>");

                            StringBuilder completeEmailTxt = new StringBuilder();
                            string s = "<html><body><h2>Non-Verified Syanpse V3 Bank Accounts</h2><p>The following <strong>[" + nonVerifiedBanks.Count +
                                       "]</strong> Nooch users have attached a Synapse bank account that is awaiting verification:</p>"
                                       + st.ToString() +
                                       "<br/><br/><small>This email was generated automatically during a daily scan of all Nooch users.</small></body></html>";

                            completeEmailTxt.Append(s);

                            #region Send Email To Nooch Admin

                            try
                            {
                                var fromAddress = Utility.GetValueFromConfig("transfersMail");

                                bool b = Utility.SendEmail(null, fromAddress, "NonVerifiedBanks@nooch.com",
                                        "Nooch Admin: Non-Verified V3 Banks List", null, null, null, null,
                                         completeEmailTxt.ToString());

                                Logger.Info("Daily Task from Startups.cs ->  notifyAdminOfBanksAwaitingVerification -> Banks Awaiting Verification Check -> Email sent to [NonVerifiedBanks@nooch.com] successfully.");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("Daily Task from Startups.cs -> notifyAdminOfBanksAwaitingVerification -> Banks Awaiting Verification Check FAILED -> Email NOT sent to [NonVerifiedBanks@nooch.com]. Exception: [" + ex + "]");
                            }

                            #endregion Send Email To Nooch Admin
                        }
                    }
                }
                catch (Exception)
                {
                    Logger.Error("Daily Task from Startups.cs -> notifyAdminOfBanksAwaitingVerification -> Banks Awaiting Verification Check FAILED (Outer)");// - Exception: [" + ex + "]");
                }
            }
        }



        public bool NoochChecks()
        {
            Logger.Info("*****  DAILY SCAN -> NoochChecks Initiated  *****");


            List<Member> membersList = CommonHelper.GetAllMembers();

            string adminUserName = CommonHelper.GetEncryptedData(Utility.GetValueFromConfig("transfersMail"));

            #region For Each Member Loop

            foreach (Member mem in membersList)
            {
                if (mem.Type == "Personal")
                {
                    try
                    {
                        //-------------------------- Incomplete Nooch Profile Push Notification ---------------------
                        #region Incomplete Profile Check

                        if ((mem.Status == "Registered" || mem.Status == "Active") &&    // If user isn't suspended, blocked
                            (string.IsNullOrEmpty(mem.Address) || mem.IsVerifiedPhone == false) &&  // If they haven't entered an address yet or verified a phone number
                            mem.IsDeleted == false)
                        {
                            var days = (DateTime.Now - mem.DateCreated.Value).TotalDays;

                            // SEND PUSH NOTIFICATION REMINDER
                            // Cliff (5/25/16): Commenting out this block b/c Push Notifications are broken until we submit an app update to Apple
                            /*string pushMsgText = "Quick reminder: you signed up for Nooch but have not completed your profile. Open Nooch to complete your profile and start sending money for FREE!";
                            
                             * if ((days >= 3.0 && days < 4.0) || (days >= 10.0 && days < 11.0))
                            {
                                if (!String.IsNullOrEmpty(mem.DeviceToken))
                                {
                                    ApplePushNotification.SendNotificationMessage(pushMsgText, 1, null, mem.DeviceToken, Utility.GetValueFromConfig("AppKey"), Utility.GetValueFromConfig("MasterSecret"));
                                    Logger.Info("Reminder Incomplete Profile Status - Push notification sent to [" + mem.DeviceToken + "].");
                                }
                            }*/

                            // SEND EMAIL REMINDER
                            if ((days >= 7.0 && days < 8.0) || (days >= 20.0 && days < 21.0) || (days >= 49.0 && days < 50.0))
                            {
                                var fromAddress = Utility.GetValueFromConfig("adminMail");
                                var toAddress = CommonHelper.GetDecryptedData(mem.UserName);
                                var tokens2 = new Dictionary<string, string>
                                                 {
                                                     {Constants.PLACEHOLDER_FIRST_NAME, CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(mem.FirstName))}
                                                 };

                                try
                                {
                                    Utility.SendEmail("profileIncomplete", fromAddress, toAddress,
                                                      "Complete your Nooch account - stop depending on cash",
                                                      null, tokens2, null, null, null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Reminder Incomplete Profile Status - Email NOT Sent - [Exception: " + ex + "]");
                                }
                            }
                        }

                        #endregion Incomplete Profile Check


                        //--------------------------  Request Waiting  ------------------------------
                        if (mem.Status == "Active" && mem.IsDeleted == false)
                        {
                            List<Transaction> transactionList = CommonHelper.GetMemberTransactions(mem.MemberId.ToString(), "Request");

                            if (transactionList.Count > 0)
                            {
                                #region Loop Through Each Transaction

                                foreach (Transaction trans in transactionList)
                                {
                                    #region Send Request Reminders

                                    if (trans.TransactionStatus == "Pending")
                                    {
                                        string requestorFirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(mem.FirstName));

                                        string msgToRecipient = "Hey! Just a reminder that " + requestorFirstName + " sent you a request for $" + trans.Amount.ToString("n2") + " on Nooch. Might want to pay up!";

                                        double days = (DateTime.Now - trans.TransactionDate.Value).TotalDays;

                                        #region P2P Transactions

                                        #region Reminder After 3, 8, 16 Days

                                        if ((days > 3.0 && days < 4.0) || (days > 8.0 && days < 9.0) || (days > 16.0 && days < 17.0))
                                        {

                                            // Check if it's a Request to Existing or Non-Nooch User

                                            if (!String.IsNullOrEmpty(trans.InvitationSentTo) ||
                                                trans.IsPhoneInvitation == true)
                                            {
                                                if (trans.IsPhoneInvitation == true)
                                                {
                                                    Logger.Info("Global -> 7 Day Old Pending Request To NON-NOOCH User Found - Sending to TDA to send reminder SMS to request recipient - [Phone #: " + trans.PhoneNumberInvited + "], [TransID: " + trans.TransactionId.ToString() + "]");
                                                }
                                                else
                                                {
                                                    Logger.Info("Global -> 7 Day Old Pending Request To NON-NOOCH User Found - Sending to TDA to send reminder email to request recipient - [" + CommonHelper.GetDecryptedData(trans.InvitationSentTo) + "], [TransID: " + trans.TransactionId.ToString() + "]");
                                                }

                                                // Request to a Non-Nooch User - Send Reminder EMAIL or SMS (the TDA function handles checking for which type)
                                                CommonHelper.SendTransactionReminderEmail("RequestMoneyReminderToNewUser", trans.TransactionId.ToString(), mem.MemberId.ToString());
                                            }
                                            else
                                            {
                                                // Should be a regular request between 2 existing users - Send Reminder Email and Push to Recipient (who will pay/reject the request)

                                                Logger.Info("Global -> 7 Day Old Pending Request To EXISTING User Found - Sending to TDA to send reminder email - [TransID: " + trans.TransactionId + "]");
                                                CommonHelper.SendTransactionReminderEmail("RequestMoneyReminderToExistingUser", trans.TransactionId.ToString(), mem.MemberId.ToString());
                                            }
                                        }

                                        #endregion Reminder After 3, 8, 16 Days

                                        #region Cancel After 20 Days

                                        // Cancelling request if not reponded in 21 days
                                        if (days > 21.0 && trans.InvitationSentTo != null)
                                        {

                                            string res = CommonHelper.CancelTransactionAfter15DaysWait(trans.TransactionId.ToString());

                                            if (res == "1")
                                            {
                                                #region Push Notification

                                                if (!String.IsNullOrEmpty(mem.DeviceToken))
                                                {
                                                    try
                                                    {
                                                        ApplePushNotification.SendNotificationMessage(msgToRecipient, 1, null, mem.DeviceToken,
                                                                                                      Utility.GetValueFromConfig("AppKey"),
                                                                                                      Utility.GetValueFromConfig("MasterSecret"));

                                                        Logger.Info("Global -> Request Waiting - Push notification sent to [" + mem.DeviceToken + "].");
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Logger.Error("Global -> Request Waiting - Push notification NOT sent to [" + mem.DeviceToken + "],  [Exception: " + ex.Message + "]");
                                                    }
                                                }

                                                #endregion Push Notification

                                                #region Email Notification

                                                string toAddress = CommonHelper.GetDecryptedData(mem.UserName);
                                                var fromAddress = Utility.GetValueFromConfig("adminMail");

                                                string TransactionAmount = trans.Amount.ToString("n2");
                                                string[] TransactionAmountSplitted = TransactionAmount.Split('.');

                                                string InvitationSentTo = (CommonHelper.GetDecryptedData(trans.InvitationSentTo).ToString());

                                                var tokens = new Dictionary<string, string>
												 {
													{Constants.PLACEHOLDER_FIRST_NAME, requestorFirstName},
													{Constants.PLACEHOLDER_NEWUSER,InvitationSentTo},
													{Constants.PLACEHOLDER_TRANSFER_AMOUNT,TransactionAmountSplitted[0].ToString()},
													{Constants.PLACEHLODER_CENTS,TransactionAmountSplitted[1].ToString()}
												 };

                                                try
                                                {
                                                    Utility.SendEmail("transferUnclaimedSender", fromAddress, toAddress,
                                                        "Your Nooch request to " + InvitationSentTo + " has been cancelled",
                                                        null, tokens, null, null, null);

                                                    Logger.Info("Global -> TransferUnclaimedSender email sent to [" + toAddress + "] successfully");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Error("Global -> TransferUnclaimedSender email NOT sent to [" + toAddress + "],  [Exception: " + ex + "]");
                                                }

                                                #endregion Email Notification
                                            }
                                        }

                                        #endregion Cancel After 20 Days

                                        #endregion P2P Transactions


                                        #region Rent Transactions


                                        #endregion Rent Transactions
                                    }

                                    #endregion Send Request Reminders
                                }

                                #endregion Loop Through Each Transaction
                            }

                            #region Automatically Cancel any Pending Requests that older than X days
                            //To check whether the NON-Nooch member has accepted a transfer or not. If the transaction date is greater than 15 days then send this message..
                            //For the sender, the transfer would appear as "Pending" (in their History screen) until the Recipient claims the money or 15 days pass.
                            //If the recipient never claims the money, the sender's Nooch account should be credited back the amount.

                            /*--------------------------------------------------------------------------------------------------------
                                 THIS IS INCOMPLETE, STILL NEED TO ADD CORRECT PLACEHOLDER LINK VALUES AND OTHER TRANSACTION INFO
                            /*--------------------------------------------------------------------------------------------------------*/
                            //List<Transactions> pendingtransactionList = obj.GetMemberTransactions(mem.MemberId.ToString(), Constants.TRANSACTION_TYPE_INVITE);
                            //if (pendingtransactionList != null && pendingtransactionList.Count > 0)
                            //{
                            //    foreach (Transactions trans in pendingtransactionList)
                            //    {
                            //        if (trans.TransactionStatus == "Pending")
                            //        {
                            //            daysSinceTransfer = (DateTime.Now - trans.TransactionDate.Value).TotalDays;
                            //            if ((daysSinceTransfer >= 30.1))
                            //            {
                            //                // Automatically set the transactionStatus field as cancelled
                            //                trans.TransactionStatus = "Cancelled";

                            //                // Update the transaction status in db
                            //                 transactionRepository.UpdateEntity(trans);

                            //                var fromAddress = Utility.GetValueFromConfig("adminMail");
                            //                var toAddress = CommonHelper.GetDecryptedData(mem.UserName);
                            //                try
                            //                {
                            //                    UtilityDataAccess.SendEmail(null, MailPriority.High, fromAddress, toAddress, 
                            //                                                null, "Nooch - Amount Credited Back.", null, null, 
                            //                                                null, null, "Transfer automatically cancelled because the recipient didn't accept within 15 days.");
                            //                    //output.InnerHtml += "<font color='green'>Email sent to Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " on email " + toAddress + ", For automatically cancelling a Pending Invite Transfer.</font><br/>";
                            //                }
                            //                catch (Exception)
                            //                {
                            //                    Logger.LogDebugMessage("CheckAutomaticWithdrawal - CheckAutomaticWithdrawal.");
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion Automatically Cancel any Pending Requests that older than X days


                            #region Micro Deposit Reminders (NOT CURRENTLY USED)
                            //--------------------------	Micro Deposit Reminders (For confirming a bank account)  --------------------------
                            //-------------------------- 					NOT USED FOR NOW						 --------------------------

                            //output.InnerHtml += "Getting List of Banks for member" + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + ".<br/>";

                            /*List<BankAccountDetails> bankList = obj.GetMemberBanks(mem.MemberId.ToString());

                              if (bankList != null)
                              {
                                  if (bankList.Count > 0)
                                  {
                                      //output.InnerHtml += "Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " Has bank details.<br/>";
                                      foreach (BankAccountDetails bank in bankList)
                                      {
                                          //output.InnerHtml += "Checking if the Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " Has non verfied bank details.<br/>";
                                          if (bank != null)
                                          {
                                              if (bank.IsVerified.Value == false)
                                              {
                                                  //output.InnerHtml += "Yes, Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " Has non verified bank details.<br/>";
                                                  mailBodyText = "Hi " + CommonHelper.GetDecryptedData(mem.FirstName) + " Check you bank account for 2 small deposits from Nooch that should arrive on " + bank.DateCreated.ToString();
                                                  days = (DateTime.Now - mem.DateCreated.Value).TotalDays;
                                                  if ((days >= 3.0 && days <= 3.23) || (days >= 7.0 && days <= 7.23) || (days >= 14.0 && days <= 14.23))
                                                  {
                                                      if (mem.DeviceToken != null && mem.DeviceToken != "")
                                                      {
                                                          ApplePushNotification.SendNotificationMessage(mailBodyText, 203, null, mem.DeviceToken, Utility.GetValueFromConfig("AppKey"), Utility.GetValueFromConfig("MasterSecret"));
                                                          Logger.LogDebugMessage("Micro Deposit Reminder Status - Push notification sent to [" + mem.DeviceToken + "].");
                                                          //output.InnerHtml += "<font color='green'>Push sent to Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " who has non verfied bank details.</font><br/>";
                                                      }
                                                  }
                                                  else if ((days >= 21.0))
                                                  {
                                                      //output.InnerHtml += "Request is more than 21 days for member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + ", Therefore deleting bank detail for non verified bank account.<br/>";
                                                      //If the account remains un-verified after 21 days, update the dbo.BankAccountDetails table for that bank account by assign the field
                                                      // IsDeleted a value of 1.
                                                      bool isdeleted = obj.DeleteMemberBank(bank.BankAccountId.ToString());
                                                  }
                                              }
                                          }
                                      }
                                  }
                              }*/
                            #endregion Automatic Withdrawals (NOT USED FOR NOW)


                            #region Automatic Withdrawals (NOT USED FOR NOW)
                            //---------------------------				For Automatic Withdrawal				---------------------------------------
                            /*  var transactionDataAccess = new TransactionDataAccess();
                                decimal currentBalance = decimal.Parse(CommonHelper.GetDecryptedData(mem.Deposit));
                                var noochConnection = new NoochDataEntities();
                                var membersAccountRepository = new Repository<Members, NoochDataEntities>(noochConnection);
                                string adminUserName = CommonHelper.GetEncryptedData(Utility.GetValueFromConfig("transfersMail"));
                                //output.InnerHtml += "Admin User Name is " + adminUserName + "<br/>";
                                var adminAccountSpecification = new Specification<Members>
                                {
                                    Predicate = accountTemp => accountTemp.UserName.Equals(adminUserName)
                                };
                                var admin = membersAccountRepository.SelectAll(adminAccountSpecification).FirstOrDefault();
                                //output.InnerHtml += "Inside Automatic Withdrawal for admin " + admin.MemberId + "<br/>";
                                //output.InnerHtml += "Inside Automatic Withdrawal for member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " and admin id is " + admin.MemberId + "<br/>";

                                //If IsFrequencyOn then take the value of WithdrawalId column from members table and get the current scheme of withdrawal from WithdrawalFrequency table.

                                //output.InnerHtml += "Checking if the frequency is ON/OFF for member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + "<br/>";
                                if ((mem.IsFrequencyOn == null) ? false : mem.IsFrequencyOn.Value)
                                {
                                    //output.InnerHtml += "Great Member Frequency is ON for member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + "<br/>";
                                    WithdrawalFrequency wf = obj.GetWithdrawalFrequency(mem.WithdrawalId.Value.ToString());
                                    if (wf != null)
                                    {
                                        //output.InnerHtml += "Member Frequency Settings are not null for member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + "<br/>";
                                        int day = (int)DateTime.Today.DayOfWeek;
                                        if (wf.Days != null)
                                        {
                                            //output.InnerHtml += "Member Frequency Settings is set to " + wf.Days + " for member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + "<br/>";
                                            if (wf.Days.Contains(day.ToString()) && DateTime.Now.Hour >= wf.Time.Value.Hours)
                                            {
                                                //output.InnerHtml += "Yes  " + wf.Days + " for member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + "<br/>";
                                                TransactionEntity transactionEntity = new TransactionEntity
                                                {
                                                    MemberId = admin.MemberId.ToString(),
                                                    RecipientId = mem.MemberId.ToString(),
                                                    Amount = currentBalance,
                                                    IsPrePaidTransaction = false,
                                                    Location = new LocationEntity
                                                    {
                                                        Latitude = float.Parse(mem.LastLocationLat.Value.ToString()),
                                                        Longitude = float.Parse(mem.LastLocationLng.Value.ToString()),
                                                        Altitude = 0.00f,
                                                        AddressLine1 = CommonHelper.GetDecryptedData(mem.Address),
                                                        AddressLine2 = CommonHelper.GetDecryptedData(mem.Address2),
                                                        City = CommonHelper.GetDecryptedData(mem.City),
                                                        State = CommonHelper.GetDecryptedData(mem.State),
                                                        Country = mem.Country,
                                                        ZipCode = CommonHelper.GetDecryptedData(mem.Zipcode),
                                                    },
                                                    TransactionDateTime = DateTime.Now.ToString()
                                                };
                                                string result = transactionDataAccess.WithDrawFund(transactionEntity);
                                                //output.InnerHtml += "<font color='green'>Transaction For Member Frequency Done With Output " + result + ".</font><br/>";
                                                //After success send email notification.
                                                var fromAddress = Utility.GetValueFromConfig("adminMail");
                                                var toAddress = CommonHelper.GetDecryptedData(mem.UserName);
                                                try
                                                {
                                                    UtilityDataAccess.SendEmail(null, MailPriority.High, fromAddress, toAddress, null, "Nooch - automatic withdrawl settings.", null, null, null, null, result);
                                                    //output.InnerHtml += "<font color='green'>Email sent to Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + ", to email " + toAddress + " For automatic withdrawal.</font><br/>";
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.LogDebugMessage("CheckAutomaticWithdrawal - CheckAutomaticWithdrawal.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Monthly Frequency
                                            //Withdrawal generated at 05:00PM on the last day of the month
                                            TransactionEntity transactionEntity = new TransactionEntity
                                            {
                                                MemberId = admin.MemberId.ToString(),
                                                RecipientId = mem.MemberId.ToString(),
                                                Amount = currentBalance,
                                                IsPrePaidTransaction = false,
                                                Location = new LocationEntity
                                                {
                                                    Latitude = float.Parse(mem.LastLocationLat.Value.ToString()),
                                                    Longitude = float.Parse(mem.LastLocationLng.Value.ToString()),
                                                    Altitude = 0.00f,
                                                    AddressLine1 = CommonHelper.GetDecryptedData(mem.Address),
                                                    AddressLine2 = CommonHelper.GetDecryptedData(mem.Address2),
                                                    City = CommonHelper.GetDecryptedData(mem.City),
                                                    State = CommonHelper.GetDecryptedData(mem.State),
                                                    Country = mem.Country,
                                                    ZipCode = CommonHelper.GetDecryptedData(mem.Zipcode),
                                                }
                                            };
                                            string result = transactionDataAccess.WithDrawFund(transactionEntity);
                                            //output.InnerHtml += "<font color='green'>Transaction For Member Monthly Frequency Done With Output " + result + ".</font><br/>";
                                            //After success send email notification.
                                            var fromAddress = Utility.GetValueFromConfig("adminMail");
                                            var toAddress = CommonHelper.GetDecryptedData(mem.UserName);
                                            try
                                            {
                                                UtilityDataAccess.SendEmail(null, MailPriority.High, fromAddress, toAddress, null, "Nooch - automatic withdrawl settings.", null, null, null, null, result);
                                                //output.InnerHtml += "<font color='green'>Email sent to Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " on email " + toAddress + ", For automatic withdrawal.</font><br/>";
                                            }
                                            catch (Exception)
                                            {
                                                Logger.LogDebugMessage("CheckAutomaticWithdrawal - CheckAutomaticWithdrawal.");
                                            }
                                        }
                                    }
                                }//if mem.IsFrequencyOn
                                else if ((mem.IsTriggerOn == null) ? false : mem.IsTriggerOn.Value)
                                {
                                    //Check if the members balance is over the amount set
                                    decimal withdrawalFrequency = (mem.WithdrawalFrequency == null) ? 0 : mem.WithdrawalFrequency.Value;
                                    if (currentBalance > withdrawalFrequency)
                                    {
                                        WithdrawalTrigger wt = obj.GetTriggerFrequency(mem.WithdrawalId.Value.ToString());
                                        if (wt != null)
                                        {
                                            if (wt.AmountCredited != null)
                                            {
                                                //Step 5 : If the scheme rule matches then do the transaction (Withdraw Fund) based on the rule
                                                TransactionEntity transactionEntity = new TransactionEntity
                                                {
                                                    MemberId = mem.MemberId.ToString(),
                                                    RecipientId = mem.MemberId.ToString(),
                                                    Amount = currentBalance,
                                                    IsPrePaidTransaction = false,
                                                };
                                                string result = transactionDataAccess.WithDrawFund(transactionEntity);
                                                //After success send email notification.
                                                var fromAddress = Utility.GetValueFromConfig("adminMail");
                                                var toAddress = CommonHelper.GetDecryptedData(mem.UserName);
                                                try
                                                {
                                                    UtilityDataAccess.SendEmail(null, MailPriority.High, fromAddress, toAddress, null, "Nooch - automatic withdrawl settings.", null, null, null, null, result);
                                                    //output.InnerHtml += "<font color='green'>Email sent to Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " on email " + toAddress + ", For automatic withdrawal.</font><br/>";
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.LogDebugMessage("CheckAutomaticWithdrawal - CheckAutomaticWithdrawal.");
                                                }
                                            }
                                            else
                                            {
                                                //Entire Balance
                                                TransactionEntity transactionEntity = new TransactionEntity
                                                {
                                                    MemberId = mem.MemberId.ToString(),
                                                    RecipientId = mem.MemberId.ToString(),
                                                    Amount = decimal.Parse(CommonHelper.GetDecryptedData(mem.Deposit)),
                                                    IsPrePaidTransaction = false,
                                                    Location = new LocationEntity
                                                    {
                                                        Latitude = float.Parse(mem.LastLocationLat.Value.ToString()),
                                                        Longitude = float.Parse(mem.LastLocationLng.Value.ToString()),
                                                        Altitude = 0.00f,
                                                        AddressLine1 = CommonHelper.GetDecryptedData(mem.Address),
                                                        AddressLine2 = CommonHelper.GetDecryptedData(mem.Address2),
                                                        City = CommonHelper.GetDecryptedData(mem.City),
                                                        State = CommonHelper.GetDecryptedData(mem.State),
                                                        Country = mem.Country,
                                                        ZipCode = CommonHelper.GetDecryptedData(mem.Zipcode),
                                                    }
                                                };
                                                string result = transactionDataAccess.WithDrawFund(transactionEntity);
                                                //After success send email notification.
                                                var fromAddress = Utility.GetValueFromConfig("adminMail");
                                                var toAddress = CommonHelper.GetDecryptedData(mem.UserName);
                                                try
                                                {
                                                    UtilityDataAccess.SendEmail(null, MailPriority.High, fromAddress, toAddress, null, "Nooch - automatic withdrawal settings.", null, null, null, null, result);
                                                    //output.InnerHtml += "<font color='green'>Email sent to Member " + mem.MemberId + ":" + mem.FirstName + ":" + mem.LastName + " on email " + toAddress + ", For automatic withdrawal.</font><br/>";
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.LogDebugMessage("CheckAutomaticWithdrawal - CheckAutomaticWithdrawal.");
                                                }
                                            }
                                        }
                                    }
                                }*/
                            #endregion Automatic Withdrawals (NOT USED FOR NOW)
                        }

                        membersList = null;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("*****  DAILY SCAN -> NoochChecks FAILED - [Exception: " + ex + "]  *****");
                    }
                }

                // **********   CHECK For SDN   **********
                if (mem.IsSDNSafe != true)
                {
                    try
                    {
                        bool b = IsListedInSDN(CommonHelper.GetDecryptedData(mem.LastName), mem.MemberId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("*****  DAILY SCAN -> NoochChecks -> SDN Check FAILED - [Exception: " + ex + "]  *****");
                    }
                }
            }

            #endregion For Each Member Loop

            // CLIFF (7/22/15) - NOW ADD CODE TO LOOP THROUGH EACH SYNAPSE BANK ACCOUNT WITH DEFAULT = '1' AND STATUS = 'Not Verified'
            // THEN CHECK TO MAKE SURE THE BANK BELONGS TO A NON-DELETED NOOCH MEMBER...
            // THEN SEND 1 EMAIL TO 'nonverifiedbanks@nooch.com' WITH THE TOTAL NUMBER OF NON-VERIFIED BANKS, PLUS A TABLE LISTING THE NAME &
            // EMAIL (USERNAME) OF EACH USER THAT HAS A NON-VERIFIED BANK.
            // The point of this is to let an admin know that a user has added a bank, but is waiting for Nooch to Verify their bank (for example,
            // if they submitted an Id Document in the app.  Without this new check, I would have to manually check every day to see if any users have a bank to Verify.

            //if (((Convert.ToInt16(DateTime.Now.DayOfWeek) + 6) % 7) < 6)
            //{
            notifyAdminOfBanksAwaitingVerification();
            //}

            return false;
        }


        /// <summary>
        /// For OFAC SDN Screening: check if a user's last name is listed in SDN database.
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="userId"></param>
        public bool IsListedInSDN(string lastName, Guid userId)
        {
            //Logger.LogInfoMessage("DAILY SCAN - IsListedInSDNList - Checking User Last Name: [" + lastName + "]");

            bool result = false;
            var userNameLowerCase = lastName.ToLower();

            using (NOOCHEntities noochConnection = new NOOCHEntities())
            {
                var memberFound = noochConnection.Members.FirstOrDefault(
                                  m => m.MemberId == userId && m.IsDeleted == false && m.IsSDNSafe != true);

                if (memberFound != null)
                {
                    result = true;

                    StringBuilder st = new StringBuilder();

                    //Check if the person exists in the SDN List or not
                    List<SDNSearchResult> watchlist = noochConnection.GetSDNListing(CommonHelper.GetDecryptedData(memberFound.LastName)).ToList(); // terrorlist WTF?

                    if (watchlist != null)
                    {
                        // hit matched send notification email to Nooch admin and update member table

                        if (watchlist.Count > 0)
                        {
                            memberFound.SDNCheckDateTime = DateTime.Now;
                            memberFound.AnyPriliminaryHit = true;

                            st.Append("<table border='1' style='border-collapse:collapse;text-align:center;'>");
                            st.Append("<tr><th>ENT Number</th>");
                            st.Append("<th>SDN Name</th>");
                            st.Append("<th>% Matched</th></tr>");

                            foreach (var terrorist in watchlist)
                            {
                                st.Append("<tr>");
                                st.Append("<td>" + terrorist.ent_num + "</td>");
                                st.Append("<td>" + terrorist.SDN_NAME + "</td>");
                                st.Append("<td>" + (terrorist.lastper + terrorist.subper) / 2 + "</td>");
                                st.Append("</tr>");
                            }
                            st.Append("</table>");

                            #region Send Admin SDN Match Found Email

                            try
                            {
                                // Send email to Nooch admin
                                var sendername = CommonHelper.GetDecryptedData(memberFound.FirstName) + " " +
                                                 CommonHelper.GetDecryptedData(memberFound.LastName);
                                var senderemail = CommonHelper.GetDecryptedData(memberFound.UserName);

                                StringBuilder str = new StringBuilder();
                                string s = "<html><body><h2>OFAC List Match Found</h2><p>An automatic SDN screening returned the following details of a flagged user:</p>"
                                            + st.ToString();

                                str.Append(s);

                                s = "<br/><p><strong>This user's Nooch Account information is:</strong></p>" +
                                    "<table border='1' style='border-collapse:collapse;'>" +
                                    "<tr><td><strong>Email Address:</strong></td><td>" + senderemail + "</td></tr>" +
                                    "<tr><td><strong>Name:</strong></td><td>" + sendername + "</td></tr>" +
                                    "<tr><td><strong>MemberID:</strong></td><td>" + memberFound.MemberId + "</td></tr>" +
                                    "<tr><td><strong>Country:</strong></td><td>" + CommonHelper.GetDecryptedData(memberFound.Country) + "</td></tr>" +
                                    "<tr><td><strong>Address:</strong></td><td>" + CommonHelper.GetDecryptedData(memberFound.Address) + "</td></tr>" +
                                    "<tr><td><strong>Phone Number:</strong></td><td>" + memberFound.ContactNumber + "</td></tr></table><br/><br/>- Nooch SDN Check</body></html>";

                                str.Append(s);

                                var fromAddress = Utility.GetValueFromConfig("transfersMail");

                                bool b = Utility.SendEmail(null, fromAddress,
                                         Utility.GetValueFromConfig("SDNMailReciever"), "SDN Match Found - V3", null, null, null,
                                         null, str.ToString());

                                Logger.Info("SDN Screening -> SDN Screening Results email sent to [SDN@nooch.com]");
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("SDN Screening -> SDN Screening Results email NOT sent to [SDN@nooch.com]. Exception: [" + ex + "]");
                            }

                            #endregion Send Admin SDN Match Found Email
                        }
                        else
                        {
                            memberFound.SDNCheckDateTime = DateTime.Now;
                            memberFound.AnyPriliminaryHit = false;
                            memberFound.ent_num = null;
                        }

                        // Update record in members table
                        noochConnection.SaveChanges();
                    }
                }
            }
            return result;
        }
    }
}
