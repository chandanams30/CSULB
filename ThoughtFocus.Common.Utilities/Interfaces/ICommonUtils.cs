using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ThoughtFocus.Common.Utilities.Implementation;

namespace ThoughtFocus.Common.Utilities.Interfaces
{
    public interface ICommonUtils
    {
        DataTable ToDataTable<T>(List<T> items);
        AttachmentFileDetails GetAttachedFileSplitValues(string filename);
        byte[] GetImageFilecontent(byte[] fileContent);
        byte[] GetFileContent(string repoPath, string userFolderPath, string fileName);
        string GetFileType(string fileExt);
        string GetAttachmentsFolderName(string combinedString);
        string GetAttachmentsSavedFileName(string combinedString);
    }
}
