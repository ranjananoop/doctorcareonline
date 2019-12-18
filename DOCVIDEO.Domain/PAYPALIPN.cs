using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DOCVIDEO.ObjectState;

namespace DOCVIDEO.Domain
{

    [Table("PAYPALIPNS")]
    public partial class PAYPALIPN : IObjectWithState
    {
        [NotMapped]
        public State State { get; set; }

        public PAYPALIPN()
        {
        }

        [Key]
        public Int64 PAYPALIPNID { get; set; }
        public string PAYPALRESPONSE { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime? MODIFIEDDATE { get; set; }
        public string MODIFIEDBY { get; set; }
        public string GUID { get; set; }
    }
}