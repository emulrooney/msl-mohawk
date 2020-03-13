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

        public string Key { get; set; }

        // New or Used key. Using integer 0 as new, 1 as used. 0 by default
        [Required, Display(Name = "Used")]
        public int UsedKey { get; set; }

        // The owner's id will be the student ID, this parameter is nullable
        [Display(Name = "Owner Id")]
        public int? OwnerId { get; set; }

        public virtual ProductName ProductName { get; set; }
    }
}
