using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program.TemplateResponse
{
    public class FormMSCP
    {
        [JsonProperty("MSCP Pathway")]
        public string MSCPPathway { get; set; }

        [JsonProperty("Application for")]
        public string ApplicationFor { get; set; }

        [JsonProperty("CSULB Campus ID #")]
        public string CSULBCampusID { get; set; }

        [JsonProperty("Social Security #")]
        public string SocialSecurity { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }

        [JsonProperty("Date of Birth")]
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }

        [JsonProperty("Bachelor’s Degree Major")]
        public string BachelorSDegreeMajor { get; set; }
        public string Institution { get; set; }

        [JsonProperty("Highest Degree Earned")]
        public string HighestDegreeEarned { get; set; }

        [JsonProperty("BEGINNING with teh most recent, list all colleges")]
        public List<BEGINNINGWithTehMostRecentListAllCollege> BEGINNINGWithTehMostRecentListAllColleges { get; set; }

        [JsonProperty("EDEL 380/200 or equivalent1")]
        public EDEL380200OrEquivalent1 EDEL380200OrEquivalent1 { get; set; }

        [JsonProperty("EDSP 303/355A or equivalent1")]
        public EDSP303355AOrEquivalent1 EDSP303355AOrEquivalent1 { get; set; }
        public List<Comments> Comments { get; set; }

        [JsonProperty("Official Section")]
        public OfficialSection OfficialSection { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class BEGINNINGWithTehMostRecentListAllCollege
    {
        [JsonProperty("College/University")]
        public string CollegeUniversity { get; set; }
        public string State { get; set; }

        [JsonProperty("Dates Attended")]
        public string DatesAttended { get; set; }

        [JsonProperty("Degree or Credential Earned")]
        public string DegreeOrCredentialEarned { get; set; }
    }

    public class EDEL380200OrEquivalent1
    {
        public string SEM { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }
    }

    public class EDSP303355AOrEquivalent1
    {
        public string SEM { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }
    }

}
