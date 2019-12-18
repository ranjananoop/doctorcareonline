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

     [Table("APPOINTMENTFOLLOWUPS")]
    public partial class APPOINTMENTFOLLOWUP : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        [Key]
        public long APPOINTMENTFOLLOWUPID { get; set; }
        
        public string DIAGNOSIS { get; set; }
        public string PRESCRIPTIONS { get; set; }
        public string FOLLOWUP { get; set; }
        public string DOCTORNOTES { get; set; }
        public string PATIENTNOTES { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime DateCreated { get; set; }
         public string MODIFIEDBY{ get; set; }
        public Nullable<DateTime> MODIFIEDON { get; set; }

        [ForeignKey("APPOINTMENTID")]
        public virtual APPOINTMENT APPOINTMENT { get; set; }
        public long APPOINTMENTID { get; set; }
    }
}