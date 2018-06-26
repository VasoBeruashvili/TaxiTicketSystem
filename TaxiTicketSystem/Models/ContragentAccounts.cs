using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("ContragentAccounts", Schema = "book")]
    public class ContragentAccounts
    {
        public int id { get; set; }
        public int? contragent_id { get; set; }
        public int? bank_id { get; set; }
        public string account { get; set; }
        public int? currency_id { get; set; }
        public int? corr_bank_id { get; set; }
        public string name { get; set; }
    }
}