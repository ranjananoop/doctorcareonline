using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace client.Web.Controllers
{
    public class MailTemplateController : Controller
    {
        //
        // GET: /MailTemplate/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Registration()
        {

            return View();
        }
        public ActionResult PatientRegistrationTemplate()
        {

            return View();
        }
        public ActionResult AppointmentConfirmationRequest()
        {

            return View();
        }
        public ActionResult AppointmentRequest()
        {

            return View();
        }
        public ActionResult AppointmentRequestDecline()
        {

            return View();
        }
        public ActionResult AppointmentConfirmation()
        {

            return View();
        }
        public ActionResult Followupemail()
        {

            return View();
        }
        public ActionResult MonthlyReport()
        {

            return View();
        }
        public ActionResult RemainderEmail()
        {

            return View();
        }
        public ActionResult PatientAppointmentCancellation()
        {

            return View();
        }
    }
}
