using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{



    public class DisputeClassDetails
    {

        public string status { get; set; }
        public string DisputeId { get; set; }

        public string TransactionId { get; set; }
        public string Sender { get; set; }

        public string Recipient { get; set; }

        public DateTime? DisputeDate { get; set; }

        public DateTime? ReviewDate { get; set; }
        public DateTime? ResolveDate { get; set; }
        public string AdminUserName { get; set; }


        public string SenderNoochId { get; set; }
        public string ReceipentNoochId { get; set; }


    }

    public class DisputesListAllDetails
    {
        public string NoOFDisputeUnderReview { get; set; }
        public string NoOfTransactionUnderReview { get; set; }
        public List<DisputeClassDetails> DisputeClassDetails { get; set; }

    }
}