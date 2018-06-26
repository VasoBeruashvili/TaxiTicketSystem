using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    public class BalanceModel
    {
        public string fakeID { get; set; }
        public string companyName { get; set; }
        public double prevPeriod { get; set; }
        public double currentPeriod { get; set; }
        public double totalPeriod { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int? id { get; set; }
    }
}