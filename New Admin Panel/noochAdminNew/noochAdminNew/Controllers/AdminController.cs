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
using System.Text;
using System.Web.Helpers;
using noochAdminNew.Classes.PushNotification;
using Newtonsoft.Json.Linq;


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

            //var CurrentYear = DateTime.Now.Year;
            //var CurrentMonth = DateTime.Now.Month;
            //var CurrentDate = DateTime.Now.Day;

            #region Rakesh Soni's Code...
            //try
            //{
            //    using (NOOCHEntities obj = new NOOCHEntities())
            //    {
            //        if (Convert.ToInt16(operation) == 0)
            //        {
            //            try
            //            {
            //                var transtLive = (from Livetranstp in obj.Transactions
            //                                  join member in obj.Members on Livetranstp.SenderId equals member.MemberId
            //                                  join membr1 in obj.Members on Livetranstp.RecipientId equals membr1.MemberId
            //                                  join loc in obj.GeoLocations on Livetranstp.LocationId equals loc.LocationId
            //                                  where Livetranstp.TransactionDate.Value.Year == CurrentYear
            //                                  && Livetranstp.TransactionDate.Value.Month == CurrentMonth
            //                                  && Livetranstp.TransactionDate.Value.Day == CurrentDate
            //                                  orderby Livetranstp.TransactionDate descending
            //                                  select new
            //                                  {
            //                                      RecepientId = member.Nooch_ID,
            //                                      SenderId = membr1.Nooch_ID,
            //                                      TransactionDate = Livetranstp.TransactionDate,
            //                                      TransactionId = Livetranstp.TransactionTrackingId,
            //                                      SenderFirstName = member.FirstName,
            //                                      SenderLastName = member.LastName,
            //                                      Amount = Livetranstp.Amount,
            //                                      RecipientFirstName = membr1.FirstName,
            //                                      receiptLastName = membr1.LastName,
            //                                      SenderNoochId = member.Nooch_ID,
            //                                      ReceiptNoochId = membr1.Nooch_ID,
            //                                      GeoLocationState = loc.State,
            //                                      GeoLocationCity = loc.City,
            //                                      TransactionStatus = Livetranstp.TransactionStatus,
            //                                      Longitude = loc.Longitude,
            //                                      latitude = loc.Latitude,
            //                                      TransactionType = Livetranstp.TransactionType,
            //                                      disputedtrack = Livetranstp.DisputeStatus

            //                                  }).Take(10).ToList();

            //                List<MemberRecentLiveTransactionData> mm = new List<MemberRecentLiveTransactionData>();

            //                foreach (var t in transtLive)
            //                {
            //                    MemberRecentLiveTransactionData merc = new MemberRecentLiveTransactionData();
            //                    merc.Amount = t.Amount.ToString();
            //                    merc.TransID = t.TransactionId.ToString();
            //                    merc.RecepientId = t.RecepientId.ToString();
            //                    merc.SenderId = t.SenderId.ToString();
            //                    merc.TransDateTime = t.TransactionDate;
            //                    merc.SenderUserName = CommonHelper.GetDecryptedData(t.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.SenderLastName.ToString());
            //                    merc.RecepientUserName = CommonHelper.GetDecryptedData(t.RecipientFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.receiptLastName.ToString());
            //                    merc.GeoStateCityLocation = t.GeoLocationState + "," + t.GeoLocationCity;
            //                    merc.Longitude = t.Longitude.ToString();
            //                    merc.Latitude = t.latitude.ToString();
            //                    merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
            //                    merc.TransactionStatus = t.TransactionStatus;
            //                    merc.DisputeStatus = t.disputedtrack;
            //                    mm.Add(merc);
            //                }

            //                ddresult.IsSuccess = true;
            //                ddresult.Message = "SuccessOperation";
            //                ddresult.RecentLiveTransaction = mm;

            //                return Json(ddresult);
            //            }
            //            catch (Exception ex)
            //            {
            //                Logger.Error("AdminController -> ShowLiveTransactionsOnDashBoard - [Exception: " + ex + "]");

            //                ddresult.IsSuccess = false;
            //                ddresult.Message = "Exception reached - Invalid Operation";
            //                return Json(ddresult);
            //            }
            //        }
            //        else if (Convert.ToInt16(operation) == 1)
            //        {
            //            try
            //            {
            //                var transtLive = (from Livetranstp in obj.TrSendEmailansactions
            //                                  join member in obj.Members on Livetranstp.SenderId equals member.MemberId
            //                                  join membr1 in obj.Members on Livetranstp.RecipientId equals membr1.MemberId
            //                                  join loc in obj.GeoLocations on Livetranstp.LocationId equals loc.LocationId
            //                                  where SqlFunctions.DatePart("week", Livetranstp.TransactionDate) == (SqlFunctions.DatePart("week", DateTime.Now)) &&
            //                                  Livetranstp.TransactionDate.Value.Year == CurrentYear

            //                                  orderby Livetranstp.TransactionDate descending
            //                                  select new
            //                                  {
            //                                      RecepientId = member.Nooch_ID,
            //                                      SenderId = membr1.Nooch_ID,
            //                                      TransactionDate = Livetranstp.TransactionDate,
            //                                      TransactionId = Livetranstp.TransactionTrackingId,
            //                                      SenderFirstName = member.FirstName,
            //                                      SenderLastName = member.LastName,
            //                                      Amount = Livetranstp.Amount,
            //                                      RecipientFirstName = membr1.FirstName,
            //                                      receiptLastName = membr1.LastName,
            //                                      SenderNoochId = member.Nooch_ID,
            //                                      ReceiptNoochId = membr1.Nooch_ID,
            //                                      GeoLocationState = loc.State,
            //                                      GeoLocationCity = loc.City,
            //                                      TransactionStatus = Livetranstp.TransactionStatus,
            //                                      Longitude = loc.Longitude,
            //                                      latitude = loc.Latitude,
            //                                      TransactionType = Livetranstp.TransactionType,
            //                                      disputedtrack = Livetranstp.DisputeStatus

            //                                  }).Take(10).ToList();

            //                List<MemberRecentLiveTransactionData> mm = new List<MemberRecentLiveTransactionData>();

            //                foreach (var t in transtLive)
            //                {
            //                    MemberRecentLiveTransactionData merc = new MemberRecentLiveTransactionData();
            //                    merc.Amount = t.Amount.ToString();
            //                    merc.TransID = t.TransactionId.ToString();
            //                    merc.RecepientId = t.RecepientId.ToString();
            //                    merc.SenderId = t.SenderId.ToString();
            //                    merc.TransDateTime = t.TransactionDate;
            //                    merc.SenderUserName = CommonHelper.GetDecryptedData(t.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.SenderLastName.ToString());
            //                    merc.RecepientUserName = CommonHelper.GetDecryptedData(t.RecipientFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.receiptLastName.ToString());
            //                    merc.GeoStateCityLocation = t.GeoLocationState + " , " + t.GeoLocationCity;
            //                    merc.Longitude = t.Longitude.ToString();
            //                    merc.Latitude = t.latitude.ToString();
            //                    merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
            //                    merc.TransactionStatus = t.TransactionStatus;
            //                    merc.DisputeStatus = t.disputedtrack;
            //                    mm.Add(merc);
            //                }

            //                ddresult.IsSuccess = true;
            //                ddresult.Message = "SuccessOperation";
            //                ddresult.RecentLiveTransaction = mm;

            //                return Json(ddresult);
            //            }
            //            catch (Exception ex)
            //            {
            //                Logger.Error("AdminController -> ShowLiveTransactionsOnDashBoard - [Exception: " + ex + "]");

            //                ddresult.IsSuccess = false;
            //                ddresult.Message = "Exception reached - Invalid Operation";
            //                return Json(ddresult);
            //            }
            //        }
            //        else if (Convert.ToInt16(operation) == 2)
            //        {
            //            try
            //            {
            //                var transtLive = (from Livetranstp in obj.Transactions
            //                                  join member in obj.Members on Livetranstp.SenderId equals member.MemberId
            //                                  join membr1 in obj.Members on Livetranstp.RecipientId equals membr1.MemberId
            //                                  join loc in obj.GeoLocations on Livetranstp.LocationId equals loc.LocationId
            //                                  where Livetranstp.TransactionDate.Value.Year == CurrentYear
            //                                   && Livetranstp.TransactionDate.Value.Month == CurrentMonth
            //                                  orderby Livetranstp.TransactionDate descending
            //                                  select new
            //                                  {
            //                                      RecepientId = member.Nooch_ID,
            //                                      SenderId = membr1.Nooch_ID,
            //                                      TransactionDate = Livetranstp.TransactionDate,
            //                                      TransactionId = Livetranstp.TransactionTrackingId,
            //                                      SenderFirstName = member.FirstName,
            //                                      SenderLastName = member.LastName,
            //                                      Amount = Livetranstp.Amount,
            //                                      RecipientFirstName = membr1.FirstName,
            //                                      receiptLastName = membr1.LastName,
            //                                      SenderNoochId = member.Nooch_ID,
            //                                      ReceiptNoochId = membr1.Nooch_ID,
            //                                      GeoLocationState = loc.State,
            //                                      GeoLocationCity = loc.City,
            //                                      TransactionStatus = Livetranstp.TransactionStatus,
            //                                      Longitude = loc.Longitude,
            //                                      latitude = loc.Latitude,
            //                                      TransactionType = Livetranstp.TransactionType,
            //                                      disputedtrack = Livetranstp.DisputeStatus

            //                                  }).Take(10).ToList();

            //                List<MemberRecentLiveTransactionData> mm = new List<MemberRecentLiveTransactionData>();
            //                foreach (var t in transtLive)
            //                {
            //                    MemberRecentLiveTransactionData merc = new MemberRecentLiveTransactionData();
            //                    merc.Amount = t.Amount.ToString();
            //                    merc.TransID = t.TransactionId.ToString();
            //                    merc.RecepientId = t.RecepientId.ToString();
            //                    merc.SenderId = t.SenderId.ToString();
            //                    merc.TransDateTime = t.TransactionDate;
            //                    merc.SenderUserName = CommonHelper.GetDecryptedData(t.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.SenderLastName.ToString());
            //                    merc.RecepientUserName = CommonHelper.GetDecryptedData(t.RecipientFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(t.receiptLastName.ToString());
            //                    merc.GeoStateCityLocation = t.GeoLocationState + " , " + t.GeoLocationCity;
            //                    merc.Longitude = t.Longitude.ToString();
            //                    merc.Latitude = t.latitude.ToString();
            //                    merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
            //                    merc.TransactionStatus = t.TransactionStatus;
            //                    merc.DisputeStatus = t.disputedtrack;
            //                    mm.Add(merc);
            //                }

            //                ddresult.IsSuccess = true;
            //                ddresult.Message = "SuccessOperation";
            //                ddresult.RecentLiveTransaction = mm;

            //                return Json(ddresult);
            //            }
            //            catch (Exception ex)
            //            {
            //                Logger.Error("AdminController -> ShowLiveTransactionsOnDashBoard - [Exception: " + ex + "]");

            //                ddresult.IsSuccess = false;
            //                ddresult.Message = "Exception reached - Invalid Operation";
            //                return Json(ddresult);
            //            }
            //        }
            //        else
            //        {
            //            ddresult.IsSuccess = false;
            //            ddresult.Message = "InValid Operation";
            //            return Json(ddresult);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error("AdminController -> ShowLiveTransactionsOnDashBoard - [Outer Exception: " + ex + "]");

            //    ddresult.IsSuccess = false;
            //    ddresult.Message = "Exception reached - Invalid Operation";
            //    return Json(ddresult);
            //} 
            #endregion

            #region Bill Gate's Code.....
            //:D  Keeping rakesh soni's code just in case if new code breaks anything on live... will revert back to original code.
            // CLIFF (5/15/16)... not sure what the above note means or which code is supposed to be correct.  Please make sure to remove old notes to keep code clean.
            //                    And please delete old code if it is not going to be used, don't just leave huge blocks of commented out code.
            try
            {
                using (NOOCHEntities obj = new NOOCHEntities())
                {
                    List<GetLiveTransactionsForDashboard_Result1> allTrans = obj.GetLiveTransactionsForDashboard(Convert.ToInt16(operation)).ToList();
                    List<MemberRecentLiveTransactionData> mm = new List<MemberRecentLiveTransactionData>();

                    if (allTrans.Count > 0)
                    {
                        foreach (var t in allTrans)
                        {
                            MemberRecentLiveTransactionData singleTrans = new MemberRecentLiveTransactionData();
                            singleTrans.Amount = t.Amount.ToString();
                            singleTrans.TransID = t.TransactionId.ToString();
                            singleTrans.TransDateTime = t.TransactionDate;
                            singleTrans.GeoStateCityLocation = t.GeoLocationCity + ", " + t.GeoLocationState;
                            singleTrans.Longitude = t.Longitude.ToString();
                            singleTrans.Latitude = t.Latitude.ToString();
                            singleTrans.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
                            singleTrans.TransactionStatus = t.TransactionStatus;
                            singleTrans.DisputeStatus = t.DisputeStatus;

                            //check to see if this transaction has record in SynapseAddTransactionResults table
                            //var SynapseAddTransactionResults = obj.SynapseAddTransactionResults.Where(tr => tr.TransactionId == t.TransactionId).FirstOrDefault();

                            //if (SynapseAddTransactionResults!=null)
                            //    singleTrans.SynapseStatus = showTransactionStatus(t.TransactionId.ToString(), SynapseAddTransactionResults.OidFromSynapse);

                            //else
                            //    singleTrans.SynapseStatus = t.SynapseStatus;

                            #region Request type transaction

                            // request type transaction b/w existing nooch users
                            if (singleTrans.TransactionType == "Request")
                            {
                                singleTrans.TransactionStatus = t.TransactionStatus == "Success"
                                                            ? "Complete (Paid)"
                                                            : t.TransactionStatus;

                                if (t.RecipientId != t.SenderId)
                                {
                                    singleTrans.SenderUserName = CommonHelper.GetMemberNameFromMemberId(t.SenderId.ToString());
                                    singleTrans.RecepientUserName = CommonHelper.GetMemberNameFromMemberId(t.RecipientId.ToString());
                                    singleTrans.RecepientId = t.RecipientId.ToString();
                                    singleTrans.SenderId = t.SenderId.ToString();
                                }
                                // request type trans to non nooch user...by phone
                                else if (t.RecipientId == t.SenderId && t.IsPhoneInvitation == true)
                                {
                                    if (!String.IsNullOrEmpty(t.PhoneNumberInvited))
                                    {
                                        singleTrans.SenderUserName = CommonHelper.FormatPhoneNumber(CommonHelper.GetDecryptedData(t.PhoneNumberInvited));
                                    }
                                    else
                                    {
                                        singleTrans.SenderUserName = "";
                                    }
                                    singleTrans.RecepientUserName = CommonHelper.GetMemberNameFromMemberId(t.RecipientId.ToString());
                                    singleTrans.RecepientId = t.RecipientId.ToString();
                                    singleTrans.SenderId = "";
                                }

                                // request type trans to non nooch user...by email
                                else if (t.RecipientId == t.SenderId && t.InvitationSentTo != null)
                                {
                                    Member m = obj.Members.Where(mmObj => mmObj.UserName == t.InvitationSentTo && mmObj.IsDeleted == false).FirstOrDefault();
                                    if (m != null)
                                        singleTrans.SenderUserName = CommonHelper.GetMemberNameFromMemberId(m.MemberId.ToString());
                                    else
                                        singleTrans.SenderUserName = !String.IsNullOrEmpty(t.InvitationSentTo) ? CommonHelper.GetDecryptedData(t.InvitationSentTo) : "";
                                    singleTrans.RecepientUserName = CommonHelper.GetMemberNameFromMemberId(t.RecipientId.ToString());
                                    singleTrans.RecepientId = t.RecipientId.ToString();
                                    singleTrans.SenderId = "";
                                }
                            }
                            #endregion

                            #region Invite type transaction

                            // invite type trans to non nooch user...by phone
                            else if (singleTrans.TransactionType == "Invite" && t.RecipientId == t.SenderId && t.IsPhoneInvitation == true)
                            {
                                if (!String.IsNullOrEmpty(t.PhoneNumberInvited))
                                {
                                    singleTrans.RecepientUserName =
                                         CommonHelper.FormatPhoneNumber(
                                             CommonHelper.GetDecryptedData(t.PhoneNumberInvited));
                                }
                                else
                                {
                                    singleTrans.RecepientUserName =
                                        "";
                                }
                                singleTrans.SenderUserName = CommonHelper.GetMemberNameFromMemberId(t.RecipientId.ToString());
                                singleTrans.RecepientId = "";
                                singleTrans.SenderId = t.SenderId.ToString();
                            }

                            // Invite type trans to non nooch user...by email
                            else if (singleTrans.TransactionType == "Invite" && t.RecipientId == t.SenderId && t.InvitationSentTo != null)
                            {
                                if (!String.IsNullOrEmpty(t.InvitationSentTo))
                                {
                                    Member m = obj.Members.Where(mmObj => mmObj.UserName == t.InvitationSentTo && mmObj.IsDeleted == false).FirstOrDefault();
                                    if (m != null)
                                        singleTrans.RecepientUserName = CommonHelper.GetMemberNameFromMemberId(m.MemberId.ToString());
                                    else
                                        singleTrans.RecepientUserName = CommonHelper.GetDecryptedData(t.InvitationSentTo);
                                }
                                else
                                {
                                    singleTrans.RecepientUserName =
                                        "";
                                }
                                singleTrans.SenderUserName = CommonHelper.GetMemberNameFromMemberId(t.RecipientId.ToString());
                                singleTrans.RecepientId = "";
                                singleTrans.SenderId = t.SenderId.ToString();
                            }

                            #endregion

                            #region Transfer, dispute or reward type transaction

                            // transfer type trans to non nooch user...by phone
                            else if (singleTrans.TransactionType == "Transfer")
                            {
                                singleTrans.RecepientUserName = CommonHelper.GetMemberNameFromMemberId(t.RecipientId.ToString());
                                singleTrans.SenderUserName = CommonHelper.GetMemberNameFromMemberId(t.SenderId.ToString());
                                singleTrans.RecepientId = t.RecipientId.ToString();
                                singleTrans.SenderId = t.SenderId.ToString();
                            }

                            // request could be disputed, reward type
                            else
                            {
                                singleTrans.RecepientUserName = CommonHelper.GetMemberNameFromMemberId(t.RecipientId.ToString());
                                singleTrans.SenderUserName = CommonHelper.GetMemberNameFromMemberId(t.SenderId.ToString());
                                singleTrans.RecepientId = t.RecipientId.ToString();
                                singleTrans.SenderId = t.SenderId.ToString();
                            }

                            #endregion

                            mm.Add(singleTrans);
                        }
                    }

                    ddresult.IsSuccess = true;
                    ddresult.Message = "SuccessOperation";
                    ddresult.RecentLiveTransaction = mm;

                    return Json(ddresult);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Cntrlr -> ShowLiveTransactionsOnDashBoard - [Outer Exception: " + ex + "]");

                ddresult.IsSuccess = false;
                ddresult.Message = "Exception reached - Invalid Operation";
                return Json(ddresult);
            }
            #endregion
        }


        public ActionResult Dashboard()
        {
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
                    var c = (from t in obj.Members
                             where t.IsDeleted == false
                             select t).ToList();
                    dd.TotalActiveUsers = c.Count;

                    // # of Active Users - TODAY
                    //c = (from t in obj.Members
                    //     where t.IsDeleted == false && t.DateCreated.Value.Day == CurrentDate && t.DateCreated.Value.Year == CurrentYear && t.DateCreated.Value.Month == CurrentMonth
                    //     select t).ToList();
                    dd.TotalNoOfActiveUser_Today = obj.GetDashboardStats("NEW Users", "Today").SingleOrDefault() ?? 0;

                    // # of Active Users - THIS MONTH
                    //c = (from t in obj.Members
                    //     where t.IsDeleted == false &&
                    //           t.DateCreated.Value.Year == CurrentYear &&
                    //           t.DateCreated.Value.Month == CurrentMonth
                    //     select t).ToList();
                    dd.TotalNoOfActiveUser_Month = obj.GetDashboardStats("NEW Users", "This month").SingleOrDefault() ?? 0;

                    // # of Active Users - THIS WEEK
                    //c = (from t in obj.Members
                    //     where t.IsDeleted == false && SqlFunctions.DatePart("week", t.DateCreated) == (SqlFunctions.DatePart("week", DateTime.Now))
                    //     select t).ToList();
                    dd.TotalNoOfActiveUser_Week = obj.GetDashboardStats("NEW Users", "This week").SingleOrDefault() ?? 0;

                    // # of Phones Verified - TODAY
                    //c = (from t in obj.Members
                    //     where t.IsDeleted == false &&
                    //           t.PhoneVerifiedOn.Value.Day == CurrentDate &&
                    //           t.PhoneVerifiedOn.Value.Year == CurrentYear &&
                    //           t.PhoneVerifiedOn.Value.Month == CurrentMonth
                    //     select t).ToList();
                    dd.TotalNoOfVerifiedPhoneUsers_Today = obj.GetDashboardStats("NEW Verified Phones", "Today").SingleOrDefault() ?? 0;

                    // # of Phones Verified - THIS MONTH
                    //c = (from t in obj.Members
                    //     where t.IsDeleted == false &&
                    //           t.PhoneVerifiedOn.Value.Year == CurrentYear &&
                    //           t.PhoneVerifiedOn.Value.Month == CurrentMonth
                    //     select t).ToList();
                    dd.TotalNoOfVerifiedPhoneUsers_Month = obj.GetDashboardStats("NEW Verified Phones", "This month").SingleOrDefault() ?? 0;

                    // # of Phones Verified - THIS WEEK
                    //c = (from t in obj.Members
                    //     where t.IsDeleted == false &&
                    //           SqlFunctions.DatePart("week", t.PhoneVerifiedOn) == (SqlFunctions.DatePart("week", DateTime.Now))
                    //     select t).ToList();
                    dd.TotalNoOfVerifiedPhoneUsers_Week = obj.GetDashboardStats("NEW Verified Phones", "This week").SingleOrDefault() ?? 0;

                    // # of Emails Verified - TODAY
                    //c = (from t in obj.AuthenticationTokens
                    //     join mem in obj.Members on t.MemberId equals mem.MemberId
                    //     where mem.IsDeleted == false && t.IsActivated == true
                    //         && t.VerifiedOn.Value.Day == CurrentDate && t.VerifiedOn.Value.Year == CurrentYear && t.VerifiedOn.Value.Month == CurrentMonth
                    //     select mem).ToList();
                    dd.TotalNoOfVerifiedEmailUsers_Today = obj.GetDashboardStats("NEW Verified Email", "Today").SingleOrDefault() ?? 0;

                    // # of Emails Verified - THIS MONTH
                    //c = (from t in obj.AuthenticationTokens
                    //     join mem in obj.Members on t.MemberId equals mem.MemberId
                    //     where mem.IsDeleted == false
                    //         && t.IsActivated == true && t.VerifiedOn.Value.Year == CurrentYear && t.VerifiedOn.Value.Month == CurrentMonth
                    //     select mem).ToList();
                    dd.TotalNoOfVerifiedEmailUsers_Month = obj.GetDashboardStats("NEW Verified Email", "This month").SingleOrDefault() ?? 0;

                    // # of Emails Verified - THIS WEEK
                    //c = (from t in obj.AuthenticationTokens
                    //     join mem in obj.Members on t.MemberId equals mem.MemberId
                    //     where mem.IsDeleted == false
                    //         && t.IsActivated == true && SqlFunctions.DatePart("week", t.VerifiedOn) == (SqlFunctions.DatePart("week", DateTime.Now))
                    //     select mem).ToList();
                    dd.TotalNoOfVerifiedEmailUsers_Week = Convert.ToInt16(obj.GetDashboardStats("NEW Verified Email", "This week").SingleOrDefault().ToString());

                    c = (from t in obj.Members
                         where t.IsDeleted == false &&
                               t.Status == "Registered"
                         select t).ToList();
                    dd.TotalRegisteredUsers = c.Count;

                    c = (from t in obj.Members
                         where t.IsDeleted == false &&
                               t.Status == "Active"
                         select t).ToList();
                    dd.TotalVerifiedEmailUsers = c.Count;

                    c = (from t in obj.Members
                         where t.IsDeleted == false &&
                               t.Status == "Suspended"
                         select t).ToList();
                    dd.TotalSuspendedUsers = c.Count;

                    c = (from t in obj.Members
                         where t.IsDeleted == false &&
                               t.IsVerifiedPhone == true
                         select t).ToList();
                    dd.TotalVerifiedPhoneUsers = c.Count;

                    c = (from t in obj.Members
                         where t.IsDeleted == false && t.Status == "Active" && t.IsVerifiedPhone == true
                         select t).ToList();
                    dd.TotalVerifiedPhoneAndEmailUsers = c.Count;

                    c = (from t in obj.Members
                         where t.IsVerifiedWithSynapse == true &&
                               t.Status == "Active" &&
                               t.IsVerifiedPhone == true
                         select t).ToList();

                    dd.TotalActiveAndVerifiedBankAccountUsers = c.Count;

                    dd.TotalActiveBankAccountUsers = c.Count;

                    dd.TotalAmountOfDollars = (from r in obj.Transactions
                                               where r.TransactionStatus == "Success"
                                               select r).ToList()
                                                    .Sum(t => t.Amount).ToString();

                    dd.TotalNoOfPaymentsCompleted = (from r in obj.Transactions
                                                     where r.TransactionStatus == "Success"
                                                     select r).ToList().Count;

                    dd.totalRequestTypeTrans = (from r in obj.Transactions
                                                where r.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw==" && r.TransactionStatus == "Success"
                                                select r).ToList().Count;

                    dd.TransactionsPendi = (from r in obj.Transactions
                                            where r.TransactionStatus == "Pending"
                                            select r).ToList().Count;


                    // have to do it long way because we have bank_name as encrypted in db
                    var allSyanpseSupportedBanks = (from ce in obj.SynapseSupportedBanks
                                                    where ce.IsDeleted == false
                                                    select ce).ToList();

                    var membersInEachBank = obj.GetMembersInEachSynapseBank().ToList();

                    List<GetMembersInEachSynapseBank_Result> decryptedList = new List<GetMembersInEachSynapseBank_Result>();

                    foreach (GetMembersInEachSynapseBank_Result mem in membersInEachBank)
                    {
                        GetMembersInEachSynapseBank_Result m = new GetMembersInEachSynapseBank_Result();
                        m.bank_name = CommonHelper.GetDecryptedData(mem.bank_name).ToLower().Trim();
                        m.CountInBank = mem.CountInBank;
                        decryptedList.Add(m);
                    }

                    List<NoOfUsersInEachBank> UserCountInEachBankPrep = new List<NoOfUsersInEachBank>();

                    foreach (SynapseSupportedBank ssb in allSyanpseSupportedBanks)
                    {
                        NoOfUsersInEachBank nusi = new NoOfUsersInEachBank();
                        nusi.BankName = ssb.BankName;
                        nusi.NoOfUsers = 0;
                        foreach (GetMembersInEachSynapseBank_Result res in decryptedList)
                        {
                            if (res.bank_name == ssb.BankName.ToLower().Trim())
                            {
                                nusi.NoOfUsers = Convert.ToInt16(res.CountInBank);
                            }
                        }

                        UserCountInEachBankPrep.Add(nusi);
                    }

                    dd.UserCountInEachBank = UserCountInEachBankPrep;
                }

                return View(dd);
            }
        }


        public void RelinkBankNotification()
        {
            Logger.Info("Admin Cntrlr -> sending notification to all users to re link their bank node");

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var membersWithABank = (from m in obj.Members
                                        join s in obj.SynapseBanksOfMembers
                                        on m.MemberId equals s.MemberId
                                        where m.IsDeleted == false && s.IsDefault == true
                                        select m).ToList();

                foreach (var member in membersWithABank)
                {
                    var toAddress = CommonHelper.GetDecryptedData(member.UserName);

                    try
                    {
                        var fromAddress = Utility.GetValueFromConfig("adminMail");
                        var memberId = member.MemberId.ToString();
                        var isRentSceneClient = member.InviteCodeId.ToString().ToLower() == "b43a36a6-1da5-47ce-a56c-6210f9ddbd22" ? "yes" : "false";
                        var companyName = isRentSceneClient == "yes" ? "Rent Scene" : "Nooch";

                        var link = "https://www.noochme.com/Nooch/createAccount?memId=" + memberId + "&type=1&update=true&rs=" + isRentSceneClient;

                        var tokens = new Dictionary<string, string>
	                                    {
	                                        {Constants.PLACEHOLDER_FIRST_NAME, CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(member.FirstName.ToString()))},
	                                        {Constants.MEMO, link}
	                                    };

                        Utility.SendEmail("relinkBankAccount", fromAddress, toAddress,
                                          "Important Update for " + companyName + " Payments - **Action Required**",
                                          null, tokens, null, null, null);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Admin Cntrlr -> RelinkBankNotification FAILED - Error sending email to: [" + toAddress +
                                     "], MemberID: [" + member.MemberId.ToString() + "], Exception: [" + ex.Message + "]");
                    }
                }
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
        [ActionName("CreditFundToMemberPostSynapseV3Test")]
        public ActionResult CreditFundToMemberPostSynapseV3Test(string transferfundto, string transferAmount, string transferNotes, string adminPin)
        {
            LoginResult res = new LoginResult();
            res.IsSuccess = false;

            #region Input Validations

            if (String.IsNullOrEmpty(transferfundto))
            {
                res.Message = "Please enter user name or NoochId of Member to transfer fund.";
            }
            if (String.IsNullOrEmpty(transferAmount))
            {
                res.Message = "Please enter transfer fund amount";
            }

            if (String.IsNullOrEmpty(transferNotes))
            {
                res.Message = "Please enter transfer notes.";
            }
            if (String.IsNullOrEmpty(adminPin))
            {
                res.Message = "Please enter admin pin.";
            }

            #endregion Input Validations


            // Check admin user details
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                string adminPinEncrypted = CommonHelper.GetEncryptedData(adminPin.Trim());

                var adminUserDetails =
                    (from c in obj.Members
                     where c.UserName == "z2/de4EMabGlzMuO7OocHw==" &&
                           c.Status == "Active" &&
                           c.PinNumber == adminPinEncrypted
                     select c).SingleOrDefault();

                if (adminUserDetails != null)
                {
                    #region Get Admin Member's Synapse Details

                    Guid AdminMemberId = Utility.ConvertToGuid(adminUserDetails.MemberId.ToString());

                    // Get Synapse account details of admin
                    var adminSynapseDetails = CommonHelper.GetSynapseBankAndUserDetailsforGivenMemberId(AdminMemberId.ToString());

                    if (adminSynapseDetails.wereBankDetailsFound != true)
                    {
                        Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Transfer ABORTED: Requester's Synapse bank account NOT FOUND - Trans Creator MemberId is: [" + AdminMemberId + "]");
                        res.Message = "Admin does not have any bank added";

                        return Json(res);
                    }

                    // Check Admins's Synapse Bank Account status
                    if (adminSynapseDetails.BankDetails != null &&
                        adminSynapseDetails.BankDetails.Status != "Verified" &&
                        adminUserDetails.IsVerifiedWithSynapse != true)
                    {
                        Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Admin's Synapse bank account exists but is not Verified and " +
                            "isVerifiedWithSynapse != true - Admin memberId is: [" + adminUserDetails.MemberId + "]");
                        res.Message = "Admin does not have any verified bank account.";

                        return Json(res);
                    }

                    #endregion Get Admin Member's Synapse Details

                    // Money recepient Member and Synapse Bank Acount Details
                    string recepientusernameencrypted = CommonHelper.GetEncryptedData(transferfundto.ToLower());
                    var recipientMemberDetails = (from c in obj.Members
                                                  where c.Nooch_ID == transferfundto ||
                                                        c.UserName == recepientusernameencrypted &&
                                                        c.Status == "Active"
                                                  select c).SingleOrDefault();

                    if (recipientMemberDetails != null)
                    {
                        // Now check recipient's Synapse details
                        Guid recepeintGuid = Utility.ConvertToGuid(recipientMemberDetails.MemberId.ToString());

                        var recipientBankDetails = CommonHelper.GetSynapseBankAndUserDetailsforGivenMemberId(recepeintGuid.ToString());

                        if (recipientBankDetails.wereBankDetailsFound != true)
                        {
                            Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Transfer ABORTED: Recepient's Synapse bank account NOT FOUND - Trans Creator MemberId is: [" + recepeintGuid + "]");
                            res.Message = "Recepient does not have any bank added";

                            return Json(res);
                        }

                        // Check Admins's Synapse Bank Account status
                        if (recipientBankDetails.BankDetails != null &&
                            recipientBankDetails.BankDetails.Status != "Verified" &&
                            recipientMemberDetails.IsVerifiedWithSynapse != true)
                        {
                            Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Recepient's Synapse bank account exists but is not Verified and " +
                                         "isVerifiedWithSynapse != true - Recepient memberId is: [" + adminUserDetails.MemberId + "]");
                            res.Message = "Recepient does not have any verified bank account.";

                            return Json(res);
                        }


                        // have admin and recepient all details to transfer money
                        #region Define Variables From Transaction for Notifications

                        Guid TransactionIdToUse = Guid.NewGuid();
                        DateTime TransactionDateTimeToUSe = DateTime.Now;
                        var fromAddress = Utility.GetValueFromConfig("transfersMail");
                        string senderUserName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(adminUserDetails.UserName));
                        string senderFirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(adminUserDetails.FirstName));
                        string senderLastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(adminUserDetails.LastName));
                        string recipientFirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(recipientMemberDetails.FirstName));
                        string recipientLastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(recipientMemberDetails.LastName));
                        string receiverUserName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(recipientMemberDetails.UserName));

                        var sender_synapse_Bank_Id = adminSynapseDetails.BankDetails.bankid.ToString();
                        decimal transactionAmount = Convert.ToDecimal(transferAmount);

                        string wholeAmount = transactionAmount.ToString("n2");
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
                        if (!String.IsNullOrEmpty(transferNotes))
                        {
                            if (transferNotes.Length > 3)
                            {
                                string firstThreeChars = transferNotes.Substring(0, 3).ToLower();
                                bool startsWithFor = firstThreeChars.Equals("for");

                                if (startsWithFor)
                                {
                                    memo = transferNotes.ToString();
                                }
                                else
                                {
                                    memo = "For: " + transferNotes.ToString();
                                }
                            }
                            else
                            {
                                memo = "For: " + transferNotes.ToString();
                            }
                        }

                        string senderPic = "https://www.noochme.com/noochweb/Assets/Images/userpic-default.png";
                        string recipientPic;

                        #endregion Define Variables From Transaction for Notifications

                        short shouldSendFailureNotifications = 0;
                        int saveToSynapseCreateOrderTable = 0;
                        int saveToTransTable = 0;
                        // Make call to SYNAPSE Order API service
                        try
                        {
                            #region Query Synapse Order API

                            string sender_oauth = CommonHelper.GetDecryptedData(adminSynapseDetails.UserDetails.access_token);
                            string sender_fingerPrint = adminUserDetails.UDID1;
                            string sender_bank_node_id = adminSynapseDetails.BankDetails.oid.ToString();
                            string amount = transferAmount;
                            string fee = "0";
                            if (transactionAmount > 10)
                            {
                                fee = "0.20"; //to offset the Synapse fee so the user doesn't pay it
                            }
                            else if (transactionAmount < 10)
                            {
                                fee = "0.10"; //to offset the Synapse fee so the user doesn't pay it
                            }
                            string receiver_oauth = CommonHelper.GetDecryptedData(recipientBankDetails.UserDetails.access_token);
                            string receiver_fingerprint = recipientMemberDetails.UDID1;
                            string receiver_bank_node_id = recipientBankDetails.BankDetails.oid.ToString();
                            string suppID_or_transID = TransactionIdToUse.ToString();

                            string iPForTransaction = CommonHelper.GetRecentOrDefaultIPOfMember(AdminMemberId);

                            SynapseV3AddTrans_ReusableClass transactionResultFromSynapseAPI =
                                CommonHelper.AddTransSynapseV3Reusable(sender_oauth, sender_fingerPrint,
                                    sender_bank_node_id,
                                    amount, fee, receiver_oauth, receiver_fingerprint, receiver_bank_node_id,
                                    suppID_or_transID,
                                    senderUserName, receiverUserName, iPForTransaction, senderLastName,
                                    recipientLastName);



                            if (transactionResultFromSynapseAPI.success == true)
                            {

                                #region Save info in SynapseCreateOrder Table


                                try
                                {


                                    #region save to synapse create trans table returned success -- now saving in Transactions table

                                    if (saveToSynapseCreateOrderTable > 0)
                                    {
                                        #region Save info in Transaction Details table

                                        Transaction transactionDetail = new Transaction();


                                        transactionDetail.TransactionTrackingId =
                                            CommonHelper.GetRandomTransactionTrackingId();
                                        transactionDetail.TransactionStatus = "Success";
                                        transactionDetail.Memo = transferNotes.Trim();

                                        transactionDetail.Amount = Convert.ToDecimal(transferAmount);


                                        transactionDetail.TransactionId = TransactionIdToUse;
                                        transactionDetail.TransactionDate = TransactionDateTimeToUSe;
                                        transactionDetail.DisputeStatus = null;

                                        transactionDetail.TransactionType = CommonHelper.GetEncryptedData("Reward");
                                        // @cliff what type it will be of ?

                                        transactionDetail.TransactionFee = 0;
                                        transactionDetail.SenderId = AdminMemberId;
                                        transactionDetail.RecipientId = recipientMemberDetails.MemberId;


                                        obj.Transactions.Add(transactionDetail);

                                        saveToTransTable = obj.SaveChanges();


                                        #endregion Save info in Transaction Details table
                                    }

                                    #endregion


                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(
                                        "Add fund to members account New Admin -> Transfer FAILED ->  [Exception: " + ex +
                                        "]");
                                }

                                #endregion Save info in SynapseCreateOrder Table

                            }
                            else
                            {
                                // synapse API call failed
                                shouldSendFailureNotifications = 1;
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Add fund to members account New Admin -> Transfer FAILED ->  [Exception: " + ex.Message + "]");
                            shouldSendFailureNotifications = 1;
                        }

                        #region Failure Sections


                        if (shouldSendFailureNotifications == 1 && saveToSynapseCreateOrderTable == 0 &&
                            saveToTransTable == 0)
                        {
                            // error while making order API 
                            Logger.Info("Add fund to members account New Admin -> Transfer FAILED. Error occur in call order API");
                            // Check if there was a failure above and we need to send the failure Email/SMS notifications to the sender.
                            if (shouldSendFailureNotifications > 0)
                            {
                                Logger.Info("Add fund to members account New Admin - THERE WAS A FAILURE - Sending Failure Notifications to both Users");

                                #region Notify Sender about failure

                                var senderNotificationSettings = CommonHelper.GetMemberNotificationSettings(adminUserDetails.MemberId.ToString());

                                if (senderNotificationSettings != null)
                                {
                                    #region Push Notification to Sender about failure


                                    if (senderNotificationSettings.TransferAttemptFailure == true)
                                    {
                                        string senderDeviceId = senderNotificationSettings != null ? adminUserDetails.DeviceToken : null;

                                        string mailBodyText = "Your attempt to send $" + transactionAmount.ToString("n2") +
                                                              " to " + recipientFirstName + " " + recipientLastName + " failed ;-(  Contact Nooch support for more info.";

                                        if (!String.IsNullOrEmpty(senderDeviceId))
                                        {
                                            try
                                            {
                                                ApplePushNotification.SendNotificationMessage(mailBodyText, 0, null, senderDeviceId,
                                                                                            Utility.GetValueFromConfig("AppKey"),
                                                                                            Utility.GetValueFromConfig("MasterSecret"));

                                                Logger.Info("Add fund to members account New Admin --> TransferMoneyUsingSynapse FAILED - Push notif sent to Sender: [" +
                                                    senderFirstName + " " + senderLastName + "] successfully.");
                                            }
                                            catch (Exception ex)
                                            {
                                                Logger.Info(
                                                    "Add fund to members account New Admin --> TransferMoneyUsingSynapse FAILED - Push notif FAILED also, SMS NOT sent to [" +
                                                    senderFirstName + " " + senderLastName + "],  [Exception: " + ex + "]");
                                            }
                                        }
                                    }

                                    #endregion Push Notification to Sender about failure

                                    #region Email notification to Sender about failure

                                    if (senderNotificationSettings.EmailTransferAttemptFailure ?? false)
                                    {
                                        var tokens = new Dictionary<string, string>
	                                {
	                                    {Constants.PLACEHOLDER_FIRST_NAME, senderFirstName + " " + senderLastName},
	                                    {Constants.PLACEHOLDER_FRIEND_FIRST_NAME, recipientFirstName + " " + recipientLastName},
	                                    {Constants.PLACEHOLDER_TRANSFER_AMOUNT, dl},
	                                    {Constants.PLACEHLODER_CENTS, ce},
	                                };

                                        var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

                                        try
                                        {
                                            Utility.SendEmail("transferFailure",
                                                fromAddress, toAddress, "Nooch transfer failure :-(", null,
                                                tokens, null, null, null);

                                            Logger.Info("Add fund to members account New Admin -> TransferMoneyUsingSynapse FAILED - Email sent to Sender: [" +
                                                toAddress + "] successfully.");
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Info("Add fund to members account New Admin -> TransferMoneyUsingSynapse --> Error: TransferAttemptFailure mail " +
                                                                   "NOT sent to [" + toAddress + "],  [Exception: " + ex + "]");
                                        }
                                    }

                                    #endregion Email notification to Sender about failure
                                }

                                #endregion Notify Sender about failure

                                if (shouldSendFailureNotifications == 1)
                                {
                                    res.Message = "There was a problem with Synapse.";
                                    return Json(res);

                                }
                                else if (saveToTransTable == 0 || saveToSynapseCreateOrderTable == 0)
                                {
                                    res.Message = "There was a problem updating Nooch DB tables.";
                                    return Json(res);
                                }
                                else
                                {
                                    res.Message = "Unknown Failure";
                                    return Json(res);

                                }
                            }
                        }

                        #endregion Failure Sections

                        else if (shouldSendFailureNotifications == 0 &&
                                 saveToSynapseCreateOrderTable == 1 &&
                                 saveToTransTable == 1)
                        {
                            #region Success Notifications
                            #region Send Email to Sender on transfer success

                            var sendersNotificationSets = CommonHelper.GetMemberNotificationSettings(adminUserDetails.MemberId.ToString());

                            if (sendersNotificationSets != null)
                            {
                                if (sendersNotificationSets != null && (sendersNotificationSets.EmailTransferSent ?? false))
                                {
                                    if (!String.IsNullOrEmpty(recipientMemberDetails.Photo) && recipientMemberDetails.Photo.Length > 20)
                                    {
                                        recipientPic = recipientMemberDetails.Photo.ToString();
                                    }

                                    var tokens = new Dictionary<string, string>
	                                    {
	                                        {Constants.PLACEHOLDER_FIRST_NAME, senderFirstName},
	                                        {Constants.PLACEHOLDER_FRIEND_FIRST_NAME, recipientFirstName + " " + recipientLastName},
	                                        {Constants.PLACEHOLDER_TRANSFER_AMOUNT, dl},
	                                        {Constants.PLACEHLODER_CENTS, ce},
	                                        {Constants.MEMO, memo}
	                                    };

                                    var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

                                    try
                                    {
                                        Utility.SendEmail("TransferSent", fromAddress, toAddress,
                                            "Your $" + wholeAmount + " payment to " + recipientFirstName + " on Nooch",
                                            null, tokens, null, null, null);

                                        Logger.Info("Add fund to members account New Admin -> TransferSent email sent to [" +
                                            toAddress + "] successfully");
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("Add fund to members account New Admin  -> EMAIL TO RECIPIENT FAILED: TransferReceived Email NOT sent to [" +
                                            toAddress + "], [Exception: " + ex + "]");
                                    }
                                }
                            }

                            #endregion Send Email to Sender on transfer success

                            #region Send Notifications to Recipient on transfer success

                            var recipNotificationSets = CommonHelper.GetMemberNotificationSettings(recipientMemberDetails.MemberId.ToString());

                            if (recipNotificationSets != null)
                            {
                                // First, send push notification
                                #region Push notification to Recipient

                                if ((recipNotificationSets.TransferReceived == null)
                                    ? false
                                    : recipNotificationSets.TransferReceived.Value)
                                {
                                    string recipDeviceId = recipNotificationSets != null ? recipientMemberDetails.DeviceToken : null;

                                    string pushBodyText = "You received $" + wholeAmount + " from " + senderFirstName +
                                                          " " + senderLastName + "! Spend it wisely :-)";
                                    try
                                    {
                                        if (recipNotificationSets != null &&
                                            !String.IsNullOrEmpty(recipDeviceId) &&
                                            (recipNotificationSets.TransferReceived ?? false))
                                        {
                                            ApplePushNotification.SendNotificationMessage(pushBodyText, 1,
                                                null, recipDeviceId,
                                                Utility.GetValueFromConfig("AppKey"),
                                                Utility.GetValueFromConfig("MasterSecret"));

                                            Logger.Info(
                                                "Add fund to members account New Admin -> SUCCESS - Push notification sent to Recipient [" +
                                                recipientFirstName + " " + recipientLastName + "] successfully.");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(
                                            "Add fund to members account New Admin  -> Success - BUT Push notification FAILURE - Push to Recipient NOT sent [" +
                                                recipientFirstName + " " + recipientLastName + "], Exception: [" + ex + "]");
                                    }
                                }

                                #endregion Push notification to Recipient

                                // Now send email notification
                                #region Email notification to Recipient

                                if (recipNotificationSets != null && (recipNotificationSets.EmailTransferReceived ?? false))
                                {
                                    if (!String.IsNullOrEmpty(adminUserDetails.Photo) && adminUserDetails.Photo.Length > 20)
                                    {
                                        senderPic = adminUserDetails.Photo.ToString();
                                    }

                                    var tokensR = new Dictionary<string, string>
	                                        {
	                                            {Constants.PLACEHOLDER_FIRST_NAME, recipientFirstName},
	                                            {Constants.PLACEHOLDER_FRIEND_FIRST_NAME, senderFirstName + " " + senderLastName},
                                                {"$UserPicture$", senderPic},
	                                            {Constants.PLACEHOLDER_TRANSFER_AMOUNT, wholeAmount},
	                                            {Constants.PLACEHOLDER_TRANSACTION_DATE, TransactionDateTimeToUSe.ToString("MMM dd")},
	                                            {Constants.MEMO, memo}
	                                        };

                                    var toAddress2 = CommonHelper.GetDecryptedData(recipientMemberDetails.UserName);

                                    try
                                    {
                                        Utility.SendEmail("TransferReceived", fromAddress, toAddress2,
                                            senderFirstName + " sent you $" + wholeAmount + " with Nooch", null, tokensR, null, null, null);

                                        Logger.Info("Add fund to members account New Admin  ->  TransferReceived Email sent to [" +
                                            toAddress2 + "] successfully");
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(
                                            "Add fund to members account New Admin  -> EMAIL TO RECIPIENT FAILED: TransferReceived Email NOT sent to [" +
                                            toAddress2 + "], [Exception: " + ex + "]");
                                    }
                                }

                                #endregion Email notification to Recipient
                            }

                            #endregion Send Notifications to Recipient on transfer success

                            #endregion Success Notifications

                            res.Message = "Your cash was sent successfully.";
                            res.IsSuccess = true;
                            return Json(res);
                        }
                        else
                        {
                            res.Message = "Server Error.";
                        }
                    }
                    else
                    {
                        res.Message = "Given username/Nooch ID not found or give username/Nooch ID not active.";
                    }
                }
                else
                {
                    res.Message = "Admin account team@nooch.com not active or invalid admin PIN passed.";
                }
            }

            return Json(res);
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
                                // Deleting all records from database
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
        public ActionResult CreateAndSaveNewAdminUser(string userName, string emailAddress, string firstName, string lastName, string level)
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


        /// <summary>
        /// To save manager or admin details into database.
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="userName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="level"></param>
        /// <param name="loggedInUserId"></param>
        public CreateAdminResultClass CreateOrUpdateAdmin(string adminId, string userName, string emailAddress,
            string firstName, string lastName, string level, Guid loggedInUserId)
        {
            CreateAdminResultClass res = new CreateAdminResultClass();
            res.IsSuccess = false;

            Logger.Info("AdminController -> CreateOrUpdateAdmin: [" + userName + "]");

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                if (String.IsNullOrEmpty(adminId))
                {
                    #region to create new admin

                    if (GetAdminDetailsByUserName(userName.Trim()) == null)
                    {
                        Guid g = Guid.NewGuid();

                        var password = GenerateRandomPassword();
                        string encryptedPassword = CommonHelper.GetEncryptedData((password.Trim()));

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

                        admin.Password = encryptedPassword;

                        obj.AdminUsers.Add(admin);
                        var i = obj.SaveChanges();

                        if (i > -1)
                        {
                            var fromAddress = Utility.GetValueFromConfig("adminMail");

                            var tokens = new Dictionary<string, string>
                        {
                            {Constants.PLACEHOLDER_FIRST_NAME, firstName},
                            {Constants.PLACEHOLDER_LAST_NAME, lastName},
                            {"$UserName$", userName},
                            {Constants.PLACEHOLDER_PASSWORD, password}
                        };
                            try
                            {
                                Logger.Info("AdminController -> CreateOrUpdateAdmin - Attempt to send email [" + emailAddress + "].");

                                Utility.SendEmail("AdminPasswordMailTemplate", fromAddress, emailAddress, "Nooch password.",
                                    null, tokens, null, null, null);

                                res.IsSuccess = true;
                                res.Message = "Success";
                            }
                            catch (Exception)
                            {
                                Logger.Error("New Admin  CreateOrUpdateAdmin - Admin default password mail not sent to [" +
                                             userName + "].");

                                res.Message = "Problem occured in sending password mail. Please retry.";
                            }
                        }
                    }

                    res.Message = "User name already exists. Please try with some other name.";
                    return res;

                    #endregion
                }

                var id = Utility.ConvertToGuid(adminId);

                var adminUser = (from c in obj.AdminUsers where c.UserName == userName select c).SingleOrDefault();
                if (adminUser != null)
                {
                    res.IsSuccess = false;
                    res.Message = "User name already exists. Please try with some other name.";
                    return res;
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


        /// <summary>
        /// Add new data in SDN table.
        /// </summary>
        /// <param name="res"></param>
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
                        Logger.Error("Admin Cntrlr -> AddNewDataInSDN FAILED - Exception: [" + ex.Message + "]");
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


        /// <summary>
        /// Insert ADD Table Data
        /// </summary>
        /// <param name="res"></param>
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


        // Add ALT Table Data
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

        public ActionResult UsersOverTime()
        {
            return View();
        }

        public ActionResult UsersInBanks()
        {
            return View();
        }

        public ActionResult TransactionVolumeOverTime()
        {
            return View();
        }

        [HttpPost]
        [ActionName("GetTransactionVolumeOverTimeData")]
        public ActionResult GetTransactionVolumeOverTimeData(string recordType, string status, string fromDate = null, string toDate = null)
        {
            getUserOverTimeResult res = new getUserOverTimeResult();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                //List<getUserOverTimeResult> users_over_time = new List<getUserOverTimeResult>();
                List<internalDataArray> extList = new List<internalDataArray>();
                List<Member> memList = new List<Member>();
                List<DurationArray> duraionList = new List<DurationArray>();

                if (recordType == "dateRange")
                {
                    DateTime todayDate = Convert.ToDateTime(toDate);
                    DateTime past7days = Convert.ToDateTime(fromDate);
                    var dateOnlyString = past7days.ToShortDateString();
                    int i = 0;
                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();// past7days.DayOfWeek.ToString();
                        if (status == "0")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => c.TransactionStatus == "Success" && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == 0).Sum(x => (double?)(x.Amount)) ?? 0;

                            if (SumOfAmount > 0)
                            {
                                nre[1] = SumOfAmount.ToString();
                                string[] nda = new string[2];
                                nda[0] = i.ToString();
                                nda[1] = past7days.ToString("dd/MM/yyyy");
                                i++;
                                DurationArray da = new DurationArray();
                                da.durationData = nda;
                                duraionList.Add(da);
                            }
                        }
                        else if (status == "1")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Cancelled" || c.TransactionStatus == "Rejected") && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == 0).Sum(x => (double?)(x.Amount)) ?? 0;

                            if (SumOfAmount > 0)
                            {
                                nre[1] = SumOfAmount.ToString();
                                string[] nda = new string[2];
                                nda[0] = i.ToString();
                                nda[1] = past7days.ToShortDateString();
                                i++;
                                DurationArray da = new DurationArray();
                                da.durationData = nda;
                                duraionList.Add(da);
                            }
                        }
                        else if (status == "2")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Pending") && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == 0).Sum(x => (double?)(x.Amount)) ?? 0;

                            if (SumOfAmount > 0)
                            {
                                nre[1] = SumOfAmount.ToString();
                                string[] nda = new string[2];
                                nda[0] = i.ToString();
                                nda[1] = past7days.ToString("dd/MM/yyyy");
                                i++;
                                DurationArray da = new DurationArray();
                                da.durationData = nda;
                                duraionList.Add(da);
                            }
                        }

                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;

                        extList.Add(id);

                        past7days = past7days.AddDays(1);
                    }
                }

                if (recordType == "daily")
                {
                    DateTime todayDate = DateTime.Now;
                    DateTime past7days = DateTime.Now.AddDays(-7);
                    int i = 0;
                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();// past7days.DayOfWeek.ToString();
                        if (status == "0")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => c.TransactionStatus == "Success" && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == 0).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "1")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Cancelled" || c.TransactionStatus == "Rejected") && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == 0).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "2")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Pending") && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == 0).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }

                        string[] nda = new string[2];
                        nda[0] = i.ToString();
                        nda[1] = past7days.DayOfWeek.ToString().Substring(0, 3) + ". " + past7days.Date.ToString("MMMM d");

                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;
                        DurationArray da = new DurationArray();
                        da.durationData = nda;

                        extList.Add(id);
                        duraionList.Add(da);
                        past7days = past7days.AddDays(1);
                        i++;
                    }
                }

                if (recordType == "weekly")
                {
                    DateTime todayDate = DateTime.Now;
                    int DayNumbr = Convert.ToInt32(todayDate.DayOfWeek) - 1;
                    DateTime past7days = DateTime.Now.AddDays(-70 - DayNumbr);
                    int j = 0;
                    while (past7days <= todayDate)
                    {
                        Double SumOfAmount = 0;
                        string[] nre = new string[2];
                        string[] nda = new string[2];
                        nre[0] = j.ToString();
                        nda[0] = j.ToString();

                        if (status == "0")
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                SumOfAmount += obj.Transactions.Where(c => c.TransactionStatus == "Success" && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == (7 - i)).Sum(x => (double?)(x.Amount)) ?? 0;
                            }
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "1")
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                SumOfAmount += obj.Transactions.Where(c => (c.TransactionStatus == "Cancelled" || c.TransactionStatus == "Rejected") && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == (7 - i)).Sum(x => (double?)(x.Amount)) ?? 0;
                            }
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "2")
                        {

                            for (int i = 0; i < 7; i++)
                            {
                                SumOfAmount += obj.Transactions.Where(c => (c.TransactionStatus == "Pending") && SqlFunctions.DateDiff("DAY", c.TransactionDate, past7days) == (7 - i)).Sum(x => (double?)(x.Amount)) ?? 0;
                            }
                            nre[1] = SumOfAmount.ToString();
                        }

                        nda[1] = past7days.ToString("M/d/yy") + " - " + past7days.AddDays(6).ToString("M/d/yy");
                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;
                        DurationArray da = new DurationArray();
                        da.durationData = nda;

                        extList.Add(id);
                        duraionList.Add(da);
                        j++;
                        past7days = past7days.AddDays(7);
                    }
                }

                else if (recordType == "monthly")
                {
                    DateTime todayDate = DateTime.Now;
                    DateTime past7days = DateTime.Now.AddMonths(-10);
                    int i = 0;
                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();// past7days.DayOfWeek.ToString();
                        if (status == "0")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Success") && c.TransactionDate.Value.Month == past7days.Month).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "1")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Cancelled" || c.TransactionStatus == "Rejected") && c.TransactionDate.Value.Month == past7days.Month).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "2")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Pending") && c.TransactionDate.Value.Month == past7days.Month).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }
                        string[] nda = new string[2];
                        nda[0] = i.ToString();

                        nda[1] = past7days.ToString("MMM 'yy");
                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;
                        DurationArray da = new DurationArray();
                        da.durationData = nda;
                        extList.Add(id);
                        duraionList.Add(da);
                        past7days = past7days.AddMonths(1);
                        i++;
                    }
                }

                else if (recordType == "yearly")
                {
                    DateTime todayDate = DateTime.Now;
                    DateTime past7days = DateTime.Now.AddYears(-10);
                    int i = 0;
                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();// past7days.DayOfWeek.ToString();
                        if (status == "0")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Success") && c.TransactionDate.Value.Year == past7days.Year).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "1")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Cancelled" || c.TransactionStatus == "Rejected") && c.TransactionDate.Value.Year == past7days.Year).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }
                        else if (status == "2")
                        {
                            var SumOfAmount = obj.Transactions.Where(c => (c.TransactionStatus == "Pending") && c.TransactionDate.Value.Year == past7days.Year).Sum(x => (double?)(x.Amount)) ?? 0;
                            nre[1] = SumOfAmount.ToString();
                        }

                        string[] nda = new string[2];
                        nda[0] = i.ToString();
                        nda[1] = past7days.Year.ToString();
                        internalDataArray id = new internalDataArray();

                        id.internalData = nre;
                        DurationArray da = new DurationArray();
                        da.durationData = nda;
                        extList.Add(id);
                        duraionList.Add(da);
                        past7days = past7days.AddYears(1);
                        i++;
                    }
                }

                res.externalData = extList;
                res.Duration = duraionList;
            }

            res.IsSuccess = true;
            res.ErrorMessage = "OK";

            return Json(res);
        }


        [HttpPost]
        [ActionName("getUsersInBanks")]
        public ActionResult getUsersInBanks()
        {
            getUserOverTimeResult res = new getUserOverTimeResult();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                List<internalDataArray> extList = new List<internalDataArray>();
                List<DurationArray> duraionList = new List<DurationArray>();

                var allSyanpseSupportedBanks = (from ce in obj.SynapseSupportedBanks
                                                where ce.IsDeleted == false
                                                select ce).ToList();

                var membersInEachBank = obj.GetMembersInEachSynapseBank().ToList();

                List<GetMembersInEachSynapseBank_Result> decryptedList = new List<GetMembersInEachSynapseBank_Result>();

                foreach (GetMembersInEachSynapseBank_Result mem in membersInEachBank)
                {
                    GetMembersInEachSynapseBank_Result m = new GetMembersInEachSynapseBank_Result();
                    m.bank_name = CommonHelper.GetDecryptedData(mem.bank_name).ToLower().Trim();
                    m.CountInBank = mem.CountInBank;
                    decryptedList.Add(m);
                }

                List<NoOfUsersInEachBank> UserCountInEachBankPrep = new List<NoOfUsersInEachBank>();

                int i = 0;

                foreach (SynapseSupportedBank ssb in allSyanpseSupportedBanks)
                {
                    NoOfUsersInEachBank nusi = new NoOfUsersInEachBank();
                    internalDataArray id = new internalDataArray();
                    DurationArray da = new DurationArray();

                    nusi.BankName = ssb.BankName;
                    nusi.NoOfUsers = 0;

                    string[] nBnks = new string[2];
                    string[] nda = new string[2];

                    foreach (GetMembersInEachSynapseBank_Result bank in decryptedList)
                    {
                        if (bank.bank_name == ssb.BankName.ToLower().Trim())
                        {
                            nusi.NoOfUsers = Convert.ToInt16(bank.CountInBank);
                            nda[0] = i++.ToString();
                            nda[1] = (bank.CountInBank).ToString();
                            nBnks[0] = nda[0];
                            nBnks[1] = ssb.BankName;
                            da.durationData = nBnks;
                            duraionList.Add(da);
                        }
                    }

                    id.internalData = nda;
                    extList.Add(id);

                    UserCountInEachBankPrep.Add(nusi);
                }

                res.IsSuccess = true;
                res.externalData = extList;
                res.Duration = duraionList;

                return Json(res);
            }
        }


        [HttpPost]
        [ActionName("GetUsersOverTimeOverTimeData")]
        public ActionResult GetUsersOverTimeOverTimeData(string dateType, string userType, string userStatus, string fromDate = null, string toDate = null)
        {
            getUserOverTimeResult res = new getUserOverTimeResult();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                List<internalDataArray> extList = new List<internalDataArray>();
                List<Member> memList = new List<Member>();
                List<DurationArray> duraionList = new List<DurationArray>();

                // 'userType'...
                // 0 = all
                // 1 = registerd
                // 2 = non-registered

                // 'UserStatus'...
                // 0 = all
                // 1 = active
                // 2 = deleted
                var includeDeletedMembers = userStatus.Equals('0') || userStatus.Equals('2') ? true : false;

                #region Custom Date Range

                if (dateType == "dateRange")
                {
                    DateTime todayDate = Convert.ToDateTime(toDate);
                    DateTime past7days = Convert.ToDateTime(fromDate);
                    var dateOnlyString = past7days.ToShortDateString();

                    int i = 0;

                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();

                        if (userStatus == "0")
                        {
                            int usersCount = (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 && (t.Status == "Active" || t.Status == "Registered")
                                              select t).Count();

                            if (usersCount > 0)
                            {
                                nre[1] = usersCount.ToString();
                                string[] nda = new string[2];
                                nda[0] = i.ToString();
                                nda[1] = past7days.ToShortDateString();

                                i++;

                                DurationArray da = new DurationArray();
                                da.durationData = nda;
                                duraionList.Add(da);
                            }

                        }
                        else if (userStatus == "1")
                        {
                            int usersCount = (from t in obj.Members where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 && (t.Status != "Active" && t.Status != "Registered" && t.Status != "Suspended" && t.Status != "Deleted") select t).Count();
                            if (usersCount > 0)
                            {
                                nre[1] = usersCount.ToString();
                                string[] nda = new string[2];
                                nda[0] = i.ToString();
                                nda[1] = dateOnlyString;
                                i++;
                                DurationArray da = new DurationArray();
                                da.durationData = nda;
                                duraionList.Add(da);
                            }
                        }

                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;

                        extList.Add(id);

                        past7days = past7days.AddDays(1);
                    }
                }

                #endregion Custom Date Range

                #region Weekly

                if (dateType == "weekly")
                {
                    DateTime todayDate = DateTime.Now;
                    int DayNumbr = Convert.ToInt32(todayDate.DayOfWeek) - 1;
                    DateTime past7days = DateTime.Now.AddDays(-70 - DayNumbr);

                    int j = 0;

                    while (past7days <= todayDate)
                    {
                        int count = 0;
                        string[] nre = new string[2];
                        string[] nda = new string[2];
                        nre[0] = j.ToString();
                        nda[0] = j.ToString();

                        #region User Type is ALL

                        if (userType == "0") // 'All' for USER TYPE (Registered/Non-registered)
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                    !String.IsNullOrEmpty(t.Status)
                                              select t).Count();
                                }
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                    !String.IsNullOrEmpty(t.Status) &&
                                                    t.IsDeleted == false
                                              select t).Count();
                                }
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                    !String.IsNullOrEmpty(t.Status) &&
                                                    t.IsDeleted == true
                                              select t).Count();
                                }
                            }
                        }

                        #endregion User Type is ALL

                        #region User Type is REGISTERED

                        else if (userType == "1") // 'Registered' (Also will include 'Active' or 'Accepted'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                   (t.Status == "Registered" || t.Status == "Active" || t.Status == "Accepted")
                                              select t).Count();
                                }
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                   (t.Status == "Registered" || t.Status == "Active" || t.Status == "Accepted") &&
                                                    t.IsDeleted == false
                                              select t).Count();
                                }
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                   (t.Status == "Registered" || t.Status == "Active" || t.Status == "Accepted") &&
                                                    t.IsDeleted == true
                                              select t).Count();
                                }
                            }
                        }

                        #endregion User Type is REGISTERED

                        #region User Type is NON-REGISTERED

                        else if (userType == "2") // 'Non-Registered'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                    t.Status != "NonRegistered"
                                              select t).Count();
                                }
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                    t.Status != "NonRegistered" &&
                                                    t.IsDeleted == false
                                              select t).Count();
                                }
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    count += (from t in obj.Members
                                              where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == (7 - i) &&
                                                    t.Status != "NonRegistered" &&
                                                    t.IsDeleted == true
                                              select t).Count();
                                }
                            }
                        }

                        #endregion User Type is NON-REGISTERED


                        nre[1] = count.ToString();
                        nda[1] = past7days.ToString("M/d/yy") + " - " + past7days.AddDays(6).ToString("M/d/yy");

                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;

                        DurationArray da = new DurationArray();
                        da.durationData = nda;

                        extList.Add(id);
                        duraionList.Add(da);
                        j++;
                        past7days = past7days.AddDays(7);
                    }
                }

                #endregion Weekly

                #region Daily

                if (dateType == "daily")
                {
                    DateTime todayDate = DateTime.Now;
                    DateTime past7days = DateTime.Now.AddDays(-7);
                    int i = 0;

                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();

                        #region User Type is ALL

                        if (userType == "0") // 'All' for USER TYPE (Registered/Non-registered)
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                !String.IsNullOrEmpty(t.Status)
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                !String.IsNullOrEmpty(t.Status) &&
                                                t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                !String.IsNullOrEmpty(t.Status) &&
                                                t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is ALL

                        #region User Type is REGISTERED

                        else if (userType == "1") // 'Registered' (Also will include 'Active' or 'Accepted'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted")
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted") &&
                                                 t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted") &&
                                                 t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is REGISTERED

                        #region User Type is NON-REGISTERED

                        else if (userType == "2") // 'Non-Registered'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                t.Status != "NonRegistered"
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                t.Status != "NonRegistered" &&
                                                t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where SqlFunctions.DateDiff("DAY", t.DateCreated, past7days) == 0 &&
                                                t.Status != "NonRegistered" &&
                                                t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is NON-REGISTERED

                        string[] nda = new string[2];
                        nda[0] = i.ToString();
                        nda[1] = past7days.DayOfWeek.ToString().Substring(0, 3) + ". " + past7days.Date.ToString("MMMM d");


                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;

                        DurationArray da = new DurationArray();
                        da.durationData = nda;

                        extList.Add(id);
                        duraionList.Add(da);
                        past7days = past7days.AddDays(1);
                        i++;
                    }
                }

                #endregion Daily

                #region Monthly

                else if (dateType == "monthly")
                {
                    DateTime todayDate = DateTime.Now;
                    DateTime past8months = DateTime.Now.AddMonths(-8);

                    int i = 0;

                    while (past8months <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();

                        #region User Type is ALL

                        if (userType == "0") // 'All' for USER TYPE (Registered/Non-registered)
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          !String.IsNullOrEmpty(t.Status)
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          !String.IsNullOrEmpty(t.Status) &&
                                          t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          !String.IsNullOrEmpty(t.Status) &&
                                          t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is ALL

                        #region User Type is REGISTERED

                        else if (userType == "1") // 'Registered' (Also will include 'Active' or 'Accepted'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted")
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted") &&
                                           t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted") &&
                                           t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is REGISTERED

                        #region User Type is NON-REGISTERED

                        else if (userType == "2") // 'Non-Registered'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          t.Status != "NonRegistered"
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          t.Status != "NonRegistered" &&
                                          t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Month == past8months.Month &&
                                          t.Status != "NonRegistered" &&
                                          t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is NON-REGISTERED


                        string[] nda = new string[2];
                        nda[0] = i.ToString();
                        nda[1] = past8months.ToString("MMM. 'yy");

                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;

                        DurationArray da = new DurationArray();
                        da.durationData = nda;
                        extList.Add(id);
                        duraionList.Add(da);
                        past8months = past8months.AddMonths(1);

                        i++;
                    }
                }

                #endregion Monthly

                #region Yearly

                else if (dateType == "yearly")
                {
                    DateTime todayDate = DateTime.Now;
                    DateTime past4years = DateTime.Now.AddYears(-4);

                    int i = 0;

                    while (past4years <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();

                        #region User Type is ALL

                        if (userType == "0") // 'All' for USER TYPE (Registered/Non-registered)
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          !String.IsNullOrEmpty(t.Status)
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          !String.IsNullOrEmpty(t.Status) &&
                                          t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          !String.IsNullOrEmpty(t.Status) &&
                                          t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is ALL

                        #region User Type is REGISTERED

                        else if (userType == "1") // 'Registered' (Also will include 'Active' or 'Accepted'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted")
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted") &&
                                          t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          (t.Status == "Active" || t.Status == "Registered" || t.Status == "Accepted") &&
                                          t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is REGISTERED

                        #region User Type is NON-REGISTERED

                        else if (userType == "2") // 'Non-Registered'
                        {
                            if (userStatus == "0") // 'All' for USER STATUS (deleted & non deleted)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          t.Status != "NonRegistered"
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "1") // Not Deleted for USER STATUS (Only IsDeleted = false)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          t.Status != "NonRegistered" &&
                                          t.IsDeleted == false
                                          select t).Count().ToString();
                            }
                            else if (userStatus == "2") // 'Deleted' for USER STATUS (Only IsDeleted = true)
                            {
                                nre[1] = (from t in obj.Members
                                          where t.DateCreated.Value.Year == past4years.Year &&
                                          t.Status != "NonRegistered" &&
                                          t.IsDeleted == true
                                          select t).Count().ToString();
                            }
                        }

                        #endregion User Type is NON-REGISTERED


                        string[] nda = new string[2];
                        nda[0] = i.ToString();
                        nda[1] = past4years.Year.ToString();

                        internalDataArray id = new internalDataArray();
                        id.internalData = nre;

                        DurationArray da = new DurationArray();
                        da.durationData = nda;

                        extList.Add(id);
                        duraionList.Add(da);

                        past4years = past4years.AddYears(1);

                        i++;
                    }
                }

                #endregion Yearly

                res.externalData = extList;
                res.Duration = duraionList;
            }

            res.IsSuccess = true;
            res.ErrorMessage = "OK";

            return Json(res);
        }


        public class getUserOverTimeResult
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public List<DurationArray> Duration { get; set; }

            public List<internalDataArray> externalData { get; set; }
        }

        public class internalDataArray
        {
            public string[] internalData { get; set; }
        }

        public class DurationArray
        {
            public string[] durationData { get; set; }
        }

        public ActionResult Transaction()
        {
            CheckSession();

            TransactionsPageData res = new TransactionsPageData();

            List<TransactionClass> adminUser = new List<TransactionClass>();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                adminUser = (from t in obj.Transactions
                             join g in obj.GeoLocations
                             on t.LocationId equals g.LocationId

                             select new TransactionClass
                             {
                                 TransactionId = t.TransactionId,
                                 TransactionType = t.TransactionType,
                                 TransactionStatus = t.TransactionStatus,
                                 Amount = t.Amount,
                                 TransactionDate = t.TransactionDate,
                                 SenderId = t.SenderId,
                                 RecipientId = t.RecipientId,
                                 TransLongi = g.Longitude,
                                 TransLati = g.Latitude,
                                 state = g.State,
                                 city = g.City,
                                 Memo = t.Memo,
                                 isPhoneInvitation = t.IsPhoneInvitation,
                                 PhoneNumberInvited = t.PhoneNumberInvited,
                                 InvitationSentTo = t.InvitationSentTo
                             }).OrderByDescending(t => t.TransactionDate).Take(100).ToList();


                Member sender = null;
                Member receiver = null;

                foreach (var transaction in adminUser.ToList())
                {
                    // +C1+zhVafHdXQXCIqjU/Zg== -- Disputed
                    // 5/KChkAfEoa6N2oo8FHwWQ== -- Reward
                    // 5dt4HUwCue532sNmw3LKDQ== - Transfer

                    // DrRr1tU1usk7nNibjtcZkA== - Invite
                    // EnOIzpmFFTEaAP16hm9Wsw== -- Rent
                    // T3EMY1WWZ9IscHIj3dbcNw== - Request

                    transaction.TransactionType = CommonHelper.GetDecryptedData(transaction.TransactionType);

                    #region Request Type Transaction

                    if (transaction.TransactionType == "Request")
                    {
                        if (transaction.RecipientId != transaction.SenderId)
                        {
                            sender = CommonHelper.GetMemberUsingGivenMemberId(transaction.RecipientId.ToString());
                            transaction.SenderNoochId = sender.Nooch_ID.ToString();
                            transaction.SenderName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(sender.FirstName)) + " " + CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(sender.LastName));
                            transaction.SenderId = sender.MemberId;

                            receiver = CommonHelper.GetMemberUsingGivenMemberId(transaction.SenderId.ToString());
                            transaction.RecepientNoochId = receiver.Nooch_ID.ToString();
                            transaction.RecipientName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(receiver.FirstName)) + " " + CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(receiver.LastName));
                            transaction.RecipientId = receiver.MemberId;
                        }
                        else if (transaction.RecipientId == transaction.SenderId && transaction.isPhoneInvitation == true)
                        {
                            // Request to non-Nooch user via PHONE

                            transaction.SenderName = !String.IsNullOrEmpty(transaction.PhoneNumberInvited) ? CommonHelper.FormatPhoneNumber(CommonHelper.GetDecryptedData(transaction.PhoneNumberInvited)) : "";
                            transaction.RecipientName = CommonHelper.GetMemberNameFromMemberId(transaction.SenderId.ToString());
                            transaction.RecipientId = transaction.SenderId;
                            transaction.SenderId = null;

                            receiver = obj.Members.FirstOrDefault(mmObj => mmObj.MemberId == transaction.SenderId);
                            if (receiver != null)
                                transaction.RecepientNoochId = receiver.Nooch_ID.ToString();
                        }
                        else if (transaction.RecipientId == transaction.SenderId && transaction.InvitationSentTo != null)
                        {
                            // Request to non-Nooch user via EMAIL

                            sender = obj.Members.FirstOrDefault(mmObj => mmObj.UserName == transaction.InvitationSentTo);
                            if (sender != null)
                            {
                                transaction.SenderName = CommonHelper.GetMemberNameFromMemberId(sender.MemberId.ToString());
                                transaction.SenderNoochId = sender.Nooch_ID.ToString();
                            }
                            else
                                transaction.SenderName = !String.IsNullOrEmpty(transaction.InvitationSentTo) ? CommonHelper.GetDecryptedData(transaction.InvitationSentTo) : "";

                            transaction.RecipientName = CommonHelper.GetMemberNameFromMemberId(transaction.SenderId.ToString());
                            transaction.RecipientId = transaction.SenderId;
                            transaction.SenderId = null;

                            receiver = obj.Members.FirstOrDefault(mmObj => mmObj.MemberId == transaction.SenderId);
                            if (receiver != null)
                                transaction.RecepientNoochId = receiver.Nooch_ID.ToString();
                        }
                    }

                    #endregion Request Type Transaction


                    #region Invite Type

                    // Invite via Phone
                    // Cliff (5/21/16): I don't think this is right since if it's an invite to a Non-Nooch user, then this user MUST
                    //                  be the "Sender".  Don't feel like going through this in detail to figure it out, but need
                    //                  to come back and check & test this entire method and make sure each section is correct.
                    else if (transaction.TransactionType == "Invite")// && transaction.RecipientId == transaction.SenderId)
                    {
                        if (transaction.isPhoneInvitation == true)
                        {
                            // Invite via PHONE

                            if (!String.IsNullOrEmpty(transaction.PhoneNumberInvited))
                            {
                                transaction.RecipientName = CommonHelper.FormatPhoneNumber(CommonHelper.GetDecryptedData(transaction.PhoneNumberInvited));
                            }
                            else
                            {
                                transaction.RecipientName = "";
                            }

                            transaction.SenderName = CommonHelper.GetMemberNameFromMemberId(transaction.SenderId.ToString());
                            transaction.RecipientId = null;
                            transaction.SenderId = transaction.SenderId;

                            sender = obj.Members.FirstOrDefault(mmObj => mmObj.MemberId == transaction.SenderId);
                            if (sender != null)
                                transaction.SenderNoochId = sender.Nooch_ID.ToString();
                        }
                        else if (transaction.InvitationSentTo != null)
                        {
                            // Invite via EMAIL

                            if (!String.IsNullOrEmpty(transaction.InvitationSentTo))
                            {
                                sender = obj.Members.FirstOrDefault(mmObj => mmObj.UserName == transaction.InvitationSentTo);
                                if (sender != null)
                                {
                                    transaction.RecipientName = CommonHelper.GetMemberNameFromMemberId(sender.MemberId.ToString());
                                    transaction.RecepientNoochId = sender.Nooch_ID.ToString();
                                }
                                else
                                    transaction.RecipientName = CommonHelper.GetDecryptedData(transaction.InvitationSentTo);
                            }
                            else
                            {
                                transaction.RecipientName = "";
                            }

                            transaction.SenderName = CommonHelper.GetMemberNameFromMemberId(transaction.SenderId.ToString());
                            transaction.RecipientId = null;
                            transaction.SenderId = transaction.SenderId;

                            sender = obj.Members.FirstOrDefault(mmObj => mmObj.MemberId == transaction.SenderId);
                            if (sender != null)
                                transaction.SenderNoochId = sender.Nooch_ID.ToString();
                        }
                    }

                    #endregion Invite Type


                    #region Transfer, Dispute or Reward type transaction

                    // transfer type trans to non nooch user...by phone request could be disputed,rent, reward type
                    else
                    {
                        transaction.RecipientName = CommonHelper.GetMemberNameFromMemberId(transaction.RecipientId.ToString());
                        transaction.SenderName = CommonHelper.GetMemberNameFromMemberId(transaction.SenderId.ToString());
                        transaction.RecipientId = transaction.RecipientId;
                        transaction.SenderId = transaction.SenderId;

                        sender = obj.Members.FirstOrDefault(mmObj => mmObj.MemberId == transaction.SenderId);
                        if (sender != null)
                            transaction.SenderNoochId = sender.Nooch_ID.ToString();

                        receiver = obj.Members.FirstOrDefault(mmObj => mmObj.MemberId == transaction.RecipientId);
                        if (receiver != null)
                            transaction.RecepientNoochId = receiver.Nooch_ID.ToString();
                    }

                    #endregion Transfer, Dispute or Reward type transaction

                    transaction.TransactionDate1 = Convert.ToDateTime(transaction.TransactionDate).ToString("MMM d, yyyy");
                    transaction.TransactionTime = Convert.ToDateTime(transaction.TransactionDate).ToString("h:mm tt");
                    transaction.Amount = Math.Round(transaction.Amount, 2);
                }
            }

            return View(adminUser);
        }


        public ActionResult CancelTransaction(string transactionId)
        {
            MemberOperationsResult res = new MemberOperationsResult();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                Guid g = new Guid(transactionId);

                var member = (from t in obj.Transactions
                              where t.TransactionId == g
                              select t).SingleOrDefault();

                if (member != null)
                {
                    member.TransactionStatus = "Cancelled";
                    int v = obj.SaveChanges();

                    res.Message = "Transaction Cancelled";
                    res.IsSuccess = true;
                }
            }

            return Json(res);
        }
    }
}