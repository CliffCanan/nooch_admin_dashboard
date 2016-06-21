using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ResponseClasses
{
    public class SaveVerificationIdDocument
    {
        public byte[] Picture { get; set; }
        public string imgPath { get; set; }
        public string MemberId { get; set; }
        public string AccessToken { get; set; }
        public bool IsPdf { get; set; }
    }
}