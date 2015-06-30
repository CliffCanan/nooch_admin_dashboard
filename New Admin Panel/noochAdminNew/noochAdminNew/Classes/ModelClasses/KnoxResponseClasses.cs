using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class KnoxResponseClasses
    {
    }
    class ResponseClass3
    {
        public InnerClass3 JSonDataResult { get; set; }
    }
    class InnerClass3
    {
        public string trans_id { get; set; }
        public string status_code { get; set; }
        public string error_code { get; set; }


    }
}