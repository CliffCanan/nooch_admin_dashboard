using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class AdminUserClass
    {
       
            public Guid UserId { get; set; }
            public string AdminLevel { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public DateTime DateCreated { get; set; }
            public Guid CreatedBy { get; set; }
            public bool ChangePasswordDone { get; set; }
            public DateTime DateModified { get; set; }
            public Guid ModifiedBy { get; set; }
        
    }
}