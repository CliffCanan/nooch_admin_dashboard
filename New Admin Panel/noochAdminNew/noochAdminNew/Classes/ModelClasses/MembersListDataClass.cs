using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class MembersListDataClass
    {
        public string Nooch_ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string ContactNumber { get; set; }

        public string Status { get; set; }

        public DateTime DateCreated { get; set; }

        public string Address { get; set; }
        public string City { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsVerifiedPhone { get; set; }

        public int TotalTransactions { get; set; }
        public string TotalAmountSent { get; set; }
    }



    public class MembersListRefCodeUsedDataClass
    {
        
        public string Name { get; set; }
        public string NoochId { get; set; }
        

        public string CodeUsed { get; set; }
        public string DateUsed { get; set; }

        
    }
}