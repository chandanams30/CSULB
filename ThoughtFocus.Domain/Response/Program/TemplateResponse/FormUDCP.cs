using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program.TemplateResponse
{
    public class FormUDCP
    {
        [JsonProperty("UDCP Pathway")]
        public UDCPPathway UDCPPathway { get; set; }

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
        public List<Comments> Comments { get; set; }

        [JsonProperty("Official Section")]
        public OfficialSection OfficialSection { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    

    public class UDCPPathway
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }


}
