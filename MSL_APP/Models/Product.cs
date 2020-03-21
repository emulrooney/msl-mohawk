using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        // Name of the products
        [Required]
        public string Name { get; set; }

        // Set the quantity limitation for students to get the product key, 0 by default
        [Display(Name = "Quantity Limit")]
        public int QuantityLimit { get; set; }

        // Count total key numbers for each product, 0 by default, should be updated when uploading the new keys
        [Display(Name = "Key Count")]
        public int KeyCount { get; set; }

        // Count how many keys have been aquired by students, 0 by default, should be updated when student aquired a key
        [Display(Name = "Used Key")]
        public int UsedKeyCount { get; set; }

        // Count how many available keys left, 0 by default, should be updated when uploading the new keys and when student aquired a key
        [Display(Name = "Remaining Key")]
        public int RemainingKeyCount => KeyCount - UsedKeyCount;

        // Actived or disabled product. Using string Active or Disable. Active by default
        [Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }

        // Optional download link for the product
        [Display(Name = "Download Link")]
        public string DownloadLink { get; set; }
    }
}
