using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FormSaveRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public PersonalInfo PersonalInfo { get; set; }
        public List<FormAttachment> FormAttachments { get; set; }
        public List<Recommender> Recommenders { get; set; }
        public FormAttachment edelFieldWorkAttachmentsInformation { get; set; }


    }

    public class PersonalInfo
    {
        public string formSchema { get; set; }
    }
    public class FormAttachment
    {
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
    public class Recommender
    {
        public string RecommenderName { get; set; }
        public string RecommenderEmail { get; set; }
        public string RecommenderAffiliation { get; set; }
    }

    public class FormSaveGridNotesRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string GridNotes { get; set; }


    }
}
