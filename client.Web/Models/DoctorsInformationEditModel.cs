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
    public class DoctorsInformationEditModel
    {
        public DoctorsInformationEditModel()
        {

            DoctorsInformationEditModels = new List<DoctorsInformationEditModel>();
        }


        [Display(Name = "Provider Type:")]
        public string ProviderType { get; set; }

        [NotMapped]
        public int DOBDAYDISPLAY { get; set; }

        [NotMapped]
        public bool validlicence { get; set; }

        [Display(Name = "Profile viewed")]
        public string HITCOUNT { get; set; }

        [Display(Name = "Clinic City:")]
        public string CLINICCITY { get; set; }
      
        public int mode { get; set; }


        public string MAPPINGADDRESS { get; set; }

        [Display(Name = "Suffixes:")]
        public string SUFFIX { get; set; }
        [Display(Name = "Suffixes:")]
        public string[] GETSUFFIX { get; set; }

        [Display(Name = " Appointments:")]
        public string appintmentdone { get; set; }

        [Display(Name = "License Expiration:")]
        public DateTime? LICENSEEXPIRESON { get; set; }

        [Display(Name = "License Expiration:")]
        public string LICENSEEXPIRESONSTRING { get; set; }


        public string appintmentpending{ get; set; }

        public bool appintmentcurrent{ get; set; }

        public bool appintmentcurrentnext { get; set; }


        [Display(Name = " Your profile is completed")]
        public string profilecompleted { get; set; }

        [Display(Name = "Payment Recieved")]
        public string paymentrecieved { get; set; }

        [Display(Name = "Patients")]
        public string patientdone { get; set; }
        
        [Display(Name = "Gender:")]
        public string SearchGender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.DateTime)]
        public DateTime? DOB { get; set; }

         [Display(Name = "Date of Birth:")]       
         public string DOBDisplay { get; set; }

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Dob month is required")]
        public string DOBMonth { get; set; }
        [Required(ErrorMessage = "Dob days is required")]
        public int DOBDAY { get; set; }

        [Required(ErrorMessage = "Dob year is required")]
        public int DOBYEAR { get; set; }
        public string trace { get; set; }

        [Display(Name = "Cancelled Appointments:")]
        public string rejectCount { get; set; }
        public string currentDate { get; set; }
        public string inboxUreadMessageCount { get; set; }

        [Display(Name = "ZIP code:")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
        public string SEARCHZIPCODE { get; set; }
        [Display(Name = "User Image:")]
        [MaxLength(256)]
        public string USERPHOTOFILEPATH { get; set; }

        [Display(Name = "First Name:")]
        [StringLength(46, ErrorMessage = "Maximum 46 characters.")]        
        public string FIRSTNAME { get; set; }

        public string MessageUpdateStatus { get; set; }

        public string USERTYPE { get; set; }
        [Display(Name = "Last Name:")]       
        [StringLength(46, ErrorMessage = "Maximum 46 characters.")]
        public string LASTNAME { get; set; }

        [Display(Name = "Specialities:")]
        public string[] Specialities { get; set; }

        public bool appointment { get; set; }



        [Display(Name = "I give the permission for the provider to see my Health History")]
        public bool uploadreport { get; set; }

        [Display(Name = "I certify that I have read and accept the Terms of Use From docare")]
        public bool termsOfUse { get; set; }

        [Display(Name = " I have read and accept the Release Statements")]
        public bool realeaseStatement { get; set; }

        [Display(Name = " Your Initials:")]
        public string patientintials { get; set; }

        [Display(Name = "I give permission for that I will not give treatment")]
        public bool medicaladviceDoctor { get; set; }

        [Display(Name = "I certify that I have read and accept the Terms of Use From docare")]
        public bool termsOfUseDoctor { get; set; }

        [Display(Name = " I have read and accept the Release Statements")]
        public bool releaseStatementDoctor { get; set; }

        [Display(Name = " Your Initials:")]
        public string doctorintials { get; set; }

       
        [Display(Name = "Practicing Since:")]
        public string PRACTISESINCESTRING { get; set; }

        [Display(Name = " Patient Rating:")]
        public Nullable<decimal> CLIENTRATING { get; set; }

         [MaxLength(150)]
        public string Comment { get; set; }

        public Nullable<DateTime> RATINGDATE { get; set; }

        public long APPOINTMENTID { get; set; }

        public string PATIENTID { get; set; }

        public string DOCTORID { get; set; }

        public int DOCTORSLOTID { get; set; }

        [Display(Name = "Have you seen this doctor before?")]
        public string ISEXISITING { get; set; }

        [Display(Name = "Reason For Visit:")]
        [StringLength(300, ErrorMessage = "Maximum 300 characters.")]
        public string REASONFORVISIT { get; set; }

        [Display(Name = "Reason Decline:")]
        [StringLength(300, ErrorMessage = "Maximum 300 characters.")]
        public string REASONTOCANCEL { get; set; }

        [Display(Name = "When:")]
        public string APPOINTMENTSTARTTIMEACTUAL { get; set; }
        public Nullable<DateTime> APPOINTMENTSTARTTIME { get; set; }
        [Display(Name = "How long would you like to see the doctor?")]
        public string APPOINTMENTENDTIME { get; set; }

        [Display(Name = "Overall Rating:")]
        public int Ratings { get; set; }
        public string ProviderId { get; set; }

        public string RemoveProviderId { get; set; }

        [Display(Name = "Provider Type:")]
        public string Speciality { get; set; }


        public byte[] PASSWORDHASH { get; set; }

        public byte[] PASSWORDSALT { get; set; }

        [Display(Name = "Gender:")]      
        public string GENDER { get; set; }

        [Display(Name = "City:")]        
        [StringLength(35, ErrorMessage = "Maximum 35 characters.")]
        public string CITY { get; set; }

        [Display(Name = "ZIP code:")]       
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
        public string ZIPCODE { get; set; }

        
        [Display(Name = "State:")]
        public string USERSTATE { get; set; }


        public int imageId { get; set; }

        [Display(Name = "Languages:")]
        public string[] Languages { get; set; }

        [Display(Name = "Language:")]
        public string Language { get; set; }

        [Display(Name = "Graduating Institution:")]
        [StringLength(52, ErrorMessage = "Maximum 52 characters.")]
        public string GraguatingIn { get; set; }

        [Display(Name = "Year of Experience:")]
        [StringLength(2, ErrorMessage = "Maximum 2 digits.")]
        public string EXPMIN { get; set; }

        [StringLength(2, ErrorMessage = "Maximum 2 digits.")]
        public string EXPMAX { get; set; }

        [Display(Name = "Educational Qualification:")]
        [StringLength(256, ErrorMessage = "Maximum 256 characters.")]
        public string EDUCATIONALQUAL { get; set; }

        [Display(Name = "Award & Certification:")]
        [StringLength(256, ErrorMessage = "Maximum 256 characters.")]
        public string AWARDCERTIFICATION { get; set; }

        [Display(Name = "Hospital Affiliation:")]
        public string HOSPITALAFFILIATION { get; set; }

        [Display(Name = "Hospital Name:")]
        [StringLength(52, ErrorMessage = "Maximum 52 characters.")]
        public string SelectedHospital { get; set; }

        [Display(Name = "Contact Person:")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string CONTACTPERSON { get; set; }

        [Display(Name = "Contact EmailId:")]       
        [EmailAddress()]
        [StringLength(30, ErrorMessage = "Maximum 30 characters.")]
        public string CONTACTMAILID { get; set; }
        [Display(Name = "Street Address1:")]
        [StringLength(95, ErrorMessage = "Maximum 95 characters.")]
        public string USERSTREETADDRESS1 { get; set; }

        

      
        [Display(Name = "Street Address2:")]
        [StringLength(95, ErrorMessage = "Maximum 95 characters.")]
        public string USERSTREETADDRESS2 { get; set; }

       


        [Display(Name = "Practice Name:")]
        [StringLength(46, ErrorMessage = "Maximum 46 characters.")]
        public string PRACTICENAME { get; set; }



        public virtual ICollection<USERSLANGUAGE> Userlanguages { get; set; }
        public virtual ICollection<DOCTORSPECIALITY> DoctorsSpecialities { get; set; }

        [Display(Name = "Medical School:")]
        [StringLength(200, ErrorMessage = "Maximum 200 characters.")]
        public string MEDICALSCHOOL { get; set; }

        [Display(Name = "Residency:")]
        [StringLength(95, ErrorMessage = "Maximum 95 characters.")]
        public string RESIDENCY { get; set; }

       

    
        [Display(Name = "Licensing State:")]
        [StringLength(20, ErrorMessage = "Maximum 20 characters.")]
        public string LICENSEIN { get; set; }

       
        [Display(Name = "NPI:")]
        [StringLength(10, ErrorMessage = "Maximum 10 digits.")]
        public string LICENSE { get; set; }



        [Display(Name = "Email :")]
        [EmailAddress()]
        [StringLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string UserName { get; set; }

        [Display(Name = "Practicing Since:")]
        public Nullable<DateTime> PRACTISESINCE { get; set; }

        [Display(Name = "Board Certifications:")]
        [StringLength(40, ErrorMessage = "Maximum 40 characters.")]
        public string CERTIFICATIONBOARD { get; set; }

       [Display(Name = "Board Certifications:")]
        public string[] CERTIFICATIONBOARDS { get; set; }
        [Display(Name = "Professional Memberships:")]
        [StringLength(200, ErrorMessage = "Maximum 200 characters.")]
        public string PROFESSIONALMEMBERSHIP { get; set; }

        [Display(Name = "Website:")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string WEBSITE { get; set; }

        [Display(Name = "PayPal Email:")]
        [EmailAddress()]
        [StringLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string PAYPALEMAIL { get; set; }

       
        public string NOTES { get; set; } 

        [Display(Name = "About :")]
        [StringLength(500, ErrorMessage = "Maximum 500 characters.")]
        public string ABOUTME { get; set; }


        [Display(Name = "Clinic ZIP code")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
        public string CLINICZIPCODE { get; set; }
        [Display(Name = "Clinic State")]
        public string CLINICUSERSTATE { get; set; }

        [Display(Name = "Street Address1:")]
        [StringLength(128, ErrorMessage = "Maximum 128 digits.")]
        public string STREETADDRESS1 { get; set; }

        [Display(Name = "Street Address2:")]
        [StringLength(128, ErrorMessage = "Maximum 128 digits.")]
        public string STREETADDRESS2 { get; set; }
        [Display(Name = "Whatsapp Number:")]
        [StringLength(11, ErrorMessage = "Maximum 11 characters.")]
        [Required(ErrorMessage = "Mobile number for Whatsapp is required")]
        public string TELEPHONE { get; set; }

        [Display(Name = "Clinic Name:")]
        [StringLength(52, ErrorMessage = "Maximum 52 characters.")]      
        public string INSTITUTIONNAME { get; set; }

        [Display(Name = "About The Clinic :")]        
        public string INSTITUTIONIMAGEPATH { get; set; }

        public string INSTITUTIONIMAGEPATHSECOND{ get; set; }
                
        public string INSTITUTIONIMAGEPATHTHIRD{ get; set; }
        public string INSTITUTIONIMAGEPATHFOURTH { get; set; }
        public State DOCTORDETAILSUPDATE { get; set; }
        public string CurrentClinicImage { get; set; }

        [Display(Name = "Time")]
        public string TimeDisplay { get; set; }

        [Display(Name = "Rate in (US) $:")]
        public string RateDisplay { get; set; }

        [Display(Name = "15min")]       
        public int RateQuatermins { get; set; }

        [Display(Name = "30min")]
        public int RateHalfmins { get; set; }

        [Display(Name = "45min")]
        public int RateFortyFivemins { get; set; }

        [Display(Name = "60min")]
        public int RateHourmins { get; set; }

        [Display(Name = "Salutation")]
        [MaxLength(10)]
        public string Salutation { get; set; }

        [Display(Name = "Time zone:")]       
        public string CurrenttimeZone { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        [StringLength(36, ErrorMessage = "Maximum 36 characters.")]
        public string OldPassword { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [StringLength(36, ErrorMessage = "Maximum 36 characters.")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [StringLength(36, ErrorMessage = "Maximum 36 characters.")]
        [Display(Name = "Confirm Password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Approved?")]
        public bool IsApproved { get; set; }
        public string MESSAGETYPE { get; set; }
        public string MESSAGESTATUS { get; set; }
        public string SENDBY { get; set; }


        [Display(Name = "About Clinic")]
        public string ABOUTCLINIC { get; set; }

        [Display(Name = "To")]
        [StringLength(90, ErrorMessage = "Maximum 90 characters.")]
        public string SENDMESSAGETO { get; set; }
        [Display(Name = "Subject")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string MESSAGESUBJECT { get; set; }
        [Display(Name = "Body")]
        [StringLength(1000, ErrorMessage = "Maximum 1000 characters.")]
        public string MESSAGEBODY { get; set; }
        [StringLength(30, ErrorMessage = "Maximum 30 characters.")]
        public string SENDMESSAGEFROM { get; set; }
        [StringLength(90, ErrorMessage = "Maximum 90 characters.")]
        public string SENDMESSAGEBCC { get; set; }
        [StringLength(90, ErrorMessage = "Maximum 90 characters.")]
        public string SENDMESSAGECC { get; set; }
        [StringLength(90, ErrorMessage = "Maximum 90 characters.")]
        public string MESSAGESTATUSFROM { get; set; }
        [StringLength(30, ErrorMessage = "Maximum 30 characters.")]
        public string MESSAGESTATUSTO { get; set; }

        public string[] UserApproved { get; set; }

        public string[] UserDisApproved { get; set; }

        [NotMapped]
        [MaxLength(200)]
        public string LOGINNANE { get; set; }

        public virtual ICollection<DoctorsInformationEditModel> DoctorsInformationEditModels { get; set; }

        public string APPOINTMENTFIRSTNAME { get; set; }


        public string APPOINTMENTLASTNAME { get; set; }



        public string APPOINTMENTSalutation { get; set; }

        public DOCSUBSCRIPTION DOCSUBSCRIPTION { get; set; }

    }

}