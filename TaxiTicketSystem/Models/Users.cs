using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Users", Schema = "book")]
    public class Users
    {
        [Key]
        public int id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int? staff_id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
    }
}