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
    
    public partial class TransactionCustomReportEntity
    {
        public System.Guid TransactionId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public Nullable<int> NoOfTransfers { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string State { get; set; }
        public Nullable<int> Age { get; set; }
        public string TotalRows { get; set; }
    }
}
