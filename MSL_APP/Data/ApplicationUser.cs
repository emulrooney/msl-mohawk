using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Data
{
    public class ApplicationUser:IdentityUser
    {
        public int StudentID { get; set; }
    }
}
