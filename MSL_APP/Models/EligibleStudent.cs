using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class EligibleStudent
    {
        [Key]
        public int Id { get; set; }

        // Student ID, ex. 000101010
        [Required, Display(Name = "Student ID"), DisplayFormat(DataFormatString = "{0:D9}", ApplyFormatInEditMode = true)]
        public int StudentID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // Student Email, ex. student.one@moahwkcollege.ca
        [Required, EmailAddress, Display(Name = "Email")]
        public string StudentEmail { get; set; }
    }
}
