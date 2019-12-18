using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOCVIDEO.ObjectState;

namespace DOCVIDEO.Domain
{
    public class Role : IObjectWithState
    {
        [Key]
        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Role Name is required")]
        [MaxLength(100)]
        public string RoleName { get; set; }

        public virtual ICollection<USERSINFORMATION> Users { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [NotMapped]
        public State State { get; set; }
    }
}
