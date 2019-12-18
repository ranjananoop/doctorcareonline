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
    public class QuickSearchModel
    {
        

        [Display(Name = "Provider Type:")]
        public string ProviderType { get; set; }

       

        [Display(Name = "Gender:")]
        [MaxLength(10)]
        public string SearchGender { get; set; }

        [Display(Name = "ZIP code:")]
        [RegularExpression(@"^.{5,}$", ErrorMessage = "Minimum 5 characters required")]
        [StringLength(6, ErrorMessage = "Maximum 6 digits.")]
        public string SEARCHZIPCODE { get; set; }
    }
}