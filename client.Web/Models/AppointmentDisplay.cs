using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace client.Web.Models
{
    public class AppointmentDisplay
    {
      
        public long APPOINTMENTID { get; set; }
        public string DOCTORID { get; set; }
        public string PATIENTID { get; set; }
        public string STATUS { get; set; }
        public Nullable<DateTime> APPOINTMENTSTARTTIME { get; set; }      
        public string CurrenttimeZone { get; set; }
        public string REASONFORVISIT { get; set; }
        public Nullable<long> PAYMENTSTATUSID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }

    }

}