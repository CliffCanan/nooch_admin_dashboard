//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace noochAdminNew.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class InviteCode
    {
        public System.Guid InviteCodeId { get; set; }
        public string code { get; set; }
        public int count { get; set; }
        public int totalAllowed { get; set; }
        public string AdminNotes { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
    }
}
