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
    
    public partial class Property
    {
        public System.Guid PropertyId { get; set; }
        public string PropStatus { get; set; }
        public string PropType { get; set; }
        public string PropName { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ContactNumber { get; set; }
        public string DefaultDueDate { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<System.Guid> LandlordId { get; set; }
        public Nullable<System.Guid> MemberId { get; set; }
        public string PropertyImage { get; set; }
        public Nullable<bool> IsSingleUnit { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> DefaulBank { get; set; }
    }
}