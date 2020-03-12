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
        [Required]
        public int StudentID { get; set; }

        // Student Email, ex. student.one@moahwkcollege.ca
        [Required, EmailAddress]
        public string StudentEmail { get; set; }
    }
}
