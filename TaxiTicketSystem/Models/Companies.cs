using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Companies", Schema = "book")]
    public class Companies
    {
        [Key]
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string chief { get; set; }

        public bool? vat { get; set; }
        public DateTime? tdate { get; set; }
    }
}