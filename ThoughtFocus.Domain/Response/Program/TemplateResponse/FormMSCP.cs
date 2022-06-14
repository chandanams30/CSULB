using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program.TemplateResponse
{
    public class FormMSCP
    {
        public string ApplicationnNumber { get; set; }
        public MSCPPathway MSCPPathway { get; set; }
        public string Applicationfor { get; set; }
        public string CSULBCampusID { get; set; }
        public string SocialSecurityNumber { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public string DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string BachelorsDegreeMajor { get; set; }
        public string Institution { get; set; }
        public HighestDegreeEarned HighestDegreeEarned { get; set; }
        public List<BeginningWithTheMostRecentListOfCollege> Beginning_With_The_Most_Recent_ListOfColleges { get; set; }
        public EDEL380200OrEquivalent1 EDEL_380_200_Or_Equivalent1 { get; set; }
        public EDSP303355AOrEquivalent1 EDSP_303_355A_Or_Equivalent1 { get; set; }
        public List<Comments> Comments { get; set; }
        public OfficialSection OfficialSection { get; set; }
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

    public class Gender
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }



    public class MSCPPathway
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }
}
