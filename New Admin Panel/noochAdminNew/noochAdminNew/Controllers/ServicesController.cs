using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Models;

namespace noochAdminNew.Controllers
{
    public class ServicesController : ApiController
    {
        // GET api/services
        [HttpGet]
        [ActionName("SearchUsername")]
        public List<MemberSearchResult> SearchUsername(string username, string _)
        {
            List<MemberSearchResult> res = new List<MemberSearchResult>();
            List<MemberSearchResult> resFinal = new List<MemberSearchResult>();

            using (NOOCHEntities obj = new NOOCHEntities())
            {
                //string ecnryptedusername = CommonHelper.GetEncryptedData(username.Trim().ToLower());

                // getting all users with active knox bank account

                var allActiveUsersWithKnox = (from c in obj.KnoxAccountDetails
                                              where c.IsDeleted == false
                                              select c).ToList();

                foreach (KnoxAccountDetail kad in allActiveUsersWithKnox)
                {
                    var mem = (from c in obj.Members where c.MemberId == kad.MemberId select c).SingleOrDefault();

                    if (mem != null)
                    {
                        MemberSearchResult msr = new MemberSearchResult();
                        if (!String.IsNullOrEmpty(mem.UserName))
                        {
                            msr.UserName = CommonHelper.GetDecryptedData(mem.UserName);
                            res.Add(msr);
                        }
                    }
                }

                resFinal = (from c in res
                            where c.UserName.Contains(username)
                            select c).ToList();
            }
            return resFinal;
        }

        public class MemberSearchResult
        {
            public string UserName { get; set; }
        }
    }
}
