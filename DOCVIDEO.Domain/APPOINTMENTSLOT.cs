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

    [Table("APPOINTMENTSLOTS")]
    public partial class APPOINTMENTSLOT : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        [Key]
        public long APPOINTMENTSLOTID { get; set; }

        public long DOCTORCONFIRMEDSLOTID { get; set; }

        [ForeignKey("APPOINTMENTID")]
        public virtual APPOINTMENT APPOINTMENT { get; set; }
        public long APPOINTMENTID { get; set; }

    }
}