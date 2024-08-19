using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Rubrics
{
    public class RubricsTemplateListResponse : BaseResponse
    {
        public List<RubricsTemplateList> RubricsTemplateList { get; set; }
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
}
