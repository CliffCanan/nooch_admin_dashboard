using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class InviteCodeDataClass
    {
        public string InviteCode_Id { get; set; }
        public string code { get; set; }
        public string count { get; set; }

        public string totalallowed { get; set; }
        public string adminnotes { get; set; }

        public string createdbyadmin_id { get; set; }
        public string createdbyadmin_name { get; set; }

        public string DateCreated { get; set; }

        public string DateModified { get; set; }
    }
}