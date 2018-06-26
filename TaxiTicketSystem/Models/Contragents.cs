using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("Contragents", Schema = "book")]
    public class Contragents
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public string name { get; set; }
        public string short_name { get; set; }
        public int? acc_use { get; set; }
        [Column("usr_column_504")]
        public string fakeID { get; set; }
        public string path { get; set; }
        public string account { get; set; }
        public string account2 { get; set; }
        public double? tax { get; set; }
        public double? min_tax { get; set; }
        public int? country_id { get; set; }
        public int? vat_type { get; set; }
        [Column("usr_column_505")]
        public string customVat { get; set; }
        public int? group_id { get; set; }
        public string code { get; set; }

        [Column("usr_column_506")]
        public string pwd { get; set; }

        public int? type { get; set; }

        public string address { get; set; }

        [Column("e_mail")]
        public string email { get; set; }

        [Column("tel")]
        public string phone { get; set; }

        [Column("usr_column_507")]
        public string contractStartDate { get; set; }

        [Column("usr_column_508")]
        public string contractExpirationDate { get; set; }

        [Column("usr_column_509")]
        public string servicePaymentDate { get; set; }

        [Column("usr_column_510")]
        public string serviceRates { get; set; }

        [Column("usr_column_511")]
        public string monthPeriod { get; set; }

        [Column("usr_column_512")]
        public string maxMonth { get; set; }
        [Column("usr_column_513")]
        public string contactPerson { get; set; }
        [Column("usr_column_514")]
        public string debts { get; set; }

        public DateTime birth_date { get; set; }
        public int client_id { get; set; }
        public int cons_period { get; set; }
        //public int create_user_id { get; set; }
        //public DateTime create_date { get; set; }
        //public int limit_type { get; set; }
        //public decimal limit_val { get; set; }
    }
}