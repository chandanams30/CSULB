using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IDocumentService
    {
        byte[] GetMergedDocument(int userId, int formId);
        byte[] GetDocument(int userId, int formAttachmentId);
    }
}
