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
    }
}
