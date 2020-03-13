using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class ProductName
    {
        [Key]
        public int Id { get; set; }

        // Name of the products
        [Required]
        public string Name { get; set; }

        // Set the quantity limitation for students to get the product key, 0 by default
        [Display(Name = "Quantity Limit")]
        public int QuantityLimit { get; set; }

        // Count how many keys left for each product, 0 by default, should be updated when uploading the new keys
        [Display(Name = "Key Count")]
        public int KeyCount { get; set; }

        // Actived or disabled product. Using integer 0 as actived, 1 as disabled. 0 by default
        [Display(Name = "Active Status")]
        public int ActiveStatus { get; set; }

        // Optional download link for the product
        [Display(Name = "Download Link")]
        public string DownloadLink { get; set; }
    }
}
