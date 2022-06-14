using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program
{
    public class  FormTemplateUDCP
    {
        public string ApplicationnNumber { get; set; }
        public UDCPPathway UDCPPathway { get; set; }
        public string Applicationfor { get; set; }
        public string CSULBCampusID { get; set; }
        public string SocialSecurityNumber { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public string DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string NativeLanguage { get; set; }
        public string BachelorsDegreeMajor { get; set; }
        public string Institution { get; set; }
        public HighestDegreeEarned HighestDegreeEarned { get; set; }
        public List<BeginningWithTheMostRecentListOfCollege> Beginning_With_The_Most_Recent_ListOfColleges { get; set; }
        public List<Comments> Comments { get; set; }
        public OfficialSection OfficialSection { get; set; }
        public dynamic Documents { get; set; }
    }

    public class Address
    {
        public string Number_Street { get; set; }
        public string AptNumber { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string AlternateEmail { get; set; }
        public string CSULBEmail { get; set; }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }

    public class BeginningWithTheMostRecentListOfCollege
    {
        public string CollegeUniversity { get; set; }
        public string State { get; set; }
        public string DatesAttended { get; set; }
        public string DegreeOrCredentialEarned { get; set; }
    }

    public class Comments
    {
        public string User { get; set; }
        public List<string> UserType { get; set; }
        public List<string> CommandType { get; set; }
        public string Date { get; set; }
        public string Comment { get; set; }
        public List<Attachment> Attachments { get; set; }
    }

    public class Gender
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }

    public class HighestDegreeEarned
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }

    public class Name
    {
        public string Last { get; set; }
        public string First { get; set; }
        public string MI { get; set; }
        public string OtherNames { get; set; }
    }

    public class OfficialSection
    {
        public string Date { get; set; }
        public string GPS { get; set; }
    }

   

    public class UDCPPathway
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }


}
