using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    public class SubBalanceModel
    {
        public DateTime tDate { get; set; }
        public string tDate1 { get; set; }
        public string tDate2 { get; set; }
        public double? amount1 { get; set; }
        public double? amount2 { get; set; }
        public double? balance { get; set; }
    }
}