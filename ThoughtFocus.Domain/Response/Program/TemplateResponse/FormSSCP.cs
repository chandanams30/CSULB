using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program.TemplateResponse
{
    public class FormSSCP
    {
        [JsonProperty("Credential Subject Area")]
        public string CredentialSubjectArea { get; set; }

        [JsonProperty("Application for")]
        public string ApplicationFor { get; set; }

        [JsonProperty("semester/year")]
        public string SemesterYear { get; set; }

        [JsonProperty("CSULB Campus ID #")]
        public string CSULBCampusID { get; set; }

        [JsonProperty("Social Security #")]
        public string SocialSecurity { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }

        [JsonProperty("Bachelor’s Degree Major")]
        public string BachelorSDegreeMajor { get; set; }
        public string Institution { get; set; }

        [JsonProperty("Highest Degree Earned")]
        public string HighestDegreeEarned { get; set; }

        [JsonProperty("Class Standing at Time of Admission to SSCP")]
        public string ClassStandingAtTimeOfAdmissionToSSCP { get; set; }

        [JsonProperty("Subject Matter Competence")]
        public string SubjectMatterCompetence { get; set; }

        [JsonProperty("APPLICANTS CURRENTLY TEACHING")]
        public APPLICANTSCURRENTLYTEACHING APPLICANTSCURRENTLYTEACHING { get; set; }

        [JsonProperty("PREVIOUS EXPERIENCE WORKING WITH CHILDREN")]
        public string PREVIOUSEXPERIENCEWORKINGWITHCHILDREN { get; set; }
        public List<Comments> Comments { get; set; }

        [JsonProperty("Official Section")]
        public OfficialSection OfficialSection { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Address
    {
        [JsonProperty("Number/Street")]
        public string NumberStreet { get; set; }

        [JsonProperty("Apt#")]
        public string Apt { get; set; }
        public string City { get; set; }

        [JsonProperty("Zip Code")]
        public string ZipCode { get; set; }

        [JsonProperty("Date of Birth")]
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }

        [JsonProperty("Alternate Email")]
        public string AlternateEmail { get; set; }

        [JsonProperty("CSULB Email")]
        public string CSULBEmail { get; set; }
    }

    public class APPLICANTSCURRENTLYTEACHING
    {
        [JsonProperty("Please indicate your current teaching status")]
        public string PleaseIndicateYourCurrentTeachingStatus { get; set; }

        [JsonProperty("District Permit")]
        public string DistrictPermit { get; set; }

        [JsonProperty("Substitute Permit")]
        public string SubstitutePermit { get; set; }

        [JsonProperty("Subject Taught")]
        public string SubjectTaught { get; set; }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }

    public class Comments
    {
        public string User { get; set; }
        public string UserType { get; set; }
        public string CommandType { get; set; }
        public string Date { get; set; }
        public string Comment { get; set; }
        public List<Attachment> Attachments { get; set; }
    }

    public class Name
    {
        public string Last { get; set; }
        public string First { get; set; }

        [JsonProperty("M.I.")]
        public string MI { get; set; }

        [JsonProperty("Other Names")]
        public string OtherNames { get; set; }
    }

    public class OfficialSection
    {
        public string Date { get; set; }
        public string GPS { get; set; }
    }

   


}
