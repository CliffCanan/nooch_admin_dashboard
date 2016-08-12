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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ImageResizer;


namespace noochAdminNew.Controllers
{
    public class MemberController : Controller
    {
        [HttpPost]
        [ActionName("ApplyOperation")]
        public ActionResult ApplyOperation(string operation, string noochIds, bool sendEmail = false)
        {
            MemberOperationsResult res = new MemberOperationsResult();
            res.IsSuccess = false;

            if (operation == "0")
            {
                res.Message = "No operation selected to perform";
                return Json(res);
            }

            if (noochIds.Length == 0)
            {
                res.Message = "No user selected to perfrom action";
                return Json(res);
            }

            try
            {
                noochIds = noochIds.TrimEnd(',');
                List<string> allNoochIdsChoosen = noochIds.Split(',').ToList<string>();

                List<MemberOperationsInnerClass> resInnerClass = new List<MemberOperationsInnerClass>();

                #region Suspend

                if (operation == "1")
                {
                    // Suspend User

                    foreach (string s in allNoochIdsChoosen)
                    {
                        using (NOOCHEntities obj = new NOOCHEntities())
                        {
                            var member = (from t in obj.Members
                                          where t.Nooch_ID == s && t.Status != Constants.STATUS_SUSPENDED
                                          select t).SingleOrDefault();

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

                                mic.NoochId = member.Nooch_ID;
                                mic.Message = "Member Suspended Successfully";
                                mic.IsSuccess = true;

                                // sending email to member
                                if (sendEmail == true)
                                {
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
                                    }
                                    catch (Exception)
                                    {
                                        // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                        Logger.Error("Admin Dash -> MemberController - SupendMember email NOT send to: [" + emailAddress + "]; Member [memberId:" +
                                                     member.MemberId + "]. Problem occured in sending mail.");

                                        mic.Message = "Member Suspension Failed";
                                        mic.IsSuccess = false;
                                    }
                                }
                                else
                                {
                                    Logger.Info("Admin Dash -> MemberController - 'sendEmail' flag was FALSE, so NOT sending Suspended Email notification");
                                }

                                resInnerClass.Add(mic);
                            }
                            else
                            {
                                mic.Message = "Member already Suspended";
                                mic.NoochId = s;
                                resInnerClass.Add(mic);
                            }
                            #endregion
                        }
                    }

                    res.IsSuccess = true;
                    res.Message = "all operations performed";
                    res.MemberOperationsOuterClass = resInnerClass;
                }

                #endregion Suspend


                #region SDN Safe

                else if (operation == "2")
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
                                    resInnerClass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("Admin Dash -> Members Controller - Mark SDN Safe FAILED - MemberId: [" +
                                                member.MemberId + "]");

                                    mic.Message = "Member SDN mark safe failed!";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    resInnerClass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Member already SDN Safe ";
                                mic.NoochId = s;
                                resInnerClass.Add(mic);
                            }
                            #endregion
                        }
                    }

                    res.IsSuccess = true;
                    res.Message = "all operations performed";
                    res.MemberOperationsOuterClass = resInnerClass;
                }

                #endregion SDN Safe


                #region Verify Phone

                else if (operation == "3")
                {
                    // Verify User's Phone Number

                    foreach (string s in allNoochIdsChoosen)
                    {
                        using (NOOCHEntities obj = new NOOCHEntities())
                        {
                            var member = (from t in obj.Members
                                          where t.Nooch_ID == s && t.ContactNumber != null
                                          select t).SingleOrDefault();

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
                                    resInnerClass.Add(mic);

                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("VerifyPhone - Attempt to verify member phone failed [" + member.MemberId +
                                                           "]. Problem occured in setting status true. ");

                                    mic.Message = "Verify Phone Failed unfortunately :-(";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    resInnerClass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Contact no. not available ";
                                mic.NoochId = s;
                                resInnerClass.Add(mic);
                            }
                            #endregion
                        }
                    }

                    res.IsSuccess = true;
                    res.Message = "all operations performed";
                    res.MemberOperationsOuterClass = resInnerClass;
                }

                #endregion Verify Phone


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
                                    resInnerClass.Add(mic);
                                }
                                catch (Exception)
                                {
                                    // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                    Logger.Error("ActivateAccount - Attempt to ActivateAccount Member failed [" + member.MemberId +
                                                           "]. Problem occured in setting status Active. ");

                                    mic.Message = "Member activation Failed!";
                                    mic.NoochId = member.Nooch_ID;
                                    mic.IsSuccess = false;
                                    resInnerClass.Add(mic);
                                }
                            }
                            else
                            {
                                mic.Message = "Member already Active ";
                                mic.NoochId = s;
                                resInnerClass.Add(mic);
                            }
                            #endregion
                        }
                    }

                    res.IsSuccess = true;
                    res.Message = "all operations performed";
                    res.MemberOperationsOuterClass = resInnerClass;
                }

                #endregion activate account - verify email


                #region Delete User

                if (operation == "5")
                {
                    // Delete User
                    try
                    {
                        foreach (string s in allNoochIdsChoosen)
                        {
                            Logger.Info("MemberController -> ApplyOperation - Delete User Block Fired - Nooch_ID: [" + s + "]");

                            using (NOOCHEntities obj = new NOOCHEntities())
                            {
                                var member = (from t in obj.Members
                                              where t.Nooch_ID == s
                                              select t).FirstOrDefault();

                                MemberOperationsInnerClass mic = new MemberOperationsInnerClass();
                                mic.IsSuccess = false;

                                #region IfMemberNotNull

                                if (member != null)
                                {
                                    if (member.IsDeleted != true)
                                    {
                                        try
                                        {
                                            member.Status = "Deleted";
                                            member.IsDeleted = true;
                                            member.DateModified = DateTime.Now;
                                            member.ModifiedBy = Utility.ConvertToGuid(Session["UserId"].ToString());
                                            obj.SaveChanges();

                                            Logger.Error("MemberController -> ApplyOperation - Member Marked Deleted - MemberID:" + member.MemberId + "]");

                                            mic.Message = "Member Deleted Successfully";
                                            mic.NoochId = member.Nooch_ID;
                                            mic.IsSuccess = true;
                                            resInnerClass.Add(mic);
                                        }
                                        catch (Exception ex)
                                        {
                                            // to return the status without inserting records in notifications or friends tables, when mail is not sent successfully.
                                            Logger.Error("MemberController -> ApplyOperation - Attempt to delete user failed - MemberID: [" + member.MemberId +
                                                         "], Exception: [" + ex.Message + "]");

                                            mic.Message = ex.Message;
                                            mic.NoochId = member.Nooch_ID;
                                        }
                                    }
                                    else
                                    {
                                        mic.Message = "Member already deleted";
                                        mic.NoochId = s;
                                    }
                                }
                                else
                                {
                                    mic.Message = "Member not found";
                                    mic.NoochId = s;
                                }

                                resInnerClass.Add(mic);

                                #endregion
                            }
                        }

                        res.IsSuccess = true;
                        res.Message = "all operations performed";
                        res.MemberOperationsOuterClass = resInnerClass;
                    }
                    catch (Exception ex)
                    {
                        Logger.Info("MemberController - ApplyOperation - Delete User Block FAILED - Exception: [" + ex + "]");
                    }
                }

                #endregion Delete User

                return Json(res);
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "Error";

                Logger.Info("MemberController - ApplyOperation FAILED - Exception: [" + ex.Message + "]");

                return Json(res);
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
                                      where (tr.TransactionStatus == "Success" &&
                                             (tr.SenderId == m.MemberId || tr.RecipientId == m.MemberId ||
                                             tr.InvitationSentTo == m.UserName || tr.InvitationSentTo == m.UserNameLowerCase ||
                                             tr.InvitationSentTo == m.SecondaryEmail))
                                      select tr).Count();

                    // transfer - 5dt4HUwCue532sNmw3LKDQ==
                    // invite   - DrRr1tU1usk7nNibjtcZkA==
                    // request  - T3EMY1WWZ9IscHIj3dbcNw==

                    var totalAmountSent = (from tr in obj.Transactions
                                           where tr.TransactionStatus == "Success" &&
                                               //(tr.TransactionType == "5dt4HUwCue532sNmw3LKDQ==" || tr.TransactionType == "DrRr1tU1usk7nNibjtcZkA==") &&
                                                (tr.SenderId == m.MemberId ||
                                                (tr.TransactionType == "T3EMY1WWZ9IscHIj3dbcNw==" &&
                                                 tr.InvitationSentTo == m.UserName ||
                                                 tr.InvitationSentTo == m.UserNameLowerCase))
                                           select tr.Amount
                        ).Sum(tr => (decimal?)tr) ?? 0;

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

                    mdc.TotalAmountSent = mdc.TotalAmountSent != "0" ? String.Format("{0:0.00}", totalAmountSent) : "0";
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

            Logger.Info("Admin Member Cntrlr - Detail Fired - Nooch_ID: [" + NoochId + "]");

            try
            {
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
                        if (String.IsNullOrEmpty(mdc.FBID) && !String.IsNullOrEmpty(Member.FacebookUserId)) mdc.FBID = Member.FacebookUserId;
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
                        mdc.PinEncrypted = Member.PinNumber;
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
                        mdc.isRentScene = Member.isRentScene ?? false;

                        //Get the Refered Code Used
                        mdc.ReferCodeUsed = (from Membr in obj.Members
                                             join Code in obj.InviteCodes
                                             on Membr.InviteCodeIdUsed equals Code.InviteCodeId
                                             where Membr.Nooch_ID == NoochId
                                             select Code.code).SingleOrDefault();

                        #region Get User's Transactions

                        try
                        {
                            var memberFullName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.FirstName)) + " " +
                                                 CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(Member.LastName));

                            var transList = (from t in obj.Transactions
                                             where (t.SenderId == Member.MemberId ||
                                                    t.RecipientId == Member.MemberId ||
                                                    t.Member.MemberId == Member.MemberId ||
                                                    t.InvitationSentTo == Member.UserName ||
                                                    t.InvitationSentTo == Member.UserNameLowerCase ||
                                                    t.InvitationSentTo == Member.SecondaryEmail)
                                             select t).OrderByDescending(r => r.TransactionDate).Take(40).ToList();

                            List<MemberDetailsTrans> mm = new List<MemberDetailsTrans>();

                            if (transList != null)
                            {
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

                                    if (!String.IsNullOrEmpty(t.SynapseStatus))
                                    {
                                        payment.SynapseStatus = t.SynapseStatus.ToString();

                                        var transIdString = t.TransactionId.ToString().ToLower();

                                        // Get the note for the most recent Synapse Status
                                        var lastStatusObj = (from statusObj in obj.TransactionsStatusAtSynapses
                                                             where statusObj.Nooch_Transaction_Id == transIdString
                                                             select statusObj).OrderByDescending(n => n.Id).FirstOrDefault();

                                        payment.SynapseStatusNote = lastStatusObj != null && !String.IsNullOrEmpty(lastStatusObj.status_note) ? lastStatusObj.status_note : "Note Not Found";
                                    }
                                    else
                                        payment.SynapseStatus = "-";

                                    if (String.IsNullOrEmpty(t.InvitationSentTo) && t.IsPhoneInvitation != true)
                                    {
                                        // Payment Request or Transfer to an EXISTING Nooch user... straight forward.
                                        payment.SenderId = t.SenderId.ToString();
                                        payment.SenderName = CommonHelper.GetDecryptedData(t.Member.FirstName) + " " + CommonHelper.GetDecryptedData(t.Member.LastName);
                                        payment.SenderNoochId = t.Member.Nooch_ID;

                                        payment.RecipientId = t.RecipientId.ToString();
                                        payment.RecipientName = CommonHelper.GetDecryptedData(t.Member1.FirstName) + " " + CommonHelper.GetDecryptedData(t.Member1.LastName);
                                        payment.RecipientNoochId = t.Member1.Nooch_ID;
                                    }
                                    else // REQUEST OR INVITE TO NON-NOOCH USER
                                    {
                                        var existingMembersName = CommonHelper.GetDecryptedData(t.Member.FirstName) + " " +
                                                                  CommonHelper.GetDecryptedData(t.Member.LastName);

                                        // Request/Invite via Email
                                        if (t.TransactionStatus == "Success")
                                        {
                                            #region New User Has Accepted & Has A Nooch Account

                                            Member newMember = new Member();

                                            if (!String.IsNullOrEmpty(t.InvitationSentTo))
                                            {
                                                // Payment was Accepted, so the invited member must have created a Nooch account.
                                                newMember = CommonHelper.GetMemberDetailsByUsername(CommonHelper.GetDecryptedData(t.InvitationSentTo));
                                            }
                                            else if (!String.IsNullOrEmpty(t.PhoneNumberInvited))
                                            {
                                                newMember = CommonHelper.GetMemberDetailsByPhone(CommonHelper.GetDecryptedData(t.PhoneNumberInvited));
                                            }

                                            if (newMember != null)
                                            {
                                                var invitedMembersName = CommonHelper.GetDecryptedData(newMember.FirstName) + " " +
                                                                         CommonHelper.GetDecryptedData(newMember.LastName);

                                                if (payment.TransactionType == "Request")
                                                {
                                                    payment.SenderName = invitedMembersName;
                                                    payment.SenderId = newMember.MemberId.ToString();
                                                    payment.SenderNoochId = newMember.Nooch_ID;

                                                    payment.RecipientName = existingMembersName;
                                                    payment.RecipientId = t.RecipientId.ToString();
                                                    payment.RecipientNoochId = t.Member.Nooch_ID;
                                                }
                                                else
                                                {
                                                    payment.SenderName = existingMembersName;
                                                    payment.SenderId = t.SenderId.ToString();
                                                    payment.SenderNoochId = t.Member.Nooch_ID;

                                                    payment.RecipientName = invitedMembersName;
                                                    payment.RecipientId = newMember.MemberId.ToString();
                                                    payment.RecipientNoochId = newMember.Nooch_ID;
                                                }
                                            }

                                            #endregion New User Has Accepted & Has A Nooch Account
                                        }
                                        else
                                        {
                                            // Payment was not (yet) Accepted, so the invited member does NOT have a Nooch account,
                                            // so use the invited email / phone

                                            payment.SenderName = existingMembersName;
                                            payment.SenderId = t.SenderId.ToString();
                                            payment.SenderNoochId = t.Member.Nooch_ID;

                                            payment.RecipientName = !String.IsNullOrEmpty(t.PhoneNumberInvited)
                                                                ? CommonHelper.FormatPhoneNumber(CommonHelper.GetDecryptedData(t.PhoneNumberInvited))
                                                                : CommonHelper.GetDecryptedData(t.InvitationSentTo);
                                            payment.RecipientId = null;
                                            payment.RecipientNoochId = null;
                                        }
                                    }

                                    mm.Add(payment);
                                }
                            }

                            mdc.Transactions = mm;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Admin Member Cntrlr - Detail - FAILED attempting to lookup user's transactions - Exception: [" + ex + "]");
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

                        mdc.DisputeTransactions = mm2;

                        #endregion Get Last 5 Disputes


                        #region Stats-Related Operations

                        // Cliff (5/24/16): These need updating... they aren't counting initial payments when the user is created from a request/invite.

                        MemberDetailsStats ms = new MemberDetailsStats();

                        ms.TotalTransfer = obj.Transactions.Where(t => t.TransactionStatus == "Success" &&
                                                                      (t.SenderId == Member.MemberId ||
                                                                       t.RecipientId == Member.MemberId ||
                                                                       t.InvitationSentTo == Member.UserName ||
                                                                       t.InvitationSentTo == Member.UserNameLowerCase ||
                                                                       t.InvitationSentTo == Member.SecondaryEmail))
                                                                       .Count().ToString();

                        decimal decTotalSent = obj.Transactions.Where(t => t.TransactionStatus == "Success" &&
                                                                          (t.SenderId == Member.MemberId ||
                                                                           t.InvitationSentTo == Member.UserName ||
                                                                           t.InvitationSentTo == Member.UserNameLowerCase ||
                                                                           t.InvitationSentTo == Member.SecondaryEmail))
                                                                           .Select(t => t.Amount).Sum(t => (decimal?)t) ?? 0;

                        string TotalSent = decTotalSent.ToString();
                        ms.TotalSent = TotalSent != "0" ? String.Format("{0:###,###.##}", Convert.ToDecimal(TotalSent)) : "0";

                        decimal decLargestSent = obj.Transactions.Where(t => t.TransactionStatus == "Success" &&
                                                                            (t.SenderId == Member.MemberId ||
                                                                             t.InvitationSentTo == Member.UserName ||
                                                                             t.InvitationSentTo == Member.UserNameLowerCase ||
                                                                             t.InvitationSentTo == Member.SecondaryEmail))
                                                                             .Select(t => t.Amount).Max(t => (decimal?)t) ?? 0;
                        string LargestSent = decLargestSent.ToString();
                        ms.LargestSent = LargestSent != "0" ? String.Format("{0:###,###.##}", Convert.ToDecimal(LargestSent)) : "0";

                        decimal totalReceivedDec = obj.Transactions.Where(t => t.TransactionStatus == "Success" &&
                                                                              (t.RecipientId == Member.MemberId ||
                                                                              (t.TransactionType == "DrRr1tU1usk7nNibjtcZkA==" &&
                                                                              (t.InvitationSentTo == Member.UserName ||
                                                                               t.InvitationSentTo == Member.UserNameLowerCase ||
                                                                               t.InvitationSentTo == Member.SecondaryEmail))))
                                                                               .Select(t => t.Amount).Sum(t => (decimal?)t) ?? 0;

                        string TotalReceived = totalReceivedDec.ToString();
                        ms.TotalReceived = TotalReceived != "0" ? String.Format("{0:###,###.##}", Convert.ToDecimal(TotalReceived)) : "0";

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
                            synapseDetail.synapseUserName = !String.IsNullOrEmpty(synapseCreateUserObj.username)
                                                            ? CommonHelper.GetDecryptedData(synapseCreateUserObj.username)
                                                            : "NO USERNAME FOUND";
                            synapseDetail.synapseUserId = synapseCreateUserObj.user_id.ToString();
                            synapseDetail.isBusiness = synapseCreateUserObj.is_business ?? false;
                            synapseDetail.userPermission = synapseCreateUserObj.permission;
                            synapseDetail.photos = synapseCreateUserObj.photos;
                            synapseDetail.physical_doc = synapseCreateUserObj.physical_doc;
                            synapseDetail.virtual_doc = synapseCreateUserObj.virtual_doc;
                            synapseDetail.extra_security = synapseCreateUserObj.extra_security;

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
                                synapseDetail.synapseBankId = CommonHelper.GetDecryptedData(synapseBankDetails.oid);
                                synapseDetail.IsAddedUsingRoutingNumber = synapseBankDetails.IsAddedUsingRoutingNumber ?? false;
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
            }
            catch (Exception ex)
            {
                Logger.Error("Admin Member Cntrlr - Detail FAILED - Nooch_ID: [" + NoochId + "], Exception: [" + ex + "]");
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
        public ActionResult EditMemberDetails(string contactno, string streetaddress, string city, string secondaryemail, string recoveryemail, string noochid, string state, string zip, string ssn, string dob, string transferLimit, string cip_tag)
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
                            if (!String.IsNullOrEmpty(cip_tag))
                            {
                                member.cipTag = cip_tag;
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
        public ActionResult UpdatePassword(string noochId, string newPassword, bool sendEmail)
        {
            MemberOperationsResult res = new MemberOperationsResult();
            res.IsSuccess = false;

            try
            {
                using (NOOCHEntities obj = new NOOCHEntities())
                {
                    var temp = CommonHelper.GetRandomTransactionTrackingId();

                    var member = (from t in obj.Members
                                  where t.Nooch_ID == noochId && t.IsDeleted == false
                                  select t).SingleOrDefault();

                    if (member != null)
                    {
                        member.Password = CommonHelper.GetEncryptedData(newPassword);
                        var save = obj.SaveChanges();

                        if (save > 0)
                        {
                            res.Message = "Password updated";
                            res.IsSuccess = true;

                            if (sendEmail)
                            {
                                var fromAddress = Utility.GetValueFromConfig("adminMail");
                                string fname = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(member.FirstName));

                                var tokens = new Dictionary<string, string>
                                {
                                    {Constants.PLACEHOLDER_FIRST_NAME, fname},
                                    {Constants.PLACEHOLDER_PASSWORD, CommonHelper.GetDecryptedData( member.Password)}
                                };

                                string toAddress = CommonHelper.GetDecryptedData(member.UserName);
                                bool emailSent = Utility.SendEmail("passwordChangedByAdmin", fromAddress, toAddress,
                                                                   "Your Nooch password has been changed", null,
                                                                    tokens, null, null, null);

                                if (emailSent)
                                    res.Message += " & email notification sent to the user";
                            }
                            else
                            {
                                res.Message += " - no email sent to user";
                            }
                        }
                    }
                    else
                    {
                        res.Message = "User not found";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr -> UpdatePassword FAILED - NoochID: [" + noochId + "], Exception: [" + ex.Message + "]");
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

            DocumentDetails.IsPdf = false;
            string pic = "";
            ResizeSettings resizeSetting = new ResizeSettings
            {
                Width = 500,
                Height = 500,
                Format = "png"
            };

            using (var noochConnection = new NOOCHEntities())
            {
                var member = noochConnection.Members.Where(m => m.Nooch_ID == NoochId).FirstOrDefault();
                DocumentDetails.MemberId = member.MemberId.ToString();
                
             
                var memGuid = member.MemberId;
                var SynapseCreateUserResult = noochConnection.SynapseCreateUserResults.Where(m => m.MemberId == memGuid).FirstOrDefault();

                if (SynapseCreateUserResult != null)
                {
                    DocumentDetails.AccessToken = SynapseCreateUserResult.access_token;

                    if (file.ContentType.ToLower() == "application/pdf")
                    {
                        pic = memGuid.ToString() + ".pdf";
                        DocumentDetails.IsPdf = true;
                    }
                    else
                    {
                        pic = memGuid.ToString() + ".png";                       
                    }

                    // CLIFF (6/10/16): THIS WAS SAVING TO A FOLDER IN THE 'noochnewadmin' PROJECT, BUT IT NEEDS TO BE IN noochservices
                    //                  SINCE THAT'S WHERE ALL OTHER USER'S DOCS ARE SAVED AND IT'S WHERE THERE SERVER EXPECTS TO FIND THE DOC
                    //                  WHEN SUBMITTING TO SYNAPSE.  I TRIED TO FIX THIS BELOW BUT CAUSED AN ERROR:
                    // string path = System.IO.Path.Combine(Server.MapPath("~/UploadedPhotos/SynapseDocuments"), pic);
                    // string path = "C:\\nooch_new_architecture\\Nooch\\Nooch.API\\UploadedPhotos\\SynapseIdDocs\\" + pic;
                     string path = "C:\\noochweb.venturepact.com\\noochservice\\UploadedPhotos\\SynapseIdDocs\\" + pic;
                    // file is uploaded

                    file.SaveAs(path);
                    DocumentDetails.imgPath = path;

                    if (DocumentDetails.IsPdf != true)
                    ImageBuilder.Current.Build(path, path, resizeSetting);  


                    member.VerificationDocumentPath = Utility.GetValueFromConfig("SynapseUploadedDocPhotoUrl") + pic;
                   //member.VerificationDocumentPath = path;
                    noochConnection.SaveChanges();

                    //synapseV3GenericResponse submitDocToSynapseRes = submitDocumentToSynapseV3(DocumentDetails);
                    synapseV3GenericResponse submitDocToSynapseRes = sendDocsToSynapseV3(DocumentDetails.MemberId);

                    if (submitDocToSynapseRes.isSuccess == true)
                        Session["status"] = "Success";
                    else
                        Session["status"] = "Failed";
                }
                else
                    Session["status"] = "Failed";

                return RedirectToAction("Detail", "Member", new { NoochId = NoochId });
            }
        }


        /// <summary>
        /// CC (8/6/16): NOT USING ANYMORE AFTER UPDATIG TO USING 'sendDocsToSynapseV3()' FOR SUBMITTING ALL DOCS AT ONE TIME
        /// </summary>
        /// <param name="DocumentDetails"></param>
        /// <returns></returns>
        public synapseV3GenericResponse submitDocumentToSynapseV3(SaveVerificationIdDocument DocumentDetails)
        {
            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            try
            {
                Logger.Info("Member Cntrlr  -> submitDocumentToSynapseV3 - MemberID: [" + DocumentDetails.MemberId + "]");

                string ImageUrlMade = Utility.GetValueFromConfig("SynapseUploadedDocPhotoUrl");

                if (DocumentDetails.imgPath != "")
                {
                    if (DocumentDetails.IsPdf == true)
                        ImageUrlMade += DocumentDetails.MemberId + ".pdf";
                    else
                        ImageUrlMade += DocumentDetails.MemberId + ".png";
                }
                else
                    ImageUrlMade += "gv_no_photo.png";

                res = submitDocumentToSynapseV3(DocumentDetails.MemberId, ImageUrlMade);
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr  - submitDocumentToSynapseV3 FAILED - MemberID: [" + DocumentDetails.MemberId + "], Exception: [" + ex + "]");
                res.msg = ex.Message;
            }

            return res;
        }


        /// <summary>
        /// CC (8/6/16): NOT USING ANYMORE AFTER UPDATIG TO USING 'sendDocsToSynapseV3()' FOR SUBMITTING ALL DOCS AT ONE TIME
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="ImageUrl"></param>
        /// <returns></returns>
        public synapseV3GenericResponse submitDocumentToSynapseV3(string MemberId, string ImageUrl)
        {
            Logger.Info("Member Cntrlr -> submitDocumentToSynapseV3 Initialized - MemberID: [" + MemberId + "], ImageUrl: [" + ImageUrl + "]");

            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            try
            {
                Guid id = Utility.ConvertToGuid(MemberId);

                using (var noochConnection = new NOOCHEntities())
                {
                    #region Get User's Synapse OAuth Consumer Key

                    string usersSynapseOauthKey = "";

                    SynapseCreateUserResult synapseUserObj = new SynapseCreateUserResult();
                    synapseUserObj = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == id && m.IsDeleted == false);

                    if (synapseUserObj == null)
                    {
                        Logger.Error("Member Cntrlr-> submitDocumentToSynapseV3 ABORTED: Member's Synapse User Details not found. [MemberId: " + MemberId + "]");
                        res.msg = "Could not find this member's account info";
                        return res;
                    }
                    else
                    {
                        #region Check If OAuth Key Still Valid

                        synapseV3checkUsersOauthKey checkTokenResult = CommonHelper.refreshSynapseV3OautKey(synapseUserObj.access_token);

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
                            Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 FAILED on Checking User's Synapse OAuth Token - " +
                                         "CheckTokenResult was NULL, MemberID: [" + MemberId + "]");
                            res.msg = "Unable to check user's Oauth Token";
                        }

                        #endregion Check If OAuth Key Still Valid
                    }

                    #endregion Get User's Synapse OAuth Consumer Key


                    #region Get User's Fingerprint

                    string usersFingerprint = "";

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
                            Logger.Info("Member Cntrlr -> submitDocumentToSynapseV3 ABORTED: Member's Fingerprint (UDID1) not found - MemberID: [" + MemberId + "]");
                            res.msg = "Could not find this member's fingerprint";
                            return res;
                        }
                        else
                        {
                            usersFingerprint = member.UDID1;
                        }
                    }

                    #endregion Get User's Fingerprint


                    #region Call Synapse /user/doc/attachments/add API

                    try
                    {
                        synapseAddDocsV3InputClass submitDocObj = new synapseAddDocsV3InputClass();

                        SynapseV3Input_login login = new SynapseV3Input_login();
                        login.oauth_key = usersSynapseOauthKey;
                        submitDocObj.login = login;

                        synapseAddDocsV3InputClass_user user = new synapseAddDocsV3InputClass_user();
                        submitDocToSynapse_user_doc doc = new submitDocToSynapse_user_doc();
                        string Extension = Path.GetExtension(ImageUrl);

                        if (Extension == ".pdf")
                            doc.attachment = "data:text/csv;base64," + CommonHelper.ConvertImageURLToBase64(ImageUrl).Replace("\\", "");
                        else if (Extension == ".png")
                            doc.attachment = "data:image/png;base64," + CommonHelper.ConvertImageURLToBase64(ImageUrl).Replace("\\", "");
                        else if (Extension == ".jpg")
                            doc.attachment = "data:image/jpg;base64," + CommonHelper.ConvertImageURLToBase64(ImageUrl).Replace("\\", "");
                        user.fingerprint = usersFingerprint;
                        //user.doc = doc;
                        submitDocObj.user = user;

                        Logger.Info("Member Cntrlr -> submitDocumentToSynapseV3 - Payload to submit to Synapse: Fingerprint: [" + user.fingerprint +
                                    "], Oauth Key: [" + login.oauth_key + "]");

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
                                var permission = resFromSynapse.user.permission ?? "NOT FOUND";
                                var physDoc = "NOT FOUND";
                                var virtualDoc = "NOT FOUND";

                                if (resFromSynapse.user.doc_status != null)
                                {
                                    physDoc = resFromSynapse.user.doc_status.physical_doc;
                                    virtualDoc = resFromSynapse.user.doc_status.virtual_doc;
                                }

                                // Update User's "Permission" in SynapseCreateUserResults Table

                                synapseUserObj.permission = resFromSynapse.user.permission.ToString();
                                synapseUserObj.photos = ImageUrl;

                                synapseUserObj.physical_doc = resFromSynapse.user.doc_status.physical_doc;
                                synapseUserObj.virtual_doc = resFromSynapse.user.doc_status.virtual_doc;

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
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr -> submitDocumentToSynapseV3 FAILED - Outer Exception - " +
                             "MemberID: [" + MemberId + "], Exception: [" + ex.Message + "]");
            }
            return res;
        }



        /// <summary>
        /// For sending all user 'Documents' (Virtual, Physical, Social) to Synapse.
        /// ADDED BY CLIFF (8/6/16)
        /// </summary>
        /// <param name="MemberId"></param>
        /// <returns></returns>
        public static synapseV3GenericResponse sendDocsToSynapseV3(string MemberId)
        {
            Logger.Info("Common Helper -> sendDocsToSynapseV3 Fired - MemberID: [" + MemberId + "]");

            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            var id = Utility.ConvertToGuid(MemberId);

            var memberEntity = CommonHelper.GetMemberDetails(MemberId);

            if (memberEntity != null)
            {
                var userNameDecrypted = CommonHelper.GetDecryptedData(memberEntity.UserName);

                #region Check User For All Required Data

                string usersFirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(memberEntity.FirstName));
                string usersLastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(memberEntity.LastName));

                string usersAddress = "";
                string usersCity = "";
                string usersState = "";
                string usersZip = "";

                DateTime usersDob;
                int usersDobDay = 0;
                int usersDobMonth = 0;
                int usersDobYear = 0;

                string usersPhone = "";
                string usersSsn = "";
                string usersFBID = "";
                string usersPhotoIDurl = "";

                bool hasSSN = false;
                bool hasFBID = false;
                bool hasPhotoID = false;

                string usersSynapseOauthKey = "";
                string usersFingerprint = "";
                string ipAddress = CommonHelper.GetRecentOrDefaultIPOfMember(id);

                using (var noochConnection = new NOOCHEntities())
                {
                    try
                    {
                        #region Initial Data Checks

                        bool isMissingSomething = false;
                        // Member found, now check that they have added
                        // • full Address (including city, zip),
                        // • SSN  *OR*  FB User ID
                        // • DOB
                        // • Fingerprint (UDID1)

                        // Check for Fingerprint (UDID1 in the database)
                        if (String.IsNullOrEmpty(memberEntity.UDID1))
                        {
                            isMissingSomething = true;
                            res.msg = " - Missing UDID";
                        }
                        else
                            usersFingerprint = memberEntity.UDID1; // (Not Encrypted)

                        // Check for Phone
                        if (String.IsNullOrEmpty(memberEntity.ContactNumber))
                        {
                            isMissingSomething = true;
                            res.msg += " - Missing Phone";
                        }
                        else
                            usersPhone = memberEntity.ContactNumber; // (Not Encrypted)

                        // Check for Address
                        if (String.IsNullOrEmpty(memberEntity.Address))
                        {
                            isMissingSomething = true;
                            res.msg += " - Missing Address";
                        }
                        else
                            usersAddress = CommonHelper.GetDecryptedData(memberEntity.Address); // (Encrypted)

                        #region Set City, State, ZIP

                        // Check for ZIP
                        if (String.IsNullOrEmpty(memberEntity.Zipcode))
                        {
                            isMissingSomething = true;
                            res.msg += " - Missing ZIP";
                        }
                        else
                            usersZip = CommonHelper.GetDecryptedData(memberEntity.Zipcode); // (Encrypted)

                        // Check for City
                        if (String.IsNullOrEmpty(memberEntity.City))
                        {
                            // Missing City, so if user has a ZIP, try getting the City & States from Google
                            if (!String.IsNullOrEmpty(usersZip))
                            {
                                var googleMapsRes = CommonHelper.GetCityAndStateFromZip(usersZip);
                                if (googleMapsRes != null && !String.IsNullOrEmpty(googleMapsRes.city))
                                {
                                    usersCity = googleMapsRes.city;
                                    usersState = googleMapsRes.stateAbbrev;
                                }
                                else
                                {
                                    isMissingSomething = true;
                                    res.msg += " - Missing City";
                                }
                            }
                            else
                            {
                                isMissingSomething = true;
                                res.msg += " - Missing City and ZIP";
                            }
                        }
                        else
                            usersCity = CommonHelper.GetDecryptedData(memberEntity.City); // (Encrypted)

                        if (String.IsNullOrEmpty(usersState))
                        {
                            if (String.IsNullOrEmpty(memberEntity.State))
                            {
                                // Missing State, so if user does have a ZIP, try getting the City & States from Google
                                if (!String.IsNullOrEmpty(usersZip))
                                {
                                    var googleMapsRes = CommonHelper.GetCityAndStateFromZip(usersZip);
                                    if (googleMapsRes != null && !String.IsNullOrEmpty(googleMapsRes.stateAbbrev))
                                        usersState = googleMapsRes.stateAbbrev;
                                }
                            }
                            else
                                usersState = CommonHelper.GetDecryptedData(memberEntity.State);
                        }

                        #endregion Set City, State


                        #region Set Date Of Birth

                        if (memberEntity.DateOfBirth == null)
                        {
                            isMissingSomething = true;
                            res.msg += " MDA - Missing Date of Birth";
                        }
                        else
                        {
                            usersDob = Convert.ToDateTime(memberEntity.DateOfBirth); // (Not Encrypted)
                            // We have DOB, now we must parse it into day, month, & year
                            usersDobDay = usersDob.Day;
                            usersDobMonth = usersDob.Month;
                            usersDobYear = usersDob.Year;
                        }

                        #endregion Set Date Of Birth


                        #region Check for SSN & FBID

                        if (!String.IsNullOrEmpty(memberEntity.SSN))
                        {
                            usersSsn = CommonHelper.GetDecryptedData(memberEntity.SSN); // (Encrypted)
                            hasSSN = true;
                        }
                        if (!String.IsNullOrEmpty(memberEntity.FacebookUserId) && memberEntity.FacebookUserId != "not connected")
                        {
                            usersFBID = memberEntity.FacebookUserId; // (Not Encrypted)
                            hasFBID = true;
                        }

                        #endregion Check for SSN & FBID

                        // Now check for ID verification document (Checking the one in the Members Table here -
                        // could also be an ID img in SynapseCreateUserResults table, which is checked for later in this method)
                        if (!String.IsNullOrEmpty(memberEntity.VerificationDocumentPath))
                        {
                            usersPhotoIDurl = memberEntity.VerificationDocumentPath;
                            hasPhotoID = true;
                        }

                        // Return if any data was missing in previous block
                        if (isMissingSomething)
                        {
                            Logger.Error("Common Helper -> sendDocsToSynapseV3 ABORTED: Member missing required info - Username: [" + userNameDecrypted + "], Message: [" + res.msg + "]");
                            return res;
                        }

                        #endregion Initial Data Checks

                        // Update Member's DB record from NULL to false (update to true later on if Verification from Synapse is completely successful)
                        if (memberEntity.IsVerifiedWithSynapse == null)
                        {
                            Logger.Info("Common Helper -> sendDocsToSynapseV3 - Setting IsVerifiedWithSynapse to FALSE since it was NULL: Username: [" + userNameDecrypted + "]");
                            memberEntity.IsVerifiedWithSynapse = false;
                        }



                        // Now check if user already has a Synapse User account (would have a record in SynapseCreateUserResults.dbo)
                        var usersSynapseDetails = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == id &&
                                                                                                          m.IsDeleted == false);

                        if (usersSynapseDetails != null)
                        {
                            noochConnection.Entry(usersSynapseDetails).Reload();
                            usersSynapseOauthKey = CommonHelper.GetDecryptedData(usersSynapseDetails.access_token);

                            // Now check again for ID verification document, now in SynapseCreateUserResults table
                            if (!String.IsNullOrEmpty(usersSynapseDetails.photos))
                            {
                                Logger.Info("Common Helper -> sendDocsToSynapseV3 - Found Photo in SynapseCreateUserResults Table - PhotoURL: [" + usersSynapseDetails.photos + "]");
                                usersPhotoIDurl = usersSynapseDetails.photos; // Override the img from the Member's table if an img is found here - this one would have been set from the landing pages.
                                hasPhotoID = true;
                            }
                        }
                        else
                        {
                            Logger.Error("Common Helper -> sendDocsToSynapseV3 ABORTED: Member's Synapse User Details not found - Username: [" + userNameDecrypted +
                                         "], MemberID: [" + MemberId + "]");

                            res.msg = "Users synapse details not found";
                            return res;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Common Helper -> sendDocsToSynapseV3 FAILED on checking for all required data - Username: [" +
                                      userNameDecrypted + "], Exception: [" + ex + "]");
                    }

                    Logger.Info("Common Helper -> sendDocsToSynapseV3 - Completed all initial data checks - All Data Found!");

                #endregion Check User For All Required Data


                    #region Send All Docs To Synapse

                    try
                    {
                        #region Call Synapse V3 /user/doc/add API

                        synapseAddDocsV3InputClass synapseAddDocsV3Input = new synapseAddDocsV3InputClass();

                        SynapseV3Input_login login = new SynapseV3Input_login();
                        login.oauth_key = usersSynapseOauthKey;

                        synapseAddDocsV3InputClass_user_docs documents = new synapseAddDocsV3InputClass_user_docs();
                        documents.email = userNameDecrypted;
                        documents.phone_number = memberEntity.ContactNumber;
                        documents.ip = ipAddress;
                        documents.name = usersFirstName + " " + usersLastName;
                        documents.alias = usersFirstName + " " + usersLastName;
                        documents.entity_type = "NOT_KNOWN";
                        documents.entity_scope = memberEntity.isRentScene == true ? "Real Estate" : "Not Known";
                        documents.day = usersDobDay;
                        documents.month = usersDobMonth;
                        documents.year = usersDobYear;
                        documents.address_street = usersAddress;
                        documents.address_city = usersCity;
                        documents.address_subdivision = usersState; // State
                        documents.address_postal_code = usersZip;
                        documents.address_country_code = "US";


                        #region Set All Document Values

                        try
                        {
                            Logger.Info("Common Helper -> sendDocsToSynapseV3 - About to attempt to set Document Values - " +
                                        " hasSSN: [" + hasSSN + "], hasFBID: [" + hasFBID + "], hasPhotoID: [" + hasPhotoID + "], MemberID: [" + MemberId + "]");

                            // VIRTUAL DOCS: Synapse lists 6 acceptable "virtual_docs" types: SSN, PASSPORT #, DRIVERS_LICENSE #, PERSONAL_IDENTIFICATION # (not sure what this is), TIN #, DUNS #
                            //               But we are only going to use SSN. For any business users, we will also need to use TIN (Tax ID) #.
                            synapseAddDocsV3InputClass_user_docs_doc virtualDocObj = new synapseAddDocsV3InputClass_user_docs_doc();
                            virtualDocObj.document_type = "SSN"; // This can also be "PASSPORT" or "DRIVERS_LICENSE"... we need to eventually support all 3 options (Rent Scene has international clients that don't have SSN but do have a Passport)
                            virtualDocObj.document_value = usersSsn; // Can also be the user's Passport # or DL #

                            documents.virtual_docs = new synapseAddDocsV3InputClass_user_docs_doc[1];
                            documents.virtual_docs[0] = virtualDocObj;

                            // If user has no SSN, still need to send an empty array to Synapse or else we get an error
                            if (!hasSSN)
                                documents.virtual_docs = documents.virtual_docs.Where(val => val.document_value != virtualDocObj.document_value).ToArray();

                            // SOCIAL DOCS: Send Facebook Profile URL by appending user's FBID to base FB URL
                            synapseAddDocsV3InputClass_user_docs_doc socialDocObj = new synapseAddDocsV3InputClass_user_docs_doc();
                            socialDocObj.document_type = "FACEBOOK";
                            socialDocObj.document_value = hasFBID ? "https://www.facebook.com/" + usersFBID : "-";

                            documents.social_docs = new synapseAddDocsV3InputClass_user_docs_doc[1];
                            documents.social_docs[0] = socialDocObj;

                            // If user has no FBID, still need to send an empty array to Synapse or else we get an error
                            if (!hasFBID)
                                documents.social_docs = documents.social_docs.Where(val => val.document_value != socialDocObj.document_value).ToArray();

                            // PHYSICAL DOCS: Send User's Photo ID if available
                            var dataType = "image/png";

                            if (hasPhotoID && !String.IsNullOrEmpty(usersPhotoIDurl))
                            {
                                if (usersPhotoIDurl.IndexOf(".jpg") > 10)
                                    dataType = "image/jpg";
                                else if (usersPhotoIDurl.IndexOf(".jpeg") > 10)
                                    dataType = "image/jpeg";
                                else if (usersPhotoIDurl.IndexOf(".pdf") > 10)
                                    dataType = "application/pdf";
                            }

                            synapseAddDocsV3InputClass_user_docs_doc physicalDocObj = new synapseAddDocsV3InputClass_user_docs_doc();
                            physicalDocObj.document_type = "GOVT_ID";
                            physicalDocObj.document_value = "data:" + dataType + ";base64," + CommonHelper.ConvertImageURLToBase64(usersPhotoIDurl).Replace("\\", "");
                            documents.physical_docs = new synapseAddDocsV3InputClass_user_docs_doc[1];
                            documents.physical_docs[0] = physicalDocObj;

                            if (!hasPhotoID)
                                documents.physical_docs = documents.physical_docs.Where(val => val.document_value != physicalDocObj.document_value).ToArray();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Common Helper -> sendDocsToSynapseV3 - Exception while setting Documents - MemberID: [" + MemberId +
                                         "], Exception: [" + ex + "]");
                        }

                        #endregion Set All Document Values


                        synapseAddDocsV3InputClass_user user = new synapseAddDocsV3InputClass_user();
                        user.fingerprint = usersFingerprint;

                        user.documents = new synapseAddDocsV3InputClass_user_docs[1];
                        user.documents[0] = documents;

                        synapseAddDocsV3Input.login = login;
                        synapseAddDocsV3Input.user = user;


                        string baseAddress = Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox"))
                                             ? "https://sandbox.synapsepay.com/api/v3/user/docs/add"
                                             : "https://synapsepay.com/api/v3/user/docs/add";


                        #region For Testing & Logging

                        if (memberEntity.MemberId.ToString().ToLower() == "b3a6cf7b-561f-4105-99e4-406a215ccf60") documents.name = "Clifford Satell";

                        try
                        {
                            Logger.Info("Common Helper -> sendDocsToSynapseV3 - About To Query Synapse (/v3/user/docs/add) -> Payload to send to Synapse: [OauthKey: " + login.oauth_key +
                                        "], Name: [" + documents.name + "], Email: [" + documents.email +
                                        "], Phone: [" + documents.phone_number + "], IP: [" + documents.ip +
                                        "], Alias: [" + documents.name + "], Entity_Type: [" + documents.entity_type +
                                        "], Entity_Scope: [" + documents.entity_scope + "], Day: [" + documents.day +
                                        "], Month: [" + documents.month + "], Year: [" + documents.year +
                                        "], address_street: [" + documents.address_street + "], Postal_code: [" + documents.address_postal_code +
                                        "], City: [" + documents.address_city + "], State: [" + documents.address_subdivision +
                                        "], country_code: [" + documents.address_country_code + "], Fingerprint: [" + user.fingerprint +
                                        "], HasSSN?: [" + hasSSN + "], SSN: [" + usersSsn + "], HasFBID?: [" + hasFBID +
                                        "], FBID: " + usersFBID + "], HasPhotoID?: " + hasPhotoID + "], BASE_ADDRESS: [" + baseAddress + "].");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Common Helper -> sendDocsToSynapseV3 - Couldn't log Synapse SSN Payload. Exception: [" + ex + "]");
                        }

                        #endregion For Testing & Logging

                        var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                        http.Accept = "application/json";
                        http.ContentType = "application/json";
                        http.Method = "POST";

                        string parsedContent = JsonConvert.SerializeObject(synapseAddDocsV3Input);

                        ASCIIEncoding encoding = new ASCIIEncoding();
                        Byte[] bytes = encoding.GetBytes(parsedContent);

                        Stream newStream = http.GetRequestStream();
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();

                        var response = http.GetResponse();
                        var stream = response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var content = sr.ReadToEnd();

                        addDocsResFromSynapse synapseResponse = JsonConvert.DeserializeObject<addDocsResFromSynapse>(content);

                        #endregion Call Synapse V3 /user/docs/add API


                        // NOW WE MUST PARSE THE SYNAPSE RESPONSE. THERE ARE 3 POSSIBLE SCENARIOS:
                        // 1.) Validation was successful - No further validation needed. Synapse returns {"success": true}
                        // 2.) Validation was PARTLY successful.  Need to do further validation.  Synapse returns: "success":true... 
                        //     plus an object "question_set", containing a series of questions and array of multiple choice answers for each question.
                        //     We will display the questions to the user via the IDVerification page (already built-in to the Add-Bank process)
                        //     NOTE: WITH NEW SYNAPSE METHOD, THE QUESTION_SET ONLY IS RETURNED IF WE SEND THE USER'S SSN. question_set will be 
                        //           in  ["user"]["documents"]["virtual_docs"]["meta"]
                        // 3.) Validation Failed:  Synapse will return HTTP Error 400 Bad Request
                        //     with an "error" object, and then a message in "error.en" that should be: "Invalid SSN information supplied. Request user to submit a copy of passport/divers license and SSN via user/doc/attachments/add"

                        #region Parse Synapse Response

                        if (synapseResponse != null)
                        {
                            JObject refreshResponse = JObject.Parse(content);
                            Logger.Info("Common Helper -> sendDocsToSynapseV3 - SYNAPSE RESPONSE IS: [" + refreshResponse + "]");

                            if (synapseResponse.success == true)
                            {
                                // Great, we have at least partial success

                                #region Update Permission in SynapseCreateUserResults Table

                                try
                                {
                                    // Update values in SynapseCreateUserResults table
                                    // Get existing record from dbo.SynapseCreateUserResults
                                    var synapseRes = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == id &&
                                                                                                             m.IsDeleted == false);

                                    if (synapseRes != null)
                                    {
                                        synapseRes.permission = synapseResponse.user.permission;
                                        synapseRes.physical_doc = synapseResponse.user.doc_status != null ? synapseResponse.user.doc_status.physical_doc : null;
                                        synapseRes.virtual_doc = synapseResponse.user.doc_status != null ? synapseResponse.user.doc_status.virtual_doc : null;
                                        synapseRes.extra_security = synapseResponse.user.extra.extra_security != null ? synapseResponse.user.extra.extra_security.ToString() : null;

                                        if (synapseResponse.user.documents != null &&
                                            synapseResponse.user.documents.Length > 0)
                                        {
                                            #region Loop Through Outer Documents Object (Should Only Be 1)

                                            foreach (addDocsResFromSynapse_user_docs doc in synapseResponse.user.documents)
                                            {
                                                // Check VIRTUAL_DOCS
                                                if (doc.virtual_docs != null && doc.virtual_docs.Length > 0)
                                                {
                                                    short n = 0;
                                                    foreach (addDocsResFromSynapse_user_docs_virtualdoc docObject in doc.virtual_docs)
                                                    {
                                                        n += 1;
                                                        Logger.Info("Common Helper -> sendDocsToSynapseV3 - VIRTUAL_DOC #[" + n + "] - Type: [" + docObject.document_type + "], Status: [" + docObject.status + "]");
                                                        if (docObject.document_type == "SSN")
                                                            synapseRes.virtual_doc = docObject.status;
                                                    }
                                                }

                                                // Check PHYSICAL_DOCS
                                                if (doc.physical_docs != null && doc.physical_docs.Length > 0)
                                                {
                                                    short n = 0;
                                                    foreach (addDocsResFromSynapse_user_docs_doc docObject in doc.physical_docs)
                                                    {
                                                        n += 1;
                                                        Logger.Info("Common Helper -> sendDocsToSynapseV3 - PHYSICAL_DOC #[" + n + "] - Type: [" + docObject.document_type + "], Status: [" + docObject.status + "]");
                                                        if (docObject.document_type == "GOVT_ID")
                                                            synapseRes.physical_doc = docObject.status;
                                                    }
                                                }

                                                // Check SOCIAL_DOCS
                                                if (doc.social_docs != null && doc.social_docs.Length > 0)
                                                {
                                                    short n = 0;
                                                    foreach (addDocsResFromSynapse_user_docs_doc docObject in doc.social_docs)
                                                    {
                                                        n += 1;
                                                        Logger.Info("Common Helper -> sendDocsToSynapseV3 - SOCIAL_DOC #[" + n + "] - Type: [" + docObject.document_type + "], Status: [" + docObject.status + "]");
                                                        if (docObject.document_type == "FACEBOOK")
                                                        {
                                                            // CC (8/5/16): Need to add DB fields to handle new Document Type of "Social"
                                                            //              Also for all 3 document types (Social, Virtual, Phsyical), we also need to store the "last updated" field to display in Admin Panel as a readable DateTime
                                                            //synCreateUserObject.virtual_doc = docObject.status;
                                                        }
                                                    }
                                                }

                                            }

                                            #endregion Loop Through Outer Documents Object (Should Only Be 1)
                                        }


                                        int save = noochConnection.SaveChanges();
                                        noochConnection.Entry(synapseRes).Reload();

                                        if (save > 0)
                                            Logger.Info("Common Helper -> sendDocsToSynapseV3 - SUCCESS response form Synapse - And Successfully updated user's record in SynapseCreateUserRes Table");
                                        else
                                            Logger.Error("Common Helper -> sendDocsToSynapseV3 - SUCCESS response form Synapse - But FAILED to update user's record in SynapseCreateUserRes Table");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Common Helper -> sendDocsToSynapseV3 - EXCEPTION on trying to update User's record in CreateSynapseUserResults Table - " +
                                                 "MemberID: [" + MemberId + "], Exception: [" + ex + "]");
                                }

                                #endregion Update Permission in CreateSynapseUserResults Table

                                // Now check if further verification is needed by checking if Synapse returned a 'question_set' object.
                                Logger.Info("Common Helper -> sendDocsToSynapseV3 - Synapse returned SUCCESS = TRUE. Now checking if additional Verification questions are required...");

                                res.isSuccess = true;

                                if (synapseResponse.user.documents != null &&
                                    synapseResponse.user.documents[0].virtual_docs != null &&
                                    synapseResponse.user.documents[0].virtual_docs.Length > 0 &&
                                    synapseResponse.user.documents[0].virtual_docs[0].meta != null)
                                {
                                    // Further Verification is needed...
                                    #region Additional Verification Questions Returned

                                    // Now make sure an Array[] set of 'questions' was returned (could be up to 5 questions, each with 5 possible answer choices)
                                    if (synapseResponse.user.documents[0].virtual_docs[0].meta.question_set != null)
                                    {
                                        Logger.Info("Common Helper -> sendDocsToSynapseV3 - Question_Set was returned, further validation will be needed. Saving ID Verification Questions...");

                                        // Saving these questions in DB.  
                                        // The user will have to answer these on the IDVerification page.
                                        // The Add-Bank page will direct the user either to the IDVerification page (via iFrame), or not if questions are not needed.

                                        // Loop through each question set (question/answers/id)
                                        #region Iterate Through Each Question And Save in DB

                                        addDocsResFromSynapse_user_docs_virtualdoc_meta_qset questionSetObj = synapseResponse.user.documents[0].virtual_docs[0].meta.question_set;

                                        foreach (synapseIdVerificationQuestionAnswerSet question in questionSetObj.questions)
                                        {
                                            SynapseIdVerificationQuestion questionForDb = new SynapseIdVerificationQuestion();
                                            questionForDb.MemberId = id;
                                            questionForDb.QuestionSetId = questionSetObj.id;
                                            questionForDb.DateCreated = DateTime.Now;
                                            questionForDb.submitted = false;

                                            questionForDb.SynpQuestionId = question.id;
                                            questionForDb.Question = question.question;

                                            questionForDb.Choice1Id = question.answers[0].id;
                                            questionForDb.Choice1Text = question.answers[0].answer;

                                            questionForDb.Choice2Id = question.answers[1].id;
                                            questionForDb.Choice2Text = question.answers[1].answer;

                                            questionForDb.Choice3Id = question.answers[2].id;
                                            questionForDb.Choice3Text = question.answers[2].answer;

                                            questionForDb.Choice4Id = question.answers[3].id;
                                            questionForDb.Choice4Text = question.answers[3].answer;

                                            questionForDb.Choice5Id = question.answers[4].id;
                                            questionForDb.Choice5Text = question.answers[4].answer;

                                            noochConnection.SynapseIdVerificationQuestions.Add(questionForDb);
                                            noochConnection.SaveChanges();
                                        }

                                        res.msg = "additional questions needed";

                                        #endregion Iterate Through Each Question And Save in DB
                                    }
                                    else
                                    {
                                        res.msg = "Server error: [Couldn't find question_set to save]";
                                        Logger.Error("Common Helper -> sendDocsToSynapseV3 FAILED - Found 'meta' object in Synapse Response, but missing 'question_set'");
                                    }

                                    #endregion Additional Verification Questions Returned
                                }
                                else if (synapseResponse.user != null)
                                {
                                    // No KBA Questions returned. Response will be the same as Register User With Synapse...

                                    // Update Member's DB record 
                                    memberEntity.IsVerifiedWithSynapse = true;
                                    memberEntity.ValidatedDate = DateTime.Now;
                                    memberEntity.DateModified = DateTime.Now;

                                    res.msg = "complete success";
                                }
                            }
                            else
                            {
                                // Response from Synapse had 'success' != true
                                // SHOULDN'T EVER GET HERE B/C IF SYNAPSE CAN'T VERIFY THE USER, IT RETURNS A 400 BAD REQUEST HTTP ERROR WITH A MESSAGE...SEE WEB EX BELOW
                                Logger.Error("Common Helper -> sendDocsToSynapseV3 FAILED: Synapse Result \"success != true\" - Username: [" + userNameDecrypted +
                                             "], MemberID: [" + MemberId + "]... SHOULDN'T EVER GET HERE B/C IF SYNAPSE CAN'T VERIFY THE USER, IT RETURNS A 400 BAD REQUEST HTTP ERROR WITH A MESSAGE...");

                                res.msg = "Add Docs response from synapse was false";
                            }
                        }
                        else
                        {
                            // Response from Synapse was null
                            Logger.Error("Common Helper -> sendDocsToSynapseV3 FAILED: Synapse Result was NULL - Username: [" + userNameDecrypted +
                                        "], MemberID : [" + MemberId + "]");

                            res.msg = "Add Docs response from synapse was null";
                        }

                        #endregion Parse Synapse Response
                    }
                    catch (WebException we)
                    {
                        #region Synapse Error Returned

                        var httpStatusCode = ((HttpWebResponse)we.Response).StatusCode;

                        var response = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                        JObject errorJsonFromSynapse = JObject.Parse(response);

                        // CLIFF (10/10/15): Synapse lists all possible V3 error codes in the docs -> Introduction -> Errors.
                        //                   We might have to do different things depending on which error is returned (like re-submitting a specific
                        //                   Document Type.  For now just pass back the error number & msg to the function that called this method.
                        string errorMsg = errorJsonFromSynapse["error"]["en"].ToString();

                        if (errorMsg != null)
                        {
                            Logger.Error("Common Helper -> sendDocsToSynapseV3 FAILED (Outer) - errorCode: [" + httpStatusCode.ToString() +
                                         "], Error Message from Synapse: [" + errorMsg + "]");

                            res.msg = errorMsg;

                            if (!String.IsNullOrEmpty(errorMsg) &&
                                (errorMsg.IndexOf("Unable to verify") > -1 ||
                                 errorMsg.IndexOf("submit a valid copy of passport") > -1))
                            {
                                Logger.Info("** THIS USER'S SSN INFO WAS NOT VERIFIED AT ALL. MUST INVESTIGATE WHY (COULD BE TYPO WITH PERSONAL INFO). " +
                                            "DETERMINE IF NECESSARY TO ASK FOR DRIVER'S LICENSE. **");

                                memberEntity.AdminNotes = "SSN INFO WAS INVALID WHEN SENT TO SYNAPSE. NEED TO COLLECT DRIVER'S LICENSE.";
                            }
                        }
                        else
                            res.msg = "MemberController Exception #2672";

                        #endregion Synapse Error Returned
                    }

                    // Save changes to Members DB
                    memberEntity.DateModified = DateTime.Now;
                    int saveMember = noochConnection.SaveChanges();

                    if (saveMember > 0)
                        Logger.Info("Common Helper -> sendDocsToSynapseV3 - Successfully updated user's record in Members Table");
                    else
                        Logger.Error("Common Helper -> sendDocsToSynapseV3 - FAILED to update user's record in Members Table");
                }
                    #endregion Send All Docs To Synapse
            }
            else
            {
                // Member not found in Nooch DB
                Logger.Info("Common Helper -> sendDocsToSynapseV3 FAILED: Member not found - MemberID: [" + MemberId + "]");
                res.msg = "Member not found";
            }

            return res;
        }


        [HttpPost]
        [ActionName("submitDocToSynapseV3_manual")]
        public ActionResult submitDocToSynapseV3_manual(string memid, string docUrl)
        {
            Logger.Info("Member Cntrlr -> submitDocumentToSynapseV3 - MemberID: [" + memid + "], DocURL: [" + docUrl + "]");

            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;
            res.msg = "Initial";

            try
            {
                if (String.IsNullOrEmpty(memid))
                {
                    res.msg = "Missing MemID";
                    return Json(res);
                }
                //else if (String.IsNullOrEmpty(docUrl))
                //{
                //    res.msg = "Missing doc URL";
                //    return Json(res);
                //}

                res = sendDocsToSynapseV3(memid);
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr - submitDocumentToSynapseV3 FAILED - userName: [" + memid + "]. Exception: [" + ex + "]");
                res.msg = ex.Message;
            }

            return Json(res);
        }


        /// <summary>
        /// For sending a user's SSN & DOB to Synapse using V3 API.
        /// UPDATE (JUNE 2016): WILL BE DEPRECATED THIS MONTH ONCE SYNAPSE FINSIHSES ADDING NEW /USER/DOCS/ADD SERVICE TO V3.0.
        /// </summary>
        /// <param name="MemberId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("submitSsnToSynapseV3")]
        public ActionResult submitSsnToSynapseV3(string memid)
        {
            Logger.Info("CommonHelper -> sendUserSsnInfoToSynapseV3 Initialized - [MemberId: " + memid + "]");

            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            var id = Utility.ConvertToGuid(memid);

            using (var noochConnection = new NOOCHEntities())
            {
                var memberEntity = noochConnection.Members.FirstOrDefault(m => m.MemberId == id && m.IsDeleted == false);

                noochConnection.Entry(memberEntity).Reload();

                if (memberEntity != null)
                {
                    var userNameDecrypted = CommonHelper.GetDecryptedData(memberEntity.UserName);

                    string usersFirstName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(memberEntity.FirstName));
                    string usersLastName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(memberEntity.LastName));

                    string usersAddress = "";
                    string usersZip = "";

                    DateTime usersDob;
                    string usersDobDay = "";
                    string usersDobMonth = "";
                    string usersDobYear = "";

                    string usersSsn = "";

                    string usersSynapseOauthKey = "";
                    string usersFingerprint = "";

                    #region Check User For All Required Data

                    try
                    {
                        bool isMissingSomething = false;
                        // Member found, now check that they have added a full Address (including city, zip), SSN, & DoB

                        // Check for Fingerprint (UDID1 in the database)
                        if (String.IsNullOrEmpty(memberEntity.UDID1))
                        {
                            isMissingSomething = true;
                            res.msg = " Common Helper - Missing UDID";
                        }
                        else
                        {
                            usersFingerprint = memberEntity.UDID1;
                        }

                        // Check for Address
                        if (String.IsNullOrEmpty(memberEntity.Address))
                        {
                            isMissingSomething = true;
                            res.msg += " Common Helper - Missing Address";
                        }
                        else
                        {
                            usersAddress = CommonHelper.GetDecryptedData(memberEntity.Address);
                        }

                        // Check for ZIP
                        if (String.IsNullOrEmpty(memberEntity.Zipcode))
                        {
                            isMissingSomething = true;
                            res.msg += " MDA - Missing ZIP";
                        }
                        else
                        {
                            usersZip = CommonHelper.GetDecryptedData(memberEntity.Zipcode);
                        }

                        // Check for SSN
                        if (string.IsNullOrEmpty(memberEntity.SSN))
                        {
                            isMissingSomething = true;
                            res.msg += " MDA - Missing SSN";
                        }
                        else
                        {
                            usersSsn = CommonHelper.GetDecryptedData(memberEntity.SSN);
                            usersSsn = usersSsn.Replace(" ", "").Replace("-", "");
                        }

                        // Check for Date Of Birth (Not encrypted)
                        if (memberEntity.DateOfBirth == null)
                        {
                            isMissingSomething = true;
                            res.msg += " MDA - Missing Date of Birth";
                        }
                        else
                        {
                            usersDob = Convert.ToDateTime(memberEntity.DateOfBirth);

                            // We have DOB, now we must parse it into day, month, & year
                            usersDobDay = usersDob.Day.ToString();
                            usersDobMonth = usersDob.Month.ToString();
                            usersDobYear = usersDob.Year.ToString();
                        }
                        // Return if any data was missing in previous block
                        if (isMissingSomething)
                        {
                            Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 ABORTED: Member has no DoB. [Username: " + userNameDecrypted + "], [Message: " + res.msg + "]");
                            return Json(res);
                        }


                        // Now check if user already has a Synapse User account (would have a record in SynapseCreateUserResults.dbo)
                        var synapseUserObj = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == id && m.IsDeleted == false);

                        if (synapseUserObj == null)
                        {
                            Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 ABORTED: Member's Synapse User Details not found. [Username: " + userNameDecrypted + "]");
                            res.msg = "Users synapse details not found";
                            return Json(res);
                        }
                        else
                        {
                            usersSynapseOauthKey = CommonHelper.GetDecryptedData(synapseUserObj.access_token);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 FAILED on checking for all required data - [Username: " +
                                      userNameDecrypted + "], [Exception: " + ex + "]");
                    }

                    #endregion Check User For All Required Data

                    if (memberEntity.IsVerifiedWithSynapse == null)
                    {
                        // Update Member's DB record from NULL to false (update to true later on if Verification from Synapse is completely successful)
                        Logger.Info("Common Helper -> sendUserSsnInfoToSynapseV3 - About to set IsVerifiedWithSynapse to False before calling Synapse: [Username: " +
                                     userNameDecrypted + "]");
                        memberEntity.IsVerifiedWithSynapse = false;
                    }

                    #region Send SSN Info To Synapse

                    try
                    {
                        #region Call Synapse V3 /user/doc/add API

                        synapseAddKycInfoInputV3Class synapseKycInput = new synapseAddKycInfoInputV3Class();

                        SynapseV3Input_login login = new SynapseV3Input_login();
                        login.oauth_key = usersSynapseOauthKey;
                        synapseKycInput.login = login;

                        addKycInfoInput_user_doc doc = new addKycInfoInput_user_doc();
                        doc.birth_day = usersDobDay;
                        doc.birth_month = usersDobMonth;
                        doc.birth_year = usersDobYear;
                        doc.name_first = usersFirstName;
                        doc.name_last = usersLastName;
                        doc.address_street1 = usersAddress;
                        doc.address_postal_code = usersZip;
                        doc.address_country_code = "US";

                        doc.document_type = "SSN"; // This can also be "PASSPORT" or "DRIVERS_LICENSE"... we need to eventually support all 3 options (Rent Scene has international clients that don't have SSN but do have a Passport)
                        doc.document_value = usersSsn; // Can also be the user's Passport # or DL #

                        addKycInfoInput_user user = new addKycInfoInput_user();
                        user.fingerprint = usersFingerprint;
                        user.doc = doc;

                        synapseKycInput.user = user;

                        string baseAddress = Convert.ToBoolean(Utility.GetValueFromConfig("IsRunningOnSandBox"))
                                             ? "https://sandbox.synapsepay.com/api/v3/user/doc/add"
                                             : "https://synapsepay.com/api/v3/user/doc/add";

                        #region For Testing & Logging

                        if (memberEntity.MemberId.ToString().ToLower() == "b3a6cf7b-561f-4105-99e4-406a215ccf60")
                        {
                            doc.name_last = "Satell";
                            doc.document_value = "195707562";
                        }

                        try
                        {
                            Logger.Info("Common Helper -> sendUserSsnInfoToSynapseV3 - About To Query Synapse (/v3/user/doc/add) -> Payload to send to Synapse: [OauthKey: " + login.oauth_key +
                                "], [Birth_day: " + doc.birth_day + "], [Birth_month: " + doc.birth_month +
                                "], [Birth_year: " + doc.birth_year + "], [name_first: " + doc.name_first +
                                "], [name_last: " + doc.name_last + "], [ssn: " + doc.document_value +
                                "], [address_street1: " + doc.address_street1 + "], [postal_code: " + doc.address_postal_code +
                                "], [country_code: " + doc.address_country_code + "], [Fingerprint: " + user.fingerprint +
                                "], [BASE_ADDRESS: " + baseAddress + "].");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Common Helper -> sendUserSSNInfoToSynapseV3 - Couldn't log Synapse SSN Payload. [Exception: " + ex + "]");
                        }

                        #endregion For Testing & Logging

                        var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                        http.Accept = "application/json";
                        http.ContentType = "application/json";
                        http.Method = "POST";

                        string parsedContent = JsonConvert.SerializeObject(synapseKycInput);
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        Byte[] bytes = encoding.GetBytes(parsedContent);

                        Stream newStream = http.GetRequestStream();
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();

                        var response = http.GetResponse();
                        var stream = response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var content = sr.ReadToEnd();

                        kycInfoResponseFromSynapse synapseResponse = new kycInfoResponseFromSynapse();
                        synapseResponse = JsonConvert.DeserializeObject<kycInfoResponseFromSynapse>(content);

                        #endregion Call Synapse V3 /user/doc/add API


                        // NOW WE MUST PARSE THE SYNAPSE RESPONSE. THERE ARE 3 POSSIBLE SCENARIOS:
                        // 1.) SSN Validation was successful. Synapse returns {"success": true}
                        // 2.) SSN Validation was PARTLY successful.  Synapse returns: "success":true... 
                        //     plus an object "question_set", containing a series of questions and array of multiple choice answers for each question.
                        //     We will display the questions to the user via the IDVerification.aspx page (already built-in to the Add-Bank process)
                        // 3.) SSN Validation Failed:  Synapse will return HTTP Error 400 Bad Request
                        //     with an "error" object, and then a message in "error.en" that should be: "Invalid SSN information supplied. Request user to submit a copy of passport/divers license and SSN via user/doc/attachments/add"

                        #region Parse Synapse Response

                        if (synapseResponse != null)
                        {
                            if (synapseResponse.success == true)
                            {
                                Logger.Info("Common Helper -> sendUserSsnInfoToSynapseV3 - Synapse returned SUCCESS = TRUE. Now checking if additional Verification questions are required...");

                                // Great, we have at least partial success. Now check if further verification is needed by checking if Synapse returned a 'question_set' object.

                                res.isSuccess = true;

                                if (synapseResponse.question_set != null)
                                {
                                    // Further Verification is needed...
                                    #region Additional Verification Questions Returned

                                    // Now make sure an Array[] set of 'questions' was returned (could be up to 5 questions, each with 5 possible answer choices)
                                    if (synapseResponse.question_set.questions != null)
                                    {
                                        Logger.Info("Common Helper -> sendUserSsnInfoToSynapseV3 - Question_Set was returned, further validation will be needed. Saving ID Verification Questions...");

                                        // Saving these questions in DB.  

                                        // UPDATE (9/29/15):
                                        // The user will have to answer these on the IDVerification.aspx page.
                                        // That's why I updated the sendSSN function to not be void and return success + a message. Based on that value,
                                        // the Add-Bank page will direct the user either to the IDVerification page (via iFrame), or not if questions are not needed.

                                        // Loop through each question set (question/answers/id)
                                        #region Iterate Through Each Question And Save in DB

                                        foreach (synapseIdVerificationQuestionAnswerSet question in synapseResponse.question_set.questions)
                                        {
                                            SynapseIdVerificationQuestion questionForDb = new SynapseIdVerificationQuestion();
                                            questionForDb.MemberId = id;
                                            questionForDb.QuestionSetId = synapseResponse.question_set.id;
                                            questionForDb.SynpQuestionId = question.id;

                                            questionForDb.DateCreated = DateTime.Now;
                                            questionForDb.submitted = false;

                                            questionForDb.person_id = synapseResponse.question_set.person_id;
                                            questionForDb.time_limit = synapseResponse.question_set.time_limit;
                                            questionForDb.score = synapseResponse.question_set.score; // THIS COULD BE NULL...
                                            questionForDb.updated_at = synapseResponse.question_set.updated_at.ToString();
                                            questionForDb.livemode = synapseResponse.question_set.livemode; // NO IDEA WHAT THIS IS FOR...
                                            questionForDb.expired = synapseResponse.question_set.expired; // SHOULD ALWAYS BE false
                                            questionForDb.created_at = synapseResponse.question_set.created_at.ToString();

                                            questionForDb.Question = question.question;

                                            questionForDb.Choice1Id = question.answers[0].id;
                                            questionForDb.Choice1Text = question.answers[0].answer;

                                            questionForDb.Choice2Id = question.answers[1].id;
                                            questionForDb.Choice2Text = question.answers[1].answer;

                                            questionForDb.Choice3Id = question.answers[2].id;
                                            questionForDb.Choice3Text = question.answers[2].answer;

                                            questionForDb.Choice4Id = question.answers[3].id;
                                            questionForDb.Choice4Text = question.answers[3].answer;

                                            questionForDb.Choice5Id = question.answers[4].id;
                                            questionForDb.Choice5Text = question.answers[4].answer;

                                            noochConnection.SynapseIdVerificationQuestions.Add(questionForDb);
                                            noochConnection.SaveChanges();
                                        }

                                        res.msg = "additional questions needed";

                                        #endregion Iterate Through Each Question And Save in DB
                                    }

                                    #endregion Additional Verification Questions Returned
                                }
                                else if (synapseResponse.user != null)
                                {
                                    // User is verified completely. In this case response is same as Register User With Synapse...
                                    // Just update permission in CreateSynapseUserResults table
                                    #region Update Permission in SynapseCreateUserResults Table

                                    try
                                    {
                                        // Get existing records from dbo.SynapseCreateUserResults for this Member
                                        var synapseRes = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == id &&
                                                                                                                 m.IsDeleted == false);

                                        if (synapseRes != null)
                                        {
                                            synapseRes.permission = synapseResponse.user.permission;
                                            synapseRes.physical_doc = synapseResponse.user.doc_status != null ? synapseResponse.user.doc_status.physical_doc : null;
                                            synapseRes.virtual_doc = synapseResponse.user.doc_status != null ? synapseResponse.user.doc_status.virtual_doc : null;
                                            synapseRes.extra_security = synapseResponse.user.extra.extra_security != null ? synapseResponse.user.extra.extra_security.ToString() : null;

                                            noochConnection.SaveChanges();
                                            noochConnection.Entry(synapseRes).Reload();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 - EXCEPTION on trying to update User's record in CreateSynapseUserResults Table - " +
                                                     "[MemberID: " + memid + "], [Exception: " + ex + "]");
                                    }

                                    #endregion Update Permission in CreateSynapseUserResults Table

                                    // Update Member's DB record
                                    memberEntity.IsVerifiedWithSynapse = true;
                                    memberEntity.ValidatedDate = DateTime.Now;
                                    memberEntity.DateModified = DateTime.Now;

                                    res.msg = "complete success";
                                }
                            }
                            else
                            {
                                // Response from Synapse had 'success' != true
                                // SHOULDN'T EVER GET HERE B/C IF SYNAPSE CAN'T VERIFY THE USER, IT RETURNS A 400 BAD REQUEST HTTP ERROR WITH A MESSAGE...SEE WEB EX BELOW
                                Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 FAILED: Synapse Result \"success != true\" - [Username: " + userNameDecrypted + "]");
                                res.msg = "SSN response from synapse was false";
                            }
                        }
                        else
                        {
                            // Response from Synapse was null
                            Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 FAILED: Synapse Result was NULL - [Username: " + userNameDecrypted + "]");
                            res.msg = "SSN response from synapse was null";
                        }

                        #endregion Parse Synapse Response
                    }
                    catch (WebException we)
                    {
                        var httpStatusCode = ((HttpWebResponse)we.Response).StatusCode;

                        var response = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();
                        JObject errorJsonFromSynapse = JObject.Parse(response);

                        // CLIFF (10/10/15): Synapse lists all possible V3 error codes in the docs -> Introduction -> Errors
                        //                   We might have to do different things depending on which error is returned... for now just pass
                        //                   back the error number & msg to the function that called this method.
                        string errorMsg = errorJsonFromSynapse["error"]["en"].ToString();

                        if (errorMsg != null)
                        {
                            Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 FAILED (Outer) - [errorCode: " + httpStatusCode.ToString() +
                                         "], [Error Message from Synapse: " + errorMsg + "]");

                            res.msg = errorMsg;

                            if (!String.IsNullOrEmpty(errorMsg) &&
                                (errorMsg.IndexOf("Unable to verify") > -1 ||
                                 errorMsg.IndexOf("submit a valid copy of passport") > -1))
                            {
                                Logger.Info("**  THIS USER'S SSN INFO WAS NOT VERIFIED AT ALL. DETERMINE IF NECESSARY TO ASK FOR DRIVER'S LICENSE.  **");

                                memberEntity.AdminNotes = "SSN INFO WAS INVALID WHEN SENT TO SYNAPSE. NEED TO COLLECT DRIVER'S LICENSE.";

                                // Email Nooch Admin about this user for manual follow-up (Send email to Cliff)
                                #region Notify Nooch Admin About Failed SSN Validation

                                try
                                {
                                    StringBuilder st = new StringBuilder();

                                    string city = !String.IsNullOrEmpty(memberEntity.City) ? CommonHelper.GetDecryptedData(memberEntity.City) : "NONE";

                                    st.Append("<table border='1' cellpadding='6' style='border-collapse:collapse;text-align:center;'>" +
                                              "<tr><th>PARAMETER</th><th>VALUE</th></tr>");
                                    st.Append("<tr><td><strong>Name</strong></td><td>" + usersFirstName + " " + usersLastName + "</td></tr>");
                                    st.Append("<tr><td><strong>MemberId</strong></td><td>" + memid + "</td></tr>");
                                    st.Append("<tr><td><strong>Nooch_ID</strong></td><td><a href=\"https://noochme.com/noochnewadmin/Member/Detail?NoochId=" + memberEntity.Nooch_ID + "\" target='_blank'>" + memberEntity.Nooch_ID + "</a></td></tr>");
                                    st.Append("<tr><td><strong>Status</strong></td><td><strong>" + memberEntity.Status + "</strong></td></tr>");
                                    st.Append("<tr><td><strong>DOB</strong></td><td>" + Convert.ToDateTime(memberEntity.DateOfBirth).ToString("MMMM dd, yyyy") + "</td></tr>");
                                    st.Append("<tr><td><strong>SSN</strong></td><td>" + usersSsn + "</td></tr>");
                                    st.Append("<tr><td><strong>Address</strong></td><td>" + usersAddress + "</td></tr>");
                                    st.Append("<tr><td><strong>City</strong></td><td>" + city + "</td></tr>");
                                    st.Append("<tr><td><strong>ZIP</strong></td><td>" + usersZip + "</td></tr>");
                                    st.Append("<tr><td><strong>Contact #</strong></td><td>" + CommonHelper.FormatPhoneNumber(memberEntity.ContactNumber) + "</td></tr>");
                                    st.Append("<tr><td><strong>Phone Verified?</strong></td><td>" + memberEntity.IsVerifiedPhone.ToString() + "</td></tr>");
                                    st.Append("<tr><td><strong>IsVerifiedWithSynapse</strong></td><td>" + memberEntity.IsVerifiedWithSynapse.ToString() + "</td></tr>");
                                    st.Append("</table>");

                                    StringBuilder completeEmailTxt = new StringBuilder();
                                    string s = "<html><body><h3>Nooch SSN Verification Failure</h3><p style='margin:0 auto 20px;'>The following Nooch user just failed an SSN Verification attempt:</p>"
                                               + st.ToString() +
                                               "<br/><br/><small>This email was generated automatically during <strong>[CommonHelper -> sendUserSsnInfoToSynapseV3]</strong>.</small></body></html>";

                                    completeEmailTxt.Append(s);

                                    Utility.SendEmail(null, "SSNFAILURE@nooch.com", "cliff@nooch.com",
                                                      "NOOCH USER'S SSN (V3) VALIDATION FAILED", null, null, null, null, completeEmailTxt.ToString());
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 FAILED - Attempted to notify Nooch Admin via email but got Exception: [" + ex + "]");
                                }

                                #endregion Notify Nooch Admin About Failed SSN Validation
                            }
                        }
                        else
                        {
                            res.msg = "Server Exception #2387";
                        }
                    }

                    // Save changes to Members DB
                    memberEntity.DateModified = DateTime.Now;
                    int save = noochConnection.SaveChanges();

                    if (save > 0)
                    {
                        Logger.Info("Common Helper -> sendUserSsnInfoToSynapseV3 SUCCESS - Updates Saved in DB - [MemberId: " + memid + "]");
                    }
                    else
                    {
                        res.msg = "Unable to save changes to Member table in DB";
                        Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 ERROR - FAILED to save Member updates in DB - [MemberId: " + memid + "]");
                    }

                    #endregion Send SSN Info To Synapse
                }
                else
                {
                    // Member not found in Nooch DB
                    Logger.Error("Common Helper -> sendUserSsnInfoToSynapseV3 FAILED: Member not found - [MemberId: " + memid + "]");
                    res.msg = "Member not found";
                }
            }

            return Json(res);
        }


        [HttpPost]
        [ActionName("refreshSynapseUserV3")]
        public ActionResult refreshSynapseUserV3(string memid)
        {
            Logger.Info("Member Cntrlr -> refreshSynapseUserV3 Initialized - [MemberId: " + memid + "]");

            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            try
            {
                Guid id = Utility.ConvertToGuid(memid);

                using (var noochConnection = new NOOCHEntities())
                {
                    #region Get User's Synapse OAuth Consumer Key

                    SynapseCreateUserResult synapseUserObj = new SynapseCreateUserResult();
                    synapseUserObj = noochConnection.SynapseCreateUserResults.FirstOrDefault(m => m.MemberId == id && m.IsDeleted == false);

                    if (synapseUserObj == null)
                    {
                        Logger.Error("Member Cntrlr -> refreshSynapseUserV3 ABORTED: Member's Synapse User Details not found. [MemberId: " + memid + "]");
                        res.msg = "Could not find this member's account info";
                        return Json(res);
                    }
                    else
                    {
                        synapseV3checkUsersOauthKey checkTokenResult = CommonHelper.refreshSynapseV3OautKey(synapseUserObj.access_token);

                        if (checkTokenResult != null)
                        {
                            if (checkTokenResult.success == true)
                            {
                                res.isSuccess = true;
                                res.msg = "Refreshed Successfully: [" + checkTokenResult.msg + "]";
                            }
                            else
                            {
                                Logger.Error("Member Cntrlr -> refreshSynapseUserV3 FAILED on Checking User's Synapse OAuth Token - " +
                                             "CheckTokenResult.msg: [" + checkTokenResult.msg + "], MemberID: [" + memid + "]");
                                res.msg = checkTokenResult.msg;
                            }
                        }
                        else
                        {
                            Logger.Error("Member Cntrlr -> refreshSynapseUserV3 FAILED on Checking User's Synapse OAuth Token - " +
                                         "CheckTokenResult was NULL, MemberID: [" + memid + "]");
                            res.msg = "Unable to check user's Oauth Token";
                        }
                    }

                    #endregion Get User's Synapse OAuth Consumer Key
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr -> refreshSynapseUserV3 FAILED - Outer Exception - " +
                             "MemberID: [" + memid + "], Exception: [" + ex.Message + "]");
            }

            return Json(res);
        }


        [HttpPost]
        [ActionName("refreshSynapseBankV3")]
        public ActionResult refreshSynapseBankV3(string memid)
        {
            Logger.Info("Member Cntrlr -> refreshSynapseBankV3 Initialized - [MemberId: " + memid + "]");

            synapseV3GenericResponse res = new synapseV3GenericResponse();
            res.isSuccess = false;

            try
            {
                var memberObj = CommonHelper.GetMemberDetails(memid);

                using (var noochConnection = new NOOCHEntities())
                {
                    #region Get User's Synapse OAuth Consumer Key

                    SynapseBanksOfMember synapseBankObj = new SynapseBanksOfMember();
                    synapseBankObj = noochConnection.SynapseBanksOfMembers.FirstOrDefault(m => m.MemberId == memberObj.MemberId &&
                                                                                               m.IsDefault == true);

                    if (synapseBankObj == null)
                    {
                        Logger.Error("Member Cntrlr -> refreshSynapseBankV3 ABORTED: Member's Synapse Bank Details not found - [MemberId: " + memid + "]");
                        res.msg = "Could not find this member's Synapse Bank info";
                        return Json(res);
                    }
                    else
                    {
                        var email = CommonHelper.GetDecryptedData(memberObj.UserName);

                        synapseSearchUserResponse checkBankResult = CommonHelper.getUserPermissionsForSynapseV3(email);

                        Logger.Info("Member Cntrlr -> refreshSynapseBankV3 - Synapse Search Response -> Sucess: [" + checkBankResult.success +
                                    "], Error Msg: [" + checkBankResult.errorMsg + "]");

                        if (checkBankResult == null || !checkBankResult.success)
                        {
                            Logger.Error("Member Cntrlr -> refreshSynapseBankV3 - User's Synapse Bank Permissions were NULL or not successful :-(");
                            res.msg = "Failed to check user's bank permissions with Synapse";
                            return Json(res);
                        }

                        // 2. Check BANK/NODE permission for SENDER
                        if (checkBankResult.users != null && checkBankResult.users.Length > 0)
                        {
                            foreach (synapseSearchUserResponse_User userObj in checkBankResult.users)
                            {
                                // iterating each node inside
                                if (userObj.nodes != null && userObj.nodes.Length > 0)
                                {
                                    var oidToCheck = CommonHelper.GetDecryptedData(synapseBankObj.oid);

                                    NodePermissionCheckResult nodePermCheckRes = CommonHelper.IsNodeActiveInGivenSetOfNodes(userObj.nodes, oidToCheck);

                                    if (nodePermCheckRes.IsPermissionfound == true && !String.IsNullOrEmpty(nodePermCheckRes.PermissionType))
                                    {
                                        if (nodePermCheckRes.PermissionType != synapseBankObj.allowed)
                                        {
                                            res.msg = "Bank Permission Updated Successfully - OLD: [" + synapseBankObj.allowed +
                                                      "], NEW: [" + nodePermCheckRes.PermissionType + "]";

                                            // Update Bank in DB
                                            synapseBankObj.allowed = nodePermCheckRes.PermissionType;

                                            noochConnection.SaveChanges();
                                        }
                                        else
                                        {
                                            res.msg = "Bank Permission Confirmed as [" + nodePermCheckRes.PermissionType + "]";
                                        }

                                        Logger.Info("Member Cntrlr -> refreshSynapseBankV3 SUCCESS - " + res.msg);
                                        res.isSuccess = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var error = "Couldn't parse user objects in Synapse /user/search response";
                            Logger.Error("Member Cntrlr -> refreshSynapseBankV3 - " + error);
                            res.msg = error;
                        }
                    }

                    #endregion Get User's Synapse OAuth Consumer Key
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Member Cntrlr -> refreshSynapseBankV3 FAILED - Outer Exception - " +
                             "MemberID: [" + memid + "], Exception: [" + ex.Message + "]");
            }

            return Json(res);
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