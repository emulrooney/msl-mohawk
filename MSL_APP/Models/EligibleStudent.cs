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

        [Required, Display(Name = "First Name")]
        [RegularExpression(@"^[A-Za-z ,.'-]+$", ErrorMessage = "Invalid Name Format")]
        public string FirstName { get; set; }

        [Required, Display(Name = "Last Name")]
        [RegularExpression(@"^[A-Za-z ,.'-]+$", ErrorMessage = "Invalid Name Format")]
        public string LastName { get; set; }

        // Student Email, ex. student.one@moahwkcollege.ca
        [Required, EmailAddress, Display(Name = "Email")]
        public string StudentEmail { get; set; }
    }
}
