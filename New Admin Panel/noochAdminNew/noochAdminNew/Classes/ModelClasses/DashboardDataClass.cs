using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class DashboardDataClass
    {
        public int TotalActiveUsers { get; set; }
        public int TotalVerifiedPhoneUsers { get; set; }
        public int TotalVerifiedEmailUsers { get; set; }
        public int TotalActiveBankAccountUsers { get; set; }

        public string TransData { get; set; }

        public string TotalAmountOfDollars { get; set; }

        public int TotalNoOfPaymentsCompleted { get; set; }

        public int totalRequestTypeTrans { get; set; }

        public int TransactionsPendi { get; set; }

        public int TotalRegisteredUsers { get; set; }
        public int TotalSuspendedUsers { get; set; }
        public int TotalVerifiedPhoneAndEmailUsers { get; set; }

        public int TotalActiveAndVerifiedBankAccountUsers { get; set; }

        public int TotalNoOfActiveUser_Today { get; set; }
        public int TotalNoOfActiveUser_Week { get; set; }
        public int TotalNoOfActiveUser_Month { get; set; }
        public int TotalNoOfVerifiedPhoneUsers_Week { get; set; }
        public int TotalNoOfVerifiedPhoneUsers_Today { get; set; }
        public int TotalNoOfVerifiedPhoneUsers_Month { get; set; }

        public int TotalNoOfVerifiedEmailUsers_Today { get; set; }
        public int TotalNoOfVerifiedEmailUsers_Week { get; set; }
        public int TotalNoOfVerifiedEmailUsers_Month { get; set; }
        public List<NoOfUsersInEachBank> UserCountInEachBank { get; set; }
    }

    public class NoOfUsersInEachBank
    {
        public string BankName { get; set; }
        public int NoOfUsers { get; set; }
        //public bool IsDefault { get; set; }
    }

    public class DashBoardLiveTransactionsOperationResult
    {
        public List<MemberRecentLiveTransactionData> RecentLiveTransaction { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }


    public class MemberRecentLiveTransactionData
    {
        public string TransID { get; set; }
        public DateTime? TransDateTime { get; set; }
        public string Amount { get; set; }
        public string RecepientId { get; set; }
        public string SenderId { get; set; }

        public string SenderUserName { get; set; }
        public string RecepientUserName { get; set; }
        public string GeoStateCityLocation { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
        public string DisputeStatus { get; set; }
        public string SynapseStatus { get; set; }
    }
}