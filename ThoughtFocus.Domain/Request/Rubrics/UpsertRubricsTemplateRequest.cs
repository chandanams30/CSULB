using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Milestones;
using ThoughtFocus.Domain.Response;

namespace ThoughtFocus.Domain.Request.Rubrics
{
    public class UpsertRubricsTemplateRequest
    {
        public int ID { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public string TemplateForm { get; set; }
        public bool IsReadytoPublish { get; set; }
        public int TotalPoints { get; set; }
        public int CreatedBy { get; set; }
    }
    public class PublishRubricsFormRequest
    {
        public int TemplateID { get; set; }
        public string TermCode { get; set; }
        public int ProgramID { get; set; }
        public int RubricTypeID { get; set; }
        public int ReviewerTypeID { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
