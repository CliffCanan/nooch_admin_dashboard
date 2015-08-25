using noochAdminNew.Classes.ModelClasses;
using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Models;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using noochAdminNew.Resources;

namespace noochAdminNew.Controllers
{
    public class MemberController : Controller
    {
        [HttpPost]
        [ActionName("ApplyOperation")]
        public ActionResult ApplyOperation(string operation, string noochIds)
        {
            MemberOperationsResult mr = new MemberOperationsResult();

            if (operation == "0")
            {
                mr.IsSuccess = false;
                mr.Message = "No operation selected to perform";
                return Json(mr);
            }

            if (noochIds.Length == 0)
            {
                mr.IsSuccess = false;
                mr.Message = "No user selected to perfrom action";
                return Json(mr);
            }


            try
            {
                noochIds = noochIds.TrimEnd(',');
                List<string> allNoochIdsChoosen = noochIds.Split(',').ToList<string>();

                List<MemberOperationsInnerClass> allinnerclass = new List<MemberOperationsInnerClass>();

                #region Suspend

                if (operation == "1")
                {
                    // Suspend User

                    foreach (string s in allNoochIdsChoosen)
                    {
                        using (NOOCHEntities obj = new NOOCHEntities())
                        {
                            var member = (from t in obj.Members where t.Nooch_ID == s && t.Status != Constants.STATUS_SUSPENDED select t).SingleOrDefault();
                            MemberOperationsInnerClass mic = new MemberOperationsInnerClass();

                            #region IfMemberNotNull
                            if (member != null)
                            {
                                member.Status = Constants.STATUS_SUSPENDED;
                                member.InvalidLoginAttemptCount = 3;
                                member.InvalidPinAttemptCount = null;
                                member.InvalidLoginTime = DateTime.Now;
                                member.InvalidPinAttemptTime = null;
                                member.DateModified = DateTime.Now;
                                member.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());
                                obj.SaveChanges();

                                // sending email to member
                                var fromAddress = Utility.GetValueFromConfig("adminMail");
                                string emailAddress = CommonHelper.GetDecryptedData(member.UserName);

                                var tokens = new Dictionary<string, string>
                                {
                                    {Constants.PLACEHOLDER_FIRST_NAME,CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(member.FirstName))}
                                };

                                try
                                {
                                    Utility.SendEmail("userSuspended", fromAddress, emailAddress,
                                        "Your Nooch account has been suspended", null, tokens, null, null, null);

                                    Logger.Info("Admin Dash -> MemberController - SupendMember email sent to: [" + emailAddress + "]; Member [memberId:" +
                                                member.MemberId + "]");

                                    mic.Message = "Member Suspended Successfully";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = true;
                                    allinnerclass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("Admin Dash -> MemberController - SupendMember email NOT send to: [" + emailAddress + "]; Member [memberId:" +
                                                member.MemberId + "]. Problem occured in sending mail.");

                                    mic.Message = "Member Suspension Failed";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    allinnerclass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Member already Suspended";
                                mic.NoochId = s;
                                allinnerclass.Add(mic);
                            }
                            #endregion

                        }
                    }
                    mr.IsSuccess = true;
                    mr.Message = "all operations performed";
                    mr.MemberOperationsOuterClass = allinnerclass;
                }

                #endregion Suspend


                #region SDN Safe

                if (operation == "2")
                {
                    // Mark User as SDN Safe

                    foreach (string s in allNoochIdsChoosen)
                    {
                        using (NOOCHEntities obj = new NOOCHEntities())
                        {
                            var member = (from t in obj.Members where t.Nooch_ID == s && t.IsSDNSafe == null || t.IsSDNSafe == false select t).SingleOrDefault();
                            MemberOperationsInnerClass mic = new MemberOperationsInnerClass();
                            
                            #region IfMemberNotNull
                            
                            if (member != null)
                            {
                                try
                                {
                                    member.IsSDNSafe = true;
                                    member.DateModified = DateTime.Now;
                                    member.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());
                                    obj.SaveChanges();

                                    Logger.Error("Admin Dash -> Members Controller - Mark SDN Safe - MemberId: [" +
                                                member.MemberId + "]");

                                    mic.Message = "Member marked SDN safe Successfully. ";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = true;
                                    allinnerclass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("Admin Dash -> Members Controller - Mark SDN Safe FAILED - MemberId: [" +
                                                member.MemberId + "]");

                                    mic.Message = "Member SDN mark safe failed!";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    allinnerclass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Member already SDN Safe ";
                                mic.NoochId = s;
                                allinnerclass.Add(mic);
                            }
                            #endregion

                        }
                    }
                    mr.IsSuccess = true;
                    mr.Message = "all operations performed";
                    mr.MemberOperationsOuterClass = allinnerclass;
                }

                #endregion SDN Safe


                #region verify phone

                if (operation == "3")
                {
                    // Verify User's Phone Number

                    foreach (string s in allNoochIdsChoosen)
                    {
                        using (NOOCHEntities obj = new NOOCHEntities())
                        {
                            var member = (from t in obj.Members where t.Nooch_ID == s && t.ContactNumber != null select t).SingleOrDefault();
                            MemberOperationsInnerClass mic = new MemberOperationsInnerClass();
                            #region IfMemberNotNull
                            if (member != null)
                            {
                                try
                                {
                                    member.IsVerifiedPhone = true;
                                    member.DateModified = DateTime.Now;
                                    member.PhoneVerifiedOn = DateTime.Now;
                                    member.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());
                                    obj.SaveChanges();

                                    Logger.Error("VerifyPhone - Attempt to verify member phone[ memberId:" +
                                                member.MemberId + "].");

                                    mic.Message = "Contact no. verified Successfully. ";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = true;
                                    allinnerclass.Add(mic);

                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("VerifyPhone - Attempt to verify member phone failed [" + member.MemberId +
                                                           "]. Problem occured in setting status true. ");

                                    mic.Message = "Verify Phone Failed unfortunately :-(";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    allinnerclass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Contact no. not available ";
                                mic.NoochId = s;
                                allinnerclass.Add(mic);
                            }
                            #endregion

                        }
                    }
                    mr.IsSuccess = true;
                    mr.Message = "all operations performed";
                    mr.MemberOperationsOuterClass = allinnerclass;
                }

                #endregion verify phone


                #region activate account - verify email

                if (operation == "4")
                {
                    // Activate User

                    foreach (string s in allNoochIdsChoosen)
                    {
                        using (NOOCHEntities obj = new NOOCHEntities())
                        {
                            var member = (from t in obj.Members where t.Nooch_ID == s && t.Status != Constants.STATUS_ACTIVE select t).SingleOrDefault();
                            MemberOperationsInnerClass mic = new MemberOperationsInnerClass();
                            #region IfMemberNotNull
                            if (member != null)
                            {
                                try
                                {
                                    member.Status = Constants.STATUS_ACTIVE;
                                    member.InvalidLoginAttemptCount = 0;
                                    member.InvalidPinAttemptCount = null;
                                    member.InvalidLoginTime = null;
                                    member.InvalidPinAttemptTime = null;
                                    member.DateModified = DateTime.Now;
                                    member.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());

                                    obj.SaveChanges();

                                    Logger.Error("ActivateAccount - Attempt to ActivateAccount member [ memberId:" +
                                                member.MemberId + "].");

                                    mic.Message = "Member activated Successfully. ";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = true;
                                    allinnerclass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("ActivateAccount - Attempt to ActivateAccount Member failed [" + member.MemberId +
                                                           "]. Problem occured in setting status Active. ");

                                    mic.Message = "Member activation Failed!";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    allinnerclass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Member already Active ";
                                mic.NoochId = s;
                                allinnerclass.Add(mic);
                            }
                            #endregion

                        }
                    }
                    mr.IsSuccess = true;
                    mr.Message = "all operations performed";
                    mr.MemberOperationsOuterClass = allinnerclass;
                }

                #endregion activate account - verify email


                #region Delete User

                if (operation == "5")
                {
                    // Delete User

                    foreach (string s in allNoochIdsChoosen)
                    {
                        using (NOOCHEntities obj = new NOOCHEntities())
                        {
                            var member = (from t in obj.Members where t.Nooch_ID == s && t.IsDeleted == false select t).SingleOrDefault();
                            MemberOperationsInnerClass mic = new MemberOperationsInnerClass();
                            #region IfMemberNotNull
                            if (member != null)
                            {
                                try
                                {
                                    member.Status = "Deleted";
                                    member.DateModified = DateTime.Now;
                                    Guid dd = Utility.ConvertToGuid(Session["UserId"].ToString());
                                    member.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());
                                    member.IsDeleted = true;
                                    obj.SaveChanges();

                                    Logger.Error("DeleteMembers - Attempt to Delete member [ memberId:" +
                                                member.MemberId + "].");

                                    mic.Message = "Member Deleted Successfully. ";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = true;
                                    allinnerclass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("DeleteMembers - Attempt to Delete member failed [" + member.MemberId +
                                                           "]. Problem occured in setting status true. ");

                                    mic.Message = "Member Deletion failed!";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    allinnerclass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Member already Deleted";
                                mic.NoochId = s;
                                allinnerclass.Add(mic);
                            }
                            #endregion

                        }
                    }
                    mr.IsSuccess = true;
                    mr.Message = "all operations performed";
                    mr.MemberOperationsOuterClass = allinnerclass;
                }

                #endregion Delete Usr


                return Json(mr);
            }
            catch (Exception ex)
            {
                mr.IsSuccess = false;
                mr.Message = "Error";

                return Json(mr);
            }
        }

        public void CheckSession()
        {
            if (Session["UserId"] == null)
            {
                RedirectToAction("Index", "Home");
            }
        }


        private List<MembersListDataClass> GetAllMembers()
        {
            List<MembersListDataClass> AllMemberFormtted = new List<MembersListDataClass>();
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var All_Members_In_Records = (from t in obj.Members
                                              // where t.IsDeleted == false  // CLIFF: Commenting this line out so this returns ALL member records
                                              select t).ToList();

                foreach (Member m in All_Members_In_Records)
                {
                    int TransCount = (from tr in obj.Transactions
                                      where (tr.Member.MemberId == m.MemberId || tr.Member1.MemberId == m.MemberId) &&
                                             tr.TransactionStatus == "Success"
                                      select tr).Count();

                    // transaction - Transfer Type - 5dt4HUwCue532sNmw3LKDQ==
                    // invite - DrRr1tU1usk7nNibjtcZkA==
                    // request - T3EMY1WWZ9IscHIj3dbcNw==

                    var sumofTransfers = (from tr in obj.Transactions
                                          where tr.TransactionStatus == "Success" && tr.TransactionType == "5dt4HUwCue532sNmw3LKDQ=="
                                                && tr.Member.MemberId == m.MemberId
                                          select tr.Amount
                        ).Sum(tr => (decimal?)tr) ?? 0;

                    var sumOfInvitations = (from tr in obj.Transactions
                                            where tr.TransactionStatus == "Success" && tr.TransactionType == "DrRr1tU1usk7nNibjtcZkA=="
                                                  && tr.Member1.MemberId == m.MemberId
                                            select tr.Amount
                        ).Sum(tr => (decimal?)tr) ?? 0;

                    var sumOfRequests = (from tr in obj.Transactions
                                         where tr.TransactionStatus == "Success" && tr.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw=="
                                               && tr.Member.MemberId == m.MemberId
                                         select tr.Amount
                        ).Sum(tr => (decimal?)tr) ?? 0;

                    var totalAmount = sumOfInvitations + sumOfRequests + sumofTransfers;

                    MembersListDataClass mdc = new MembersListDataClass();
                    mdc.Nooch_ID = m.Nooch_ID;
                    mdc.FirstName = !String.IsNullOrEmpty(m.FirstName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(m.FirstName)) : "";
                    mdc.LastName = !String.IsNullOrEmpty(m.LastName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(m.LastName)) : "";
                    mdc.UserName = !String.IsNullOrEmpty(m.UserName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(m.UserName)) : "";

                    if (m.ContactNumber != null)
                    {
                        if (m.ContactNumber.Length == 10)
                        {
                            mdc.ContactNumber = "(" + m.ContactNumber;
                            mdc.ContactNumber = mdc.ContactNumber.Insert(4, ")");
                            mdc.ContactNumber = mdc.ContactNumber.Insert(5, " ");
                            mdc.ContactNumber = mdc.ContactNumber.Insert(9, "-");
                        }
                        else
                        {
                            mdc.ContactNumber = m.ContactNumber;
                        }
                    }
                    else
                    {
                        mdc.ContactNumber = m.ContactNumber;
                    }

                    mdc.Status = m.Status;
                    mdc.IsDeleted = m.IsDeleted ?? false;
                    mdc.IsVerifiedPhone = m.IsVerifiedPhone ?? false;
                    mdc.City = !String.IsNullOrEmpty(m.City) ? CommonHelper.GetDecryptedData(m.City) : "";

                    mdc.TotalAmountSent = mdc.TotalAmountSent != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(totalAmount))).ToString() : "0";
                    mdc.DateCreated = Convert.ToDateTime(m.DateCreated);

                    mdc.TotalTransactions = TransCount;
                    AllMemberFormtted.Add(mdc);
                }
            }
            return AllMemberFormtted;

        }


        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ListAll()
        {
            List<MembersListDataClass> AllMemberFormtted = GetAllMembers();

            return View(AllMemberFormtted);
        }


        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult Detail(string NoochId)
        {
            CheckSession();

            MemberDetailsClass mdc = new MemberDetailsClass();
            // getting details of noochid passed
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var Member = (from m in obj.Members where m.Nooch_ID == NoochId select m).SingleOrDefault();

                if (Member != null)
                {
                    mdc.DateCreated = Convert.ToDateTime(Member.DateCreated);
                    mdc.DateCreatedFormatted = String.Format("{0: MMMM d, yyyy}", Member.DateCreated);
                    mdc.FBID = !String.IsNullOrEmpty(Member.FacebookAccountLogin) ? CommonHelper.GetDecryptedData(Member.FacebookAccountLogin) : "";
                    mdc.FirstName = !String.IsNullOrEmpty(Member.FirstName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.FirstName)) : "";
                    mdc.LastName = !String.IsNullOrEmpty(Member.LastName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.LastName)) : "";
                    mdc.UserName = !String.IsNullOrEmpty(Member.UserName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.UserName)) : "";
                    mdc.SecondaryEmail = Member.SecondaryEmail;
                    mdc.RecoveryEmail = !String.IsNullOrEmpty(Member.RecoveryEmail) ? CommonHelper.GetDecryptedData(Member.RecoveryEmail) : "";
                    mdc.ContactNumber = !String.IsNullOrEmpty(Member.ContactNumber) ? CommonHelper.FormatPhoneNumber(Member.ContactNumber) : "";
                    mdc.ImageURL = Member.Photo;
                    mdc.Address = !String.IsNullOrEmpty(Member.Address) ? CommonHelper.GetDecryptedData(Member.Address) : "";
                    mdc.City = !String.IsNullOrEmpty(Member.City) ? CommonHelper.GetDecryptedData(Member.City) : "";
                    mdc.State = !String.IsNullOrEmpty(Member.State) ? CommonHelper.GetDecryptedData(Member.State) : "";
                    mdc.Zipcode = !String.IsNullOrEmpty(Member.Zipcode) ? CommonHelper.GetDecryptedData(Member.Zipcode) : "";
                    mdc.Status = Member.Status;
                    mdc.PinNumber = !String.IsNullOrEmpty(Member.PinNumber) ? CommonHelper.GetDecryptedData(Member.PinNumber) : "";
                    mdc.IsPhoneVerified = Member.IsVerifiedPhone ?? false;
                    mdc.Nooch_ID = NoochId;
                    mdc.dob = Convert.ToDateTime(Member.DateOfBirth).ToString("M/d/yyyy");
                    mdc.ssn = !String.IsNullOrEmpty(Member.SSN) ? CommonHelper.GetDecryptedData(Member.SSN) : "";
                    mdc.idDocUrl = Member.VerificationDocumentPath;
                    mdc.adminNote = Member.AdminNotes;

                    //Get the Refered Code Used
                    mdc.ReferCodeUsed = (from Membr in obj.Members join Code in obj.InviteCodes on Membr.InviteCodeIdUsed equals Code.InviteCodeId where Membr.Nooch_ID == NoochId select Code.code).SingleOrDefault();

                    var MemberKnoxDetails = (from m in obj.KnoxAccountDetails where m.Member.MemberId == Member.MemberId && m.IsDeleted == false select m).FirstOrDefault();
                    mdc.IsKnocAvailable = (MemberKnoxDetails != null);
                    if (mdc.IsKnocAvailable)
                    {
                        mdc.KnoxBankIcon = MemberKnoxDetails.BankImageURL;
                        mdc.KnoxTransId = CommonHelper.GetDecryptedData(MemberKnoxDetails.TransId);
                    }


                    #region Get User's Transactions

                    var transtp = (from t in obj.Transactions
                                   where
                                       t.Member.MemberId == Member.MemberId && (t.TransactionType == "5dt4HUwCue532sNmw3LKDQ==" || t.TransactionType == "+C1+zhVafHdXQXCIqjU/Zg==" || t.TransactionType == "DrRr1tU1usk7nNibjtcZkA==") &&
                            (t.TransactionStatus == "Success" || t.TransactionStatus == "Rejected" || t.TransactionStatus == "Pending" || t.TransactionStatus == "Cancelled")
                                   select t).OrderByDescending(r => r.TransactionDate).Take(10).ToList();

                    List<MemberDetailsTrans> mm = new List<MemberDetailsTrans>();
                    foreach (Transaction t in transtp)
                    {
                        MemberDetailsTrans merc = new MemberDetailsTrans();
                        //merc.Amount = t.Amount != 0 ? t.Amount.ToString("C", CultureInfo.CurrentCulture) : "0";
                        merc.AmountNew = t.Amount.ToString();
                        merc.TransID = t.TransactionTrackingId.ToString();
                        merc.RecepientId = t.Member1.Nooch_ID.ToString();
                        merc.TransDateTime = Convert.ToDateTime(t.TransactionDate).ToString();
                        merc.RecepientUserName = CommonHelper.GetDecryptedData(t.Member1.UserName.ToString());

                        merc.GeoLocation = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.City + " , " + Geo_loc.State).SingleOrDefault();
                        merc.Longitude = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.Longitude).SingleOrDefault().ToString();
                        merc.Latitude = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.Latitude).SingleOrDefault().ToString();

                        merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
                        merc.TransactionStatus = t.TransactionStatus;
                        mm.Add(merc);
                    }

                    #endregion Get User's Transactions


                    #region Get Last 5 Disputes

                    var disputeTrans = (from t in obj.Transactions
                                        where
                                            (t.Member.MemberId == Member.MemberId || t.Member1.MemberId == Member.MemberId)
                                            && (t.DisputeStatus != null) &&
                             (t.DisputeTrackingId != null)
                                        select t).OrderByDescending(r => r.TransactionDate).Take(10).ToList();


                    List<MemberDetailsDisputeTrans> mm2 = new List<MemberDetailsDisputeTrans>();
                    foreach (Transaction t in disputeTrans)
                    {
                        MemberDetailsDisputeTrans merc = new MemberDetailsDisputeTrans();
                        merc.Dispute_ID = t.DisputeTrackingId.ToString();
                        merc.Transaction_ID = t.TransactionTrackingId.ToString();
                        merc.Dispute_Date = t.DisputeDate.ToString();
                        merc.Status = CommonHelper.GetDecryptedData(t.DisputeStatus);
                        merc.Admin_Notes = t.AdminNotes;
                        merc.Resolved_Date = Convert.ToDateTime(t.ResolvedDate).ToString();
                        merc.Review_Date = Convert.ToDateTime(t.ReviewDate).ToString();
                        merc.Subject = t.Subject;

                        merc.RecepientId = t.Member1.Nooch_ID.ToString();
                        merc.TransDateTime = Convert.ToDateTime(t.TransactionDate).ToString();
                        merc.RecepientUserName = CommonHelper.GetDecryptedData(t.Member1.UserName.ToString());

                        merc.GeoLocation = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.City + " , " + Geo_loc.State).SingleOrDefault();
                        merc.Longitude = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.Longitude).SingleOrDefault().ToString();
                        merc.Latitude = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.Latitude).SingleOrDefault().ToString();

                        merc.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
                        merc.TransactionStatus = t.TransactionStatus;

                        mm2.Add(merc);
                    }

                    mdc.Transactions = mm;
                    mdc.DisputeTransactions = mm2;

                    #endregion Get Last 5 Disputes


                    #region Stats-Related Operations

                    MemberDetailsStats ms = new MemberDetailsStats();

                    ms.TotalTransfer = obj.GetReportsForMember(Member.MemberId.ToString(), "Total_P2P_transfers").SingleOrDefault();

                    string TotalSent = obj.GetReportsForMember(Member.MemberId.ToString(), "Total_$_Sent").SingleOrDefault();
                    ms.TotalSent = TotalSent != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(TotalSent))).ToString() : "0";

                    string TotalReceived = obj.GetReportsForMember(Member.MemberId.ToString(), "Total_$_Received").SingleOrDefault();
                    ms.TotalReceived = TotalReceived != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(TotalReceived))).ToString() : "0";

                    string LargestSent = obj.GetReportsForMember(Member.MemberId.ToString(), "Largest_sent_transfer").SingleOrDefault();
                    ms.LargestSent = LargestSent != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(LargestSent))).ToString() : "0";

                    mdc.MemberStats = ms;

                    #endregion Stats-Related Operations

                    // Calculate number of weeks since the user created an account
                    double no_Of_Since_Joined = (DateTime.Now - Convert.ToDateTime(Member.DateCreated)).TotalDays / 7;
                    mdc.WeeksSinceJoined = Convert.ToInt16(no_Of_Since_Joined);


                    // Check whether Synapse Bank details available or not
                    var synapse = (from Syn in obj.SynapseBanksOfMembers
                                   join mem in obj.Members on Syn.MemberId equals mem.MemberId
                                   where Syn.IsDefault == true && mem.Nooch_ID == NoochId
                                   select Syn).FirstOrDefault();

                    mdc.IsSynapseDetailAvailable = (synapse != null);

                    if (mdc.IsSynapseDetailAvailable)
                    {
                        // Get the user's Synapse account details 
                        var synapseDetailFromDb = (from Syn in obj.SynapseBanksOfMembers
                                                   join mem in obj.Members on Syn.MemberId equals mem.MemberId
                                                   where Syn.IsDefault == true && mem.Nooch_ID == NoochId
                                                   select Syn).FirstOrDefault();

                        SynapseDetailOFMember synapseDetail = new SynapseDetailOFMember();

                        if (synapseDetailFromDb != null)
                        {
                            synapseDetail.synapseConsumerKey = "";
                            synapseDetail.synapseBankId = synapseDetailFromDb.bankid; // This is the 4 or 5 digit ID from Synapse for this account
                            synapseDetail.BankId = synapseDetailFromDb.Id; // This is the Nooch DB "ID", which is just the row number of the account... NOT the same as the Synapse Bank ID
                            synapseDetail.SynapseBankStatus = (string.IsNullOrEmpty(synapseDetailFromDb.Status)
                                ? "Not Verified" : synapseDetailFromDb.Status);
                            synapseDetail.SynapseBankNickName = !String.IsNullOrEmpty(synapseDetailFromDb.nickname) ? CommonHelper.GetDecryptedData(synapseDetailFromDb.nickname) : "";
                            synapseDetail.nameFromSynapseBank = !String.IsNullOrEmpty(synapseDetailFromDb.name_on_account) ? CommonHelper.GetDecryptedData(synapseDetailFromDb.name_on_account) : " No Name Returned";
                            synapseDetail.emailFromSynapseBank = !String.IsNullOrEmpty(synapseDetailFromDb.email) ? synapseDetailFromDb.email : "No Email Returned";
                            synapseDetail.phoneFromSynapseBank = !String.IsNullOrEmpty(synapseDetailFromDb.phone_number) ? CommonHelper.FormatPhoneNumber(synapseDetailFromDb.phone_number) : "No Phone Returned";
                            synapseDetail.SynpaseBankAddedOn = synapseDetailFromDb.AddedOn != null
                                ? Convert.ToDateTime(synapseDetailFromDb.AddedOn).ToString("MMM dd, yyyy") : "";
                            synapseDetail.SynpaseBankVerifiedOn = synapseDetailFromDb.VerifiedOn != null
                                ? Convert.ToDateTime(synapseDetailFromDb.VerifiedOn).ToString("MMM dd, yyyy") : "";

                            synapseDetail.SynapseBankName = !String.IsNullOrEmpty(synapseDetailFromDb.bank_name) ? CommonHelper.GetDecryptedData(synapseDetailFromDb.bank_name) : "Not Found";
                        }
                        mdc.SynapseDetails = synapseDetail;
                    }


                    // Get the 3 most recent IP Addresses for this user
                    mdc.MemberIpAddr = (from memIpADD in obj.MembersIPAddresses
                                        join mem in obj.Members on
                                             memIpADD.MemberId equals mem.MemberId
                                        where mem.Nooch_ID == NoochId
                                        select new MemberIpAddrreses
                                        {
                                            IpAddress = memIpADD.Ip,
                                            Date = (DateTime)memIpADD.ModifiedOn
                                        }
                                          ).OrderByDescending(r => r.Date).Take(5).ToList();


                    #region Get any members referred by this user

                    if (Member.InviteCodeId != null)
                    {
                        Guid g = Utility.ConvertToGuid(Member.InviteCodeId.ToString());
                        var allreferrals =
                            (from c in obj.Members where c.InviteCodeIdUsed == g && c.IsDeleted == false select c)
                                .Take(5).OrderByDescending(m => m.DateCreated).ToList();
                        List<ReferredMembers> memreferred = new List<ReferredMembers>();
                        if (allreferrals.Count > 0)
                        {
                            foreach (Member m in allreferrals)
                            {
                                ReferredMembers rm = new ReferredMembers();
                                rm.MemberName = CommonHelper.GetDecryptedData(m.FirstName);
                                rm.ImageUrl = m.Photo;
                                memreferred.Add(rm);
                            }
                        }

                        mdc.Referrals = memreferred;
                    }
                    #endregion Get any members referred by this user

                }
            }
            return View(mdc);
        }

        /// <summary>
        /// Updates a member's Nooch account details.
        /// </summary>
        /// <param name="contactno"></param>
        /// <param name="streetaddress"></param>
        /// <param name="city"></param>
        /// <param name="secondaryemail"></param>
        /// <param name="recoveryemail"></param>
        /// <param name="noochid"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        [HttpPost]
        [ActionName("EditMemberDetails")]
        public ActionResult EditMemberDetails(string contactno, string streetaddress, string city, string secondaryemail, string recoveryemail, string noochid, string state, string zip)
        {
            MemberEditResultClass re = new MemberEditResultClass();
            if (String.IsNullOrEmpty(noochid))
            {
                re.IsSuccess = false;
                re.Message = "Invalid nooch id passed";
            }
            else
            {
                // Get member from DB
                using (NOOCHEntities obj = new NOOCHEntities())
                {
                    var member =
                        (from t in obj.Members where t.Nooch_ID == noochid && t.IsDeleted == false select t)
                            .SingleOrDefault();

                    if (member == null)
                    {
                        re.IsSuccess = false;
                        re.Message = "Member not found";
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(contactno))
                        {
                            member.ContactNumber = contactno.Trim();
                        }

                        if (!String.IsNullOrEmpty(zip))
                        {
                            member.Zipcode = CommonHelper.GetEncryptedData(zip.Trim());
                        }
                        if (!String.IsNullOrEmpty(state))
                        {
                            member.State = CommonHelper.GetEncryptedData(state.Trim());
                        }

                        if (!String.IsNullOrEmpty(streetaddress))
                        {
                            member.Address = CommonHelper.GetEncryptedData(streetaddress.Trim());
                        }

                        if (!String.IsNullOrEmpty(city))
                        {
                            member.City = CommonHelper.GetEncryptedData(city.Trim());
                        }

                        if (!String.IsNullOrEmpty(secondaryemail))
                        {
                            member.SecondaryEmail = secondaryemail.Trim();
                        }

                        if (!String.IsNullOrEmpty(recoveryemail))
                        {
                            member.RecoveryEmail = CommonHelper.GetEncryptedData(recoveryemail.Trim());
                        }
                        re.contactnum = contactno;
                        re.Address = streetaddress;
                        re.secondaryemail = secondaryemail;
                        re.recoveryemail = recoveryemail;
                        re.contactnum = contactno;
                        re.City = city;
                        re.state = state;
                        re.zip = zip;

                        obj.SaveChanges();
                        re.IsSuccess = true;
                        re.Message = "Member record updated successfully";
                    }
                }
            }
            return Json(re);
        }


        public string GetRandomPinNumber()
        {
            const string chars = "0123456789";
            var random = new Random();
            var randomId = new string(
                Enumerable.Repeat(chars, 4)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return randomId;
        }


        [HttpPost]
        [ActionName("ResetPin")]
        public ActionResult ResetPin(string noochId)
        {
            Logger.Info("Admin Dash -> ResetPin - [noochId:" + noochId + "]");

            LoginResult lr = new LoginResult();

            using (var noochConnection = new NOOCHEntities())
            {
                // getting member from db
                var mem =
                    (from c in noochConnection.Members where c.Nooch_ID == noochId && c.IsDeleted == false select c)
                        .SingleOrDefault();

                if (mem != null)
                {
                    #region if member found

                    Guid id = Utility.ConvertToGuid(mem.MemberId.ToString());
                    var admin = Utility.ConvertToGuid(Session["UserId"].ToString());

                    string randomPinNumber = GetRandomPinNumber();
                    string encryptedPinNumber = CommonHelper.GetEncryptedData(randomPinNumber);
                    var fromAddress = Utility.GetValueFromConfig("adminMail");
                    string emailAddress = CommonHelper.GetDecryptedData(mem.UserName);

                    mem.PinNumber = encryptedPinNumber;
                    mem.DateModified = DateTime.Now;
                    mem.ModifiedBy = admin;

                    var tokens = new Dictionary<string, string>
                    {
                        {
                            Constants.PLACEHOLDER_FIRST_NAME,
                            CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(mem.FirstName))
                        },
                        {Constants.PLACEHOLDER_PINNUMBER, randomPinNumber}
                    };


                    int result = noochConnection.SaveChanges();

                    if (result > 0)
                    {
                        try
                        {
                            Utility.SendEmail("PinNumberMailTemplate",
                                fromAddress, emailAddress,
                                "Your Nooch PIN has been reset.", null, tokens,
                                null, null, null);
                            lr.IsSuccess = true;
                            lr.Message = "Pin Number updated successully.";
                            lr.Pin = randomPinNumber;
                        }
                        catch (Exception)
                        {
                            Logger.Error("NoochNewAdmin - ResetPin[  PIN number email not send to [" + emailAddress +
                                         "]. Problem occured in sending PIN number mail. ");
                            lr.IsSuccess = false;
                            lr.Message = "Error occuerred on server, please retry.";
                        }
                    }
                    else
                    {
                        lr.IsSuccess = false;
                        lr.Message = "Error occuerred on server, please retry.";
                    }

                    #endregion
                }
                else
                {
                    lr.IsSuccess = false;
                    lr.Message = "Member not found";
                }
            }
            return Json(lr);
        }


        [HttpPost]
        [ActionName("VerifyAccount")]
        public ActionResult VerifyAccount(string accountId)
        {
            Logger.Info("Admin Dash -> VerifyAccount - [Synapse Bank ID: " + accountId + "]");

            LoginResult lr = new LoginResult();
            int bnkid = Convert.ToInt16(accountId);

            using (var noochConnection = new NOOCHEntities())
            {
                // Get Synapse Bank info from DB
                var bank = (from c in noochConnection.SynapseBanksOfMembers 
                            where c.Id == bnkid select c)
                            .SingleOrDefault();

                if (bank != null)
                {
                    if (bank.Status == "Verified")
                    {
                        lr.IsSuccess = false;
                        lr.Message = "Bank account already verified.";
                    }
                    else
                    {
                        bank.Status = "Verified";
                        bank.VerifiedOn = DateTime.Now;

                        int result = noochConnection.SaveChanges();

                        if (result > 0)
                        {
                            lr.IsSuccess = true;
                            lr.Message = "Bank account verified successfully.";

                            try
                            {
                                var memberId = bank.MemberId;
                                var BankName = CommonHelper.GetDecryptedData(bank.bank_name);
                                var bankNickName = CommonHelper.GetDecryptedData(bank.nickname);

                                #region Set Bank Logo URL Variable

                                string appPath = "https://www.noochme.com/noochweb/";
                                var bankLogoUrl = "";

                                switch (BankName)
                                {
                                    case "Ally":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/ally.png");
                                        }
                                        break;
                                    case "Bank of America":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/bankofamerica.png");
                                        }
                                        break;
                                    case "Wells Fargo":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/WellsFargo.png");
                                        }
                                        break;
                                    case "Chase":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/chase.png");
                                        }
                                        break;
                                    case "Citibank":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/citibank.png");
                                        }
                                        break;
                                    case "TD Bank":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/td.png");
                                        }
                                        break;
                                    case "Capital One 360":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/capone360.png");
                                        }
                                        break;
                                    case "US Bank":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/usbank.png");
                                        }
                                        break;
                                    case "PNC":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/pnc.png");
                                        }
                                        break;
                                    case "SunTrust":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/suntrust.png");
                                        }
                                        break;
                                    case "USAA":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/usaa.png");
                                        }
                                        break;

                                    case "First Tennessee":
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/firsttennessee.png");
                                        }
                                        break;
                                    default:
                                        {
                                            bankLogoUrl = String.Concat(appPath, "Assets/Images/bankPictures/no.png");
                                        }
                                        break;
                                }
                                #endregion Set Bank Logo URL Variable

                                // Get Member Info from DB
                                var noochMember = (from c in noochConnection.Members
                                            where c.MemberId.Equals(memberId) && c.IsDeleted == false
                                            select c)
                                            .FirstOrDefault();

                                if (noochMember != null)
                                {
                                    var toAddress = noochMember.UserName.ToLower();
                                    var fromAddress = Utility.GetValueFromConfig("adminMail");

                                    var firstNameForEmail = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(noochMember.FirstName));
                                    var fullName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(noochMember.FirstName)) + " " +
                                                   CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(noochMember.LastName));

                                    var tokens = new Dictionary<string, string>
                                            {
                                                {"$FirstName$", firstNameForEmail},
                                                {"$BankName$", BankName},
                                                {"$RecipientFullName$", fullName},
                                                {"$Recipient$", bankNickName},
                                                {"$Amount$", bankLogoUrl},
                                            };

                                    Utility.SendEmail("bankVerified", fromAddress, toAddress,
                                        "Your bank account has been verified on Nooch", null, tokens,
                                        null, null, null);

                                    Logger.Info("Admin Dash -> Member Controller - Verify Bank Account - bankVerified email sent successfully to: [" + toAddress + "]");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("Admin Dash -> Member Controller - Verify Bank Account - bankVerified email NOT sent successfully for BankID: ["
                                             + accountId + "],  [Exception: " + ex + "]");
                            }

                        }
                        else
                        {
                            lr.IsSuccess = false;
                            lr.Message = "Error occurred on server, please retry.";
                        }
                    }
                }
                else
                {
                    lr.IsSuccess = false;
                    lr.Message = "Bank account not found";
                }
            }
            return Json(lr);
        }


        [HttpPost]
        [ActionName("UnVerifyAccount")]
        public ActionResult UnVerifyAccount(string accountId)
        {
            Logger.Info("Admin Dash -> UnVerifyAccount - [Synapse Bank ID: " + accountId + "]");

            LoginResult lr = new LoginResult();
            int bnkid = Convert.ToInt16(accountId);

            using (var noochConnection = new NOOCHEntities())
            {
                // Get Synapse Bank info from DB
                var bank = (from c in noochConnection.SynapseBanksOfMembers
                            where c.Id == bnkid
                            select c)
                            .SingleOrDefault();

                if (bank != null)
                {
                    if (bank.Status != "Verified")
                    {
                        lr.IsSuccess = false;
                        lr.Message = "Bank account not yet verified.";
                    }
                    else
                    {
                        bank.Status = "Pending Review";

                        int result = noochConnection.SaveChanges();

                        if (result > 0)
                        {
                            lr.IsSuccess = true;
                            lr.Message = "Bank account un-verified successfully.";
                        }
                        else
                        {
                            lr.IsSuccess = false;
                            lr.Message = "Error occuerred on server, please retry.";
                        }
                    }
                }

                else
                {
                    lr.IsSuccess = false;
                    lr.Message = "Bank account not found";
                }
            }
            return Json(lr);
        }


        [HttpPost]
        [ActionName("AdminNoteAboutUserModalPopup")]
        public ActionResult AdminNoteAboutUserModalPopup(string noochId)
        {
            AdminNoteResult lr = new AdminNoteResult();

            try
            {
                using (var noochConnection = new NOOCHEntities())
                {
                    // Get Member from DB
                    var mem =
                        (from c in noochConnection.Members where c.Nooch_ID == noochId && c.IsDeleted == false select c)
                            .SingleOrDefault();

                    if (mem != null)
                    {
                        #region if member found
                        string adminnote = mem.AdminNotes;
                        lr.IsSuccess = true;
                        lr.Message = "Success";
                        lr.AdminNote = adminnote;

                        return Json(lr);
                        #endregion
                    }
                    else
                    {
                        lr.IsSuccess = false;
                        lr.Message = "Member not found";
                        return Json(lr);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Admin Dash -> AdminNoteAboutUserModalPopup FAILED - [Nooch_Id: " + noochId + "]. Exception: " + ex + "]");

                lr.IsSuccess = false;
                lr.Message = "Error";

                return Json(lr);
            }
        }


        [HttpPost]
        [ActionName("SaveAdminNoteForUser")]
        public string SaveAdminNoteForUser(string noochId, string AdminNote)
        {
            try
            {
                using (var noochConnection = new NOOCHEntities())
                {
                    // getting member from db
                    var mem = noochConnection.Members.FirstOrDefault(c => c.Nooch_ID == noochId && c.IsDeleted == false);

                    mem.AdminNotes = AdminNote;
                    int i = noochConnection.SaveChanges();
                    return "Success";
                }
            }

            catch (Exception ex)
            {
                Logger.Info("Admin Dash -> SaveAdminNoteForUser FAILED - [Nooch_Id: " + noochId + "]. Exception: " + ex + "]");

                return "Fail";
            }
        }
    }
}