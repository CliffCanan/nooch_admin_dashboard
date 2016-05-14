using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
     
        public class ResponseFromShowTrans
        {
            public string error_code { get; set; }
            public string http_code { get; set; }
            public int page { get; set; }
            public int page_count { get; set; }
            public bool success { get; set; }
            public Trans[] trans { get; set; }
            public int trans_count { get; set; }
        }

        public class Trans
        {
            public _Id _id { get; set; }
            public Amount amount { get; set; }
            public Client client { get; set; }
            public Extra extra { get; set; }
            public Fee[] fees { get; set; }
            public From from { get; set; }
            public Recent_Status recent_status { get; set; }
            public Timeline[] timeline { get; set; }
            public To to { get; set; }
        }

        public class _Id
        {
            [JsonProperty(PropertyName = "$oid")]
            public string oid { get; set; }
        }

        public class Amount
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class Client
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Extra
        {
            public Created_On created_on { get; set; }
            public string ip { get; set; }
            public string latlon { get; set; }
            public string note { get; set; }
            public Other other { get; set; }
            public Process_On process_on { get; set; }
            public string supp_id { get; set; }
            public string webhook { get; set; }
        }

        public class Created_On
        {
            public long date { get; set; }
        }

        public class Other
        {
        }

        public class Process_On
        {
            public long date { get; set; }
        }

        public class From
        {
            public Id id { get; set; }
            public string nickname { get; set; }
            public string type { get; set; }
            public User user { get; set; }
        }

        public class Id
        {
            [JsonProperty(PropertyName = "$oid")]
            public string oid { get; set; }
        }

        public class User
        {
            public _Id1 _id { get; set; }
            public string[] legal_names { get; set; }
        }

        public class _Id1
        {
            [JsonProperty(PropertyName = "$oid")]
            public string oid { get; set; }
        }

        public class Recent_Status
        {
            public Date date { get; set; }
            public string note { get; set; }
            public string status { get; set; }
            public string status_id { get; set; }
        }

        public class Date
        {
            public long date { get; set; }
        }

        public class To
        {
            public Id1 id { get; set; }
            public string nickname { get; set; }
            public string type { get; set; }
            public User1 user { get; set; }
        }

        public class Id1
        {
            [JsonProperty(PropertyName = "$oid")]
            public string oid { get; set; }
        }

        public class User1
        {
            public _Id2 _id { get; set; }
            public string[] legal_names { get; set; }
        }

        public class _Id2
        {
            [JsonProperty(PropertyName = "$oid")]
            public string oid { get; set; }
        }

        public class Fee
        {
            public float fee { get; set; }
            public string note { get; set; }
            public To1 to { get; set; }
        }

        public class To1
        {
            public Id2 id { get; set; }
        }

        public class Id2
        {
            [JsonProperty(PropertyName = "$oid")]
            public string oid { get; set; }
        }

        public class Timeline
        {
            public Date1 date { get; set; }
            public string note { get; set; }
            public string status { get; set; }
            public string status_id { get; set; }
        }

        public class Date1
        {
            public long date { get; set; }
        }

    
}