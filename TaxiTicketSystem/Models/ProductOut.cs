using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaxiTicketSystem.Models
{
    [Table("ProductOut", Schema = "doc")]
    public class ProductOut
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("general_id")]
        public int GeneralId { get; set; }
        [ForeignKey("GeneralId")]
        public GeneralDocs GeneralDocs { get; set; }

        [Column("sender_IdNum")]
        [MaxLength(50)]
        public string SenderIdNum { get; set; }

        [Column("sender_name")]
        [MaxLength(50)]
        public string SenderName { get; set; }

        [Column("transp_start_place")]
        [MaxLength(200)]
        public string TransportStartPlace { get; set; }

        [Column("transporter_name")]
        [MaxLength(50)]
        public string TransporterName { get; set; }

        [Column("transporter_IdNum")]
        [MaxLength(50)]
        public string TransporterIdNum { get; set; }

        [Column("reciever_IdNum")]
        [MaxLength(50)]
        public string RecieverIdNum { get; set; }

        [Column("reciever_name")]
        [MaxLength(50)]
        public string RecieverName { get; set; }

        [Column("transp_end_place")]
        [MaxLength(200)]
        public string TransportEndPlace { get; set; }

        [Column("responsable_person")]
        [MaxLength(50)]
        public string ResponsablePerson { get; set; }

        [Column("responsable_person_num")]
        [MaxLength(50)]
        public string ResponsablePersonNum { get; set; }

        [Column("responsable_person_date")]
        public Nullable<DateTime> ResponsablePersonDate { get; set; }

        [Column("transport_model")]
        [MaxLength(50)]
        public string TransportModel { get; set; }

        [Column("transport_number")]
        [MaxLength(50)]
        public string TransportNumber { get; set; }

        [Column("driver_card_number")]
        [MaxLength(50)]
        public string DriverCardNumber { get; set; }

        [Column("avto")]
        public Nullable<bool> Avto { get; set; }

        [Column("railway")]
        public Nullable<bool> Railway { get; set; }

        [Column("other")]
        public Nullable<bool> Other { get; set; }

        [Column("out_type")]
        public Nullable<byte> OutType { get; set; }

        [Column("discount_percent")]
        public Nullable<double> DiscountPercent { get; set; }

        [Column("agreement_id")]
        public Nullable<int> AgreementId { get; set; }

        [Column("pay_type")]
        [MaxLength(50)]
        public string PayType { get; set; }

        [Column("pay_date")]
        public Nullable<DateTime> PayDate { get; set; }

        [Column("coeff")]
        public Nullable<int> Coeff { get; set; }

        [Column("bonus")]
        public Nullable<double> Bonus { get; set; }

        [Column("bonus_coeff")]
        public Nullable<int> BonusCoeff { get; set; }

        [Column("is_waybill")]
        public Nullable<int> IsWaybill { get; set; }

        [Column("waybill_id")]
        public Nullable<int> WaybillId { get; set; }

        [Column("waybill_type")]
        public Nullable<int> WaybillType { get; set; }

        [Column("waybill_cost")]
        public Nullable<double> WaybillCost { get; set; }

        [Column("delivery_date")]
        public Nullable<DateTime> DeliveryDate { get; set; }

        [Column("waybill_status")]
        public Nullable<int> WaybillStatus { get; set; }

        [Column("transport_begin_date")]
        public Nullable<DateTime> TransportBeginDate { get; set; }

        [Column("activate_date")]
        public Nullable<DateTime> ActivateDate { get; set; }

        [Column("transport_cost_payer")]
        public Nullable<int> TransportCostPayer { get; set; }

        [Column("transport_type_id")]
        public Nullable<int> TransportTypeId { get; set; }

        [Column("driver_name")]
        [MaxLength(50)]
        public string DriverName { get; set; }

        [Column("waybill_num")]
        [MaxLength(50)]
        public string WaybillNum { get; set; }

        [Column("parent_waybill_id")]
        public Nullable<int> ParentWaybillId { get; set; }

        [Column("price_type")]
        public Nullable<int> PriceType { get; set; }

        [Column("staff_id")]
        public Nullable<int> StaffId { get; set; }

        [Column("is_foreign")]
        public Nullable<byte> IsForeign { get; set; }

        [Column("comment")]
        [MaxLength(250)]
        public string Comment { get; set; }

        [Column("card_id")]
        public Nullable<int> CardId { get; set; }

        [Column("auto_production")]
        public byte AutoProduction { get; set; }

        [Column("invoice_bank_id")]
        public Nullable<int> InvoiceBankId { get; set; }

        [Column("invoice_term")]
        [MaxLength(50)]
        public string InvoiceTerm { get; set; }

        [Column("discount_round")]
        public Nullable<double> DiscountRound { get; set; }

        [Column("is_auto_project")]
        public Nullable<bool> IsAutoProject { get; set; }

        //[Column("transport_text")]
        //public string TransportText { get; set; }

        [Column("usr_column_515")]
        public string StartTime { get; set; }
        [Column("usr_column_516")]
        public string EndTime { get; set; }
        [Column("usr_column_517")]
        public string TraveledDistance { get; set; }
        [Column("usr_column_518")]
        public string ParkingCosts { get; set; }
        [Column("usr_column_519")]
        public string CustomsFees { get; set; }
        [Column("usr_column_520")]
        public string AdditionalCosts { get; set; }
        [Column("usr_column_521")]
        public string WithoutPrint { get; set; }
        [Column("usr_column_522")]
        public string CommentOut { get; set; }
        [Column("usr_column_523")]
        public string CarNumber { get; set; }
        [Column("usr_column_524")]
        public string AddedWithWeb { get; set; }
        [Column("usr_column_525")]
        public string Deleted { get; set; }

        [Column("usr_column_526")]
        public string contrVat { get; set; }

        public ProductOut()
        {
            this.SenderIdNum = string.Empty;
            this.SenderName = string.Empty;
            this.TransportStartPlace = string.Empty;
            this.TransporterName = string.Empty;
            this.TransporterIdNum = string.Empty;
            this.RecieverIdNum = string.Empty;
            this.RecieverName = string.Empty;
            this.TransportEndPlace = string.Empty;
            this.ResponsablePerson = string.Empty;
            this.ResponsablePersonNum = string.Empty;
            this.TransportModel = string.Empty;
            this.TransportNumber = string.Empty;
            this.DriverCardNumber = string.Empty;
            this.Avto = false;
            this.Railway = false;
            this.Other = false;
            this.OutType = 0;
            this.DiscountPercent = 0;
            this.AgreementId = 0;
            this.PayType = "0";
            this.PayDate = DateTime.Now;
            this.Coeff = 0;
            this.Bonus = 0;
            this.BonusCoeff = 0;
            this.IsWaybill = 1;
            this.WaybillId = 0;
            this.WaybillType = 2;
            this.WaybillCost = 0;
            this.DeliveryDate = DateTime.Now;
            this.WaybillStatus = -1;
            this.TransportBeginDate = DateTime.Now;
            this.ActivateDate = DateTime.Today.AddSeconds(10);
            this.ResponsablePersonDate = DateTime.Now;
            this.TransportCostPayer = 1;
            this.TransportTypeId = 1;
            this.DriverName = string.Empty;
            this.ParentWaybillId = 0;
            this.PriceType = 3;
            this.StaffId = 0;
            this.IsForeign = 0;
            this.Comment = string.Empty;
            this.CardId = 0;
            this.InvoiceBankId = 0;
            this.DiscountRound = 0;
            this.IsAutoProject = false;
            this.InvoiceTerm = string.Empty;
        }
    }
}