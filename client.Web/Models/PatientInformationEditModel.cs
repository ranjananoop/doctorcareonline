using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;
using System.Web.Mvc;

namespace client.Web.Models
{
    public class PatientInformationEditModel
    {
        public PatientInformationEditModel()
        {

            DoctorsInformationEditModels = new List<DoctorsInformationEditModel>();
        }


        public Nullable<DateTime> RATINGDATE { get; set; }
        [Display(Name = "Video Appointments")]
        public string appintmentdone { get; set; }
        [Display(Name = "Clinic Appointments")]
        public string appintmentdoneclinic { get; set; }

        [Display(Name = " Your profile is completed")]
        public string profilecompleted { get; set; }

        public bool appintmentcurrent { get; set; }
        [Display(Name = "Payment Done")]
        public string paymentdone { get; set; }

        [Display(Name = "Preffered providers")]
        public string prefferedprovidercount { get; set; }

        public string Comment { get; set; }
        [Display(Name = "Salutation")]
        [MaxLength(10)]
        public string Salutation { get; set; }

        [Display(Name = "First Name:")]
             
        public string FIRSTNAME { get; set; }

        public string MessageUpdateStatus { get; set; }

        public string USERTYPE { get; set; }
        [Display(Name = "Last Name:")]       
        public string LASTNAME { get; set; }

        public string inboxUreadMessageCount { get; set; }
       
     

       
       

       
        [Display(Name = "User Image:")]
        [MaxLength(256)]
        public string USERPHOTOFILEPATH { get; set; }


       

        public virtual ICollection<DoctorsInformationEditModel> DoctorsInformationEditModels { get; set; }

    }
}