using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program
{
    public class FormTemplateESCP
    {
        [JsonProperty("Credential Pathway")]
        public CredentialPathway CredentialPathway { get; set; }

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

        [JsonProperty("Bachelor’s Degree Major")]
        public string BachelorSDegreeMajor { get; set; }
        public string Institution { get; set; }

        [JsonProperty("Highest Degree Earned")]
        public HighestDegreeEarned HighestDegreeEarned { get; set; }

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
   

    public class CredentialPathway
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }

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

    #region Extra Classes
    //public class HighestDegreeEarned
    //{
    //    public List<string> Choices { get; set; }
    //    public string Selected { get; set; }
    //}

    //public class Name
    //{
    //    public string Last { get; set; }
    //    public string First { get; set; }

    //    [JsonProperty("M.I.")]
    //    public string MI { get; set; }

    //    [JsonProperty("Other Names")]
    //    public string OtherNames { get; set; }
    //}

    //public class OfficialSection
    //{
    //    public string Date { get; set; }
    //    public string GPS { get; set; }
    //}

    //public class Address
    //{
    //    [JsonProperty("Number/Street")]
    //    public string NumberStreet { get; set; }

    //    [JsonProperty("Apt#")]
    //    public string Apt { get; set; }
    //    public string City { get; set; }

    //    [JsonProperty("Zip Code")]
    //    public string ZipCode { get; set; }

    //    [JsonProperty("Date of Birth")]
    //    public string DateOfBirth { get; set; }
    //    public string Phone { get; set; }

    //    [JsonProperty("Alternate Email")]
    //    public string AlternateEmail { get; set; }

    //    [JsonProperty("CSULB Email")]
    //    public string CSULBEmail { get; set; }
    //}

    //public class Attachment
    //{
    //    public string FileName { get; set; }
    //    public string FilePath { get; set; }
    //    public string FileType { get; set; }
    //}

    //public class BEGINNINGWithTehMostRecentListAllCollege
    //{
    //    [JsonProperty("College/University")]
    //    public string CollegeUniversity { get; set; }
    //    public string State { get; set; }

    //    [JsonProperty("Dates Attended")]
    //    public string DatesAttended { get; set; }

    //    [JsonProperty("Degree or Credential Earned")]
    //    public string DegreeOrCredentialEarned { get; set; }
    //}

    //public class Comments
    //{
    //    public string User { get; set; }
    //    public List<string> UserType { get; set; }
    //    public List<string> CommandType { get; set; }
    //    public string Date { get; set; }
    //    public string Comment { get; set; }
    //    public List<Attachment> Attachments { get; set; }
    //}
    #endregion



}
