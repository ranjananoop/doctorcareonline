using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DOCVIDEO.Domain;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using DOCVIDEO.ObjectState;
namespace client.Web.Models
{
    public class BookingAppointmentModelOuter
    {



        public int RateQuatermins { get; set; }

        [Display(Name = "Rate in (USD):")]
        public string RateDisplay { get; set; }

        public long APPOINTMENTID { get; set; }

        public string DOCTORID { get; set; }
        public long DOCTORSLOTID { get; set; }


        [Display(Name = "Have you seen this doctor before?")]
        public string ISEXISITING { get; set; }

        public string tabvalue { get; set; }

        public string ISNOTEXISITING { get; set; }

        [Display(Name = "Reason for visit:")]       
        public string REASONFORVISIT { get; set; }

        [Display(Name = "When:")]
        public string APPOINTMENTSTARTTIMEACTUAL { get; set; }

        public Nullable<DateTime> APPOINTMENTSTARTTIME { get; set; }

        [Display(Name = "How long would you like to see the doctor?")]
        public string APPOINTMENTENDTIME { get; set; }

        [Display(Name = "With:")]
        public string Speciality { get; set; }



        public string MessageUpdateStatus { get; set; }

        public string pstatus { get; set; }
        
        
       


        [Display(Name = "First Name")]       
        public string FIRSTNAME { get; set; }


        [Display(Name = "Last Name")]       
        public string LASTNAME { get; set; }

        [Display(Name = "First Name")]
        public string PATFIRSTNAME { get; set; }


        [Display(Name = "Last Name")]
        public string PATLASTNAME { get; set; }
        [Display(Name = "Gender")]        
        public string GENDER { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DOB { get; set; }

        [Display(Name = "Date of birth")]
        public string DOBMonth { get; set; }
        public string DOBDAY { get; set; }


        public string DOBYEAR { get; set; }


        [Display(Name = "Street Address")]       
        public string STREETADDRESS { get; set; }

        [Display(Name = "Salutation")]

        public string Salutation { get; set; }

        [Display(Name = "City")]       
        public string CITY { get; set; }

        [Display(Name = "For non US - citizens")]
        public bool isnotUSA { get; set; }

        [Display(Name = "State")]
        public string USERSTATE { get; set; }


        [Display(Name = "ZIP code")]
        public string ZIPCODE { get; set; }


        [Display(Name = "Email ")]
        [EmailAddress()]
        public string UserName { get; set; }

        [Display(Name = "Email ")]
        [EmailAddress()]
        public string UserNameLogin { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }




        
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PASSWORDS { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.Web.Mvc.Compare("PASSWORDS", ErrorMessage = "Confirm password didn't match with the password")]
        public string CONFIRMPASSWORD { get; set; }

        [Display(Name = "Time zone")]
        public string CurrenttimeZone { get; set; }


        [Display(Name = "Country")]        
        public string COUNTRY { get; set; }

        [Display(Name = "Security Questions")]      
        public string SECURITYQESTID1 { get; set; }

        [Display(Name = "Answer")]       
        public string ANSWER1 { get; set; }

        [Display(Name = "Security Question2")]       
        public string SECURITYQESTID2 { get; set; }

        [Display(Name = "Answer")]       
        public string ANSWER2 { get; set; }


        [Display(Name = "I agree and accept the terms of service")]
        public bool AGREETERMS { get; set; }

        [Display(Name = "Verification Code")]
        public int? VERIFICATIONCODE { get; set; }


       
        [NotMapped]
        public State State { get; set; }

    }
}