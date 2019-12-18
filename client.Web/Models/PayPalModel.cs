using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace client.Web.Models
{
    public class PayPalModel
    {
        public string DOCTORID { get; set; }
        public string PAYPALID { get; set; }
        public string STATUS { get; set; }
        public string RATE { get; set; }
        public string PAYPALKEY { get; set; }
    }
}