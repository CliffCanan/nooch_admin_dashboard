using noochAdminNew.Classes.ModelClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ResponseClasses
{
    public class MemberOperationsResult
    {
        public bool IsSuccess { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }

        public List<MemberOperationsInnerClass> MemberOperationsOuterClass { get; set; }

        public List<MembersListDataClass> Members { get; set; }
    }

    public class MemberOperationsInnerClass
    {
        public string Message { get; set; }
        public string NoochId { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class MemberEditResultClass
    {
        public string Message { get; set; }
        public string NoochId { get; set; }
        public bool IsSuccess { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string recoveryemail { get; set; }
        public string secondaryemail { get; set; }
        public string contactnum { get; set; }

        public string state { get; set; }
        public string zip { get; set; }
        public string ssn { get; set; }
        public string dob { get; set; }
    }
}