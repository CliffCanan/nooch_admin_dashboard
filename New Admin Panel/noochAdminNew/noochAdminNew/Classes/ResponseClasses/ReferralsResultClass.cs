using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ResponseClasses
{
    public class ReferralsResultClass
    {
        public string MemberName { get; set; }
        public string NoochId { get; set; }

        public string RefCode { get; set; }

        public int ReferralsCount { get; set; }
    }
}