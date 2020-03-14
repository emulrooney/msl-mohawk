using System;
using System.Collections.Generic;
using System.Text;

namespace MSL_APP
{
    public struct InvalidCsvEntry
    {
        public int LineNumber { get; set; }
        public string Content { get; set; }
        public InvalidCsvEntry(int lineNumber, string content)
        {
            LineNumber = lineNumber;
            Content = content;
        }
    }

}
