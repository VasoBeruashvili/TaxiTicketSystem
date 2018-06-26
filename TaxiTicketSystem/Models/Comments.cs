using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Comments", Schema = "book")]
    public class Comments
    {
        [Key]
        public int id { get; set; }
        public string comment { get; set; }
        public int general_id { get; set; }

        [ForeignKey("general_id")]
        public GeneralDocs GeneralDocs { get; set; }
    }
}