using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;
namespace client.Web.Models
{
    public class BookingAppointmentModel
    {
        public long APPOINTMENTID { get; set; }

        public string DOCTORID { get; set; }
        public long DOCTORSLOTID { get; set; }

        public int RateQuatermins { get; set; }

        [Display(Name = "Rate in (USD):")]
        public string RateDisplay { get; set; }

      [Display(Name = "Have you seen this doctor before?")]
       public string ISEXISITING { get; set; }

      public string tabvalue { get; set; }

       public string ISNOTEXISITING { get; set; } 
        [Display(Name = "Reason for visit:")]
        [StringLength(46, ErrorMessage = "Maximum 46 characters.")]
        public string REASONFORVISIT { get; set; }

        [Display(Name = "When:")]
        public string APPOINTMENTSTARTTIMEACTUAL { get; set; }
        public Nullable<DateTime> APPOINTMENTSTARTTIME { get; set; }
         [Display(Name = "How long would you like to see the doctor?")]
        public string APPOINTMENTENDTIME { get; set; }

        [Display(Name = "With:")]
        public string Speciality { get; set; }

        public string FIRSTNAME { get; set; }

        public string LASTNAME { get; set; }

        public string MessageUpdateStatus { get; set; }

        [NotMapped]
        [Display(Name = "Provider Type:")]
        public string ProviderType { get; set; }


        [NotMapped]
        [Display(Name = "Gender:")]
        [MaxLength(10)]
        public string SearchGender { get; set; }

        [NotMapped]
        [Display(Name = "ZIP code:")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
        public string SEARCHZIPCODE { get; set; }
        [NotMapped]
        public State State { get; set; }

    }
}