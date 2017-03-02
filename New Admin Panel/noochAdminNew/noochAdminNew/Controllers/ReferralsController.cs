using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using noochAdminNew.Classes.ModelClasses;
using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Models;

namespace noochAdminNew.Controllers
{
    public class ReferralsController : Controller
    {
        public ActionResult Index()
        {
            Logger.Info("ReferralsCnrlr -> Loading Referral Page...");
            List<InviteCodeDataClass> allinvitecodes = GetAllInviteCodes();

            // Get top 5 recent referrals
            ViewBag.RecentReffarals = GetMostRecent5Referrals();
            ViewBag.TopReferringMembers = GetTopReferrals(1);
            ViewBag.TopReferringMembersWeekly = GetTopReferrals(2);
            ViewBag.TopReferringMembersDay = GetTopReferrals(3);

            Logger.Info("ReferralsCnrlr -> About to return the View");
            return View(allinvitecodes);
        }


        // to read all invite codes from db
        private List<InviteCodeDataClass> GetAllInviteCodes()
        {
            Logger.Info("ReferralsCntlr -> GetAllInviteCodes() Fired");

            List<InviteCodeDataClass> li = new List<InviteCodeDataClass>();
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var all_invite_codes = (from t in obj.InviteCodes
                                        select t).ToList();

                //Logger.Info("ReferralsCntlr -> GetAllInviteCodes -> Got [" + all_invite_codes.Count + "] Invite Codes from DB!");

                foreach (InviteCode m in all_invite_codes)
                {
                    InviteCodeDataClass idc = new InviteCodeDataClass();
                    var createdby = "";

                    if (m.CreatedBy != null)
                    {
                        Guid g = Utility.ConvertToGuid(m.CreatedBy.ToString());
                        var cre = (from abc in obj.AdminUsers
                                   where abc.UserId.Equals(g)
                                   select abc).SingleOrDefault();

                        if (cre != null)
                            createdby = cre.FirstName;
                    }

                    idc.createdbyadmin_name = createdby;
                    idc.InviteCode_Id = m.InviteCodeId.ToString();
                    idc.code = m.code;
                    idc.count = m.count.ToString();
                    idc.totalallowed = m.totalAllowed.ToString();
                    idc.adminnotes = m.AdminNotes;

                    DateTime? dt = m.CreatedOn;
                    idc.DateCreated = dt != null ? String.Format("{0:MMM d, yyyy}", dt) : "";

                    DateTime? dt2 = m.ModifiedOn;
                    idc.DateModified = dt2 != null ? String.Format("{0:MMM d, yyyy}", dt2) : "";

                    // idc.createdbyadmin_id = m.CreatedBy.ToString();

                    li.Add(idc);
                }

                Logger.Info("ReferralsCntlr -> GetAllInviteCodes -> Returning [" + li.Count + "] objects");
            }

            return li;
        }


        public ActionResult JoinningsWithGivenReferralCode(string RefCode)
        {
            List<MembersListDataClass> AllMemberFormtted = GetAllMembersJoinedWithGivenRefCode(RefCode);

            return View(AllMemberFormtted);
        }


        private List<ReferralsResultClass> GetTopReferrals(int filterType)
        {
            Logger.Info("ReferralsCntlr -> GetTopReferrals Fired - FilterType: [" + filterType + "]");

            List<ReferralsResultClass> li = new List<ReferralsResultClass>();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                List<GetTopReferringMembers_Result> allRef = new List<GetTopReferringMembers_Result>();

                switch (filterType)
                {
                    case 1:
                        allRef = obj.GetTopReferringMembers(1).ToList();
                        break;
                    case 2:
                        allRef = obj.GetTopReferringMembers(2).ToList();
                        break;
                    case 3:
                        allRef = obj.GetTopReferringMembers(3).ToList();
                        break;
                    default:
                        allRef = obj.GetTopReferringMembers(1).ToList();
                        break;
                }

                foreach (GetTopReferringMembers_Result v in allRef)
                {
                    try
                    {
                        ReferralsResultClass rrc = new ReferralsResultClass();
                        Guid icode = Utility.ConvertToGuid(v.InviteCode.ToString());

                        //Logger.Info("ReferralsCntlr -> GetTopReferrals CHECKPOINT #2 - InviteCodeID: [" + v.InviteCode.ToString() + "]");

                        // Get Member Object From Given Invite Code
                        var mem = (from member in obj.Members
                                   where member.InviteCodeId == icode &&
                                         member.InviteCodeId != new Guid("00000000-0000-0000-0000-000000000000")
                                   select new
                                        {
                                            member.FirstName,
                                            member.LastName,
                                            member.Nooch_ID
                                        }).SingleOrDefault();

                        if (mem != null)
                        {
                            // Get Invite Code Info
                            var icinfo = (from code in obj.InviteCodes
                                          where code.InviteCodeId == icode
                                          select code.code).SingleOrDefault();

                            if (icinfo != null)
                            {
                                rrc.MemberName = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(mem.FirstName)) + " " +
                                                 CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(mem.LastName));
                                rrc.ReferralsCount = Convert.ToInt16(v.Referrals);
                                rrc.RefCode = icinfo.ToUpper();
                                rrc.NoochId = mem.Nooch_ID;

                                li.Add(rrc);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("ReferralsCntlr -> GetTopReferrals ERROR - Exception: [" + ex.Message + "]");
                    }
                }

                return li;
            }
        }


        private List<MembersListDataClass> GetAllMembersJoinedWithGivenRefCode(string RefCode)
        {
            List<MembersListDataClass> AllMemberFormtted = new List<MembersListDataClass>();
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                string cc = RefCode.Trim().ToUpper();

                var r = (from c in obj.InviteCodes where c.code == cc select c).SingleOrDefault();
                if (r != null)
                {
                    var All_Members_In_Records = (from t in obj.Members
                                                  where t.IsDeleted == false && t.InviteCodeIdUsed == r.InviteCodeId
                                                  select t).ToList();

                    foreach (Member m in All_Members_In_Records)
                    {
                        int TransCount =
                            (from tr in obj.Transactions
                             where tr.Member.MemberId == m.MemberId || tr.Member1.MemberId == m.MemberId
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
                        mdc.FirstName = !String.IsNullOrEmpty(m.FirstName) ? CommonHelper.GetDecryptedData(m.FirstName) : "";
                        mdc.LastName = !String.IsNullOrEmpty(m.LastName) ? CommonHelper.GetDecryptedData(m.LastName) : "";
                        mdc.UserName = !String.IsNullOrEmpty(m.UserName) ? CommonHelper.GetDecryptedData(m.UserName) : "";

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
                                mdc.ContactNumber = m.ContactNumber;
                        }
                        else
                            mdc.ContactNumber = m.ContactNumber;

                        mdc.Status = m.Status;
                        mdc.IsDeleted = m.IsDeleted ?? false;
                        mdc.IsVerifiedPhone = m.IsVerifiedPhone ?? false;
                        mdc.City = !String.IsNullOrEmpty(m.City) ? CommonHelper.GetDecryptedData(m.City) : "";

                        mdc.TotalAmountSent = mdc.TotalAmountSent != "0" ? Convert.ToDecimal(String.Format("{0:0.00}", Convert.ToDecimal(totalAmount))).ToString() : "0";

                        mdc.TotalTransactions = TransCount;
                        mdc.DateCreated = Convert.ToDateTime(m.DateCreated);
                        AllMemberFormtted.Add(mdc);
                    }
                }
            }


            return AllMemberFormtted;
        }


        private List<MembersListRefCodeUsedDataClass> GetMostRecent5Referrals()
        {
            Logger.Info("ReferralsCntlr -> GetMostRecent5Referrals Fired");

            List<MembersListRefCodeUsedDataClass> AllMemberFormtted = new List<MembersListRefCodeUsedDataClass>();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var lastFiveMembers = (from member in obj.Members
                                       where member.IsDeleted == false &&
                                             member.InviteCodeIdUsed != null &&
                                             member.InviteCodeIdUsed != new Guid("00000000-0000-0000-0000-000000000000")
                                       select member)
                                       .ToList()
                                       .OrderByDescending(m => m.DateCreated).Take(5);

                foreach (Member m in lastFiveMembers)
                {
                    MembersListRefCodeUsedDataClass mdc = new MembersListRefCodeUsedDataClass();

                    mdc.Name = CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(m.FirstName)) + " " +
                               CommonHelper.UppercaseFirst(CommonHelper.GetDecryptedData(m.LastName));
                    mdc.DateUsed = String.Format("{0: MMMM d, yyyy}", m.DateCreated);
                    mdc.NoochId = m.Nooch_ID;

                    var codeused = (from c in obj.InviteCodes
                                    where c.InviteCodeId == m.InviteCodeIdUsed
                                    select c).SingleOrDefault();

                    if (codeused != null)
                    {
                        //Logger.Info("ReferralsCntlr -> GetMostRecent5Referrals - CHECKPOINT - NAME: [" + mdc.Name + "], Code Used: [" + codeused.code + "]");
                        mdc.CodeUsed = codeused.code.Trim().ToUpper();
                    }

                    AllMemberFormtted.Add(mdc);
                }

                Logger.Info("ReferralsCntlr -> GetMostRecent5Referrals -> Returning [" + AllMemberFormtted.Count + "] Objects");
            }

            return AllMemberFormtted;
        }

        //
        // GET: /Referrals/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Referrals/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Referrals/Create

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

        //
        // GET: /Referrals/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Referrals/Edit/5

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

        //
        // GET: /Referrals/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Referrals/Delete/5

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


        [HttpPost]
        [ActionName("CreateNewReferralCode")]
        public ActionResult CreateNewReferralCode(string newCode, string allowedUses, string newCodeNotes)
        {
            Guid d = Utility.ConvertToGuid(Session["UserId"].ToString());
            CreateNewReferralCodeResultClass s = new CreateNewReferralCodeResultClass();

            if (!String.IsNullOrEmpty(newCode.Trim()) && !String.IsNullOrEmpty(allowedUses.Trim()) && !String.IsNullOrEmpty(newCodeNotes.Trim()))
            {
                // checking if valid int passed
                int value;
                if (int.TryParse(allowedUses, out value))
                {
                    s = createNewReferralCode(newCode.ToUpper(), value, newCodeNotes, d);
                }
                else
                {
                    s.IsSuccess = false;
                    s.Message = "Max allowed users shouls be integer value !";
                }
            }
            else
            {
                s.IsSuccess = false;
                s.Message = "Invalid data passed, please retry!";
            }

            return Json(s);
        }


        private CreateNewReferralCodeResultClass createNewReferralCode(string newCode, int allowedUses, string newCodeNotes, Guid AdminGuid)
        {
            CreateNewReferralCodeResultClass s2 = new CreateNewReferralCodeResultClass();
            // checking if given referral code already exists
            using (NOOCHEntities obj = new NOOCHEntities())
            {
                var trimmedcode = newCode.Trim();
                var r = (from c in obj.InviteCodes
                         where c.code == trimmedcode
                         select c).ToList();

                if (r.Count == 0)
                {
                    InviteCode ivc = new InviteCode();
                    Guid codeIf = Guid.NewGuid();
                    ivc.InviteCodeId = codeIf;
                    ivc.CreatedOn = DateTime.Now;

                    ivc.CreatedBy = AdminGuid;

                    ivc.code = trimmedcode;
                    ivc.AdminNotes = newCodeNotes;
                    ivc.totalAllowed = allowedUses;
                    ivc.count = 0;

                    obj.InviteCodes.Add(ivc);
                    obj.SaveChanges();
                    s2.IsSuccess = true;
                    s2.Message = "Invite code added successfully !";
                }
                else
                {
                    // Update existing invite code value
                    var rxistingCode = r.ElementAt(0);

                    // checking if new value is greater or less then existing one
                    rxistingCode.AdminNotes = newCodeNotes;
                    rxistingCode.ModifiedOn = DateTime.Now;
                    if (rxistingCode.count < allowedUses)
                    {
                        rxistingCode.totalAllowed = allowedUses;
                        obj.SaveChanges();
                        s2.IsSuccess = true;
                        s2.Message = "Invite code updated successfully !";
                    }
                    else
                    {
                        s2.IsSuccess = false;
                        s2.Message = "Invite code already used " + rxistingCode.count + " times.";
                    }
                }

                return s2;
            }
        }

    }
}
