using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using noochAdminNew.Classes.ModelClasses;
using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace noochAdminNew
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string DummyCacheItemKey = "transactionSynapseStatus";
        private const string DummyPageUrl =
    "http://localhost:51347/Home/AddJobCache";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            log4net.Config.XmlConfigurator.Configure();
        }

        public void Application_BeginRequest(object sender, EventArgs e)
        
        {
            if (HttpContext.Current.Cache["jobKey"] == null)
            {
                HttpContext.Current.Cache.Add("jobkey", "jobValue", null, DateTime.MaxValue, TimeSpan.FromHours(24),
                    CacheItemPriority.Default, JobCacheRemoved);
            }

        }


        public  void JobCacheRemoved(string key,
            object value, CacheItemRemovedReason reason)
        {
            WebClient client = new WebClient();
            client.DownloadData(DummyPageUrl);


            // Do the service works
            updateTransactionStatusService();

        }

        public void updateTransactionStatusService()
        {
            Logger.Info("  Global.asax -Crone Job -> Updating Transaction's Synapse status  from daily job Scheduler");
            SynapseV3ShowTransInput transInput = new SynapseV3ShowTransInput();
            List<Transaction> allTransactions = new List<Transaction>();


            //get transaction details
            using (var noochConnection = new NOOCHEntities())
            {
                var transactions = (from tr in noochConnection.Transactions
                                    join sa in noochConnection.SynapseAddTransactionResults
                                     on tr.TransactionId equals sa.TransactionId
                                    join syn in noochConnection.SynapseCreateUserResults
                                    on tr.SenderId equals syn.MemberId
                                    where tr.TransactionStatus=="Success"
                                    select new
                                    {
                                        tr,
                                        sa,
                                        syn

                                    }).ToList();


                foreach (var objT in transactions)
                {
                    Logger.Info(" Global.asax -> Updating transaction's Synapse status using  showTransactions serivice  for Transaction id - [TransactionId: " + objT.tr.TransactionId + "] in a daily scheduled job");

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
                            Logger.Error(" Global.asax -> GetSynapseBankAndUserDetailsforGivenMemberId FAILED on Checking User's Synapse OAuth Token - " +
                                                   "CheckTokenResult.msg: [" + checkTokenResult.msg + "], MemberID: [" + tran.SenderId + "]");


                            return;
                        }
                    }
                    else
                    {
                        Logger.Error(" Global.asax -> GetSynapseBankAndUserDetailsforGivenMemberId FAILED on Checking User's Synapse OAuth Token - " +
                                                   "CheckTokenResult was NULL, MemberID: [" + tran.SenderId + "]");

                        return;
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
                                Logger.Info(" Global.asax ->response from showTransactioFromSynapseV3 is false  for transaction - [transactionID: " + tran.TransactionId + "]");

                            }
                        }
                        else
                        {
                            Logger.Info(" Global.asax ->response from showTransactioFromSynapseV3 is null  for transaction - [transactionID: " + tran.TransactionId + "]");

                        }

                    }
                    catch (WebException ex)
                    {
                        Logger.Error(" Global.asax ->Error in Showing showTransactioFromSynapseV3   - - [MemberID: " + tran.SenderId + "]");

                    }

                }
            }
        }

    }
}