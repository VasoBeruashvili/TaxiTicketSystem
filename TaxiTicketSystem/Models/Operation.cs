using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Operation", Schema = "doc")]
    public class Operation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("general_id")]
        public int GeneralId { get; set; }
        [ForeignKey("GeneralId")]
        public GeneralDocs GeneralDocs { get; set; }

        [MaxLength(200)]
        [Column("person")]
        public string Person { get; set; }

        [Column("person_id")]
        public Nullable<int> PersonId { get; set; }

        [Column("is_document")]
        public Nullable<byte> IsDocument { get; set; }

        [Column("analytic_code")]
        public Nullable<int> AnalyticCode { get; set; }

        [Column("type")]
        public Nullable<int> Type { get; set; }

        [Column("client_id")]
        public Nullable<int> ClientId { get; set; }

        public Operation()
        {
            this.Type = 0;
            this.IsDocument = 0;
            this.Person = string.Empty;
            this.ClientId = 0;
        }
    }
}