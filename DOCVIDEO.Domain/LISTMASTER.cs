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

    [Table("LISTMASTERS")]
    public partial class LISTMASTER : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        public LISTMASTER()
        {
            this.LISTVALUEs = new HashSet<LISTVALUE>();
        }
         [Key]
        public long LISTID { get; set; }
        public string LISTCODE { get; set; }
    
        public virtual ICollection<LISTVALUE> LISTVALUEs { get; set; }
    }
}
