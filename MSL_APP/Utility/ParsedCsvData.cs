using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Utility
{
    /// <summary>
    /// Simple struct to hold parsed data. Contains two dictionaries using the line number as a key* to store
    /// either the parsed output or the string that failed to parse as the value.
    /// 
    /// Note that the key is NOT an integer to avoid issues with ASP failing to serialize items in TempData
    /// </summary>
    public class ParsedCsvData<T>
    {
        public ParsedCsvData()
        {
            ValidList = new Dictionary<string, T>();
            InvalidList = new Dictionary<string, string>();
        }

        public Dictionary<string, T> ValidList { get; set; }
        public Dictionary<string, string> InvalidList { get; set; }

    }
}
