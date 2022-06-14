using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program.TemplateResponse
{
    public class FormGraduatePrograms
    {
        public string ApplicationNumber { get; set; }
        public Name Name { get; set; }
        public Email Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CSULBCampusID { get; set; }
        public string LanguagesSpokenOtherThanEnglish { get; set; }
        public string SemesterofApplication { get; set; }
        public string Program { get; set; }
        public List<Comments> Comments { get; set; }
        public dynamic Documents { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class Email
    {
        public string EmailAddress { get; set; }
        public string CSULBEmail { get; set; }
    }
}
