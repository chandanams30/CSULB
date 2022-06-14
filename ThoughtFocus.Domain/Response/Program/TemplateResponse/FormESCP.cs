using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program.TemplateResponse
{
    public class FormESCP
    {
        public string ApplicationnNumber { get; set; }
        public CredentialPathway CredentialPathway { get; set; }
        public string Applicationfor { get; set; }
        public string CSULBCampusID { get; set; }
        public string SocialSecurityNumber { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public string DateOfBirth { get; set; }
        public string BachelorsDegreeMajor { get; set; }
        public string Institution { get; set; }
        public HighestDegreeEarned HighestDegreeEarned { get; set; }
        public List<BeginningWithTheMostRecentListOfCollege> Beginning_With_The_Most_Recent_ListOfColleges { get; set; }
        public EDSP454OrEquivalent2 EDSP_454_or_equivalent2 { get; set; }
        public EDP405OrEquivalent2 ED_P_405_or_equivalent2 { get; set; }
        public EDSP350OrEquivalent2 EDSP_350_or_equivalent2 { get; set; }
        public EDP301OrEDP302OrEquivalent2 ED_P_301_or_ED_P_302_or_equivalent2 { get; set; }
        public ETEC110OrEquivalent2 ETEC_110_or_equivalent2 { get; set; }
        public List<Comments> Comments { get; set; }
        public OfficialSection Official_Section { get; set; }
    }

    public class BeginningWithTheMostRecentListOfCollege
    {
        public string CollegeUniversity { get; set; }
        public string State { get; set; }
        public string DatesAttended { get; set; }
        public string DegreeOrCredentialEarned { get; set; }
    }



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

    public class HighestDegreeEarned
    {
        public List<string> Choices { get; set; }
        public string Selected { get; set; }
    }

}
