using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Cars", Schema = "book")]
    public class Cars
    {
        [Key]
        public int id { get; set; }
        public string num { get; set; }
        [Column("usr_column_503")]
        public string fakeID { get; set; }
    }
}