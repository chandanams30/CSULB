using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Rubrics
{
    public class RubricsTemplateListResponse : BaseResponse
    {
        public List<RubricsTemplateList> RubricsTemplateList { get; set; }
    }
    public class GetRubricSubmittedFormsListResponse : BaseResponse
    {
        public List<GetRubricSubmittedFormsResponse> rubricSubmittedFormsList { get; set; }
    }
    public class PublishedRubricsDetailsResponse : BaseResponse
    {
        public RubricsDetails rubricsDetails{ get; set;}
    }
    public class RubricsApplicationFormResponse : BaseResponse
    {
        public RubricsApplicationForm rubricsApplicationForm { get; set;}
        public RubricsFormActivityHandler rubricsFormActivityHandler { get; set;}
    }
    public class RubricsTemplateList
    {
        public int ID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public bool IsReadytoPublish { get; set; }
        public int TotalPoints { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPublished { get; set; }
    }
    public class GetRubricSubmittedFormsResponse
    {
        public int FilledRubricID { get; set; }
        public int PublishRubricID { get; set; }
        public int TemplateID { get; set; }
        public string StudentName { get; set; }
        public int CSULBID { get; set; }
        public string ProgramName { get; set; }
        public string TemplateName { get; set; }
        public int FormID { get; set; }
        public string TermName { get; set; }
        public string ReviewerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReviewerID { get; set; }
        public string TermCode { get; set; }
        public string State { get; set; }
    }
    public class RubricsDetails
    {
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public string TermName { get; set; }
        public string ProgramName { get; set; }
        public string TemplateForm { get; set; }
        public int TotalPoints { get; set; }
    }
    public class RubricsApplicationForm
    {
        public int FilledRubricID { get; set; }
        public int PublishedRubricID { get; set; }
        public int FormID { get; set; }
        public string RubricForm { get; set; }
        public int UserID { get; set; }
        public int Status { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public int ApplicationTypeID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public int TotalPoints { get; set; }
        public string StudentName { get; set; }
        public string ProgramName { get; set; }
        public string TermName { get; set; }
        public string StudentEmail { get; set; }
        public int CSULBID { get; set; }
    }
    public class RubricsFormActivityHandler
    {
        public string ActivityHandler { get; set; }
    }
}
