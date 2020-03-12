using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class ProductKey
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ProductName")]
        public int NameId { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        // New or Used key. Using integer 0 as new, 1 as used. 0 by default
        public int ActiveStatus { get; set; }

    }
}
