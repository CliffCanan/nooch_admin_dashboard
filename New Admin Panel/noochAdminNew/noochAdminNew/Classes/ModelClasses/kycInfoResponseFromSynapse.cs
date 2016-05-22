using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noochAdminNew.Classes.ModelClasses
{
    public class kycInfoResponseFromSynapse
    {
        public synapseV3Response_message message { get; set; }
        public bool success { get; set; }
        public synapseV3Result_user user { get; set; }

        public synapseV3Response_error error { get; set; }
        public synapseIdVerificationQuestionSet question_set { get; set; } // Only returned for */user/doc/verify* if further verification required
    }

    public class synapseV3Response_message
    {
        public string en { get; set; }
    }

    public class synapseIdVerificationQuestionSet
    {
        public int created_at { get; set; }
        public bool expired { get; set; }
        public string id { get; set; }
        public bool livemode { get; set; }
        public string obj { get; set; }
        public string person_id { get; set; }
        public string score { get; set; }
        public int time_limit { get; set; }
        public int updated_at { get; set; }
        public List<synapseIdVerificationQuestionAnswerSet> questions { get; set; }
    }

    public class synapseIdVerificationQuestionAnswerSet
    {
        public short id { get; set; }
        public string question { get; set; }
        public List<synapseIdVerificationAnswerChoices> answers { get; set; }
    }

    public class synapseIdVerificationAnswerChoices
    {
        public short id { get; set; }
        public string answer { get; set; }
    }

    public class synapseIdVerificationAnswersInput
    {
        public SynapseV3Input_login login { get; set; }
        public synapseSubmitIdAnswers_answers_input user { get; set; }
    }

    public class synapseSubmitIdAnswers_answers_input
    {
        //public SynapseV3Input_login login { get; set; }
        public synapseSubmitIdAnswers_docSet doc { get; set; }
        public string fingerprint { get; set; }
    }

    public class synapseSubmitIdAnswers_docSet
    {
        public string question_set_id { get; set; }
        public synapseSubmitIdAnswers_Input_quest[] answers { get; set; }
    }

    public class synapseSubmitIdAnswers_Input_quest
    {
        public int question_id { get; set; }
        public int answer_id { get; set; }
    }

    public class synapseV3Result_user  // RESULT CLASS
    {
        public synapseV3Result_user_id _id { get; set; }
        public synapseV3Result_user_client client { get; set; }
        public synapseV3Result_user_extra extra { get; set; }
        public string[] legal_names { get; set; }
        public synapseV3Result_user_logins[] logins { get; set; }
        public bool is_hidden { get; set; }

        public string permission { get; set; }
        public string[] photos { get; set; }
        public string[] phone_numbers { get; set; }

        public synapseV3Result_user_docStatus doc_status { get; set; }
    }

    public class synapseV3Result_user_client
    {
        public string id { get; set; } // This is an integer ID
        public string name { get; set; }
    }

    public class synapseV3Result_user_extra
    {
        public synapseV3Result_user_extra_dateJoined date_joined { get; set; }
        public bool is_business { get; set; }
        public string supp_id { get; set; }
    }

    public class synapseV3Result_user_logins
    {
        public string email { get; set; }
        public bool read_only { get; set; }
    }

    public class synapseV3Result_user_extra_dateJoined
    {
        public DateTime date { get; set; }
    }

    public class synapseV3Result_user_docStatus
    {
        public string physical_doc { get; set; } // This is an integer ID
        public string virtual_doc { get; set; }
    }
}