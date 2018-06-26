using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("CompanyAccounts", Schema = "book")]
    public class CompanyAccounts
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
    }
}