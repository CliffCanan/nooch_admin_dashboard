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
        
        public string page { get; set; }
 
    }
    public class SynapseV3ShowTransInput_filter_Trans_id
    {

        [JsonProperty(PropertyName = "$oid")]
        public string oid { get; set; }
    }
  
     

}