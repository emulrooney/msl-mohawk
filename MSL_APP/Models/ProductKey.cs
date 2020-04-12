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

        [ForeignKey("Product")]
        public int NameId { get; set; }

        [Required]
        [RegularExpression(@"^[_A-z0-9]*((-|\s)*[_A-z0-9])*$", ErrorMessage = "Invalid Key Format")]
        public string Key { get; set; }

        // New or Used key. New by default
        [Required, Display(Name = "Status")]
        public string Status { get; set; }

        // The owner's id will be the student ID, this parameter is nullable
        [Display(Name = "Owner Id"), DisplayFormat(DataFormatString = "{0:D9}", ApplyFormatInEditMode = true)]
        public int? OwnerId { get; set; }

        [Display(Name = "Product Name")]
        public virtual Product Product { get; set; }
    }
}
