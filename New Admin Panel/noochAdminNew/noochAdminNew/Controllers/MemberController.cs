using noochAdminNew.Classes.ModelClasses;
using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Models;
using System;
using System.Collections.Generic;
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
                                    Logger.Info("SupendMember - Attempt to send mail for Supend Member[ memberId:" +
                                                member.MemberId + "].");
                                    Utility.SendEmail("userSuspended", fromAddress, emailAddress,
                                        "Your Nooch account has been suspended", null, tokens, null, null, null);
                                    mic.Message = "Member Suspended Successfully";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = true;
                                    allinnerclass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("SupendMember --> Email not send to [" + member.MemberId +
                                                           "]. Problem occured in sending Supend Member status mail. ");

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

                                    Logger.Error("MarkSDNSAFE - Attempt to mark member sdn safe[ memberId:" +
                                                member.MemberId + "].");

                                    mic.Message = "Member marked SDN safe Successfully. ";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = true;
                                    allinnerclass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("MarkSDNSAFE - Attempt to mark member sdn safe failed [" + member.MemberId +
                                                           "]. Problem occured in setting status true. ");

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

                                    mic.Message = "Member SDN mark safe Failed !! ";
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
                    mdc.FirstName = CommonHelper.GetDecryptedData(m.FirstName);
                    mdc.LastName = CommonHelper.GetDecryptedData(m.LastName);
                    mdc.UserName = CommonHelper.GetDecryptedData(m.UserName);

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
                    mdc.City = CommonHelper.GetDecryptedData(m.City);

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

                    if (Member.FacebookAccountLogin != null)
                    {
                        if (!String.IsNullOrEmpty(CommonHelper.GetDecryptedData(Member.FacebookAccountLogin.ToString())))
                        {
                            mdc.FBID = CommonHelper.GetDecryptedData(Member.FacebookAccountLogin);
                        }
                    }
                    mdc.FirstName = CommonHelper.GetDecryptedData(Member.FirstName).ToUpper();
                    mdc.LastName = CommonHelper.GetDecryptedData(Member.LastName).ToUpper();
                    mdc.UserName = CommonHelper.GetDecryptedData(Member.UserName);
                    mdc.SecondaryEmail = Member.SecondaryEmail;
                    mdc.RecoveryEmail = CommonHelper.GetDecryptedData(Member.RecoveryEmail);
                    mdc.ContactNumber = Member.ContactNumber;
                    mdc.ImageURL = Member.Photo;
                    mdc.Address = CommonHelper.GetDecryptedData(Member.Address);
                    mdc.City = CommonHelper.GetDecryptedData(Member.City);
                    mdc.State = CommonHelper.GetDecryptedData(Member.State);
                    mdc.Zipcode = CommonHelper.GetDecryptedData(Member.Zipcode);
                    mdc.Status = Member.Status;
                    mdc.PinNumber = CommonHelper.GetDecryptedData(Member.PinNumber);
                    mdc.IsPhoneVerified = Member.IsVerifiedPhone ?? false;
                    mdc.Nooch_ID = NoochId;
                    //Get the Refered Code Used
                    mdc.ReferCodeUsed = (from Membr in obj.Members join Code in obj.InviteCodes on Membr.InviteCodeIdUsed equals Code.InviteCodeId where Membr.Nooch_ID == NoochId select Code.code).SingleOrDefault();

                    var MemberKnoxDetails = (from m in obj.KnoxAccountDetails where m.Member.MemberId == Member.MemberId && m.IsDeleted == false select m).FirstOrDefault();
                    mdc.IsKnocAvailable = (MemberKnoxDetails != null);
                    if (mdc.IsKnocAvailable)
                    {
                        mdc.KnoxBankIcon = MemberKnoxDetails.BankImageURL;
                        mdc.KnoxTransId = CommonHelper.GetDecryptedData(MemberKnoxDetails.TransId);
                    }


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


                    // getting last 5 disputes
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

                    // stats related operations
                    MemberDetailsStats ms = new MemberDetailsStats();

                    ms.TotalTransfer = obj.GetReportsForMember(Member.MemberId.ToString(), "Total_P2P_transfers").SingleOrDefault();


                    string TotalSent =
                     obj.GetReportsForMember(Member.MemberId.ToString(), "Total_$_Sent").SingleOrDefault();
                    ms.TotalSent = TotalSent != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(TotalSent))).ToString() : "0";

                    string TotalReceived =
                      obj.GetReportsForMember(Member.MemberId.ToString(), "Total_$_Received").SingleOrDefault();
                    ms.TotalReceived = TotalReceived != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(TotalReceived))).ToString() : "0";

                    string LargestSent =
                      obj.GetReportsForMember(Member.MemberId.ToString(), "Largest_sent_transfer").SingleOrDefault();
                    ms.LargestSent = LargestSent != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(LargestSent))).ToString() : "0";

                    mdc.MemberStats = ms;
                    double no_Of_Since_Joined = (DateTime.Now - Convert.ToDateTime(Member.DateCreated)).TotalDays / 7;
                    mdc.WeeksSinceJoined = Convert.ToInt16(no_Of_Since_Joined);


                    // Check whther synapse details available or not
                    var synapse = (from Syn in obj.SynapseBanksOfMembers
                                   join mem in obj.Members on Syn.MemberId equals mem.MemberId
                                   where Syn.IsDefault == true && mem.Nooch_ID == NoochId
                                   select Syn).FirstOrDefault();
                    mdc.IsSynapseDetailAvailable = (synapse != null);
                    if (mdc.IsSynapseDetailAvailable)
                    {
                        // get thhe synapse details 
                        var synapseDetail = (from Syn in obj.SynapseBanksOfMembers
                                             join mem in obj.Members on Syn.MemberId equals mem.MemberId
                                             where Syn.IsDefault == true && mem.Nooch_ID == NoochId
                                             select new SynapseDetailOFMember
                                             {
                                                 SynpaseBankName = Syn.bank_name,
                                                 SynpaseBankStatus = (string.IsNullOrEmpty(Syn.Status) ? "Not Verified" : Syn.Status),
                                                 BankId=Syn.Id
                                             }
                            
                                             ).FirstOrDefault();

                        if (synapseDetail != null)
                        {
                            if (!String.IsNullOrEmpty(synapseDetail.SynpaseBankName))
                            {
                                synapseDetail.SynpaseBankName =
                                    CommonHelper.GetDecryptedData(synapseDetail.SynpaseBankName);

                            }
                        }
                        mdc.SynapseDetails = synapseDetail;
                    }
                    // Get the three recent  Ip address of member
                    mdc.MemberIpAddr = (from memIpADD in obj.MembersIPAddresses
                                        join mem in obj.Members on
                                            memIpADD.MemberId equals mem.MemberId
                                        where mem.Nooch_ID == NoochId
                                        select new MemberIpAddrreses
                                        {
                                            IpAddress = memIpADD.Ip,
                                            Date = (DateTime)memIpADD.ModifiedOn
                                        }
                                          ).OrderByDescending(r => r.Date).Take(3).ToList(); 


                    // getting members referred
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
                }
            }
            return View(mdc);
        }

        // to update member details
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
                // getting member from db
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
            Logger.Info("NoochNewAdmin - ResetPin[ noochId:" + noochId + "].");

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
                        // email to user

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
                }

                #endregion

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
            Logger.Info("NoochNewAdmin - VerifyAccount[ accountId:" + accountId + "].");

            LoginResult lr = new LoginResult();
            int bnkid = Convert.ToInt16(accountId);
            using (var noochConnection = new NOOCHEntities())
            {
                // getting member from db
                var mem =
                    (from c in noochConnection.SynapseBanksOfMembers where c.Id == bnkid select c)
                        .SingleOrDefault();

                if (mem != null)
                {
                    #region if member found

                    mem.Status="Verified";

                    


                    int result = noochConnection.SaveChanges();

                    if (result > 0)
                    {
                        // email to user

                      
                            lr.IsSuccess = true;
                            lr.Message = "Bank account verified successfully.";
                      
                    }
                    else
                    {
                        lr.IsSuccess = false;
                        lr.Message = "Error occuerred on server, please retry.";
                    }
                }

                    #endregion

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
                    // getting member from db
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
                return "Fail";
            }
        }
    }
}