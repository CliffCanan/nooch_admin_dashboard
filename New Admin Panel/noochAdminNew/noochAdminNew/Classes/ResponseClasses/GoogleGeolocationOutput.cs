using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ResponseClasses
{
    public class GoogleGeolocationOutput
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string GoogleStatus { get; set; }
        public string CompleteAddress { get; set; }
        public string StateName { get; set; }

        public string Zip { get; set; }
    }
}