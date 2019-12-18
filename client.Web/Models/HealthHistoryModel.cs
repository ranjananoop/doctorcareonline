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
    public class HealthHistoryModel
    {

      
        [Display(Name = "Email :")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string UserName { get; set; }

        public string MessageUpdateStatus { get; set; }
        [Display(Name = "Report Name")]
        [StringLength(32, ErrorMessage = "Maximum 32 characters.")]
        public string REPORTNAME { get; set; }

        [Display(Name = "Comments")]
        [StringLength(700, ErrorMessage = "Maximum 700 characters.")]
        public string NOTES { get; set; }

        [Display(Name = "Attach Reports")]
        public string REPORTPATH { get; set; }

        public long PATIENTREPORTID { get; set; }

        public string tabindex { get; set; }

        [Display(Name = "Povemail")]
        [StringLength(30, ErrorMessage = "Maximum 30 characters.")]
        public string POVEMAIL { get; set; }

        public Nullable<long> IMAGEID { get; set; }

        [Display(Name = "Patient Type")]
        [MaxLength(100)]
        public string PATIENTTYPE { get; set; }

        public string DOCTORREPORTSTATUS { get; set; }
      
        public int MEDICATIONID { get; set; }

        [Display(Name = "Medication Name")]
        [StringLength(32, ErrorMessage = "Maximum 32 characters.")]
        public string MEDICATIONNAME { get; set; }

          [Display(Name = "Prescribed Doctor")]
        public string DOCTORID { get; set; }

        [Display(Name = "Prescribed Date")]
        public Nullable<DateTime> PRESCRIBEDDATE { get; set; }

        [Display(Name = "Stopped Date")]
        public Nullable<DateTime> STOPPEDDATE { get; set; }

        [Display(Name = "Comments")]
        [StringLength(700, ErrorMessage = "Maximum 700 characters.")]
        public string COMMENT { get; set; }

        [Display(Name = "Comments")]
        [StringLength(700, ErrorMessage = "Maximum 700 characters.")]
        public string IMUCOMMENT { get; set; }

        [Display(Name = "Comments")]
        [StringLength(700, ErrorMessage = "Maximum 700 characters.")]
        public string ALCOMMENT { get; set; }

        public int[] MessageDeleteIds { get; set; }

        public int[] ImmuneDeleteIDs { get; set; }

        public int[] MedicationDeleteIDs { get; set; }

        public int[] ProcedureDeleteIDs { get; set; }

        public int[] AllergyDeleteIDs { get; set; }

        public int[] PatientRepoDeleteIDs { get; set; }
        
        public int IMMUNIZATIONID { get; set; }

        [Display(Name = "Immunization Name")]
        [StringLength(32, ErrorMessage = "Maximum 32 characters.")]
        public string IMMUNIZATIONNAME { get; set; }

         [Display(Name = "Immune Date")]
        public Nullable<DateTime> IMMUNEDATE { get; set; }


        
         public int ALLERGYID { get; set; }

         [Display(Name = "Allergy Report Name")]
         [StringLength(32, ErrorMessage = "Maximum 32 characters.")]
         public string ALLERGYREPORTNAME { get; set; }

          [Display(Name = "Attach Reports")]
         public string ALLERGYFILEPATH { get; set; }

         [MaxLength(200)]
         public string LOGINNANE { get; set; }

         [MaxLength(256)]
         public string USERPHOTOFILEPATH { get; set; }


         public int flag { get; set; }

         public int ID { get; set; }

         public int PROCEDUREID { get; set; }

         [Display(Name = "Procedure Name")]
         [StringLength(50, ErrorMessage = "Maximum 50 characters.")]
         public string PROCEDUREREPORTNAME { get; set; }

         [Display(Name = "Procedure Date")]
         public Nullable<DateTime> PROCEDUREDATE { get; set; }

         [Display(Name = "Comments")]
         [StringLength(150, ErrorMessage = "Maximum 150 characters.")]
         public string PROCEDURENOTES { get; set; }

        
         [Display(Name = "Provider Type:")]
         public string ProviderType { get; set; }


       
         [Display(Name = "Gender:")]
         public string SearchGender { get; set; }

      
         [Display(Name = "ZIP code:")]
         [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
         [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
         public string SEARCHZIPCODE { get; set; }

         public long MESSAGEID { get; set; }
         [StringLength(46, ErrorMessage = "Maximum 46 characters.")]
         public string MESSAGESUBJECT { get; set; }
         public DateTime SENDDATE { get; set; }
         public string MESSAGEBODY { get; set; }
         public string MESSAGETYPE { get; set; }
         public string MESSAGESTATUS { get; set; }
         public string SENDBY { get; set; }
         public string SENDMESSAGETO { get; set; }
         public string SENDMESSAGEFROM { get; set; }
         public string SENDMESSAGEBCC { get; set; }
         public string SENDMESSAGECC { get; set; }
         public string MESSAGESTATUSFROM { get; set; }
         public string MESSAGESTATUSTO { get; set; }

        
        
        [NotMapped]
        public State State { get; set; }

    }
}