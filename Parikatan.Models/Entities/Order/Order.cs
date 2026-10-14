using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Web.Mvc.Html;
using Core.Enums;

namespace Models.Entities
{

    public class Order : BaseEntity
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
            ZarinpallAuthorities = new List<ZarinpallAuthority>();
        }
 

        [Display(Name = "کاربر")]
        public long? UserId { get; set; }


        [Display(Name = "جمع کل")]
        [Column(TypeName = "Money")]
        public decimal TotalAmount { get; set; }

           
        [Display(Name = "مبلغ پرداختی")]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "مبلغ باقی مانده")]
        public decimal RemainAmount { get; set; }
         

        [Display(Name = "شهر")]
        public long? CityId { get; set; }

        [Display(Name = "آدرس")]
        public string Address { get; set; }

        [Display(Name = "کد رهگیری پرداخت")]
        public string SaleReferenceId { get; set; }

        [Display(Name = "وضعیت سفارش")]
        public OrderStatus OrderStatus { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ZarinpallAuthority> ZarinpallAuthorities { get; set; }
        public virtual City City { get; set; }

        [Display(Name = "پرداخت شده")]
        public bool IsPaid { get; set; }


        public long? DiscountCodeId { get; set; }

        public virtual DiscountCode DiscountCode { get; set; }
        [Display(Name = "هزینه حمل")]
        public decimal? ShippingAmount { get; set; }

        [Display(Name = "جمع فاکتور")]
        public decimal? SubTotal { get; set; }

        [Display(Name = "مبلغ تخفیف")]
        public decimal? DiscountAmount { get; set; }
        

        [Display(Name = "نام تحویل گیرنده")]
        public string DeliverFullName { get; set; }
        [Display(Name = "شماره موبایل تحویل گیرنده")]
        public string DeliverCellNumber { get; set; }
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }
        [Display(Name = "تاریخ پرداخت")]
        public DateTime? PaymentDate { get; set; } 
       
         
        [Display(Name = "توضیحات مشتری")]
        [DataType(DataType.MultilineText)]
        public string CustomerDesc { get; set; }
 
        [Display(Name = "تاریخ سفارش")]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "روش پرداخت")]
        public long? PaymentTypeId { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        #region NotMapped

        [NotMapped]
        [Display(Name = "جمع فاکتور")]
        public string SubTotalStr
        {
            get
            {
                if (SubTotal != null)
                    return SubTotal.Value.ToString("n0") ;

                return string.Empty;
            }
        }
        [NotMapped]
        [Display(Name = "جمع نهایی")]
        public string TotalAmountStr
        {
            get { return TotalAmount.ToString("n0") ; }
        }
      
        [NotMapped]
        [Display(Name = "مبلغ تخفیف")]
        public string DiscountAmountStr
        {
            get
            {
                if (DiscountAmount != null)
                    return DiscountAmount.Value.ToString("n0");

                return string.Empty;
            }
        }

        [NotMapped]
        [Display(Name = "مبلغ پرداختی")]
        public string PaymentAmountStr
        {
            get
            {
                if (PaymentAmount != null)
                    return PaymentAmount.ToString("n0");

                return string.Empty;
            }
        }

        [NotMapped]
        [Display(Name = "مبلغ باقی مانده")]
        public string RemainAmountStr
        {
            get
            {
                if (RemainAmount != null)
                    return RemainAmount.ToString("n0");

                return string.Empty;
            }
        }
          

        [NotMapped]
        [Display(Name = "تاریخ ثبت سفارش")]
        public string OrderDateStr
        {
            get
            {

                switch (CreationDate.DayOfWeek.ToString().ToLower())
                {
                    case "saturday":
                        return "شنبه - " + CreationDate;
                    case "sunday":
                        return "یکشنبه - " + CreationDate;
                    case "monday":
                        return "دوشنبه - " + CreationDate;
                    case "tuesday":
                        return "سه شنبه - " + CreationDate;
                    case "wednesday":
                        return "چهارشنبه - " + CreationDate;
                    case "thursday":
                        return "پنج شنبه - " + CreationDate;
                    case "friday":
                        return "جمعه - " + CreationDate;
                }


                return CreationDate.ToString();
            }
        }

        [NotMapped]
        [Display(Name = "تاریخ فاکتور ")]
        public string FactorDateStr
        {
            get
            {
                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                string year = pc.GetYear(CreationDate).ToString().PadLeft(4, '0');
                string month = pc.GetMonth(CreationDate).ToString().PadLeft(2, '0');
                string day = pc.GetDayOfMonth(CreationDate).ToString().PadLeft(2, '0');
                return String.Format("{0}/{1}/{2}", year, month, day);
            }
        }

        [NotMapped]
        [Display(Name = "وضعیت سفارش")]
        public virtual string OrderStatusStr {
            get
            {
                return EnumExtensions.GetDisplayName(OrderStatus);
            } }

        [NotMapped]
        [Display(Name = "نوع سفارش")]
        public virtual string OrderTypeStr
        {
            get
            {
                return EnumExtensions.GetDisplayName(OrderType);
            } }

        #endregion

        public OrderType OrderType { get; set; }

        public long? ReferralId { get; set; }

        public virtual Referral Referral { get; set; }

    }
}
