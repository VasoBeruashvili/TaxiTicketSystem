using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Params", Schema = "config")]
    public class Params
    {
        [Key]
        public string name { get; set; }
        public string value { get; set; }
    }
}