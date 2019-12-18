using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DOCVIDEO.Domain;
using DOCVIDEO.PatientInformationServiceBoundedContext;
using DOCVIDEO.PatientInformationServiceRepoUOW;
using DOCVIDEO.UserServiceBoundedContext;
using DOCVIDEO.UserServiceRepoUOW;
using PatientInformationService.Repositories.Disconnected;
using System.IO;
using DOCVIDEO.ObjectState;
using client.Web.Models;
using System.Web.Routing;
using DOCVIDEO.Utility;
using client.Web;
using DOCVIDEO.ErrorLoggingContext;
using UserService.Repositories.Disconnected;
using DOCVIDEO.UserServiceRepoUOW.Disconnected;
using System.Collections.ObjectModel;
//using DOCVIDEO.BOL;
//using DOCVIDEO.DAL;
//using DOCVIDEO.PayPalWrapper;
using PayPal.AdaptivePayments.Model;
namespace client.Web.Controllers
{
    public class PatientProfileController : Controller
    {
        //

        // GET: /PatientProfile/

        AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
        MessageUnitOfWork muok = new MessageUnitOfWork();
        private readonly MessageUnitOfWork muw = new MessageUnitOfWork();
        private readonly AppointmentSlotsUnitOfWork apUOW = new AppointmentSlotsUnitOfWork();
        private readonly AppointmentsUnitOfWork appUOW = new AppointmentsUnitOfWork();
        private readonly DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
        public DoctorInformationController control = new DoctorInformationController();
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        private AccountMembershipService account = new AccountMembershipService();
        public static string Users;
        public static string Speciality;
        public static string GENDER;
        public static string ZIPCODE;
        public static string UserProfilepic;
        public static string loginUserName;

        private readonly DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        [Authorize]
        public ActionResult DoctorsRatingsViews(string id)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            if (id != null)
            {
                Session["DoctorId"] = id;
            }
            else
            {
                string temp = Convert.ToString(Session["DoctorId"]);
                if (temp != "")
                {
                    id = temp;
                }
            }
            DoctorsInformationEditModel model = control.DoctorDisplayData(id);

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var tempdata =
                 apuow.Context.APPOINTMENTS
                   .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                   (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                   .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                   (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.COMMENT, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID })
                .Where(f => f.DOCTORID == id).ToList();
            if (tempdata != null)
            {
                foreach (var value in tempdata)
                {
                    DoctorsInformationEditModel rating = new DoctorsInformationEditModel();
                    rating.FIRSTNAME = value.FIRSTNAME;
                    rating.LASTNAME = value.LASTNAME;
                    rating.USERPHOTOFILEPATH = value.USERPHOTOFILEPATH;
                    rating.CLIENTRATING = value.CLIENTRATING;
                    rating.RATINGDATE = value.RATINGDATE;
                    rating.Comment = value.COMMENT;
                    model.DoctorsInformationEditModels.Add(rating);
                }
            }


            return View(model);
        }
        [Authorize]
        public ActionResult DoctorsInformatonViews(string id, int mode = 0)
        {
            int modevalue = 0;
            if (id != null)
            {
                Session["DoctorId"] = id;
                Session["mode"] = mode;
            }
            else
            {
                string temp = Convert.ToString(Session["DoctorId"]);
                modevalue = Convert.ToInt32(Session["mode"]);
                if (temp != "")
                {
                    id = temp;
                }
            }
            List<string> profileViews = null;
            bool countUpdated = false;

            if (Session["ProfileList"] != null)
            {
                profileViews = (List<string>)Session["ProfileList"];
                if (!profileViews.Contains(id))
                {
                    profileViews.Add(id);
                    Session["ProfileList"] = profileViews;
                }
                else
                {
                    countUpdated = true;
                }
            }
            else
            {
                profileViews = new List<string>();
                profileViews.Add(id);
                Session["ProfileList"] = profileViews;
            }

            //db related code
            if (!countUpdated)
            {
                using (var repo = new PatientInformationRepository(uow))
                {
                    var temp = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == id);
                    temp.MODIFIEDON = DateTime.UtcNow;
                    int value = Convert.ToInt32(temp.HITCOUNT) + 1;
                    temp.HITCOUNT = value.ToString();
                    temp.State = DOCVIDEO.ObjectState.State.Modified;
                    repo.InsertOrUpdate(temp);
                    uow.Save();
                }

            }
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }

            DoctorsInformationEditModel model = control.DoctorDisplayData(id);
            if (mode != 0)
            {
                model.mode = mode;
            }
            else
            {
                model.mode = modevalue;
            }

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var tempdata =
                 apuow.Context.APPOINTMENTS
                   .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                   (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE })
                   .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                   (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID })
                .Where(f => f.DOCTORID == id).ToList();
            if (tempdata != null)
            {
                foreach (var value in tempdata)
                {
                    DoctorsInformationEditModel rating = new DoctorsInformationEditModel();
                    rating.FIRSTNAME = value.FIRSTNAME;
                    rating.LASTNAME = value.LASTNAME;
                    rating.USERPHOTOFILEPATH = value.USERPHOTOFILEPATH;
                    rating.CLIENTRATING = value.CLIENTRATING;
                    rating.RATINGDATE = value.RATINGDATE;
                    model.DoctorsInformationEditModels.Add(rating);
                }
            }


            return View(model);
        }

        [Authorize]
        public ActionResult AppoinmnetRestriction()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            USERSINFORMATION model = new USERSINFORMATION();
            string data = Convert.ToString(Session["ErrorMessage"]);
            if (data != null)
            {

                model.MessageUpdateStatus = data;
            }
            return View(model);
        }
        [Authorize]
        public ActionResult BookYourAppointmentClinic(int id)
        {

            if (id == 0)
            {
                id = Convert.ToInt32(Session["SlotId"]);
            }
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }



            var tempdata = apUOW.Context.DOCTORSLOTS.SingleOrDefault(f => f.DOCTORSLOTID == id);
            var tempSlotid = apUOW.Context.APPOINTMENTSLOTS.SingleOrDefault(f => f.DOCTORCONFIRMEDSLOTID == id);
            if (tempSlotid != null)
            {
                Session["ErrorMessage"] = "You can't have Appointment, doctors slot is already booked";
                return RedirectToAction("AppoinmnetRestriction");
            }

            var pdata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            var ddata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == tempdata.UserName);
            var ddatasl = uow.Context.DoctorsInformations.SingleOrDefault(f => f.UserName == tempdata.UserName);


            if (pdata != null)
            {
                if (pdata.COUNTRY != "United States")
                {
                    Session["ErrorMessage"] = "You can't have Clinic Appointment";
                    return RedirectToAction("AppoinmnetRestriction");
                }
            }
            if (pdata != null && ddata != null)
            {
                if (pdata.USERSTATE != null && ddatasl.STATELICENSE != null)
                {
                    if (pdata.USERSTATE.Trim() != ddatasl.STATELICENSE.Trim())
                    {
                        Session["ErrorMessage"] = "You can't have Appointment with different State doctors";
                        return RedirectToAction("AppoinmnetRestriction");
                    }
                }
            }

            //if (tempdata.FROMTIME < GetLocalTime(DateTime.UtcNow, tempdata.UserName))
            if (tempdata.FROMTIME < DateTime.UtcNow)
            {
                Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                return RedirectToAction("AppoinmnetRestriction");
            }







            BookingAppointmentModel model = new BookingAppointmentModel();
            model.APPOINTMENTSTARTTIME = tempdata.FROMTIME;
            model.DOCTORSLOTID = id;
            model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(tempdata.FROMTIME, tempdata.UserName)) + " " + getAbbreiviation(ddata.CurrenttimeZone);
            model.RateQuatermins = duow.Context.DoctorPayRates.SingleOrDefault(f => f.UserName == ddata.UserName && f.DURATION == 15).RATE;
            model.FIRSTNAME = ddata.FIRSTNAME;
            model.LASTNAME = ddata.LASTNAME;
            model.DOCTORID = tempdata.UserName;
            string temps = null;
            var userspecialyid = duow.Context.Doctorspecialities.Where(w => w.UserName == tempdata.UserName).Select(s => new { s.SPECIALITY });
            if (userspecialyid != null)
            {
                foreach (var spec in userspecialyid)
                {
                    if (temps == null)
                    {
                        temps = spec.SPECIALITY.ToString();
                    }
                    else
                    {
                        temps = temps + "," + spec.SPECIALITY.ToString();
                    }
                }
                if (temps != null)
                {
                    model.Speciality = temps;
                }
            }

            return View(model);

        }

        [Authorize]
        [HttpPost]
        public ActionResult BookYourAppointmentClinic(BookingAppointmentModel model)
        {
            long id = model.DOCTORSLOTID;
            var pdata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            if (pdata != null)
            {
                if (pdata.COUNTRY != "United States")
                {
                    Session["ErrorMessage"] = "You can't have Clinic Appointment";
                    return RedirectToAction("AppoinmnetRestriction");
                }
            }

            var tempdatas = apUOW.Context.DOCTORSLOTS.SingleOrDefault(f => f.DOCTORSLOTID == id);
            if (tempdatas.FROMTIME < DateTime.UtcNow)
            {
                Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                return RedirectToAction("AppoinmnetRestriction");
            }


            // var timezone = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == tempdatas.UserName).CurrenttimeZone;
            //  StringBuilder bodyMsg = new StringBuilder();
            // var tempPateint = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            // var docclinicinfo = duow.Context.DocWorkInstitutions.SingleOrDefault(f => f.UserName == tempdatas.UserName);

            AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
            if (!auok.Context.APPOINTMENTS.Where(f => f.DOCTORSLOTID == id).Any())
            {
                string comment = "Patient Book Your Appointment Clinic.";
                string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.PatientBookYourAppointmentClinic, Convert.ToInt32(model.DOCTORSLOTID), 0, User.Identity.Name, data);



                using (var repo = new AppointmentServiceRepository(auok))
                {
                    APPOINTMENT ap = new APPOINTMENT();
                    ap.APPOINTMENTENDTIME = tempdatas.TOTIME;
                    ap.APPOINTMENTSTARTTIME = tempdatas.FROMTIME;
                    ap.uploadreport = false;
                    ap.termsOfUse = false;
                    ap.realeaseStatement = false;
                    ap.patientintials = "";
                    ap.termsOfUseDoctor = false;
                    ap.releaseStatementDoctor = false;
                    ap.medicaladviceDoctor = false;
                    ap.doctorintials = "";
                    ap.BOOKEDON = DateTime.UtcNow;
                    ap.CREATEDBY = User.Identity.Name;
                    ap.DateCreated = DateTime.UtcNow;
                    ap.DOCTORID = tempdatas.UserName;
                    ap.PATIENTID = User.Identity.Name;
                    ap.STATUS = "CLINIC APPT RQSTD";
                    ap.REASONFORVISIT = model.REASONFORVISIT;
                    ap.ISEXISITING = "";
                    ap.MODIFIEDBY = User.Identity.Name;
                    ap.MODIFIEDON = DateTime.UtcNow;
                    ap.DOCTORSLOTID = tempdatas.DOCTORSLOTID;
                    ap.State = DOCVIDEO.ObjectState.State.Added;
                    repo.InsertOrUpdate(ap);
                    auok.Save();

                    /*

                    bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                    bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                    bodyMsg.Append("<div>");
                    bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='padding:10px;'>");
                    bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                    bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear " + tempPateint.FIRSTNAME + " " + tempPateint.LASTNAME + "</h2>");
                    bodyMsg.Append("<div style='float:left;width:490px;'> ");
                    bodyMsg.Append("<div>Your request for appoint with doctor  on DATE " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(ap.APPOINTMENTSTARTTIME, tempdatas.UserName)) + " " + getAbbreiviation(timezone) + " at  ADDRESS:" + docclinicinfo.INSTITUTIONNAME + " ," + docclinicinfo.CLINICCITY + " has ");
                    bodyMsg.Append("been sent. Once the doctor confirms, you will receive ");
                    bodyMsg.Append("confirmation email.");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<p>Please");
                    bodyMsg.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn 'title='Click'  style='color: #1072B5;'> Click here  </a>   to login to your doccare account!");
                    bodyMsg.Append("</p>");
                    bodyMsg.Append("<div style='color:#1072B5;margin:10px 0px -10px 0px;'>");
                    bodyMsg.Append("<strong>Benefits of signing up");
                    bodyMsg.Append("</strong>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='font-style: italic;font-weight: bold;line-height: 20px;'>");
                    bodyMsg.Append("<ul>");
                    bodyMsg.Append("<li>Access to doctors’ profiles and quality e-medicine</li>");
                    bodyMsg.Append("<li>No travel time</li>");
                    bodyMsg.Append("<li>Minimal wait time </li>");
                    bodyMsg.Append("<li>No insurance paperwork</li>");
                    bodyMsg.Append("<li>Electronic scheduling</li>");
                    bodyMsg.Append("<li>Avoid waiting rooms </li>");
                    bodyMsg.Append("<li>Eliminate time off from work. </li>");
                    bodyMsg.Append("</ul>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>");
                    bodyMsg.Append("<strong>See your Doctor, anytime, anywhere!");
                    bodyMsg.Append("</strong>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<p>If you have questions about doccare online, please email us at");
                    bodyMsg.Append("<a href='#' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
                    bodyMsg.Append("</a> For FAQ, please visit ");
                    bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'> www.PROFESSORSONLINE.com/faq.");
                    bodyMsg.Append("</a>");
                    bodyMsg.Append("</p>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='float:right; margin-left:10px;width:230px;'>");
                    bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
                    bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>doccare online helps doctors and patients connect securely anytime, anywhere!");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='clear:both;'>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='font-size:12px;background:#D3D0D0;padding: 10px;clear:both;margin-top: 10px;'>");
                    bodyMsg.Append("<strong> Disclaimer:");
                    bodyMsg.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
                    bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
                    bodyMsg.Append("</strong>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("</div>");
                    DOCVIDEO.Utility.MailUtility.SendEmail("anupranjan.1983@gmail.com", User.Identity.Name, "", "", "doccare online has sent a secure email to your doctor for  Appointment request at the clinic", bodyMsg.ToString(), false, "", 0);


                    // doctor mail
                    var tempDoctor = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == tempdatas.UserName);

                    var appointmentidfetch = auok.Context.APPOINTMENTS.SingleOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == User.Identity.Name && f.DOCTORSLOTID == id && f.STATUS == "CLINIC APPT RQSTD");
                    StringBuilder bodyMsgDoc = new StringBuilder();
                    bodyMsgDoc.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                    bodyMsgDoc.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                    bodyMsgDoc.Append("<div>");
                    bodyMsgDoc.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='padding:10px;'>");
                    bodyMsgDoc.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                    bodyMsgDoc.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear  " + tempDoctor.Salutation + " " + tempDoctor.FIRSTNAME + " " + tempDoctor.LASTNAME + "</h2>");
                    bodyMsgDoc.Append("<div style='float:left;width:490px;'> ");
                    bodyMsgDoc.Append("<div>Please confirm Appointment  on DATE " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(ap.APPOINTMENTSTARTTIME, tempdatas.UserName)) + " " + getAbbreiviation(timezone) + " to meet your patient.");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<p>Please");
                    bodyMsgDoc.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "DoctorInformation/DoctorAppointmentConfirmation?id=" + appointmentidfetch.APPOINTMENTID + " 'title='Click'  style='color: #1072B5;'> Click here  </a>   to login to your doccare account!");
                    bodyMsgDoc.Append("</p>");
                    bodyMsgDoc.Append("<div style='color:#1072B5;margin:10px 0px -10px 0px;'>");
                    bodyMsgDoc.Append("<strong>Benefits of signing up");
                    bodyMsgDoc.Append("</strong>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='font-style: italic;font-weight: bold;line-height: 20px;'>");
                    bodyMsgDoc.Append("<ul>");
                    bodyMsgDoc.Append("<li>Effective way to provide quality healthcare to patients");
                    bodyMsgDoc.Append("</li>");
                    bodyMsgDoc.Append("<li>Market physicians’ services online");
                    bodyMsgDoc.Append("</li>");
                    bodyMsgDoc.Append("<li>Access to new local and foreign patients.");
                    bodyMsgDoc.Append("</li>");
                    bodyMsgDoc.Append("<li>Payment at time of service");
                    bodyMsgDoc.Append("</li>");
                    bodyMsgDoc.Append("<li>No insurance paperwork");
                    bodyMsgDoc.Append("</li>");
                    bodyMsgDoc.Append("<li>Flexibility for scheduling: online or in office");
                    bodyMsgDoc.Append("</li>");
                    bodyMsgDoc.Append("<li>Minimal support staff");
                    bodyMsgDoc.Append("</li>");
                    bodyMsgDoc.Append("</ul>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>");
                    bodyMsgDoc.Append("<strong>See your patients, anytime, anywhere!");
                    bodyMsgDoc.Append("</strong>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<p>If you have questions about doccare online, please email us at");
                    bodyMsgDoc.Append("<a href='#' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
                    bodyMsgDoc.Append("</a> For FAQ, please visit ");
                    bodyMsgDoc.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'> www.PROFESSORSONLINE.com/faq.");
                    bodyMsgDoc.Append("</a>");
                    bodyMsgDoc.Append("</p>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='float:right; margin-left:10px;width:230px;'>");
                    bodyMsgDoc.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
                    bodyMsgDoc.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>doccare online helps doctors and patients connect securely anytime, anywhere!");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='clear:both;'>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='font-size:12px;background:#D3D0D0;padding: 10px;clear:both;margin-top: 10px;'>");
                    bodyMsgDoc.Append("<strong> Disclaimer:");
                    bodyMsgDoc.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
                    bodyMsgDoc.Append("<strong>could subject you to penalties described in Federal and State law.");
                    bodyMsgDoc.Append("</strong>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("</div>");

                    DOCVIDEO.Utility.MailUtility.SendEmail("anupranjan.1983@gmail.com", tempdatas.UserName, "", "", "PROFESSORSONLINE has a received a secure email for Clinic Appointment.", bodyMsgDoc.ToString(), false, "", 0);
                      */
                }

            }
            else
            {
                Session["ErrorMessage"] = "You can't have Appointment as it's already booked";
                return RedirectToAction("AppoinmnetRestriction");
            }


            using (MessageRepository repo = new MessageRepository(muw))
            {
                MESSAGE message = new MESSAGE();

                message.MESSAGESUBJECT = "Appointment Clinic request";
                message.MESSAGEBODY = "Appointment Clinic Booking request";
                message.DateCreated = DateTime.UtcNow;
                message.SENDMESSAGETO = tempdatas.UserName;
                message.SENDMESSAGEFROM = User.Identity.Name;
                message.SENDBY = User.Identity.Name;
                message.MESSAGESTATUSFROM = "ACTIVE";
                message.MESSAGESTATUSTO = "ACTIVE";
                message.MESSAGETYPE = "BOOK_APPOINTMENT";
                message.MESSAGESTATUS = "UnRead";
                message.MODIFIEDON = DateTime.UtcNow;
                message.SENDDATE = DateTime.UtcNow;
                message.State = DOCVIDEO.ObjectState.State.Added;
                repo.InsertOrUpdate(message);
                muw.Save();

            }
            Session["ErrorMessage"] = "Clinic Appointment Booked Successfully.Please Check your email for Appointment details.";
            return RedirectToAction("AppoinmnetRestriction");

        }
        [Authorize]
        //public ActionResult BookYourAppointment(double timeZoneOffset, int dstObserved, int id=0, string tabvalue = "tab1", string pstatus = null, int appointmentId = 0, int pay=0)
        public ActionResult BookYourAppointment(int id = 0, string tabvalue = "tab1", string pstatus = null, int appointmentId = 0, int pay = 0)
        {
            if (id == 0)
            {
                id = Convert.ToInt32(Session["SlotId"]);
            }
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            if (pstatus != null)
            {
                if (pstatus == "cancel")
                {

                    var tempdatas = apUOW.Context.DOCTORSLOTS.SingleOrDefault(f => f.DOCTORSLOTID == id);
                    var temp = auok.Context.APPOINTMENTS.SingleOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == User.Identity.Name && f.STATUS == "WAITING FOR PAYMENT" && f.APPOINTMENTID == appointmentId);
                    if (temp != null)
                    {
                        using (var repo = new AppointmentServiceRepository(auok))
                        {


                            temp.State = DOCVIDEO.ObjectState.State.Deleted;
                            repo.Delete(Convert.ToInt32(temp.APPOINTMENTID));
                            auok.Save();
                        }
                    }

                }
                else if (pstatus == "success")
                {

                    var tempdatas = apUOW.Context.DOCTORSLOTS.SingleOrDefault(f => f.DOCTORSLOTID == id);
                    var temp = auok.Context.APPOINTMENTS.SingleOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == User.Identity.Name && f.STATUS == "WAITING FOR PAYMENT" && f.APPOINTMENTID == appointmentId);
                    if (temp != null)
                    {
                        using (var repo = new AppointmentServiceRepository(auok))
                        {

                            temp.STATUS = "PAYMENT DONE";
                            temp.MODIFIEDBY = User.Identity.Name;
                            temp.MODIFIEDON = DateTime.UtcNow;
                            temp.PAYAMOUNT = Convert.ToDecimal(pay);
                            temp.State = DOCVIDEO.ObjectState.State.Modified;
                            repo.InsertOrUpdate(temp);
                            auok.Save();
                        }
                    }
                }
            }

            var tempdata = apUOW.Context.DOCTORSLOTS.SingleOrDefault(f => f.DOCTORSLOTID == id);
            var tempSlotid = apUOW.Context.APPOINTMENTSLOTS.SingleOrDefault(f => f.DOCTORCONFIRMEDSLOTID == id);
            if (tabvalue == "tab1")
            {
                if (tempSlotid != null)
                {
                    Session["ErrorMessage"] = "You can't have Appointment, doctors slot is already booked";
                    return RedirectToAction("AppoinmnetRestriction");
                }
            }
            var pdata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            var ddata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == tempdata.UserName);
            var ddatasl = uow.Context.DoctorsInformations.SingleOrDefault(f => f.UserName == tempdata.UserName);


            if (tabvalue == "tab1")
            {
                if (pdata != null && ddata != null)
                {
                    if (pdata.USERSTATE != null && ddatasl.STATELICENSE != null)
                    {
                        if (pdata.USERSTATE.Trim() != ddatasl.STATELICENSE.Trim())
                        {
                            Session["ErrorMessage"] = "You can't have Appointment with different State doctors";
                            return RedirectToAction("AppoinmnetRestriction");
                        }
                    }
                }

                //if (tempdata.FROMTIME <  GetLocalTime(DateTime.UtcNow,tempdata.UserName,(timeZoneOffset+dstObserved)))
                if (tempdata.FROMTIME < DateTime.UtcNow)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                    return RedirectToAction("AppoinmnetRestriction");
                }
                //int tempappdata = appUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.PATIENTID == User.Identity.Name && f.STATUS != "VIDEO APPT CNFRD").Count();
                //if (tempappdata != 0)
                //{
                //    Session["ErrorMessage"] = "You can't have Appointment as you have a appointment request pending";
                //    return RedirectToAction("AppoinmnetRestriction");
                //}
            }
            if (tabvalue == "tab1")
            {
                int pendingAppointment = appUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                  || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as Appointment slot is already booked.";
                    return RedirectToAction("AppoinmnetRestriction");
                }
            }

            if (tabvalue == "tab1")
            {
                int pendingAppointment = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == User.Identity.Name && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                  || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as you already have an confirmed appointment at  this time .";
                    return RedirectToAction("AppoinmnetRestriction");
                }
            }

            if (tabvalue == "tab1")
            {
                var pendingAppointment = appUOW.Context.APPOINTMENTS.SingleOrDefault(f => f.PATIENTID == User.Identity.Name && f.DOCTORID == tempdata.UserName && f.DOCTORSLOTID == id);
                if (pendingAppointment != null)
                {
                    if (pendingAppointment.STATUS == "VIDEO APPT RQSTD")
                    {
                        tabvalue = "tab2";
                    }
                    if (pendingAppointment.STATUS == "PAYMENT DONE")
                    {
                        tabvalue = "tab3";
                    }
                }
            }


            BookingAppointmentModel model = new BookingAppointmentModel();
            model.APPOINTMENTSTARTTIME = tempdata.FROMTIME;
            model.DOCTORSLOTID = id;
            model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(tempdata.FROMTIME, tempdata.UserName)) + " " + getAbbreiviation(ddata.CurrenttimeZone);
            model.RateQuatermins = duow.Context.DoctorPayRates.SingleOrDefault(f => f.UserName == ddata.UserName && f.DURATION == 15).RATE;
            model.FIRSTNAME = ddata.FIRSTNAME;
            model.LASTNAME = ddata.LASTNAME;
            model.tabvalue = tabvalue;
            model.DOCTORID = tempdata.UserName;
            string temps = null;
            var userspecialyid = duow.Context.Doctorspecialities.Where(w => w.UserName == tempdata.UserName).Select(s => new { s.SPECIALITY });
            if (userspecialyid != null)
            {
                foreach (var spec in userspecialyid)
                {
                    if (temps == null)
                    {
                        temps = spec.SPECIALITY.ToString();
                    }
                    else
                    {
                        temps = temps + "," + spec.SPECIALITY.ToString();
                    }
                }
                if (temps != null)
                {
                    model.Speciality = temps;
                }
            }

            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult BookYourAppointment(BookingAppointmentModel model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            return View(_PayForVisit(model));
        }
        [Authorize]
        public ActionResult _ResonForVisit()
        {

            return View();
        }
        [Authorize]
        public ActionResult AppoinmnetRating(int id)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }

            APPOINTMENTRATING model = new APPOINTMENTRATING();
            AppointmentsRatingUnitOfWork aruow = new AppointmentsRatingUnitOfWork();
            int tempcount = aruow.Context.AppointmentRatings.Where(f => f.APPOINTMENTID == id).Count();
            int temp = aruow.Context.APPOINTMENTS.Where(f => f.APPOINTMENTID == id && f.STATUS == "VIDEO APPT CMPLD").Count();
            if (tempcount != 0)
            {
                return RedirectToAction("AppoinmnetRatingRestriction");
            }
            else if (temp == 1)
            {

                DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
                var data = apuow.Context.APPOINTMENTS.SingleOrDefault(f => f.APPOINTMENTID == id);
                var tempdata = apuow.Context.UserInformations.SingleOrDefault(f => f.UserName == data.DOCTORID);
                model.FIRSTNAME = tempdata.FIRSTNAME;
                model.LASTNAME = tempdata.LASTNAME;
                model.APPOINTMENTID = data.APPOINTMENTID;



            }
            else
            {
                return RedirectToAction("AppoinmnetRatingRestriction");
            }
            return View(model);
        }
        [Authorize]
        public ActionResult AppoinmnetRatingRestriction()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            return View();
        }
        [HttpPost]
        [Authorize]
        public JsonResult AppintmentRating(APPOINTMENTRATING model)
        {
            USERSINFORMATION user = null;
            AppointmentsRatingUnitOfWork aruow = new AppointmentsRatingUnitOfWork();
            using (var repo = new AppointmentRatingServiceRepository(aruow))
            {
                APPOINTMENTRATING temp = new APPOINTMENTRATING();
                temp.CLIENTRATING = model.CLIENTRATING;
                temp.COMMENT = model.COMMENT;
                temp.DOCTORONTIME = model.DOCTORONTIME;
                temp.APPOINTMENTID = model.APPOINTMENTID;
                temp.CREATEDBY = User.Identity.Name;
                temp.DateCreated = DateTime.UtcNow;
                temp.MODIFIEDBY = User.Identity.Name;
                temp.RATINGDATE = DateTime.UtcNow;
                temp.MODIFIEDON = DateTime.UtcNow;
                temp.State = DOCVIDEO.ObjectState.State.Added;
                repo.InsertOrUpdate(temp);
                aruow.Save();
            }

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var data = apuow.Context.APPOINTMENTS.SingleOrDefault(f => f.APPOINTMENTID == model.APPOINTMENTID);
            var temprating = duow.Context.AppointmentRatings
            .Join(duow.Context.Appointments, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
            (ui, ds) => new { ui.CLIENTRATING, ds.DOCTORID }).Where(f => f.DOCTORID == data.DOCTORID);
            int count = 0;
            int sum = 0;
            if (temprating != null)
                foreach (var ratedata in temprating)
                {
                    sum = sum + Convert.ToInt32(ratedata.CLIENTRATING);
                    count = count + 1;
                }
            if (count != 0)
            {
                var average = sum / count;
                user = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == data.DOCTORID);
                user.PASSWORDHASH = user.PASSWORDHASH;
                user.PASSWORDSALT = user.PASSWORDSALT;
                user.AverageRating = Convert.ToDecimal(average);

                using (UserRepository repo4 = new UserRepository(duow))
                {
                    user.State = State.Modified;
                    repo4.InsertOrUpdateGraph(user);
                    duow.Save();
                }
            }
            return Json(new { msg = "Appointment Rating Done" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult AppintmentConfirm(BookingAppointmentModel model)
        {

            
               string timezone=null;
               var tempdatas = apUOW.Context.DOCTORSLOTS.SingleOrDefault(f => f.DOCTORSLOTID == model.DOCTORSLOTID);
               var timezonevalue = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == tempdatas.UserName);
               if (timezonevalue != null)
               {
                   timezone = timezonevalue.CurrenttimeZone;
               }
               else
               {
                   timezone = "Central Standard Time";
               }

               var temp = auok.Context.APPOINTMENTS.SingleOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == User.Identity.Name && f.STATUS == "PAYMENT DONE" && f.DOCTORSLOTID==model.DOCTORSLOTID);
               using (var repo = new AppointmentServiceRepository(auok))
               {
                
                   temp.STATUS = "VIDEO APPT RQSTD";
                   temp.MODIFIEDBY = User.Identity.Name;
                   temp.MODIFIEDON = DateTime.UtcNow;
                   temp.State = DOCVIDEO.ObjectState.State.Modified;
                   repo.InsertOrUpdate(temp);
                   auok.Save();
               }
               var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
               var tempPateint = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
              /* StringBuilder bodyMsg = new StringBuilder();  
       
           
               bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
               bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
               bodyMsg.Append("<div>");
               bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("<div style='padding:10px;'>");
               bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
               bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear " + tempPateint.FIRSTNAME + " " + tempPateint.LASTNAME + "</h2>");
               bodyMsg.Append("<div style='float:left;width:490px;'> ");
               bodyMsg.Append("<div>Your request for appoint with doctor  on DATE " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID)) + " " + getAbbreiviation(tempvalue) + "  has been sent. Once ");
               bodyMsg.Append("the doctor confirms, you will receive confirmation email. ");
               bodyMsg.Append("</div>");
               bodyMsg.Append("<p>Please ");
               bodyMsg.Append("<a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn' style='color:#1072B5;'> Click here");
               bodyMsg.Append("</a>  to login to your doccare account!");
               bodyMsg.Append("</p>   ");
               bodyMsg.Append("<div style='color: #1072B5; margin: 10px 0px -10px 0px;'><strong>Benefits of signing up</strong></div>");
               bodyMsg.Append("<div style='font-style: italic; font-weight: bold; line-height: 20px;'>");
               bodyMsg.Append("<ul>");
               bodyMsg.Append("<li>Access to doctors’ profiles and quality e-medicine</li>");
               bodyMsg.Append("<li>No travel time</li>");
               bodyMsg.Append("<li>Minimal wait time </li>");
               bodyMsg.Append("<li>No insurance paperwork</li>");
               bodyMsg.Append("<li>Electronic scheduling</li>");
               bodyMsg.Append("<li>Avoid waiting rooms </li>");
               bodyMsg.Append("<li>Eliminate time off from work. </li>");
               bodyMsg.Append("</ul>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>");
               bodyMsg.Append("<strong>See your Doctors, anytime, anywhere!");
               bodyMsg.Append("</strong>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("<p>If you have questions about doccare online, please email us at");
               bodyMsg.Append("<a href='#' style='color:#1072B5;'>  helpdesk@PROFESSORSONLINE.com");
               bodyMsg.Append("</a> For FAQ, please visit ");
               bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>  www.PROFESSORSONLINE.com/faq.");
               bodyMsg.Append("</a>");
               bodyMsg.Append("</p>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("<div style='float:right; margin-left:10px;width:230px;'>");
               bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
               bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'> doccare online helps doctors and patients connect securely anytime, anywhere!");
               bodyMsg.Append("</div>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("<div style='clear:both;'>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("<div style='font-size:12px;background:#D3D0D0;padding: 10px;clear:both;margin-top: 10px;'>");
               bodyMsg.Append("<strong> Disclaimer:");
               bodyMsg.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
               bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
               bodyMsg.Append("</strong>");
               bodyMsg.Append("</div>");
               bodyMsg.Append("</div>");
               //DOCVIDEO.Utility.MailUtility.SendEmail("anupranjan.1983@gmail.com", temp.PATIENTID, "", "", "doccare online has a received a secure email for online Appointment." , bodyMsg.ToString(), false, "", 0);

               var tempDoctor = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID);
               StringBuilder bodyMsgDoc = new StringBuilder();
                       bodyMsgDoc.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                       bodyMsgDoc.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                       bodyMsgDoc.Append("<div>");
                       bodyMsgDoc.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<div style='padding:10px;'>");
                       bodyMsgDoc.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                       bodyMsgDoc.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear  " +tempDoctor.Salutation + " "  + tempDoctor.FIRSTNAME + " " + tempDoctor.LASTNAME + "</h2>");
                       bodyMsgDoc.Append("<div style='float:left;width:490px;'> ");
                       bodyMsgDoc.Append("<div>Please confirm Appointment  on DATE " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME, tempdatas.UserName)) + " " + getAbbreiviation(tempvalue) + " to meet your patient.");   
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<p>Please");
                       bodyMsgDoc.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "DoctorInformation/DoctorAppointmentConfirmation?id="+temp.APPOINTMENTID+" 'title='Click' style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");                                 
                       bodyMsgDoc.Append("</a>  to login to your doccare account!");
                       bodyMsgDoc.Append("</p>");
                       bodyMsgDoc.Append("<div style='color:#1072B5;margin:10px 0px -10px 0px;'>");
                       bodyMsgDoc.Append("<strong> Benefits of signing up");
                       bodyMsgDoc.Append("</strong>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<div style='font-style: italic;font-weight: bold;line-height: 20px;'>");
                       bodyMsgDoc.Append("<ul>");
                       bodyMsgDoc.Append("<li>Effective way to provide quality healthcare to patients");
                       bodyMsgDoc.Append("</li>");
                       bodyMsgDoc.Append("<li>Market physicians’ services online");
                       bodyMsgDoc.Append("</li>");
                       bodyMsgDoc.Append("<li>Access to new local and foreign patients.");
                       //bodyMsgDoc.Append("<li>Access to new local and foreign patients.");
                       //bodyMsgDoc.Append("<li>Access to new local and foreign patients.");
                       bodyMsgDoc.Append("</li>");
                       bodyMsgDoc.Append("<li>Payment at time of service");
                       bodyMsgDoc.Append("</li>");
                       bodyMsgDoc.Append("<li>No insurance paperwork");
                       bodyMsgDoc.Append("</li>");
                       bodyMsgDoc.Append("<li>Flexibility for scheduling: online or in office");
                       bodyMsgDoc.Append("</li>");
                       bodyMsgDoc.Append("<li>Minimal support staff");
                       bodyMsgDoc.Append("</li>");
                       bodyMsgDoc.Append("</ul>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>");
                       bodyMsgDoc.Append("<strong>See your patients, anytime, anywhere!");
                       bodyMsgDoc.Append("</strong>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<p>If you have questions about doccare online, please email us at");
                       bodyMsgDoc.Append("<a href='#' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
                       bodyMsgDoc.Append("</a> For FAQ, please visit ");
                       bodyMsgDoc.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'> www.PROFESSORSONLINE.com/faq.");
                       bodyMsgDoc.Append("</a>");
                       bodyMsgDoc.Append("</p>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<div style='float:right; margin-left:10px;width:230px;'>");
                       bodyMsgDoc.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
                       bodyMsgDoc.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'> doccare online helps doctors and patients connect securely anytime, anywhere!");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<div style='clear:both;'>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("<div style='font-size:12px;background:#D3D0D0;padding: 10px;clear:both;margin-top: 10px;'>");
                       bodyMsgDoc.Append("<strong> Disclaimer:");
                       bodyMsgDoc.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
                       bodyMsgDoc.Append("<strong>could subject you to penalties described in Federal and State law.");
                       bodyMsgDoc.Append("</strong>");
                       bodyMsgDoc.Append("</div>");
                       bodyMsgDoc.Append("</div>");

              // DOCVIDEO.Utility.MailUtility.SendEmail("anupranjan.1983@gmail.com", temp.DOCTORID, "", "", "doccare online has a received a secure email for online Appointment.", bodyMsgDoc.ToString(), false, "", 0);
               
            string comment = "Patient Appointment Confirmation Request.";
            string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
            //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.PatientAppointmentConfirm, Convert.ToInt32(model.DOCTORSLOTID), 0, User.Identity.Name, data);
            */
            using (MessageRepository repo = new MessageRepository(muw))
            {
                MESSAGE message = new MESSAGE();

                message.MESSAGESUBJECT = "Appointment  request";
                message.MESSAGEBODY = "Appointment  Booking request";
                message.DateCreated = DateTime.UtcNow;
                message.SENDMESSAGETO = model.DOCTORID;
                message.SENDMESSAGEFROM = User.Identity.Name;
                message.SENDBY = User.Identity.Name;
                message.MESSAGESTATUSFROM = "ACTIVE";
                message.MESSAGESTATUSTO = "ACTIVE";
                message.MESSAGETYPE = "BOOK_APPOINTMENT";
                message.MESSAGESTATUS = "UnRead";
                message.MODIFIEDON = DateTime.UtcNow;
                message.SENDDATE = DateTime.UtcNow;
                message.State = DOCVIDEO.ObjectState.State.Added;
                repo.InsertOrUpdate(message);
                muw.Save();

            }
            return Json(new { msg = "Appointment Request Sent" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult Appintments(BookingAppointmentModel model)
        {
            bool flag = false;
            DateTime temp = model.APPOINTMENTSTARTTIME.Value.AddMinutes(15);
            int tempdata = apUOW.Context.DOCTORSLOTS.Where(f => f.FROMTIME >= model.APPOINTMENTSTARTTIME && f.TOTIME <= temp && f.UserName == model.DOCTORID).Count();

            int bookedslot = apUOW.Context.DOCTORSLOTS
                 .Join(apUOW.Context.APPOINTMENTSLOTS, ds => ds.DOCTORSLOTID, aps => aps.DOCTORCONFIRMEDSLOTID,
                 (ds, aps) => new { ds.DOCTORSLOTID, aps.DOCTORCONFIRMEDSLOTID, ds.FROMTIME, ds.TOTIME, ds.UserName }).Where(f => f.FROMTIME >= model.APPOINTMENTSTARTTIME && f.TOTIME <= temp && f.UserName == model.DOCTORID && f.DOCTORSLOTID == f.DOCTORCONFIRMEDSLOTID)
                .Distinct().Count();
            tempdata = tempdata - bookedslot;
            if (tempdata >= 1)
            {
                flag = true;
            }


            int rate = duow.Context.DoctorPayRates.SingleOrDefault(f => f.UserName == model.DOCTORID && f.DURATION == 15).RATE;

            if (rate == 0)
            {
                return Json(new { msg = "Error : Requested Slots Pay Rates are not Available ,Try different duration" }, JsonRequestBehavior.AllowGet);
            }
            if (flag)
            {
                AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
                using (var repo = new AppointmentServiceRepository(auok))
                {
                    APPOINTMENT ap = new APPOINTMENT();
                    ap.APPOINTMENTENDTIME = model.APPOINTMENTSTARTTIME.Value.AddMinutes(15);
                    ap.APPOINTMENTSTARTTIME = model.APPOINTMENTSTARTTIME;
                    ap.uploadreport = false;
                    ap.termsOfUse = false;
                    ap.realeaseStatement = false;
                    ap.patientintials = "";
                    ap.termsOfUseDoctor = false;
                    ap.releaseStatementDoctor = false;
                    ap.medicaladviceDoctor = false;
                    ap.doctorintials = "";
                    ap.BOOKEDON = DateTime.UtcNow;
                    ap.CREATEDBY = User.Identity.Name;
                    ap.DateCreated = DateTime.UtcNow;
                    ap.DOCTORID = model.DOCTORID;
                    ap.PATIENTID = User.Identity.Name;
                    ap.STATUS = "WAITING FOR PAYMENT";
                    if (model.REASONFORVISIT != null)
                    {
                        ap.REASONFORVISIT = model.REASONFORVISIT;
                    }
                    else
                    {
                        ap.REASONFORVISIT = "";
                    }
                    ap.ISEXISITING = model.ISEXISITING;
                    ap.MODIFIEDBY = User.Identity.Name;
                    ap.MODIFIEDON = DateTime.UtcNow;
                    ap.DOCTORSLOTID = model.DOCTORSLOTID;
                    Session["DOCTORID"] = model.DOCTORID;
                    ap.State = DOCVIDEO.ObjectState.State.Added;
                    repo.InsertOrUpdate(ap);
                    auok.Save();

                }
            }
            else
            {
                return Json(new { msg = " Error : Requested Slots are not Available" }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult _PayForVisit()
        {

            return View();
        }
        [Authorize]
        [HttpPost]
        public BookingAppointmentModel _PayForVisit(BookingAppointmentModel model)
        {


            var tempdata = apUOW.Context.DOCTORSLOTS.SingleOrDefault(f => f.DOCTORSLOTID == model.DOCTORSLOTID);
            var temp = auok.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.PATIENTID == User.Identity.Name
                        &&   f.STATUS == "WAITING FOR PAYMENT" && f.DOCTORSLOTID == model.DOCTORSLOTID)
                        .OrderByDescending(f => f.DateCreated).Take(1);

            var ddata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == tempdata.UserName);

            model.APPOINTMENTSTARTTIME = tempdata.FROMTIME;
            model.DOCTORSLOTID = model.DOCTORSLOTID;
            model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", tempdata.FROMTIME);
            model.FIRSTNAME = ddata.FIRSTNAME;
            model.LASTNAME = ddata.LASTNAME;
            model.DOCTORID = tempdata.UserName;
            string temps = null;
            var userspecialyid = duow.Context.Doctorspecialities.Where(w => w.UserName == tempdata.UserName)
                .Select(s => new { s.SPECIALITY });
            if (userspecialyid != null)
            {
                foreach (var spec in userspecialyid)
                {
                    if (temps == null)
                    {
                        temps = spec.SPECIALITY.ToString();
                    }
                    else
                    {
                        temps = temps + "," + spec.SPECIALITY.ToString();
                    }
                }
                if (temps != null)
                {
                    model.Speciality = temps;
                }
            }
            long id = 0;
            int duration = Convert.ToInt32(model.APPOINTMENTENDTIME);
            int rate = duow.Context.DoctorPayRates.SingleOrDefault(f => f.UserName == model.DOCTORID && f.DURATION == 15).RATE;

            if (rate == 0)
            {
                ModelState.AddModelError("", "Error :Appointment cannot be done as no payrate information for this duration");

                if (temp != null)
                {
                    foreach (var tempvalue in temp)
                    {
                        using (var repo = new AppointmentServiceRepository(auok))
                        {


                            tempvalue.State = DOCVIDEO.ObjectState.State.Deleted;
                            repo.Delete(Convert.ToInt32(tempvalue.APPOINTMENTID));
                            auok.Save();
                        }
                    }
                }
                return model;
            }
            else
            {
                if (temp != null)
                {
                    foreach (var tempvalue in temp)
                    {
                        id = tempvalue.APPOINTMENTID;
                    }
                    //string docotorPayPalId = ddata.PAYPALEMAIL;
                    //PaySample samplePay = new PaySample();
                    ////if (docotorPayPalId == "" || docotorPayPalId==null)
                    ////    docotorPayPalId = "anoopranjan.1983@gmail.com";
                    //PayRequest requestPay = samplePay.ParallelPayment(rate, docotorPayPalId, model.DOCTORSLOTID, id);
                    //PayResponse pr = samplePay.PayAPIOperations(requestPay, false);

                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["RETURN_URL"] + "/PatientProfile/BookYourAppointment/?pstatus=success&appointmentId=" + id + "&tabvalue=tab3" + "&id=" + model.DOCTORSLOTID + "&pay=" + rate);

                }
            }
            return model;
        }


        [Authorize]
        public ActionResult _ConfirmPayForVisit()
        {
            return View();
        }
        [Authorize]
        public ActionResult MyProfile()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            var messagecounts = muok.Context.Messages.Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUS == "UnRead" && f.MESSAGESTATUSFROM == "ACTIVE").Count();
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            string[] prefereddoctorslist = null;
            var data = pduow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name).Select(s => new { s.DOCTORID });
            string tempdata = null;
            foreach (var temp in data)
            {
                if (tempdata == null)
                {
                    tempdata = temp.DOCTORID;
                }
                else
                {
                    tempdata = tempdata + "," + temp.DOCTORID;
                }

            }

            if (tempdata != null)
            {
                prefereddoctorslist = tempdata.Split(',');
            }
            else
            {
                prefereddoctorslist = null;
            }

            ViewBag.PreferedDoctors = prefereddoctorslist;
            string id = User.Identity.Name;
            Session["LogInError"] = "";
            USERSINFORMATION user = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == id);
            Session["UserName"] = user.FIRSTNAME;
            Session["ImagePath"] = user.USERPHOTOFILEPATH;
            if (user.DOB != null)
            {
                user.DOBDISPLAY = user.DOB.Value.ToString("d");
            }
            user.messageCount = messagecounts.ToString();
            _quicksearch();
            string message = Convert.ToString(Session["Message"]);
            user.MessageUpdateStatus = message;
            Session["Message"] = "";
            if (user.DOB != null)
            {
                user.DOBDAYDISPLAY = user.DOB.Value.Day;
                user.DOBDAY = user.DOB.Value.Day;
                user.DOBMonth = user.DOB.Value.Month.ToString();
                user.DOBYEAR = user.DOB.Value.Year;
            }
            return View(user);
        }
        [HttpPost]
        [Authorize]
        public ActionResult MyProfile(USERSINFORMATION model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;

            View(model);

            return RedirectToAction("MyProfile");
        }


        [Authorize]
        [HttpGet]
        public ActionResult Personalinfo()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            USERSINFORMATION usersinformation = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            if (usersinformation.DOB != null)
            {
                usersinformation.DOBDAYDISPLAY = usersinformation.DOB.Value.Day;
                usersinformation.DOBDAY = usersinformation.DOB.Value.Day;
                usersinformation.DOBMonth = usersinformation.DOB.Value.Month.ToString();
                usersinformation.DOBYEAR = usersinformation.DOB.Value.Year;
            }
            usersinformation.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return View(usersinformation);


        }

        [Authorize]
        [HttpPost]
        public ActionResult Personalinfo(USERSINFORMATION usersinformation)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            using (var repo = new PatientInformationRepository(uow))
            {
                var temp = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
                temp.MODIFIEDON = DateTime.UtcNow;
                if (usersinformation.FIRSTNAME != null && usersinformation.LASTNAME != null && usersinformation.GENDER != null && usersinformation.DOBYEAR != 0 && usersinformation.DOBMonth != null && usersinformation.DOBDAY != 0)
                {
                    if (usersinformation.FIRSTNAME.Length > 0 && usersinformation.LASTNAME.Length > 0 && usersinformation.GENDER.Length > 0 && usersinformation.DOBYEAR != 0 && usersinformation.DOBMonth.Length > 0 && usersinformation.DOBDAY != 0)
                    {
                        if (ModelState.IsValid)
                        {
                            DateTime tempdate = new DateTime(Convert.ToInt32(usersinformation.DOBYEAR), Convert.ToInt32(usersinformation.DOBMonth), Convert.ToInt32(usersinformation.DOBDAY));

                            if (usersinformation.Salutation != null)
                                temp.Salutation = usersinformation.Salutation;
                            temp.FIRSTNAME = usersinformation.FIRSTNAME;
                            temp.LASTNAME = usersinformation.LASTNAME;
                            temp.GENDER = usersinformation.GENDER;
                            temp.PASSWORDHASH = temp.PASSWORDHASH;
                            temp.PASSWORDSALT = temp.PASSWORDSALT;
                            usersinformation.DOB = tempdate;
                            temp.DOB = usersinformation.DOB;
                            temp.DateLastActivity = DateTime.UtcNow;
                            temp.DateLastLogin = DateTime.UtcNow;
                            temp.DateLastPasswordChange = DateTime.UtcNow;
                            temp.IsApproved = true;
                            temp.MODIFIEDBY = User.Identity.Name;
                            temp.MODIFIEDON = DateTime.UtcNow;
                            temp.DOBMonth = "";
                            temp.DOBDAY = 1;
                            temp.DOBYEAR = 1;
                            if (UserProfilepic != null)
                                temp.USERPHOTOFILEPATH = UserProfilepic;
                            repo.InsertOrUpdate(temp);
                            uow.Save();
                            ModelState.AddModelError("", "Basic Information  updated");
                        }
                    }
                    else
                    {
                        if (usersinformation.DOBYEAR == 0 || usersinformation.DOBMonth.Length == 0 || usersinformation.DOBDAY == 0)
                        {
                            ModelState.AddModelError("", "Error :Date of birth is Required");
                        }
                        if (usersinformation.FIRSTNAME.Length == 0)
                        {
                            ModelState.AddModelError("", "Error :First Name is Required");
                        }
                        if (usersinformation.LASTNAME.Length == 0)
                        {
                            ModelState.AddModelError("", "Error :Last Name is Required");
                        }
                        if (usersinformation.GENDER.Length == 0)
                        {
                            ModelState.AddModelError("", "Error :Gender is Required");
                        }

                        return View(usersinformation);
                    }
                }
                else
                {
                    if (usersinformation.DOBYEAR == 0 || usersinformation.DOBMonth == null || usersinformation.DOBDAY == 0)
                    {
                        ModelState.AddModelError("", "Error :Date of birth is Required");
                    }
                    if (usersinformation.FIRSTNAME == null)
                    {
                        ModelState.AddModelError("", "Error :First Name is Required");
                    }
                    if (usersinformation.LASTNAME == null)
                    {
                        ModelState.AddModelError("", "Error :Last Name is Required");
                    }
                    if (usersinformation.GENDER == null)
                    {
                        ModelState.AddModelError("", "Error :Gender is Required");
                    }

                    return View(usersinformation);
                }

            }
            return RedirectToAction("ContactInformation");


        }
        public string Usertype()
        {
            DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();
            var data = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            return data.USERTYPE;
        }
        [Authorize]
        public ActionResult VideoConf(string id)
        {
            DoctorsInformationEditModel temp = new DoctorsInformationEditModel();
            return View(temp);
        }


        public ActionResult CurrentDate()
        {
            //wed ,Sep 04,2013
            //    var currentdate=DateTime.Now.DayOfWeek.ToString().Substring(0,2)+","+DateTime.Now.Date.M
            return Json(new { msg = DateTime.Now.Date.ToString("ddd,MMM dd,yyyy") }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MessageCount()
        {
            var messagecounts = muok.Context.Messages.Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUS == "UnRead" && f.MESSAGESTATUSFROM == "ACTIVE").ToList();
            if (messagecounts != null)
            {
                return Json(new { msg = messagecounts.Count.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "0" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImagePath()
        {
            var data = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            if (data != null)
            {
                return Json(new { msg = data.USERPHOTOFILEPATH }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "no data" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Username()
        {
            var data = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            if (data != null)
            {
                return Json(new { msg = data.Salutation + " " + data.LASTNAME }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "no data" }, JsonRequestBehavior.AllowGet);
        }



        [Authorize]
        public ActionResult BasicInformation()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            string id = User.Identity.Name;
            USERSINFORMATION usersinformation = uow.Context.UsersInformations.Find(id);
            _changepassword();
            _quicksearch();
            if (usersinformation == null)
            {
                return HttpNotFound();
            }
            if (usersinformation.DOB != null)
            {
                usersinformation.DOBDAYDISPLAY = usersinformation.DOB.Value.Day;
                usersinformation.DOBDAY = usersinformation.DOB.Value.Day;
                usersinformation.DOBMonth = usersinformation.DOB.Value.Month.ToString();
                usersinformation.DOBYEAR = usersinformation.DOB.Value.Year;
            }
            return View(usersinformation);
        }

        [Authorize]
        public ActionResult Appointment(string id = "tab1")
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            APPOINTMENT model = new APPOINTMENT();
            if (id != "")
            {
                model.tabindex = id;
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Appointment(APPOINTMENT model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            return View();
        }

        public ActionResult GetUpcomingAppintmentresent(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.DOCTORID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.STATUS, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.APPOINTMENTID }).Where(f => f.PATIENTID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= DateTime.UtcNow && (f.STATUS == "VIDEO APPT RQSTD" || f.STATUS == "VIDEO APPT CNCLD" || f.STATUS == "CLINIC APPT RQSTD" || f.STATUS == "CLINIC APPT CNCLD" || f.STATUS == "PATIENT CLINIC CNCLD" || f.STATUS == "PATIENT VIDEO CNCLD")).OrderBy(f => f.APPOINTMENTSTARTTIME);
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        public ActionResult GetUpcomingAppintment(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            DateTime currentLocalTime = GetLocalTime(DateTime.UtcNow);
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.DOCTORID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.STATUS, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.APPOINTMENTID }).Where(f => f.PATIENTID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= DateTime.UtcNow && (f.STATUS == "VIDEO APPT CNFRD" || f.STATUS == "CLINIC APPT CNFRD")).OrderBy(f => f.APPOINTMENTSTARTTIME);
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }
        public int ProfilePercent(string id)
        {
            double percent = 0;
            int total = 11;
            int currentvalue = 0;

            var userinfo = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == id);
            if (userinfo != null)
            {
                if (userinfo.Salutation != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.FIRSTNAME != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.LASTNAME != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.DOB != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.USERSTREETADDRESS1 != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.CITY != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.GENDER != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.USERSTATE != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.ZIPCODE != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.CurrenttimeZone != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.USERPHOTOFILEPATH != null)
                    currentvalue = currentvalue + 1;

            }


            if (currentvalue != 0)
            {
                percent = (currentvalue * 100) / total;
            }
            int value = Convert.ToInt32(percent);
            return value;
        }
        [Authorize]
        public ActionResult Dashboard()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DateTime datepool = DateTime.UtcNow.AddMinutes(-30);
            DateTime current = DateTime.UtcNow;
            PatientInformationEditModel model = new PatientInformationEditModel();
            model.inboxUreadMessageCount = muok.Context.Messages.Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUS == "UnRead" && f.MESSAGESTATUSFROM == "ACTIVE").Count().ToString();
            string id = User.Identity.Name;
            var data = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == id);
            model.FIRSTNAME = data.FIRSTNAME;
            model.LASTNAME = data.LASTNAME;
            model.USERPHOTOFILEPATH = data.USERPHOTOFILEPATH;
            model.Salutation = data.Salutation;
            model.paymentdone = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "VIDEO APPT CMPLD").Sum(f => f.PAYAMOUNT).ToString();
            model.appintmentdone = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "VIDEO APPT CMPLD").Select(f => new { f.DOCTORID }).Distinct().Count().ToString();
            model.appintmentdoneclinic = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "CLINIC APPT RQSTD").Select(f => new { f.DOCTORID }).Distinct().Count().ToString();
            model.profilecompleted = "0%";
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            model.prefferedprovidercount = pduow.Context.UserInformations.Join(pduow.Context.PrefferedDoctors, ui => ui.UserName, ds => ds.DOCTORID,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.GENDER, ui.USERSTATE, ds.UserName, ds.DOCTORID }).Where(ds => ds.UserName == User.Identity.Name).Distinct().ToList().Count().ToString();

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();

            var tempdata =
                 apuow.Context.APPOINTMENTS
                   .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                   (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                   .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                   (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ui.COMMENT })
                .Where(f => f.PATIENTID == id).OrderByDescending(f => f.RATINGDATE).Take(2).ToList();
            if (tempdata != null)
            {
                foreach (var value in tempdata)
                {
                    DoctorsInformationEditModel rating = new DoctorsInformationEditModel();
                    rating.FIRSTNAME = value.FIRSTNAME;
                    rating.LASTNAME = value.LASTNAME;
                    rating.USERPHOTOFILEPATH = value.USERPHOTOFILEPATH;
                    rating.CLIENTRATING = value.CLIENTRATING;
                    rating.RATINGDATE = value.RATINGDATE;
                    rating.Comment = value.COMMENT;
                    model.DoctorsInformationEditModels.Add(rating);
                }
            }
            model.appintmentcurrent = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "VIDEO APPT CNFRD" && (f.APPOINTMENTSTARTTIME < current && f.APPOINTMENTSTARTTIME > datepool)).Select(f => new { f.DOCTORSLOTID }).Distinct().Any();
            model.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return View(model);
        }

        [Authorize]
        public ActionResult SocialHubConnect()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DateTime datepool = DateTime.UtcNow.AddMinutes(-30);
            DateTime current = DateTime.UtcNow;
            PatientInformationEditModel model = new PatientInformationEditModel();
            model.inboxUreadMessageCount = muok.Context.Messages.Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUS == "UnRead" && f.MESSAGESTATUSFROM == "ACTIVE").Count().ToString();
            string id = User.Identity.Name;
            var data = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == id);
            model.FIRSTNAME = data.FIRSTNAME;
            model.LASTNAME = data.LASTNAME;
            model.USERPHOTOFILEPATH = data.USERPHOTOFILEPATH;
            model.Salutation = data.Salutation;
            model.paymentdone = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "VIDEO APPT CMPLD").Sum(f => f.PAYAMOUNT).ToString();
            model.appintmentdone = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "VIDEO APPT CMPLD").Select(f => new { f.DOCTORID }).Distinct().Count().ToString();
            model.appintmentdoneclinic = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "CLINIC APPT RQSTD").Select(f => new { f.DOCTORID }).Distinct().Count().ToString();
            model.profilecompleted = "0%";
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            model.prefferedprovidercount = pduow.Context.UserInformations.Join(pduow.Context.PrefferedDoctors, ui => ui.UserName, ds => ds.DOCTORID,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.GENDER, ui.USERSTATE, ds.UserName, ds.DOCTORID }).Where(ds => ds.UserName == User.Identity.Name).Distinct().ToList().Count().ToString();

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();

            var tempdata =
                 apuow.Context.APPOINTMENTS
                   .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                   (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                   .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                   (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ui.COMMENT })
                .Where(f => f.PATIENTID == id).OrderByDescending(f => f.RATINGDATE).Take(2).ToList();
            if (tempdata != null)
            {
                foreach (var value in tempdata)
                {
                    DoctorsInformationEditModel rating = new DoctorsInformationEditModel();
                    rating.FIRSTNAME = value.FIRSTNAME;
                    rating.LASTNAME = value.LASTNAME;
                    rating.USERPHOTOFILEPATH = value.USERPHOTOFILEPATH;
                    rating.CLIENTRATING = value.CLIENTRATING;
                    rating.RATINGDATE = value.RATINGDATE;
                    rating.Comment = value.COMMENT;
                    model.DoctorsInformationEditModels.Add(rating);
                }
            }
            model.appintmentcurrent = appUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == id && f.STATUS == "VIDEO APPT CNFRD" && (f.APPOINTMENTSTARTTIME < current && f.APPOINTMENTSTARTTIME > datepool)).Select(f => new { f.DOCTORSLOTID }).Distinct().Any();
            model.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return View(model);
        }

        public string getAbbreiviation(string id)
        {
            string abb = null;
            switch (id)
            {
                case "Samoa Standard Time":
                    abb = "SST";
                    break;

                case "Central Standard Time":
                    abb = "CST";
                    break;

                case "Central Standard Time (Mexico)":
                    abb = "CST";
                    break;

                case "Hawii Standard Time":
                    abb = "	HAST";
                    break;

                case "Alaskan standard Time":
                    abb = "AKST";
                    break;

                case "Pacific Standard Time (Mexico)":
                    abb = "PST";
                    break;

                case "Pacific Standard Time":
                    abb = "PST";
                    break;

                case "SA Pacific Standard Time":
                    abb = "PST";
                    break;

                case "US Mountain Standard Time":
                    abb = "MST";
                    break;

                case "Mountain Standard Time (Mexico)":
                    abb = "MST";
                    break;

                case "Mountain Standard Time":
                    abb = "MST";
                    break;

                case "Eastern Standard Time":
                    abb = "EST";
                    break;

                case "US Eastern Standard Time":
                    abb = "EST";
                    break;

                case "Chamorro Standard Time":
                    abb = "CHST";
                    break;

                case "India Standard Time":
                    abb = "IST";
                    break;
                default:
                    abb = "CST";
                    return abb;
            }
            return abb;
        }
        public ActionResult GetUpcomingAppintmentDocFilter(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            DateTime enddate = new DateTime();
            enddate = GetLocalTime(DateTime.UtcNow.AddDays(2), User.Identity.Name);
            DateTime starttime = new DateTime();
            starttime = GetLocalTime(DateTime.UtcNow, User.Identity.Name);
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            //var data = apuow.Context.UserInformations
            //   .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.PATIENTID,
            //   (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.DOCTORID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= starttime && f.APPOINTMENTSTARTTIME <= enddate && (f.STATUS == "VIDEO APPT CNFRD" || f.STATUS == "VIDEO APPT RQSTD" || f.STATUS == "VIDEO APPT CNCLD")).OrderBy(f => f.APPOINTMENTSTARTTIME).Take(5);
            var data = apuow.Context.UserInformations
                          .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.PATIENTID,
                          (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.DOCTORID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= DateTime.UtcNow && (f.STATUS == "VIDEO APPT CNFRD" || f.STATUS == "CLINIC APPT CNFRD")).OrderBy(f => f.APPOINTMENTSTARTTIME).Take(5);


            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Welcomepage()
        {
            return View();
        }

        public ActionResult GetUpcomingAppintmentDoc(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.PATIENTID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.DOCTORID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= DateTime.UtcNow && (f.STATUS == "VIDEO APPT CNFRD" || f.STATUS == "CLINIC APPT CNFRD")).OrderBy(f => f.APPOINTMENTSTARTTIME);
            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        public ActionResult GetUpcomingAppintmentDocRequest(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.PATIENTID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.DOCTORID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= DateTime.UtcNow && (f.STATUS == "VIDEO APPT RQSTD" || f.STATUS == "VIDEO APPT CNCLD" || f.STATUS == "CLINIC APPT RQSTD" || f.STATUS == "CLINIC APPT RQSTD" || f.STATUS == "CLINIC APPT CNCLD")).OrderBy(f => f.APPOINTMENTSTARTTIME);
            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        public ActionResult GetUpcomingResentAppintment(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            DateTime currentdate = DateTime.UtcNow;
            DateTime comingdate = DateTime.UtcNow.AddDays(3);
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.DOCTORID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.PATIENTID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= currentdate && f.APPOINTMENTSTARTTIME <= comingdate && f.STATUS == "VIDEO APPT CNFRD").OrderByDescending(f => f.APPOINTMENTID);
            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }


        public ActionResult GetUpcomingResentAppintmentDash(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            DateTime currentdate = DateTime.UtcNow;
            DateTime comingdate = DateTime.UtcNow.AddDays(3);
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.DOCTORID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.PATIENTID == User.Identity.Name && f.APPOINTMENTSTARTTIME >= currentdate && f.APPOINTMENTSTARTTIME <= comingdate && (f.STATUS == "VIDEO APPT CNFRD" || f.STATUS == "CLINIC APPT CNFRD")).OrderByDescending(f => f.APPOINTMENTID).Take(5);
            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {

                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        public ActionResult GetPastAppintment(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.DOCTORID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.PATIENTID == User.Identity.Name && f.APPOINTMENTSTARTTIME < DateTime.UtcNow).OrderByDescending(f => f.APPOINTMENTSTARTTIME);

            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items; result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        public ActionResult GetPastAppintmentMain(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.DOCTORID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.PATIENTID == User.Identity.Name && f.APPOINTMENTSTARTTIME < DateTime.UtcNow).OrderByDescending(f => f.APPOINTMENTSTARTTIME);

            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {
                bool t = apuow.Context.AppointmentRatings.Any(f => f.APPOINTMENTID == temp.APPOINTMENTID);
                if (!t)
                {
                    DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                    var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                    AppointmentDisplay value = new AppointmentDisplay();
                    value.APPOINTMENTID = temp.APPOINTMENTID;
                    value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                    value.REASONFORVISIT = temp.REASONFORVISIT;
                    value.STATUS = temp.STATUS;
                    value.CurrenttimeZone = getAbbreiviation(tempvalue);
                    value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                    value.PATIENTID = temp.PATIENTID;
                    value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                    items.Add(value);
                }

            }
            result.Data = items; result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }
        public ActionResult GetPastAppintmentDoc(DOCTORSLOT doctorSlot)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            JsonResult result = new JsonResult();
            apuow.Context.Configuration.ProxyCreationEnabled = false;
            var data = apuow.Context.UserInformations
               .Join(apuow.Context.APPOINTMENTS, ui => ui.UserName, ap => ap.PATIENTID,
               (ui, ap) => new { ui.FIRSTNAME, ui.Salutation, ui.LASTNAME, ap.DOCTORID, ap.APPOINTMENTSTARTTIME, ap.REASONFORVISIT, ap.PAYMENTSTATUSID, ap.PATIENTID, ap.STATUS, ap.APPOINTMENTID }).Where(f => f.DOCTORID == User.Identity.Name && f.APPOINTMENTSTARTTIME < DateTime.UtcNow || (f.STATUS == "PATIENT VIDEO CNCLD")).OrderByDescending(f => f.APPOINTMENTSTARTTIME);
            List<AppointmentDisplay> items = new List<AppointmentDisplay>();
            foreach (var temp in data)
            {
                DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                AppointmentDisplay value = new AppointmentDisplay();
                value.APPOINTMENTID = temp.APPOINTMENTID;
                value.APPOINTMENTSTARTTIME = GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                value.REASONFORVISIT = temp.REASONFORVISIT;
                value.STATUS = temp.STATUS;
                value.CurrenttimeZone = getAbbreiviation(tempvalue);
                value.PAYMENTSTATUSID = temp.PAYMENTSTATUSID;
                value.PATIENTID = temp.PATIENTID;
                value.FIRSTNAME = temp.Salutation + " " + temp.FIRSTNAME + " " + temp.LASTNAME;
                items.Add(value);

            }
            result.Data = items; result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }
        [HttpGet]
        public ActionResult CancelAppointment(int id)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            var temp = auok.Context.APPOINTMENTS.SingleOrDefault(f => f.APPOINTMENTID == id);
            var tempDoctor = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID);
            BookingAppointmentModel model = new BookingAppointmentModel();
            model.APPOINTMENTSTARTTIME = temp.APPOINTMENTSTARTTIME;
            model.DOCTORSLOTID = temp.DOCTORSLOTID;
            model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt} {1}", GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID), getAbbreiviation(tempDoctor.CurrenttimeZone));
            model.FIRSTNAME = tempDoctor.FIRSTNAME;
            model.LASTNAME = tempDoctor.LASTNAME;
            model.DOCTORID = temp.DOCTORID;
            model.REASONFORVISIT = temp.REASONFORVISIT;
            model.APPOINTMENTID = temp.APPOINTMENTID;
            string temps = null;
            var userspecialyid = duow.Context.Doctorspecialities.Where(w => w.UserName == temp.DOCTORID).Select(s => new { s.SPECIALITY });
            if (userspecialyid != null)
            {
                foreach (var spec in userspecialyid)
                {
                    if (temps == null)
                    {
                        temps = spec.SPECIALITY.ToString();
                    }
                    else
                    {
                        temps = temps + "," + spec.SPECIALITY.ToString();
                    }
                }
                if (temps != null)
                {
                    model.Speciality = temps;
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult CancelAppointment(BookingAppointmentModel model)
        {
            
            string subject = null;
            string appointmentplace = null;
            var temp = auok.Context.APPOINTMENTS.SingleOrDefault(f => f.APPOINTMENTID == model.APPOINTMENTID);
            using (var repo = new AppointmentServiceRepository(auok))
            {
                if (temp.STATUS == "VIDEO APPT CNFRD")
                {
                    temp.STATUS = "PATIENT VIDEO CNCLD";
                    subject = "You have an important message from doccare online regarding your Online Appointment";
                    appointmentplace = "online";
                }else if (temp.STATUS == "CLINIC APPT CNFRD")
                {
                    temp.STATUS = "PATIENT CLINIC CNCLD";
                    subject = "You have an important message from doccare online regarding your Clinic Appointment";
                    appointmentplace = "clinic";
                }

                temp.MODIFIEDBY = User.Identity.Name;
                temp.MODIFIEDON = DateTime.UtcNow;
                temp.State = DOCVIDEO.ObjectState.State.Modified;
                repo.InsertOrUpdate(temp);
                auok.Save();
            }
            var tempvalue = duow.Context.UserInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
            var tempPateint = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            var tempDoctor = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == temp.DOCTORID);
          /*  StringBuilder bodyMsg = new StringBuilder();
            bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
            bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
            bodyMsg.Append("<div>");
            bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<div style='padding:10px;'>");
            bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
            bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear Dr." + tempDoctor.FIRSTNAME + " " + tempDoctor.LASTNAME + "");
            bodyMsg.Append("</h2>");
            bodyMsg.Append("<div style='float:left;width:490px;'> ");
            bodyMsg.Append("<div>We regret to inform, your patient has cancelled the "+appointmentplace+" appointment ");
            bodyMsg.Append("for Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME, User.Identity.Name)) + " " + getAbbreiviation(tempvalue) + " due to unforeseen reasons..");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<p>Please ");
            bodyMsg.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn 'title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");
            bodyMsg.Append("</a>  to login to your doccare account to rebook your ");
            bodyMsg.Append("Appointment or find another doctor");
            bodyMsg.Append("</p>");
            bodyMsg.Append("<div style='color:#1072B5;margin:10px0px-10px0px;'>");
            bodyMsg.Append("<strong>Benefits of signing up");
            bodyMsg.Append("</strong>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<div style='font-style:italic;font-weight:bold;line-height:20px;'>");
            bodyMsg.Append("<ul>");
            bodyMsg.Append("<li>Effective way to provide quality healthcare to patients");
            bodyMsg.Append("</li>");
            bodyMsg.Append("<li>Market physicians’services online");
            bodyMsg.Append("</li>");
            bodyMsg.Append("<li>Access to new local and foreign patients.");
            bodyMsg.Append("</li>");
            bodyMsg.Append("<li>Payment at time of service");
            bodyMsg.Append("</li>");
            bodyMsg.Append("<li>No insurance paper work");
            bodyMsg.Append("</li>");
            bodyMsg.Append("<li>Flexibility for scheduling :online or in office");
            bodyMsg.Append("</li>");
            bodyMsg.Append("<li>Minimal support staff");
            bodyMsg.Append("</li>");
            bodyMsg.Append("</ul>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>");
            bodyMsg.Append("<strong>See your patient , anytime, anywhere!");
            bodyMsg.Append("</strong>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<p>If you have questions about doccare online, please email us at");
            bodyMsg.Append("<a href='#' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
            bodyMsg.Append("</a> For FAQ, please visit ");
            bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'> www.PROFESSORSONLINE.com/faq.");
            bodyMsg.Append("</a>");
            bodyMsg.Append("</p>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<div style='float:right; margin-left:10px;width:230px;'>");
            bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
            bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>doccare online helps doctors and patients connect securely anytime, anywhere!");
            bodyMsg.Append("</div>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<div style='clear:both;'>");
            bodyMsg.Append("</div> ");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<div style='font-size:12px;background:#D3D0D0;padding: 10px;clear:both;margin-top: 10px;'>");
            bodyMsg.Append("<strong>Disclaimer:");
            bodyMsg.Append("</strong>");
            bodyMsg.Append("This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
            bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
            bodyMsg.Append("</strong>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("</div>");

            DOCVIDEO.Utility.MailUtility.SendEmail("anupranjan.1983@gmail.com", temp.DOCTORID, "", "", subject, bodyMsg.ToString(), false, "", 0);
            */
            string comment = "Cancel Appointment.";
            string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
            //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.CancelAppointment, Convert.ToInt32(model.APPOINTMENTID), 0, User.Identity.Name, data);
            using (MessageRepository repo = new MessageRepository(muw))
            {
                MESSAGE message = new MESSAGE();

                message.MESSAGESUBJECT = "Appointment  cancelled";
                message.MESSAGEBODY = "Appointment  Booking cancelled";
                message.DateCreated = DateTime.UtcNow;
                message.SENDMESSAGETO = model.DOCTORID;
                message.SENDMESSAGEFROM = User.Identity.Name;
                message.SENDBY = User.Identity.Name;
                message.MESSAGESTATUSFROM = "ACTIVE";
                message.MESSAGESTATUSTO = "ACTIVE";
                message.MESSAGETYPE = "CANCELLED_APPOINTMENT";
                message.MESSAGESTATUS = "UnRead";
                message.MODIFIEDON = DateTime.UtcNow;
                message.SENDDATE = DateTime.UtcNow;
                message.State = DOCVIDEO.ObjectState.State.Added;
                repo.InsertOrUpdate(message);
                muw.Save();

            }
            return Json(new { msg = "Appointment Cancelled" }, JsonRequestBehavior.AllowGet);
        }

        public void PreferedDoctorUpdate(DoctorsInformationEditModel model)
        {
            if (model.ProviderId != null)
            {
                PrefferedDoctorUnitOfWork puow = new PrefferedDoctorUnitOfWork();
                PrefferdDoctorServiceRepository repo1 = new PrefferdDoctorServiceRepository(puow);

                PREFERREDDOCTOR prefered = new PREFERREDDOCTOR();
                prefered.CREATEDBY = User.Identity.Name;
                prefered.MODIFIEDON = DateTime.UtcNow;
                prefered.DateCreated = DateTime.UtcNow;
                prefered.MODIFIEDBY = User.Identity.Name;
                prefered.STATUS = "A";
                prefered.PREFERREDDATE = DateTime.UtcNow;
                prefered.DOCTORID = model.ProviderId;
                prefered.UserName = User.Identity.Name;
                prefered.State = DOCVIDEO.ObjectState.State.Added;
                repo1.InsertOrUpdate(prefered);
                puow.Save();
            }
            if (model.RemoveProviderId != null)
            {
                PrefferedDoctorUnitOfWork puow = new PrefferedDoctorUnitOfWork();
                PrefferdDoctorServiceRepository repo1 = new PrefferdDoctorServiceRepository(puow);
                var temp = puow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name && f.DOCTORID == model.RemoveProviderId);
                if (temp != null)
                {
                    foreach (var tempdatavalue in temp)
                    {
                        PrefferedDoctorUnitOfWork puow1 = new PrefferedDoctorUnitOfWork();
                        PrefferdDoctorServiceRepository repo2 = new PrefferdDoctorServiceRepository(puow1);
                        tempdatavalue.State = State.Deleted;
                        repo2.Delete(tempdatavalue.PREFERREDDOCTORID);
                        puow1.Save();
                    }
                }
            }
        }



        [HttpGet]
        [Authorize]
        public ActionResult DoctorsSearch()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            string[] prefereddoctorslist = null;
            var data = pduow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name).Select(s => new { s.DOCTORID });
            string tempdata = null;
            foreach (var temp in data)
            {
                if (tempdata == null)
                {
                    tempdata = temp.DOCTORID;
                }
                else
                {
                    tempdata = tempdata + "," + temp.DOCTORID;
                }

            }

            if (tempdata != null)
            {
                prefereddoctorslist = tempdata.Split(',');
            }
            else
            {
                prefereddoctorslist = null;
            }

            ViewBag.PreferedDoctors = prefereddoctorslist;
            _advancequicksearch();
            DoctorsInformationEditModel model = new DoctorsInformationEditModel();
            if (Convert.ToString(Session["GENDER"]) != "")
            {
                model.GENDER = Convert.ToString(Session["GENDER"]);
            }
            if (Convert.ToString(Session["ZIPCODE"]) != "")
            {
                model.ZIPCODE = Convert.ToString(Session["ZIPCODE"]);
            }
            if (Convert.ToString(Session["CITY"]) != "")
            {
                model.CITY = Convert.ToString(Session["CITY"]);
            }
            if (Convert.ToString(Session["USERSTATE"]) != "")
            {
                model.USERSTATE = Convert.ToString(Session["USERSTATE"]);
            }
            if (Convert.ToString(Session["Speciality"]) != "")
            {
                model.Speciality = Convert.ToString(Session["Speciality"]);
            }
            if (Convert.ToString(Session["Speciality"]) != "")
            {
                model.Language = Convert.ToString(Session["Language"]);
            }
            _DoctorsSearch();

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public ActionResult DoctorsSearch(DoctorsInformationEditModel model)
        {


            return View(model);
        }

        [HttpGet]
        [Authorize]
        public DoctorsInformationEditModel _DoctorsSearch()
        {

            DoctorsInformationEditModel model = new DoctorsInformationEditModel();
            return model;
        }

        [HttpGet]
        [Authorize]
        public ActionResult _pateintinfodisplay()
        {

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult _pateintconctdisplay()
        {

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult _pateintinfoedit()
        {

            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult _pateintinfoedit(USERSINFORMATION usersinformation)
        {
            DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();
            using (var repo = new PatientInformationRepository(uow))
            {

                var temp = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
                temp.MODIFIEDON = DateTime.UtcNow;
                if (usersinformation.FIRSTNAME != null && usersinformation.LASTNAME != null && usersinformation.GENDER != null)
                {
                    if (usersinformation.FIRSTNAME.Length > 0 && usersinformation.LASTNAME.Length > 0 && usersinformation.GENDER.Length > 0)
                    {
                        if (ModelState.IsValid)
                        {
                            DateTime tempdate = new DateTime();
                            if (usersinformation.DOBYEAR > 0 && usersinformation.DOBMonth != null && usersinformation.DOBDAY > 0)
                            {
                                tempdate = new DateTime(Convert.ToInt32(usersinformation.DOBYEAR), Convert.ToInt32(usersinformation.DOBMonth), Convert.ToInt32(usersinformation.DOBDAY));
                            }
                            if (usersinformation.Salutation != null)
                                temp.Salutation = usersinformation.Salutation;
                            temp.FIRSTNAME = usersinformation.FIRSTNAME;
                            temp.LASTNAME = usersinformation.LASTNAME;
                            temp.GENDER = usersinformation.GENDER;
                            temp.PASSWORDHASH = temp.PASSWORDHASH;
                            temp.PASSWORDSALT = temp.PASSWORDSALT;
                            if (UserProfilepic != null)
                                temp.USERPHOTOFILEPATH = UserProfilepic;
                            usersinformation.DOB = tempdate;
                            temp.DOB = usersinformation.DOB;
                            temp.DateLastActivity = DateTime.UtcNow;
                            temp.DateLastLogin = DateTime.UtcNow;
                            temp.DateLastPasswordChange = DateTime.UtcNow;
                            temp.IsApproved = true;
                            temp.MODIFIEDBY = User.Identity.Name;
                            temp.MODIFIEDON = DateTime.UtcNow;
                            temp.DOBMonth = "";
                            temp.DOBDAY = 1;
                            temp.DOBYEAR = 1;
                            if (usersinformation.USERPHOTOFILEPATH != null)
                            {
                                temp.USERPHOTOFILEPATH = usersinformation.USERPHOTOFILEPATH;
                                usersinformation.USERPHOTOFILEPATH = temp.USERPHOTOFILEPATH;
                            }
                            else
                            {
                                usersinformation.USERPHOTOFILEPATH = temp.USERPHOTOFILEPATH;
                            }
                            repo.InsertOrUpdate(temp);
                            uow.Save();
                        }
                    }
                    else
                    {
                        if (usersinformation.FIRSTNAME.Length == 0)
                        {
                            Session["Message"] = "Error :First Name is Required";
                            return Json(new { msg = "Error :First Name is Required" }, JsonRequestBehavior.AllowGet);
                        }
                        if (usersinformation.LASTNAME.Length == 0)
                        {
                            Session["Message"] = "Error :Last Name is Required";
                            return Json(new { msg = "Error :Last Name is Required" }, JsonRequestBehavior.AllowGet);

                        }
                        if (usersinformation.GENDER.Length == 0)
                        {
                            Session["Message"] = "Error :Gender is Required";
                            return Json(new { msg = "Error :Gender is Required" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    if (usersinformation.FIRSTNAME.Length == 0)
                    {
                        Session["Message"] = "Error :First Name is Required";
                        return Json(new { msg = "Error :First Name is Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (usersinformation.LASTNAME.Length == 0)
                    {
                        Session["Message"] = "Error :Last Name is Required";
                        return Json(new { msg = "Error :Last Name is Required" }, JsonRequestBehavior.AllowGet);

                    }
                    if (usersinformation.GENDER.Length == 0)
                    {
                        Session["Message"] = "Error :Gender is Required";
                        return Json(new { msg = "Error :Gender is Required" }, JsonRequestBehavior.AllowGet);
                    }

                }

            }
            usersinformation.messageCount = muok.Context.Messages.Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUS == "UnRead" && f.MESSAGESTATUSFROM == "ACTIVE").Count().ToString();
            Session["Message"] = "Basic Information  updated";
            return Json(new { msg = "Basic Information  updated" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult _pateintconctedit(USERSINFORMATION usersinformation)
        {
            DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();
            if (usersinformation.USERSTREETADDRESS1 != null && usersinformation.CITY != null && (usersinformation.USERSTATE != null || usersinformation.COUNTRY != null) && usersinformation.ZIPCODE != null)
            {
                if (usersinformation.USERSTREETADDRESS1 != null && usersinformation.CITY != null && (usersinformation.USERSTATE != null || usersinformation.COUNTRY != null) && usersinformation.ZIPCODE != null)
                {
                    using (var repo = new PatientInformationRepository(uow))
                    {
                        var temp = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
                        temp.MODIFIEDON = DateTime.UtcNow;
                        temp.USERSTREETADDRESS1 = usersinformation.USERSTREETADDRESS1;
                        temp.USERSTREETADDRESS2 = usersinformation.USERSTREETADDRESS2;
                        temp.CITY = usersinformation.CITY;
                        if (usersinformation.COUNTRY == "United States")
                        {
                            temp.USERSTATE = usersinformation.USERSTATE;
                        }
                        else
                        {
                            temp.USERSTATE = null;
                        }

                        temp.COUNTRY = usersinformation.COUNTRY;
                        temp.ZIPCODE = usersinformation.ZIPCODE;

                        if (usersinformation.CurrenttimeZone != null)
                            temp.CurrenttimeZone = usersinformation.CurrenttimeZone;
                        temp.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(temp);
                        uow.Save();



                    }
                }
                else
                {
                    if (usersinformation.USERSTREETADDRESS1.Length == 0)
                    {
                        Session["Message"] = "Error :Street Address Required";
                        return Json(new { msg = "Error :Street Address Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (usersinformation.CITY.Length == 0)
                    {
                        Session["Message"] = "Error :City Required";
                        return Json(new { msg = "Error :City Required" }, JsonRequestBehavior.AllowGet);
                    } if (usersinformation.COUNTRY.Length == 0)
                    {
                        Session["Message"] = "Error :Country Required";
                        return Json(new { msg = "Error :Country Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (usersinformation.ZIPCODE.Length == 0)
                    {
                        Session["Message"] = "Error :Zipcode Required";
                        return Json(new { msg = "Error :Zipcode Required" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            else
            {
                if (usersinformation.USERSTREETADDRESS1 == null)
                {
                    Session["Message"] = "Error :Street Address Required";
                    return Json(new { msg = "Error :Street Address Required" }, JsonRequestBehavior.AllowGet);
                }
                if (usersinformation.CITY == null)
                {
                    Session["Message"] = "Error :City Required";
                    return Json(new { msg = "Error :City Required" }, JsonRequestBehavior.AllowGet);
                } if (usersinformation.COUNTRY == null)
                {
                    Session["Message"] = "Error : Country Required";
                    return Json(new { msg = "Error :Country Required" }, JsonRequestBehavior.AllowGet);
                }
                if (usersinformation.ZIPCODE == null)
                {
                    Session["Message"] = "Error :Zipcode Required";
                    return Json(new { msg = "Error :Zipcode Required" }, JsonRequestBehavior.AllowGet);
                }

            }

            Session["Message"] = "Basic Information  updated";
            return Json(new { msg = "Basic Information  updated" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult BasicInformation(USERSINFORMATION usersinformation)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }

            using (var repo = new PatientInformationRepository(uow))
            {
                var temp = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
                temp.MODIFIEDON = DateTime.UtcNow;
                if (usersinformation.DOBYEAR.ToString() != null && usersinformation.DOBMonth.ToString() != null && usersinformation.DOBDAY.ToString() != null && usersinformation.FIRSTNAME != null && usersinformation.LASTNAME != null && usersinformation.GENDER != null || usersinformation.DOBYEAR != 0 || usersinformation.DOBMonth != null || usersinformation.DOBDAY != 0)
                {
                    if (ModelState.IsValid)
                    {
                        DateTime tempdate = new DateTime(Convert.ToInt32(usersinformation.DOBYEAR), Convert.ToInt32(usersinformation.DOBMonth), Convert.ToInt32(usersinformation.DOBDAY));

                        temp.FIRSTNAME = usersinformation.FIRSTNAME;
                        temp.LASTNAME = usersinformation.LASTNAME;
                        temp.GENDER = usersinformation.GENDER;
                        temp.PASSWORDHASH = temp.PASSWORDHASH;
                        temp.PASSWORDSALT = temp.PASSWORDSALT;
                        usersinformation.DOB = tempdate;
                        temp.DOB = usersinformation.DOB;
                        temp.DateLastActivity = DateTime.UtcNow;
                        temp.DateLastLogin = DateTime.UtcNow;
                        temp.DateLastPasswordChange = DateTime.UtcNow;
                        temp.IsApproved = true;
                        temp.MODIFIEDBY = User.Identity.Name;
                        temp.MODIFIEDON = DateTime.UtcNow;
                        temp.DOBMonth = "";
                        temp.DOBDAY = 1;
                        temp.DOBYEAR = 1;
                        if (usersinformation.USERPHOTOFILEPATH != null)
                        {
                            temp.USERPHOTOFILEPATH = usersinformation.USERPHOTOFILEPATH;
                            usersinformation.USERPHOTOFILEPATH = temp.USERPHOTOFILEPATH;
                        }
                        else
                        {
                            usersinformation.USERPHOTOFILEPATH = temp.USERPHOTOFILEPATH;
                        }
                        repo.InsertOrUpdate(temp);
                        uow.Save();
                        ModelState.AddModelError("", "Basic Information  updated");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error :All Fields Required");
                }

            }

            return View(usersinformation);
        }


        [Authorize]
        public ActionResult ContactInformationupdate()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            //double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.Now)) ? 1 : 0);
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            string id = User.Identity.Name;
            USERSINFORMATION usersinformation = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == id);
            _quicksearch();
            if (usersinformation == null)
            {
                return HttpNotFound();
            }
            return View(usersinformation);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ContactInformationupdate(USERSINFORMATION usersinformation)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            } ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            if (usersinformation.USERSTREETADDRESS1 != null && usersinformation.USERSTREETADDRESS2 != null && usersinformation.CITY != null && (usersinformation.USERSTATE != null || usersinformation.COUNTRY != null) && usersinformation.ZIPCODE != null)
            {
                using (var repo = new PatientInformationRepository(uow))
                {
                    var temp = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
                    temp.MODIFIEDON = DateTime.UtcNow;
                    temp.USERSTREETADDRESS1 = usersinformation.USERSTREETADDRESS1;
                    temp.USERSTREETADDRESS2 = usersinformation.USERSTREETADDRESS2;
                    temp.CITY = usersinformation.CITY;
                    temp.USERSTATE = usersinformation.USERSTATE;
                    temp.COUNTRY = usersinformation.COUNTRY;
                    temp.ZIPCODE = usersinformation.ZIPCODE;
                    temp.USERSTATE = usersinformation.USERSTATE;
                    if (usersinformation.CurrenttimeZone != null)
                        temp.CurrenttimeZone = usersinformation.CurrenttimeZone;
                    temp.State = DOCVIDEO.ObjectState.State.Modified;
                    repo.InsertOrUpdate(temp);
                    uow.Save();

                    ModelState.AddModelError("", "Contact Information updated");

                }
            }
            else
            {
                ModelState.AddModelError("", "Error : All fields Required .");
            }


            return View(usersinformation);
        }


        [Authorize]
        public ActionResult ContactInformation()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            //double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.Now)) ? 1 : 0);
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            string id = User.Identity.Name;
            USERSINFORMATION usersinformation = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == id);
            _quicksearch();
            if (usersinformation == null)
            {
                return HttpNotFound();
            }
            usersinformation.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return View(usersinformation);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ContactInformation(USERSINFORMATION usersinformation)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            if ((usersinformation.USERSTREETADDRESS1 != null) && usersinformation.CITY != null && (usersinformation.USERSTATE != null || usersinformation.COUNTRY != null) && usersinformation.ZIPCODE != null)
            {
                if ((usersinformation.USERSTREETADDRESS1.Length > 0) && usersinformation.CITY.Length > 0 && (usersinformation.COUNTRY.Length > 0) && usersinformation.ZIPCODE.Length > 0)
                {
                    using (var repo = new PatientInformationRepository(uow))
                    {
                        var temp = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
                        temp.MODIFIEDON = DateTime.UtcNow;
                        temp.USERSTREETADDRESS1 = usersinformation.USERSTREETADDRESS1;
                        temp.USERSTREETADDRESS2 = usersinformation.USERSTREETADDRESS2;
                        temp.CITY = usersinformation.CITY;
                        if (usersinformation.COUNTRY != null && usersinformation.COUNTRY == "United States")
                        {
                            temp.USERSTATE = usersinformation.USERSTATE;
                        }
                        temp.COUNTRY = usersinformation.COUNTRY;
                        temp.ZIPCODE = usersinformation.ZIPCODE;
                        temp.USERSTATE = usersinformation.USERSTATE;
                        temp.IsfirstLogin = "false";
                        if (usersinformation.CurrenttimeZone != null)
                            temp.CurrenttimeZone = usersinformation.CurrenttimeZone;
                        temp.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(temp);
                        uow.Save();

                        ModelState.AddModelError("", "Contact Information updated");

                    }
                }
                else
                {
                    if ((usersinformation.USERSTREETADDRESS1.Length == 0))
                    {
                        ModelState.AddModelError("", "Error : Street address are required .");
                    }
                    if (usersinformation.CITY.Length == 0)
                    {
                        ModelState.AddModelError("", "Error : City is required .");
                    }
                    if ((usersinformation.USERSTATE.Length == 0 || usersinformation.COUNTRY.Length == 0))
                    {
                        ModelState.AddModelError("", "Error : Either Country or State is required .");
                    }
                    if (usersinformation.ZIPCODE.Length == 0)
                    {
                        ModelState.AddModelError("", "Error : Zipcode is Required .");
                    }
                    return View(usersinformation);
                }
            }
            else
            {
                if ((usersinformation.USERSTREETADDRESS1 == null || usersinformation.USERSTREETADDRESS2 == null))
                {
                    ModelState.AddModelError("", "Error : Street address are required .");
                }
                if (usersinformation.CITY == null)
                {
                    ModelState.AddModelError("", "Error : City is required .");
                }
                if ((usersinformation.USERSTATE != null || usersinformation.COUNTRY != null))
                {
                    ModelState.AddModelError("", "Error : Either Country or State is required .");
                }
                if (usersinformation.ZIPCODE != null)
                {
                    ModelState.AddModelError("", "Error : Zipcode is Required .");
                }
                return View(usersinformation);
            }


            return RedirectToAction("Dashboard");
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> UserImageUpload)
        {
            Users = User.Identity.Name;
            StringBuilder fileNames = new StringBuilder();
            try
            {
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(User.Identity.Name));

                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(User.Identity.Name));
                }
                //  The Name of the Upload component is "files"
                if (UserImageUpload != null)
                {
                    foreach (var file in UserImageUpload)
                    {
                        //    Some browsers send file names with full path.
                        //    We are only interested in the file name.
                        var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        var fileNamewithoutExt = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                        var fileName = Path.GetFileName(fileNamewithoutExt + "_" + DateTime.UtcNow.ToString("MMddyyyyhhmmss") + ext);


                        var physicalPath = Path.Combine(Server.MapPath("/PatientProfile/" + User.Identity.Name), fileName);
                        fileNames.Append("/PatientProfile/" + User.Identity.Name + "/" + fileName);
                        file.SaveAs(physicalPath);



                        if (Usertype() != "P")
                        {
                            return RedirectToAction("LogOn", "Account");
                        }
                        var userdata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
                        UserProfilepic = fileNames.ToString();

                        //if (temppicture != userdata.USERPHOTOFILEPATH && temppicture != "")
                        //{
                        //    userdata.USERPHOTOFILEPATH = temppicture;                           
                        //    using (PatientInformationRepository repo = new PatientInformationRepository(uow))
                        //    {
                        //        userdata.State = State.Modified;
                        //        repo.InsertOrUpdateGraph(userdata);
                        //        uow.Save();                                
                        //    }
                        //}                       

                    }
                }
            }
            catch (Exception ex)
            {
                // throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }



            return Json(new { status = fileNames.ToString(), type = "save" }, "text/plain");

        }

        //[HttpPost]
        //[Authorize]
        //public ActionResult ProfilePicure(USERSINFORMATION user)
        //{
        //    if (Usertype() != "P")
        //    {
        //        return RedirectToAction("LogOn", "Account");
        //    }
        //    var userdata = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
        //    var temppicture = Convert.ToString(Session["ImagePath"]);
        //    if (temppicture != userdata.USERPHOTOFILEPATH && temppicture!="")
        //    {
        //        userdata.USERPHOTOFILEPATH = temppicture;
        //        Session["ImagePath"] = userdata.USERPHOTOFILEPATH;
        //    using (PatientInformationRepository repo = new PatientInformationRepository(uow))
        //    {
        //        userdata.State = State.Modified;
        //        repo.InsertOrUpdateGraph(userdata);
        //        uow.Save();
        //            ModelState.AddModelError("", "Profile picture updated");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Error : No profile picture to update");               
        //    }
        //    return View(userdata);
        //}
        [HttpGet]
        [Authorize]
        public ActionResult QuickSearch()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            string[] prefereddoctorslist = null;
            var data = pduow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name).Select(s => new { s.DOCTORID });
            string tempdata = null;
            foreach (var temp in data)
            {
                if (tempdata == null)
                {
                    tempdata = temp.DOCTORID;
                }
                else
                {
                    tempdata = tempdata + "," + temp.DOCTORID;
                }

            }

            if (tempdata != null)
            {
                prefereddoctorslist = tempdata.Split(',');
            }
            else
            {
                prefereddoctorslist = null;
            }

            ViewBag.PreferedDoctors = prefereddoctorslist;
            _quicksearch();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdvancedQuickSearch()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            string[] prefereddoctorslist = null;
            var data = pduow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name).Select(s => new { s.DOCTORID });
            string tempdata = null;
            foreach (var temp in data)
            {
                if (tempdata == null)
                {
                    tempdata = temp.DOCTORID;
                }
                else
                {
                    tempdata = tempdata + "," + temp.DOCTORID;
                }

            }

            if (tempdata != null)
            {
                prefereddoctorslist = tempdata.Split(',');
            }
            else
            {
                prefereddoctorslist = null;
            }

            ViewBag.PreferedDoctors = prefereddoctorslist;
            _advancequicksearch();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult AppointmentQuickSearch()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            string[] prefereddoctorslist = null;
            var data = pduow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name).Select(s => new { s.DOCTORID });
            string tempdata = null;
            foreach (var temp in data)
            {
                if (tempdata == null)
                {
                    tempdata = temp.DOCTORID;
                }
                else
                {
                    tempdata = tempdata + "," + temp.DOCTORID;
                }

            }

            if (tempdata != null)
            {
                prefereddoctorslist = tempdata.Split(',');
            }
            else
            {
                prefereddoctorslist = null;
            }

            ViewBag.PreferedDoctors = prefereddoctorslist;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult AppointmentQuickSearch(APPOINTMENT model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult QuickSearchOne(USERSINFORMATION model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            Speciality = model.ProviderType;
            GENDER = model.SearchGender;
            ZIPCODE = model.SEARCHZIPCODE;
            return Json(new { status = "Success", type = "save" }, "text/plain");


        }



        [HttpGet]
        [Authorize]
        public ActionResult Index(string id, string MessageBody)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }

            Session["SendTo"] = id;
            Session["MessageBody"] = MessageBody;

            return RedirectToAction("Index", "Message");
        }

        public ActionResult GetQuickProviders()
        {
            DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;
            if (GENDER == null && ZIPCODE == null && Speciality == null)
            {
                var data = uow.Context.UserInformations.Where(f => f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList(); result.Data = data;
            }
            else if (GENDER != null && ZIPCODE == null && Speciality == null)
            {
                var data = uow.Context.UserInformations.Where(f => f.GENDER == GENDER && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList(); result.Data = data;
            }
            else if (Speciality != null && ZIPCODE == null && GENDER == null)
            {
                var data = uow.Context.UserInformations
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.FIRSTNAME, ui.IsApproved, ui.USERTYPE, ui.LASTNAME, ui.AverageRating, ui.UserName, ui.GENDER, ui.CITY, ui.USERSTATE, ui.ZIPCODE, ds.SPECIALITY, ui.USERPHOTOFILEPATH }).Where(f => f.SPECIALITY == Speciality && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList(); result.Data = data;
            }
            else if (GENDER == null && ZIPCODE != null && Speciality == null)
            {
                var data = uow.Context.UserInformations.Where(f => f.ZIPCODE == ZIPCODE && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList(); result.Data = data;
            }
            if (ZIPCODE != null && Speciality != null)
            {
                var data = uow.Context.UserInformations
                .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                (ui, ds) => new { ui.FIRSTNAME, ui.IsApproved, ui.USERTYPE, ui.LASTNAME, ui.AverageRating, ui.UserName, ui.GENDER, ui.CITY, ui.USERSTATE, ui.ZIPCODE, ds.SPECIALITY, ui.USERPHOTOFILEPATH }).Where(f => f.ZIPCODE == ZIPCODE
                && f.SPECIALITY == Speciality && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList(); result.Data = data;
            }
            else if (GENDER != null && Speciality != null)
            {

                var data = uow.Context.UserInformations
                .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                (ui, ds) => new { ui.FIRSTNAME, ui.IsApproved, ui.USERTYPE, ui.LASTNAME, ui.UserName, ui.AverageRating, ui.GENDER, ui.CITY, ui.USERSTATE, ui.ZIPCODE, ds.SPECIALITY, ui.USERPHOTOFILEPATH }).Where(f => f.GENDER == GENDER
                && f.SPECIALITY == Speciality && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList(); result.Data = data;
            }

            else if (GENDER != null && ZIPCODE != null)
            {
                var data = uow.Context.UserInformations.Where(f => f.GENDER == GENDER && f.ZIPCODE == ZIPCODE && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList(); result.Data = data;
            }

            if (result.Data == null)
            {
                return Json(new { msg = "No records" }, JsonRequestBehavior.AllowGet);
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;


            return result;
        }

        public ActionResult AllDoctors()
        {
            DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;
            var temp = uow.Context.UserInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            if (temp.USERSTATE != null)
            {
                var data = uow.Context.UserInformations.Where(f => f.USERSTATE == temp.USERSTATE && f.USERTYPE == "D" && f.IsApproved == true).OrderByDescending(f => f.DateCreated).Take(5).ToList();
                result.Data = data;
            }
            else
            {
                var data = uow.Context.UserInformations.Where(f => f.USERTYPE == "D" && f.IsApproved == true).OrderByDescending(f => f.DateCreated).Take(5).ToList();
                result.Data = data;
            }


            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;


            return result;
        }

        public ActionResult Remove(string[] fileNames)
        {
            //  The parameter of the Remove action must be called "fileNames"
            StringBuilder files = new StringBuilder();
            try
            {
                if (fileNames != null)
                {
                    foreach (var fullName in fileNames)
                    {
                        var fileName = Path.GetFileName(fullName);
                        var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                        files.Append(fileName + ",");
                        //  TODO: Verify user permissions

                        if (System.IO.File.Exists(physicalPath))
                        {
                            //     The files are not actually removed in this demo
                            System.IO.File.Delete(physicalPath);
                        }

                        UserProfilepic = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name).USERPHOTOFILEPATH;

                    }
                }
            }
            catch (Exception ex)
            {
                // throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }

            //  Return an empty string to signify success
            return Json(new { status = files.ToString(), type = "remove" }, "text/plain");
        }


        [Authorize]
        public void _changepassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;

        }

        [HttpPost]
        [Authorize]
        public JsonResult _changepassword(USERSINFORMATION model)
        {

            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            try
            {
                ViewBag.PasswordLength = MembershipService.MinPasswordLength;
                if (model.OldPassword != null || model.NewPassword != null || model.ConfirmPassword != null)
                {
                    if (model.ConfirmPassword == model.NewPassword)
                    {
                        if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                        {

                        }
                        else
                        {
                            return Json(new { msg = "Error : Invalid Old Password" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { msg = "Error : New Password and Confirm Password is not matching" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { msg = "Error : Password Fields are Required" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Error : Invalid Operation" }, JsonRequestBehavior.AllowGet);
            }            // If we got this far, something failed, redisplay form

            return Json(new { msg = "Password Changed Successfully" }, JsonRequestBehavior.AllowGet);
        }
        public void _quicksearch()
        {


        }
        public void _advancequicksearch()
        {


        }




        [Authorize]
        public ActionResult ProfilePicure()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            USERSINFORMATION model = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            _quicksearch();
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult PreferredProviders()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            string[] prefereddoctorslist = null;
            var data = pduow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name).Select(s => new { s.DOCTORID });
            string tempdata = null;
            foreach (var temp in data)
            {
                if (tempdata == null)
                {
                    tempdata = temp.DOCTORID;
                }
                else
                {
                    tempdata = tempdata + "," + temp.DOCTORID;
                }

            }

            if (tempdata != null)
            {
                prefereddoctorslist = tempdata.Split(',');
            }
            else
            {
                prefereddoctorslist = null;
            }

            ViewBag.PreferedDoctors = prefereddoctorslist;
            _preferredproviders();
            _advancequicksearch();


            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult _preferredproviders()
        {
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            string[] prefereddoctorslist = null;
            var data = pduow.Context.PrefferedDoctors.Where(f => f.UserName == User.Identity.Name).Select(s => new { s.DOCTORID });
            string tempdata = null;
            foreach (var temp in data)
            {
                if (tempdata == null)
                {
                    tempdata = temp.DOCTORID;
                }
                else
                {
                    tempdata = tempdata + "," + temp.DOCTORID;
                }

            }

            if (tempdata != null)
            {
                prefereddoctorslist = tempdata.Split(',');
            }
            else
            {
                prefereddoctorslist = null;
            }

            ViewBag.PreferedDoctors = prefereddoctorslist;
            return View();
        }
        [Authorize]
        public ActionResult GetPreferredProviders()
        {
            JsonResult result = new JsonResult();
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            var datavalue = pduow.Context.UserInformations
                  .Join(pduow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                      .Join(pduow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ds.SPECIALITY, ui.STATELICENSE })
                    .Join(pduow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS, ui.STATELICENSE })
                .Join(pduow.Context.PrefferedDoctors, ui => ui.UserName, ds => ds.DOCTORID,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.SPECIALITY, ui.INSTITUTIONNAME, ui.CLINICCITY, ui.STREETADDRESS1, ui.STREETADDRESS2, ui.CLINICUSERSTATE, ui.CLINICZIPCODE, ds.UserName, ds.DOCTORID, ui.STATELICENSE }).Where(ds => ds.UserName == User.Identity.Name).Distinct().ToList();

            result.Data = datavalue;

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;


            return result;

        }

        [Authorize]
        public ActionResult GetPreferredProvidersMain()
        {
            JsonResult result = new JsonResult();
            PrefferedDoctorUnitOfWork pduow = new PrefferedDoctorUnitOfWork();
            var datavalue = pduow.Context.UserInformations
                 .Join(pduow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                (ui, di) => new { ui.USERPHOTOFILEPATH, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX })
                    .Join(pduow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ds.SPECIALITY })
                  .Join(pduow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
              .Join(pduow.Context.PrefferedDoctors, ui => ui.UserName, ds => ds.DOCTORID,
              (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.SPECIALITY, ui.INSTITUTIONNAME, ui.CLINICCITY, ui.STREETADDRESS1, ui.STREETADDRESS2, ui.CLINICUSERSTATE, ui.CLINICZIPCODE, ds.UserName, ds.DOCTORID }).Where(ds => ds.UserName == User.Identity.Name).Distinct().Take(5).ToList();


            result.Data = datavalue;

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;


            return result;

        }
        [Authorize]
        public ActionResult PreferredProviders(DoctorsInformationEditModel model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }

            return View();
        }

        public DateTime GetUTCtime(DateTime current)
        {
            var timezone = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == User.Identity.Name).CurrenttimeZone;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);

            DateTime d = current.AddHours(offSetHours);

            return d;

        }

        public DateTime GetLocalTime(DateTime utcTime)
        {


            var timezone = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == User.Identity.Name).CurrenttimeZone;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);

            DateTime d = utcTime.AddHours(offSetHours);

            return d;

        }
        public DateTime GetLocalTime(DateTime? utcTime, double timezoneOffset)
        {


            var timezone = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == User.Identity.Name).CurrenttimeZone;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);

            DateTime d = utcTime.Value.AddHours(offSetHours);
            d.AddHours(timezoneOffset * -1);
            return d;

        }

        public DateTime GetLocalTime(DateTime utcTime, string DOCTORID)
        {


            var timezone = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == DOCTORID).CurrenttimeZone;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);

            DateTime d = utcTime.AddHours(offSetHours);

            return d;

        }

        public DateTime GetLocalTime(DateTime? utcTime, string DOCTORID)
        {


            var timezone = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == DOCTORID).CurrenttimeZone;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);

            DateTime d = utcTime.Value.AddHours(offSetHours);

            return d;

        }

        public DateTime GetLocalTime(DateTime? utcTime, string DOCTORID, double timezoneOffset)
        {


            var timezone = uow.Context.UsersInformations.SingleOrDefault(w => w.UserName == DOCTORID).CurrenttimeZone;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);

            DateTime d = utcTime.Value.AddHours(offSetHours);
            d = d.AddHours((timezoneOffset * -1));
            return d;

        }
    }
}
