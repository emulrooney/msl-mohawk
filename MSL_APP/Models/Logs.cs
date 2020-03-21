using MSL_APP.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MSL_APP.Models
{
    public class Logs
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public string StudentEmail { get; set; }

        public string Action { get; set; }

        public string Product { get; set; }

        public string ProductKey { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;

    }
}
