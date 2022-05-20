using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program.TemplateResponse
{
    public class FormESCP
    {
        [JsonProperty("Credential Pathway")]
        public string CredentialPathway { get; set; }

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

        [JsonProperty("EDSP 454 or equivalent2")]
        public EDSP454OrEquivalent2 EDSP454OrEquivalent2 { get; set; }

        [JsonProperty("ED P 405 or equivalent2")]
        public EDP405OrEquivalent2 EDP405OrEquivalent2 { get; set; }

        [JsonProperty("EDSP 350 or equivalent2")]
        public EDSP350OrEquivalent2 EDSP350OrEquivalent2 { get; set; }

        [JsonProperty("ED P 301 or ED P 302 or equivalent2")]
        public EDP301OrEDP302OrEquivalent2 EDP301OrEDP302OrEquivalent2 { get; set; }

        [JsonProperty("ETEC 110 or equivalent2")]
        public ETEC110OrEquivalent2 ETEC110OrEquivalent2 { get; set; }
        public List<Comments> Comments { get; set; }

        [JsonProperty("Official Section")]
        public OfficialSection OfficialSection { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
 

    public class EDP301OrEDP302OrEquivalent2
    {
        public string SEM { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }
    }

    public class EDP405OrEquivalent2
    {
        public string SEM { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }
    }

    public class EDSP350OrEquivalent2
    {
        public string SEM { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }
    }

    public class EDSP454OrEquivalent2
    {
        public string SEM { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }
    }

    public class ETEC110OrEquivalent2
    {
        public string SEM { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }
    }

    


}
