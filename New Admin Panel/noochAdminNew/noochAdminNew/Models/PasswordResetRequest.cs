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
    
    public partial class PasswordResetRequest
    {
        public int Id { get; set; }
        public Nullable<System.Guid> MemberId { get; set; }
        public Nullable<System.DateTime> RequestedOn { get; set; }
    
        public virtual Member Member { get; set; }
    }
}
