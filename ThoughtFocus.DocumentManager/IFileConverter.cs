using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ThoughtFocus.DocumentManager
{
    public interface IFileConverter
    {
        void ConvertDocumentToPDF(string srcFilename, string dstFilename);
        //void ConvertSpreadsheetToPDF(string srcFilename, string dstFilename);
        void ConvertImageToPDF(string srcFilename, string dstFilename);
        void ConvertHTMLToPDF(string srcFilename, string dstFilename);
        void Merge(String OutFile, string[] mergelist);
        void Merge(string OutFile, MemoryStream[] msList);
        //void ConvertWorksheetRangeToPDF();
        //void ConvertWorksheetToPDF(string srcFileName, string sheetName);
    }
}
