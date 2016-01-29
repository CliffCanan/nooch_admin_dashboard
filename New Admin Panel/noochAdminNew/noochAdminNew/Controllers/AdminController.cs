﻿using noochAdminNew.Classes.ModelClasses;
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

                            // request type trans to non nooch user...by email
                            else if (singleTrans.TransactionType == "Invite" && t.RecipientId == t.SenderId && t.InvitationSentTo != null)
                            {
                                if (!String.IsNullOrEmpty(t.InvitationSentTo))
                                {
                                    singleTrans.RecepientUserName =

                                            CommonHelper.GetDecryptedData(t.InvitationSentTo);
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
                Logger.Error("AdminController -> ShowLiveTransactionsOnDashBoard - [Outer Exception: " + ex + "]");

                ddresult.IsSuccess = false;
                ddresult.Message = "Exception reached - Invalid Operation";
                return Json(ddresult);
            }
            #endregion
        }

        public ActionResult Dashboard()
        {
            //var CurrentYear = DateTime.Now.Year;
            //var CurrentMonth = DateTime.Now.Month;
            //var CurrentDate = DateTime.Now.Day;
            //var TodayDate = DateTime.Today.ToShortDateString();
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

                    //c = (from t in obj.Members
                    //     join kad in obj.KnoxAccountDetails
                    //         on t.MemberId equals kad.MemberId
                    //     where t.IsDeleted == false &&
                    //           t.Status == "Active" &&
                    //           t.IsVerifiedPhone == true &&
                    //         kad.IsDeleted == false
                    //     select t).ToList();

                    c = (from t in obj.Members
                         
                         where t.IsVerifiedWithSynapse==true &&
                               t.Status == "Active" &&
                               t.IsVerifiedPhone == true 
                             
                         select t).ToList();

                    dd.TotalActiveAndVerifiedBankAccountUsers = c.Count;

                    //c = (from t in obj.Members
                    //     join kad in obj.KnoxAccountDetails
                    //         on t.MemberId equals kad.MemberId
                    //     where t.IsDeleted == false && kad.IsDeleted == false
                    //     select t).ToList();
                    dd.TotalActiveBankAccountUsers = c.Count;

                    dd.TotalAmountOfDollars = (from r in obj.Transactions
                                               where r.TransactionStatus == "success"
                                               select r).ToList()
                                                    .Sum(t => t.Amount).ToString();

                    dd.TotalNoOfPaymentsCompleted = (from r in obj.Transactions
                                                     where r.TransactionStatus == "success"
                                                     select r).ToList().Count;

                    dd.totalRequestTypeTrans = (from r in obj.Transactions
                                                where r.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw==" && r.TransactionStatus == "success"
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



                    // **No Of User Of Each Bank   
                    //var ss = (from SSB in obj.SynapseSupportedBanks
                    //          select new NoOfUsersInEachBank
                    //          {
                    //              BankName = SSB.BankName,
                    //              NoOfUsers = (from SBM in obj.SynapseBanksOfMembers
                    //                           where SBM.IsDefault == true &&
                    //                                 SBM.bank_name == SSB.BankName
                    //                           group SBM by SBM.bank_name into s
                    //                           select s.Count()).FirstOrDefault()
                    //          }).OrderBy(a => a.BankName).ToList();
                    dd.UserCountInEachBank = UserCountInEachBankPrep;
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

// Commented out this method coz.. knoxAccountDetails table is no longer in db  -- Malkit 21 Jan 16

//        [HttpPost]
//        [ActionName("CreditFundToMemberPost")]
//        public ActionResult CreditFundToMemberPost(string transferfundto, string transferAmount, string transferNotes, string adminPin)
//        {
//            LoginResult lr = new LoginResult();
//            // performing validations over input

//            #region input validations

//            if (String.IsNullOrEmpty(transferfundto))
//            {
//                lr.IsSuccess = false;
//                lr.Message = "Please enter user name or NoochId of Member to transfer fund.";
//            }
//            if (String.IsNullOrEmpty(transferAmount))
//            {
//                lr.IsSuccess = false;
//                lr.Message = "Please enter transfer fund amount";
//            }

//            if (String.IsNullOrEmpty(transferNotes))
//            {
//                lr.IsSuccess = false;
//                lr.Message = "Please enter transfer notes.";
//            }
//            if (String.IsNullOrEmpty(adminPin))
//            {
//                lr.IsSuccess = false;
//                lr.Message = "Please enter admin pin.";
//            }

//            #endregion

//            // CLIFF (9/7/15): THIS MUST BE UPDATED TO USE SYNAPSE V3 INSTEAD OF KNOX

//            // **********************  THIS REMAINS INCOMPLETE!  **********************

//            // 1. check admin user knox account and other details
//            // 2. check fund receiver knox account details

//            // Check admin user details
//            using (NOOCHEntities obj = new NOOCHEntities())
//            {
//                var adminUserDetails =
//                    (from c in obj.Members
//                     where c.UserName == "z2/de4EMabGlzMuO7OocHw==" &&
//                           c.Status == "Active" &&
//                           c.PinNumber == CommonHelper.GetEncryptedData(adminPin.Trim())
//                     select c).SingleOrDefault();

//                if (adminUserDetails != null)
//                {
//                    Guid AdminMemberId = Utility.ConvertToGuid(adminUserDetails.MemberId.ToString());

//                    // Get Synapse account details of admin
//                    var adminSynapseDetails =
//                        (from c in obj.SynapseBanksOfMembers
//                         where c.MemberId == AdminMemberId && c.IsDefault == true
//                         select c).SingleOrDefault();

//                    if (adminSynapseDetails != null)
//                    {
//                        // Now get the Recipient's info from Members table
//                        string recepientusernameencrypted = CommonHelper.GetEncryptedData(transferfundto.ToLower());

//                        var recipientMemberDetails = (from c in obj.Members
//                                                      where c.Nooch_ID == transferfundto ||
//                                                            c.UserName == recepientusernameencrypted &&
//                                                            c.Status == "Active"
//                                                      select c).SingleOrDefault();

//                        if (recipientMemberDetails != null)
//                        {
//                            // Now check recipient's Synapse details
//                            Guid recepeintGuid = Utility.ConvertToGuid(recipientMemberDetails.MemberId.ToString());

//                            var recipientBankDetails =
//                                (from c in obj.KnoxAccountDetails
//                                 where c.MemberId == recepeintGuid && c.IsDeleted == false
//                                 select c).SingleOrDefault();

//                            if (recipientBankDetails != null)
//                            {
//                                string transactionTrackingId = GetRandomTransactionTrackingId();

//                                Transaction trans = new Transaction();
//                                trans.TransactionId = Guid.NewGuid();
//                                trans.SenderId = AdminMemberId;
//                                trans.RecipientId = recepeintGuid;
//                                trans.Amount = Convert.ToDecimal(transferAmount);

//                                trans.TransactionDate = DateTime.Now;
//                                trans.DisputeStatus = null;
//                                trans.TransactionStatus = "Success";
//                                trans.TransactionType = CommonHelper.GetEncryptedData("Reward");
//                                trans.DeviceId = null;
//                                trans.TransactionTrackingId = transactionTrackingId;
//                                trans.Memo = transferNotes.Trim();
//                                trans.Picture = null;

//                                GeoLocation geo = new GeoLocation();
//                                geo.LocationId = Guid.NewGuid();
//                                geo.Latitude = null;
//                                geo.Longitude = null;
//                                geo.Altitude = null;
//                                geo.AddressLine1 = null;
//                                geo.AddressLine2 = null;
//                                geo.City = null;
//                                geo.State = null;
//                                geo.Country = null;
//                                geo.ZipCode = null;
//                                geo.DateCreated = DateTime.Now;


//                                // making api call to knox
//                                WebClient wc = new WebClient();

//                                string KNoxApiKey = Utility.GetValueFromConfig("KnoxApiKey");
//                                string KNoxApiPass = Utility.GetValueFromConfig("KnoxApiPass");

//                                string c = "https://knoxpayments.com/json/pinpayment.php?payee_key=" +
//                                    //RECEPEINT_USER_KEY +
//                                    //"&payee_pass=" + RECEPEINT_USER_PASS + "&payor_key=" + ADMIN_USER_KEY +
//                                           "&payor_pass=" +
//                                    //ADMIN_USER_PASS + "&trans_id=" + trans.TransactionId + "&PARTNER_KEY=" +
//                                           KNoxApiKey + "&amount=" + trans.Amount + "&recur_status=ot";
//                                string knoxPinPaymentResults = wc.DownloadString(c);

//                                ResponseClass3 m = JsonConvert.DeserializeObject<ResponseClass3>(knoxPinPaymentResults);
//                                if (m != null)
//                                {
//                                    #region parsed response successfully
//                                    /*
//                                    string KnoxTransStatus = m.JSonDataResult.status_code;
//                                    string KnoxTransErrorCode = m.JSonDataResult.error_code;
//                                    string KnoxTransId = m.JSonDataResult.trans_id;

//                                    if (KnoxTransStatus != null)
//                                    {
//                                        Logger.Info(
//                                            "TransferFundToMemberFromNEWADMIN_PANLE -> knoxPinPaymentResult Status Code for Nooch TransID [" +
//                                            trans.TransactionId + "] is: " + KnoxTransStatus);
//                                    }
//                                    if (KnoxTransErrorCode != null)
//                                    {
//                                        Logger.Info(
//                                            "TransferFundToMemberFromNEWADMIN_PANLE -> knoxPinPaymentResult ERROR Code for Nooch TransID [" +
//                                            trans.TransactionId + "] is: " + KnoxTransErrorCode);
//                                    }

//                                    if (KnoxTransStatus == "PAID" && KnoxTransErrorCode == null)
//                                    {
//                                        #region Knox returned Paid

//                                        #region email content preparation

//                                        string senderFirstName =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(adminUserDetails.FirstName));
//                                        string senderLastName =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(adminUserDetails.LastName));
//                                        string recipientFirstName =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(recepientdetails.FirstName));
//                                        string recipientLastName =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(recepientdetails.LastName));

//                                        string wholeAmount = trans.Amount.ToString("n2");
//                                        string[] s3 = wholeAmount.Split('.');
//                                        string ce = "";
//                                        string dl = "";
//                                        if (s3.Length <= 1)
//                                        {
//                                            dl = s3[0].ToString();
//                                            ce = "00";
//                                        }
//                                        else
//                                        {
//                                            ce = s3[1].ToString();
//                                            dl = s3[0].ToString();
//                                        }

//                                        string memo = "";
//                                        if (trans.Memo != null && trans.Memo != "")
//                                        {
//                                            if (trans.Memo.Length > 3)
//                                            {
//                                                string firstThreeChars = trans.Memo.Substring(0, 3).ToLower();
//                                                bool startWithFor = firstThreeChars.Equals("for");

//                                                if (startWithFor)
//                                                {
//                                                    memo = trans.Memo.ToString();
//                                                }
//                                                else
//                                                {
//                                                    memo = "For " + trans.Memo.ToString();
//                                                }
//                                            }
//                                            else
//                                            {
//                                                memo = "For " + trans.Memo.ToString();
//                                            }
//                                        }

//                                        #endregion

//                                        string senderPic;
//                                        string recipientPic;
//                                        var friendDetails =
//                                            CommonHelper.GetMemberNotificationSettings(
//                                                adminUserDetails.MemberId.ToString());

//                                        #region email to admin on successfully sending fund

//                                        if (friendDetails != null)
//                                        {
//                                            // for TransferSent email notification
//                                            if (friendDetails != null && (friendDetails.EmailTransferSent ?? false))
//                                            {
//                                                if (recepientdetails.Photo != null && recepientdetails.Photo != "")
//                                                {
//                                                    string lastFourOfRecipientsPic =
//                                                        recepientdetails.Photo.Substring(recepientdetails.Photo.Length -
//                                                                                         15);
//                                                    if (lastFourOfRecipientsPic != "gv_no_photo.png")
//                                                    {
//                                                        recipientPic = "";
//                                                    }
//                                                    else
//                                                    {
//                                                        recipientPic = recepientdetails.Photo.ToString();
//                                                    }
//                                                }

//                                                var tokens = new Dictionary<string, string>
//                                                {
//                                                    {Constants.PLACEHOLDER_FIRST_NAME, senderFirstName},
//                                                    {
//                                                        Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
//                                                        recipientFirstName + " " + recipientLastName
//                                                    },
//                                                    {Constants.PLACEHOLDER_TRANSFER_AMOUNT, dl},
//                                                    {Constants.PLACEHLODER_CENTS, ce},
//                                                    {Constants.MEMO, memo}
//                                                };

//                                                var fromAddress = Utility.GetValueFromConfig("transfersMail");
//                                                var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

//                                                try
//                                                {
//                                                    // email notification
//                                                    //Utility.SendEmail("TransferSent", 
//                                                    //    fromAddress, toAddress, null,
//                                                    //    "Your $" + wholeAmount + " payment to " + recipientFirstName +
//                                                    //    " on Nooch",
//                                                    //    null, tokens, null, null, null);

//                                                    Utility.SendEmail("TransferSent", fromAddress, toAddress,
//                                                        "Your $ " + wholeAmount + " payment to " + recipientFirstName +
//                                                        " on Nooch", null,
//                                                        tokens, null, null, null);


//                                                    Logger.Info(
//                                                        "Add fund to members account New Admin --> TransferSent status mail sent to [" +
//                                                        toAddress + "].");
//                                                }
//                                                catch (Exception)
//                                                {
//                                                    Logger.Error(
//                                                        "Add fund to members account New Admin --> TransferSent mail NOT sent to [" +
//                                                        toAddress +
//                                                        "]. Problem occurred in sending mail.");
//                                                }


//                                            }
//                                        }

//                                        #endregion


//                                        #region EmailAndPushNotificationToRecepientOnTransferReceive

//                                        // for push notification
//                                        //var friendDetails = memberDataAccess.GetMemberNotificationSettingsByUserName(CommonHelper.GetDecryptedData(receiverAccountDetail.UserName));
//                                        var friendDetails2 =
//                                            CommonHelper.GetMemberNotificationSettings(
//                                                recepientdetails.MemberId.ToString());
//                                        if (friendDetails2 != null)
//                                        {
//                                            string deviceId2 = friendDetails2 != null
//                                                ? recepientdetails.DeviceToken
//                                                : null;

//                                            string mailBodyText = "You received $" + wholeAmount + " from " +
//                                                                  senderFirstName +
//                                                                  " " + senderLastName;

//                                            if ((friendDetails2.TransferReceived == null)
//                                                ? false
//                                                : friendDetails2.TransferReceived.Value)
//                                            {
//                                                try
//                                                {
//                                                    // push notifications
//                                                    if (friendDetails2 != null && !String.IsNullOrEmpty(deviceId2) &&
//                                                        (friendDetails2.TransferReceived ?? false))
//                                                    {
//                                                        Utility.SendNotificationMessage(mailBodyText, 1,
//                                                            null, deviceId2,
//                                                            Utility.GetValueFromConfig("AppKey"),
//                                                            Utility.GetValueFromConfig("MasterSecret"));

//                                                        Logger.Info(
//                                                            "Add fund to member from new admin panel --> Push notification sent to Sender DeviceID:[" +
//                                                            deviceId2 + "] successfully.");
//                                                    }
//                                                }
//                                                catch (Exception)
//                                                {
//                                                    Logger.Error(
//                                                        "Add fund to member from new admin panel --> Error: Push notification NOT sent to Sender DeviceID: [" +
//                                                        deviceId2 + "]");
//                                                }
//                                            }

//                                            // for TransferReceived email notification
//                                            if (friendDetails2 != null &&
//                                                (friendDetails2.EmailTransferReceived ?? false))
//                                            {
//                                                if (adminUserDetails.Photo != null && adminUserDetails.Photo != "")
//                                                {
//                                                    string lastFourOfSendersPic =
//                                                        adminUserDetails.Photo.Substring(adminUserDetails.Photo.Length -
//                                                                                         15);
//                                                    if (lastFourOfSendersPic != "gv_no_photo.png")
//                                                    {
//                                                        senderPic = "";
//                                                    }
//                                                    else
//                                                    {
//                                                        senderPic = adminUserDetails.Photo.ToString();
//                                                    }
//                                                }

//                                                var tokensR = new Dictionary<string, string>
//                                                {
//                                                    {Constants.PLACEHOLDER_FIRST_NAME, recipientFirstName},
//                                                    {
//                                                        Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
//                                                        senderFirstName + " " + senderLastName
//                                                    },
//                                                    {Constants.PLACEHOLDER_TRANSFER_AMOUNT, wholeAmount},
//                                                    {
//                                                        Constants.PLACEHOLDER_TRANSACTION_DATE,
//                                                        Convert.ToDateTime(trans.TransactionDate)
//                                                            .ToString("MMM dd")
//                                                    },
//                                                    {Constants.MEMO, memo}
//                                                };

//                                                // for TransferReceived email notification                            
//                                                var fromAddress = Utility.GetValueFromConfig("transfersMail");
//                                                var toAddress2 = CommonHelper.GetDecryptedData(recepientdetails.UserName);

//                                                try
//                                                {
//                                                    // email notification
//                                                    Utility.SendEmail("TransferReceived", fromAddress, toAddress2,
//                                                        senderFirstName + " sent you $" + wholeAmount + " with Nooch",
//                                                        null, tokensR, null, null, null);

//                                                    Logger.Info(
//                                                        "Add fund to member from new admin panel --> TransferReceived Email sent to [" +
//                                                        toAddress2 + "] successfully.");
//                                                }
//                                                catch (Exception)
//                                                {
//                                                    Logger.Error(
//                                                        "Add fund to member from new admin panel --> Error: TransferReceived Email NOT sent to [" +
//                                                        toAddress2 + "]");
//                                                }
//                                            }
//                                        }

//                                        #endregion



//                                        try
//                                        {
//                                            obj.GeoLocations.Add(geo);

//                                            obj.SaveChanges();


//                                            obj.Transactions.Add(trans);
//                                            obj.SaveChanges();
//                                            lr.IsSuccess = true;
//                                            lr.Message = "fund succesfully added to member account.";
//                                        }
//                                        catch (Exception)
//                                        {

//                                            lr.IsSuccess = false;
//                                            lr.Message = "Error occured while saving transaction in db.";
//                                        }


//                                        #endregion
//                                    }
//                                    else
//                                    {
//                                        #region emailSendingonTransferAttemtFailure

//                                        // for push notification in case of failure

//                                        var senderNotificationSettings =
//                                            CommonHelper.GetMemberNotificationSettings(
//                                                adminUserDetails.MemberId.ToString());

//                                        if (senderNotificationSettings != null)
//                                        {
//                                            string senderFirstNameFailure =
//                                                CommonHelper.UppercaseFirst(
//                                                    CommonHelper.GetDecryptedData(adminUserDetails.FirstName));
//                                            string senderLastNameFailure =
//                                                CommonHelper.UppercaseFirst(
//                                                    CommonHelper.GetDecryptedData(adminUserDetails.LastName));
//                                            string recipientFirstNameFailure =
//                                                CommonHelper.UppercaseFirst(
//                                                    CommonHelper.GetDecryptedData(recepientdetails.FirstName));
//                                            string recipientLastNameFailure =
//                                                CommonHelper.UppercaseFirst(
//                                                    CommonHelper.GetDecryptedData(recepientdetails.LastName));


//                                            // for TransferAttemptFailure email notification
//                                            if (senderNotificationSettings != null &&
//                                                (senderNotificationSettings.EmailTransferAttemptFailure ?? false))
//                                            {
//                                                string s2 = trans.Amount.ToString("n2");
//                                                string[] s3 = s2.Split('.');

//                                                var tokensF = new Dictionary<string, string>
//                                                {
//                                                    {
//                                                        Constants.PLACEHOLDER_FIRST_NAME,
//                                                        senderFirstNameFailure + " " + senderLastNameFailure
//                                                    },
//                                                    {
//                                                        Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
//                                                        CommonHelper.GetDecryptedData(recepientdetails.UserName)
//                                                    },
//                                                    {Constants.PLACEHOLDER_TRANSFER_AMOUNT, s3[0].ToString()},
//                                                    {Constants.PLACEHLODER_CENTS, s3[1].ToString()},
//                                                };

//                                                var fromAddress = Utility.GetValueFromConfig("transfersMail");
//                                                var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

//                                                try
//                                                {
//                                                    // email notification
//                                                    Utility.SendEmail("transferFailure",
//                                                        fromAddress, toAddress, null,
//                                                        "Nooch transfer failure", tokensF, null, null, null);

//                                                    Logger.Info(
//                                                        "Add fund to member new admin panel --> Transfer FAILED --> Email sent to Sender: [" +
//                                                        toAddress + "] successfully.");
//                                                }
//                                                catch (Exception)
//                                                {
//                                                    Logger.Error(
//                                                        "Add fund to member new admin panel --> Error: TransferAttemptFailure mail not sent to [" +
//                                                        toAddress + "]");
//                                                }
//                                            }
//                                        }

//                                        #endregion

//                                        lr.IsSuccess = false;
//                                        lr.Message = "Knox payment failed.";
//                                    }


//                                    */
//                                    #endregion
//                                }
//                                else
//                                {
//                                    #region emailSendingonTransferAttemtFailure
//                                    /*

//                                    // for push notification in case of failure

//                                    var senderNotificationSettings =
//                                        CommonHelper.GetMemberNotificationSettings(adminUserDetails.MemberId.ToString());

//                                    if (senderNotificationSettings != null)
//                                    {
//                                        string senderFirstNameFailure =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(adminUserDetails.FirstName));
//                                        string senderLastNameFailure =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(adminUserDetails.LastName));
//                                        string recipientFirstNameFailure =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(recepientdetails.FirstName));
//                                        string recipientLastNameFailure =
//                                            CommonHelper.UppercaseFirst(
//                                                CommonHelper.GetDecryptedData(recepientdetails.LastName));


//                                        // for TransferAttemptFailure email notification
//                                        if (senderNotificationSettings != null &&
//                                            (senderNotificationSettings.EmailTransferAttemptFailure ?? false))
//                                        {
//                                            string s2 = trans.Amount.ToString("n2");
//                                            string[] s3 = s2.Split('.');

//                                            var tokensF = new Dictionary<string, string>
//                                            {
//                                                {
//                                                    Constants.PLACEHOLDER_FIRST_NAME,
//                                                    senderFirstNameFailure + " " + senderLastNameFailure
//                                                },
//                                                {
//                                                    Constants.PLACEHOLDER_FRIEND_FIRST_NAME,
//                                                    CommonHelper.GetDecryptedData(recepientdetails.UserName)
//                                                },
//                                                {Constants.PLACEHOLDER_TRANSFER_AMOUNT, s3[0].ToString()},
//                                                {Constants.PLACEHLODER_CENTS, s3[1].ToString()},
//                                            };

//                                            var fromAddress = Utility.GetValueFromConfig("transfersMail");
//                                            var toAddress = CommonHelper.GetDecryptedData(adminUserDetails.UserName);

//                                            try
//                                            {
//                                                // email notification
//                                                Utility.SendEmail("transferFailure",
//                                                    fromAddress, toAddress, null,
//                                                    "Nooch transfer failure", tokensF, null, null, null);

//                                                Logger.Info(
//                                                    "Add fund to member new admin panel --> Transfer FAILED --> Email sent to Sender: [" +
//                                                    toAddress + "] successfully.");
//                                            }
//                                            catch (Exception)
//                                            {
//                                                Logger.Error(
//                                                    "Add fund to member new admin panel --> Error: TransferAttemptFailure mail not sent to [" +
//                                                    toAddress + "]");
//                                            }
//                                        }
//                                    }
//*/
//                                    #endregion

//                                    lr.IsSuccess = false;
//                                    lr.Message = "Synapse payment failed.";
//                                }
//                            }
//                            else
//                            {
//                                lr.IsSuccess = false;
//                                lr.Message = "Recepeint Synapse account not available.";
//                            }
//                        }
//                        else
//                        {
//                            lr.IsSuccess = false;
//                            lr.Message = "Given username/nooch id not found or give username/nooch id not active.";
//                        }
//                    }
//                    else
//                    {
//                        lr.IsSuccess = false;
//                        lr.Message = "Admin Synapse account not available.";
//                    }
//                }
//                else
//                {
//                    lr.IsSuccess = false;
//                    lr.Message = "Admin account team@nooch.com not active or invalid admin PIN passed.";
//                }
//            }
//            return Json(lr);
//        }


        [HttpPost]
        [ActionName("CreditFundToMemberPostSynapseV3Test")]
        public ActionResult CreditFundToMemberPostSynapseV3Test(string transferfundto, string transferAmount, string transferNotes, string adminPin)
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
                    #region Admin Member and synapse details
                    Guid AdminMemberId = Utility.ConvertToGuid(adminUserDetails.MemberId.ToString());

                    // Get Synapse account details of admin
                    var adminSynapseDetails =
                        CommonHelper.GetSynapseBankAndUserDetailsforGivenMemberId(AdminMemberId.ToString());

                    if (adminSynapseDetails.wereBankDetailsFound != true)
                    {
                        Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Transfer ABORTED: Requester's Synapse bank account NOT FOUND - Trans Creator MemberId is: [" + AdminMemberId + "]");
                        lr.Message = "Admin does not have any bank added";
                        lr.IsSuccess = false;
                        return Json(lr);
                    }

                    // Check Admins's Synapse Bank Account status
                    if (adminSynapseDetails.BankDetails != null &&
                        adminSynapseDetails.BankDetails.Status != "Verified" &&
                        adminUserDetails.IsVerifiedWithSynapse != true)
                    {
                        Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Admin's Synapse bank account exists but is not Verified and " +
                            "isVerifiedWithSynapse != true - Admin memberId is: [" + adminUserDetails.MemberId + "]");
                        lr.Message = "Admin does not have any verified bank account.";
                        lr.IsSuccess = false;
                        return Json(lr);
                    }
                    #endregion

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

                        var recipientBankDetails =
                                                    CommonHelper.GetSynapseBankAndUserDetailsforGivenMemberId(recepeintGuid.ToString());

                        if (recipientBankDetails.wereBankDetailsFound != true)
                        {
                            Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Transfer ABORTED: Recepient's Synapse bank account NOT FOUND - Trans Creator MemberId is: [" + recepeintGuid + "]");
                            lr.Message = "Recepient does not have any bank added";
                            lr.IsSuccess = false;
                            return Json(lr);
                        }

                        // Check Admins's Synapse Bank Account status
                        if (recipientBankDetails.BankDetails != null &&
                            recipientBankDetails.BankDetails.Status != "Verified" &&
                            recipientMemberDetails.IsVerifiedWithSynapse != true)
                        {
                            Logger.Error("Add fund to members account New Admin -> Transfer FAILED -> Recepient's Synapse bank account exists but is not Verified and " +
                                "isVerifiedWithSynapse != true - Recepient memberId is: [" + adminUserDetails.MemberId + "]");
                            lr.Message = "Recepient does not have any verified bank account.";
                            lr.IsSuccess = false;
                            return Json(lr);
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

                            string sender_oauth = CommonHelper.GetDecryptedData( adminSynapseDetails.UserDetails.access_token);
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
                                    // Add entry in SynapseV3CreateTransResults Table in Nooch DB
                                    SynapseV3CreateTransResults orderRes = new SynapseV3CreateTransResults();

                                    #region Preparing stuff to save in Synapse Create Order Result Table V3

                                    orderRes.trans_amount = transferAmount;
                                    orderRes.trans_id_oid =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans._id.ToString();
                                    orderRes.trans_currency =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.amount.currency;

                                    orderRes.extra_created_on_date =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.extra.created_on
                                            .ToString();
                                    orderRes.extra_process_on_date =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.extra.process_on
                                            .ToString();
                                    orderRes.extra_supp_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.extra.supp_id.ToString
                                            ();

                                    orderRes.synapse_fee_fee =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.fees[0].fee;
                                    orderRes.synapse_fee_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.fees[0].to.id.oid;
                                    orderRes.synapse_fee_note =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.fees[0].note;

                                    orderRes.nooch_fee_fee =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.fees[1].fee;
                                    orderRes.nooch_fee_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.fees[1].to.id.oid;
                                    orderRes.nooch_fee_note =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.fees[1].note;

                                    orderRes.from_user_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.from.user._id.id
                                            .ToString();
                                    //orderRes.from_node_id= sender_bank_node_id;
                                    orderRes.from_node_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.from.id.oid;
                                    orderRes.from_node_type =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.from.type;
                                    //orderRes.from_node_nickname=



                                    orderRes.recent_status =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.recent_status.status;
                                    orderRes.recent_note =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.recent_status.note;
                                    orderRes.recent_status_date =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.recent_status.date
                                            .ToString();
                                    orderRes.recent_status_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.recent_status
                                            .status_id;


                                    orderRes.to_user_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.to.user._id.id;
                                    orderRes.to_node_id =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.to.id.oid;
                                    orderRes.to_node_type =
                                        transactionResultFromSynapseAPI.responseFromSynapse.trans.to.type;

                                    orderRes.NoochTransactionDate = TransactionDateTimeToUSe;
                                    orderRes.NoochTransactionId = TransactionIdToUse.ToString();

                                    #endregion

                                    obj.SynapseV3CreateTransResults.Add(orderRes);
                                    saveToSynapseCreateOrderTable = obj.SaveChanges();

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
                            Logger.Error(
                                        "Add fund to members account New Admin -> Transfer FAILED ->  [Exception: " + ex +
                                        "]");

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
                                    lr.Message = "There was a problem with Synapse.";
                                    lr.IsSuccess = false;
                                    return Json(lr);

                                }
                                else if (saveToTransTable == 0 || saveToSynapseCreateOrderTable == 0)
                                {
                                    lr.Message = "There was a problem updating Nooch DB tables.";
                                    lr.IsSuccess = false;
                                    return Json(lr);
                                }
                                else
                                {
                                    lr.Message = "Unknown Failure";
                                    lr.IsSuccess = false;
                                    return Json(lr);

                                }
                            }
                        }
                        #endregion Failure Sections
                        else if (shouldSendFailureNotifications == 0 && saveToSynapseCreateOrderTable == 1 &&
                            saveToTransTable == 1)
                        {
                            #region Success notifications
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

                            #endregion
                            lr.Message = "Your cash was sent successfully.";
                            lr.IsSuccess = true;
                            return Json(lr);
                        }
                        else
                        {
                            lr.Message = "Server Error.";
                            lr.IsSuccess = false;
                            return Json(lr);
                        }



                    }

                    else
                    {
                        lr.IsSuccess = false;
                        lr.Message = "Given username/nooch id not found or give username/nooch id not active.";
                        return Json(lr);
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

        [HttpPost]
        [ActionName("GetUsersOverTimeOverTimeData")]
        public ActionResult GetUsersOverTimeOverTimeData(string recordType,string status)
        {

            getUserOverTimeResult res = new getUserOverTimeResult();
            using (NOOCHEntities obj = new NOOCHEntities())
            {

                //List<getUserOverTimeResult> users_over_time = new List<getUserOverTimeResult>();
                List<internalDataArray> extList = new List<internalDataArray>();
                List<Member> memList = new List<Member>();
                List<DurationArray> duraionList = new List<DurationArray>();
                
                 

                if (recordType == "weekly")
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
                            nre[1] = (from t in obj.Members where t.DateCreated == past7days && (t.Status=="Active" || t.Status=="Registered")  select t).Count().ToString();
                        }
                        else if (status == "1")
                        {
                            nre[1] = (from t in obj.Members where t.DateCreated == past7days && (t.Status != "Active" && t.Status != "Registered" && t.Status != "Suspended" && t.Status != "Deleted") select t).Count().ToString();
                        }
                        
                        string[] nda = new string[2];
                        nda[0] = i.ToString();
                        nda[1] = past7days.DayOfWeek.ToString();
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

                else if (recordType == "monthly")
                {
                    DateTime todayDate = DateTime.Now;
                    DateTime past7days = DateTime.Now.AddMonths(-7);
                    int i = 0;
                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();// past7days.DayOfWeek.ToString();
                        if (status == "0")
                        {
                            nre[1] = (from t in obj.Members where t.DateCreated.Value.Month == past7days.Month && (t.Status == "Active" || t.Status == "Registered") select t).Count().ToString();
                        }else if (status == "1")
                            nre[1] = (from t in obj.Members where t.DateCreated.Value.Month == past7days.Month && (t.Status != "Active" && t.Status != "Registered" && t.Status != "Suspended" && t.Status != "Deleted") select t).Count().ToString();
                        string[] nda = new string[2];
                        nda[0] = i.ToString();
                        nda[1] = past7days.ToString("MMM");
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
                    DateTime past7days = DateTime.Now.AddYears(-7);
                    int i = 0;
                    while (past7days <= todayDate)
                    {
                        string[] nre = new string[2];
                        nre[0] = i.ToString();// past7days.DayOfWeek.ToString();
                        if (status == "0")
                        {
                            nre[1] = (from t in obj.Members where t.DateCreated.Value.Year == past7days.Year && (t.Status == "Active" || t.Status == "Registered") select t).Count().ToString();
                        }else if(status=="1")
                            nre[1] = (from t in obj.Members where t.DateCreated.Value.Year == past7days.Year && (t.Status != "Active" && t.Status != "Registered" && t.Status != "Suspended" && t.Status != "Deleted") select t).Count().ToString();
                            
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
            TransactionsPageData res = new TransactionsPageData();

            List<TransactionClass> adminUser = new List<TransactionClass>();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                List<Transaction> admin = new List<Transaction>();

                adminUser = (from t in obj.Transactions

                             join g in obj.GeoLocations
                             on
                              t.LocationId equals g.LocationId

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
                                 TransAlti = g.Altitude,
                                 TransLati = g.Latitude,
                                 state = g.State,
                                 city = g.City

                             }

                              ).ToList();

                foreach (var transaction in adminUser.ToList())
                {
                    //+C1+zhVafHdXQXCIqjU/Zg== -- Disputed
                    // 5/KChkAfEoa6N2oo8FHwWQ== -- Reward
                    // 5dt4HUwCue532sNmw3LKDQ== - Transfer

                    // DrRr1tU1usk7nNibjtcZkA== - Invite
                    // EnOIzpmFFTEaAP16hm9Wsw== -- Rent
                    // T3EMY1WWZ9IscHIj3dbcNw== - Request

                    if (transaction.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw==")
                    {
                        transaction.TransactionType = "Reward";
                        // sender user
                        Member sender = CommonHelper.GetMemberUsingGivenMemberId(transaction.RecipientId.ToString());
                        transaction.SenderNoochId = sender.Nooch_ID.ToString();
                        transaction.SenderName = CommonHelper.GetDecryptedData(sender.FirstName) + " " + CommonHelper.GetDecryptedData(sender.LastName);
                        transaction.SenderId = sender.MemberId;

                        Member receiver = CommonHelper.GetMemberUsingGivenMemberId(transaction.SenderId.ToString());
                        transaction.RecepientNoochId = receiver.Nooch_ID.ToString();
                        transaction.RecipienName = CommonHelper.GetDecryptedData(receiver.FirstName) + " " + CommonHelper.GetDecryptedData(receiver.LastName);
                        transaction.RecipientId = receiver.MemberId;
                    }
                    else
                    {
                        transaction.TransactionType = CommonHelper.GetDecryptedData(transaction.TransactionType);
                        // sender user
                        Member sender = CommonHelper.GetMemberUsingGivenMemberId(transaction.SenderId.ToString());
                        transaction.SenderNoochId = sender.Nooch_ID.ToString();
                        transaction.SenderName = CommonHelper.GetDecryptedData(sender.FirstName) + " " + CommonHelper.GetDecryptedData(sender.LastName);

                        Member receiver = CommonHelper.GetMemberUsingGivenMemberId(transaction.RecipientId.ToString());
                        transaction.RecepientNoochId = receiver.Nooch_ID.ToString();
                        transaction.RecipienName = CommonHelper.GetDecryptedData(receiver.FirstName) + " " + CommonHelper.GetDecryptedData(receiver.LastName);
                        transaction.RecipientId = receiver.MemberId;
                    }

                    adminUser.Add(transaction);
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
                var member = (from t in obj.Transactions where t.TransactionId == g select t).SingleOrDefault();
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