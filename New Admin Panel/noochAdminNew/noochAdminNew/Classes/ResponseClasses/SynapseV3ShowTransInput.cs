using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ResponseClasses
{
    public class SynapseV3ShowTransInput
    {
        public SynapseV3TransInput_login login { get; set; }
        public SynapseV3TransInput_user user { get; set; }
        public SynapseV3ShowTransInput_filter filter { get; set; }
    }
    public class SynapseV3TransInput_login
    {
        public string oauth_key { get; set; }
    }
    public class SynapseV3TransInput_user
    {
        public string fingerprint { get; set; }
    }
    public class SynapseV3ShowTransInput_filter
    {
        public SynapseV3ShowTransInput_filter_Trans_id _id { get; set; }
        public SynapseV3ShowTransInput_filter_from from { get; set; }
        public SynapseV3ShowTransInput_filter_to to { get; set; }
        public SynapseV3ShowTransInput_filter_recent_status recent_status { get; set; }
        public SynapseV3ShowTransInput_filter_amount amount { get; set; }
       
        public SynapseV3ShowTransInput_filter_extra extra { get; set; }
        public string page { get; set; }
 
    }
    public class SynapseV3ShowTransInput_filter_Trans_id
    {

        [JsonProperty(PropertyName = "$oid")]
        public string oid { get; set; }
    }
    public class SynapseV3ShowTransInput_filter_from
    {
        public string type { get; set; }
        public string id { get; set; }
    }
    public class SynapseV3ShowTransInput_filter_to
    {
        public string type { get; set; }
        public string id { get; set; }
    }
    public class SynapseV3ShowTransInput_filter_amount
    {
        public double amount { get; set; }
     
    }
    public class SynapseV3ShowTransInput_filter_extra
    {
        public string supp_id { get; set; }
        public string note { get; set; }
        public string webhook { get; set; }   
       
        public string ip { get; set; }
    }
    public class SynapseV3ShowTransInput_filter_recent_status
    {
        public string status_id { get; set; }
    }
     

}