using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("GroupContragents", Schema = "book")]
    public class GroupContragents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
        public int Id { get; set; }

        [Column("parent_id")]
        public Nullable<int> ParentId { get; set; }

        [Column("path")]
        public string Path { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("tag")]
        public string Tag { get; set; }

        [Column("account")]
        public string Account { get; set; }
    }
}