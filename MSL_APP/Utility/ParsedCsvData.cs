using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Utility
{
    /// <summary>
    /// Simple class to hold parsed data. Valid entries are added to a list of strings (to be turned
    /// into appropriate objects later) while invalid entries are stored along with their 
    /// </summary>
    public class ParsedCsvData
    {
        public List<string> ValidList { get; set; } = new List<string>();
        public List<InvalidCsvEntry> InvalidList { get; set; } = new List<InvalidCsvEntry>();


    }
}
