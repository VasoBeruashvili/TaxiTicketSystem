using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("ProductsFlow", Schema = "doc")]
    public class ProductsFlow
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("general_id")]
        public int GeneralId { get; set; }
        [ForeignKey("GeneralId")]
        public GeneralDocs GeneralDocs { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("product_tree_path")]
        public string ProductTreePath { get; set; }

        [Column("amount")]
        public Nullable<double> Amount { get; set; }

        [Column("price")]
        public Nullable<double> Price { get; set; }

        [Column("store_id")]
        public Nullable<int> StoreId { get; set; }

        [Column("vat_percent")]
        public Nullable<decimal> VatPercent { get; set; }

        [Column("self_cost")]
        public Nullable<double> SelfCost { get; set; }

        [Column("coeff")]
        public Nullable<int> Coeff { get; set; }

        [Column("is_order")]
        public Nullable<byte> IsOrder { get; set; }

        [Column("is_expense")]
        public Nullable<byte> IsExpense { get; set; }

        [Column("is_move")]
        public Nullable<byte> IsMove { get; set; }

        [Column("visible")]
        public Nullable<byte> Visible { get; set; }

        [Column("parent_product_id")]
        public Nullable<int> ParentProductId { get; set; }

        [Column("ref_id")]
        public Nullable<int> RefId { get; set; }

        [Column("unit_id")]
        public Nullable<int> UnitId { get; set; }

        [Column("comment")]
        public string Comment { get; set; }

        [Column("discount_percent")]
        public Nullable<double> DiscountPercent { get; set; }

        [Column("discount_value")]
        public Nullable<double> DiscountValue { get; set; }

        [Column("original_price")]
        public Nullable<double> OriginalPrice { get; set; }

        [Column("in_id")]
        public Nullable<int> InId { get; set; }

        [Column("vendor_id")]
        public Nullable<int> VendorId { get; set; }

        [Column("cafe_status")]
        public Nullable<byte> CafeStatus { get; set; }

        [Column("out_id")]
        public Nullable<int> OutId { get; set; }

        [Column("sub_id")]
        public Nullable<int> SubId { get; set; }

        [Column("service_product_id")]
        public Nullable<int> ServiceProductId { get; set; }

        [Column("uid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid Uid { get; set; }

        [Column("service_staff_id")]
        public Nullable<int> ServiceStaffId { get; set; }

        [Column("staff_salary")]
        public Nullable<double> StaffSalary { get; set; }

        [Column("product_bonus")]
        public Nullable<double> ProductBonus { get; set; }

        [Column("cafe_comment")]
        public string CafeComment { get; set; }

        [Column("cafe_send_date")]
        public Nullable<DateTime> CafeSendDate { get; set; }

        //[Column("ref_uid")]
        //public string RefUid { get; set; }

        //[Column("excise")]
        //public double Excise { get; set; }

        public ProductsFlow()
        {
            this.ProductTreePath = string.Empty;
            this.VatPercent = 0;
            this.SelfCost = 0;
            this.ParentProductId = 0;
            this.RefId = 0;
            this.DiscountPercent = 0;
            this.DiscountValue = 0;
            this.CafeStatus = 0;
            this.SubId = 0;
            this.ServiceProductId = 0;
            this.StaffSalary = 0;
            this.ProductBonus = 0;
            this.IsOrder = 0;
            this.IsExpense = 0;
            this.IsMove = 0;
            this.Visible = 1;
            this.InId = 0;
            this.OutId = 0;
            this.VendorId = 0;
            this.OriginalPrice = 0;
            this.Comment = string.Empty;
        }
    }
}