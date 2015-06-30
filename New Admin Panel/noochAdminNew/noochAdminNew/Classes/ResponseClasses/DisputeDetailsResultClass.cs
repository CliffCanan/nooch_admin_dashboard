using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ResponseClasses
{
    public class DisputeDetailsResultClass
    {
        public string TransactionId { get; set; }
        public string DisputeId { get; set; }
        public string SenderId { get; set; }
        public string ReceiptId { get; set; }
        public string Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionFor { get; set; }
        public string Transactionlocation { get; set; }
        public DateTime? DisputeDate { get; set; }
        public string SenderImg { get; set; }
        public string ReceiptImg { get; set; }

        public string Memo { get; set; }
        //  public List<MemberDisputeDetailsInnerResultClass> MemberDisputeDetailsInnerResultClass { get; set; }
        public bool IsResultSucess { get; set; }
        public string ResultMessage { get; set; }//IsSuccess

    }


    public class MemberDisputeStatusChangeClass
    {
        public string AdminNotee { get; set; }
        public string DisputeStatus { get; set; }
        public string ReceiptFirstName { get; set; }
        public string ReceiptLastName { get; set; }
        public DateTime? DisputeDate { get; set; }
        public string DisputeId { get; set; }
        public string TransactionId { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public decimal Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string TransactionLocation { get; set; }
        public string SenderImg { get; set; }
        public string ReceiptImg { get; set; }

        public DateTime ResolvedDate { get; set; }
        public string AdminName { get; set; }//IsSuccess
        public string RaisedBy { get; set; }//IsSuccess
        public bool IsAllowPushNotifications { get; set; }
        public string UserName { get; set; }
        public string SenderNoochId { get; set; }

        public string ReceiptNoochId { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiptUserName { get; set; }


        public string Message { get; set; }

        public bool IsSuccess { get; set; }



    }
    public class MemberDisputeOperationsResultClass
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public string DisputeId { get; set; }


    }



    public class MemberFinalResultOperation
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public List<MemberDisputeOperationsResultClass> MemberDisputeSUbListClass { get; set; }
    }








}