using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FileHelpers;

namespace noochAdminNew.Classes.ModelClasses
{
    public class OfacList
    {
        [DelimitedRecord("|")]
        [IgnoreLast(1)]


        public class SDNEntity
        {
            public int ent_num { get; set; }
            public string SDN_Name { get; set; }
            public string SDN_Type { get; set; }
            public string Program { get; set; }

            public string Title { get; set; }
            public string Call_Sign { get; set; }
            public string Vess_Type { get; set; }
            public string Tonnage { get; set; }
            public string GRT { get; set; }
            public string Vess_Flag { get; set; }
            public string Vess_Owner { get; set; }
            public string Remarks { get; set; }

        }




        [DelimitedRecord("|")]
        [IgnoreLast(1)]
        public class ADDEntity
        {
            public int ent_num { get; set; }
            public int Add_num { get; set; }
            public string Address { get; set; }
            public string CityStateProvincePostalCode { get; set; }

            public string Country { get; set; }
            public string Add_remarks { get; set; }


        }



        [DelimitedRecord("|")]
        [IgnoreLast(1)]
        public class ALTEntity
        {
            public int ent_num { get; set; }
            public int alt_num { get; set; }
            public string alt_type { get; set; }
            public string alt_name { get; set; }

            public string Country { get; set; }



        }


        public DbSet<SDNEntity> SDNDataSet { get; set; }
    }
}