using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DOCVIDEO.ObjectState;

namespace DOCVIDEO.Domain
{

    [Table("DOCSUBSCRIPTIONS")]
    public partial class DOCSUBSCRIPTION : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        public DOCSUBSCRIPTION()
        {
        }
        [Key]
        public long DOCSUBSCRIPTIONID { get; set; }

        public int? MaxNumberOfPayments { get; set; }
        public DateTime? StartingDate { get; set; }
        public string  PinType { get; set; }
        public decimal? MaxAmountPerPayment { get; set; }
        public string  CurrencyCode { get; set; }
        public string  SenderEmail { get; set; }
        public string  VerifySign { get; set; }
        public string  TestIpn { get; set; }
        public int? DateOfMonth { get; set; }
        public int? CurrentNumberOfPayments { get; set; }
        public string  PreapprovalKey { get; set; }
        public DateTime? EndingDate { get; set; }
        public bool IsApproved { get; set; }
        public string  TransactionType { get; set; }
        public string  DayOfWeek { get; set; }
        public string  Status { get; set; }
        public decimal? CurrentTotalAmountOfAllPayments { get; set; }
        public int? CurrentPeriodAttempts { get; set; }
        public string  Charset { get; set; }
        public string PaymentPeriod { get; set; }
        public string  NotifyVersion { get; set; }
        public decimal? MaxTotalAmountOfAllPayments { get; set; }
        public string ERRORDETAILS { get; set; }

        [ForeignKey("UserName")]
        public virtual USERSINFORMATION USERSINFORMATIONS { get; set; }
        public string UserName { get; set; }


        public DateTime? CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime? MODIFIEDDATE { get; set; }
        public string MODIFIEDBY { get; set; }
    }
}