using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Staff", Schema = "book")]
    public class Staff
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        [Column("usr_column_501")]
        public string fakeID { get; set; }
    }
}