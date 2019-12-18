//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DOCVIDEO.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using DOCVIDEO.ObjectState;

    [Table("SUBSCRIPTIONS")]
    public partial class SUBSCRIPTION : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        public SUBSCRIPTION()
        {
            this.SUBSCRIPTIONINSTALLMENTS = new HashSet<SUBSCRIPTIONINSTALLMENT>();
        }

         [Key]
        public long SUBSCRIPTIONID { get; set; }
        public string PAYPALSUBSCRID { get; set; }
        public string TXNTYPE { get; set; }
        public Nullable<DateTime> SUBSCRDATE { get; set; }
        public Nullable<long> PERIOD { get; set; }
        public string CURRENCY { get; set; }
        public string IPNTRACKID { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public string RECEIVEREMAIL { get; set; }
        public string PAYEREMAIL { get; set; }
        public string PAYERID { get; set; }
        public string ITEMNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        
        public string COUNTRY { get; set; }
        public Nullable<DateTime> DateCreated { get; set; }
        public Nullable<DateTime> MODIFIEDON { get; set; }
        public Nullable<long> CREATEDBY { get; set; }
         public string MODIFIEDBY{ get; set; }
        public string TRIALDAYS { get; set; }
        public string INSTALLMENTDURATION { get; set; }
        public string ERRORDETAILS { get; set; }

        [ForeignKey("UserName")]
        public virtual USERSINFORMATION USERSINFORMATIONS { get; set; }
        public string UserName { get; set; }

        public virtual ICollection<SUBSCRIPTIONINSTALLMENT> SUBSCRIPTIONINSTALLMENTS { get; set; }
    }
}