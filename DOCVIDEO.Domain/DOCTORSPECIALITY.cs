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

    [Table("DOCTORSPECIALITIES")]
    public partial class DOCTORSPECIALITY : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }      

        [Key]
        public int SPECIALITYID { get; set; }
        public string SPECIALITY { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime DateCreated { get; set; }
        public string MODIFIEDBY{ get; set; }
        public Nullable<DateTime> MODIFIEDON { get; set; }

        [ForeignKey("UserName")]
        public virtual USERSINFORMATION USERSINFORMATIONS { get; set; }
        public string UserName { get; set; }
    }
}
