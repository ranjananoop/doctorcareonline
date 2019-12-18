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
    public class PatientRegistrationModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        [MaxLength(100)]
        public string FIRSTNAME { get; set; }

        public string tabvalue { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public string LASTNAME { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        [MaxLength(10)]
        public string GENDER { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DOB { get; set; }

        [Display(Name = "Date of birth")]     
        public string DOBMonth { get; set; }       
        public string DOBDAY { get; set; }

       
        public string DOBYEAR { get; set; }

        
        [Display(Name = "Street Address")]
        [MaxLength(256)]
        public string STREETADDRESS { get; set; }

         [Display(Name = "Salutation")]
        
         public string Salutation { get; set; }

        [Display(Name = "City")]
        [MaxLength(100)]
        public string CITY { get; set; }

        [Display(Name = "For non US - citizens")]        
        public bool isnotUSA { get; set; }
       
        [Display(Name = "State")]
        public string USERSTATE { get; set; }

   
        [Display(Name = "ZIP code")]
        public string ZIPCODE { get; set; }


        [Display(Name = "Email")]
        [EmailAddress()]
        [StringLength(30, ErrorMessage = "Maximum 30 characters.")]
        [Required(ErrorMessage = "Email-id is required")] 
        public string UserName { get; set; }



        [Required(ErrorMessage = "Password is required")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^.{8,}$", ErrorMessage = "Minimum 8 characters required")]
        [StringLength(32, ErrorMessage = "Maximum 32 characters only")]
        public string PASSWORD { get; set; }

         [DataType(DataType.Password)]
         [Display(Name = "Confirm Password")]
         [System.Web.Mvc.Compare("PASSWORD", ErrorMessage = "Confirm password didn't match with the password")] 
        public string CONFIRMPASSWORD { get; set; }

        [Display(Name = "Time zone")]     
        public string CurrenttimeZone { get; set; }

     
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is Required ")]
        public string COUNTRY { get; set; }

        [Display(Name = "Security Questions")]
        [Required(ErrorMessage = "Security Question Required ")]
        public string SECURITYQESTID1 { get; set; }

        [Display(Name = "Answer")]
        [Required(ErrorMessage = "Security Answer Required ")]
        public string ANSWER1 { get; set; }

        [Display(Name = "Security Question2")]
        [MaxLength(256)]      
        public string SECURITYQESTID2 { get; set; }

        [Display(Name = "Answer")]
        [MaxLength(256)]      
        public string ANSWER2 { get; set; }

       [Required(ErrorMessage = "I agree and accept the terms of service is required")]
        [Display(Name = "I agree and accept the terms of service")]
        public bool AGREETERMS { get; set; }

      [Display(Name = "Captcha")]
        public int? VERIFICATIONCODE { get; set; }


        public State DOCTORREGISTRATION { get; set; }


       
    }
}