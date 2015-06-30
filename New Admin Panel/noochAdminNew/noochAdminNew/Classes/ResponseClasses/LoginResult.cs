using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ResponseClasses
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public string Pin { get; set; }
    }
    public class AdminNoteResult
    {
        public bool IsSuccess { get; set; }
        public string AdminNote { get; set; }
        public string Message { get; set; }
        public string Pin { get; set; }
    }
}