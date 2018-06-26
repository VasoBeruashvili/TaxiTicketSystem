using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Entries", Schema = "doc")]
    public class Entries
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("general_id")]
        public int GeneralId { get; set; }
        [ForeignKey("GeneralId")]
        public GeneralDocs GeneralDocs { get; set; }

        [MaxLength(50)]
        [Column("debit_acc")]
        public string DebitAcc { get; set; }

        [MaxLength(50)]
        [Column("credit_acc")]
        public string CreditAcc { get; set; }

        [Column("amount")]
        public Nullable<double> Amount { get; set; }

        [Column("rate")]
        public Nullable<double> Rate { get; set; }

        [Column("currency_id")]
        public Nullable<int> CurrencyId { get; set; }

        [Column("n")]
        public Nullable<double> N { get; set; }

        [Column("n2")]
        public Nullable<double> N2 { get; set; }

        [Column("a1")]
        public Nullable<int> A1 { get; set; }

        [Column("a2")]
        public Nullable<int> A2 { get; set; }

        [Column("a3")]
        public Nullable<int> A3 { get; set; }

        [Column("b1")]
        public Nullable<int> B1 { get; set; }

        [Column("b2")]
        public Nullable<int> B2 { get; set; }

        [Column("b3")]
        public Nullable<int> B3 { get; set; }

        [MaxLength(100)]
        [Column("comment")]
        public string Comment { get; set; }

        [Column("project_id")]
        public Nullable<int> ProjectId { get; set; }

        public Entries()
        {
            this.Rate = 1;
            this.A1 = 0;
            this.A2 = 0;
            this.A3 = 0;
            this.B1 = 0;
            this.B2 = 0;
            this.B3 = 0;
            this.N = 0;
            this.N2 = 0;
            this.Comment = string.Empty;
        }
    }
}