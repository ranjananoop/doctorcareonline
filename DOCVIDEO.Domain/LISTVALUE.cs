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

    [Table("LISTVALUES")]
    public partial class LISTVALUE : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

         [Key]
        public long LISTVALUEID { get; set; }
      
        public string VALUEKEY { get; set; }
        public string VALUEDESCRIPTION { get; set; }
        public Nullable<int> VALUEORDER { get; set; }
        
        [ForeignKey("LISTID")]
        public virtual LISTMASTER LISTMASTER { get; set; }
        public long LISTID { get; set; }
        
    }
}
