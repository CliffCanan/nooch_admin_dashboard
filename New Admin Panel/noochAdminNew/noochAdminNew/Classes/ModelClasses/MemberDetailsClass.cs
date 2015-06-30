﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class MemberDetailsClass
    {
        public string Nooch_ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UDID1 { get; set; }
        public string UserName { get; set; }
        public string SecondaryEmail { get; set; }
        public string RecoveryEmail { get; set; }
        public string TertiaryEmail { get; set; }
        public string PinNumber { get; set; }
        public string Password { get; set; }
        public string ContactNumber { get; set; }
        public string Status { get; set; }
        public string ImageURL { get; set; }

        public string FBID { get; set; }

        public bool IsKnocAvailable { get; set; }

        public string KnoxBankIcon { get; set; }
        public string ReferCodeUsed { get; set; }
        public string KnoxTransId { get; set; }

        public bool IsPhoneVerified { get; set; }

        public DateTime DateCreated { get; set; }
        public string DateCreatedFormatted { get; set; }

        public int WeeksSinceJoined { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }

        public List<MemberDetailsTrans> Transactions { get; set; }

        public List<MemberDetailsDisputeTrans> DisputeTransactions { get; set; }

        public MemberDetailsStats MemberStats { get; set; }

        public List<ReferredMembers> Referrals { get; set; }

        public bool IsSynapseDetailAvailable { get; set; }
        public SynapseDetailOFMember SynapseDetails { get; set; }
        public List<MemberIpAddrreses> MemberIpAddr { get; set; }
    }

    public class MemberDetailsTrans
    {
        public string TransID { get; set; }
        public string TransDateTime { get; set; }
        public string Amount { get; set; }
        public string AmountNew { get; set; }
        public string RecepientId { get; set; }
        public string RecepientUserName { get; set; }
        public string GeoLocation { get; set; }
        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }

    }

    public class MemberDetailsStats
    {
        public string TotalTransfer { get; set; }
        public string TotalSent { get; set; }
        public string TotalReceived { get; set; }
        public string LargestSent { get; set; }
        public string LargestReceived { get; set; }
        

    }
    public class MemberDetailsDisputeTrans
    {
        public string Status { get; set; }
        public string Dispute_ID { get; set; }
        public string Subject { get; set; }
        public string Admin_Notes { get; set; }
        public string Dispute_Date { get; set; }
        public string Review_Date { get; set; }
        public string Resolved_Date { get; set; }
        public string RecepientUserName { get; set; }
        public string RecepientId { get; set; }
        public string TransDateTime { get; set; }
        public string TransactionStatus { get; set; }

        

        public string Transaction_ID { get; set; }


        public string GeoLocation { get; set; }
        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string TransactionType { get; set; }

    }

    public class ReferredMembers
    {
        public string MemberName { get; set; }
        public string ImageUrl { get; set; }
        

    }

    public class MemberIpAddrreses
    {
        public string IpAddress { get; set; }
        public DateTime Date { get; set; }
    }



    public class SynapseDetailOFMember
    {
        public string SynpaseBankStatus { get; set; }

        public string SynpaseBankName { get; set; }

    }
}