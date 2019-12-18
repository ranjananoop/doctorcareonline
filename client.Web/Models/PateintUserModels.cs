﻿using System;
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
    public class PateintUserModels
    {
        [NotMapped]
        public State State { get; set; }

        public PateintUserModels()
        {
            DoctorInformations = new List<DOCTORSINFORMATION>();           
            Userlanguages = new List<USERSLANGUAGE>();
            Doctorspecialities = new List<DOCTORSPECIALITY>();
            PatientInformations = new List<PATIENTINFORMATION>();
            UserPasswords = new List<USERPASSWORD>();
            DoctorPayRates = new List<DOCTORPAYRATE>();
            DoctorStatuses = new List<DOCTORSTATUS>();  
            UserLogins=new List<USERLOGIN>();
        }

        [Key]
        [Required(ErrorMessage = "User Name is required")]      
        [MaxLength(100)]
        public string UserName { get; set; }
        
     

        [Display(Name = "Registered on")]        
        public DateTime? REGISTEREDON { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(100)]
        public string FIRSTNAME { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public string LASTNAME { get; set; }

        [Display(Name = "Gender")]
        [MaxLength(10)]
        public string GENDER { get; set; }

       [Display(Name = "Date of birth")]
        public DateTime DOB { get; set; }

       [Display(Name = "Date of birth")]
       [NotMapped]
       [Required(ErrorMessage = "Dob month is required")]
       public string DOBMonth { get; set; }
       [Required(ErrorMessage = "Dob days is required")]
       [NotMapped]
       public int DOBDAY { get; set; }

       [Required(ErrorMessage = "Dob year is required")]
       [NotMapped]
       public int DOBYEAR { get; set; }
        [NotMapped]
       public int DOBDAYDISPLAY { get; set; }

       

        [NotMapped]
        public string DOBDISPLAY { get; set; }

        [Display(Name = "Stree Address")]
        [MaxLength(250)]
        public string STREETADDRESS { get; set; }

        [Display(Name = "City")]
        [MaxLength(50)]
        public string CITY { get; set; }

        [Display(Name = "Country")]       
        public string COUNTRY { get; set; }

        [Display(Name = "State")]
        public string USERSTATE { get; set; }

        [Display(Name = "ZIP code")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
        public string ZIPCODE { get; set; }   

        [Display(Name = "User Type")]
        [MaxLength(100)]
        public string USERTYPE { get; set; }

        [NotMapped]
        public string MessageUpdateStatus { get; set; }

        [Display(Name = "User Image")]
        [MaxLength(256)]
        public string USERPHOTOFILEPATH { get; set; }
          
        [Display(Name = "Status")]
        [MaxLength(100)]
        public string STATUS { get; set; }

        [Display(Name = "Security Question")]
        [MaxLength(100)]
        public string SECURITYQESTID1 { get; set; }

        [Display(Name = "Security Question")]
        [MaxLength(100)]
        public string SECURITYQESTID2 { get; set; }

        [Display(Name = "Answer")]
        [MaxLength(100)]
        public string ANSWER1 { get; set; }

        [Display(Name = "Answer")]
        [MaxLength(100)]
        public string ANSWER2 { get; set; }

        [Display(Name = "AGREETERMS")]

        public bool? AGREETERMS { get; set; }

        [Display(Name = "CREATED BY")]
        [MaxLength(100)]
        public string CREATEDBY { get; set; }


        [Display(Name = " MODIFIED  BY")]
        [MaxLength(100)]
        public string MODIFIEDBY { get; set; }

        public Nullable<DateTime> MODIFIEDON { get; set; }

        [Display(Name = "Verification Code")]       
        public int? VERIFICATIONCODE { get; set; }

        [NotMapped]       
        [Display(Name = "Provider Type:")]
        public string ProviderType { get; set; }

        [NotMapped]
        [Display(Name = "New Messages:")]
        public string messageCount { get; set; }

        [NotMapped]      
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [NotMapped]             
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        [Display(Name = "Gender :")]
        [MaxLength(10)]
        public string SearchGender { get; set; }

        [NotMapped]
        [Display(Name = "ZIP code:")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
        public string SEARCHZIPCODE { get; set; }

        public virtual ICollection<DOCTORSINFORMATION> DoctorInformations { get; set; }
        public virtual ICollection<USERSLANGUAGE> Userlanguages { get; set; }
        public virtual ICollection<USERPASSWORD> UserPasswords { get; set; } 
        public virtual ICollection<DOCTORSPECIALITY> Doctorspecialities { get; set; }
        public virtual ICollection<PATIENTINFORMATION> PatientInformations { get; set; }
        public virtual ICollection<DOCTORPAYRATE> DoctorPayRates { get; set; }
        public virtual ICollection<DOCTORSTATUS> DoctorStatuses { get; set; } 
        public virtual ICollection<USERLOGIN> UserLogins { get; set; } 
        [MaxLength(64)]
        public byte[] PASSWORDHASH { get; set; }

        [MaxLength(128)]
        public byte[] PASSWORDSALT { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email :")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Comment { get; set; }

        [Display(Name = "Approved?")]
        public bool IsApproved { get; set; }

        [Display(Name = "Created On")]
        public DateTime? DateCreated { get; set; }

        [Display(Name = "Last Login Date")]
        public DateTime? DateLastLogin { get; set; }

        [Display(Name = "Last Activity Date")]
        public DateTime? DateLastActivity { get; set; }

        [Display(Name = "Last Password Change Date")]
        public DateTime? DateLastPasswordChange { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}