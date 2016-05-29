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
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Drawing;
using Newtonsoft.Json.Linq;


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
            catch (Exception)
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
                Response.Redirect("https://noochme.com/noochnewadmin/");
            }
        }


        /// <summary>
        /// For getting the page to view ALL MEMBERS.
        /// </summary>
        /// <returns></returns>
        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ListAll()
        {
            List<MembersListDataClass> AllMemberFormtted = GetAllMembers("Personal");

            return View(AllMemberFormtted);
        }


        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ListAllLandlords()
        {
            List<MembersListDataClass> AllLandlords = GetAllMembers("Landlord");

            return View(AllLandlords);
        }


        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult ListAllTenants()
        {
            List<MembersListDataClass> AllTenants = GetAllMembers("Tenant");

            return View(AllTenants);
        }


        /// <summary>
        /// Method to return the details of all MEMBERS.
        /// </summary>
        /// <param name="type">Specifies one of: 'Personal', 'Landlord', or 'Tenant' and returns only that type of Member.</param>
        /// <returns></returns>
        private List<MembersListDataClass> GetAllMembers(string type)
        {
            List<MembersListDataClass> AllMemberFormtted = new List<MembersListDataClass>();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                List<Member> All_Members_In_Records = new List<Member>();

                if (type == "Personal")
                {
                    All_Members_In_Records = (from t in obj.Members
                                              where ((t.Type == "Personal" ||
                                                    t.Type == "Business" ||
                                                    t.Type == "Personal - Browser") && t.IsDeleted == false)
                                              select t).ToList();
                }
                else if (type == "Landlord")
                {
                    All_Members_In_Records = (from t in obj.Members
                                              where t.Type == "Landlord"
                                              select t).ToList();
                }
                else if (type == "Tenant")
                {
                    All_Members_In_Records = (from t in obj.Members
                                              where t.Type == "Tenant"
                                              select t).ToList();
                }

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
                                          where tr.TransactionStatus == "Success" &&
                                               (tr.TransactionType == "5dt4HUwCue532sNmw3LKDQ==" || tr.TransactionType == "DrRr1tU1usk7nNibjtcZkA==") &&
                                                tr.Member.MemberId == m.MemberId
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
        public ActionResult Detail(string NoochId)
        {
            CheckSession();

            MemberDetailsClass mdc = new MemberDetailsClass();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                Member Member = new Member();

                if (NoochId.Length < 20) // it's a Nooch_Id and not a MemberID
                {
                    Member = (from m in obj.Members
                              where m.Nooch_ID == NoochId
                              select m).SingleOrDefault();
                }
                else
                {
                    Guid memGuid = new Guid(NoochId);

                    Member = (from m in obj.Members
                              where m.MemberId == memGuid
                              select m).SingleOrDefault();
                }

                if (Member != null)
                {
                    mdc.memberId = Member.MemberId.ToString();
                    mdc.DateCreated = Convert.ToDateTime(Member.DateCreated);
                    mdc.DateCreatedFormatted = String.Format("{0: MMM d, yyyy}", Member.DateCreated);
                    mdc.FBID = !String.IsNullOrEmpty(Member.FacebookAccountLogin) ? CommonHelper.GetDecryptedData(Member.FacebookAccountLogin) : "";
                    mdc.FirstName = !String.IsNullOrEmpty(Member.FirstName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.FirstName)) : "";
                    mdc.LastName = !String.IsNullOrEmpty(Member.LastName) ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.LastName)) : "";
                    mdc.UserName = !String.IsNullOrEmpty(Member.UserName) ? CommonHelper.GetDecryptedData(Member.UserName) : "";
                    mdc.SecondaryEmail = !String.IsNullOrEmpty(Member.SecondaryEmail) && Member.SecondaryEmail.Length > 40
                                         ? CommonHelper.GetDecryptedData(Member.SecondaryEmail)
                                         : Member.SecondaryEmail;
                    mdc.RecoveryEmail = !String.IsNullOrEmpty(Member.RecoveryEmail) && Member.RecoveryEmail.Length > 40
                                        ? CommonHelper.GetDecryptedData(Member.RecoveryEmail)
                                        : Member.RecoveryEmail;
                    mdc.ContactNumber = !String.IsNullOrEmpty(Member.ContactNumber) ? CommonHelper.FormatPhoneNumber(Member.ContactNumber) : "";
                    mdc.ImageURL = Member.Photo ?? "https://www.noochme.com/noochweb/Assets/Images/userpic-default.png";
                    mdc.Address = !String.IsNullOrEmpty(Member.Address) ? CommonHelper.GetDecryptedData(Member.Address) : "";
                    mdc.City = !String.IsNullOrEmpty(Member.City) ? CommonHelper.GetDecryptedData(Member.City) : "";
                    mdc.State = !String.IsNullOrEmpty(Member.State) ? CommonHelper.GetDecryptedData(Member.State) : "";
                    mdc.Zipcode = !String.IsNullOrEmpty(Member.Zipcode) ? CommonHelper.GetDecryptedData(Member.Zipcode) : "";
                    mdc.Status = Member.Status;
                    mdc.PinNumber = !String.IsNullOrEmpty(Member.PinNumber) ? CommonHelper.GetDecryptedData(Member.PinNumber) : "";
                    mdc.IsPhoneVerified = Member.IsVerifiedPhone ?? false;
                    mdc.Nooch_ID = Member.Nooch_ID;
                    mdc.type = Member.Type;
                    mdc.dob = Convert.ToDateTime(Member.DateOfBirth).ToString("M/d/yyyy");
                    mdc.ssn = !String.IsNullOrEmpty(Member.SSN) ? CommonHelper.GetDecryptedData(Member.SSN) : "";
                    mdc.idDocUrl = Member.VerificationDocumentPath;
                    mdc.adminNote = Member.AdminNotes;
                    mdc.isSdnSafe = Member.IsSDNSafe ?? false;
                    mdc.IsVerifiedWithSynapse = Member.IsVerifiedWithSynapse;
                    mdc.UDID1 = !String.IsNullOrEmpty(Member.UDID1) ? Member.UDID1 : "NULL";
                    mdc.DeviceToken = !String.IsNullOrEmpty(Member.DeviceToken) ? Member.DeviceToken : "NULL";
                    mdc.AccessToken = !String.IsNullOrEmpty(Member.AccessToken) ? Member.AccessToken : "NULL";
                    mdc.lastlat = (Member.LastLocationLat != null && Member.LastLocationLat != 0) ? Member.LastLocationLat.ToString() : "none";
                    mdc.lastlong = (Member.LastLocationLat != null && Member.LastLocationLng != 0) ? Member.LastLocationLng.ToString() : "none";
                    mdc.TransferLimit = Member.TransferLimit ?? "0.00";
                    mdc.cipTag = !String.IsNullOrEmpty(Member.cipTag) ? Member.cipTag : "NULL";

                    //Get the Refered Code Used
                    mdc.ReferCodeUsed = (from Membr in obj.Members
                                         join Code in obj.InviteCodes
                                         on Membr.InviteCodeIdUsed equals Code.InviteCodeId
                                         where Membr.Nooch_ID == NoochId
                                         select Code.code).SingleOrDefault();


                    #region Get User's Transactions

                    var memberFullName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.FirstName)) + " " +
                                         CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.LastName));

                    var transList = (from t in obj.Transactions
                                     where (t.Member.MemberId == Member.MemberId ||
                                            t.RecipientId == Member.MemberId ||
                                            t.SenderId == Member.MemberId ||
                                            t.InvitationSentTo == Member.UserName)
                                     //(t.TransactionType == "5dt4HUwCue532sNmw3LKDQ==" || t.TransactionType == "+C1+zhVafHdXQXCIqjU/Zg==" || t.TransactionType == "DrRr1tU1usk7nNibjtcZkA==")
                                     //(t.TransactionStatus == "Success" || t.TransactionStatus == "Rejected" || t.TransactionStatus == "Pending" || t.TransactionStatus == "Cancelled")
                                     select t).OrderByDescending(r => r.TransactionDate).Take(25).ToList();


                    List<MemberDetailsTrans> mm = new List<MemberDetailsTrans>();

                    foreach (Transaction t in transList)
                    {
                        MemberDetailsTrans payment = new MemberDetailsTrans();

                        payment.AmountNew = t.Amount.ToString();
                        payment.TransID = t.TransactionTrackingId.ToString();
                        payment.TransDate = Convert.ToDateTime(t.TransactionDate).ToString("MMM d, yyyy");

                        payment.TransDatePaid = t.DateAccepted != null ? "PAID ON:  " + Convert.ToDateTime(t.DateAccepted).ToString("MMM d, yyyy") : null;
                        payment.TransTime = Convert.ToDateTime(t.TransactionDate).ToString("h:mm tt");
                        payment.TransactionStatus = t.TransactionStatus == "Success" && t.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw=="
                                                    ? "Complete (Paid)"
                                                    : t.TransactionStatus;
                        payment.TransactionType = CommonHelper.GetDecryptedData(t.TransactionType);
                        payment.Memo = t.Memo;
                        payment.GeoLocation = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.City + " , " + Geo_loc.State).SingleOrDefault();
                        payment.Longitude = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.Longitude).SingleOrDefault().ToString();
                        payment.Latitude = (from Geo_loc in obj.GeoLocations where Geo_loc.LocationId == t.LocationId select Geo_loc.Latitude).SingleOrDefault().ToString();

                        payment.SynapseStatus = (!String.IsNullOrEmpty(t.SynapseStatus)) ? t.SynapseStatus.ToString() : "-";

                        if (payment.TransactionType == "Request")
                        {
                            #region Requests

                            // Note: The 'SenderId' & 'RecipientId' would both be the SENDER if it's a request to a Non-Nooch user.
                            //       So if the RecipientID matches this user's MemberID, then we know for sure that this user sent the request,
                            //       b/c a non-Nooch user couldn't have sent the request to this user.
                            if (t.RecipientId == Member.MemberId)
                            {
                                // This member SENT the request, so he is considered the "Recipient" for requests, since the other user will pay (send $ to) him.
                                payment.RecipientId = Member.Nooch_ID;
                                payment.RecipientName = memberFullName;

                                // Now determine if it was sent to an existing user or a Non-Nooch user
                                if (String.IsNullOrEmpty(t.InvitationSentTo) && t.IsPhoneInvitation != true)
                                {
                                    // Request to an EXISTING user
                                    payment.SenderName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member1.FirstName)) + " " +
                                                         CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member1.LastName));
                                    payment.SenderId = t.Member1.Nooch_ID;
                                }
                                else if (!String.IsNullOrEmpty(t.InvitationSentTo)) // Request to Non-Nooch user via EMAIL
                                {
                                    // Now check if the request has been paid already or not
                                    if (t.TransactionStatus == "Success")
                                    {
                                        // Completed request, so get the new user's Member Details from the InvitationSentTo value
                                        // Note: even if the new user entered a diff email to pay the request, the server saves the original Invited email as the SecondaryEmail.
                                        Member newUserWhoPaidTheRequest = CommonHelper.GetMemberDetailsByUsername(CommonHelper.GetDecryptedData(t.InvitationSentTo));

                                        payment.SenderId = newUserWhoPaidTheRequest != null ? newUserWhoPaidTheRequest.Nooch_ID : "";
                                        payment.SenderName = newUserWhoPaidTheRequest != null
                                                             ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(newUserWhoPaidTheRequest.FirstName)) + " " +
                                                               CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(newUserWhoPaidTheRequest.LastName))
                                                             : CommonHelper.GetDecryptedData(t.InvitationSentTo);
                                    }
                                    else
                                    {
                                        // Request is either still pending, or was rejected/cancelled... so there isn't a New Member to get, so just use the InvitationSentTo email
                                        payment.SenderId = "";
                                        payment.SenderName = CommonHelper.GetDecryptedData(t.InvitationSentTo);
                                    }
                                }
                                else if (t.IsPhoneInvitation == true) // Request to Non-Nooch user via PHONE
                                {
                                    // Now check if the request has been paid already or not
                                    if (t.TransactionStatus == "Success")
                                    {
                                        // Completed request, so get the new user's Member Details from the PhoneNumberInvited value
                                        Member newUserWhoPaidTheRequest = CommonHelper.GetMemberDetailsByPhone(CommonHelper.GetDecryptedData(t.InvitationSentTo));

                                        payment.SenderId = newUserWhoPaidTheRequest != null ? newUserWhoPaidTheRequest.Nooch_ID : "";
                                        payment.SenderName = newUserWhoPaidTheRequest != null
                                                             ? CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(newUserWhoPaidTheRequest.FirstName)) + " " +
                                                               CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(newUserWhoPaidTheRequest.LastName))
                                                             : CommonHelper.FormatPhoneNumber(CommonHelper.GetDecryptedData(t.PhoneNumberInvited));
                                    }
                                    else
                                    {
                                        // Request is either still pending, or was rejected/cancelled... so there isn't a New Member to get, so just use the PhoneNumberInvited
                                        payment.SenderId = "";
                                        payment.SenderName = !String.IsNullOrEmpty(t.PhoneNumberInvited)
                                                             ? CommonHelper.FormatPhoneNumber(CommonHelper.GetDecryptedData(t.PhoneNumberInvited))
                                                             : "No Phone # Found";
                                    }
                                }
                            }
                            else
                            {
                                // This member RECEIVED the request, so he is considered the "sender" for requests, since he must pay (send $ to) the other user.

                                //payment.RecipientName = memberFullName;
                                //payment.RecipientId = Member.Nooch_ID;

                                payment.SenderName = memberFullName;
                                payment.SenderId = Member.Nooch_ID;
                                payment.RecipientName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member1.FirstName)) + " " +
                                                        CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member1.LastName));
                                payment.RecipientId = t.Member1.Nooch_ID;
                            }

                            #endregion Requests
                        }
                        else
                        {
                            #region Transfers

                            if (t.SenderId == Member.MemberId) // This member SENT the transfer
                            {
                                payment.SenderName = memberFullName;
                                payment.SenderId = Member.Nooch_ID;
                                payment.RecipientId = t.Member1.Nooch_ID;
                                payment.RecipientName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member1.FirstName)) + " " +
                                                        CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member1.LastName));
                            }
                            else // This member RECEIVED the transfer
                            {
                                payment.SenderName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member.FirstName)) + " " +
                                                     CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(t.Member.LastName));
                                payment.SenderId = t.Member.Nooch_ID;
                                payment.RecipientName = memberFullName;
                                payment.RecipientId = Member.Nooch_ID;
                            }

                            #endregion Transfers
                        }

                        mm.Add(payment);
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

                    // Cliff (5/24/16): These need updating... they aren't counting initial payments when the user is created from a request/invite.

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


                    #region Get Synapse Details

                    // Check whether the user has a Synapse Account
                    var synapseCreateUserObj = (from Syn in obj.SynapseCreateUserResults
                                                where Syn.IsDeleted == false && Syn.MemberId == Member.MemberId
                                                select Syn).FirstOrDefault();

                    mdc.IsSynapseDetailAvailable = (synapseCreateUserObj != null);

                    if (mdc.IsSynapseDetailAvailable)
                    {
                        SynapseDetailOFMember synapseDetail = new SynapseDetailOFMember();

                        synapseDetail.userDateCreated = Convert.ToDateTime(synapseCreateUserObj.DateCreated).ToString("MMM dd, yyyy");
                        synapseDetail.synapseRefreshKey = !String.IsNullOrEmpty(synapseCreateUserObj.refresh_token)
                                                              ? CommonHelper.GetDecryptedData(synapseCreateUserObj.refresh_token)
                                                              : "REFRESH TOKEN NOT FOUND";
                        synapseDetail.synapseConsumerKey = !String.IsNullOrEmpty(synapseCreateUserObj.access_token)
                                                           ? CommonHelper.GetDecryptedData(synapseCreateUserObj.access_token)
                                                           : "AUTH TOKEN NOT FOUND";
                        synapseDetail.synapseUserId = synapseCreateUserObj.user_id.ToString();
                        synapseDetail.isBusiness = synapseCreateUserObj.is_business ?? false;
                        synapseDetail.userPermission = synapseCreateUserObj.permission;
                        synapseDetail.photos = synapseCreateUserObj.photos;

                        // Now get the user's Synapse Bank details
                        var synapseBankDetails = (from Syn in obj.SynapseBanksOfMembers
                                                  where Syn.IsDefault == true && Syn.MemberId == Member.MemberId
                                                  select Syn).FirstOrDefault();

                        if (synapseBankDetails != null)
                        {
                            synapseDetail.SynapseBankName = !String.IsNullOrEmpty(synapseBankDetails.bank_name)
                                                            ? CommonHelper.GetDecryptedData(synapseBankDetails.bank_name)
                                                            : "Not Found";
                            synapseDetail.BankId = synapseBankDetails.Id; // This is the Nooch DB "ID", which is just the row number of the account... NOT the same as the Synapse Bank ID
                            synapseDetail.synapseBankId = synapseBankDetails.oid;

                            synapseDetail.SynapseBankStatus = (String.IsNullOrEmpty(synapseBankDetails.Status)
                                                              ? "Not Verified"
                                                              : synapseBankDetails.Status);
                            synapseDetail.mfaVerified = synapseBankDetails.mfa_verifed ?? false;
                            synapseDetail.SynapseBankNickName = !String.IsNullOrEmpty(synapseBankDetails.nickname)
                                                                ? CommonHelper.GetDecryptedData(synapseBankDetails.nickname)
                                                                : "";
                            synapseDetail.nameFromSynapseBank = !String.IsNullOrEmpty(synapseBankDetails.name_on_account)
                                                                ? CommonHelper.GetDecryptedData(synapseBankDetails.name_on_account)
                                                                : "No Name Returned";
                            synapseDetail.SynpaseBankAddedOn = synapseBankDetails.AddedOn != null
                                                               ? Convert.ToDateTime(synapseBankDetails.AddedOn).ToString("MMM dd, yyyy")
                                                               : "";
                            synapseDetail.SynpaseBankVerifiedOn = synapseBankDetails.VerifiedOn != null
                                                                  ? Convert.ToDateTime(synapseBankDetails.VerifiedOn).ToString("MMM dd, yyyy")
                                                                  : "";

                            // CLIFF (5/8/16): Added these V3 fields so we can display on the Member Details page of Admin Dash
                            synapseDetail.allowed = !String.IsNullOrEmpty(synapseBankDetails.allowed)
                                                    ? synapseBankDetails.allowed
                                                    : "no value";
                            synapseDetail.bankClass = !String.IsNullOrEmpty(synapseBankDetails.@class)
                                                    ? synapseBankDetails.@class
                                                    : " - ";
                            synapseDetail.bankType = !String.IsNullOrEmpty(synapseBankDetails.type_bank)
                                                    ? synapseBankDetails.type_bank
                                                    : " - ";
                            synapseDetail.nodeType = !String.IsNullOrEmpty(synapseBankDetails.type_synapse)
                                                    ? synapseBankDetails.type_synapse
                                                    : " - ";

                            // CLIFF (5/8/16): I don't think these are needed anymore with V3 b/c Synapse no longer sends these data
                            synapseDetail.emailFromSynapseBank = !String.IsNullOrEmpty(synapseBankDetails.email) // CLIFF (5/8/16): I don't think this is needed anymore with V3
                                     ? synapseBankDetails.email : "";
                            synapseDetail.phoneFromSynapseBank = !String.IsNullOrEmpty(synapseBankDetails.phone_number)
                                     ? CommonHelper.FormatPhoneNumber(synapseBankDetails.phone_number) : "";
                        }

                        mdc.SynapseDetails = synapseDetail;
                    }

                    #endregion Get Synapse Details

                    // Get the 3 most recent IP Addresses for this user
                    mdc.MemberIpAddr = (from memIpADD in obj.MembersIPAddresses
                                        where memIpADD.MemberId == Member.MemberId
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

                        var allreferrals = (from c in obj.Members
                                            where c.InviteCodeIdUsed == g && c.IsDeleted == false
                                            select c)
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

                    if (mdc.type == "Tenant")
                    {
                        List<Tenants> tenants = GetTenants(Member.Nooch_ID);
                        mdc.tenant = tenants.FirstOrDefault();
                    }
                }
                if (Session["status"] != null)
                {
                    mdc.DocStatus = Session["status"].ToString();
                    Session["status"] = null;
                }
            }

            return View(mdc);
        }


        [HttpPost]
        [ActionName("ReSendVrificationSMS")]
        public ActionResult ReSendVrificationSMS(string noochIds)
        {
            MemberOperationsResult res = new MemberOperationsResult();
            res.IsSuccess = false;

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var member = (from t in obj.Members
                              where t.Nooch_ID == noochIds && t.IsDeleted == false
                              select t).SingleOrDefault();

                if (member != null)
                {
                    string fname = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(member.FirstName));
                    string MessageBody = "Hi " + fname + ", This is Nooch - just need to verify this is your phone number. Please reply 'Go' to confirm your phone number.";
                    res.Message = Utility.SendSMS(Utility.RemovePhoneNumberFormatting(member.ContactNumber), MessageBody, member.MemberId.ToString());

                    if (res.Message != "Failure")
                    {
                        res.IsSuccess = true;
                    }
                }
            }

            return Json(res);
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
        public ActionResult EditMemberDetails(string contactno, string streetaddress, string city, string secondaryemail, string recoveryemail, string noochid, string state, string zip, string ssn, string dob, string transferLimit)
        {
            Logger.Info("Admin Member Controller -> EditMemberDetails Initiated - Contact Number: [" + contactno + "], Street Address: [" + streetaddress +
                        "], City: [" + city + "], State: [" + state + "], ZIP: [" + zip + "], secondaryEmail: [" + secondaryemail + "], NoochID: [" + noochid +
                        "], SSN [" + ssn + "], DOB: [" + dob + "]");

            MemberEditResultClass res = new MemberEditResultClass();
            res.IsSuccess = false;

            if (String.IsNullOrEmpty(noochid))
            {
                res.Message = "Invalid nooch id passed";
            }
            else
            {
                try
                {
                    // Get member from DB
                    using (NOOCHEntities obj = new NOOCHEntities())
                    {
                        var member = (from t in obj.Members
                                      where t.Nooch_ID == noochid && t.IsDeleted == false
                                      select t).SingleOrDefault();

                        if (member == null)
                        {
                            res.Message = "Member not found";
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

                            if (!String.IsNullOrEmpty(ssn))
                            {
                                member.SSN = CommonHelper.GetEncryptedData(ssn.Trim());
                            }
                            if (!String.IsNullOrEmpty(transferLimit))
                            {
                                member.TransferLimit = transferLimit.Trim();
                            }

                            if (!String.IsNullOrEmpty(dob) && dob != "1/1/0001")
                            {
                                DateTime dateofbirth;
                                if (!DateTime.TryParse(dob, out dateofbirth))
                                {
                                    Logger.Error("Admin Member Controller -> EditMemberDetails - DOB was NULL - [MemberID: " + member.MemberId + "]");
                                }

                                member.DateOfBirth = dateofbirth;
                            }
                            member.DateModified = DateTime.Now;

                            obj.SaveChanges();

                            res.Address = streetaddress;
                            res.secondaryemail = secondaryemail;
                            res.recoveryemail = recoveryemail;
                            res.contactnum = CommonHelper.FormatPhoneNumber(contactno);
                            res.City = city;
                            res.state = state.ToUpper();
                            res.zip = zip;
                            res.ssn = ssn;
                            res.dob = dob;
                            res.IsSuccess = true;
                            res.Message = "Member record updated successfully";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Admin MemberController -> EditMemberDetails FAILED - [Exception: " + ex.Message + "]");
                }
            }

            return Json(res);
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
                var mem = (from c in noochConnection.Members
                           where c.Nooch_ID == noochId && c.IsDeleted == false
                           select c).SingleOrDefault();

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
        public ActionResult VerifyAccount(string accountId, bool sendEmail)
        {
            Logger.Info("Admin Dash -> VerifyAccount - Synapse Bank ID: [" + accountId + "], sendEmail: [" + sendEmail + "]");

            LoginResult lr = new LoginResult();
            lr.IsSuccess = false;

            int bnkid = Convert.ToInt16(accountId);

            using (var noochConnection = new NOOCHEntities())
            {
                // Get Synapse Bank info from DB
                var bank = (from c in noochConnection.SynapseBanksOfMembers
                            where c.Id == bnkid
                            select c).SingleOrDefault();

                if (bank != null)
                {
                    if (bank.Status == "Verified")
                    {
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

                            #region Notify User by Email

                            if (sendEmail != false)
                            {
                                try
                                {
                                    Guid memberId = new Guid(bank.MemberId.ToString());
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
                                        var toAddress = CommonHelper.GetDecryptedData(noochMember.UserName).ToLower();
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

                            #endregion Notify User by Email
                        }
                        else
                        {
                            lr.Message = "Error occurred on server, please retry.";
                        }
                    }
                }
                else
                {
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
            lr.IsSuccess = false;

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
                            lr.Message = "Error occuerred on server, please retry.";
                        }
                    }
                }

                else
                {
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

        [HttpPost]
        [ActionName("UpdatePassword")]
        public ActionResult UpdatePassword(string noochIds, string newPassword)
        {

            MemberOperationsResult res = new MemberOperationsResult();
            res.IsSuccess = false;

            try
            {
                using (NOOCHEntities obj = new NOOCHEntities())
                {

                    var temp = CommonHelper.GetRandomTransactionTrackingId();

                    var member = (from t in obj.Members
                                  where t.Nooch_ID == noochIds && t.IsDeleted == false
                                  select t).SingleOrDefault();

                    if (member != null)
                    {
                        member.Password = CommonHelper.GetEncryptedData(newPassword);
                        var o = obj.SaveChanges();

                        if (o == 1)
                        {
                            res.Message = "Password updated";
                            res.IsSuccess = true;

                            string fname = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(member.FirstName));
                            string MessageBody = "Hi " + fname + ", This is Nooch - Your password has been updated to" + member.Password;

                            var fromAddress = Utility.GetValueFromConfig("adminMail");

                            var tokens = new Dictionary<string, string>
                        {
                            {Constants.PLACEHOLDER_FIRST_NAME, fname},
                            {Constants.PLACEHOLDER_PASSWORD, CommonHelper.GetDecryptedData( member.Password)}
                        };

                            string toAddress = CommonHelper.GetDecryptedData(member.UserName);
                            bool emailSent = Utility.SendEmail("passwordChangedByAdmin",
                                                    fromAddress, toAddress, "Your Nooch password has been changed", null,
                                                    tokens, null, null, null);

                            if (emailSent)
                                res.Message += " & Email has been Sent.";
                        }
                        else
                            res.IsSuccess = false;
                    }
                    else
                    {
                        res.Message = "User not found";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr -> UpdatePassword FAILED - NoochID: [" + noochIds + "], Exception: [" + ex.Message + "]");
            }

            return Json(res);
        }


        [HttpPost]
        [ActionName("GenerateNewPassword")]
        public ActionResult GenerateNewPassword()
        {
            MemberOperationsResult res = new MemberOperationsResult();
            res.Message = CommonHelper.GetRandomTransactionTrackingId();
            res.IsSuccess = true;
            return Json(res);
        }


        [HttpPost]
        [ActionName("Details")]
        public ActionResult Details(HttpPostedFileBase file, string NoochId)
        {
            SaveVerificationIdDocument DocumentDetails = new SaveVerificationIdDocument();

            using (var noochConnection = new NOOCHEntities())
            {
                var member = noochConnection.Members.Where(m => m.Nooch_ID == NoochId).FirstOrDefault();

                DocumentDetails.MemberId = member.MemberId.ToString();

                var MemberId = Utility.ConvertToGuid(DocumentDetails.MemberId.ToString());
                var SynapseCreateUserResult = noochConnection.SynapseCreateUserResults.Where(m => m.MemberId == MemberId).FirstOrDefault();

                if (SynapseCreateUserResult != null)
                {
                    DocumentDetails.AccessToken = SynapseCreateUserResult.access_token;
                    string pic = MemberId.ToString() + ".png";
                    string path = System.IO.Path.Combine(Server.MapPath("~/UploadedPhotos/SynapseDocuments"), pic);

                    // file is uploaded
                    file.SaveAs(path);
                    DocumentDetails.imgPath = path;

                    synapseV3GenericResponse submitDocToSynapseRes = submitDocumentToSynapseV3(DocumentDetails);
                    if (submitDocToSynapseRes.isSuccess == true)
                    {
                        Session["status"] = "Success";
                    }
                    else
                    {
                        Session["status"] = "Failed";
                    }
                    return RedirectToAction("Detail", "Member", new { NoochId = NoochId });
                }
                else
                {
                    Session["status"] = "Failed";
                    return RedirectToAction("Detail", "Member", new { NoochId = NoochId });
                }
            }
        }


        public synapseV3GenericResponse submitDocumentToSynapseV3(SaveVerificationIdDocument DocumentDetails)
        {
            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            try
            {
                Logger.Info("Member Cntrlr  -> submitDocumentToSynapseV3 - [MemberId: " + DocumentDetails.MemberId + "]");

                // Make URL from byte array...b/c submitDocumentToSynapseV3() expects URL of image.
                string ImageUrlMade = "";

                if (DocumentDetails.imgPath != "")
                {
                    ImageUrlMade = Utility.GetValueFromConfig("SynapseUploadedDocPhotoUrl") + DocumentDetails.MemberId + ".png";
                }
                else
                {
                    ImageUrlMade = Utility.GetValueFromConfig("SynapseUploadedDocPhotoUrl") + "gv_no_photo.png";
                }

                var submitDocResult = submitDocumentToSynapseV3(DocumentDetails.MemberId, ImageUrlMade);

                res.isSuccess = submitDocResult.isSuccess;
                res.msg = submitDocResult.msg;

                return res;
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr  - submitDocumentToSynapseV3 FAILED - [userName: " + DocumentDetails.MemberId + "]. Exception: [" + ex + "]");
                throw new Exception("Server Error.");
            }
        }


        public synapseV3GenericResponse submitDocumentToSynapseV3(string MemberId, string ImageUrl)
        {
            Logger.Info("Member Cntrlr -> submitDocumentToSynapseV3 Initialized - [MemberId: " + MemberId + "]");

            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            try
            {
                Guid id = Utility.ConvertToGuid(MemberId);

                #region Get User's Synapse OAuth Consumer Key

                string usersSynapseOauthKey = "";
                using (var noochConnection = new NOOCHEntities())
                {
                    var usersSynapseDetails = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == id && m.IsDeleted == false);

                    if (usersSynapseDetails == null)
                    {
                        Logger.Error("Member Cntrlr-> submitDocumentToSynapseV3 ABORTED: Member's Synapse User Details not found. [MemberId: " + MemberId + "]");
                        res.msg = "Could not find this member's account info";
                        return res;
                    }
                    else
                    {
                        #region Check If OAuth Key Still Valid

                        synapseV3checkUsersOauthKey checkTokenResult = CommonHelper.refreshSynapseV3OautKey(usersSynapseDetails.access_token);

                        if (checkTokenResult != null)
                        {
                            if (checkTokenResult.success == true)
                            {
                                usersSynapseOauthKey = CommonHelper.GetDecryptedData(checkTokenResult.oauth_consumer_key);
                            }
                            else
                            {
                                Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 FAILED on Checking User's Synapse OAuth Token - " +
                                             "CheckTokenResult.msg: [" + checkTokenResult.msg + "], MemberID: [" + MemberId + "]");
                                res.msg = checkTokenResult.msg;
                            }
                        }
                        else
                        {
                            Logger.Error("Member Cntrlr -> GetSynapseBankAndUserDetailsforGivenMemberId FAILED on Checking User's Synapse OAuth Token - " +
                                         "CheckTokenResult was NULL, MemberID: [" + MemberId + "]");
                            res.msg = "Unable to check user's Oauth Token";
                        }

                        #endregion Check If OAuth Key Still Valid
                    }
                }

                #endregion Get User's Synapse OAuth Consumer Key


                #region Get User's Fingerprint

                string usersFingerprint = "";

                using (var noochConnection = new NOOCHEntities())
                {
                    var member = noochConnection.Members.FirstOrDefault(m => m.MemberId == id && m.IsDeleted != true);

                    if (member == null)
                    {
                        Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 ABORTED: Member not found. [MemberId: " + MemberId + "]");
                        res.msg = "Member not found";
                        return res;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(member.UDID1))
                        {
                            Logger.Info("Member Cntrlr -> submitDocumentToSynapseV3 ABORTED: Member's Fingerprint not found. [MemberId: " + MemberId + "]");
                            res.msg = "Could not find this member's fingerprint";
                            return res;
                        }
                        else
                        {
                            usersFingerprint = member.UDID1;
                        }
                    }
                }

                #endregion Get User's Fingerprint


                #region Call Synapse /user/doc/attachments/add API

                try
                {
                    submitDocToSynapseV3Class submitDocObj = new submitDocToSynapseV3Class();

                    SynapseV3Input_login login = new SynapseV3Input_login();
                    login.oauth_key = usersSynapseOauthKey;
                    submitDocObj.login = login;

                    submitDocToSynapse_user user = new submitDocToSynapse_user();
                    submitDocToSynapse_user_doc doc = new submitDocToSynapse_user_doc();
                    doc.attachment = "data:text/csv;base64," + CommonHelper.ConvertImageURLToBase64(ImageUrl).Replace("\\", "");

                    user.fingerprint = usersFingerprint;
                    user.doc = doc;

                    submitDocObj.user = user;

                    string baseAddress = Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox")) ? "https://sandbox.synapsepay.com/api/v3/user/doc/attachments/add" : "https://synapsepay.com/api/v3/user/doc/attachments/add";

                    var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                    http.Accept = "application/json";
                    http.ContentType = "application/json";
                    http.Method = "POST";

                    string parsedContent = JsonConvert.SerializeObject(submitDocObj);
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    Byte[] bytes = encoding.GetBytes(parsedContent);

                    Stream newStream = http.GetRequestStream();
                    newStream.Write(bytes, 0, bytes.Length);
                    newStream.Close();

                    var response = http.GetResponse();
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                    JObject refreshResponse = JObject.Parse(content);

                    kycInfoResponseFromSynapse resFromSynapse = new kycInfoResponseFromSynapse();

                    resFromSynapse = JsonConvert.DeserializeObject<kycInfoResponseFromSynapse>(content);

                    if (resFromSynapse != null)
                    {
                        if (resFromSynapse.success == true || resFromSynapse.success.ToString().ToLower() == "true")
                        {
                            var permission = resFromSynapse.user.permission != null ? resFromSynapse.user.permission : "NOT FOUND";
                            var physDoc = "NOT FOUND";
                            var virtualDoc = "NOT FOUND";

                            if (resFromSynapse.user.doc_status != null)
                            {
                                physDoc = resFromSynapse.user.doc_status.physical_doc;
                                virtualDoc = resFromSynapse.user.doc_status.virtual_doc;
                            }

                            // Update User's "Permission" in SynapseCreateUserResults Table
                            using (var noochConnection = new NOOCHEntities())
                            {
                                var synapseUser = noochConnection.SynapseCreateUserResults.Where(m => m.MemberId == id && m.IsDeleted == false).FirstOrDefault();
                                synapseUser.permission = resFromSynapse.user.permission.ToString();
                                synapseUser.photos = ImageUrl;

                                // Cliff (5/21/16): ALSO NEED TO SAVE "virtual_doc" and "physical_doc" VALUES IN NOOCH DB, BUT FIELDS DON'T EXIST YET
                                //storing user.doc_status.physical_doc , user.doc_status.virtual_doc and user.extra.extra_security
                                synapseUser.physical_doc = resFromSynapse.user.doc_status.physical_doc;
                                synapseUser.virtual_doc = resFromSynapse.user.doc_status.virtual_doc;

                                JToken http_code = refreshResponse["http_code"];
                                if (http_code != null)
                                {
                                    if (http_code.ToString() == "200")
                                    {
                                        JToken extra_Security_Obj = refreshResponse["user"]["extra"]["extra_security"];

                                        if (extra_Security_Obj != null)
                                        {
                                            synapseUser.extra_security = extra_Security_Obj.ToString();
                                        }

                                    }
                                }


                                int save = noochConnection.SaveChanges();

                                if (save > 0)
                                {
                                    Logger.Info("Member Cntrlr -> submitDocumentToSynapseV3 - SUCCESSFULLY updated user's Synapse Permission in SynapseCreateUserResult Table" +
                                                "New Permission: [" + permission + "], ID Doc URL: [" + ImageUrl + "]");
                                    res.msg = "ID doc saved and submitted successfully.";
                                    res.isSuccess = true;
                                }
                                else
                                {
                                    Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 - FAILED to save new Permission and ImageURL in SynapseCreateUserResult Table" +
                                                "New Permission: [" + permission + "], ID Doc URL: [" + ImageUrl + "]");
                                    res.msg = "Error saving ID doc";
                                }
                            }
                        }
                        else
                        {
                            res.msg = "Got a response, but success was not true";
                            Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 FAILED - Got Synapse response, but success was NOT 'true' - [MemberID: " + MemberId + "]");
                        }
                    }
                    else
                    {
                        res.msg = "Verification response was null";
                        Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 FAILED - Synapse response was NULL - [MemberID: " + MemberId + "]");
                    }
                }
                catch (WebException ex)
                {
                    // TO DO: ADD ERROR HANDLING FOR SYNAPSE RESPONSE...

                    res.msg = "Member Cntrlr Exception #1864";
                    Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 FAILED - Catch [Exception: " + ex.Message + "]");
                }

                #endregion Call Synapse /user/doc/attachments/add API
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 FAILED - Outer Exception - " +
                             "MemberID: [" + MemberId + "], Exception: [" + ex.Message + "]");
            }

            return res;
        }


        public List<Tenants> GetTenants(String NoochId)
        {
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                List<Tenants> tenants = new List<Tenants>();

                var tenant = (from t in obj.Tenants
                              join ut in obj.UnitsOccupiedByTenants on t.TenantId equals ut.TenantId
                              join pu in obj.PropertyUnits on ut.UnitId equals pu.UnitId
                              join p in obj.Properties on pu.PropertyId equals p.PropertyId
                              join m in obj.Members on t.MemberId equals m.MemberId
                              where m.Nooch_ID == NoochId

                              select new Tenants
                              {
                                  LastPayment = ut.LastPaymentAmount,
                                  Status = pu.Status,
                                  Rent = pu.UnitRent,
                                  Unit = pu.UnitNumber,
                                  dueDate = pu.DueDate,
                                  LeaseLength = pu.LeaseLength,
                                  Property = p.PropName,
                                  AutoPay = t.IsAutopayOn,
                                  AdminNote = m.AdminNotes,
                                  LastPaymentDate = ut.LastPaymentDate
                              });

                foreach (var t in tenant)
                {
                    Tenants tn = new Tenants();
                    tn.LastPayment = t.LastPayment;
                    tn.Status = t.Status;
                    tn.Rent = t.Rent;
                    tn.Unit = t.Unit;
                    tn.dueDate = t.dueDate;
                    tn.Property = t.Property;
                    tn.LeaseLength = t.LeaseLength;
                    tn.AutoPay = t.AutoPay;
                    if (t.LastPaymentDate != null)
                    {
                        tn.LastPaymentDate1 = Convert.ToDateTime(t.LastPaymentDate).ToString("MMM d, yyyy");
                    }

                    tenants.Add(tn);
                }
                return tenants;
            }
        }

    }
}