using System;
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
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("data source=54.201.43.89;initial catalog=NOOCH;user id=sa;password=Singh@123;");

            //app.UseHangfireDashboard();
            app.UseHangfireServer();
             
            //RecurringJob.AddOrUpdate(() => Logger.Info("Auto Task Running"), "0 12 * */2");
            //RecurringJob.AddOrUpdate(() => Logger.Info("Auto Task Running"), Cron.Minutely);
            RecurringJob.AddOrUpdate(() => updateTransactionStatusService(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => notifyAdminOfBanksAwaitingVerification(), Cron.Daily);
          
        }


        public void updateTransactionStatusService()
        {
            Logger.Info("Daily Task from Startups.cs -> updateTransactionStatusService -> ---------------------- Job Initiated at ["+DateTime.Now+"]----------------------");
            SynapseV3ShowTransInput transInput = new SynapseV3ShowTransInput();

            //get transaction details
            using (var noochConnection = new NOOCHEntities())
            {
                var transactions = (from tr in noochConnection.Transactions
                                    join sa in noochConnection.SynapseAddTransactionResults
                                     on tr.TransactionId equals sa.TransactionId
                                    join syn in noochConnection.SynapseCreateUserResults
                                    on tr.SenderId equals syn.MemberId
                                    where tr.TransactionStatus == "Success"
                                    select new
                                    {
                                        tr,
                                        sa,
                                        syn
                                    }).ToList();


                foreach (var objT in transactions)
                {
                    Logger.Info("Daily Task from Startups.cs -> updateTransactionStatusService -> Updating transaction's Synapse status using  showTransactions serivice  for Transaction id - [TransactionId: " + objT.tr.TransactionId + "] in a daily scheduled job");

                    Transaction tran = objT.tr;
                    var usersSynapseOauthKey = "";
                    #region Check If OAuth Key Still Valid
                    // if testing
                    synapseV3checkUsersOauthKey checkTokenResult = CommonHelper.refreshSynapseV3OautKey(objT.syn.access_token);

                    // if live
                    //synapseV3checkUsersOauthKey checkTokenResult = refreshSynapseV3OautKey(createSynapseUserObj.access_token,false);
                    //                                                                   

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

                    string baseAddress = "";

                    baseAddress = Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox")) ? "https://sandbox.synapsepay.com/api/v3/trans/show" : "https://synapsepay.com/api/v3/trans/show";
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
                                //updating transaction Table in db

                                tran.SynapseStatus = jsonFromSynapse["trans"][0]["recent_status"]["status"].ToString();

                                noochConnection.SaveChanges();
                            }
                            else
                            {
                                tran.SynapseStatus = "";
                                noochConnection.SaveChanges();
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


        private void notifyAdminOfBanksAwaitingVerification()
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
                            string s = "<html><body><h2>Non-Verified Syanpse Bank Accounts</h2><p>The following <strong>[" + nonVerifiedBanks.Count +
                                       "]</strong> Nooch users have attached a Synapse bank account that is awaiting verification:</p>"
                                       + st.ToString() +
                                       "<br/><br/><small>This email was generated automatically during a daily scan of all Nooch users.</small></body></html>";

                            completeEmailTxt.Append(s);

                            #region Send Email To Nooch Admin

                            try
                            {
                                var fromAddress = Utility.GetValueFromConfig("transfersMail");

                                bool b = Utility.SendEmail(null,  fromAddress,
                                    "NonVerifiedBanks@nooch.com", "Nooch Admin: Non-Verified Bank List",null,  null, null, null,
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
    }
}
