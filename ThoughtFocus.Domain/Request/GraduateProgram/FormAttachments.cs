using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FormAttachments
    {
        public byte[] FileContent { get; set; }
        public string Filename { get; set; }
    }
    public class RecommendationAttachments
    {
        public byte[] FileContent { get; set; }
        public string Filename { get; set; }
    }
}
