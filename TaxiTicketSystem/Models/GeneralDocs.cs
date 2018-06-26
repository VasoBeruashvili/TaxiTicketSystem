using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("GeneralDocs", Schema = "doc")]
    public class GeneralDocs
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tdate")]
        public Nullable<DateTime> Tdate { get; set; }

        [Column("tdate1")]
        public Nullable<DateTime> Tdate1 { get; set; }

        [Column("doc_num")]
        public Nullable<Int64> DocNum { get; set; }

        [Column("doc_num_prefix")]
        public string DocNumPrefix { get; set; }

        [Column("doc_type")]
        public Nullable<int> DocType { get; set; }

        [Column("purpose")]
        public string Purpose { get; set; }

        [Column("amount")]
        public Nullable<double> Amount { get; set; }

        [Column("currency_id")]
        public Nullable<int> CurrencyId { get; set; }

        [Column("rate")]
        public Nullable<double> Rate { get; set; }

        [Column("vat")]
        public Nullable<double> Vat { get; set; }

        [Column("ref_id")]
        public Nullable<int> RefId { get; set; }

        [Column("user_id")]
        public Nullable<int> UserId { get; set; }

        [Column("param_id1")]
        public Nullable<int> ParamId1 { get; set; }

        [Column("param_id2")]
        public Nullable<int> ParamId2 { get; set; }

        [Column("status_id")]
        public Nullable<int> StatusId { get; set; }

        [Column("make_entry")]
        public Nullable<bool> MakeEntry { get; set; }

        [Column("project_id")]
        public Nullable<int> ProjectId { get; set; }

        [Column("uid")]
        public string Uid { get; set; }

        [Column("sync_status")]
        public Nullable<byte> SyncStatus { get; set; }

        [Column("waybill_num")]
        public string WaybillNum { get; set; }

        [Column("store_id")]
        public Nullable<int> StoreId { get; set; }

        [Column("analytic_code")]
        public Nullable<int> AnalyticCode { get; set; }

        [Column("amount2")]
        public Nullable<double> Amount2 { get; set; }

        [Column("contragent_sub_id")]
        public Nullable<int> ContragentSubId { get; set; }

        [Column("contragent_id")]
        public Nullable<int> ContragentId { get; set; }

        [Column("is_blocked")]
        public Nullable<bool> IsBlocked { get; set; }

        [Column("is_packed")]
        public Nullable<bool> IsPacked { get; set; }

        [Column("is_deleted")]
        public Nullable<bool> IsDeleted { get; set; }

        [Column("delete_user_id")]
        public Nullable<int> DeleteUserId { get; set; }

        [Column("delete_date")]
        public Nullable<DateTime> DeleteDate { get; set; }

        [Column("create_date")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        [Column("house_id")]
        public int HouseId { get; set; }

        [NotMapped]
        public double TotalSumAll { get; set; }
        [NotMapped]
        public string customVat { get; set; }


        public virtual ICollection<ProductsFlow> ProductsFlows { get; set; }
        public virtual ICollection<ProductOut> ProductOuts { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }

        public GeneralDocs()
        {
            this.DocNumPrefix = string.Empty;
            this.DocNum = 0;
            this.Amount = 0;
            this.Vat = 0;
            this.RefId = 0;
            this.ParamId1 = 0;
            this.ParamId2 = 0;
            this.SyncStatus = 0;
            this.StoreId = 0;
            this.AnalyticCode = 0;
            this.Amount2 = 0;
            this.ContragentSubId = 0;
            this.ContragentId = 0;
            this.IsBlocked = false;
            this.IsPacked = false;
            this.IsDeleted = false;
            this.MakeEntry = false;
            this.StatusId = 0;
            this.ProjectId = 1;
            this.HouseId = 1;
            this.CurrencyId = 1;
            this.Rate = 1.0;
            this.Uid = Guid.NewGuid().ToString();
        }
    }
}