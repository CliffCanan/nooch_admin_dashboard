using noochAdminNew.Classes.ModelClasses;
using noochAdminNew.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Resources;
using Newtonsoft.Json;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.IO;
using FileHelpers;
using System.Data.Entity;


namespace noochAdminNew.Controllers
{
    public class AdminController : Controller
    {

        public void CheckSession()
        {

            if (Session["UserId"] == null)
            {
                RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        [ActionName("ShowLiveTransactionsOnDashBoard")]

        public ActionResult ShowLiveTransactionsOnDashBoard(string operation)
        {
            DashBoardLiveTransactionsOperationResult ddresult = new DashBoardLiveTransactionsOperationResult();
            var CurrentYear = DateTime.Now.Year;
            var CurrentMonth = DateTime.Now.Month;
            var CurrentDate = DateTime.Now.Day;

            try
            {
                using (NOOCHEntities obj = new NOOCHEntities())
                {
                    if (Convert.ToInt16(operation) == 0)
                    {
                        try
                        {
                            var transtLive = (from Livetranstp in obj.Transactions
                                              join member in obj.Members on Livetranstp.SenderId equals member.MemberId
                                              join membr1 in obj.Members on Livetranstp.RecipientId equals membr1.MemberId
                                              join loc in obj.GeoLocations on Livetranstp.LocationId equals loc.LocationId
                                              where Livetranstp.TransactionDate.Value.Year == CurrentYear
                                              && Livetranstp.TransactionDate.Value.Month == CurrentMonth
                                              && Livetranstp.TransactionDate.Value.Day == CurrentDate
                                              orderby Livetranstp.TransactionDate descending
                                              select new
                                              {

                                                  RecepientId = member.Nooch_ID,
                                                  SenderId = membr1.Nooch_ID,
                                                  TransactionDate = Livetranstp.TransactionDate,
                                                  TransactionId = Livetranstp.TransactionTrackingId,
                                                  SenderFirstName = member.FirstName,
                                                  SenderLastName = member.LastName,
                                                  Amount = Livetranstp.Amount,
                                                  RecipientFirstName = membr1.FirstName,
                                                  receiptLastName = membr1.LastName,
                                                  SenderNoochId = member.Nooch_ID,
                                                  ReceiptNoochId = membr1.Nooch_ID,
                                                  GeoLocationState = loc.State,
                                                  GeoLocationCity = loc.City,
                                                  TransactionStatus = Livetranstp.TransactionStatus,
                                                  Longitude = loc.Longitude,
                                                  latitude = loc.Latitude,
                                                  TransactionType = Livetranstp.TransactionType,
                                                  disputedtrack = Livetranstp.DisputeStatus

                                              }).Take(10).ToList();

                            List<MemberRecentLiveTransactionData> mm = new List<MemberRecentLiveTransactionData>();
                            foreach (var t in transtLive)
                            {
                                MemberRecentLiveTransactionData merc = new MemberRecentLiveTransactionData();
                                merc.Amount = t.Amount.ToString();
                                merc.TransID = t.TransactionId.ToString();
                                merc.RecepientId = t.RecepientId.ToString();
                                merc.SenderId = t.SenderId.ToString();
                                merc.TransDateTime = t.TransactionDate;
                                merc.SenderUserName = CommonHelper.GetDecryptedData(t.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.SenderLastName.ToString());
                                merc.RecepientUserName = CommonHelper.GetDecryptedData(t.RecipientFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.receiptLastName.ToString());
                                merc.GeoStateCityLocation = t.GeoLocationState + "," + t.GeoLocationCity;
                                merc.Longitude = t.Longitude.ToString();
                                merc.Latitude = t.latitude.ToString();
                                merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
                                merc.TransactionStatus = t.TransactionStatus;
                                merc.DisputeStatus = t.disputedtrack;
                                mm.Add(merc);
                            }

                            ddresult.IsSuccess = true;
                            ddresult.Message = "SuccessOperation";
                            ddresult.RecentLiveTransaction = mm;

                            return Json(ddresult);
                        }
                        catch (Exception ex)
                        {
                            ddresult.IsSuccess = false;
                            ddresult.Message = "InValid Operation";
                            return Json(ddresult);
                        }
                    }
                    else if (Convert.ToInt16(operation) == 1)
                    {
                        try
                        {
                            var transtLive = (from Livetranstp in obj.Transactions
                                              join member in obj.Members on Livetranstp.SenderId equals member.MemberId
                                              join membr1 in obj.Members on Livetranstp.RecipientId equals membr1.MemberId
                                              join loc in obj.GeoLocations on Livetranstp.LocationId equals loc.LocationId
                                              where SqlFunctions.DatePart("week", Livetranstp.TransactionDate) == (SqlFunctions.DatePart("week", DateTime.Now))

                                              orderby Livetranstp.TransactionDate descending
                                              select new
                                              {
                                                  RecepientId = member.Nooch_ID,
                                                  SenderId = membr1.Nooch_ID,
                                                  TransactionDate = Livetranstp.TransactionDate,
                                                  TransactionId = Livetranstp.TransactionTrackingId,
                                                  SenderFirstName = member.FirstName,
                                                  SenderLastName = member.LastName,
                                                  Amount = Livetranstp.Amount,
                                                  RecipientFirstName = membr1.FirstName,
                                                  receiptLastName = membr1.LastName,
                                                  SenderNoochId = member.Nooch_ID,
                                                  ReceiptNoochId = membr1.Nooch_ID,
                                                  GeoLocationState = loc.State,
                                                  GeoLocationCity = loc.City,
                                                  TransactionStatus = Livetranstp.TransactionStatus,
                                                  Longitude = loc.Longitude,
                                                  latitude = loc.Latitude,
                                                  TransactionType = Livetranstp.TransactionType,
                                                  disputedtrack = Livetranstp.DisputeStatus

                                              }).Take(10).ToList();

                            List<MemberRecentLiveTransactionData> mm = new List<MemberRecentLiveTransactionData>();
                            foreach (var t in transtLive)
                            {
                                MemberRecentLiveTransactionData merc = new MemberRecentLiveTransactionData();
                                merc.Amount = t.Amount.ToString();
                                merc.TransID = t.TransactionId.ToString();
                                merc.RecepientId = t.RecepientId.ToString();
                                merc.SenderId = t.SenderId.ToString();
                                merc.TransDateTime = t.TransactionDate;
                                merc.SenderUserName = CommonHelper.GetDecryptedData(t.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.SenderLastName.ToString());
                                merc.RecepientUserName = CommonHelper.GetDecryptedData(t.RecipientFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.receiptLastName.ToString());
                                merc.GeoStateCityLocation = t.GeoLocationState + " , " + t.GeoLocationCity;
                                merc.Longitude = t.Longitude.ToString();
                                merc.Latitude = t.latitude.ToString();
                                merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
                                merc.TransactionStatus = t.TransactionStatus;
                                merc.DisputeStatus = t.disputedtrack;
                                mm.Add(merc);
                            }

                            ddresult.IsSuccess = true;
                            ddresult.Message = "SuccessOperation";
                            ddresult.RecentLiveTransaction = mm;

                            return Json(ddresult);
                        }
                        catch (Exception ex)
                        {
                            ddresult.IsSuccess = false;
                            ddresult.Message = "InValid Operation";
                            return Json(ddresult);
                        }
                    }
                    else if (Convert.ToInt16(operation) == 2)
                    {
                        try
                        {
                            var transtLive = (from Livetranstp in obj.Transactions
                                              join member in obj.Members on Livetranstp.SenderId equals member.MemberId
                                              join membr1 in obj.Members on Livetranstp.RecipientId equals membr1.MemberId
                                              join loc in obj.GeoLocations on Livetranstp.LocationId equals loc.LocationId
                                              where Livetranstp.TransactionDate.Value.Year == CurrentYear
                                               && Livetranstp.TransactionDate.Value.Month == CurrentMonth

                                              orderby Livetranstp.TransactionDate descending
                                              select new
                                              {

                                                  RecepientId = member.Nooch_ID,
                                                  SenderId = membr1.Nooch_ID,
                                                  TransactionDate = Livetranstp.TransactionDate,
                                                  TransactionId = Livetranstp.TransactionTrackingId,
                                                  SenderFirstName = member.FirstName,
                                                  SenderLastName = member.LastName,
                                                  Amount = Livetranstp.Amount,
                                                  RecipientFirstName = membr1.FirstName,
                                                  receiptLastName = membr1.LastName,
                                                  SenderNoochId = member.Nooch_ID,
                                                  ReceiptNoochId = membr1.Nooch_ID,
                                                  GeoLocationState = loc.State,
                                                  GeoLocationCity = loc.City,
                                                  TransactionStatus = Livetranstp.TransactionStatus,
                                                  Longitude = loc.Longitude,
                                                  latitude = loc.Latitude,
                                                  TransactionType = Livetranstp.TransactionType,
                                                  disputedtrack = Livetranstp.DisputeStatus

                                              }).Take(10).ToList();

                            List<MemberRecentLiveTransactionData> mm = new List<MemberRecentLiveTransactionData>();
                            foreach (var t in transtLive)
                            {
                                MemberRecentLiveTransactionData merc = new MemberRecentLiveTransactionData();
                                merc.Amount = t.Amount.ToString();
                                merc.TransID = t.TransactionId.ToString();
                                merc.RecepientId = t.RecepientId.ToString();
                                merc.SenderId = t.SenderId.ToString();
                                merc.TransDateTime = t.TransactionDate;
                                merc.SenderUserName = CommonHelper.GetDecryptedData(t.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.SenderLastName.ToString());
                                merc.RecepientUserName = CommonHelper.GetDecryptedData(t.RecipientFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.receiptLastName.ToString());
                                merc.GeoStateCityLocation = t.GeoLocationState + " , " + t.GeoLocationCity;
                                merc.Longitude = t.Longitude.ToString();
                                merc.Latitude = t.latitude.ToString();
                                merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
                                merc.TransactionStatus = t.TransactionStatus;
                                merc.DisputeStatus = t.disputedtrack;
                                mm.Add(merc);
                            }

                            ddresult.IsSuccess = true;
                            ddresult.Message = "SuccessOperation";
                            ddresult.RecentLiveTransaction = mm;

                            return Json(ddresult);
                        }
                        catch (Exception ex)
                        {
                            ddresult.IsSuccess = false;
                            ddresult.Message = "InValid Operation";
                            return Json(ddresult);
                        }
                    }
                    else
                    {
                        ddresult.IsSuccess = false;
                        ddresult.Message = "InValid Operation";
                        return Json(ddresult);
                    }
                }
            }
            catch (Exception ex)
            {
                ddresult.IsSuccess = false;
                ddresult.Message = "InValid Operation";
                return Json(ddresult);
            }
        }

        public ActionResult Dashboard()
        {
            var CurrentYear = DateTime.Now.Year;
            var CurrentMonth = DateTime.Now.Month;
            var CurrentDate = DateTime.Now.Day;
            var TodayDate = DateTime.Today.ToShortDateString();
            DashboardDataClass dd = new DashboardDataClass();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                using (NOOCHEntities obj = new NOOCHEntities())
                {
                    var c = (from t in obj.Members where t.IsDeleted == false select t).ToList();
                    dd.TotalActiveUsers = c.Count;

                    //No of user activate this Today
                    c = (from t in obj.Members where t.IsDeleted == false && t.DateCreated.Value.Day == CurrentDate && t.DateCreated.Value.Year == CurrentYear && t.DateCreated.Value.Month == CurrentMonth select t).ToList();
                    dd.TotalNoOfActiveUser_Today = c.Count;

                    //No of user activate this Month
                    c = (from t in obj.Members where t.IsDeleted == false && t.DateCreated.Value.Year == CurrentYear && t.DateCreated.Value.Month == CurrentMonth select t).ToList();
                    dd.TotalNoOfActiveUser_Month = c.Count;

                    //No of user activate this Week
                    c = (from t in obj.Members where t.IsDeleted == false && SqlFunctions.DatePart("week", t.DateCreated) == (SqlFunctions.DatePart("week", DateTime.Now)) select t).ToList();
                    dd.TotalNoOfActiveUser_Week = c.Count;

                    //No of Phone Number Verified Today
                    c = (from t in obj.Members where t.IsDeleted == false && t.PhoneVerifiedOn.Value.Day == CurrentDate && t.PhoneVerifiedOn.Value.Year == CurrentYear && t.PhoneVerifiedOn.Value.Month == CurrentMonth select t).ToList();
                    dd.TotalNoOfVerifiedPhoneUsers_Today = c.Count;

                    //No of Phone Number Verified Month
                    c = (from t in obj.Members where t.IsDeleted == false && t.PhoneVerifiedOn.Value.Year == CurrentYear && t.PhoneVerifiedOn.Value.Month == CurrentMonth select t).ToList();
                    dd.TotalNoOfVerifiedPhoneUsers_Month = c.Count;

                    //No of Phone Number Verified week
                    c = (from t in obj.Members where t.IsDeleted == false && SqlFunctions.DatePart("week", t.PhoneVerifiedOn) == (SqlFunctions.DatePart("week", DateTime.Now)) select t).ToList();
                    dd.TotalNoOfVerifiedPhoneUsers_Week = c.Count;

                    //No of email Verified Today
                    c = (from t in obj.AuthenticationTokens
                         join mem in obj.Members on t.MemberId equals mem.MemberId
                         where mem.IsDeleted == false && t.IsActivated == true
                             && t.VerifiedOn.Value.Day == CurrentDate && t.VerifiedOn.Value.Year == CurrentYear && t.VerifiedOn.Value.Month == CurrentMonth
                         select mem).ToList();
                    dd.TotalNoOfVerifiedEmailUsers_Today = c.Count;

                    //No of Email Verified Month
                    c = (from t in obj.AuthenticationTokens
                         join mem in obj.Members on t.MemberId equals mem.MemberId
                         where mem.IsDeleted == false
                             && t.IsActivated == true && t.VerifiedOn.Value.Year == CurrentYear && t.VerifiedOn.Value.Month == CurrentMonth
                         select mem).ToList();
                    dd.TotalNoOfVerifiedEmailUsers_Month = c.Count;


                    //No of Email Verified week
                    c = (from t in obj.AuthenticationTokens
                         join mem in obj.Members on t.MemberId equals mem.MemberId
                         where mem.IsDeleted == false
                             && t.IsActivated == true && SqlFunctions.DatePart("week", t.VerifiedOn) == (SqlFunctions.DatePart("week", DateTime.Now))
                         select mem).ToList();
                    dd.TotalNoOfVerifiedEmailUsers_Week = c.Count;

                    c = (from t in obj.Members where t.IsDeleted == false && t.Status == "Registered" select t).ToList();
                    dd.TotalRegisteredUsers = c.Count;

                    c = (from t in obj.Members where t.IsDeleted == false && t.Status == "Active" select t).ToList();
                    dd.TotalVerifiedEmailUsers = c.Count;

                    c = (from t in obj.Members where t.IsDeleted == false && t.Status == "Suspended" select t).ToList();
                    dd.TotalSuspendedUsers = c.Count;

                    c =
                        (from t in obj.Members where t.IsDeleted == false && t.IsVerifiedPhone == true select t).ToList();
                    dd.TotalVerifiedPhoneUsers = c.Count;

                    c =
                        (from t in obj.Members
                         where t.IsDeleted == false && t.Status == "Active" && t.IsVerifiedPhone == true
                         select t).ToList();
                    dd.TotalVerifiedPhoneAndEmailUsers = c.Count;

                    c = (from t in obj.Members
                         join kad in obj.KnoxAccountDetails
                             on t.MemberId equals kad.MemberId
                         where
                             t.IsDeleted == false && t.Status == "Active" && t.IsVerifiedPhone == true &&
                             kad.IsDeleted == false
                         select t).ToList();
                    dd.TotalActiveAndVerifiedBankAccountUsers = c.Count;

                    c = (from t in obj.Members
                         join kad in obj.KnoxAccountDetails
                             on t.MemberId equals kad.MemberId
                         where t.IsDeleted == false && kad.IsDeleted == false
                         select t).ToList();
                    dd.TotalActiveBankAccountUsers = c.Count;

                    dd.TotalAmountOfDollars =
                        (from r in obj.Transactions where r.TransactionStatus == "success" select r).ToList()
                            .Sum(t => t.Amount)
                            .ToString();
                    dd.TotalNoOfPaymentsCompleted =
                        (from r in obj.Transactions where r.TransactionStatus == "success" select r).ToList().Count;

                    dd.totalRequestTypeTrans = (from r in obj.Transactions
                                                where r.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw==" && r.TransactionStatus == "success"
                                                select r).ToList().Count;

                    dd.TransactionsPendi =
                        (from r in obj.Transactions where r.TransactionStatus == "Pending" select r).ToList().Count;

                    // dd.TransData = "[                [gd(2012, 1, 1), 7], [gd(2012, 1, 2), 6], [gd(2012, 1, 3), 4], [gd(2012, 1, 4), 8],                [gd(2012, 1, 5), 9], [gd(2012, 1, 6), 7], [gd(2012, 1, 7), 5], [gd(2012, 1, 8), 4],                [gd(2012, 1, 9), 7], [gd(2012, 1, 10), 8], [gd(2012, 1, 11), 9], [gd(2012, 1, 12), 6],                [gd(2012, 1, 13), 4], [gd(2012, 1, 14), 5], [gd(2012, 1, 15), 11], [gd(2012, 1, 16), 8],                [gd(2012, 1, 17), 8], [gd(2012, 1, 18), 11], [gd(2012, 1, 19), 11], [gd(2012, 1, 20), 6],                [gd(2012, 1, 21), 6], [gd(2012, 1, 22), 8], [gd(2012, 1, 23), 11], [gd(2012, 1, 24), 13],                [gd(2012, 1, 25), 7], [gd(2012, 1, 26), 9], [gd(2012, 1, 27), 9], [gd(2012, 1, 28), 8],                [gd(2012, 1, 29), 5], [gd(2012, 1, 30), 8], [gd(2012, 1, 31), 25]            ]               "; 

                    // **No Of User Of Each Bank   
                    var ss = (from SSB in obj.SynapseSupportedBanks
                              select new NoOfUsersInEachBank
                              {
                                  BankName = SSB.BankName,
                                  NoOfUsers = (from SBM in obj.SynapseBanksOfMembers
                                               where SBM.IsDefault == true && SBM.bank_name == SSB.BankName
                                               group SBM by SBM.bank_name into s
                                               select s.Count()).FirstOrDefault()
                              }).OrderBy(a => a.BankName).ToList();
                    dd.UserCountInEachBank = ss;
                    // var ss = obj.Database.SqlQuery<NoOfUsersInEachBank>("select BankName,(select count(*) from SynapseBanksOfMembers where bank_name=ss.BankName and IsDefault=1 )as NoOfUsers from SynapseSupportedBanks ss order by BankName").ToList(); 
                }

                return View(dd);
            }
        }



        public ActionResult CreditFundToMember()
        {
            if (Session["UserId"] == null && Session["RoleId"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ActionName("CreditFundToMemberPost")]
        public ActionResult CreditFundToMemberPost(string transferfundto, string transferAmount, string transferNotes,
            string adminPin)
        {
            LoginResult lr = new LoginResult();
            // performing validations over input

            #region input validations

            if (String.IsNullOrEmpty(transferfundto))
            {
                lr.IsSuccess = false;
                lr.Message = "Please enter user name or NoochId of Member to transfer fund.";
            }
            if (String.IsNullOrEmpty(transferAmount))
            {
                lr.IsSuccess = false;
                lr.Message = "Please enter transfer fund amount";
            }

            if (String.IsNullOrEmpty(transferNotes))
            {
                lr.IsSuccess = false;
                lr.Message = "Please enter transfer notes.";
            }
            if (String.IsNullOrEmpty(adminPin))
            {
                lr.IsSuccess = false;
                lr.Message = "Please enter admin pin.";
            }

            #endregion

            // 1.check admin user knox account and other details
            // 2. check fund receiver knox account details

            // checking admin user details
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                string adminpinEncrypted = CommonHelper.GetEncryptedData(adminPin.Trim());
                var adminUserDetails =
                    (from c in obj.Members
                     where
                         c.UserName == "z2/de4EMabGlzMuO7OocHw==" && c.Status == "Active" &&
                         c.PinNumber == adminpinEncrypted
                     select c).SingleOrDefault();

                if (adminUserDetails != null)
                {
                    Guid AdminMemberId = Utility.ConvertToGuid(adminUserDetails.MemberId.ToString());
                    // checking knox account details of admin
                    var adminKnoxDetails =
                        (from c in obj.KnoxAccountDetails
                         where c.MemberId == AdminMemberId && c.IsDeleted == false
                         select c).SingleOrDefault();

                    if (adminKnoxDetails != null)
                    {
                        // checking fund recepient details
                        string recepientusernameencrypted = CommonHelper.GetEncryptedData(transferfundto.ToLower());
                        var recepientdetails = (from c in obj.Members
                                                where
                                                    c.Nooch_ID == transferfundto ||
                                                    c.UserName == recepientusernameencrypted && c.Status == "Active"
                                                select c).SingleOrDefault();
                        if (recepientdetails != null)
                        {
                            // checking recepient knox details
                            Guid recepeintGuid = Utility.ConvertToGuid(recepientdetails.MemberId.ToString());
                            var recepeintknoxdetails =
                                (from c in obj.KnoxAccountDetails
                                 where c.MemberId == recepeintGuid && c.IsDeleted == false
                                 select c).SingleOrDefault();
                            if (recepeintknoxdetails != null)
                            {
                                // all set to call pin payment service
                                //string ADMIN_KNOX_TRANS_ID = CommonHelper.GetDecryptedData(adminKnoxDetails.TransId.ToString());
                                //string ADMIN_USER_KEY = CommonHelper.GetDecryptedData(adminKnoxDetails.UserKey.ToString());
                                //string ADMIN_USER_PASS = CommonHelper.GetDecryptedData(adminKnoxDetails.UserPass.ToString());

                                //string RECEPEINT_KNOX_TRANS_ID = CommonHelper.GetDecryptedData(recepeintknoxdetails.TransId.ToString());
                                //string RECEPEINT_USER_KEY = CommonHelper.GetDecryptedData(recepeintknoxdetails.UserKey.ToString());
                                //string RECEPEINT_USER_PASS = CommonHelper.GetDecryptedData(recepeintknoxdetails.UserPass.ToString());

                                string transactionTrackingId = GetRandomTransactionTrackingId();

                                Transaction trans = new Transaction();
                                trans.TransactionId = Guid.NewGuid();
                                trans.SenderId = AdminMemberId;
                                trans.RecipientId = recepeintGuid;
                                trans.Amount = Convert.ToDecimal(transferAmount);

                                trans.TransactionDate = DateTime.Now;
                                trans.DisputeStatus = null;
                                trans.TransactionStatus = "Success";
                                trans.TransactionType = CommonHelper.GetEncryptedData("Reward");
                                trans.DeviceId = null;
                                trans.TransactionTrackingId = transactionTrackingId;
                                trans.Memo = transferNotes.Trim();
                                trans.Picture = null;

                                // geolocation details
                                GeoLocation geo = new GeoLocation();
                                geo.LocationId = Guid.NewGuid();
                                geo.Latitude = null;
                                geo.Longitude = null;
                                geo.Altitude = null;
                                geo.AddressLine1 = null;
                                geo.AddressLine2 = null;
                                geo.City = null;
                                geo.State = null;
                                geo.Country = null;
                                geo.ZipCode = null;
                                geo.DateCreated = DateTime.Now;


                                // making api call to knox
                                WebClient wc = new WebClient();

                                string KNoxApiKey = Utility.GetValueFromConfig("KnoxApiKey");
                                string KNoxApiPass = Utility.GetValueFromConfig("KnoxApiPass");

                                string c = "https://knoxpayments.com/json/pinpayment.php?payee_key=" +
                                           //RECEPEINT_USER_KEY +
                                           //"&payee_pass=" + RECEPEINT_USER_PASS + "&payor_key=" + ADMIN_USER_KEY +
                                           "&payor_pass=" +
                                           //ADMIN_USER_PASS + "&trans_id=" + trans.TransactionId + "&PARTNER_KEY=" +
                                           KNoxApiKey + "&amount=" + trans.Amount + "&recur_status=ot";
                                string knoxPinPaymentResults = wc.DownloadString(c);

                                ResponseClass3 m = JsonConvert.DeserializeObject<ResponseClass3>(knoxPinPaymentResults);
                                if (m != null)
                                {
                                    #region parsed response successfully
/*
                                    string KnoxTransStatus = m.JSonDataResult.status_code;
                                    string KnoxTransErrorCode = m.JSonDataResult.error_code;
                                    string KnoxTransId = m.JSonDataResult.trans_id;

                                    if (KnoxTransStatus != null)
                                    {
                                        Logger.Info(
                                            "TransferFundToMemberFromNEWADMIN_PANLE -> knoxPinPaymentResult Status Code for Nooch TransID [" +
                                            trans.TransactionId + "] is: " + KnoxTransStatus);
                                    }
                                    if (KnoxTransErrorCode != null)
                                    {
                                        Logger.Info(
                                            "TransferFundToMemberFromNEWADMIN_PANLE -> knoxPinPaymentResult ERROR Code for Nooch TransID [" +
                                            trans.TransactionId + "] is: " + KnoxTransErrorCode);
                                    }

                                    if (KnoxTransStatus == "PAID" && KnoxTransErrorCode == null)
                                    {
                                        #region Knox returned Paid

                                        #region email content preparation

                                        string senderFirstName =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(adminUserDetails.FirstName));
                                        string senderLastName =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(adminUserDetails.LastName));
                                        string recipientFirstName =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(recepientdetails.FirstName));
                                        string recipientLastName =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(recepientdetails.LastName));

                                        string wholeAmount = trans.Amount.ToString("n2");
                                        string[] s3 = wholeAmount.Split('.');
                                        string ce = "";
                                        string dl = "";
                                        if (s3.Length <= 1)
                                        {
                                            dl = s3[0].ToString();
                                            ce = "00";
                                        }
                                        else
                                        {
                                            ce = s3[1].ToString();
                                            dl = s3[0].ToString();
                                        }

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

                                        #endregion

                                        string senderPic;
                                        string recipientPic;
                                        var friendDetails =
                                            CommonHelper.GetMemberNotificationSettings(
                                                adminUserDetails.MemberId.ToString());

                                        #region email to admin on successfully sending fund

                                        if (friendDetails != null)
                                        {
                                            // for TransferSent email notification
                                            if (friendDetails != null && (friendDetails.EmailTransferSent ?? false))
                                            {
                                                if (recepientdetails.Photo != null && recepientdetails.Photo != "")
                                                {
                                                    string lastFourOfRecipientsPic =
                                                        recepientdetails.Photo.Substring(recepientdetails.Photo.Length -
                                                                                         15);
                                                    if (lastFourOfRecipientsPic != "gv_no_photo.png")
                                                    {
                                                        recipientPic = "";
                                                    }
                                                    else
                                                    {
                                                        recipientPic = recepientdetails.Photo.ToString();
                                                    }
                                                }

                                                var tokens = new Dictionary<string, string>
                                                {
                                                    {Constants.PLACEHOLDER_FIRST_NAME, senderFirstName},
                                                    {
                                                        Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
                                                        recipientFirstName + " " + recipientLastName
                                                    },
                                                    {Constants.PLACEHOLDER_TRANSFER_AMOUNT, dl},
                                                    {Constants.PLACEHLODER_CENTS, ce},
                                                    {Constants.MEMO, memo}
                                                };

                                                var fromAddress = Utility.GetValueFromConfig("transfersMail");
                                                var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

                                                try
                                                {
                                                    // email notification
                                                    //Utility.SendEmail("TransferSent", 
                                                    //    fromAddress, toAddress, null,
                                                    //    "Your $" + wholeAmount + " payment to " + recipientFirstName +
                                                    //    " on Nooch",
                                                    //    null, tokens, null, null, null);

                                                    Utility.SendEmail("TransferSent", fromAddress, toAddress,
                                                        "Your $ " + wholeAmount + " payment to " + recipientFirstName +
                                                        " on Nooch", null,
                                                        tokens, null, null, null);


                                                    Logger.Info(
                                                        "Add fund to members account New Admin --> TransferSent status mail sent to [" +
                                                        toAddress + "].");
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.Error(
                                                        "Add fund to members account New Admin --> TransferSent mail NOT sent to [" +
                                                        toAddress +
                                                        "]. Problem occurred in sending mail.");
                                                }


                                            }
                                        }

                                        #endregion


                                        #region EmailAndPushNotificationToRecepientOnTransferReceive

                                        // for push notification
                                        //var friendDetails = memberDataAccess.GetMemberNotificationSettingsByUserName(CommonHelper.GetDecryptedData(receiverAccountDetail.UserName));
                                        var friendDetails2 =
                                            CommonHelper.GetMemberNotificationSettings(
                                                recepientdetails.MemberId.ToString());
                                        if (friendDetails2 != null)
                                        {
                                            string deviceId2 = friendDetails2 != null
                                                ? recepientdetails.DeviceToken
                                                : null;

                                            string mailBodyText = "You received $" + wholeAmount + " from " +
                                                                  senderFirstName +
                                                                  " " + senderLastName;

                                            if ((friendDetails2.TransferReceived == null)
                                                ? false
                                                : friendDetails2.TransferReceived.Value)
                                            {
                                                try
                                                {
                                                    // push notifications
                                                    if (friendDetails2 != null && !String.IsNullOrEmpty(deviceId2) &&
                                                        (friendDetails2.TransferReceived ?? false))
                                                    {
                                                        Utility.SendNotificationMessage(mailBodyText, 1,
                                                            null, deviceId2,
                                                            Utility.GetValueFromConfig("AppKey"),
                                                            Utility.GetValueFromConfig("MasterSecret"));

                                                        Logger.Info(
                                                            "Add fund to member from new admin panel --> Push notification sent to Sender DeviceID:[" +
                                                            deviceId2 + "] successfully.");
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.Error(
                                                        "Add fund to member from new admin panel --> Error: Push notification NOT sent to Sender DeviceID: [" +
                                                        deviceId2 + "]");
                                                }
                                            }

                                            // for TransferReceived email notification
                                            if (friendDetails2 != null &&
                                                (friendDetails2.EmailTransferReceived ?? false))
                                            {
                                                if (adminUserDetails.Photo != null && adminUserDetails.Photo != "")
                                                {
                                                    string lastFourOfSendersPic =
                                                        adminUserDetails.Photo.Substring(adminUserDetails.Photo.Length -
                                                                                         15);
                                                    if (lastFourOfSendersPic != "gv_no_photo.png")
                                                    {
                                                        senderPic = "";
                                                    }
                                                    else
                                                    {
                                                        senderPic = adminUserDetails.Photo.ToString();
                                                    }
                                                }

                                                var tokensR = new Dictionary<string, string>
                                                {
                                                    {Constants.PLACEHOLDER_FIRST_NAME, recipientFirstName},
                                                    {
                                                        Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
                                                        senderFirstName + " " + senderLastName
                                                    },
                                                    {Constants.PLACEHOLDER_TRANSFER_AMOUNT, wholeAmount},
                                                    {
                                                        Constants.PLACEHOLDER_TRANSACTION_DATE,
                                                        Convert.ToDateTime(trans.TransactionDate)
                                                            .ToString("MMM dd")
                                                    },
                                                    {Constants.MEMO, memo}
                                                };

                                                // for TransferReceived email notification                            
                                                var fromAddress = Utility.GetValueFromConfig("transfersMail");
                                                var toAddress2 = CommonHelper.GetDecryptedData(recepientdetails.UserName);

                                                try
                                                {
                                                    // email notification
                                                    Utility.SendEmail("TransferReceived", fromAddress, toAddress2,
                                                        senderFirstName + " sent you $" + wholeAmount + " with Nooch",
                                                        null, tokensR, null, null, null);

                                                    Logger.Info(
                                                        "Add fund to member from new admin panel --> TransferReceived Email sent to [" +
                                                        toAddress2 + "] successfully.");
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.Error(
                                                        "Add fund to member from new admin panel --> Error: TransferReceived Email NOT sent to [" +
                                                        toAddress2 + "]");
                                                }
                                            }
                                        }

                                        #endregion



                                        try
                                        {
                                            obj.GeoLocations.Add(geo);

                                            obj.SaveChanges();


                                            obj.Transactions.Add(trans);
                                            obj.SaveChanges();
                                            lr.IsSuccess = true;
                                            lr.Message = "fund succesfully added to member account.";
                                        }
                                        catch (Exception)
                                        {

                                            lr.IsSuccess = false;
                                            lr.Message = "Error occured while saving transaction in db.";
                                        }


                                        #endregion
                                    }
                                    else
                                    {
                                        #region emailSendingonTransferAttemtFailure

                                        // for push notification in case of failure

                                        var senderNotificationSettings =
                                            CommonHelper.GetMemberNotificationSettings(
                                                adminUserDetails.MemberId.ToString());

                                        if (senderNotificationSettings != null)
                                        {
                                            string senderFirstNameFailure =
                                                CommonHelper.UppercaseFirst(
                                                    CommonHelper.GetDecryptedData(adminUserDetails.FirstName));
                                            string senderLastNameFailure =
                                                CommonHelper.UppercaseFirst(
                                                    CommonHelper.GetDecryptedData(adminUserDetails.LastName));
                                            string recipientFirstNameFailure =
                                                CommonHelper.UppercaseFirst(
                                                    CommonHelper.GetDecryptedData(recepientdetails.FirstName));
                                            string recipientLastNameFailure =
                                                CommonHelper.UppercaseFirst(
                                                    CommonHelper.GetDecryptedData(recepientdetails.LastName));


                                            // for TransferAttemptFailure email notification
                                            if (senderNotificationSettings != null &&
                                                (senderNotificationSettings.EmailTransferAttemptFailure ?? false))
                                            {
                                                string s2 = trans.Amount.ToString("n2");
                                                string[] s3 = s2.Split('.');

                                                var tokensF = new Dictionary<string, string>
                                                {
                                                    {
                                                        Constants.PLACEHOLDER_FIRST_NAME,
                                                        senderFirstNameFailure + " " + senderLastNameFailure
                                                    },
                                                    {
                                                        Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
                                                        CommonHelper.GetDecryptedData(recepientdetails.UserName)
                                                    },
                                                    {Constants.PLACEHOLDER_TRANSFER_AMOUNT, s3[0].ToString()},
                                                    {Constants.PLACEHLODER_CENTS, s3[1].ToString()},
                                                };

                                                var fromAddress = Utility.GetValueFromConfig("transfersMail");
                                                var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

                                                try
                                                {
                                                    // email notification
                                                    Utility.SendEmail("transferFailure",
                                                        fromAddress, toAddress, null,
                                                        "Nooch transfer failure", tokensF, null, null, null);

                                                    Logger.Info(
                                                        "Add fund to member new admin panel --> Transfer FAILED --> Email sent to Sender: [" +
                                                        toAddress + "] successfully.");
                                                }
                                                catch (Exception)
                                                {
                                                    Logger.Error(
                                                        "Add fund to member new admin panel --> Error: TransferAttemptFailure mail not sent to [" +
                                                        toAddress + "]");
                                                }
                                            }
                                        }

                                        #endregion

                                        lr.IsSuccess = false;
                                        lr.Message = "Knox payment failed.";
                                    }


                                    */
                                    #endregion
                                }
                                else
                                {
                                    #region emailSendingonTransferAttemtFailure
                                    /*

                                    // for push notification in case of failure

                                    var senderNotificationSettings =
                                        CommonHelper.GetMemberNotificationSettings(adminUserDetails.MemberId.ToString());

                                    if (senderNotificationSettings != null)
                                    {
                                        string senderFirstNameFailure =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(adminUserDetails.FirstName));
                                        string senderLastNameFailure =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(adminUserDetails.LastName));
                                        string recipientFirstNameFailure =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(recepientdetails.FirstName));
                                        string recipientLastNameFailure =
                                            CommonHelper.UppercaseFirst(
                                                CommonHelper.GetDecryptedData(recepientdetails.LastName));


                                        // for TransferAttemptFailure email notification
                                        if (senderNotificationSettings != null &&
                                            (senderNotificationSettings.EmailTransferAttemptFailure ?? false))
                                        {
                                            string s2 = trans.Amount.ToString("n2");
                                            string[] s3 = s2.Split('.');

                                            var tokensF = new Dictionary<string, string>
                                            {
                                                {
                                                    Constants.PLACEHOLDER_FIRST_NAME,
                                                    senderFirstNameFailure + " " + senderLastNameFailure
                                                },
                                                {
                                                    Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
                                                    CommonHelper.GetDecryptedData(recepientdetails.UserName)
                                                },
                                                {Constants.PLACEHOLDER_TRANSFER_AMOUNT, s3[0].ToString()},
                                                {Constants.PLACEHLODER_CENTS, s3[1].ToString()},
                                            };

                                            var fromAddress = Utility.GetValueFromConfig("transfersMail");
                                            var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

                                            try
                                            {
                                                // email notification
                                                Utility.SendEmail("transferFailure",
                                                    fromAddress, toAddress, null,
                                                    "Nooch transfer failure", tokensF, null, null, null);

                                                Logger.Info(
                                                    "Add fund to member new admin panel --> Transfer FAILED --> Email sent to Sender: [" +
                                                    toAddress + "] successfully.");
                                            }
                                            catch (Exception)
                                            {
                                                Logger.Error(
                                                    "Add fund to member new admin panel --> Error: TransferAttemptFailure mail not sent to [" +
                                                    toAddress + "]");
                                            }
                                        }
                                    }
*/
                                    #endregion

                                    lr.IsSuccess = false;
                                    lr.Message = "Knox payment failed.";
                                }
                            }
                            else
                            {
                                lr.IsSuccess = false;
                                lr.Message = "Recepeint knox account not available.";
                            }
                        }
                        else
                        {
                            lr.IsSuccess = false;
                            lr.Message = "Given username/nooch id not found or give username/nooch id not active.";
                        }
                    }
                    else
                    {
                        lr.IsSuccess = false;
                        lr.Message = "Admin knox account not available.";
                    }
                }
                else
                {
                    lr.IsSuccess = false;
                    lr.Message = "Admin account team@nooch.com not active or invalid admin PIN passed.";
                }
            }
            return Json(lr);
        }


        public string GetRandomTransactionTrackingId()
        {
            const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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
                    using (NOOCHEntities obj = new NOOCHEntities())
                    {
                        var existingtrans =
                            (from c in obj.Transactions where c.TransactionTrackingId == randomId select c)
                                .SingleOrDefault();

                        if (existingtrans == null)
                        {
                            return randomId;
                        }

                        j += i + 1;

                    }
                }
            }
            return null;
        }


        // If session is null then redirect to home 
        public ActionResult OFAC()
        {

            if (Session["UserId"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Upload the SDN , ADD files And save them in Db
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveOFACListInDb()
        {
            string flag = "true", result = "";
            Boolean IsSuccess = true;
           // if ((((Request.Files[0].FileName.Contains("SDN")) || (Request.Files[0].FileName.Contains("sdn"))) && (Request.Files[0].ContentLength > 0)) && (((Request.Files[1].FileName.Contains("ADD")) || (Request.Files[1].FileName.Contains("add"))) && (Request.Files[1].ContentLength > 0)) && (((Request.Files[2].FileName.Contains("ALT")) || (Request.Files[2].FileName.Contains("alt")) && (Request.Files[2].ContentLength > 0))))
            //{

                // Code For  SDN File
                if (((Request.Files[0].FileName.Contains("SDN")) || (Request.Files[0].FileName.Contains("sdn"))) && (Request.Files[0].ContentLength > 0))
                {
                    try
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "Content/";
                        string filename = Path.GetFileName(Request.Files[0].FileName);
                        Request.Files[0].SaveAs(Path.Combine(path, filename));

                        // code for reading content of SDN pipe file                    
                        FileHelperEngine engine = new FileHelperEngine(typeof(OfacList.SDNEntity));

                        OfacList.SDNEntity[] res = engine.ReadFile(path + filename) as OfacList.SDNEntity[];

                        if (res.Count() > 0)
                        {
                            using (NOOCHEntities NoochConnection = new NOOCHEntities())
                            {
                                try
                                {
                                    // Delete all records from database 
                                    NoochConnection.Database.ExecuteSqlCommand("TRUNCATE TABLE SDN");
                                    IsSuccess = true;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Error In Deleting SDN");

                                    flag = "false";
                                    result = ex.ToString();
                                    IsSuccess = false;
                                }

                                if (IsSuccess == true)
                                {
                                    // records deleted, then insert new records into DB
                                    string d = AddNewDataInSDN(res);
                                    if (d != "Records Added Successfully.")
                                    {
                                        Logger.Error("Error In Inserting SDN");
                                        flag = "false";
                                        result = d;
                                    }
                                    else
                                    {
                                        Logger.Info("SDN Uploaded Successfully");
                                        result = "SDN Table records updated successfully.";

                                    }
                                }
                            }
                        }

                        // delete file from uploads folder
                        if ((System.IO.File.Exists(path + filename)))
                        {
                            System.IO.File.Delete(path + filename);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error In SDN");
                        flag = "false";
                        result = ex.Message.ToString();
                    }

                }
                else if (((Request.Files[0].FileName.Contains("SDN") == false) || (Request.Files[0].FileName.Contains("sdn") == false)) && (Request.Files[0].ContentLength > 0))
                {
                    Logger.Error("Invalid files Uploaded.Kindly upload in correct formats for SDN");

                    flag = "false";
                    result = "Invalid file name for SDN table file.";

                }

                // Code For ADD File Type ADD
                if (((Request.Files[1].FileName.Contains("ADD")) || (Request.Files[1].FileName.Contains("add"))) && (Request.Files[1].ContentLength > 0))
                {
                    try
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "Content/";
                        string filename = Path.GetFileName(Request.Files[1].FileName);
                        Request.Files[1].SaveAs(Path.Combine(path, filename));

                        // code for reading content of ADD pipe file
                        FileHelperEngine engineadd = new FileHelperEngine(typeof(OfacList.ADDEntity));

                        OfacList.ADDEntity[] addres = engineadd.ReadFile(path + filename) as OfacList.ADDEntity[];

                        if (addres.Count() > 0)
                        {
                            // drop existing records from database and add new
                            using (NOOCHEntities NoochConnection = new NOOCHEntities())
                            {
                                try
                                {
                                    // Deletting all records from database
                                    NoochConnection.Database.ExecuteSqlCommand("TRUNCATE TABLE [ADD]");
                                    IsSuccess = true;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Error In Deleting Add");
                                    flag = "false";
                                    result = ex.ToString();
                                    IsSuccess = false;
                                }
                            }


                            if (IsSuccess == true)
                            {
                                // records deleted, code to insert new records into DB
                                string d = AddNewDataInADD(addres);
                                if (d != "Records Added Successfully.")
                                {
                                    Logger.Error("Error In Inserting ADD");
                                    flag = "false";
                                    result += d;
                                }
                                else
                                {
                                    Logger.Info("ADD Uploaded SuccessFully");
                                    result = result + " ADD Table records updated successfully.";
                                }
                            }

                        }

                        // delete file from uploads folder
                        if ((System.IO.File.Exists(path + filename)))
                        {
                            System.IO.File.Delete(path + filename);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error In ADD file");
                        flag = "false";
                        result = result + ex.Message.ToString();
                    }


                }
                else if (((Request.Files[1].FileName.Contains("ADD") == false) || (Request.Files[1].FileName.Contains("add") == false)) && (Request.Files[1].ContentLength > 0))
                {
                    Logger.Error("Invalid files Uploaded.Kindly upload in correct formats for ALT");
                    flag = "false";
                    result += "Invalid file name for ADD table file.";

                }


                // Code For ALT File Type 
                if (((Request.Files[2].FileName.Contains("ALT")) || (Request.Files[2].FileName.Contains("ALT"))) && (Request.Files[2].ContentLength > 0))
                {
                    try
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "Content/";
                        string filename = Path.GetFileName(Request.Files[2].FileName);
                        Request.Files[2].SaveAs(Path.Combine(path, filename));


                        // code for reading content of ALT pipe file
                        FileHelperEngine enginealt = new FileHelperEngine(typeof(OfacList.ALTEntity));

                        OfacList.ALTEntity[] altress = enginealt.ReadFile(path + filename) as OfacList.ALTEntity[];

                        if (altress.Count() > 0)
                        {
                            // drop existing records from database and add new
                            using (NOOCHEntities NoochConnection = new NOOCHEntities())
                            {
                                try
                                {
                                    // Deletting all records from database
                                    NoochConnection.Database.ExecuteSqlCommand("TRUNCATE TABLE ALT");
                                    IsSuccess = true;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Error In Deleting ALT");
                                    flag = "false";
                                    result = ex.ToString();
                                    IsSuccess = false;
                                }
                            }

                            if (IsSuccess == true)
                            {
                                // records deleted, code to insert new records into DB
                                string d = AddNewDataInALT(altress);
                                if (d != "Records Added Successfully.")
                                {
                                    Logger.Error("Error In Inserting ALT");
                                    flag = "false";
                                    result += d;
                                }
                                else
                                {
                                    Logger.Info("ALT Uploaded SuccessFully");
                                    result = result + " ALT Table records updated successfully.";

                                }
                            }
                        }

                        // delete file from uploads folder
                        if ((System.IO.File.Exists(path + filename)))
                        {
                            System.IO.File.Delete(path + filename);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Error In ALT File");
                        flag = "false";
                        result = result + ex.Message.ToString();
                    }
                }
                else if (((Request.Files[2].FileName.Contains("ALT") == false) || (Request.Files[2].FileName.Contains("alt") == false)) && (Request.Files[2].ContentLength > 0))
                {
                    flag = "false";
                    result += "Invalid file name for ALT table file.";
                    Logger.Error("Invalid files Uploaded.Kindly upload in correct formats for ALT");

                }

                if (flag.Equals("true"))
                {
                    result = "Files uploaded successfully.";
                }

            //}
            //else
            //{
            //    Logger.Error("Invalid files Uploaded.Kindly upload in correct formats");

            //    flag = "false";
            //    result = "Invalid files Uploaded.";
            //}

            ViewData["result"] = result;

            return View("OFAC");
        }



        public ActionResult SearchAdmin()
        {

            // getting all active admins from db
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                List<SearchAdminResultClass> sac = new List<SearchAdminResultClass>();
                var alladmins = (from c in obj.AdminUsers where c.Status == "Active" select c).ToList();
                foreach (AdminUser ad in alladmins)
                {
                    SearchAdminResultClass sc = new SearchAdminResultClass();

                    sc.AdminFirstName = ad.FirstName;
                    sc.AdminLastName = ad.LastName;
                    sc.AdminEmail = ad.Email;
                    sc.AdminUserName = ad.UserName;
                    sc.AdminLevel = ad.AdminLevel;

                    sac.Add(sc);
                }
                return View(sac);
            }
        }

        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [ActionName("CreateAndSaveNewAdminUser")]
        public ActionResult CreateAndSaveNewAdminUser(string userName, string emailAddress, string firstName,
            string lastName, string level)
        {
            Guid d = Utility.ConvertToGuid(Session["UserId"].ToString());
            CreateAdminResultClass s = new CreateAdminResultClass();
            if (!String.IsNullOrEmpty(userName.Trim()) && !String.IsNullOrEmpty(emailAddress.Trim()) &&
                !String.IsNullOrEmpty(firstName.Trim()) && !String.IsNullOrEmpty(lastName.Trim()) &&
                !String.IsNullOrEmpty(level.Trim()))
            {
                s = CreateOrUpdateAdmin("", userName, emailAddress, firstName, lastName, level, d);
            }
            else
            {
                s.IsSuccess = false;
                s.Message = "Invalid data passed, please retry!";
            }

            return Json(s);

        }


        private static string GenerateRandomPassword()
        {

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@!#$%^&*_+";
            var random = new Random();

            var randomId = new string(
                Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return randomId;
        }



        public AdminUser GetAdminDetailsByUserName(string username)
        {
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var adminUser =
                    (from c in obj.AdminUsers where c.UserName == username && c.Status == "Active" select c)
                        .SingleOrDefault();
                return adminUser;
            }

        }


        public AdminUser GetAdminDetailsByAdminId(Guid adminid)
        {
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var adminUser = (from c in obj.AdminUsers where c.UserId == adminid select c).SingleOrDefault();
                return adminUser;
            }

        }

        //To save manager or admin details into database.
        public CreateAdminResultClass CreateOrUpdateAdmin(string adminId, string userName, string emailAddress,
            string firstName, string lastName, string level, Guid loggedInUserId)
        {
            CreateAdminResultClass car = new CreateAdminResultClass();
            Logger.Info("New Admin - CreateOrUpdateAdmin [" + userName + "].");
            using (NOOCHEntities obj = new NOOCHEntities())
            {

                // create new admin
                if (String.IsNullOrEmpty(adminId))
                {
                    #region to create new admin

                    if (GetAdminDetailsByUserName(userName.Trim()) == null)
                    {
                        //send default password mail to user
                        var fromAddress = Utility.GetValueFromConfig("adminMail");
                        var password = GenerateRandomPassword();
                        string encryptedPassword = CommonHelper.GetEncryptedData((password.Trim()));
                        // Add any tokens you want to find/replace within your template file
                        var tokens = new Dictionary<string, string>
                        {
                            {Constants.PLACEHOLDER_FIRST_NAME, firstName},
                            {Constants.PLACEHOLDER_LAST_NAME, lastName},
                            {Constants.PLACEHOLDER_PASSWORD, password}
                        };
                        try
                        {
                            Logger.Info("New Admin Attempt to send email [" + emailAddress + "].");
                            //Utility.SendEmail("AdminPasswordMailTemplate",
                            //fromAddress, emailAddress, 
                            //"Nooch password.",null, password,
                            //tokens, null, null, null);

                            Utility.SendEmail("AdminPasswordMailTemplate", fromAddress, emailAddress, "Nooch password.",
                                null, tokens, null, null, null);

                        }
                        catch (Exception)
                        {
                            // to revert the member record when mail is not sent successfully.
                            Logger.Error("New Admin  CreateOrUpdateAdmin - Admin default password mail not sent to [" +
                                         userName + "].");

                            car.IsSuccess = false;
                            car.Message = "Problem occured in sending password mail. Please retry.";
                            return car;

                        }

                        Guid g = Guid.NewGuid();
                        var admin = new AdminUser
                        {
                            UserId = g,
                            UserName = userName,
                            Email = emailAddress,
                            FirstName = firstName,
                            LastName = lastName,
                            AdminLevel = level,
                            Status = "Active",
                            ChangePasswordDone = false,
                            CreatedBy = Utility.ConvertToGuid(Session["UserId"].ToString()),
                            Password = password,
                            DateCreated = DateTime.Now
                        };

                        //Session["UserId"] = query.UserId;
                        //Session["RoleId"] = query.AdminLevel;
                        admin.Password = encryptedPassword;

                        obj.AdminUsers.Add(admin);
                        obj.SaveChanges();
                        car.IsSuccess = true;
                        car.Message = "Success";

                        return car;
                    }
                    car.IsSuccess = false;
                    car.Message = "User name already exists. Please try with some other name.";
                    return car;

                    #endregion
                }

                Logger.Info("New Admin - Update admin user [" + userName + "].");
                
                // edit admin details

                var id = Utility.ConvertToGuid(adminId);

                var adminUser = (from c in obj.AdminUsers where c.UserName == userName select c).SingleOrDefault();
                if (adminUser != null)
                {
                    car.IsSuccess = false;
                    car.Message = "User name already exists. Please try with some other name.";
                    return car;
                }

                return UpdateAdminUser(userName, emailAddress, firstName, lastName, level, loggedInUserId);
            }
        }


        private static CreateAdminResultClass UpdateAdminUser(string userName, string emailAddress, string firstName,
            string lastName, string level, Guid loggedInUserId)
        {
            CreateAdminResultClass car = new CreateAdminResultClass();
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var adminUser = (from c in obj.AdminUsers where c.UserName == userName select c).SingleOrDefault();
                if (adminUser != null)
                {
                    adminUser.UserName = userName;
                    adminUser.Email = emailAddress;
                    adminUser.FirstName = firstName;
                    adminUser.LastName = lastName;
                    adminUser.AdminLevel = level;
                    adminUser.DateModified = DateTime.Now;
                    adminUser.ModifiedBy = loggedInUserId;
                    obj.SaveChanges();
                }
                car.IsSuccess = true;
                car.Message = "Success";

                return car;
            }
        }


        ////////////Add new data in SDN table
        public string AddNewDataInSDN(OfacList.SDNEntity[] res)
        {
            bool b = true;

            using (var noochConnection = new NOOCHEntities())
            {
                foreach (var item in res)
                {
                    try
                    {
                        var query = new SDN
                        {
                            ent_num = item.ent_num,
                            SDN_Name = (item.SDN_Name == "-0- ") ? null : item.SDN_Name.Trim(new char[] { '"', ' ' }),
                            SDN_Type = (item.SDN_Type == "-0- ") ? null : item.SDN_Type.Trim(new char[] { '"', ' ' }),
                            Program = (item.Program == "-0- ") ? null : item.Program.Trim(new char[] { '"', ' ' }),
                            Title = (item.Title == "-0- ") ? null : item.Title.Trim(new char[] { '"', ' ' }),
                            Call_Sign = (item.Call_Sign == "-0- ") ? null : item.Call_Sign.Trim(new char[] { '"', ' ' }),
                            Vess_Type = (item.Vess_Type == "-0- ") ? null : item.Vess_Type.Trim(new char[] { '"', ' ' }),
                            Tonnage = (item.Tonnage == "-0- ") ? null : item.Tonnage.Trim(new char[] { '"', ' ' }),
                            GRT = (item.GRT == "-0- ") ? null : item.GRT.Trim(new char[] { '"', ' ' }),
                            Vess_Flag = (item.Vess_Flag == "-0- ") ? null : item.Vess_Flag.Trim(new char[] { '"', ' ' }),
                            Vess_Owner = (item.Vess_Owner == "-0- ") ? null : item.Vess_Owner.Trim(new char[] { '"', ' ' }),
                            Remarks = (item.Remarks == "-0- ") ? null : item.Remarks.Trim(new char[] { '"', ' ' })

                        };

                        noochConnection.SDNs.Add(query);
                        int i = noochConnection.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        b = false;
                    }
                }

                if (b)
                {
                    return "Records Added Successfully.";
                }
                else
                    return "something went wrong while adding SDN list, please retry.";
            }
        }


        //Insert ADD Table Data
        public string AddNewDataInADD(OfacList.ADDEntity[] res)
        {
            bool b = true;

            using (var noochConnection = new NOOCHEntities())
            {
                foreach (var item in res)
                {
                    try
                    {
                        var s = new ADD
                        {
                            ent_num = item.ent_num,
                            Add_num = item.Add_num,
                            Address = (item.Address == "-0- ") ? null : item.Address.Trim(new char[] { '"', ' ' }),
                            CityStateProvincePostalCode = (item.CityStateProvincePostalCode == "-0- ") ? null : item.CityStateProvincePostalCode.Trim(new char[] { '"', ' ' }),
                            Country = (item.Country == "-0- ") ? null : item.Country.Trim(new char[] { '"', ' ' }),
                            Add_remarks = (item.Add_remarks == "-0- ") ? null : item.Add_remarks.Trim(new char[] { '"', ' ' })

                        };

                        noochConnection.ADDs.Add(s);
                        int i = noochConnection.SaveChanges();
                    }
                    catch
                    {
                        b = false;
                    }
                }

                if (b)
                {
                    return "Records Added Successfully.";
                }
                else
                    return "something went wrong while adding ADD list, please retry.";
            }
        }


        //Add ALT Table Data
        public string AddNewDataInALT(OfacList.ALTEntity[] res)
        {
            bool b = true;

            using (var noochConnection = new NOOCHEntities())
            {
                foreach (var item in res)
                {
                    try
                    {
                        var s = new ALT
                        {
                            ent_num = item.ent_num,
                            alt_num = item.alt_num,
                            alt_type = (item.alt_type == "-0- ") ? null : item.alt_type.Trim(new char[] { '"', ' ' }),
                            alt_name = (item.alt_name == "-0- ") ? null : item.alt_name.Trim(new char[] { '"', ' ' }),
                            Country = (item.Country == "-0- ") ? null : item.Country.Trim(new char[] { '"', ' ' })
                        };

                        noochConnection.ALTs.Add(s);
                        int i = noochConnection.SaveChanges();
                    }
                    catch
                    {
                        b = false;
                    }
                }

                if (b)
                {
                    return "Records Added Successfully.";
                }
                else
                    return "something went wrong while adding ALT list, please retry.";
            }
        }
    }
}