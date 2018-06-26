using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    public class DocItem
    {
        public GeneralDocs GeneralDocsItem { get; set; }
        public List<ProductsFlow> ProductsFlowList { get; set; }
        public List<ProductOut> ProductOutItem { get; set; }
    }
}