using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using noochAdminNew.Classes.ModelClasses;
using noochAdminNew.Models;
using Newtonsoft.Json;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Classes.ResponseClasses;
using System.Net.Mail;
using System.Net;
using noochAdminNew.Resources;

namespace noochAdminNew.Controllers
{
    public class DisputesController : Controller
    {

        [HttpPost]
        [ActionName("ApplyDisputeOperation")]

        public string ApplyDisputeOperation(string operation, string DisputeId, string AdminNotes)
        {

            MemberFinalResultOperation Main = new MemberFinalResultOperation();

            MemberDisputeOperationsResultClass mr = new MemberDisputeOperationsResultClass();
            List<MemberDisputeOperationsResultClass> allinnerclass = new List<MemberDisputeOperationsResultClass>();

            MemberDisputeStatusChangeClass ll = new MemberDisputeStatusChangeClass();
            var fromAddress = Utility.GetValueFromConfig("adminMail");
            var deviceId = string.Empty;
            bool isAllowPushNotifications = false;
            string resultMessage = string.Empty;

            Guid GUID_MemberId = Utility.ConvertToGuid((Session["UserId"].ToString()));
            string MemberId = GUID_MemberId.ToString();

            if (operation == "0")
            {
                Main.IsSuccess = false;
                Main.Message = "No operation selected to perform";
                string json = JsonConvert.SerializeObject(Main);
                return json;
            }

            if (DisputeId.Length == 0)
            {
                Main.IsSuccess = false;
                Main.Message = "No user selected to perfrom action";
                // return Json(mr);
                string json = JsonConvert.SerializeObject(Main);
                return json;
            }


            try
            {
                DisputeId = DisputeId.TrimEnd(',');
                List<string> allNoochIdsChoosen = DisputeId.Split(',').ToList<string>();

                #region Operations
                //If the Status is Under Review

                #region Review Operations
                if (operation == "1")
                {
                    var disputeStatus = "Under Review";

                    foreach (string s in allNoochIdsChoosen)
                    {
                        string disputeIDD = s;
                        {
                            using (NOOCHEntities obj = new NOOCHEntities())
                            {
                                var AdminName = (from t in obj.AdminUsers where t.UserId == GUID_MemberId select t.UserName).SingleOrDefault();

                                var DisputeQuery = (from c in obj.Transactions
                                                    join sendr in obj.Members on c.SenderId equals sendr.MemberId
                                                    join Receipt in obj.Members on c.RecipientId equals Receipt.MemberId
                                                    join LocationDetail in obj.GeoLocations on c.LocationId equals LocationDetail.LocationId
                                                    where c.DisputeTrackingId == disputeIDD
                                                    select new MemberDisputeStatusChangeClass
                                                    {
                                                        AdminNotee = c.AdminNotes,
                                                        DisputeStatus = c.DisputeStatus,
                                                        ReceiptFirstName = Receipt.FirstName,
                                                        ReceiptLastName = Receipt.LastName,
                                                        DisputeDate = c.DisputeDate,
                                                        DisputeId = c.DisputeTrackingId,
                                                        TransactionId = c.TransactionTrackingId,
                                                        SenderFirstName = sendr.FirstName,
                                                        SenderLastName = sendr.LastName,
                                                        Amount = c.Amount,
                                                        TransactionDate = c.TransactionDate,
                                                        TransactionLocation = LocationDetail.AddressLine1 + ", " + LocationDetail.AddressLine2 + ", " + LocationDetail.City + ", " + LocationDetail.State,
                                                        SenderImg = sendr.Photo,
                                                        ReceiptImg = Receipt.Photo,
                                                        RaisedBy = c.RaisedBy,
                                                        SenderNoochId = sendr.Nooch_ID,
                                                        ReceiptNoochId = Receipt.Nooch_ID,
                                                        SenderUserName = sendr.UserName,
                                                        ReceiptUserName = Receipt.UserName
                                                    }).SingleOrDefault();


                                if (DisputeQuery != null)
                                {
                                    var tt = obj.Transactions.First(o => o.DisputeTrackingId == disputeIDD);
                                    tt.AdminNotes = " ";
                                    tt.DisputeStatus = CommonHelper.GetEncryptedData(disputeStatus);
                                    tt.ResolvedDate = DateTime.Now;
                                    tt.AdminName = AdminName.ToString();
                                    if (!string.IsNullOrEmpty(AdminNotes))
                                    {
                                        tt.AdminNotes = AdminNotes.ToString();
                                    }

                                    //var mem = obj.Members.First(o => o.Nooch_ID == DisputeQuery.SenderNoochId && o.Status != Constants.STATUS_ACTIVE );
                                    //mem.Status = Constants.STATUS_ACTIVE;
                                    //mem.DateModified = DateTime.Now;
                                    //mem.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());


                                    int i = obj.SaveChanges();

                                    #region DisputeTakenForReviewCode

                                    if (disputeStatus.Equals("Under Review"))
                                    {
                                        #region Send Email To Sender

                                        string sender_FirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.SenderFirstName));
                                        string sender_LastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.SenderLastName));
                                        string recip_FirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.ReceiptFirstName));
                                        string recip_LastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.ReceiptLastName));

                                        string sender_EmailAddress = CommonHelper.GetDecryptedData(DisputeQuery.SenderUserName);
                                        string recip_EmailAddress = CommonHelper.GetDecryptedData(DisputeQuery.ReceiptUserName);

                                        string amountVal = DisputeQuery.Amount.ToString("n2");

                                        var tokens = new Dictionary<string, string>
                                        {
                                                                      
                                            {Constants.PLACEHOLDER_FIRST_NAME, sender_FirstName },
                                            {Constants.PLACEHOLDER_TRANSACTION_ID,  DisputeQuery.DisputeId },
                                            {Constants.PLACEHOLDER_TRANSFER_AMOUNT, amountVal },
                                            {Constants.PLACEHOLDER_TRANSACTION_DATE,  Convert.ToDateTime(DisputeQuery.TransactionDate).ToString("MMM dd yyyy") },
                                            {Constants.PLACEHOLDER_RECIPIENT_FIRST_NAME,  recip_FirstName + " " + recip_LastName }
                                        };

                                        try
                                        {
                                            Logger.Info("ChangeDisputeStatus - Dispute Taken For Review email sent to: ["
                                                            + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

                                            Utility.SendEmail("DisputeReviewMailTemplate_Sender", fromAddress, sender_EmailAddress,
                                                    "Nooch Dispute taken for review", null, tokens, null, null, null);

                                            mr.IsSuccess = true;
                                            mr.Message = "emailSend";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);
                                        }
                                        catch (Exception)
                                        {
                                            Logger.Error("ChangeDisputeStatus -> Dispute Taken For Review email NOT sent to: [" + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

                                            mr.IsSuccess = false;
                                            mr.Message = "EmailNotSent";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);
                                        }

                                        #endregion


                                        //RAKESH : IF RECEIPIENT NOT ABLE TO DIPUTE THE TRANSACTION AND WE ARE NOT SENDING MAIL TO RECEIPT ,SO CAN I REMOVE THIS COMMENTED SECTION  ...
                                        //CLIFF: No, please see my original note below... we still need to notify BOTH users about the resolution

                                        //#region Send Email To Recipient // NOTE: RECIPIENTS CANNOT DISPUTE A TRANSFER ON NOOCH, SO THE SENDER IS ALWAYS THE DISPUT-ER.  NOW NEED TO NOTIFY THE RECIPIENT.

                                        //
                                        /*RAKESH : Okk Cliff. Now ,I made the regions to notify receipient in both cases('Under Review','Resolved')  
                                        * and make changes in functions as you suggested i.e the popup and multiple selection option for dispute
                                        * status change , now  using one method ( "ApplyDisputeOperation") ,So no need for other method,
                                        * kindly check it , then i remove the "ChangeDisputeStatus" method from page.*/


                                        #region Notify Recipient
                                        tokens = new Dictionary<string, string>
                                   {
                                        {Constants.PLACEHOLDER_FIRST_NAME, recip_FirstName },
                                        {Constants.PLACEHOLDER_TRANSACTION_ID,  DisputeQuery.DisputeId },
                                        {Constants.PLACEHOLDER_TRANSFER_AMOUNT, amountVal },
                                        {Constants.PLACEHOLDER_TRANSACTION_DATE,  Convert.ToDateTime(DisputeQuery.TransactionDate).ToString("MMM dd yyyy") },
                                        {Constants.PLACEHOLDER_RECIPIENT_FIRST_NAME,  sender_FirstName + " " + sender_LastName }
                                  };

                                        try
                                        {
                                            Utility.SendEmail("DisputeReviewMailTemplate_Recip", fromAddress, recip_EmailAddress,
                                                    "Nooch Dispute taken for review", null, tokens, null, null, null);

                                            Logger.Info("ChangeDisputeStatus -> Dispute Resolved email sent to: ["
                                                        + recip_FirstName + " " + recip_LastName + " | " + recip_EmailAddress + "]");

                                            mr.IsSuccess = true;
                                            mr.Message = "emailSend";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);
                                        }
                                        catch (Exception)
                                        {
                                            Logger.Error("ChangeDisputeStatus - Dispute status mail NOT sent to: [" + recip_FirstName +
                                                         " " + recip_LastName + " | " + recip_EmailAddress + "]");


                                            mr.IsSuccess = false;
                                            mr.Message = "EmailNotSent";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);
                                        }
                                        #endregion


                                    }
                                    #endregion
                #endregion
                                }
                            }
                        }
                    }
                    Main.IsSuccess = true;
                    Main.Message = "all operations performed";
                    Main.MemberDisputeSUbListClass = allinnerclass;
                }

                #endregion Review

                ///////////////////////
                else if (operation == "2")
                {
                    var disputeStatus = "Resolved";

                    foreach (string s in allNoochIdsChoosen)
                    {
                        string disputeIDD = s;
                        {
                            using (NOOCHEntities obj = new NOOCHEntities())
                            {
                                var AdminName = (from t in obj.AdminUsers where t.UserId == GUID_MemberId select t.UserName).SingleOrDefault();

                                var DisputeQuery = (from c in obj.Transactions
                                                    join sendr in obj.Members on c.SenderId equals sendr.MemberId
                                                    join Receipt in obj.Members on c.RecipientId equals Receipt.MemberId
                                                    join LocationDetail in obj.GeoLocations on c.LocationId equals LocationDetail.LocationId
                                                    where c.DisputeTrackingId == disputeIDD
                                                    select new MemberDisputeStatusChangeClass
                                                    {
                                                        AdminNotee = c.AdminNotes,
                                                        DisputeStatus = c.DisputeStatus,
                                                        ReceiptFirstName = Receipt.FirstName,
                                                        ReceiptLastName = Receipt.LastName,
                                                        DisputeDate = c.DisputeDate,
                                                        DisputeId = c.DisputeTrackingId,
                                                        TransactionId = c.TransactionTrackingId,
                                                        SenderFirstName = sendr.FirstName,
                                                        SenderLastName = sendr.LastName,
                                                        Amount = c.Amount,
                                                        TransactionDate = c.TransactionDate,
                                                        TransactionLocation = LocationDetail.AddressLine1 + ", " + LocationDetail.AddressLine2 + ", " + LocationDetail.City + ", " + LocationDetail.State,
                                                        SenderImg = sendr.Photo,
                                                        ReceiptImg = Receipt.Photo,
                                                        RaisedBy = c.RaisedBy,
                                                        SenderNoochId = sendr.Nooch_ID,
                                                        ReceiptNoochId = Receipt.Nooch_ID,
                                                        SenderUserName = sendr.UserName,
                                                        ReceiptUserName = Receipt.UserName
                                                    }).SingleOrDefault();

                                if (DisputeQuery != null)
                                {
                                    var tt = obj.Transactions.First(o => o.DisputeTrackingId == disputeIDD);
                                    tt.AdminNotes = " ";
                                    tt.DisputeStatus = CommonHelper.GetEncryptedData(disputeStatus);
                                    tt.ResolvedDate = DateTime.Now;
                                    tt.AdminName = AdminName.ToString();
                                    if (!string.IsNullOrEmpty(AdminNotes))
                                    {
                                        tt.AdminNotes = AdminNotes.ToString();
                                    }

                                    ////
                                    var mem = obj.Members.First(o => o.Nooch_ID == DisputeQuery.SenderNoochId && o.Status != Constants.STATUS_ACTIVE);
                                    mem.Status = Constants.STATUS_ACTIVE;
                                    mem.InvalidLoginAttemptCount = 0;
                                    mem.InvalidPinAttemptCount = null;
                                    mem.InvalidLoginTime = null;
                                    mem.InvalidPinAttemptTime = null;
                                    mem.DateModified = DateTime.Now;
                                    mem.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());








                                    int i = obj.SaveChanges();

                                    #region DisputeTakenForReviewCode

                                    if (disputeStatus.Equals("Resolved"))
                                    {
                                        #region Sender
                                        //if (DisputeQuery.RaisedBy.Equals("Sender"))
                                        //{
                                        string sender_FirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.SenderFirstName));
                                        string sender_LastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.SenderLastName));
                                        string recip_FirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.ReceiptFirstName));
                                        string recip_LastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.ReceiptLastName));

                                        string sender_EmailAddress = CommonHelper.GetDecryptedData(DisputeQuery.SenderUserName);
                                        string recip_EmailAddress = CommonHelper.GetDecryptedData(DisputeQuery.ReceiptUserName);

                                        string amountVal = DisputeQuery.Amount.ToString("n2");

                                        //////////////////////////
                                        var tokens = new Dictionary<string, string>
                                            {
                                            {Constants.PLACEHOLDER_FIRST_NAME, sender_FirstName },
                                            {Constants.PLACEHOLDER_TRANSACTION_ID,  DisputeQuery.DisputeId },
                                            {Constants.PLACEHOLDER_TRANSFER_AMOUNT, amountVal },
                                            {Constants.PLACEHOLDER_TRANSACTION_DATE,  Convert.ToDateTime(DisputeQuery.TransactionDate).ToString("MMM dd yyyy") },
                                            {Constants.PLACEHOLDER_RECIPIENT_FIRST_NAME,  recip_FirstName + " " + recip_LastName } };

                                        try
                                        {
                                            Logger.Info("ChangeDisputeStatus -> Dispute status mail sent to:[ "
                                                            + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

                                            Utility.SendEmail("DisputeResolvedMailTemplate_Sender", fromAddress, sender_EmailAddress,
                                                "Nooch Dispute has been resolved successfully", null, tokens, null, null, null);

                                            mr.IsSuccess = true;
                                            mr.Message = "EmailSent";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);
                                        }
                                        catch (Exception)
                                        {
                                            Logger.Error("ChangeDisputeStatus -> Dispute status Email not sent to: [" + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

                                            mr.IsSuccess = false;
                                            mr.Message = "EmailNotSent";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);
                                        }
                                        //}
                                        #endregion

                                        #region Notify Recipient
                                        tokens = new Dictionary<string, string>
                                {
                                        {Constants.PLACEHOLDER_FIRST_NAME, recip_FirstName },
                                        {Constants.PLACEHOLDER_TRANSACTION_ID,  DisputeQuery.DisputeId },
                                        {Constants.PLACEHOLDER_TRANSFER_AMOUNT, amountVal },
                                        {Constants.PLACEHOLDER_TRANSACTION_DATE,  Convert.ToDateTime(DisputeQuery.TransactionDate).ToString("MMM dd yyyy") },
                                        {Constants.PLACEHOLDER_RECIPIENT_FIRST_NAME,  sender_FirstName + " " + sender_LastName }
                                };

                                        try
                                        {

                                            Logger.Info("ChangeDisputeStatus -> Dispute Resolved email sent to: ["
                                                        + recip_FirstName + " " + recip_LastName + " | " + recip_EmailAddress + "]");

                                            Utility.SendEmail("DisputeResolvedMailTemplate_Recip", fromAddress, recip_EmailAddress,
                                                    "Nooch Dispute has been resolved successfully", null, tokens, null, null, null);


                                            mr.IsSuccess = true;
                                            mr.Message = "emailSend";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);

                                        }
                                        catch (Exception)
                                        {
                                            Logger.Error("ChangeDisputeStatus - Dispute status mail NOT sent to: [" + recip_FirstName +
                                                         " " + recip_LastName + " | " + recip_EmailAddress + "]");

                                            mr.IsSuccess = false;
                                            mr.Message = "EmailNotSent";
                                            mr.DisputeId = DisputeQuery.DisputeId;
                                            allinnerclass.Add(mr);
                                        }
                                        #endregion

                                    }
                                    #endregion
                                }
                            }
                        }
                        Main.IsSuccess = true;
                        Main.Message = "all operations performed";
                        Main.MemberDisputeSUbListClass = allinnerclass;
                    }
                }
                string json = JsonConvert.SerializeObject(Main);
                return json;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Main.IsSuccess = false;
                Main.Message = "Dispute update failed :(";

                string json = JsonConvert.SerializeObject(Main);
                return json;
            }
        }


        // GET: /Disputes/
        //[HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ListAll()
        {
            DisputesListAllDetails DisList = new DisputesListAllDetails();
            List<DisputeClassDetails> dd = new List<DisputeClassDetails>();

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

                    var query = (from Disputetran in obj.Transactions
                                 join member in obj.Members on Disputetran.SenderId equals member.MemberId
                                 join membr1 in obj.Members on Disputetran.RecipientId equals membr1.MemberId
                                 where Disputetran.DisputeTrackingId != string.Empty && Disputetran.DisputeStatus != "RZKgHECeAxJW/E+/IRj3Dg=="
                                 select new
                                 {
                                     status = Disputetran.DisputeStatus,
                                     DisputeDate = Disputetran.DisputeDate,
                                     DisputeId = Disputetran.DisputeTrackingId,
                                     TransactionId = Disputetran.TransactionId,
                                     SenderFirstName = member.FirstName,
                                     SenderLastName = member.LastName,
                                     ResolveDate = Disputetran.ResolvedDate,
                                     RecipientFirstName = membr1.FirstName,
                                     receiptLastName = membr1.LastName,
                                     SenderNoochId = member.Nooch_ID,
                                     ReceiptNoochId = membr1.Nooch_ID,
                                 }).ToList();

                    // No Of Under Review transaction 
                    var c = query.Count(t => t.status == "oRhhSHTgR2+/GyhkNL8acw=="); // 'oRhhSHTgR2+/GyhkNL8acw==' is 'Reported'
                    DisList.NoOFDisputeUnderReview = c.ToString();

                    // No Of new Disputed  transaction 
                    c = query.Count(t => t.status == "7J2lG6wsvZKpEs16CQjlTA==");
                    DisList.NoOfTransactionUnderReview = c.ToString();

                    foreach (var item in query)
                    {
                        dd.Add(new DisputeClassDetails
                        {
                            status = CommonHelper.GetDecryptedData(item.status),
                            DisputeDate = item.DisputeDate,
                            DisputeId = item.DisputeId,
                            TransactionId = item.TransactionId.ToString(),
                            Sender = CommonHelper.GetDecryptedData(item.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(item.SenderLastName.ToString()),
                            ResolveDate = item.ResolveDate,
                            SenderNoochId = item.SenderNoochId.ToString(),
                            ReceipentNoochId = item.ReceiptNoochId.ToString(),
                            Recipient = CommonHelper.GetDecryptedData(item.RecipientFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(item.receiptLastName.ToString()),
                        });
                    }

                    DisList.DisputeClassDetails = dd;
                }
            }
            return View(DisList);
        }

        [HttpPost]
        [ActionName("Details")]
        public ActionResult Details(string id)
        {
            DisputeDetailsResultClass DD = new DisputeDetailsResultClass();

            if (String.IsNullOrEmpty(id))
            {
                DD.IsResultSucess = false;
                DD.ResultMessage = "Error";
            }
            else
            {
                using (NOOCHEntities obj = new NOOCHEntities())
                {
                    var memd = (from c in obj.Transactions
                                join sendr in obj.Members on c.SenderId equals sendr.MemberId
                                join Receipt in obj.Members on c.RecipientId equals Receipt.MemberId
                                join LocationDetail in obj.GeoLocations on c.LocationId equals LocationDetail.LocationId
                                where c.DisputeTrackingId == id
                                select new
                                {
                                    ReceiptFirstName = Receipt.FirstName,
                                    ReceiptLastName = Receipt.LastName,
                                    DisputeDate = c.DisputeDate,
                                    DisputeId = c.DisputeTrackingId,
                                    TransactionId = c.TransactionTrackingId,
                                    SenderFirstName = sendr.FirstName,
                                    SenderLastName = sendr.LastName,
                                    Amount = c.Amount,
                                    TransactionDate = c.TransactionDate,
                                    TransactionLocation = LocationDetail.AddressLine1 + ", " + LocationDetail.AddressLine2 + ", " + LocationDetail.City + ", " + LocationDetail.State,
                                    SenderImg = sendr.Photo,
                                    ReceiptImg = Receipt.Photo,
                                    Memo = c.Memo
                                }).ToList();

                    foreach (var item in memd)
                    {
                        {
                            DD.Amount = item.Amount.ToString();
                            DD.DisputeDate = item.DisputeDate;
                            DD.DisputeId = item.DisputeId;
                            DD.TransactionId = item.TransactionId.ToString();
                            DD.SenderId = CommonHelper.GetDecryptedData(item.SenderFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(item.SenderLastName.ToString());
                            DD.ReceiptId = CommonHelper.GetDecryptedData(item.ReceiptFirstName.ToString()) + " " + CommonHelper.GetDecryptedData(item.ReceiptLastName.ToString());
                            DD.Transactionlocation = item.TransactionLocation;
                            DD.TransactionDate = item.TransactionDate;
                            DD.SenderImg = item.SenderImg.ToString();
                            DD.ReceiptImg = item.ReceiptImg.ToString();
                            DD.TransactionFor = item.Memo;
                        }
                        DD.IsResultSucess = true;
                        DD.ResultMessage = "DataFetch";
                    }
                }
            }
            return Json(DD);
        }

        //[HttpPost]
        //[ActionName("ChangeDisputeStatus")]
        //public string ChangeDisputeStatus(string DisputeId, string AdminNotes, string disputeStatus)
        //{
        //    using (NOOCHEntities obj = new NOOCHEntities())
        //    {
        //        // Email Variables
        //        MemberDisputeOperationsResultClass ll = new MemberDisputeOperationsResultClass();
        //        var fromAddress = Utility.GetValueFromConfig("adminMail");
        //        var mailBodyText = string.Empty;
        //        var emailAddress = string.Empty;
        //        var deviceId = string.Empty;
        //        bool isAllowPushNotifications = false;
        //        string resultMessage = string.Empty;
        //        string firstName = string.Empty;
        //        string lastName = string.Empty;

        //        Guid GUID_MemberId = Utility.ConvertToGuid((Session["UserId"].ToString()));
        //        string MemberId = GUID_MemberId.ToString();

        //        var AdminName = (from t in obj.AdminUsers where t.UserId == GUID_MemberId select t.UserName).SingleOrDefault();

        //        //Get the Transaction's info

        //        var DisputeQuery = (from c in obj.Transactions
        //                            join sendr in obj.Members on c.SenderId equals sendr.MemberId
        //                            join Receipt in obj.Members on c.RecipientId equals Receipt.MemberId
        //                            join LocationDetail in obj.GeoLocations on c.LocationId equals LocationDetail.LocationId
        //                            where c.DisputeTrackingId == DisputeId
        //                            select new MemberDisputeStatusChangeClass
        //                            {
        //                                AdminNotee = c.AdminNotes,
        //                                DisputeStatus = c.DisputeStatus,
        //                                ReceiptFirstName = Receipt.FirstName,
        //                                ReceiptLastName = Receipt.LastName,
        //                                DisputeDate = c.DisputeDate,
        //                                DisputeId = c.DisputeTrackingId,
        //                                TransactionId = c.TransactionTrackingId,
        //                                SenderFirstName = sendr.FirstName,
        //                                SenderLastName = sendr.LastName,
        //                                Amount = c.Amount,
        //                                TransactionDate = c.TransactionDate,
        //                                TransactionLocation = LocationDetail.AddressLine1 + ", " + LocationDetail.AddressLine2 + ", " + LocationDetail.City + ", " + LocationDetail.State,
        //                                SenderImg = sendr.Photo,
        //                                ReceiptImg = Receipt.Photo,
        //                                RaisedBy = c.RaisedBy,
        //                                SenderNoochId = sendr.Nooch_ID,
        //                                ReceiptNoochId = Receipt.Nooch_ID,
        //                                SenderUserName = sendr.UserName,
        //                                ReceiptUserName = Receipt.UserName
        //                            }).SingleOrDefault();


        //        if (DisputeQuery != null)
        //        {
        //            // Dispute found, now update it's status and notify users

        //            var tt = obj.Transactions.First(o => o.DisputeTrackingId == DisputeId);
        //            tt.AdminNotes = AdminNotes;
        //            tt.DisputeStatus = CommonHelper.GetEncryptedData(disputeStatus);
        //            tt.ResolvedDate = DateTime.Now;
        //            tt.AdminName = AdminName.ToString();
        //            int i = obj.SaveChanges();

        //            string sender_FirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.SenderFirstName));
        //            string sender_LastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.SenderLastName));
        //            string recip_FirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.ReceiptFirstName));
        //            string recip_LastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(DisputeQuery.ReceiptLastName));
        //            string dispute_Id = DisputeQuery.DisputeId;
        //            string sender_EmailAddress = CommonHelper.GetDecryptedData(DisputeQuery.SenderUserName);
        //            string recip_EmailAddress = CommonHelper.GetDecryptedData(DisputeQuery.ReceiptUserName);

        //            string amountVal = DisputeQuery.Amount.ToString("n2");

        //            #region DisputeTakenForReview Code
        //            if (disputeStatus.Equals("Under Review"))
        //            {
        //                // If the Disputed Raised By sender
        //                #region IfRaisedBySender
        //                if (DisputeQuery.RaisedBy.Equals("Sender"))
        //                {
        //                    var tokens = new Dictionary<string, string>
        //                        {
        //                            {Constants.PLACEHOLDER_FIRST_NAME, sender_FirstName },
        //                            {Constants.PLACEHOLDER_TRANSACTION_ID,  dispute_Id },
        //                            {Constants.PLACEHOLDER_TRANSFER_AMOUNT, amountVal },
        //                            {Constants.PLACEHOLDER_TRANSACTION_DATE,  Convert.ToDateTime(DisputeQuery.TransactionDate).ToString("MMM dd yyyy") },
        //                            {Constants.PLACEHOLDER_RECIPIENT_FIRST_NAME,  recip_FirstName + " " + recip_LastName }
        //                        };

        //                    try
        //                    {
        //                        Logger.Info("ChangeDisputeStatus - Dispute Taken For Review email sent to: ["
        //                                                            + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

        //                        Utility.SendEmail("DisputeReviewMailTemplate_Sender", fromAddress, sender_EmailAddress,
        //                                "Nooch dispute taken for review", null, tokens, null, null, null);

        //                        ll.IsSuccess = true;
        //                        ll.Message = "Dispute resolved";
        //                    }
        //                    catch (Exception)
        //                    {
        //                        Logger.Error("ChangeDisputeStatus - Dispute Taken For Review email NOT sent to: [" + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

        //                        ll.IsSuccess = false;
        //                        ll.Message = "EmailNotSent";
        //                    }
        //                }

        //                #endregion

        //                string json = JsonConvert.SerializeObject(ll);
        //                return json;
        //            }
        //            #endregion

        //            else if (disputeStatus.Equals("Resolved"))
        //            {
        //                #region Notify Sender
        //                var tokens = new Dictionary<string, string>
        //                        {
        //                                {Constants.PLACEHOLDER_FIRST_NAME, sender_FirstName },
        //                                {Constants.PLACEHOLDER_TRANSACTION_ID,  dispute_Id },
        //                                {Constants.PLACEHOLDER_TRANSFER_AMOUNT, amountVal },
        //                                {Constants.PLACEHOLDER_TRANSACTION_DATE,  Convert.ToDateTime(DisputeQuery.TransactionDate).ToString("MMM dd yyyy") },
        //                                {Constants.PLACEHOLDER_RECIPIENT_FIRST_NAME,  recip_FirstName + " " + recip_LastName }
        //                        };

        //                try
        //                {
        //                    Logger.Info("ChangeDisputeStatus -> Dispute Resolved email sent to:[ "
        //                                                         + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

        //                    Utility.SendEmail("DisputeResolvedMailTemplate_Sender", fromAddress, sender_EmailAddress,
        //                            "Your Nooch Dispute has been resolved", null, tokens, null, null, null);
        //                    ll.IsSuccess = true;
        //                    ll.Message = "Dispute resolved";
        //                }
        //                catch (Exception)
        //                {
        //                    Logger.Error("ChangeDisputeStatus -> Dispute Resolved email NOT sent to: [" + sender_FirstName + " " + sender_LastName + " | " + sender_EmailAddress + "]");

        //                    ll.IsSuccess = false;
        //                    ll.Message = "EmailNotSent";
        //                }
        //                #endregion

        //                #region Notify Recipient
        //                 tokens = new Dictionary<string, string>
        //                        {
        //                                {Constants.PLACEHOLDER_FIRST_NAME, recip_FirstName },
        //                                {Constants.PLACEHOLDER_TRANSACTION_ID,  DisputeQuery.DisputeId },
        //                                {Constants.PLACEHOLDER_TRANSFER_AMOUNT, amountVal },
        //                                {Constants.PLACEHOLDER_TRANSACTION_DATE,  Convert.ToDateTime(DisputeQuery.TransactionDate).ToString("MMM dd yyyy") },
        //                                {Constants.PLACEHOLDER_RECIPIENT_FIRST_NAME,  sender_FirstName + " " + sender_LastName }
        //                        };

        //                try
        //                {
        //                    Utility.SendEmail("DisputeResolvedMailTemplate_Sender", fromAddress, recip_EmailAddress,
        //                            "Nooch Dispute has been resolved", null, tokens, null, null, null);

        //                    ll.IsSuccess = true;
        //                    ll.Message = "SuccessfullyReview";

        //                    Logger.Info("ChangeDisputeStatus -> Dispute Resolved email sent to: ["
        //                                + recip_FirstName + " " + recip_LastName + " | " + recip_EmailAddress + "]");
        //                }
        //                catch (Exception)
        //                {
        //                    Logger.Error("ChangeDisputeStatus - Dispute status mail NOT sent to: [" + recip_FirstName +
        //                                 " " + recip_LastName + " | " + recip_EmailAddress + "]");

        //                    ll.IsSuccess = false;
        //                    ll.Message = "EmailNotSent";
        //                }
        //                #endregion

        //            }
        //            string jsonn = JsonConvert.SerializeObject(ll);
        //            return jsonn;
        //        }

        //        else
        //        {
        //            ll.IsSuccess = false;
        //            ll.Message = "Data Not Found";
        //            string json = JsonConvert.SerializeObject(ll);
        //            return json;
        //        }
        //    }
        //}


        // POST: /Disputes/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: /Disputes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: /Disputes/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: /Disputes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: /Disputes/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}