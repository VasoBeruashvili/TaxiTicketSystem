using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Banks", Schema = "book")]
    public class Banks
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public bool resident { get; set; }
        public string swift_code { get; set; }
    }
}