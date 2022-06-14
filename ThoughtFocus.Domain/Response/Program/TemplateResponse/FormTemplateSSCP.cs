using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program
{
    public class FormTemplateSSCP
    {
        public string ApplicationnNumber { get; set; }
        public CredentialSubjectArea CredentialSubjectArea { get; set; }
        public string Applicationfor { get; set; }
        public string CSULBCampusID { get; set; }
        public string SocialSecurityNumber { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public string BachelorsDegreeMajor { get; set; }
        public string Institution { get; set; }
        public HighestDegreeEarned HighestDegreeEarned { get; set; }
        public ClassStandingAtTimeOfAdmissionToSSCP Class_Standing_At_Time_of_Admission_to_SSCP { get; set; }
        public SubjectMatterCompetence Subject_Matter_Competence { get; set; }
        public APPLICANTSCURRENTLYTEACHING APPLICANTS_CURRENTLY_TEACHING { get; set; }
        public string PREVIOUS_EXPERIENCE_WORKING_WITH_CHILDREN { get; set; }
        public List<Comments> Comments { get; set; }
        public OfficialSection OfficialSection { get; set; }
        public dynamic Documents { get; set; }
    }


    public class APPLICANTSCURRENTLYTEACHING
    {
        public PleaseIndicateYourCurrentTeachingStatus Please_indicate_your_current_teaching_status { get; set; }

        [JsonProperty("District Permit")]
        public string DistrictPermit { get; set; }

        [JsonProperty("Substitute Permit")]
        public string SubstitutePermit { get; set; }

        [JsonProperty("Subject Taught")]
        public string SubjectTaught { get; set; }
    }



    public class ClassStandingAtTimeOfAdmissionToSSCP
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }

   

    public class CredentialSubjectArea
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }



    public class PleaseIndicateYourCurrentTeachingStatus
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }


    public class SubjectMatterCompetence
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }



}
