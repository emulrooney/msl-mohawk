using Microsoft.AspNetCore.Identity;
using MSL_APP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Data
{
    public class ApplicationUser:IdentityUser
    {

        [Required, Display(Name = "Student ID"), DisplayFormat(DataFormatString = "{0:D9}", ApplyFormatInEditMode = true)]
        public int StudentId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }

        [ForeignKey("EligibleStudent")]
        public int? EligibleId { get; set; }

        [Display(Name = "Eligible")]
        public virtual EligibleStudent EligibleStudent { get; set; }
    }
}
