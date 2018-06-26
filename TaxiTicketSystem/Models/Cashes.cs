using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Cashes", Schema = "book")]
    public class Cashes
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
    }
}