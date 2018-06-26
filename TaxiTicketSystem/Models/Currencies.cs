using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Currencies", Schema = "book")]
    public class Currencies
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public double? rate { get; set; }
        public int is_auto { get; set; }
    }
}