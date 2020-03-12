using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class ProductKeyLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public int StudentId { get; set; }

        public string StudentEmail { get; set; }

        public string Action { get; set; }

        public string ProductName { get; set; }

        public string ProductKey { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;

    }
}
