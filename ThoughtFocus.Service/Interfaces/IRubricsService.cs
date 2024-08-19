using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Rubrics;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Rubrics;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IRubricsService
    {
        RubricsTemplateListResponse GetRubricsTemplateList();
        BaseResponse UpsertRubricsTemplate(UpsertRubricsTemplateRequest input);
        BaseResponse PublishRubricsForm(PublishRubricsFormRequest input);
        RubricsTemplateByIDResponse GetRubricsTemplateByID(int TemplateID);
    }
}
