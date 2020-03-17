using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class StudentKey
    {
        [Display(Name = "Product")]
        public string Product { get; set; }

        [Display(Name = "Key")]
        public string Key { get; set; }

        [Display(Name = "Download Link")]
        public string DownloadLink { get; set; }
    }
}
