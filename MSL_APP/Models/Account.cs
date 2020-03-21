using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        // Student ID, ex. 000101010
        [Required, Display(Name = "Admin ID"), DisplayFormat(DataFormatString = "{0:D9}", ApplyFormatInEditMode = true)]
        public int AdminID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // Student Email, ex. student.one@moahwkcollege.ca
        [Required, EmailAddress, Display(Name = "Email")]
        public string AdminEmail { get; set; }

        [NotMapped]
        [Required, DataType(DataType.Password), Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
