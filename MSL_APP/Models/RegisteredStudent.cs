using MSL_APP.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class RegisteredStudent
    {
        [Key]
        public int Id { get; set; }

        // Link registered accounts to authentication system for setting permissions
        [ForeignKey("User")] 
        public string UserId { get; set; }

        // Link registered account to eligible student table for registration restrictions, set to be nullable
        [ForeignKey("EligibleStudent")]
        public int? EligibleId { get; set; }

        [Required, Display(Name = "Student ID")]
        public int StudentId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required, EmailAddress, Display(Name = "Email")]
        public string StudentEmail { get; set; }

        [Display(Name = "Eligible")]
        public virtual EligibleStudent EligibleStudent { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
