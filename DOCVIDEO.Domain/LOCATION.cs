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

     [Table("LOCATIONS")]
    public partial class LOCATION : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        [Key]
        public long LOCID { get; set; }
        public int LOCLEVEL { get; set; }
        public int LOCTYPEVID { get; set; }
        public long PARENTLOCID { get; set; }
        public string LOCDESCRIPTION { get; set; }
        public string LOCDZIPCODE { get; set; }
    }
}
