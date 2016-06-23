using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using AutoMapper;
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

                var allActiveUsersWithKnox = (from c in obj.Members where c.IsVerifiedWithSynapse == true && c.IsDeleted == false select c).ToList();

                foreach (Member kad in allActiveUsersWithKnox)
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

        [HttpGet]
        [ActionName("TestingPaginationForAllMembersPage")]
        public void TestingPaginationForAllMembersPage()
        {
            using (NOOCHEntities obj = new NOOCHEntities())
            {

                var AllMembers = obj.Members.Take(20).Where(m => m.IsDeleted == false).ToList();



                Mapper.Initialize(cfg => cfg.CreateMap<Member, MemberPOCO>()

                    // could be blank things
                    .ForMember(dest => dest.UserName, dest => dest.MapFrom((src => src.UserName != null ? CommonHelper.GetDecryptedData(src.UserName) : "")))
                    .ForMember(dest => dest.UserNameLowerCase, dest => dest.MapFrom((src => src.UserNameLowerCase != null ? CommonHelper.GetDecryptedData(src.UserNameLowerCase) : "")))
                    .ForMember(dest => dest.FirstName, dest => dest.MapFrom((src => src.FirstName != null ? CommonHelper.GetDecryptedData(src.FirstName) : "")))
                    .ForMember(dest => dest.LastName, dest => dest.MapFrom((src => src.LastName != null ? CommonHelper.GetDecryptedData(src.LastName) : "")))

                    //could be null...address related stuff
                    .ForMember(dest => dest.City, dest => dest.MapFrom((src => src.City != null ? CommonHelper.GetDecryptedData(src.City) : "")))
                    .ForMember(dest => dest.Address, dest => dest.MapFrom((src => src.Address != null ? CommonHelper.GetDecryptedData(src.Address) : "")))
                    .ForMember(dest => dest.State, dest => dest.MapFrom((src => src.State != null ? CommonHelper.GetDecryptedData(src.State) : "")))
                    .ForMember(dest => dest.Zipcode, dest => dest.MapFrom((src => src.Zipcode != null ? CommonHelper.GetDecryptedData(src.Zipcode) : "")))

                    .ForMember(dest => dest.MemberId, dest => dest.MapFrom((src => new Guid(src.MemberId.ToString()))))
                    
                    .ForMember(dest => dest.ContactNumber, dest => dest.MapFrom((src => src.ContactNumber)))
                    .ForMember(dest => dest.DateCreated, dest => dest.MapFrom((src => src.DateCreated)))
                    .ForMember(dest => dest.Status, dest => dest.MapFrom((src => src.Status)))
                    

                    );

                var list = Mapper.Map<List<Member>, List<MemberPOCO>>(AllMembers);

            }
        }
    }
}
