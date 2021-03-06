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

    [Table("DOCTORAVAILABILITIES")]
    public partial class DOCTORAVAILABILITY : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        public DOCTORAVAILABILITY()
        {
            this.DOCTORSLOTS = new HashSet<DOCTORSLOT>();
        }

        [Key]
        public long AVAILABILITYID { get; set; }
       
        public DateTime STARTTIME { get; set; }
        public DateTime ENDTIME { get; set; }
        public int DURATION { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public string PATTERN { get; set; }
        public string TYPE { get; set; }
        public string WORKINGDAYS { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime DateCreated { get; set; }
         public string MODIFIEDBY{ get; set; }
        public Nullable<DateTime> MODIFIEDON { get; set; }

        [ForeignKey("UserName")]
        public virtual USERSINFORMATION USERSINFORMATIONS { get; set; }
        public string UserName { get; set; }

        public virtual ICollection<DOCTORSLOT> DOCTORSLOTS { get; set; }
    }
}
