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
    
    public partial class RentTransaction
    {
        public System.Guid RentTransactionId { get; set; }
        public Nullable<System.Guid> LandlordId { get; set; }
        public Nullable<System.Guid> TenantId { get; set; }
        public string TransactionStatus { get; set; }
        public Nullable<System.DateTime> TransCreatedOn { get; set; }
        public Nullable<System.DateTime> TransRespondedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> UOBTId { get; set; }
        public Nullable<bool> IsDisputed { get; set; }
        public string DisputeStatus { get; set; }
        public string RaisedBy { get; set; }
        public string RaisedById { get; set; }
        public string AdminNotes { get; set; }
        public string Memo { get; set; }
        public Nullable<bool> IsRecurring { get; set; }
        public Nullable<System.DateTime> NextRecurrTransDueDate { get; set; }
        public Nullable<System.Guid> RecentRecurrTransId { get; set; }
        public string Attachment { get; set; }
        public string TransactionType { get; set; }
        public string Amount { get; set; }
        public Nullable<int> NoOfTimesToRecurr { get; set; }
        public string UserPreferredDayOfMonthForAutoPay { get; set; }
    
        public virtual Landlord Landlord { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual UnitsOccupiedByTenant UnitsOccupiedByTenant { get; set; }
    }
}
