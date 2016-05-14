using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class MemberDetailsClass
    {
        public string memberId { get; set; }
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
        public string dob { get; set; }
        public string ssn { get; set; }
        public string idDocUrl { get; set; }
        public string FBID { get; set; }
        public string DeviceToken { get; set; }
        public string AccessToken { get; set; }
        public string type { get; set; }
        public string ReferCodeUsed { get; set; }
        public bool IsPhoneVerified { get; set; }

        public DateTime DateCreated { get; set; }
        public string DateCreatedFormatted { get; set; }
        public int WeeksSinceJoined { get; set; }

        public string Address { get; set; }
        public string TransferLimit { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string lastlat { get; set; }
        public string lastlong { get; set; }

        public string adminNote { get; set; }
        public bool isSdnSafe { get; set; }

        public List<MemberDetailsTrans> Transactions { get; set; }

        public List<MemberDetailsDisputeTrans> DisputeTransactions { get; set; }

        public MemberDetailsStats MemberStats { get; set; }

        public List<ReferredMembers> Referrals { get; set; }

        public bool IsSynapseDetailAvailable { get; set; }
        public bool? IsVerifiedWithSynapse { get; set; }
        public SynapseDetailOFMember SynapseDetails { get; set; }
        public List<MemberIpAddrreses> MemberIpAddr { get; set; }

        public List<Tenants> tenants { get; set; }
        public Tenants tenant { get; set; }
        public string DocStatus { get; set; }
    }

    public class MemberDetailsTrans
    {
        public string TransID { get; set; }
        public string TransDate { get; set; }
        public string TransTime { get; set; }
        public string Amount { get; set; }
        public string AmountNew { get; set; }
        public string RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientUserName { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string GeoLocation { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
        public string Memo { get; set; }
        public string UDID { get; set; }
        public string DeviceToken { get; set; }
        public string AccessToken { get; set; }
        public bool IsUnUsualTrans { get; set; }
        public string SynapseStatus { get; set; }


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
        // Bank-Related
        public int BankId { get; set; }
        public string synapseBankId { get; set; }

        public string SynapseBankStatus { get; set; }
        public string SynapseBankName { get; set; }
        public string SynapseBankNickName { get; set; }
        public bool mfaVerified { get; set; }

        public string nameFromSynapseBank { get; set; }
        public string emailFromSynapseBank { get; set; } // No longer passed by Synapse in V3
        public string phoneFromSynapseBank { get; set; } // No longer passed by Synapse in V3

        public string SynpaseBankAddedOn { get; set; }
        public string SynpaseBankVerifiedOn { get; set; }
        public string allowed { get; set; }

        public string bankClass { get; set; }
        public string bankType { get; set; }
        public string nodeType { get; set; }

        // User-Related
        public string userDateCreated { get; set; }
        public string synapseConsumerKey { get; set; }
        public string synapseRefreshKey { get; set; }
        public string synapseUserId { get; set; }

        public bool isBusiness { get; set; }
        public string userPermission { get; set; }
        public string photos { get; set; }
    }

    public class Tenants
    {
        public string Property { get; set; }
        public string Unit { get; set; }
        public string Rent { get; set; }
        public string Status { get; set; }
        public string LastPayment { get; set; }

        public DateTime? LastPaymentDate { get; set; }
        public string LastPaymentDate1 { get; set; }
        public string dueDate { get; set; }
        public bool? AutoPay { get; set; }
        public string LeaseLength { get; set; }
        public string AdminNote { get; set; }
    }

}