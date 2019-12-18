using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DOCVIDEO.ErrorLoggingContext;
using client.Web.Models;
using System.Web.Routing;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;
using DOCVIDEO.UserServiceRepoUOW;
using DOCVIDEO.Utility;
using UserService.Repositories.Disconnected;
using System.Collections.ObjectModel;
using DOCVIDEO.UserServiceRepoUOW.Disconnected;
using DOCVIDEO.UserServiceBoundedContext;
using DOCVIDEO.PatientInformationServiceBoundedContext;
using DOCVIDEO.PatientInformationServiceRepoUOW;
using PatientInformationService.Repositories.Disconnected;
using client.Web.Controllers;
using OpenTok;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using Microsoft.Web.Infrastructure;
using System.Web.Razor;
using System.Web.WebPages.Deployment;
using System.Web.WebPages.Razor;
using System.Threading.Tasks;

//  new addition
using System.Data.Entity;
using System.Web.Security;
using client.Web;
using System.Net;
using System.Collections.Specialized;
//using DOCVIDEO.PayPalWrapper;
//using DOCVIDEO.BOL;
//using DOCVIDEO.DAL;
using PayPal.AdaptivePayments.Model;

namespace client.Web.Controllers
{
    public class DoctorInformationController : Controller
    {
        private readonly DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uws = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();
        private readonly MessageUnitOfWork muw = new MessageUnitOfWork();
        private readonly DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
        private readonly UserLanguageUnitOfWork luow = new UserLanguageUnitOfWork();
        private readonly SpecialityUnitOfWork suow = new SpecialityUnitOfWork();
        private readonly DoctorInformationUnitOfWork diuow = new DoctorInformationUnitOfWork();
        private readonly WorkInstitutionUnitOfWork uw = new WorkInstitutionUnitOfWork();
        AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
        private readonly DOCVIDEO.UserServiceRepoUOW.UnitOfWork duow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
        private readonly AppointmentSlotsUnitOfWork apUOW = new AppointmentSlotsUnitOfWork();
        private readonly AppointmentStatusUnitOfWork asUOW = new AppointmentStatusUnitOfWork();
        private readonly AppointmentsUnitOfWork appointmentUOW = new AppointmentsUnitOfWork();
        private readonly AppointmentDoctorNotesUnitOfWork anUOW = new AppointmentDoctorNotesUnitOfWork();
        private readonly ChatMessageUnitOfWork cmUOW = new ChatMessageUnitOfWork();
        
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        private AccountMembershipService account = new AccountMembershipService();
        public static string Users = null;
        public static string patientId = null;
        public static string doctorId = null;
        public static long appointmentId =0;
        public static string UserProfilePic;
        public static string imagepath;
        public static string LoginFirstName;
        public static string LoginLastName;
        public static string profilepath;
        public static string message;
        public static string currentDate = DateTime.Now.Date.ToString("ddd,MMM dd,yyyy");
        public static string messagecount;
        public static string Usertype;

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public string UsertypeDoc()
        {
            var data = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
            return data.USERTYPE;
        }
        [Authorize]
        public void _changepassword()
        {


        }
        public string messagecountvalue()
        {
            MessageUnitOfWork muw = new MessageUnitOfWork();
            int value = muw.Context.Messages.Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUS == "UnRead" && f.MESSAGESTATUSFROM == "ACTIVE").Count();
            return value.ToString();
        }

        [HttpPost]
        public ActionResult _changepassword(DoctorsInformationEditModel model)
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
                            return Json(new { msg = "Password Changed Successfully" }, JsonRequestBehavior.AllowGet);
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
            }
            // If we got this far, something failed, redisplay form

        }
        [HttpGet]
        [Authorize]
        public ActionResult DoctorsInformatonEdit(string id)
        {

            if (UsertypeDoc() != "D")
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
            if (id == null)
                id = User.Identity.Name;
            _DoctorsInfoEdit(id);
            _DoctorsInfoClinicPhotosEdit(id);
            _DoctorsPayRatesEdit(id);
            _DoctorsNpiDisplay(id);
            _DoctorseduDisplay(id);
            _DoctorscontactDisplay(id);

            DoctorsInformationEditModel model = DoctorDisplayData(id);
            string message = Convert.ToString(Session["Message"]);
            model.inboxUreadMessageCount = messagecountvalue();
            model.MessageUpdateStatus = message;
            Session["Message"] = "";
            profilepath = model.USERPHOTOFILEPATH;
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult SocialHubConnect()
        {

            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            model.inboxUreadMessageCount = messagecountvalue();
            string id = model.UserName;

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var rejectdata = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == id && f.STATUS == "VIDEO APPT CNCLD").Count();
            model.rejectCount = rejectdata.ToString();
            var tempdata =
                 apuow.Context.APPOINTMENTS
                   .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                   (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                   .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                   (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ui.COMMENT })
                .Where(f => f.DOCTORID == id).OrderByDescending(f => f.RATINGDATE).Take(2).ToList();
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
            model.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult AppoinmnetRestriction()
        {

            BookingAppointmentModelOuter model = new BookingAppointmentModelOuter();
            string data = Convert.ToString(Session["ErrorMessage"]);
            if (data != null)
            {

                model.MessageUpdateStatus = data;
            }
            return View(model);
        }


        [HttpGet]
        [Authorize]
        public ActionResult DoctorsInformatonMain(string id)
        {
            GetLocalTime(DateTime.UtcNow);
            if (UsertypeDoc() != "D")
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
            if (id == null)
                id = User.Identity.Name;
            _DoctorsInfoEdit(id);
            _DoctorsInfoClinicPhotosEdit(id);
            _DoctorsPayRatesEdit(id);
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            model.inboxUreadMessageCount = messagecountvalue();
            profilepath = model.USERPHOTOFILEPATH;
            return View(_doctorpatientreview(model));
        }

        [HttpGet]
        [Authorize]
        public ActionResult DoctorsReports()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);

            temp.inboxUreadMessageCount = messagecountvalue();
            return View(temp);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult DoctorsSearch()
        {

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
        [HttpGet]
        [Authorize]
        public ActionResult Inbox()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);

            model.inboxUreadMessageCount = messagecountvalue();

            return View(model);
        }
        [Authorize]
        public ActionResult Appointment()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);

            temp.inboxUreadMessageCount = messagecountvalue();

            return View(temp);
        }
        [Authorize]
        public ActionResult DoctorChangedPassword()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.inboxUreadMessageCount = messagecountvalue();

            return View(temp);
        }

        [Authorize]
        public ActionResult EducationalInfo()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.inboxUreadMessageCount = messagecountvalue();
            temp.profilecompleted = ProfilePercent(User.Identity.Name).ToString();

            return View(temp);
        }

        [Authorize]
        public ActionResult ClinicInfo()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.inboxUreadMessageCount = messagecountvalue();
            temp.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return View(temp);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ClinicInfo(DoctorsInformationEditModel updatemodel)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }

            _DoctorsInfoClinicPhotosEdit(updatemodel);
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            USERSINFORMATION user = null;


            user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);

            model.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return RedirectToAction("ClinicInfo");

        }
        [Authorize]
        public ActionResult VideoInfo()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.inboxUreadMessageCount = messagecountvalue();
            temp.profilecompleted = ProfilePercent(User.Identity.Name).ToString();

            return View(temp);
        }

        [Authorize]
        [HttpPost]
        public ActionResult VideoInfo(DoctorsInformationEditModel model)
        {
            USERSINFORMATION user = null;


            user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }

            try
            {
                if (model.RateQuatermins.ToString() != null && model.RateQuatermins != 0)
                {
                    PayRateUnitOfWork puow = new PayRateUnitOfWork();
                    DoctorPayRateServiceRepository repo = new DoctorPayRateServiceRepository(puow);

                    var DoctorPayrate = puow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == User.Identity.Name && f.DURATION == 15);
                    if (DoctorPayrate != null)
                    {
                        DoctorPayrate.CREATEDBY = User.Identity.Name;
                        DoctorPayrate.MODIFIEDON = DateTime.UtcNow;
                        DoctorPayrate.DateCreated = DateTime.UtcNow;
                        DoctorPayrate.MODIFIEDBY = User.Identity.Name;
                        DoctorPayrate.State = State.Modified;
                        DoctorPayrate.RATE = model.RateQuatermins;
                        DoctorPayrate.ACTIVEFROM = DateTime.UtcNow;
                        repo.InsertOrUpdate(DoctorPayrate);
                        puow.Save();
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Error :Pay rates required");
                    return View(model);
                }

                if (model.PAYPALEMAIL != null)
                {
                    using (UserRepository repo4 = new UserRepository(uow))
                    {
                        user.PAYPALEMAIL = model.PAYPALEMAIL;
                        user.State = State.Modified;
                        repo4.InsertOrUpdateGraph(user);
                        uow.Save();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error :Paypal E-mail required");
                    return View(model);
                }
                ModelState.AddModelError("", "Video Info  updated");
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EducationalInfo(DoctorsInformationEditModel updatemodel)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }

            DOCTORSINFORMATION docinfo = null;


            docinfo = diuow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);


            try
            {
                if (updatemodel != null)
                {


                    string temp = null;
                    if (updatemodel.GETSUFFIX != null)
                    {
                        foreach (var data in updatemodel.GETSUFFIX)
                        {
                            if (temp == null)
                            {
                                temp = data;
                            }
                            else
                            {
                                temp = temp + " , " + data;
                            }
                        }
                        docinfo.SUFFIX = temp;
                    }
                    temp = null;

                    docinfo.EDUCATIONALQUAL = updatemodel.EDUCATIONALQUAL;
                    docinfo.AWARDCERTIFICATION = updatemodel.AWARDCERTIFICATION;
                    docinfo.PROFESSIONALMEMBERSHIP = updatemodel.PROFESSIONALMEMBERSHIP;
                    if (updatemodel.CERTIFICATIONBOARDS != null)
                    {
                        foreach (var data in updatemodel.CERTIFICATIONBOARDS)
                        {
                            if (temp == null)
                            {
                                temp = data;
                            }
                            else
                            {
                                temp = temp + " , " + data;
                            }
                        }
                        docinfo.CERTIFICATIONBOARD = temp;
                    }

                    docinfo.HOSPITALAFFILIATION = updatemodel.HOSPITALAFFILIATION;
                    docinfo.MEDICALSCHOOL = updatemodel.MEDICALSCHOOL;
                    docinfo.PRACTICENAME = updatemodel.PRACTICENAME;
                    docinfo.RESIDENCY = updatemodel.RESIDENCY;
                    docinfo.PAYPALEMAIL = updatemodel.PAYPALEMAIL;
                    docinfo.MODIFIEDBY = User.Identity.Name;
                    docinfo.MODIFIEDON = DateTime.UtcNow;
                    docinfo.UserName = updatemodel.UserName;
                    docinfo.State = State.Modified;
                    docinfo.PRACTISESINCE = updatemodel.PRACTISESINCE;
                    docinfo.ABOUTME = updatemodel.ABOUTME;
                    updatemodel.MessageUpdateStatus = "Doctors Details Updated";
                    message = "Doctors Details Updated";











                    if (updatemodel.Specialities != null)
                    {
                        temp = null;
                        foreach (var data in updatemodel.Specialities)
                        {
                            if (temp == null)
                            {
                                temp = data;
                            }
                            else
                            {
                                temp = temp + " , " + data;
                            }
                        }

                        var datas = suow.Context.DoctorsSpecialities.FirstOrDefault(f => f.UserName == updatemodel.UserName);
                        if (datas == null)
                        {

                            SpecialityUnitOfWork suow2 = new SpecialityUnitOfWork();
                            DoctorSpecialitiesServiceRepository repo5 = new DoctorSpecialitiesServiceRepository(suow2);


                            DOCTORSPECIALITY speciality = new DOCTORSPECIALITY();
                            speciality.CREATEDBY = User.Identity.Name;
                            speciality.MODIFIEDON = DateTime.UtcNow;
                            speciality.DateCreated = DateTime.UtcNow;
                            speciality.MODIFIEDBY = User.Identity.Name;
                            speciality.State = State.Added;
                            speciality.UserName = updatemodel.UserName;
                            speciality.SPECIALITY = temp;
                            repo5.InsertOrUpdate(speciality);
                            suow2.Save();
                            suow2 = null;
                            repo5 = null;

                        }
                        else
                        {
                            using (DoctorSpecialitiesServiceRepository repo2 = new DoctorSpecialitiesServiceRepository(suow))
                            {
                                datas.MODIFIEDON = DateTime.UtcNow;
                                datas.MODIFIEDBY = User.Identity.Name;
                                datas.State = State.Modified;
                                datas.SPECIALITY = temp;
                                repo2.InsertOrUpdate(datas);
                                suow.Save();
                            }
                        }
                    }
                    using (DoctorInformationServiceRepository repo4 = new DoctorInformationServiceRepository(diuow))
                    {
                        docinfo.State = State.Modified;
                        repo4.InsertOrUpdate(docinfo);
                        diuow.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }

            return RedirectToAction("VideoInfo");

        }

        [Authorize]
        public ActionResult PersonalInfo()
        {
            if (UsertypeDoc() != "D")
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
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.inboxUreadMessageCount = messagecountvalue();
            temp.profilecompleted = ProfilePercent(User.Identity.Name).ToString();

            return View(temp);
        }

        [Authorize]
        [HttpPost]
        public ActionResult PersonalInfo(DoctorsInformationEditModel updatemodel)
        {
            if (UsertypeDoc() != "D")
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
            USERSINFORMATION user = null;

            user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
            if (updatemodel.DOBYEAR == 0 || updatemodel.DOBMonth == null || updatemodel.DOBDAY == 0)
            {
                message = "Date fields are required";
                return View(updatemodel);
            }
            if (updatemodel.PRACTISESINCE > DateTime.UtcNow)
            {
                message = "Error : Practise since date is invalid ";
                return View(updatemodel);
            }

            try
            {
                if (updatemodel.FIRSTNAME != null)
                {
                    user.FIRSTNAME = updatemodel.FIRSTNAME;
                    user.LASTNAME = updatemodel.LASTNAME;
                    user.Salutation = updatemodel.Salutation;
                    user.USERSTREETADDRESS1 = updatemodel.USERSTREETADDRESS1;
                    user.USERSTREETADDRESS2 = updatemodel.USERSTREETADDRESS2;
                    user.CITY = updatemodel.CITY;
                    user.USERSTATE = updatemodel.USERSTATE;
                    user.ZIPCODE = updatemodel.ZIPCODE;
                    user.UserName = updatemodel.UserName;
                    user.PASSWORDHASH = user.PASSWORDHASH;
                    user.PASSWORDSALT = user.PASSWORDSALT;
                    user.DateLastActivity = DateTime.UtcNow;
                    user.DateLastLogin = DateTime.UtcNow;
                    if (updatemodel.CurrenttimeZone != null)
                        user.CurrenttimeZone = updatemodel.CurrenttimeZone;
                    user.DateLastPasswordChange = DateTime.UtcNow;
                    DateTime tempdate = new DateTime(Convert.ToInt32(updatemodel.DOBYEAR), Convert.ToInt32(updatemodel.DOBMonth), Convert.ToInt32(updatemodel.DOBDAY));
                    updatemodel.DOB = tempdate;
                    user.DOB = updatemodel.DOB;
                    user.IsApproved = true;
                    user.MODIFIEDBY = User.Identity.Name;
                    user.MODIFIEDON = DateTime.UtcNow;
                    if (UserProfilePic != null)
                    {
                        user.USERPHOTOFILEPATH = UserProfilePic;
                    }

                    updatemodel.MessageUpdateStatus = "Doctors Details Updated";
                    message = "Doctors Details Updated";





                    if (updatemodel.Languages != null)
                    {
                        using (UserLanguageServiceRepository repo = new UserLanguageServiceRepository(luow))
                        {

                            repo.Delete(updatemodel.UserName);
                            luow.Save();
                        }
                        for (int i = 0; i < updatemodel.Languages.Length; i++)
                        {
                            UserLanguageUnitOfWork luow2 = new UserLanguageUnitOfWork();
                            UserLanguageServiceRepository repo3 = new UserLanguageServiceRepository(luow2);


                            USERSLANGUAGE UserLanguages = new USERSLANGUAGE();
                            UserLanguages.CREATEDBY = User.Identity.Name;
                            UserLanguages.MODIFIEDON = DateTime.UtcNow;
                            UserLanguages.DateCreated = DateTime.UtcNow;
                            UserLanguages.MODIFIEDBY = User.Identity.Name;
                            UserLanguages.State = State.Added;
                            UserLanguages.UserName = updatemodel.UserName;
                            UserLanguages.LANGUAGEKEYID = updatemodel.Languages[i];
                            repo3.InsertOrUpdate(UserLanguages);
                            luow2.Save();
                            luow2 = null;
                            repo3 = null;

                        }
                    }

                    using (UserRepository repo4 = new UserRepository(uow))
                    {
                        user.State = State.Modified;
                        repo4.InsertOrUpdateGraph(user);
                        uow.Save();
                        return RedirectToAction("EducationalInfo");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }


            return View(updatemodel);
        }

        [HttpGet]
        [Authorize]
        public ActionResult SentItems()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.inboxUreadMessageCount = messagecountvalue();

            return View(temp);
        }


        [HttpGet]
        [Authorize]
        public void _DoctorsSearch()
        {

        }
        [HttpGet]
        [Authorize]
        public ActionResult Index(string sendto, string MessageBody)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);

            temp.SENDMESSAGETO = sendto;
            temp.MESSAGEBODY = MessageBody;
            temp.inboxUreadMessageCount = messagecountvalue();
            temp.INSTITUTIONNAME = "Test";

            return View(temp);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(DoctorsInformationEditModel temp)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            string updatemessage = null;
            try
            {
                if (ModelState.IsValid)
                {
                    if (temp.SENDMESSAGETO != null)
                    {
                        if (temp.MESSAGESUBJECT != null)
                        {

                           // MailUtility.SendEmail("info@PROFESSORSONLINE.com", temp.SENDMESSAGETO, "", "", "doccare online E-Mail", "Dear Pateint , doctor has sent a mail.Please check your doccare online Inbox", false, "", 0);
                            using (MessageRepository repo = new MessageRepository(muw))
                            {
                                MESSAGE message = new MESSAGE();
                                StringBuilder bodyMsg = new StringBuilder();
                                message.MESSAGESUBJECT = temp.MESSAGESUBJECT;
                                message.MESSAGEBODY = temp.MESSAGEBODY;
                                message.DateCreated = DateTime.UtcNow;
                                message.SENDMESSAGETO = temp.SENDMESSAGETO;
                                message.SENDMESSAGEFROM = User.Identity.Name;
                                message.SENDBY = User.Identity.Name;
                                message.MESSAGESTATUSFROM = "ACTIVE";
                                message.MESSAGESTATUSTO = "ACTIVE";
                                message.MESSAGETYPE = "Ordinary";
                                message.MESSAGESTATUS = "UnRead";
                                message.MODIFIEDON = DateTime.UtcNow;
                                message.SENDDATE = DateTime.UtcNow;
                                message.State = DOCVIDEO.ObjectState.State.Added;
                                repo.InsertOrUpdate(message);
                                muw.Save();
                                updatemessage = "Message sent successfully";
                                var patient = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.SENDMESSAGETO);
                                if (patient != null)
                                {
                                    /*
                                    bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                                    bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                                    bodyMsg.Append("<div>");
                                    bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("<div style='padding:10px;'>");
                                    bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                                    bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'>Dear," + patient.FIRSTNAME + " " + patient.LASTNAME + "</h2>");
                                    bodyMsg.Append("<div style='float:left;width:440px;'> ");
                                    bodyMsg.Append("<div>");
                                    bodyMsg.Append("Your Doctor has send you a secure email. Please log in to doccare to ");
                                    bodyMsg.Append("Review and respond the email.");
                                    bodyMsg.Append("<p>");
                                    bodyMsg.Append("Please  <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn 'title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");
                                    bodyMsg.Append("</a> to log in to your doccare account!");
                                    bodyMsg.Append("</p>");
                                    bodyMsg.Append("</div> ");
                                    bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 25px;font-style: italic;margin:100px 0px;'>");
                                    bodyMsg.Append("<strong>See your Doctors, anytime, anywhere!");
                                    bodyMsg.Append("</strong>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("<p>If you have questions about doccare online, please email us at");
                                    bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'>  helpdesk@PROFESSORSONLINE.com.");
                                    bodyMsg.Append("</a> For FAQ, please visit");
                                    bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>");
                                    bodyMsg.Append("  www.PROFESSORSONLINE.com/faq. ");
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
                                    bodyMsg.Append("<strong>");
                                    bodyMsg.Append("Disclaimer:");
                                    bodyMsg.Append("</strong> ");
                                    bodyMsg.Append("This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
                                    bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
                                    bodyMsg.Append("</strong>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("</div>");

                                    DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", temp.SENDMESSAGETO, "", "", "doccare online has a secure email from your Doctor.", bodyMsg.ToString(), false, "", 0);
                                   */
                                }

                            }
                        }
                        else
                        {
                            updatemessage = "Error :Message Subject  Required";
                        }
                    }
                    else
                    {
                        updatemessage = "Error :Send Message address  Required";
                    }
                }
                else
                {
                    updatemessage = "Error :Model Not Valid";
                }

            }
            catch (Exception ex)
            {
                updatemessage = "Error :" + ex.StackTrace.ToString();
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            temp.INSTITUTIONNAME = "Test";
            model.MessageUpdateStatus = updatemessage;
            return View(model);
        }


        public JsonResult GetData(JsonResult data)
        {

            JsonResult result = new JsonResult();
            result.Data = data;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult GetSpecialites(DoctorsInformationEditModel model)
        {
            string temp = null;
            var tempdata = uow.Context.Doctorspecialities.Where(f => f.UserName == model.UserName);
            foreach (var data in tempdata)
            {

                if (temp == null)
                {
                    temp = data.SPECIALITY.ToString();
                }
                else
                {
                    temp = temp + " , " + data.SPECIALITY.ToString();
                }
            }
            JsonResult result = new JsonResult();
            result.Data = temp;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [AllowAnonymous]
        public ActionResult DoctorsSearchData(DoctorsInformationEditModel model)
        {
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;

            Session["GENDER"] = model.GENDER;
            Session["ZIPCODE"] = model.ZIPCODE;
            Session["CITY"] = model.CITY;
            Session["USERSTATE"] = model.USERSTATE;
            Session["Speciality"] = model.Speciality;
            Session["Language"] = model.Language;



            if (model.GENDER == null && model.ZIPCODE == null && model.CITY == null && model.USERSTATE == null && model.Speciality == null && model.Language == null)
            {


                var data = uow.Context.UserInformations
                         .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                      (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                      .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                      (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                    .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                      (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS }).Where(f => f.USERTYPE == "D" && f.IsApproved == true).ToList();

                result.Data = data;
                Session["Gridresult"] = data;

            }
            else if (model.GENDER != null && model.ZIPCODE != null && model.CITY != null && model.USERSTATE != null && model.Speciality != null && model.Language != null)
            {
                var data = uow.Context.UserInformations
                   .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ds.SPECIALITY })
                  .Join(uow.Context.UserLanguages, ui => ui.UserName, ul => ul.UserName,
                  (ui, ul) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SPECIALITY, ul.LANGUAGEKEYID })
                  .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SPECIALITY, ui.LANGUAGEKEYID, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, di => di.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SPECIALITY, ui.LANGUAGEKEYID, ui.INSTITUTIONNAME, ui.CLINICCITY, di.MEDICALSCHOOL, di.SUFFIX, di.STATELICENSE, di.PRACTISESINCE, ui.STREETADDRESS1, ui.STREETADDRESS2, ui.CLINICUSERSTATE, ui.CLINICZIPCODE })
                  .Where(f => f.GENDER == model.GENDER
                  && f.CITY.ToLower() == model.CITY.ToLower()
                  && f.CLINICZIPCODE == model.ZIPCODE
                  && f.STATELICENSE == model.USERSTATE
                  && f.SPECIALITY.Contains(model.Speciality)
                  && f.LANGUAGEKEYID == model.Language
                  && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;

            }
            else if (model.GENDER != null && model.ZIPCODE != null && model.CITY != null && model.USERSTATE != null && model.Speciality != null && model.Language != null)
            {
                var data = uow.Context.UserInformations
                 .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE, })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.USERTYPE, ui.IsApproved, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.UserLanguages, ui => ui.UserName, ul => ul.UserName,
                 (ui, ul) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, ul.LANGUAGEKEYID })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, ui.LANGUAGEKEYID, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                  .Where(f => f.GENDER == model.GENDER
                 && f.CITY.ToLower() == model.CITY.ToLower()
                 && f.CLINICZIPCODE == model.ZIPCODE
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.LANGUAGEKEYID == model.Language
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.ZIPCODE != null && model.CITY != null && model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                 (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE, })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                  .Where(f => f.GENDER == model.GENDER
                 && f.CITY.ToLower() == model.CITY.ToLower()
                 && f.CLINICZIPCODE == model.ZIPCODE
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;

            }
            else if (model.ZIPCODE != null && model.CITY != null && model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                 .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE, })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.CITY.ToLower() == model.CITY.ToLower()
                 && f.CLINICZIPCODE == model.ZIPCODE
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.CITY != null && model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE, })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.CITY.ToLower() == model.CITY.ToLower()
                 && f.GENDER == model.GENDER
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.ZIPCODE != null && model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE, })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.ZIPCODE == model.ZIPCODE
                 && f.GENDER == model.GENDER
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.ZIPCODE != null && model.CITY != null && model.USERSTATE != null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE, })
               .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                    .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.GENDER == model.GENDER
                        && f.CLINICZIPCODE == model.ZIPCODE
                        && f.STATELICENSE == model.USERSTATE
                        && f.USERTYPE == "D"
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.CITY != null && model.ZIPCODE != null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                  .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                    .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.GENDER == model.GENDER
                        && f.STATELICENSE == model.USERSTATE
                        && f.CLINICZIPCODE == model.ZIPCODE
                        && f.USERTYPE == "D"
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.CITY != null && model.USERSTATE != null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                    .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                    .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.GENDER == model.GENDER
                        && f.STATELICENSE == model.USERSTATE
                        && f.USERTYPE == "D"
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.ZIPCODE != null && model.CITY != null && model.USERSTATE != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                    .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.ZIPCODE == model.ZIPCODE
                        && f.STATELICENSE == model.USERSTATE
                        && f.USERTYPE == "D"
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.CITY != null && model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.CITY.ToLower() == model.CITY.ToLower()
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.GENDER == model.GENDER
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.CITY != null && model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.CITY.ToLower() == model.CITY.ToLower()
                 && f.STATELICENSE == model.USERSTATE
                 && f.SPECIALITY.Contains(model.Speciality)
                 && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.CITY != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                    .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.GENDER == model.GENDER
                        && f.STATELICENSE == model.USERSTATE
                        && f.USERTYPE == "D"
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.ZIPCODE != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                  .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.GENDER == model.GENDER
                        && f.CLINICZIPCODE == model.ZIPCODE
                        && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.USERSTATE != null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                    .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.GENDER == model.GENDER
                        && f.STATELICENSE == model.USERSTATE
                        && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.CITY != null && model.ZIPCODE != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.CITY == model.CITY
                        && f.CLINICZIPCODE == model.ZIPCODE
                        && f.USERTYPE == "D"
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.CITY != null && model.USERSTATE != null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.CITY == model.CITY
                        && f.STATELICENSE == model.USERSTATE
                        && f.USERTYPE == "D"
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.ZIPCODE != null && model.USERSTATE != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.CLINICZIPCODE == model.ZIPCODE
                        && f.STATELICENSE == model.USERSTATE
                        && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.USERSTATE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                .Where(f => f.STATELICENSE == model.USERSTATE
                && f.SPECIALITY.Contains(model.Speciality) && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.Speciality != null)
            {

                var data = uow.Context.UserInformations
                  .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                .Where(f => f.GENDER == model.GENDER
                && f.SPECIALITY.Contains(model.Speciality) && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }

            else if (model.CITY != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                .Where(f => f.CITY == model.CITY
                && f.SPECIALITY.Contains(model.Speciality) && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.ZIPCODE != null && model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                   .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                    .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                .Where(f => f.ZIPCODE == model.ZIPCODE
                && f.SPECIALITY.Contains(model.Speciality) && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.GENDER != null && model.ZIPCODE == null && model.CITY == null && model.USERSTATE == null && model.Speciality == null && model.Language == null)
            {
                var data = uow.Context.UserInformations
                   .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.STREETADDRESS1, dwi.CLINICCITY, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.GENDER == model.GENDER && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.CITY != null && model.ZIPCODE == null && model.GENDER != null && model.USERSTATE == null && model.Speciality == null && model.Language == null)
            {
                var data = uow.Context.UserInformations
                       .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.CITY == model.CITY && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.ZIPCODE != null && model.GENDER != null && model.CITY == null && model.USERSTATE == null && model.Speciality == null && model.Language == null)
            {
                var data = uow.Context.UserInformations
                   .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.CLINICZIPCODE == model.ZIPCODE
                        && f.USERTYPE == "D"
                        && f.GENDER == model.GENDER
                        && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.USERSTATE != null && model.GENDER == null && model.ZIPCODE == null && model.CITY == null && model.Speciality == null && model.Language == null)
            {
                var data = uow.Context.UserInformations
                .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.STATELICENSE == model.USERSTATE && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.Speciality != null)
            {
                var data = uow.Context.UserInformations
                   .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                 .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                 (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.SPECIALITY.Contains(model.Speciality) && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.ZIPCODE != null && model.GENDER == null && model.CITY == null && model.USERSTATE == null && model.Speciality == null && model.Language == null && model.MEDICALSCHOOL == null && model.INSTITUTIONNAME == null)
            {
                var data = uow.Context.UserInformations
                      .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                         .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                     .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                    .Where(f => f.CLINICZIPCODE == model.ZIPCODE && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }
            else if (model.Language != null)
            {
                var data = uow.Context.UserInformations
                .Join(uow.Context.DoctorsInformations, ui => ui.UserName, ds => ds.UserName,
                  (ui, di) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, di.SUFFIX, di.STATELICENSE })
                .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ds.SPECIALITY })
                .Join(uow.Context.UserLanguages, ui => ui.UserName, ul => ul.UserName,
                 (ui, ul) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, ul.LANGUAGEKEYID })
                 .Join(uow.Context.DocWorkInstitutions, ui => ui.UserName, dwi => dwi.UserName,
                  (ui, dwi) => new { ui.USERPHOTOFILEPATH, ui.Salutation, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.AverageRating, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ui.SUFFIX, ui.STATELICENSE, ui.SPECIALITY, ui.LANGUAGEKEYID, dwi.INSTITUTIONNAME, dwi.CLINICCITY, dwi.STREETADDRESS1, dwi.STREETADDRESS2, dwi.CLINICUSERSTATE, dwi.CLINICZIPCODE, dwi.MAPPINGADDRESS })
                 .Where(f => f.LANGUAGEKEYID == model.Language && f.USERTYPE == "D" && f.IsApproved == true).Distinct().ToList();
                result.Data = data;
                Session["Gridresult"] = data;
            }

            if (result.Data == null)
            {
                return Json(new { msg = "No records" }, JsonRequestBehavior.AllowGet);
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;


            return result;
        }

        [AllowAnonymous]
        public JsonResult GetExistingSession()
        {
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;


            if (Convert.ToString(Session["Gridresult"]) != "")
            {
                result.Data = Session["Gridresult"];
            }


            if (result.Data == null)
            {
                return Json(new { msg = "No records" }, JsonRequestBehavior.AllowGet);
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return result;
        }




        [HttpPost]
        [Authorize]
        public ActionResult DoctorsInformatonEdit(DoctorsInformationEditModel updatemodel)
        {
            if (UsertypeDoc() != "D")
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
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            if (message != null)
            {
                model.MessageUpdateStatus = message;
            }
            message = null;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DoctorsInformatonMain(DoctorsInformationEditModel updatemodel)
        {
            if (UsertypeDoc() != "D")
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
            _DoctorsInfoEdit(_DoctorsInfoClinicPhotosEdit(_DoctorsPayRatesEdit(updatemodel)));
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            if (message != null)
            {
                model.MessageUpdateStatus = message;
            }
            message = null;
            return View(model);
        }
        [Authorize]
        public ActionResult DoctorsInformatonViews(string id)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }

            if (id == null)
            {
                id = User.Identity.Name;
            }
            _DoctorsInfoDisplay(id);
            _DoctorsInfoClinicPhotosDisplay(id);
            _DoctorsPayRatesDisplay(id);
            _DoctorsPayRatesEdit(id);
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            model.inboxUreadMessageCount = messagecountvalue();
            return View(_doctorreview(model));
        }
        [AllowAnonymous]
        public ActionResult DoctorsInformatonViewsAnnonymus(string id, int mode = 0)
        {
            Session["DoctorId"] = id;
            Session["DOCFIRSTNAME"] = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == id).FIRSTNAME;
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
                using (var repo = new UserRepository(uow))
                {
                    var temp = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == id);
                    temp.MODIFIEDON = DateTime.UtcNow;
                    int value = Convert.ToInt32(temp.HITCOUNT) + 1;
                    temp.HITCOUNT = value.ToString();
                    temp.State = DOCVIDEO.ObjectState.State.Modified;
                    repo.InsertOrUpdate(temp);
                    uow.Save();
                }

            }


            _DoctorsInfoDisplayCustom(id);
            _DoctorsInfoDisplay(id);
            _DoctorsInfoClinicPhotosDisplay(id);
            _DoctorsPayRatesEdit(id);
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            var data = apuow.Context.APPOINTMENTS
                .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ui.COMMENT })
             .Where(f => f.DOCTORID == id).ToList();
            if (data != null)
            {
                foreach (var value in data)
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
            model.mode = mode;
            model.inboxUreadMessageCount = messagecountvalue();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult DoctorsInformatonCalender(string id)
        {
            Session["DoctorId"] = id;
            Session["DOCFIRSTNAME"] = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == id).FIRSTNAME;
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
                using (var repo = new UserRepository(uow))
                {
                    var temp = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == id);
                    temp.MODIFIEDON = DateTime.UtcNow;
                    int value = Convert.ToInt32(temp.HITCOUNT) + 1;
                    temp.HITCOUNT = value.ToString();
                    temp.State = DOCVIDEO.ObjectState.State.Modified;
                    repo.InsertOrUpdate(temp);
                    uow.Save();
                }

            }


            _DoctorsInfoDisplayCustom(id);
            _DoctorsInfoDisplay(id);
            _DoctorsInfoClinicPhotosDisplay(id);
            _DoctorsPayRatesEdit(id);
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            var data = apuow.Context.APPOINTMENTS
                .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ui.COMMENT })
             .Where(f => f.DOCTORID == id).ToList();
            if (data != null)
            {
                foreach (var value in data)
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
            model.inboxUreadMessageCount = messagecountvalue();
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult DoctorsInformatonViewsReviews(string id)
        {
            if (id == null)
                id = User.Identity.Name;
            _DoctorsInfoDisplay(id);
            _DoctorsInfoClinicPhotosDisplay(id);
            _DoctorsPayRatesEdit(id);
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            model.inboxUreadMessageCount = messagecountvalue();
            return View(_doctorreview(model));
        }
        [Authorize]
        public ActionResult DoctorsInformatonReviews()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            model.inboxUreadMessageCount = messagecountvalue();
            return View(_doctorreview(model));
        }
        public int ProfilePercent(string id)
        {
            double percent = 0;
            int total = 50;
            int currentvalue = 0;

            var userinfo = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == id);
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
                if (userinfo.USERSTATE != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.ZIPCODE != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.CurrenttimeZone != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.PAYPALEMAIL != null)
                    currentvalue = currentvalue + 1;
                if (userinfo.USERPHOTOFILEPATH != null)
                    currentvalue = currentvalue + 1;

            }

            var userdocinfo = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == id);
            if (userdocinfo != null)
            {
                if (userdocinfo.EDUCATIONALQUAL != null)
                    currentvalue = currentvalue + 1;
                if (userdocinfo.HOSPITALAFFILIATION != null)
                    currentvalue = currentvalue + 1;
                if (userdocinfo.CERTIFICATIONBOARD != null)
                    currentvalue = currentvalue + 1;
                if (userdocinfo.PROFESSIONALMEMBERSHIP != null)
                    currentvalue = currentvalue + 1;
                if (userdocinfo.SUFFIX != null)
                    currentvalue = currentvalue + 1;
                if (userdocinfo.PRACTISESINCE != null)
                    currentvalue = currentvalue + 1;
                if (userdocinfo.PRACTICENAME != null)
                    currentvalue = currentvalue + 1;
            }

            var userdocclinicinfo = uow.Context.DocWorkInstitutions.FirstOrDefault(f => f.UserName == id);
            if (userdocclinicinfo != null)
            {
                if (userdocclinicinfo.CONTACTPERSON != null)
                    currentvalue = currentvalue + 1;
                if (userdocclinicinfo.CONTACTMAILID != null)
                    currentvalue = currentvalue + 1;
                if (userdocclinicinfo.INSTITUTIONNAME != null)
                    currentvalue = currentvalue + 1;
                if (userdocclinicinfo.STREETADDRESS1 != null)
                    currentvalue = currentvalue + 1;
                if (userdocclinicinfo.CLINICCITY != null)
                    currentvalue = currentvalue + 1;
                if (userdocclinicinfo.CLINICUSERSTATE != null)
                    currentvalue = currentvalue + 1;
                if (userdocclinicinfo.CLINICZIPCODE != null)
                    currentvalue = currentvalue + 1;
                if (userdocclinicinfo.TELEPHONE != null)
                    currentvalue = currentvalue + 1;

            }


            if (uow.Context.DoctorSlots.Any(f => f.UserName == id && f.AVAILABILITYMODE == "C"))
                currentvalue = currentvalue + 10;

            var userdocpayratesinfo = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == id && f.DURATION == 15);
            if (userdocpayratesinfo != null)
            {
                if (uow.Context.DoctorSlots.Any(f => f.UserName == id && f.AVAILABILITYMODE == "V") && userdocpayratesinfo.RATE.ToString() != null)
                    currentvalue = currentvalue + 10;

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
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            model.inboxUreadMessageCount = messagecountvalue();
            string id = model.UserName;

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var rejectdata = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == id && f.STATUS == "VIDEO APPT CNCLD").Count();
            model.rejectCount = rejectdata.ToString();
            var tempdata =
                 apuow.Context.APPOINTMENTS
                   .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                   (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                   .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                   (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ui.COMMENT })
                .Where(f => f.DOCTORID == id).OrderByDescending(f => f.RATINGDATE).Take(2).ToList();
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
            model.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            return View(model);
        }

        [Authorize]
        public ActionResult ReleaseStatement()
        {

            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            model.inboxUreadMessageCount = messagecountvalue();
            model.appointment = false;
            //DateTime endtime=DateTime.UtcNow.AddSeconds(-1)
            var tempUsertype = appointmentUOW.Context.UserInformations.FirstOrDefault(w => (w.UserName == User.Identity.Name));
            if (tempUsertype.USERTYPE == "P")
            {
                //var temp = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD");
                var temp = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.Now && w.APPOINTMENTENDTIME >= DateTime.Now) && w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                if (temp != null)
                {
                    foreach (var t in temp)
                    {
                        if (t.APPOINTMENTENDTIME.Value.AddSeconds(-1) >= DateTime.UtcNow)
                        {
                            int totalMinutes = Convert.ToInt32((t.APPOINTMENTENDTIME.Value.Subtract(t.APPOINTMENTSTARTTIME.Value)).TotalMinutes);
                            var docdata = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == t.DOCTORID && f.DURATION == 15);
                            if (docdata != null)
                            {
                                model.APPOINTMENTID = t.APPOINTMENTID;
                                model.appointment = true;
                                model.TimeDisplay = docdata.DURATION.ToString();
                                model.RateDisplay = docdata.RATE.ToString();
                            }
                            break;
                        }
                    }

                }
            }
            else if (tempUsertype.USERTYPE == "D")
            {

                var temp = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.Now && w.APPOINTMENTENDTIME >= DateTime.Now) && w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                if (temp != null)
                {
                    foreach (var t in temp)
                    {
                        if (t.APPOINTMENTENDTIME.Value.AddSeconds(-1) >= DateTime.UtcNow)
                        {
                            int totalMinutes = Convert.ToInt32((t.APPOINTMENTENDTIME.Value.Subtract(t.APPOINTMENTSTARTTIME.Value)).TotalMinutes);
                            var docdata = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == t.DOCTORID && f.DURATION == 15);
                            if (docdata != null)
                            {
                                model.APPOINTMENTID = t.APPOINTMENTID;
                                model.appointment = true;
                                model.TimeDisplay = docdata.DURATION.ToString();
                                model.RateDisplay = docdata.RATE.ToString();
                            }
                            break;
                        }
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ReleaseStatement(DoctorsInformationEditModel model)
        {
            var temp = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(f => f.APPOINTMENTID == model.APPOINTMENTID);
            if (temp != null)
            {
                if (model.USERTYPE == "P")
                {
                    using (var repo = new AppointmentServiceRepository(appointmentUOW))
                    {
                        temp.uploadreport = model.uploadreport;
                        temp.termsOfUse = model.termsOfUse;
                        temp.realeaseStatement = model.realeaseStatement;
                        temp.patientintials = model.patientintials;
                        temp.MODIFIEDBY = User.Identity.Name;
                        temp.MODIFIEDON = DateTime.UtcNow;
                        temp.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(temp);
                        appointmentUOW.Save();

                        if (model.termsOfUse == true && model.realeaseStatement == true)
                        {
                            return RedirectToAction("VideoConf");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error :You have not accepted the terms of use and release statements");
                        }
                    }
                }
                else if (model.USERTYPE == "D")
                {
                    using (var repo = new AppointmentServiceRepository(appointmentUOW))
                    {
                        temp.medicaladviceDoctor = model.medicaladviceDoctor;
                        temp.termsOfUseDoctor = model.termsOfUseDoctor;
                        temp.releaseStatementDoctor = model.releaseStatementDoctor;
                        temp.doctorintials = model.doctorintials;
                        temp.MODIFIEDBY = User.Identity.Name;
                        temp.MODIFIEDON = DateTime.UtcNow;
                        temp.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(temp);
                        appointmentUOW.Save();

                        if (model.termsOfUseDoctor == true && model.releaseStatementDoctor == true)
                        {
                            return RedirectToAction("VideoConf");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error : You have not accepted the terms of use and release statements");
                        }
                    }
                }

            }

            return View(model);
        }
        [Authorize]
        public DoctorsInformationEditModel _doctorreview(DoctorsInformationEditModel model)
        {

            string id = model.UserName;

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var rejectdata = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == id && f.STATUS == "VIDEO APPT CNCLD").Count();
            model.rejectCount = rejectdata.ToString();
            var tempdata =
                 apuow.Context.APPOINTMENTS
                   .Join(apuow.Context.AppointmentRatings, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                   (ui, ds) => new { ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ds.CLIENTRATING, ds.RATINGDATE, ds.COMMENT })
                   .Join(apuow.Context.UserInformations, ui => ui.PATIENTID, ul => ul.UserName,
                   (ui, ul) => new { ul.FIRSTNAME, ul.LASTNAME, ul.USERPHOTOFILEPATH, ui.CLIENTRATING, ui.RATINGDATE, ui.APPOINTMENTID, ui.DOCTORID, ui.PATIENTID, ui.COMMENT })
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


            _DoctorsReviewRatingDisplay();
            return model;
        }
        [Authorize]
        public DoctorsInformationEditModel _doctorpatientreview(DoctorsInformationEditModel model)
        {

            string id = model.UserName;

            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var rejectdata = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == id && f.STATUS == "VIDEO APPT CNCLD").Count();
            model.rejectCount = rejectdata.ToString();
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

            _DoctorsReviewRatingDisplay();
            return model;
        }
        [Authorize]
        public void _DoctorsReviewRatingDisplay()
        {

        }
        [Authorize]
        public ActionResult DoctorsInformatonUpdateAvailabilty()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            string id = User.Identity.Name;
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            model.inboxUreadMessageCount = messagecountvalue();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DoctorsInformatonUpdateAvailabilty(DoctorsInformationEditModel model)
        {
            var user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }

            try
            {
                if (model.RateQuatermins.ToString() != null && model.RateQuatermins != 0)
                {
                    PayRateUnitOfWork puow = new PayRateUnitOfWork();
                    DoctorPayRateServiceRepository repo = new DoctorPayRateServiceRepository(puow);

                    var DoctorPayrate = puow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == User.Identity.Name && f.DURATION == 15);
                    if (DoctorPayrate != null)
                    {
                        DoctorPayrate.CREATEDBY = User.Identity.Name;
                        DoctorPayrate.MODIFIEDON = DateTime.UtcNow;
                        DoctorPayrate.DateCreated = DateTime.UtcNow;
                        DoctorPayrate.MODIFIEDBY = User.Identity.Name;
                        DoctorPayrate.State = State.Modified;
                        DoctorPayrate.RATE = model.RateQuatermins;
                        DoctorPayrate.ACTIVEFROM = DateTime.UtcNow;
                        repo.InsertOrUpdate(DoctorPayrate);
                        puow.Save();
                    }
                    message = "Payrate updated";
                }

                using (UserRepository repo4 = new UserRepository(uow))
                {
                    user.PAYPALEMAIL = model.PAYPALEMAIL;
                    user.State = State.Modified;
                    repo4.InsertOrUpdateGraph(user);
                    uow.Save();
                }

                message = "Payrate updated";
                model.MessageUpdateStatus = "Doctor PayRates Updated";
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }

            model = DoctorDisplayData(User.Identity.Name);
            model.inboxUreadMessageCount = messagecountvalue();
            return View(model);
        }

        [Authorize]
        public ActionResult DoctorsInformatonClinicPhotos(string id)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            id = User.Identity.Name;
            DoctorsInformationEditModel model = DoctorDisplayData(id);

            _DoctorsInfoClinicPhotosEdit(id);
            string message = Convert.ToString(Session["Message"]);
            model.inboxUreadMessageCount = messagecountvalue();
            model.MessageUpdateStatus = message;
            Session["Message"] = "";
            model.inboxUreadMessageCount = messagecountvalue();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DoctorsInformatonClinicPhotos(DoctorsInformationEditModel updatemodel)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            _DoctorsInfoClinicPhotosEdit(updatemodel);
            DoctorsInformatonEdit(User.Identity.Name);
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public DoctorsInformationEditModel _DoctorsInfoClinicPhotosEdit(DoctorsInformationEditModel model)
        {

            try
            {
                DOCTORWORKINGINSTITUION data = uw.Context.DoctorsWorkInstitutions.FirstOrDefault(f => f.UserName == User.Identity.Name);
                if (data != null)
                {
                    DoctorWorkingInstituteServiceRepository repo = new DoctorWorkingInstituteServiceRepository(uw);
                    if (model.INSTITUTIONNAME != null && model.CLINICZIPCODE != null && model.CLINICUSERSTATE != null && model.CONTACTPERSON != null && model.CONTACTMAILID != null)
                    {
                        //if (model.CLINICZIPCODE != null)
                            data.CLINICZIPCODE = model.CLINICZIPCODE;
                        //if (model.CLINICUSERSTATE != null)
                            data.CLINICUSERSTATE = model.CLINICUSERSTATE;
                        //if (model.STREETADDRESS1 != null)
                            data.STREETADDRESS1 = model.STREETADDRESS1;
                        //if (model.STREETADDRESS2 != null)
                            data.STREETADDRESS2 = model.STREETADDRESS2;
                        if (imagepath != null)
                        {
                            data.INSTITUTIONIMAGEPATH = imagepath;
                        }
                        //if (model.CLINICCITY != null)
                            data.CLINICCITY = model.CLINICCITY;
                        //if (model.CONTACTPERSON != null)
                            data.CONTACTPERSON = model.CONTACTPERSON;
                        //if (model.CONTACTMAILID != null)
                        data.CONTACTMAILID = model.CONTACTMAILID;
                        data.ABOUTCLINIC = model.ABOUTCLINIC;
                        data.MODIFIEDBY = User.Identity.Name;
                        data.MODIFIEDON = DateTime.UtcNow;
                        data.TELEPHONE = model.TELEPHONE;
                        if (model.INSTITUTIONNAME != null)
                            data.INSTITUTIONNAME = model.INSTITUTIONNAME;
                        if (model.STREETADDRESS1 != null)
                            data.MAPPINGADDRESS = model.STREETADDRESS1.Replace(" ", "");
                        if (model.STREETADDRESS2 != null)
                            data.MAPPINGADDRESS = data.MAPPINGADDRESS + "," + model.STREETADDRESS2.Replace(" ", "");
                        if (model.CLINICUSERSTATE != null)
                            data.MAPPINGADDRESS = data.MAPPINGADDRESS + "," + model.CLINICUSERSTATE.Replace(" ", "");

                        ModelState.AddModelError("", "Clinic Details Updated.");
                        repo.InsertOrUpdate(data);
                        uw.Save();



                    }
                    else
                    {
                        if (model.CLINICZIPCODE == null)

                            ModelState.AddModelError("", "Error :Clinic Zip code is Required.");
                        if (model.CLINICUSERSTATE == null)
                            ModelState.AddModelError("", "Error :Clinic UserState is Required.");
                        if (model.STREETADDRESS1 == null)
                            ModelState.AddModelError("", "Error :Clinic Street Address1 is Required.");
                        if (model.CLINICCITY == null)
                            ModelState.AddModelError("", "Error :Clinic City is Required.");
                        if (model.CONTACTPERSON == null)
                            ModelState.AddModelError("", "Error :Clinic Contact Person is Required.");
                        if (model.CONTACTMAILID == null)
                            ModelState.AddModelError("", "Error :Clinic Contact Email-id is Required.");
                        if (model.INSTITUTIONNAME == null)
                            ModelState.AddModelError("", "Error :Clinic Name is Required.");
                        model.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
                        return model;
                    }

                }
                else
                {
                    DoctorWorkingInstituteServiceRepository repo = new DoctorWorkingInstituteServiceRepository(uw);
                    DOCTORWORKINGINSTITUION dw = new DOCTORWORKINGINSTITUION();
                    dw.CLINICZIPCODE = model.CLINICZIPCODE;
                    dw.CLINICUSERSTATE = model.CLINICUSERSTATE;
                    dw.STREETADDRESS1 = model.STREETADDRESS1;
                    dw.STREETADDRESS2 = model.STREETADDRESS2;
                    if (model.CLINICCITY != null)
                        dw.CLINICCITY = model.CLINICCITY;
                    if (model.CONTACTPERSON != null)
                        dw.CONTACTPERSON = model.CONTACTPERSON;
                    if (model.CONTACTMAILID != null)
                        dw.CONTACTMAILID = model.CONTACTMAILID;
                    dw.CREATEDBY = User.Identity.Name;
                    dw.DateCreated = DateTime.UtcNow;
                    if (imagepath != null)
                    {
                        dw.INSTITUTIONIMAGEPATH = imagepath;
                    }
                    dw.MODIFIEDBY = User.Identity.Name;
                    dw.MODIFIEDON = DateTime.UtcNow;
                    dw.TELEPHONE = model.TELEPHONE;
                    dw.UserName = model.UserName;
                    dw.INSTITUTIONNAME = model.INSTITUTIONNAME;
                    model.INSTITUTIONIMAGEPATH = dw.INSTITUTIONIMAGEPATH;
                    ModelState.AddModelError("", "Clinic Details Updated.");
                    repo.InsertOrUpdate(dw);
                    uw.Save();
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            return model;
        }

        [HttpPost]
        [Authorize]
        public DoctorsInformationEditModel _DoctorsPayRatesEdit(DoctorsInformationEditModel model)
        {
            try
            {
                if (model.RateQuatermins.ToString() != null && model.RateQuatermins != 0)
                {
                    PayRateUnitOfWork puow = new PayRateUnitOfWork();
                    DoctorPayRateServiceRepository repo = new DoctorPayRateServiceRepository(puow);

                    var DoctorPayrate = puow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == model.UserName && f.DURATION == 15);
                    if (DoctorPayrate != null)
                    {
                        DoctorPayrate.CREATEDBY = User.Identity.Name;
                        DoctorPayrate.MODIFIEDON = DateTime.UtcNow;
                        DoctorPayrate.DateCreated = DateTime.UtcNow;
                        DoctorPayrate.MODIFIEDBY = User.Identity.Name;
                        DoctorPayrate.State = State.Modified;
                        DoctorPayrate.RATE = model.RateQuatermins;
                        DoctorPayrate.ACTIVEFROM = DateTime.UtcNow;
                        repo.InsertOrUpdate(DoctorPayrate);
                        puow.Save();
                    }
                    message = "Payrate updated";
                }
                if (model.RateHalfmins.ToString() != null && model.RateHalfmins != 0)
                {
                    PayRateUnitOfWork puow1 = new PayRateUnitOfWork();
                    DoctorPayRateServiceRepository repo1 = new DoctorPayRateServiceRepository(puow1);

                    var DoctorPayrate = puow1.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == model.UserName && f.DURATION == 30);
                    if (DoctorPayrate != null)
                    {
                        DoctorPayrate.CREATEDBY = User.Identity.Name;
                        DoctorPayrate.MODIFIEDON = DateTime.UtcNow;
                        DoctorPayrate.DateCreated = DateTime.UtcNow;
                        DoctorPayrate.MODIFIEDBY = User.Identity.Name;
                        DoctorPayrate.State = State.Modified;
                        DoctorPayrate.RATE = model.RateHalfmins;
                        DoctorPayrate.ACTIVEFROM = DateTime.UtcNow;
                        repo1.InsertOrUpdate(DoctorPayrate);
                        puow1.Save();
                    }
                    message = "Payrate updated";
                }
                if (model.RateFortyFivemins.ToString() != null && model.RateFortyFivemins != 0)
                {
                    PayRateUnitOfWork puow3 = new PayRateUnitOfWork();
                    DoctorPayRateServiceRepository repo3 = new DoctorPayRateServiceRepository(puow3);

                    var DoctorPayrate = puow3.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == model.UserName && f.DURATION == 45);
                    if (DoctorPayrate != null)
                    {
                        DoctorPayrate.CREATEDBY = User.Identity.Name;
                        DoctorPayrate.MODIFIEDON = DateTime.UtcNow;
                        DoctorPayrate.DateCreated = DateTime.UtcNow;
                        DoctorPayrate.MODIFIEDBY = User.Identity.Name;
                        DoctorPayrate.State = State.Modified;
                        DoctorPayrate.RATE = model.RateFortyFivemins;
                        DoctorPayrate.ACTIVEFROM = DateTime.UtcNow;
                        repo3.InsertOrUpdate(DoctorPayrate);
                        puow3.Save();
                    }
                    message = "Payrate updated";
                }
                if (model.RateHourmins.ToString() != null && model.RateHourmins != 0)
                {
                    PayRateUnitOfWork puow2 = new PayRateUnitOfWork();
                    DoctorPayRateServiceRepository repo2 = new DoctorPayRateServiceRepository(puow2);

                    var DoctorPayrate = puow2.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == model.UserName && f.DURATION == 60);
                    if (DoctorPayrate != null)
                    {
                        DoctorPayrate.CREATEDBY = User.Identity.Name;
                        DoctorPayrate.MODIFIEDON = DateTime.UtcNow;
                        DoctorPayrate.DateCreated = DateTime.UtcNow;
                        DoctorPayrate.MODIFIEDBY = User.Identity.Name;
                        DoctorPayrate.State = State.Modified;
                        DoctorPayrate.RATE = model.RateHourmins;
                        DoctorPayrate.ACTIVEFROM = DateTime.UtcNow;
                        repo2.InsertOrUpdate(DoctorPayrate);
                        puow2.Save();
                    }
                }
                message = "Payrate updated";
                model.MessageUpdateStatus = "Doctor PayRates Updated";
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            return model;
        }
        [AllowAnonymous]
        public DoctorsInformationEditModel DoctorDisplayData(string id)
        {


            USERSINFORMATION user = null;
            DOCTORSINFORMATION docinfo = null;
            //DateTime datepool = GetLocalTime(DateTime.UtcNow.AddMinutes(-60), id);
            //DateTime current = GetLocalTime(DateTime.UtcNow, id);
            DateTime datepool = DateTime.UtcNow.AddMinutes(-30);
            DateTime current = DateTime.UtcNow;
            MessageUnitOfWork muw = new MessageUnitOfWork();

            string temp = null;
            string temp2 = null;
            string temp3 = null;

            DoctorsInformationEditModel model = new DoctorsInformationEditModel();
            WorkInstitutionUnitOfWork uw = new WorkInstitutionUnitOfWork();

            DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
            user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == id);
            docinfo = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == id);
            var docpayrate = uow.Context.DoctorPayRates.Where(f => f.UserName == id);
            model.inboxUreadMessageCount = muw.Context.Messages.Where(f => f.SENDMESSAGETO == user.UserName && f.MESSAGESTATUS == "UnRead" && f.MESSAGESTATUSFROM == "ACTIVE").ToList().Count().ToString();

            messagecount = model.inboxUreadMessageCount;
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();
            var rejectdata = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == user.UserName && f.STATUS == "VIDEO APPT CNCLD").Count();
            model.rejectCount = rejectdata.ToString();
            var appintmentdone = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == user.UserName && f.STATUS == "Appointment Done").Count();
            model.appintmentdone = appintmentdone.ToString();
            model.paymentrecieved = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == user.UserName && f.STATUS == "Appointment Done").Sum(f => f.PAYAMOUNT).ToString();
            model.patientdone = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == user.UserName && f.STATUS == "Appointment Done").Select(f => new { f.PATIENTID }).Distinct().Count().ToString();
            model.appintmentpending = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == user.UserName && (f.STATUS == "VIDEO APPT RQSTD" || f.STATUS == "CLINIC APPT RQSTD") && f.APPOINTMENTSTARTTIME > DateTime.UtcNow).Select(f => new { f.PATIENTID }).Distinct().Count().ToString();
            model.appintmentcurrent = apuow.Context.APPOINTMENTS.Where(f => f.DOCTORID == user.UserName && f.STATUS == "VIDEO APPT CNFRD" && (f.APPOINTMENTSTARTTIME < current && f.APPOINTMENTSTARTTIME > datepool)).Select(f => new { f.DOCTORSLOTID }).Distinct().Any();
            model.profilecompleted = "0%";
            var temprating = uow.Context.AppointmentRatings
                 .Join(uow.Context.Appointments, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                 (ui, ds) => new { ui.CLIENTRATING, ds.DOCTORID }).Where(f => f.DOCTORID == id);
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
                model.Ratings = Convert.ToInt32(user.AverageRating);
            }

            var value = uw.Context.DoctorsWorkInstitutions.FirstOrDefault(w => w.UserName == id);
            var userlangid = uow.Context.UserLanguages
                .Join(uow.Context.languages, ul => ul.LANGUAGEKEYID, l => l.LANGUAGEKEYID,
                (ul, li) => new { ul.LANGUAGEKEYID, li.DESCRIPTION, ul.UserName })
                .Where(f => f.UserName == id);

            string userspecialyid = null;
            var userspecialyiddata = uow.Context.Doctorspecialities.FirstOrDefault(w => w.UserName == id);
            if (userspecialyiddata != null)
            {
                userspecialyid = userspecialyiddata.SPECIALITY;
            }

            var PayrateQuater = uow.Context.DoctorPayRates.Where(f => f.UserName == id && f.DURATION == 15).Select(s => s.RATE).FirstOrDefault();

            var PayrateHalf = uow.Context.DoctorPayRates.Where(f => f.UserName == id && f.DURATION == 30).Select(s => s.RATE).FirstOrDefault();

            var PayrateFortyFive = uow.Context.DoctorPayRates.Where(f => f.UserName == id && f.DURATION == 45).Select(s => s.RATE).FirstOrDefault();

            var PayrateHour = uow.Context.DoctorPayRates.Where(f => f.UserName == id && f.DURATION == 60).Select(s => s.RATE).FirstOrDefault();

            if (PayrateQuater.ToString() != null)
            {
                model.RateQuatermins = PayrateQuater;
            }

            if (PayrateHalf.ToString() != null)
            {
                model.RateHalfmins = PayrateHalf;
            }

            if (PayrateFortyFive.ToString() != null)
            {
                model.RateFortyFivemins = PayrateFortyFive;
            }

            if (PayrateHour.ToString() != null)
            {
                model.RateHourmins = PayrateHour;
            }

            if (userspecialyid != null)
            {

                model.Specialities = userspecialyid.Split(',');
                model.Speciality = userspecialyid;

            }

            if (userlangid != null)
            {
                foreach (var lang in userlangid)
                {
                    if (temp3 == null)
                    {
                        temp3 = lang.DESCRIPTION;
                    }
                    else
                    {
                        temp3 = temp3 + " , " + lang.DESCRIPTION;
                    }
                    if (temp2 == null)
                    {

                        temp2 = lang.LANGUAGEKEYID.ToString();
                    }
                    else
                    {
                        temp2 = temp2 + " , " + lang.LANGUAGEKEYID.ToString();
                    }
                }
                if (temp2 != null)
                {
                    model.Languages = temp2.Split(',');

                }
                if (temp3 != null)
                {
                    model.Language = temp3;
                }
            }

            if (value != null)
            {

                model.STREETADDRESS1 = value.STREETADDRESS1;
                model.STREETADDRESS2 = value.STREETADDRESS2;
                model.CLINICUSERSTATE = value.CLINICUSERSTATE;
                model.ABOUTCLINIC = value.ABOUTCLINIC;
                model.CLINICCITY = value.CLINICCITY;
                model.CLINICZIPCODE = value.CLINICZIPCODE;
                model.CONTACTPERSON = value.CONTACTPERSON;
                model.CONTACTMAILID = value.CONTACTMAILID;
                model.TELEPHONE = value.TELEPHONE;
                model.MAPPINGADDRESS = value.MAPPINGADDRESS;
                if (value.INSTITUTIONNAME != null)
                {
                    model.INSTITUTIONNAME = value.INSTITUTIONNAME;
                }
                else
                {
                    model.INSTITUTIONNAME = "";
                }
                model.INSTITUTIONIMAGEPATH = value.INSTITUTIONIMAGEPATH;

            }
            else
            {
                model.INSTITUTIONNAME = "";
            }
            if (user != null)
            {
                model.currentDate = currentDate;
                model.Salutation = user.Salutation;
                model.FIRSTNAME = user.FIRSTNAME;
                model.LASTNAME = user.LASTNAME;
                model.GENDER = user.GENDER;
                model.USERSTREETADDRESS1 = user.USERSTREETADDRESS1;
                model.USERSTREETADDRESS2 = user.USERSTREETADDRESS2;
                model.CITY = user.CITY;
                model.USERSTATE = user.USERSTATE;
                model.ZIPCODE = user.ZIPCODE;
                model.UserName = user.UserName;
                model.USERPHOTOFILEPATH = user.USERPHOTOFILEPATH;
                model.USERTYPE = user.USERTYPE;
                model.TELEPHONE = model.TELEPHONE;
                model.CurrenttimeZone = user.CurrenttimeZone;
                model.HITCOUNT = user.HITCOUNT;
                model.PAYPALEMAIL = user.PAYPALEMAIL;
                if (user.DOB != null)
                {
                    model.DOB = user.DOB;
                    model.DOBDAYDISPLAY = user.DOB.Value.Day;
                    model.DOBDAY = user.DOB.Value.Day;
                    model.DOBMonth = user.DOB.Value.Month.ToString();
                    model.DOBYEAR = user.DOB.Value.Year;
                    model.DOBDisplay = user.DOB.Value.ToShortDateString();
                }
                LoginFirstName = user.FIRSTNAME;
                LoginLastName = user.LASTNAME;
                profilepath = user.USERPHOTOFILEPATH;


            }
            if (docinfo != null)
            {

                model.LICENSEIN = docinfo.STATELICENSE;
                model.LICENSEEXPIRESON = docinfo.LICENSEEXPIRESON;
                if (docinfo.LICENSEEXPIRESON < DateTime.UtcNow)
                {
                    model.validlicence = true;
                }
                else
                {
                    model.validlicence = false;
                }
                model.LICENSE = docinfo.LICENSE;
                model.PROFESSIONALMEMBERSHIP = docinfo.PROFESSIONALMEMBERSHIP;
                model.CERTIFICATIONBOARD = docinfo.CERTIFICATIONBOARD;
                model.HOSPITALAFFILIATION = docinfo.HOSPITALAFFILIATION;
                model.MEDICALSCHOOL = docinfo.MEDICALSCHOOL;
                model.PRACTICENAME = docinfo.PRACTICENAME;
                model.RESIDENCY = docinfo.RESIDENCY;
                model.EDUCATIONALQUAL = docinfo.EDUCATIONALQUAL;
                model.AWARDCERTIFICATION = docinfo.AWARDCERTIFICATION;
                if (model.LICENSEEXPIRESON != null)
                {
                    model.LICENSEEXPIRESON = docinfo.LICENSEEXPIRESON;
                    model.LICENSEEXPIRESONSTRING = docinfo.LICENSEEXPIRESON.Value.ToShortDateString();
                }
                if (docinfo.PRACTISESINCE != null)
                {
                    model.PRACTISESINCE = docinfo.PRACTISESINCE;
                    model.PRACTISESINCESTRING = docinfo.PRACTISESINCE.Value.ToShortDateString();
                }
                if (docinfo.SUFFIX != null)
                {
                    model.SUFFIX = docinfo.SUFFIX;
                    model.GETSUFFIX = docinfo.SUFFIX.Split(',');
                }
                if (docinfo.CERTIFICATIONBOARD != null)
                {
                    model.CERTIFICATIONBOARD = docinfo.CERTIFICATIONBOARD;
                    model.CERTIFICATIONBOARDS = docinfo.CERTIFICATIONBOARD.Split(',');
                }

                if (docinfo.WEBSITE != null)
                {
                    if (!docinfo.WEBSITE.Contains("http://"))
                    {
                        model.WEBSITE = "http://" + docinfo.WEBSITE;
                    }
                    else
                    {
                        model.WEBSITE = docinfo.WEBSITE;
                    }
                }
                model.ABOUTME = docinfo.ABOUTME;
            }

            return model;
        }





        [HttpGet]
        [Authorize]
        public void _DoctorsInfoClinicPhotosEdit(string id)
        {

        }

        [HttpGet]
        [Authorize]
        public ActionResult DoctorAppointmentConfirmation(int id, int mode = 0)
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
            DoctorsInformationEditModel model = DoctorDisplayData(User.Identity.Name);
            var temp = auok.Context.APPOINTMENTS.FirstOrDefault(f => f.APPOINTMENTID == id && (f.STATUS == "VIDEO APPT CNFRD" || f.STATUS == "VIDEO APPT CNCLD" || f.STATUS == "VIDEO APPT RQSTD" || f.STATUS == "CLINIC APPT CNCLD" || f.STATUS == "CLINIC APPT RQSTD" || f.STATUS == "CLINIC APPT CNFRD"));
            if (temp != null)
            {


                model.REASONFORVISIT = temp.REASONFORVISIT;
                model.APPOINTMENTID = temp.APPOINTMENTID;
                model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME.Value,User.Identity.Name));
                model.APPOINTMENTSTARTTIME = temp.APPOINTMENTSTARTTIME;
                var tempdata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.PATIENTID);
                model.APPOINTMENTFIRSTNAME = tempdata.FIRSTNAME;
                model.APPOINTMENTLASTNAME = tempdata.LASTNAME;
                model.APPOINTMENTSalutation = tempdata.Salutation;

                model.appointment = true;
                model.mode = mode;


            }
            else
            {
                model.appointment = false;
            }
            model.inboxUreadMessageCount = messagecountvalue();
            return View(model);
        }

        [Authorize]
        public ActionResult AppintmentConfirm(DoctorsInformationEditModel model)
        {
            JsonResult result = new JsonResult();
            AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
            string subject = null;
            string appointmentplace = null;
            var data = Convert.ToString(Session["DOCTORID"]);

            var temp = auok.Context.APPOINTMENTS.FirstOrDefault(f => f.APPOINTMENTID == model.APPOINTMENTID);
            if (temp.APPOINTMENTSTARTTIME < DateTime.UtcNow)
            {
                return Json(new { msg = "Error : Appointment Cannot be Confirmed it is Past Date" }, JsonRequestBehavior.AllowGet);
            }
            var tempdata = apUOW.Context.DOCTORSLOTS.Where(f => f.FROMTIME >= temp.APPOINTMENTSTARTTIME && f.TOTIME <= temp.APPOINTMENTENDTIME && f.UserName == temp.DOCTORID);
            try
            {
                if (tempdata != null)
                {
                    foreach (var datavalue in tempdata)
                    {
                        APPOINTMENTSLOT slot = new APPOINTMENTSLOT();
                        slot.APPOINTMENTID = temp.APPOINTMENTID;
                        slot.DOCTORCONFIRMEDSLOTID = datavalue.DOCTORSLOTID;
                        slot.State = State.Added;
                        temp.APPOINTMENTSLOTS.Add(slot);
                    }
                }
                if (temp != null)
                {

                    using (var repo = new AppointmentServiceRepository(auok))
                    {

                        if (temp.STATUS == "VIDEO APPT RQSTD" || temp.STATUS == "VIDEO APPT CNCLD" || temp.STATUS == "VIDEO APPT CNFRD")
                        {
                            temp.STATUS = "VIDEO APPT CNFRD";
                            appointmentplace = "online";
                            subject = "Congratulations! Your online appt. request has been confirmed!";
                        }

                        if (temp.STATUS == "CLINIC APPT RQSTD" || temp.STATUS == "CLINIC APPT CNCLD" || temp.STATUS == "CLINIC APPT CNFRD")
                        {
                            temp.STATUS = "CLINIC APPT CNFRD";
                            appointmentplace = "clinic";
                            subject = "Congratulations! Your clinic appt. request has been confirmed!";
                        }

                        temp.MODIFIEDBY = User.Identity.Name;
                        temp.MODIFIEDON = DateTime.UtcNow;
                        temp.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(temp);
                        auok.Save();

                    }
                }
                var tempvalue = duow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                var tempPateint = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.PATIENTID);
                StringBuilder bodyMsg = new StringBuilder();
                bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                bodyMsg.Append("<div>");
                bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<div style='padding:10px;'>");
                bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear " + tempPateint.FIRSTNAME + " " + tempPateint.LASTNAME + "");
                bodyMsg.Append("</h2>");
                bodyMsg.Append("<div style='float:left;width:490px;'> ");
                bodyMsg.Append("<div>Your request for " + appointmentplace + " appointment  with doctor  on  DATE:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME, User.Identity.Name, 0)) + " " + getAbbreiviation(tempvalue) + " has ");
                bodyMsg.Append("been confirmed. Please be sure to note the appointment date and time.");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<p>The appointments can be cancelled 24hrs before start of the ");
                bodyMsg.Append("Appointment.");
                bodyMsg.Append("</p>   ");
                bodyMsg.Append("<div style='color:#1072B5;margin:10px 0px -10px 0px;'>");
                bodyMsg.Append("<strong>Benefits of signing up:");
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
                bodyMsg.Append("<strong>");
                bodyMsg.Append("See your doctors, anytime, anywhere!");
                bodyMsg.Append("</strong>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<p>If you have questions about doccare online, please email us at");
                bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
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
                bodyMsg.Append("<strong>");
                bodyMsg.Append("Disclaimer:");
                bodyMsg.Append("</strong>");
                bodyMsg.Append(" This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
                bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
                bodyMsg.Append("</strong>");
                bodyMsg.Append("</div> ");
                bodyMsg.Append("</div>");
                //DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", temp.PATIENTID, "", "", subject, bodyMsg.ToString(), false, "", 0);
                using (MessageRepository repo = new MessageRepository(muw))
                {
                    MESSAGE message = new MESSAGE();

                    message.MESSAGESUBJECT = "Appointment Confirmed";
                    message.MESSAGEBODY = "Appointment confirmed from Doctor";
                    message.DateCreated = DateTime.UtcNow;
                    message.SENDMESSAGETO = temp.PATIENTID;
                    message.SENDMESSAGEFROM = temp.DOCTORID;
                    message.SENDBY = temp.DOCTORID;
                    message.MESSAGESTATUSFROM = "ACTIVE";
                    message.MESSAGESTATUSTO = "ACTIVE";
                    message.MESSAGETYPE = "ACCEPT_APPOINTMENT";
                    message.MESSAGESTATUS = "UnRead";
                    message.MODIFIEDON = DateTime.UtcNow;
                    message.SENDDATE = DateTime.UtcNow;
                    message.State = DOCVIDEO.ObjectState.State.Added;
                    repo.InsertOrUpdate(message);
                    muw.Save();

                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }

            model.inboxUreadMessageCount = messagecountvalue();
            return Json(new { msg = "Appointment Confirmed" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult AppintmentCanceled(DoctorsInformationEditModel model)
        {
            JsonResult result = new JsonResult();
            string subject = null;
            AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
            try
            {

                var temp = auok.Context.APPOINTMENTS.FirstOrDefault(f => f.APPOINTMENTID == model.APPOINTMENTID);
                var tempdata = apUOW.Context.APPOINTMENTSLOTS.Where(f => f.APPOINTMENTID == model.APPOINTMENTID).ToList();
                if (tempdata.Count > 0)
                {
                    foreach (var datavalue in tempdata)
                    {
                        ConfirmedAppointmentSlotsUnitOfWork cauow = new ConfirmedAppointmentSlotsUnitOfWork();
                        ConfirmedAppointmentSlotsServiceRepository repo = new ConfirmedAppointmentSlotsServiceRepository(cauow);

                        int temps = Convert.ToInt32(datavalue.APPOINTMENTSLOTID);

                        repo.Delete(temps);
                        cauow.Save();
                        repo = null;
                        cauow = null;



                    }
                }
                if (temp != null)
                {
                    using (var repo = new AppointmentServiceRepository(auok))
                    {
                        if (temp.STATUS == "VIDEO APPT RQSTD" || temp.STATUS == "VIDEO APPT CNCLD" || temp.STATUS == "VIDEO APPT CNFRD")
                        {
                            temp.STATUS = "VIDEO APPT CNCLD";
                            subject = "You have an important message from doccare online regarding your Online Appointment";
                        }

                        if (temp.STATUS == "CLINIC APPT RQSTD" || temp.STATUS == "CLINIC APPT CNCLD" || temp.STATUS == "CLINIC APPT CNFRD")
                        {
                            temp.STATUS = "CLINIC APPT CNCLD";
                            subject = "You have an important message from doccare online regarding your Clinic Appointment";
                        }

                        temp.MODIFIEDBY = User.Identity.Name;
                        temp.MODIFIEDON = DateTime.UtcNow;
                        temp.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(temp);
                        auok.Save();

                    }
                }
                var tempvalue = duow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
                var tempPateint = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.PATIENTID);
               /* StringBuilder bodyMsg = new StringBuilder();

                bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                bodyMsg.Append("<div>");
                bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo'/>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<div style='padding:10px;'>");
                bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear " + tempPateint.FIRSTNAME + " " + tempPateint.LASTNAME + "");
                bodyMsg.Append("</h2>");
                bodyMsg.Append("<div style='float:left;width:490px;'> ");
                bodyMsg.Append("<div>We regret to inform, your doctor has cancelled the online appointment ");
                bodyMsg.Append("for Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME, User.Identity.Name, 0)) + " " + getAbbreiviation(tempvalue) + " due to unforeseen reasons..");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<p>Please ");
                bodyMsg.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn 'title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");
                bodyMsg.Append("</a>  to login to your doccare account to rebook your ");
                bodyMsg.Append("Appointment or find another doctor");
                bodyMsg.Append("</p>");
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
                bodyMsg.Append("<strong>See your patient , anytime, anywhere!");
                bodyMsg.Append("</strong>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<p>If you have questions about doccare online, please email us at");
                bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
                bodyMsg.Append("</a> For FAQ, please visit ");
                bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>  www.PROFESSORSONLINE.com/faq.");
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
                DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", temp.PATIENTID, "", "", subject, bodyMsg.ToString(), false, "", 0);
                */
                using (MessageRepository repo = new MessageRepository(muw))
                {
                    MESSAGE message = new MESSAGE();

                    message.MESSAGESUBJECT = "Appointment Cancelled  " + model.APPOINTMENTSTARTTIMEACTUAL;
                    message.MESSAGEBODY = "Appointment Cancelled from Doctor Due to reason" + model.REASONTOCANCEL;
                    message.DateCreated = DateTime.UtcNow;
                    message.SENDMESSAGETO = temp.PATIENTID;
                    message.SENDMESSAGEFROM = temp.DOCTORID;
                    message.SENDBY = temp.DOCTORID;
                    message.MESSAGESTATUSFROM = "ACTIVE";
                    message.MESSAGESTATUSTO = "ACTIVE";
                    message.MESSAGETYPE = "REJECT_APPOINTMENT";
                    message.MESSAGESTATUS = "UnRead";
                    message.MODIFIEDON = DateTime.UtcNow;
                    message.SENDDATE = DateTime.UtcNow;
                    message.State = DOCVIDEO.ObjectState.State.Added;
                    repo.InsertOrUpdate(message);
                    muw.Save();

                }

                if (asUOW.Context.CancelledAppointments.Where(f => f.APPOINTMENTID == model.APPOINTMENTID).Count() == 0)
                {
                    using (DoctorAppointmentServiceRepository repo1 = new DoctorAppointmentServiceRepository(asUOW))
                    {
                        CANCELLEDAPPOINTMENT cana = new CANCELLEDAPPOINTMENT();


                        cana.DateCreated = DateTime.UtcNow;
                        cana.MODIFIEDON = DateTime.UtcNow;
                        cana.MODIFIEDBY = User.Identity.Name;
                        cana.CREATEDBY = User.Identity.Name;
                        cana.REASONTOCANCEL = model.REASONTOCANCEL;
                        cana.CANCELDATE = DateTime.UtcNow;
                        cana.APPOINTMENTID = model.APPOINTMENTID;
                        cana.State = DOCVIDEO.ObjectState.State.Added;
                        repo1.InsertOrUpdate(cana);
                        asUOW.Save();

                    }
                }
                else
                {
                    using (DoctorAppointmentServiceRepository repo1 = new DoctorAppointmentServiceRepository(asUOW))
                    {
                        CANCELLEDAPPOINTMENT cana = asUOW.Context.CancelledAppointments.FirstOrDefault(f => f.APPOINTMENTID == model.APPOINTMENTID);


                        cana.DateCreated = DateTime.UtcNow;
                        cana.MODIFIEDON = DateTime.UtcNow;
                        cana.MODIFIEDBY = User.Identity.Name;
                        cana.CREATEDBY = User.Identity.Name;
                        cana.REASONTOCANCEL = model.REASONTOCANCEL;
                        cana.CANCELDATE = DateTime.UtcNow;
                        cana.APPOINTMENTID = model.APPOINTMENTID;
                        cana.State = DOCVIDEO.ObjectState.State.Modified;
                        repo1.InsertOrUpdate(cana);
                        asUOW.Save();

                    }
                }

            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }

            model.inboxUreadMessageCount = messagecountvalue();
            return Json(new { msg = "Appointment Cancelled" }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Authorize]
        public void _DoctorsPayRatesEdit(string id)
        {

        }

        [HttpGet]
        [Authorize]
        public void _DoctorsPayRatesDisplay(string id)
        {

        }



        [HttpGet]
        [Authorize]
        public void _DoctorsInfoEdit(string id)
        {


        }

        [HttpGet]
        [Authorize]
        public void _DoctorsNpiEdit(string id)
        {


        }


        [HttpGet]
        [Authorize]
        public void _DoctorseduEdit(string id)
        {


        }

        [HttpGet]
        [Authorize]
        public void _DoctorscontactEdit(string id)
        {


        }




        public ActionResult Save(IEnumerable<HttpPostedFileBase> UserImageUpload, string UserName)
        {
            Users = UserName;
            StringBuilder fileNames = new StringBuilder();
            try
            {
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(UserName));

                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(UserName));
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


                        var physicalPath = Path.Combine(Server.MapPath("/DoctorInformation/" + UserName), fileName);
                        fileNames.Append("/DoctorInformation/" + UserName + "/" + fileName);
                        file.SaveAs(physicalPath);

                        UserProfilePic = fileNames.ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }



            return Json(new { status = fileNames.ToString(), type = "save" }, "text/plain");

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
                        var temp = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
                        UserProfilePic = temp.USERPHOTOFILEPATH;

                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }

            //  Return an empty string to signify success
            return Json(new { status = files.ToString(), type = "remove" }, "text/plain");
        }

        public ActionResult ClinicImageSave(IEnumerable<HttpPostedFileBase> UserClinicImageUpload, string UserName)
        {
            Users = UserName;
            StringBuilder fileNames = new StringBuilder();
            try
            {
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(UserName));

                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(UserName));
                }
                // The Name of the Upload component is "files"
                if (UserClinicImageUpload != null)
                {
                    foreach (var file in UserClinicImageUpload)
                    {
                        //   Some browsers send file names with full path.
                        //   We are only interested in the file name.
                        var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        var fileNamewithoutExt = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                        var fileName = Path.GetFileName(fileNamewithoutExt + "_" + DateTime.UtcNow.ToString("MMddyyyyhhmmss") + ext);


                        var physicalPath = Path.Combine(Server.MapPath("/DoctorInformation/" + UserName), fileName);
                        fileNames.Append("/DoctorInformation/" + UserName + "/" + fileName);
                        file.SaveAs(physicalPath);
                        imagepath = fileNames.ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }



            return Json(new { status = fileNames.ToString(), type = "save" }, "text/plain");

        }

        public ActionResult ClinicImageRemove(string[] fileNames)
        {
            //The parameter of the Remove action must be called "fileNames"
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            var temp = uw.Context.DoctorsWorkInstitutions.Where(f => f.UserName == User.Identity.Name).OrderByDescending(f => f.DateCreated).FirstOrDefault();
            imagepath = temp.INSTITUTIONIMAGEPATH;
            // Return an empty string to signify success
            return Json(new { status = files.ToString(), type = "remove" }, "text/plain");
        }




        [HttpPost]
        [Authorize]
        public JsonResult _DoctorsInfoEdit(DoctorsInformationEditModel updatemodel)
        {
            USERSINFORMATION user = null;
            DOCTORSINFORMATION docinfo = null;
            DoctorInformationUnitOfWork diuow = new DoctorInformationUnitOfWork();
            DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
            user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
            docinfo = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);




            try
            {
                if (updatemodel.FIRSTNAME != null && updatemodel.Salutation != null && updatemodel.LASTNAME != null && updatemodel.GENDER != null && updatemodel.GETSUFFIX != null)
                {
                    if (updatemodel.DOBYEAR == 0 || updatemodel.DOBMonth == null || updatemodel.DOBDAY == 0)
                    {
                        message = "Date fields are required";
                        Session["Message"] = "Date fields are required";
                        return Json(new { msg = "Date fields are required" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        DateTime tempdate = new DateTime(Convert.ToInt32(updatemodel.DOBYEAR), Convert.ToInt32(updatemodel.DOBMonth), Convert.ToInt32(updatemodel.DOBDAY));

                        if (updatemodel.PRACTISESINCE > DateTime.UtcNow)
                        {
                            message = "Error : Practise since date is invalid ";
                            Session["Message"] = "Error : Practise since date is invalid ";
                            return Json(new { msg = "Error : Practise since date is invalid " }, JsonRequestBehavior.AllowGet);
                        }

                        user.Salutation = updatemodel.Salutation;
                        user.FIRSTNAME = updatemodel.FIRSTNAME;
                        user.LASTNAME = updatemodel.LASTNAME;
                        user.GENDER = updatemodel.GENDER;
                        updatemodel.DOB = tempdate;
                        user.DOB = updatemodel.DOB;
                        user.PASSWORDHASH = user.PASSWORDHASH;
                        user.PASSWORDSALT = user.PASSWORDSALT;
                        user.DateLastActivity = DateTime.UtcNow;
                        user.DateLastLogin = DateTime.UtcNow;



                        user.IsApproved = true;
                        user.MODIFIEDBY = User.Identity.Name;
                        user.MODIFIEDON = DateTime.UtcNow;
                        if (UserProfilePic != null)
                        {
                            user.USERPHOTOFILEPATH = UserProfilePic;
                        }
                        string temp = null;
                        if (updatemodel.GETSUFFIX != null)
                        {
                            foreach (var data in updatemodel.GETSUFFIX)
                            {
                                if (temp == null)
                                {
                                    temp = data;
                                }
                                else
                                {
                                    temp = temp + " , " + data;
                                }
                            }
                            docinfo.SUFFIX = temp;
                        }



                        docinfo.State = State.Modified;
                        user.DoctorInformations.Add(docinfo);
                        updatemodel.MessageUpdateStatus = "Doctors Details Updated";
                        message = "Doctors Details Updated";

                        Session["Message"] = "Doctors Details Updated";



                        if (updatemodel.Languages != null)
                        {
                            using (UserLanguageServiceRepository repo = new UserLanguageServiceRepository(luow))
                            {

                                repo.Delete(updatemodel.UserName);
                                luow.Save();
                            }
                            for (int i = 0; i < updatemodel.Languages.Length; i++)
                            {
                                UserLanguageUnitOfWork luow2 = new UserLanguageUnitOfWork();
                                UserLanguageServiceRepository repo3 = new UserLanguageServiceRepository(luow2);


                                USERSLANGUAGE UserLanguages = new USERSLANGUAGE();
                                UserLanguages.CREATEDBY = User.Identity.Name;
                                UserLanguages.MODIFIEDON = DateTime.UtcNow;
                                UserLanguages.DateCreated = DateTime.UtcNow;
                                UserLanguages.MODIFIEDBY = User.Identity.Name;
                                UserLanguages.State = State.Added;
                                UserLanguages.UserName = updatemodel.UserName;
                                UserLanguages.LANGUAGEKEYID = updatemodel.Languages[i];
                                repo3.InsertOrUpdate(UserLanguages);
                                luow2.Save();
                                luow2 = null;
                                repo3 = null;

                            }
                        }





                        if (updatemodel.Specialities != null)
                        {
                            temp = null;
                            foreach (var data in updatemodel.Specialities)
                            {
                                if (temp == null)
                                {
                                    temp = data;
                                }
                                else
                                {
                                    temp = temp + " , " + data;
                                }
                            }

                            var datas = suow.Context.DoctorsSpecialities.FirstOrDefault(f => f.UserName == updatemodel.UserName);
                            if (datas == null)
                            {

                                SpecialityUnitOfWork suow2 = new SpecialityUnitOfWork();
                                DoctorSpecialitiesServiceRepository repo5 = new DoctorSpecialitiesServiceRepository(suow2);


                                DOCTORSPECIALITY speciality = new DOCTORSPECIALITY();
                                speciality.CREATEDBY = User.Identity.Name;
                                speciality.MODIFIEDON = DateTime.UtcNow;
                                speciality.DateCreated = DateTime.UtcNow;
                                speciality.MODIFIEDBY = User.Identity.Name;
                                speciality.State = State.Added;
                                speciality.UserName = updatemodel.UserName;
                                speciality.SPECIALITY = temp;
                                repo5.InsertOrUpdate(speciality);
                                suow2.Save();
                                suow2 = null;
                                repo5 = null;

                            }
                            else
                            {
                                using (DoctorSpecialitiesServiceRepository repo2 = new DoctorSpecialitiesServiceRepository(suow))
                                {
                                    datas.MODIFIEDON = DateTime.UtcNow;
                                    datas.MODIFIEDBY = User.Identity.Name;
                                    datas.State = State.Modified;
                                    datas.SPECIALITY = temp;
                                    repo2.InsertOrUpdate(datas);
                                    suow.Save();
                                }
                            }
                        }
                        using (UserRepository repo4 = new UserRepository(uow))
                        {
                            user.State = State.Modified;
                            repo4.InsertOrUpdateGraph(user);
                            uow.Save();
                        }
                    }
                }
                else
                {
                    if (updatemodel.FIRSTNAME == null)
                    {
                        Session["Message"] = "Error :First Name Required";
                        return Json(new { msg = "Error :First Name Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.Salutation == null)
                    {
                        Session["Message"] = "Error :Salutation Required";
                        return Json(new { msg = "Salutation Required" }, JsonRequestBehavior.AllowGet);
                    } if (updatemodel.LASTNAME == null)
                    {
                        Session["Message"] = "Error :Last Name Required";
                        return Json(new { msg = "Error :Last Name Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.GENDER == null)
                    {
                        Session["Message"] = "Error :Gender Required";
                        return Json(new { msg = "Error :Gender Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.GETSUFFIX == null)
                    {
                        Session["Message"] = "Error :Suffix Required";
                        return Json(new { msg = "Error :Suffix Required" }, JsonRequestBehavior.AllowGet);
                    }


                }
            }
            catch (Exception ex)
            {
                Session["Message"] = "Please Retry Server Error";
                return Json(new { msg = "Please Retry Server Error" }, JsonRequestBehavior.AllowGet);
            }
            Session["Message"] = "Doctors Details Updated";
            return Json(new { msg = "Doctors Details Updated" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize]
        public JsonResult _DoctorscontactEdit(DoctorsInformationEditModel updatemodel)
        {
            USERSINFORMATION user = null;


            user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);

            try
            {
                if (updatemodel.USERSTREETADDRESS1 != null && updatemodel.CITY != null && updatemodel.USERSTATE != null && updatemodel.ZIPCODE != null)
                {

                    user.USERSTREETADDRESS1 = updatemodel.USERSTREETADDRESS1;
                    user.USERSTREETADDRESS2 = updatemodel.USERSTREETADDRESS2;
                    user.CITY = updatemodel.CITY;
                    user.USERSTATE = updatemodel.USERSTATE;
                    user.ZIPCODE = updatemodel.ZIPCODE;
                    user.PASSWORDHASH = user.PASSWORDHASH;
                    user.PASSWORDSALT = user.PASSWORDSALT;
                    user.DateLastActivity = DateTime.UtcNow;
                    user.DateLastLogin = DateTime.UtcNow;
                    if (updatemodel.CurrenttimeZone != null)
                        user.CurrenttimeZone = updatemodel.CurrenttimeZone;
                    user.DateLastPasswordChange = DateTime.UtcNow;

                    user.IsApproved = true;
                    user.MODIFIEDBY = User.Identity.Name;
                    user.MODIFIEDON = DateTime.UtcNow;

                    updatemodel.MessageUpdateStatus = "Doctors Details Updated";
                    message = "Doctors Details Updated";
                    using (UserRepository repo4 = new UserRepository(uow))
                    {
                        user.State = State.Modified;
                        repo4.InsertOrUpdateGraph(user);
                        uow.Save();
                    }
                }
                else
                {
                    if (updatemodel.USERSTREETADDRESS1 == null)
                    {
                        Session["Message"] = "Error :Street Address Required";
                        return Json(new { msg = "Error :Street Address Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.CITY == null)
                    {
                        Session["Message"] = "Error :City Required";
                        return Json(new { msg = "Error :City Required" }, JsonRequestBehavior.AllowGet);
                    } if (updatemodel.USERSTATE == null)
                    {
                        Session["Message"] = "Error :User State Required";
                        return Json(new { msg = "Error :user State Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.ZIPCODE == null)
                    {
                        Session["Message"] = "Error :Zipcode Required";
                        return Json(new { msg = "Error :Zipcode Required" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                Session["Message"] = "Error : Server please retry";
                return Json(new { msg = "Error : Server please retry" }, JsonRequestBehavior.AllowGet);
            }

            Session["Message"] = "Doctors Details Updated";
            return Json(new { msg = "Doctors Details Updated" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize]
        public JsonResult _DoctorseduEdit(DoctorsInformationEditModel updatemodel)
        {
            DOCTORSINFORMATION docinfo = null;


            docinfo = diuow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);


            try
            {
                if (updatemodel.EDUCATIONALQUAL != null && updatemodel.AWARDCERTIFICATION != null && updatemodel.PROFESSIONALMEMBERSHIP != null && updatemodel.CERTIFICATIONBOARDS != null && updatemodel.HOSPITALAFFILIATION != null && updatemodel.ABOUTME != null)
                {


                    string temp = null;


                    docinfo.EDUCATIONALQUAL = updatemodel.EDUCATIONALQUAL;
                    docinfo.AWARDCERTIFICATION = updatemodel.AWARDCERTIFICATION;
                    docinfo.PROFESSIONALMEMBERSHIP = updatemodel.PROFESSIONALMEMBERSHIP;
                    if (updatemodel.CERTIFICATIONBOARDS != null)
                    {
                        foreach (var data in updatemodel.CERTIFICATIONBOARDS)
                        {
                            if (temp == null)
                            {
                                temp = data;
                            }
                            else
                            {
                                temp = temp + " , " + data;
                            }
                        }
                        docinfo.CERTIFICATIONBOARD = temp;
                    }

                    docinfo.HOSPITALAFFILIATION = updatemodel.HOSPITALAFFILIATION;
                    docinfo.MODIFIEDBY = User.Identity.Name;
                    docinfo.MODIFIEDON = DateTime.UtcNow;
                    docinfo.State = State.Modified;
                    docinfo.ABOUTME = updatemodel.ABOUTME;
                    updatemodel.MessageUpdateStatus = "Doctors Details Updated";
                    message = "Doctors Details Updated";
                    using (DoctorInformationServiceRepository repo4 = new DoctorInformationServiceRepository(diuow))
                    {
                        docinfo.State = State.Modified;
                        repo4.InsertOrUpdate(docinfo);
                        diuow.Save();
                    }
                }
                else
                {

                    if (updatemodel.EDUCATIONALQUAL == null)
                    {
                        Session["Message"] = "Error :Educational Qualification Required";
                        return Json(new { msg = "Error :Educational Qualification Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.AWARDCERTIFICATION == null)
                    {
                        Session["Message"] = "Error :Award Certification Required";
                        return Json(new { msg = "Error :Award Certification Required" }, JsonRequestBehavior.AllowGet);
                    } if (updatemodel.PROFESSIONALMEMBERSHIP == null)
                    {
                        Session["Message"] = "Error :Professional Membership Required";
                        return Json(new { msg = "Error :Professional Membership Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.CERTIFICATIONBOARDS == null)
                    {
                        Session["Message"] = "Error :Certification Required";
                        return Json(new { msg = "Error :Certification Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.HOSPITALAFFILIATION == null)
                    {
                        Session["Message"] = "Error :Hospital Affiliation Required";
                        return Json(new { msg = "Error :Hospital Affiliation" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.ABOUTME == null)
                    {
                        Session["Message"] = "Error :About Me Required";
                        return Json(new { msg = "Error :About Me Required" }, JsonRequestBehavior.AllowGet);
                    }


                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            Session["Message"] = "Doctors Details Updated";
            return Json(new { msg = "Doctors Details Updated" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize]
        public JsonResult _DoctorsNpiEdit(DoctorsInformationEditModel updatemodel)
        {

            DOCTORSINFORMATION docinfo = null;
            DoctorInformationUnitOfWork diuow = new DoctorInformationUnitOfWork();

            docinfo = diuow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);


            try
            {
                if (updatemodel.LICENSE != null && updatemodel.LICENSEEXPIRESON != null && updatemodel.PRACTISESINCE != null && updatemodel.LICENSEIN != null)
                {


                    docinfo.LICENSE = updatemodel.LICENSE;
                    docinfo.LICENSEEXPIRESON = updatemodel.LICENSEEXPIRESON;
                    docinfo.PRACTISESINCE = updatemodel.PRACTISESINCE;
                    docinfo.LICENSEDIN = updatemodel.LICENSEIN;
                    docinfo.STATELICENSE = updatemodel.LICENSEIN;
                    docinfo.MODIFIEDBY = User.Identity.Name;
                    docinfo.MODIFIEDON = DateTime.UtcNow;
                    docinfo.State = State.Modified;
                    updatemodel.MessageUpdateStatus = "Doctors Details Updated";
                    message = "Doctors Details Updated";

                    using (DoctorInformationServiceRepository repo4 = new DoctorInformationServiceRepository(diuow))
                    {
                        docinfo.State = State.Modified;
                        repo4.InsertOrUpdate(docinfo);
                        diuow.Save();
                    }
                }
                else
                {
                    if (updatemodel.LICENSE == null)
                    {
                        Session["Message"] = "Error :NPI Required";
                        return Json(new { msg = "Error :NPI Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.LICENSEEXPIRESON == null)
                    {
                        Session["Message"] = "Error :NPI Experience Date Required";
                        return Json(new { msg = "Error :NPI Experience Date Required" }, JsonRequestBehavior.AllowGet);
                    } if (updatemodel.PRACTISESINCE == null)
                    {
                        Session["Message"] = "Error :Practice Since Required";
                        return Json(new { msg = "Error :Practice Since Required" }, JsonRequestBehavior.AllowGet);
                    }
                    if (updatemodel.LICENSEIN == null)
                    {
                        Session["Message"] = "Error :NPI Date Required";
                        return Json(new { msg = "Error :NPI Date Required" }, JsonRequestBehavior.AllowGet);
                    }



                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }


            Session["Message"] = "Doctors Details Updated";
            return Json(new { msg = "Doctors Details Updated" }, JsonRequestBehavior.AllowGet);

        }




        [HttpGet]
        [Authorize]
        public void _DoctorsInfoClinicPhotosDisplay(string id)
        {


        }

        [HttpGet]
        [Authorize]
        public ActionResult _DoctorsInfoDisplay(string id)
        {
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult _DoctorsNpiDisplay(string id)
        {
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult _DoctorseduDisplay(string id)
        {
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult _DoctorscontactDisplay(string id)
        {
            DoctorsInformationEditModel model = DoctorDisplayData(id);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public void _DoctorsInfoDisplayCustom(string id)
        {

        }


        public JsonResult CheckClinicExistingSlots()
        {
            try
            {
                var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == User.Identity.Name && w.AVAILABILITYMODE == "C").Any();
                if (existingSlots)
                {
                    return Json(new { msg = "Doctor slot exists in Clinic" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "notexists" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckExistingSlots()
        {
            try
            {
                var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == User.Identity.Name && w.AVAILABILITYMODE == "V").Any();
                if (existingSlots)
                {
                    return Json(new { msg = "Doctor slot exists in Video" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "notexists" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetDoctorsSlotstest(List<DOCTORSLOT> doctorSlots)
        {
            try
            {
                var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == User.Identity.Name && w.AVAILABILITYMODE == "V").Any();
                if (existingSlots)
                {

                    //Delete the current slots for the particular doctor
                    //using (var delRepo = new AppointmentSlotsServiceRepository(apUOW))
                    //{
                    //    delRepo.Delete(User.Identity.Name, "V");
                    //    apUOW.Save();
                    //}

                }
                if (doctorSlots.Count > 0)
                {


                    doctorSlots.ForEach(f =>
                    {
                        f.CREATEDBY = User.Identity.Name; f.DateCreated = DateTime.UtcNow; f.MODIFIEDBY = User.Identity.Name; f.MODIFIEDON = DateTime.UtcNow;
                        f.State = State.Added; f.UserName = User.Identity.Name;
                        f.FROMTIME = GetUTCtime(f.FROMTIME, f.DSTObserved, f.TimeZoneOffset);
                        f.TOTIME = GetUTCtime(f.TOTIME, f.DSTObserved, f.TimeZoneOffset);
                    });

                    foreach (var slot in doctorSlots)
                    {
                        if (!ValidateSlot(slot.FROMTIME, slot.TOTIME, "C"))
                        {
                            if (!ValidateSlot(slot.FROMTIME, slot.TOTIME, "V"))
                            {
                                using (var asUOW = new AppointmentSlotsUnitOfWork())
                                {
                                    using (var repo = new AppointmentSlotsServiceRepository(asUOW))
                                    {

                                        repo.InsertOrUpdate(slot);
                                        asUOW.Save();

                                    }
                                }
                            }
                        }
                        else
                        {
                            return Json(new { msg = "Slot Already Exists in Clinic Appointments" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
            }
            catch (Exception)
            {

                return Json(new { msg = "Failure " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Succesfully Created" }, JsonRequestBehavior.AllowGet);
        }

        public bool ValidateSlot(DateTime start, DateTime end, string type)
        {
            DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork apuow = new DOCVIDEO.UserServiceRepoUOW.AppointmentsUnitOfWork();

            var exist = apuow.Context.DOCTORSLOTS.Where(w => w.UserName == User.Identity.Name && w.FROMTIME == start && w.TOTIME == end && w.AVAILABILITYMODE == type).Any();


            return exist;
        }

        public JsonResult SetDoctorsClinicSlots(List<DOCTORSLOT> doctorSlots)
        {
            try
            {

                var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == User.Identity.Name && w.AVAILABILITYMODE == "C").Any();
                if (existingSlots)
                {
                    //Delete the current slots for the particular doctor
                    //using (var delRepo = new AppointmentSlotsServiceRepository(apUOW))
                    //{
                    //    delRepo.Delete(User.Identity.Name, "C");
                    //    apUOW.Save();
                    //}                       
                }

                if (doctorSlots.Count > 0)
                {
                    doctorSlots.ForEach(f =>
                    {
                        f.CREATEDBY = User.Identity.Name; f.DateCreated = DateTime.UtcNow; f.MODIFIEDBY = User.Identity.Name; f.MODIFIEDON = DateTime.UtcNow;
                        f.State = State.Added; f.UserName = User.Identity.Name;
                        f.FROMTIME = GetUTCtime(f.FROMTIME, f.DSTObserved, f.TimeZoneOffset);
                        f.TOTIME = GetUTCtime(f.TOTIME, f.DSTObserved, f.TimeZoneOffset);
                    });

                    foreach (var slot in doctorSlots)
                    {
                        if (!ValidateSlot(slot.FROMTIME, slot.TOTIME, "V"))
                        {
                            if (!ValidateSlot(slot.FROMTIME, slot.TOTIME, "C"))
                            {
                                using (var asUOW = new AppointmentSlotsUnitOfWork())
                                {
                                    using (var repo = new AppointmentSlotsServiceRepository(asUOW))
                                    {

                                        repo.InsertOrUpdate(slot);
                                        asUOW.Save();

                                    }
                                }
                            }
                        }
                        else
                        {
                            return Json(new { msg = "Slot Already Exists in Video Appointments" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception)
            {

                return Json(new { msg = "Failure " }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Succesfully Created" }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetExistingDoctorClinicSlots(DOCTORSLOT doctorSlot)
        {
            JsonResult result = new JsonResult();
            try
            {
                string id = Convert.ToString(Session["DoctorId"]);
                if (id != "")
                {
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    apUOW.Context.Configuration.LazyLoadingEnabled = false;
                    var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == id && w.AVAILABILITYMODE == "C").ToList();
                    //existingSlots.ForEach(f =>
                    //{

                    //    f.FROMTIME = GetLocalTime(f.FROMTIME, f.UserName);
                    //    f.TOTIME = GetLocalTime(f.TOTIME, f.UserName);
                    //});
                    var takenSlots = apUOW.Context.DOCTORSLOTS.Join(apUOW.Context.APPOINTMENTS, d => d.DOCTORSLOTID, a => a.DOCTORSLOTID,
                         (d, a) => new { d.DOCTORSLOTID, d.AVAILABILITYMODE }).Where(w => w.AVAILABILITYMODE == "C").ToList();

                    for (var i = 0; i < existingSlots.Count; i++)
                    {
                        existingSlots[i].FROMTIME = GetLocalTime(existingSlots[i].FROMTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                        existingSlots[i].TOTIME = GetLocalTime(existingSlots[i].TOTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                        for (var j = 0; j < takenSlots.Count; j++)
                        {
                            if (existingSlots[i].DOCTORSLOTID == takenSlots[j].DOCTORSLOTID)
                            {
                                existingSlots[i].STATUS = "true";
                            }
                        }
                    }
                    result.Data = existingSlots;

                }
                else
                {
                    return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }
        [AllowAnonymous]
        public JsonResult GetExistingDoctorSlots(DOCTORSLOT doctorSlot)
        {
            JsonResult result = new JsonResult();
            try
            {
                string id = Convert.ToString(Session["DoctorId"]);
                if (id != "")
                {
                    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    apUOW.Context.Configuration.LazyLoadingEnabled = false;

                    var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == id && w.AVAILABILITYMODE == "V").ToList();

                    var takenSlots = apUOW.Context.DOCTORSLOTS.Join(apUOW.Context.APPOINTMENTS, d => d.DOCTORSLOTID, a => a.DOCTORSLOTID,
                        (d, a) => new { d.DOCTORSLOTID, d.AVAILABILITYMODE }).Where(w => w.AVAILABILITYMODE == "V").ToList();

                    for (var i = 0; i < existingSlots.Count; i++)
                    {
                        existingSlots[i].FROMTIME = GetLocalTime(existingSlots[i].FROMTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                        existingSlots[i].TOTIME = GetLocalTime(existingSlots[i].TOTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                        for (var j = 0; j < takenSlots.Count; j++)
                        {
                            if (existingSlots[i].DOCTORSLOTID == takenSlots[j].DOCTORSLOTID)
                            {
                                existingSlots[i].STATUS = "true";
                            }
                        }
                    }

                    result.Data = existingSlots;

                }
                else
                {
                    return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        public JsonResult GetClinicExistingSlots(DOCTORSLOT doctorSlot)
        {
            JsonResult result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                apUOW.Context.Configuration.LazyLoadingEnabled = false;
                var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == User.Identity.Name && w.AVAILABILITYMODE == "C").ToList();
                //existingSlots.ForEach(f =>
                //{

                //    f.FROMTIME = GetLocalTime(f.FROMTIME);
                //    f.TOTIME = GetLocalTime(f.TOTIME);
                //});
                var takenSlots = apUOW.Context.DOCTORSLOTS.Join(apUOW.Context.APPOINTMENTS, d => d.DOCTORSLOTID, a => a.DOCTORSLOTID,
                        (d, a) => new { d.DOCTORSLOTID, d.AVAILABILITYMODE }).Where(w => w.AVAILABILITYMODE == "C").ToList();

                for (var i = 0; i < existingSlots.Count; i++)
                {
                    existingSlots[i].FROMTIME = GetLocalTime(existingSlots[i].FROMTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                    existingSlots[i].TOTIME = GetLocalTime(existingSlots[i].TOTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                    for (var j = 0; j < takenSlots.Count; j++)
                    {
                        if (existingSlots[i].DOCTORSLOTID == takenSlots[j].DOCTORSLOTID)
                        {
                            existingSlots[i].STATUS = "true";
                        }
                    }
                }

                result.Data = existingSlots;
            }
            catch (Exception)
            {
                return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }
        public JsonResult GetExistingSlots(DOCTORSLOT doctorSlot)
        {
            JsonResult result = new JsonResult();
            try
            {
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                apUOW.Context.Configuration.LazyLoadingEnabled = false;
                var existingSlots = apUOW.Context.DOCTORSLOTS.Where(w => w.UserName == User.Identity.Name && w.AVAILABILITYMODE == "V").ToList();
                var takenSlots = apUOW.Context.DOCTORSLOTS.Join(apUOW.Context.APPOINTMENTS, d => d.DOCTORSLOTID, a => a.DOCTORSLOTID,
                        (d, a) => new { d.DOCTORSLOTID, d.AVAILABILITYMODE }).Where(w => w.AVAILABILITYMODE == "V").ToList();

                for (var i = 0; i < existingSlots.Count; i++)
                {
                    existingSlots[i].FROMTIME = GetLocalTime(existingSlots[i].FROMTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                    existingSlots[i].TOTIME = GetLocalTime(existingSlots[i].TOTIME, existingSlots[i].UserName, (doctorSlot.TimeZoneOffset + doctorSlot.DSTObserved));
                    for (var j = 0; j < takenSlots.Count; j++)
                    {
                        if (existingSlots[i].DOCTORSLOTID == takenSlots[j].DOCTORSLOTID)
                        {
                            existingSlots[i].STATUS = "true";
                        }
                    }
                }
                //existingSlots.ForEach(f =>
                //{

                //    f.FROMTIME = GetLocalTime(f.FROMTIME);
                //    f.TOTIME = GetLocalTime(f.TOTIME);
                //});
                result.Data = existingSlots;
            }
            catch (Exception)
            {
                return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateAppointments(int id)
        {

            return View();

        }

        private string GetVideoSessionId(out string token)
        {
            try
            {
                OpenTok.OpenTokSDK openTok = new OpenTok.OpenTokSDK();
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add(SessionPropertyConstants.P2P_PREFERENCE, "enabled");
                string ipAdd = Request.ServerVariables["REMOTE_ADDR"];
                //string ipAdd2 = Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
                string session_id = openTok.CreateSession(ipAdd);
                //string token = openTok.GenerateToken(session_id);
                token = openTok.GenerateToken(session_id);

                return session_id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

            }
           
        }


        private void SetSession(out string token, string sessionID)
        {
            OpenTok.OpenTokSDK openTok = new OpenTok.OpenTokSDK();
            token = openTok.GenerateToken(sessionID);
        }


        public ActionResult VideoConf()
        {
            Session["PateintID"] = "";
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.FIRSTNAME = LoginFirstName;
            temp.LASTNAME = LoginLastName;
            temp.USERPHOTOFILEPATH = profilepath;
            temp.inboxUreadMessageCount = messagecount;
            var tempUsertype = appointmentUOW.Context.UserInformations.FirstOrDefault(w => (w.UserName == User.Identity.Name));
            if (tempUsertype.USERTYPE == "D")
            {
                var tempinner = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.Now && w.APPOINTMENTENDTIME >= DateTime.Now) && w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                if (tempinner != null)
                {
                    if (tempinner.Count == 0)
                    {
                        return RedirectToAction("VideoConfMain");
                    }
                    foreach (var t in tempinner)
                    {
                        if (t.APPOINTMENTENDTIME.Value.AddSeconds(-1) >= DateTime.UtcNow)
                        {
                            int totalMinutes = Convert.ToInt32((t.APPOINTMENTENDTIME.Value.Subtract(t.APPOINTMENTSTARTTIME.Value)).TotalMinutes);
                            var docdata = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == t.DOCTORID && f.DURATION == 15);
                            if (t != null)
                            {
                                Session["PateintID"] = t.PATIENTID;

                                temp.uploadreport = t.uploadreport;
                                temp.APPOINTMENTID = t.APPOINTMENTID;

                            }

                            break;
                        }

                    }

                }

                //  var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD" && w.termsOfUseDoctor == true && w.releaseStatementDoctor == true);

                //  var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => w.APPOINTMENTID == 151);

            }
            else if (tempUsertype.USERTYPE == "P")
            {

                var tempinner = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.Now && w.APPOINTMENTENDTIME >= DateTime.Now) && w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                if (tempinner != null)
                {
                    if (tempinner.Count == 0)
                    {
                        return RedirectToAction("VideoConfMain");
                    }
                    foreach (var t in tempinner)
                    {
                        if (t.APPOINTMENTENDTIME.Value.AddSeconds(-1) >= DateTime.UtcNow)
                        {
                            int totalMinutes = Convert.ToInt32((t.APPOINTMENTENDTIME.Value.Subtract(t.APPOINTMENTSTARTTIME.Value)).TotalMinutes);
                            var docdata = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == t.DOCTORID && f.DURATION == 15);
                            if (t != null)
                            {
                               //Session["PateintID"] = "anoopranjan.1983@gmail.com";
                                Session["PateintID"] = t.PATIENTID;
                                temp.uploadreport = true;

                                temp.APPOINTMENTID = t.APPOINTMENTID;

                            }

                            break;
                        }

                    }

                }

                // var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD" && w.realeaseStatement==true && w.termsOfUse==true);
                //  var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => w.APPOINTMENTID == 151);


            }
            string PateintId = Convert.ToString(Session["PateintID"]);
            if (PateintId == "")
            {
               // Session["PateintID"] = "anoopranjan.1983@gmail.com";
            }

            return View(temp);
        }


        public ActionResult VideoConfMain()
        {

            //Session["PateintID"] = "";
            //DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            //temp.appintmentcurrentnext = false;
            //temp.FIRSTNAME = LoginFirstName;
            //temp.LASTNAME = LoginLastName;
            //temp.USERPHOTOFILEPATH = profilepath;
            //temp.inboxUreadMessageCount = messagecount;
            //var tempUsertype = appointmentUOW.Context.UserInformations.FirstOrDefault(w => (w.UserName == User.Identity.Name));
            //if (tempUsertype.USERTYPE == "D")
            //{
            //    var nextapp = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME >= DateTime.UtcNow) && w.DOCTORID == User.Identity.Name && (w.STATUS == "VIDEO APPT CNFRD")).OrderBy(f => f.APPOINTMENTSTARTTIME).Take(1);
            //    if (nextapp != null)
            //    {



            //        foreach (var dt in nextapp)
            //        {
            //            var patientDetails = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == dt.PATIENTID);
            //            temp.appintmentcurrentnext = true;
            //            temp.APPOINTMENTFIRSTNAME = patientDetails.FIRSTNAME;
            //            temp.APPOINTMENTLASTNAME = patientDetails.LASTNAME;
            //            temp.APPOINTMENTSalutation = patientDetails.Salutation;
            //            var timezonedoc = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == dt.DOCTORID).CurrenttimeZone;
            //            temp.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(dt.APPOINTMENTSTARTTIME.Value, User.Identity.Name));
            //            temp.APPOINTMENTSTARTTIMEACTUAL = temp.APPOINTMENTSTARTTIMEACTUAL + " " + getAbbreiviation(timezonedoc);

            //        }
            //    }
            //}
            //else if (tempUsertype.USERTYPE == "P")
            //{
            //    var nextapp = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME >= DateTime.UtcNow) && w.PATIENTID == User.Identity.Name && (w.STATUS == "VIDEO APPT CNFRD")).OrderBy(f => f.APPOINTMENTSTARTTIME).Take(1);

            //    if (nextapp != null)
            //    {

            //        foreach (var dt in nextapp)
            //        {
            //            var doctDetails=uow.Context.UserInformations.FirstOrDefault(f => f.UserName == dt.DOCTORID);
            //            temp.appintmentcurrentnext = true;
            //            temp.APPOINTMENTFIRSTNAME = doctDetails.FIRSTNAME;
            //            temp.APPOINTMENTLASTNAME = doctDetails.LASTNAME;
            //            temp.APPOINTMENTSalutation = doctDetails.Salutation;
            //            var timezonedoc = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == dt.DOCTORID).CurrenttimeZone;
            //            temp.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(dt.APPOINTMENTSTARTTIME.Value, dt.DOCTORID));
            //            temp.APPOINTMENTSTARTTIMEACTUAL = temp.APPOINTMENTSTARTTIMEACTUAL + " " + getAbbreiviation(timezonedoc);
            //        }
            //    }
            //}
            //string PateintId = Convert.ToString(Session["PateintID"]);
            //return View(temp);


            Session["PateintID"] = "";
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.FIRSTNAME = LoginFirstName;
            temp.LASTNAME = LoginLastName;
            temp.USERPHOTOFILEPATH = profilepath;
            temp.inboxUreadMessageCount = messagecount;
            var tempUsertype = appointmentUOW.Context.UserInformations.FirstOrDefault(w => (w.UserName == User.Identity.Name));
            if (tempUsertype.USERTYPE == "D")
            {
                //var tempinner = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.Now && w.APPOINTMENTENDTIME >= DateTime.Now) && w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                var tempinner = appointmentUOW.Context.APPOINTMENTS.Where(w =>  w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                if (tempinner != null)
                {
                    if (tempinner.Count == 0)
                    {
                       // return RedirectToAction("VideoConfMain");// to be removed
                    }
                    foreach (var t in tempinner)
                    {
                        if (t.APPOINTMENTENDTIME.Value.AddSeconds(-1) >= DateTime.UtcNow)
                        {
                            int totalMinutes = Convert.ToInt32((t.APPOINTMENTENDTIME.Value.Subtract(t.APPOINTMENTSTARTTIME.Value)).TotalMinutes);
                            var docdata = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == t.DOCTORID && f.DURATION == 15);
                            if (t != null)
                            {
                                //Session["PateintID"] = t.PATIENTID;
                                //Session["DoctorID"] = t.DOCTORID;

                                temp.uploadreport = t.uploadreport;
                                temp.APPOINTMENTID = t.APPOINTMENTID;
                                Session["APPOINTMENTID"] = t.APPOINTMENTID;
                            }

                            break;
                        }

                    }

                }

                //  var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD" && w.termsOfUseDoctor == true && w.releaseStatementDoctor == true);

                //  var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => w.APPOINTMENTID == 151);

            }
            else if (tempUsertype.USERTYPE == "P")
            {

               // var tempinner = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.Now && w.APPOINTMENTENDTIME >= DateTime.Now) && w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                var tempinner = appointmentUOW.Context.APPOINTMENTS.Where(w => w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").ToList();

                if (tempinner != null)
                {
                    if (tempinner.Count == 0)
                    {
                        // return RedirectToAction("VideoConfMain"); // to be removed
                    }
                    foreach (var t in tempinner)
                    {
                        if (t.APPOINTMENTENDTIME.Value.AddSeconds(-1) >= DateTime.UtcNow)
                        {
                            int totalMinutes = Convert.ToInt32((t.APPOINTMENTENDTIME.Value.Subtract(t.APPOINTMENTSTARTTIME.Value)).TotalMinutes);
                            var docdata = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == t.DOCTORID && f.DURATION == 15);
                            if (t != null)
                            {
                               // Session["PateintID"] = t.PATIENTID;
                                //Session["DoctorID"] = t.DOCTORID;
                                temp.uploadreport = true;
                                temp.APPOINTMENTID = t.APPOINTMENTID;
                                Session["APPOINTMENTID"] = t.APPOINTMENTID;
                            }

                            break;
                        }

                    }

                }

                // var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD" && w.realeaseStatement==true && w.termsOfUse==true);
                //  var data = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(w => w.APPOINTMENTID == 151);


            }
            string PateintId = Convert.ToString(Session["PateintID"]);
            if (PateintId == "")
            {
                // Session["PateintID"] = "anoopranjan.1983@gmail.com";
            }

            return View(temp);
        }


        public JsonResult CheckAppointmentExists()
        {
            try
            {
                APPOINTMENT appExists = null;
                var userType = appointmentUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;
                if (userType == "D")
                {
                   // appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").FirstOrDefault();

                    appExists = appointmentUOW.Context.APPOINTMENTS.Where(w =>  w.DOCTORID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").FirstOrDefault();
                }
                else
                {
                   // appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").FirstOrDefault();

                    appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => w.PATIENTID == User.Identity.Name && w.STATUS == "VIDEO APPT CNFRD").FirstOrDefault();

                }

                if (appExists != null)
                {
                    double totalMinutes = (appExists.APPOINTMENTENDTIME.Value.Subtract(appExists.APPOINTMENTSTARTTIME.Value)).TotalMinutes;
                    double lapsedMinutes = appExists.LAPSEDMINUTES.HasValue ? appExists.LAPSEDMINUTES.Value : 0;
                    string apptExists = string.Empty;
                    if(totalMinutes -lapsedMinutes<0)
                        apptExists = "notexists";
                    else
                        apptExists = "exists";
                    return Json(new
                    {
                        msg = apptExists,
                        doctorID = appExists.DOCTORID,
                        patientID = appExists.PATIENTID,
                        startTime = appExists.APPOINTMENTSTARTTIME,
                        endTime = appExists.APPOINTMENTENDTIME,
                        userType = userType,
                        totalMinutes = totalMinutes,
                        elapsedMinutes = lapsedMinutes,
                        appointmentID = appExists.APPOINTMENTID
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { msg = "notexists" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { msg = "Please Try Again" }, JsonRequestBehavior.AllowGet);
            }
            
        }


        public JsonResult CreateVideoSession()
        {
            try
            {
                APPOINTMENT appExists = null;
                string token = string.Empty;
                string sessionId = string.Empty;
                var userType = appointmentUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;
                if (userType == "D")
                {
                   // appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.STATUS == "VIDEO APPT CNFRD" && w.DOCTORID == User.Identity.Name).FirstOrDefault();

                    appExists = appointmentUOW.Context.APPOINTMENTS.Where(w =>  w.STATUS == "VIDEO APPT CNFRD" && w.DOCTORID == User.Identity.Name).FirstOrDefault();
                }
                else
                {
                   // appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => (w.APPOINTMENTSTARTTIME <= DateTime.UtcNow && w.APPOINTMENTENDTIME >= DateTime.UtcNow) && w.STATUS == "VIDEO APPT CNFRD" && w.PATIENTID == User.Identity.Name).FirstOrDefault();

                      appExists = appointmentUOW.Context.APPOINTMENTS.Where(w =>  w.STATUS == "VIDEO APPT CNFRD" && w.PATIENTID == User.Identity.Name).FirstOrDefault();

                }

                if (appExists != null)
                {
                    if (!string.IsNullOrEmpty(appExists.VIDEOSESSIONID))
                    {
                        //set session id
                        sessionId = appExists.VIDEOSESSIONID;
                        SetSession(out token, sessionId);
                    }
                    else
                    {
                        //create a new session id
                        sessionId = GetVideoSessionId(out token);
                    }
                    using (var repo = new AppointmentServiceRepository(appointmentUOW))
                    {
                        appExists.MODIFIEDBY = User.Identity.Name;
                        appExists.MODIFIEDON = DateTime.UtcNow;
                        appExists.ISVIDEOACTIVE = true;
                        appExists.VIDEOSESSIONID = sessionId;
                        appExists.State = State.Modified;
                        appExists.CHATSESSIONID = appExists.CHATSESSIONID == null ? 0 : appExists.CHATSESSIONID + 1;

                        repo.InsertOrUpdate(appExists);
                        appointmentUOW.Save();
                    }
                }


                return Json(new { sessionid = sessionId, token = token }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return Json(new { msg = "Please Try Again" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "notexists" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult EndVideoSession()
        {
            try
            {
                APPOINTMENT appExists = null;
                var userType = appointmentUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;
                if (userType == "D")
                    appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => w.ISVIDEOACTIVE == true && w.DOCTORID == User.Identity.Name).FirstOrDefault();
                else
                    appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => w.ISVIDEOACTIVE == true && w.PATIENTID == User.Identity.Name).FirstOrDefault();

                if (appExists != null)
                {
                    using (var repo = new AppointmentServiceRepository(appointmentUOW))
                    {
                        appExists.MODIFIEDBY = User.Identity.Name;
                        appExists.MODIFIEDON = DateTime.UtcNow;
                        appExists.ISVIDEOACTIVE = false;
                        appExists.VIDEOSESSIONID = string.Empty;
                        appExists.STATUS = "VIDEO APPT CMPLD";
                        appExists.State = State.Modified;

                        repo.InsertOrUpdate(appExists);
                        appointmentUOW.Save();
                    }
                }

                return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return Json(new { msg = "Please Try Again" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "notexists" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SetTimer(int appointmentId)
        {
            try
            {
                APPOINTMENT appExists = null;
                // var userType = appointmentUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;
                // if (userType == "D")
                appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => w.ISVIDEOACTIVE == true && w.DOCTORID == User.Identity.Name && w.APPOINTMENTID == appointmentId).FirstOrDefault();
                //else
                //    appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => w.ISVIDEOACTIVE == true && w.PATIENTID == User.Identity.Name).FirstOrDefault();

                if (appExists != null)
                {
                    using (var repo = new AppointmentServiceRepository(appointmentUOW))
                    {
                        appExists.MODIFIEDBY = User.Identity.Name;
                        appExists.MODIFIEDON = DateTime.UtcNow;
                        appExists.SHOWTIMER = true;
                        appExists.State = State.Modified;

                        repo.InsertOrUpdate(appExists);
                        appointmentUOW.Save();
                    }
                }

                return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return Json(new { msg = "Please Try Again" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "notexists" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ShowTimer(int appointmentId)
        {
            try
            {
                APPOINTMENT appExists = null;

                appExists = appointmentUOW.Context.APPOINTMENTS.Where(w => w.ISVIDEOACTIVE == true && w.PATIENTID == User.Identity.Name && w.SHOWTIMER == true && w.APPOINTMENTID == appointmentId).FirstOrDefault();

                if (appExists != null)
                {
                    int totalMinutes = (appExists.APPOINTMENTENDTIME.Value.Subtract(appExists.APPOINTMENTSTARTTIME.Value)).Minutes;
                    return Json(new
                    {
                        msg = "success",
                        totalMinutes = totalMinutes,
                        elapsedMinutes = appExists.LAPSEDMINUTES.HasValue ? appExists.LAPSEDMINUTES.Value : 0,

                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { msg = "Please Try Again" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return Json(new { msg = "Please Try Again" }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { msg = "notexists" }, JsonRequestBehavior.AllowGet);
        }

        public string GetTime()
        {
            DateTime dt = new DateTime();
            // dt = Convert.ToDateTime("April 9, 2014 22:38:10");
            dt = Convert.ToDateTime("April 9, 2014 22:38:10");
            // return dt.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            return dt.ToString();
        }
        /// <summary>
        /// Get the last 20 messages for this room
        /// </summary>
        public JsonResult GetMessages(CHATMESSAGE cMessage)
        {


            var userType = cmUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;

            // var request = cmUOW.Context.ChatMessages.Where(s => s.DOCTORID == cMessage.DOCTORID && s.PATIENTID == cMessage.PATIENTID && (s.TIMESTAMP >= cMessage.StartTime && s.TIMESTAMP <= cMessage.EndTime) && s.APPOINTMENTID == cMessage.APPOINTMENTID);
            string doctorId = Convert.ToString(Session["DoctorId"]);
            string patientId = Convert.ToString(Session["PatientId"]);
            long appointmentId = Convert.ToInt32(Session["APPOINTMENTID"]);
           // var request = cmUOW.Context.ChatMessages.Where(s => s.DOCTORID == doctorId && s.PATIENTID == patientId &&  s.APPOINTMENTID == appointmentId);
           var request = cmUOW.Context.ChatMessages.Where(s => s.DOCTORID == cMessage.DOCTORID && s.PATIENTID == cMessage.PATIENTID && (s.TIMESTAMP >= cMessage.StartTime && s.TIMESTAMP <= cMessage.EndTime) && s.APPOINTMENTID == cMessage.APPOINTMENTID);

            if (request != null)
            {
                StringBuilder sb = new StringBuilder();
                int ctr = 0;    // toggle counter for alternating color

                foreach (var req in request)
                {
                    if (ctr == 0)
                    {
                        sb.Append("<div style='padding: 10px;'>");
                        ctr = 1;
                    }
                    else
                    {
                        sb.Append("<div style='background-color: #EFEFEF; padding: 10px;'>");
                        ctr = 0;
                    }

                    sb.Append("<img src='Images/womanIcon.gif' style='vertical-align:middle' alt=''> <span style='color: black; font-weight: bold;'>" + req.MESSAGEFROM +":"+ req.CHATTEXT + ":</span>  </div>");
                }
                return Json(new { text = sb.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { text = "failure" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This will insert the passed text to the doctornotes table in the database
        /// </summary>

        public JsonResult InsertDoctorNotes(DoctorsInformationEditModel model)
        {
            var userType = cmUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;



            using (DoctorNotesServiceRepository repo1 = new DoctorNotesServiceRepository(anUOW))
            {

                if (userType == "D")
                {
                    DOCTORNOTES data = new DOCTORNOTES();
                    data.NOTES = model.NOTES;
                    data.APPOINTMENTID = model.APPOINTMENTID;
                    data.MODIFIEDBY = User.Identity.Name;
                    data.MODIFIEDON = DateTime.UtcNow;
                    data.CREATEDBY = User.Identity.Name;
                    data.DateCreated = DateTime.UtcNow;
                    data.State = DOCVIDEO.ObjectState.State.Added;
                    repo1.InsertOrUpdate(data);
                    anUOW.Save();

                }


            }

            return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This will insert the passed text to the message table in the database
        /// </summary>
        public JsonResult InsertMessage(CHATMESSAGE chatMessage)
        {
            var userType = cmUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;

            CHATMESSAGE message = new CHATMESSAGE();
            if (userType == "D")
            {
                message.DOCTORID = User.Identity.Name;
               // message.PATIENTID = chatMessage.PATIENTID;
                message.PATIENTID = Convert.ToString( Session["PatientId"]);
            }
            else
            {
                message.DOCTORID = Convert.ToString(Session["DoctorId"]);
                //message.DOCTORID = chatMessage.DOCTORID;
                message.PATIENTID = User.Identity.Name;

            }
            message.MESSAGEFROM = User.Identity.Name;
            // message.APPOINTMENTID = chatMessage.APPOINTMENTID;
            message.APPOINTMENTID = Convert.ToInt32(Session["APPOINTMENTID"]);

            if (String.IsNullOrEmpty(chatMessage.CHATTEXT))
            {
                message.CHATTEXT = chatMessage.CHATTEXT.Replace("<", "");
            }
            else
            {
                message.CHATTEXT = chatMessage.CHATTEXT;
            }

            //message.ToUserID = null;            // in the future, we will use this value for private messages
            message.TIMESTAMP = DateTime.UtcNow;

            using (var repo = new ChatMessageServiceRepository(cmUOW))
            {
                repo.InsertOrUpdate(message);
                cmUOW.Save();
            }

            return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveFile(IEnumerable<HttpPostedFileBase> files, string doctorID, string patientID)
        {
            StringBuilder fileNames = new StringBuilder();
            try
            {
                bool IsExists = System.IO.Directory.Exists(Server.MapPath("ChatFiles"));

                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("ChatFiles"));
                }
                //  The Name of the Upload component is "files"
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        //    Some browsers send file names with full path.
                        //    We are only interested in the file name.
                        var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        var fileNamewithoutExt = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                        var fileName = Path.GetFileName(fileNamewithoutExt + "_" + DateTime.UtcNow.ToString("MMddyyyyhhmmss") + ext);


                        var physicalPath = Path.Combine(Server.MapPath("/DoctorInformation/ChatFiles"), fileName);
                        // fileNames.Append("/DoctorInformation/ChatFiles/" + fileName);
                        file.SaveAs(physicalPath);

                        CHATMESSAGE cm = new CHATMESSAGE();
                        cm.DOCTORID = doctorID;
                        cm.PATIENTID = patientID;
                        cm.TIMESTAMP = DateTime.UtcNow;
                        cm.MESSAGEFROM = patientID;
                        cm.CHATTEXT = string.Format("Please click the filename to download : <a color='blue' href='/DoctorInformation/ChatFiles/{0}' target='_blank'>{1}</a>", fileName, file.FileName);
                        using (var chatUOW = new ChatMessageUnitOfWork())
                        {
                            using (var repo = new ChatMessageServiceRepository(chatUOW))
                            {
                                repo.InsertOrUpdate(cm);
                                chatUOW.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }



            return Json(new { status = fileNames.ToString(), type = "save" }, "text/plain");

        }


        /// <summary>
        /// This will insert the passed text to the message table in the database
        /// </summary>
        public JsonResult MinutesElapsedUpdate(APPOINTMENT appointment)
        {

            using (AppointmentsUnitOfWork appointmentUOW = new AppointmentsUnitOfWork())
            {
                var userType = cmUOW.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name).USERTYPE;
                if (userType == "D")
                {
                    using (var repo = new AppointmentServiceRepository(appointmentUOW))
                    {
                        var temp = appointmentUOW.Context.APPOINTMENTS.Find(appointment.APPOINTMENTID);
                        if (temp != null)
                        {
                            double totalMinutes = (temp.APPOINTMENTENDTIME.Value.Subtract(temp.APPOINTMENTSTARTTIME.Value)).TotalMinutes;
                            double lapsedMinutes = temp.LAPSEDMINUTES.HasValue ? temp.LAPSEDMINUTES.Value : 0;
                            string apptExists = string.Empty;
                            if (lapsedMinutes >= totalMinutes)
                                apptExists = "notexists";
                            else
                                apptExists = "exists";

                            if (apptExists == "exists")
                            {
                                temp.LAPSEDMINUTES = temp.LAPSEDMINUTES.HasValue ? temp.LAPSEDMINUTES + 1 : 0;
                                //temp.MODIFIEDBY = User.Identity.Name;
                                temp.MODIFIEDON = DateTime.UtcNow;
                                temp.State = State.Modified;
                                repo.InsertOrUpdate(temp);
                                appointmentUOW.Save();
                            }
                            else
                            {
                                temp.MODIFIEDON = DateTime.UtcNow;
                                temp.ISVIDEOACTIVE = false;
                                temp.VIDEOSESSIONID = string.Empty;
                                temp.STATUS = "VIDEO APPT CMPLD";
                                temp.State = State.Modified;

                                repo.InsertOrUpdate(temp);
                                appointmentUOW.Save();
                            }
                        }
                    }
                    return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { msg = "wait" }, JsonRequestBehavior.AllowGet);
        }

        public DateTime GetUTCtime(DateTime current, int dstObserved, double timezoneOffset)
        {
            var timezone = uow.Context.UserInformations.FirstOrDefault(w => w.UserName == User.Identity.Name).CurrenttimeZone;
            if (timezone == null)
                timezone = "Central Standard Time";

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);

            double clientoffSetHours = timezoneOffset + dstObserved;
            //if (clientoffSetHours > 0)
            //    clientoffSetHours = clientoffSetHours * -1;

            if (offSetHours == clientoffSetHours)
                return current;
            else
            {
                current = current.AddHours(clientoffSetHours);
                DateTime d = current.AddHours(offSetHours * -1);

                return d;
            }
        }

        public DateTime GetLocalTime(DateTime utcTime)
        {
            DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
            double offSetHours = 0;
            var timezone = uow.Context.UserInformations.FirstOrDefault(w => w.UserName == User.Identity.Name).CurrenttimeZone;
            if (timezone != null)
            {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);
            }
            else
            {
                timezone = "Central Standard Time";
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);
            }

            DateTime d = utcTime.AddHours(offSetHours);

            return d;

        }

        public DateTime GetLocalTime(DateTime utcTime, string DOCTORID)
        {
            DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
            double offSetHours = 0;
            var timezone = uow.Context.UserInformations.FirstOrDefault(w => w.UserName == DOCTORID).CurrenttimeZone;
            if (timezone != null)
            {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);
            }
            else
            {
                timezone = "Central Standard Time";
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);
            }



            DateTime d = utcTime.AddHours(offSetHours);

            return d;

        }

        public DateTime GetLocalTime(DateTime? utcTime, string DOCTORID, double timezoneOffset)
        {

            DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
            double offSetHours = 0;
            var timezone = uow.Context.UserInformations.FirstOrDefault(w => w.UserName == DOCTORID).CurrenttimeZone;
            if (timezone != null)
            {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);
            }
            else
            {
                timezone = "Central Standard Time";
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.UtcNow)) ? 1 : 0);
            }



            DateTime d = utcTime.Value.AddHours(offSetHours);
            d = d.AddHours((timezoneOffset * -1));
            return d;


        }



        [AllowAnonymous]
        public ActionResult _ResonForVisit()
        {

            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult _AppointmentLogIn()
        {

            return View();
        }

        [AllowAnonymous]
        public ActionResult _PayForVisit()
        {

            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult BookYourAppointment(BookingAppointmentModelOuter model)
        {

            return View(_PayForVisit(model));
        }
        [AllowAnonymous]
        [HttpPost]
        public BookingAppointmentModelOuter _PayForVisit(BookingAppointmentModelOuter model)
        {

            string Patientid = Convert.ToString(Session["PatientID"]);
            var tempdata = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == model.DOCTORSLOTID);
            var temp = auok.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.PATIENTID == Patientid 
                        && f.STATUS == "WAITING FOR PAYMENT" && f.DOCTORSLOTID == model.DOCTORSLOTID)
                        .OrderByDescending(f => f.DateCreated).Take(1);

            var ddata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);

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
                        temps = temps + " , " + spec.SPECIALITY.ToString();
                    }
                }
                if (temps != null)
                {
                    model.Speciality = temps;
                }
            }
            long id = 0;
            int duration = Convert.ToInt32(model.APPOINTMENTENDTIME);
            int rate = duow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == model.DOCTORID && f.DURATION == duration).RATE;

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
                    //string docotorPayPalId = duow.Context.DoctorsInformations.FirstOrDefault(s => s.UserName == model.DOCTORID).PAYPALEMAIL;
                    //PaySample samplePay = new PaySample();
                    //if (docotorPayPalId == "" || docotorPayPalId == null)
                    //    docotorPayPalId = "anoopranjan.1983@gmail.com";
                    //PayRequest requestPay = samplePay.ParallelPaymentOuter(rate, docotorPayPalId, model.DOCTORSLOTID, id);
                    ////   requestPay.senderEmail = "sanjeevsridhar@gmail.com";
                    //PayResponse pr = samplePay.PayAPIOperations(requestPay,false);
                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["RETURN_URL"] + "/DoctorInformation/BookYourAppointment/?pstatus=success&appointmentId=" + id + "&tabvalue=tab3" + "&id=" + model.DOCTORSLOTID + "&pay=" + rate);

                }
            }
            return model;
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult BookYourAppointment(int id = 0, string tabvalue = "tab0", string pstatus = null, int appointmentId = 0, int pay = 0)
        {
            BookingAppointmentModelOuter model = new BookingAppointmentModelOuter();
            if (id == 0)
            {
                id = Convert.ToInt32(Session["SlotId"]);
            }
            string Patientid = Convert.ToString(Session["PatientID"]);
            if (id != 0)
            {
                if (pstatus != null)
                {
                    model.pstatus = pstatus;
                    if (pstatus == "cancel")
                    {

                        var tempdatas = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
                        var temp = auok.Context.APPOINTMENTS.FirstOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == Patientid && f.STATUS == "WAITING FOR PAYMENT" && f.APPOINTMENTID == appointmentId);
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

                        var tempdatas = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
                        var temp = auok.Context.APPOINTMENTS.FirstOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == Patientid && f.STATUS == "WAITING FOR PAYMENT" && f.APPOINTMENTID == appointmentId);
                        if (temp != null)
                        {
                            using (var repo = new AppointmentServiceRepository(auok))
                            {

                                temp.STATUS = "PAYMENT DONE";
                                temp.MODIFIEDBY = Patientid;
                                temp.MODIFIEDON = DateTime.UtcNow;
                                temp.PAYAMOUNT = Convert.ToDecimal(pay);
                                temp.State = DOCVIDEO.ObjectState.State.Modified;
                                repo.InsertOrUpdate(temp);
                                auok.Save();
                            }

                        }
                    }
                }

                var tempdata = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
                var tempSlotid = apUOW.Context.APPOINTMENTSLOTS.FirstOrDefault(f => f.DOCTORCONFIRMEDSLOTID == id);

                if (tempSlotid != null)
                {
                    Session["ErrorMessage"] = "You can't have Appointment, doctors slot is already booked";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                var pdata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
                var ddata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);
                var ddatasl = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);

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

                if (tempdata.FROMTIME < GetLocalTime(DateTime.UtcNow, tempdata.UserName))
                {
                    Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                    return RedirectToAction("AppoinmnetRestriction");
                }
                //int tempappdata = appointmentUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.PATIENTID == User.Identity.Name && f.STATUS != "VIDEO APPT CNFRD").Count();
                //if (tempappdata != 0)
                //{
                //    Session["ErrorMessage"] = "You can't have Appointment as you have a appointment request pending";
                //    return RedirectToAction("AppoinmnetRestriction");
                //}


                int pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                  || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as Appointment slot is already booked.";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == Patientid && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                  || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as you already have an confirmed appointment at  this time .";
                    return RedirectToAction("AppoinmnetRestriction");
                }





                model.APPOINTMENTSTARTTIME = tempdata.FROMTIME;
                model.DOCTORSLOTID = id;
                model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", tempdata.FROMTIME) + " " + getAbbreiviation(ddata.CurrenttimeZone);
                model.RateQuatermins = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == ddata.UserName && f.DURATION == 15).RATE;
                model.FIRSTNAME = ddata.FIRSTNAME;
                model.LASTNAME = ddata.LASTNAME;
                model.APPOINTMENTENDTIME = "15";

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
                            temps = temps + " , " + spec.SPECIALITY.ToString();
                        }
                    }
                    if (temps != null)
                    {
                        model.Speciality = temps;
                    }
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult PreBookYourAppointment(int id)
        {
            Session["SlotId"] = id;

            var tempdata = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
            var tempSlotid = apUOW.Context.APPOINTMENTSLOTS.FirstOrDefault(f => f.DOCTORCONFIRMEDSLOTID == id);

            if (tempSlotid != null)
            {
                Session["ErrorMessage"] = "You can't have Appointment, doctors slot is already booked";
                return RedirectToAction("AppoinmnetRestriction");
            }



            if (tempdata.FROMTIME < GetLocalTime(DateTime.UtcNow, tempdata.UserName))
            {
                Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                return RedirectToAction("AppoinmnetRestriction");
            }


            int pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
              || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
            if (pendingAppointment != 0)
            {
                Session["ErrorMessage"] = "You can't have Appointment as Appointment slot is already booked.";
                return RedirectToAction("AppoinmnetRestriction");
            }

            BookingAppointmentModelOuter model = new BookingAppointmentModelOuter();

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ClinicPreBookYourAppointment(int id)
        {
            Session["SlotId"] = id;

            var tempdata = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
            var tempSlotid = appointmentUOW.Context.APPOINTMENTS.FirstOrDefault(f => f.DOCTORSLOTID == id);

            var pdata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
            if (pdata != null)
            {
                if (pdata.COUNTRY != "United States")
                {
                    Session["ErrorMessage"] = "You can't have Clinic Appointment";
                    return RedirectToAction("AppoinmnetRestriction");
                }
            }


            if (tempSlotid != null)
            {
                Session["ErrorMessage"] = "You can't have Appointment, doctors slot is already booked";
                return RedirectToAction("AppoinmnetRestriction");
            }



            if (tempdata.FROMTIME < GetLocalTime(DateTime.UtcNow, tempdata.UserName))
            {
                Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                return RedirectToAction("AppoinmnetRestriction");
            }


            int pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
              || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
            if (pendingAppointment != 0)
            {
                Session["ErrorMessage"] = "You can't have Appointment as Appointment slot is already booked.";
                return RedirectToAction("AppoinmnetRestriction");
            }

            BookingAppointmentModelOuter model = new BookingAppointmentModelOuter();

            return View(model);
        }



        [AllowAnonymous]
        public ActionResult Appintments(BookingAppointmentModel model)
        {
            string Patientid = Convert.ToString(Session["PatientID"]);
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

            int duration = Convert.ToInt32(model.APPOINTMENTENDTIME);
            int rate = duow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == model.DOCTORID && f.DURATION == 15).RATE;

            if (rate == 0)
            {
                return Json(new { msg = "Error : Requested Slots Pay Rates are not Available ,Please try for a different doctor " }, JsonRequestBehavior.AllowGet);
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
                    ap.CREATEDBY = Patientid;
                    ap.DateCreated = DateTime.UtcNow;
                    ap.DOCTORID = model.DOCTORID;
                    ap.PATIENTID = Patientid;
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
                    ap.MODIFIEDBY = Patientid;
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

        [AllowAnonymous]
        public ActionResult AppintmentConfirmOuter(BookingAppointmentModel model)
        {
            string Patientid = Convert.ToString(Session["PatientID"]);
            string comment = "Appoinment Confirm Outer.";
            string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
            
            //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.AppointmentConfirmOuter, Convert.ToInt32(model.DOCTORSLOTID), 0, Patientid, data);
           
            /*
            string timezone = null;
            string Patientid = Convert.ToString(Session["PatientID"]);
           var tempdatas = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == model.DOCTORSLOTID);
            var timezonevalue = uow.Context.UserInformations.FirstOrDefault(w => w.UserName == tempdatas.UserName);
            if (timezonevalue != null)
            {
                timezone = timezonevalue.CurrenttimeZone;
            }
            else
            {
                timezone = "Central Standard Time";
            }

            var temp = auok.Context.APPOINTMENTS.FirstOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == Patientid && f.STATUS == "PAYMENT DONE" && f.DOCTORSLOTID == tempdatas.DOCTORSLOTID);
            using (var repo = new AppointmentServiceRepository(auok))
            {

                temp.STATUS = "VIDEO APPT RQSTD";
                temp.MODIFIEDBY = Patientid;
                temp.MODIFIEDON = DateTime.UtcNow;
                temp.State = DOCVIDEO.ObjectState.State.Modified;
                repo.InsertOrUpdate(temp);
                auok.Save();
            }
            var tempvalue = duow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.DOCTORID).CurrenttimeZone;
            var tempPateint = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
            var tempDoctor = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == temp.DOCTORID);
            StringBuilder bodyMsg = new StringBuilder();

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
            bodyMsg.Append("<div>Your request for appoint with doctor  on DATE " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME, temp.DOCTORID, 0)) + " " + getAbbreiviation(tempvalue) + "  has been sent. Once ");
            bodyMsg.Append("the doctor confirms, you will receive confirmation email. ");
            bodyMsg.Append("</div>");
            bodyMsg.Append("<p>Please ");
            bodyMsg.Append("<a href='#' style='color:#1072B5;'>Click here");
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
            bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
            bodyMsg.Append("</a> For FAQ, please visit ");
            bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>  www.PROFESSORSONLINE.com/faq.");
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
            bodyMsg.Append("<strong>Disclaimer:");
            bodyMsg.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
            bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
            bodyMsg.Append("</strong>");
            bodyMsg.Append("</div>");
            bodyMsg.Append("</div>");
            DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", temp.PATIENTID, "", "", "PROFESSORSONLINE has a received a secure email for online Appointment.", bodyMsg.ToString(), false, "", 0);



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
            bodyMsgDoc.Append("<div>Please confirm Appointment  on DATE " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(temp.APPOINTMENTSTARTTIME, tempdatas.UserName, 0)) + " " + getAbbreiviation(tempvalue) + " to meet your patient.");
            bodyMsgDoc.Append("</div>");
            bodyMsgDoc.Append("<p>Please");
            bodyMsgDoc.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "DoctorInformation/DoctorAppointmentConfirmation?id=" + temp.APPOINTMENTID + " 'title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");
            bodyMsgDoc.Append("</a>  to login to your doccare account!");
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
            bodyMsgDoc.Append("<strong>See your Doctor, anytime, anywhere!");
            bodyMsgDoc.Append("</strong>");
            bodyMsgDoc.Append("</div>");
            bodyMsgDoc.Append("<p>If you have questions about doccare online, please email us at");
            bodyMsgDoc.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
            bodyMsgDoc.Append("</a> For FAQ, please visit ");
            bodyMsgDoc.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>  www.PROFESSORSONLINE.com/faq.");
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

            DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", temp.DOCTORID, "", "", "PROFESSORSONLINE has a received a secure email for online Appointment.", bodyMsgDoc.ToString(), false, "", 0);
           
            using (MessageRepository repo = new MessageRepository(muw))
            {
                MESSAGE message = new MESSAGE();

                message.MESSAGESUBJECT = "Appointment  request";
                message.MESSAGEBODY = "Appointment  Booking request";
                message.DateCreated = DateTime.UtcNow;
                message.SENDMESSAGETO = temp.DOCTORID;
                message.SENDMESSAGEFROM = Patientid;
                message.SENDBY = Patientid;
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
            */
            return Json(new { msg = "Appointment Request Sent" }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult PreBookYourAppointmentSecond()
        {
            BookingAppointmentModelOuter model = new BookingAppointmentModelOuter();
            int id = Convert.ToInt32(Session["SlotId"]);
            string Patientid = Convert.ToString(Session["PatientID"]);
            if (id != 0)
            {


                var tempdata = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
                var tempSlotid = apUOW.Context.APPOINTMENTSLOTS.FirstOrDefault(f => f.DOCTORCONFIRMEDSLOTID == id);

                if (tempSlotid != null)
                {
                    Session["ErrorMessage"] = "You can't have Appointment, doctors slot is already booked";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                var pdata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
                var ddata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);

                var ddatasl = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);

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
                if (tempdata.FROMTIME < GetLocalTime(DateTime.UtcNow, tempdata.UserName))
                {
                    Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                    return RedirectToAction("AppoinmnetRestriction");
                }


                int pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                  || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as Appointment slot is already booked.";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == Patientid && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                   || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as you already have an confirmed appointment at  this time .";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                model.APPOINTMENTSTARTTIME = tempdata.FROMTIME;
                model.DOCTORSLOTID = id;
                model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", tempdata.FROMTIME) + " " + getAbbreiviation(ddata.CurrenttimeZone);
                model.RateQuatermins = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == ddata.UserName && f.DURATION == 15).RATE;
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
                            temps = temps + " , " + spec.SPECIALITY.ToString();
                        }
                    }
                    if (temps != null)
                    {
                        model.Speciality = temps;
                    }
                }
            }



            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ClinicPreBookYourAppointmentSecond()
        {
            BookingAppointmentModelOuter model = new BookingAppointmentModelOuter();
            int id = Convert.ToInt32(Session["SlotId"]);
            string Patientid = Convert.ToString(Session["PatientID"]);
            if (id != 0)
            {
                var patientData = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
                if (patientData != null)
                {
                    if (patientData.COUNTRY != "United States")
                    {
                        Session["ErrorMessage"] = "You can't have Clinic Appointment";
                        return RedirectToAction("AppoinmnetRestriction");
                    }
                }

                var tempdata = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
                var tempSlotid = apUOW.Context.APPOINTMENTSLOTS.FirstOrDefault(f => f.DOCTORCONFIRMEDSLOTID == id);

                if (tempSlotid != null)
                {
                    Session["ErrorMessage"] = "You can't have Appointment, doctors slot is already booked";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                var pdata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
                var ddata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);
                var ddatasl = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);

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

                if (tempdata.FROMTIME < DateTime.UtcNow)
                //if (tempdata.FROMTIME < GetLocalTime(DateTime.UtcNow, tempdata.UserName))
                {
                    Session["ErrorMessage"] = "You can't have Appointment as it's past time. Please Try Again";
                    return RedirectToAction("AppoinmnetRestriction");
                }


                int pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.DOCTORID == tempdata.UserName && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                  || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as Appointment slot is already booked.";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                pendingAppointment = appointmentUOW.Context.APPOINTMENTS.Where(f => f.PATIENTID == Patientid && f.STATUS == "VIDEO APPT CNFRD" && ((f.APPOINTMENTSTARTTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME)
                   || (f.APPOINTMENTENDTIME > tempdata.FROMTIME && f.APPOINTMENTSTARTTIME < tempdata.TOTIME))).Count();
                if (pendingAppointment != 0)
                {
                    Session["ErrorMessage"] = "You can't have Appointment as you already have an confirmed appointment at  this time .";
                    return RedirectToAction("AppoinmnetRestriction");
                }

                model.APPOINTMENTSTARTTIME = tempdata.FROMTIME;
                model.DOCTORSLOTID = id;
                model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(tempdata.FROMTIME,tempdata.UserName)) + " " + getAbbreiviation(ddata.CurrenttimeZone);
                model.RateQuatermins = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == ddata.UserName && f.DURATION == 15).RATE;
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
                            temps = temps + " , " + spec.SPECIALITY.ToString();
                        }
                    }
                    if (temps != null)
                    {
                        model.Speciality = temps;
                    }
                }
            }



            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ClinicPreBookYourAppointmentSecond(BookingAppointmentModelOuter model)
        {
            string Patientid = Convert.ToString(Session["PatientID"]);
            AppointmentsUnitOfWork auok = new AppointmentsUnitOfWork();
            if (!auok.Context.APPOINTMENTS.Where(f => f.DOCTORSLOTID == model.DOCTORSLOTID).Any())
            {
                var tempdatas = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == model.DOCTORSLOTID);
                if (tempdatas.FROMTIME < GetLocalTime(DateTime.UtcNow, tempdatas.UserName))
                {
                    Session["ErrorMessage"] = "You can't have Appointment as it's past time";
                    return RedirectToAction("AppoinmnetRestriction");
                }
              //  var timezone = uow.Context.UserInformations.FirstOrDefault(w => w.UserName == tempdatas.UserName).CurrenttimeZone;
                var pdata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
                var ddata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdatas.UserName);
               // StringBuilder bodyMsg = new StringBuilder();
                var tempPateint = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
                model.APPOINTMENTSTARTTIME = tempdatas.FROMTIME;
                model.DOCTORSLOTID = tempdatas.DOCTORSLOTID;
                model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", tempdatas.FROMTIME);
                model.FIRSTNAME = ddata.FIRSTNAME;
                model.LASTNAME = ddata.LASTNAME;
                model.DOCTORID = tempdatas.UserName;
                string temps = null;
                var userspecialyid = duow.Context.Doctorspecialities.Where(w => w.UserName == tempdatas.UserName).Select(s => new { s.SPECIALITY });
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
                            temps = temps + " , " + spec.SPECIALITY.ToString();
                        }
                    }
                    if (temps != null)
                    {
                        model.Speciality = temps;
                    }
                }


                using (MessageRepository repo = new MessageRepository(muw))
                {
                    MESSAGE message = new MESSAGE();

                    message.MESSAGESUBJECT = "Appointment Clinic request";
                    message.MESSAGEBODY = "Appointment Clinic Booking request";
                    message.DateCreated = DateTime.UtcNow;
                    message.SENDMESSAGETO = tempdatas.UserName;
                    message.SENDMESSAGEFROM = Patientid;
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
                    ap.CREATEDBY = Patientid;
                    ap.DateCreated = DateTime.UtcNow;
                    ap.DOCTORID = tempdatas.UserName;
                    ap.PATIENTID = Patientid;
                    ap.STATUS = "CLINIC APPT RQSTD";
                    if (model.REASONFORVISIT != null)
                    {
                        ap.REASONFORVISIT = model.REASONFORVISIT;
                    }
                    else
                    {
                        ap.REASONFORVISIT = "";
                    }
                    
                    ap.ISEXISITING = "";
                    ap.MODIFIEDBY = Patientid;
                    ap.MODIFIEDON = DateTime.UtcNow;
                    ap.DOCTORSLOTID = tempdatas.DOCTORSLOTID;
                    ap.State = DOCVIDEO.ObjectState.State.Added;
                    repo.InsertOrUpdate(ap);
                    auok.Save();
                    string comment = "Clinic Pre Book Your Appointment Second.";
                    string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                    //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.ClinicPreBookYourAppointmentSecond, 0, Convert.ToInt32(model.DOCTORSLOTID), Patientid, data);
                       
                    /*
                    var tempDoctor = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdatas.UserName);
                    var docclinicinfo = uow.Context.DocWorkInstitutions.FirstOrDefault(f => f.UserName == tempdatas.UserName);
                    StringBuilder bodyMsgDoc = new StringBuilder();
                    var appointmentidfetch = auok.Context.APPOINTMENTS.FirstOrDefault(f => f.DOCTORID == tempdatas.UserName && f.PATIENTID == Patientid && f.DOCTORSLOTID == tempdatas.DOCTORSLOTID && f.STATUS == "CLINIC APPT RQSTD");
                    var urlTemp = Url.Content("~/content/images/logo.png");
                    bodyMsgDoc.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                    bodyMsgDoc.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                    bodyMsgDoc.Append("<div>");
                    bodyMsgDoc.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + Url.Content("~/content/images/logo.png") + "' title='doccare_logo' alt='doccare logo'/>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='padding:10px;'>");
                    bodyMsgDoc.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                    bodyMsgDoc.Append("<h2 style='color:#1072B5;font-style: italic;'> Dear  " + tempDoctor.Salutation + " " + tempDoctor.FIRSTNAME + " " + tempDoctor.LASTNAME + "</h2>");
                    bodyMsgDoc.Append("<div style='float:left;width:490px;'> ");
                    bodyMsgDoc.Append("<div>Please confirm Appointment  on DATE " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(ap.APPOINTMENTSTARTTIME, tempdatas.UserName, 0)) + " " + getAbbreiviation(timezone) + " to meet your patient.");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<p>Please");
                    bodyMsgDoc.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "DoctorInformation/DoctorAppointmentConfirmation?id=" + appointmentidfetch.APPOINTMENTID + " 'title='Click' style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");
                    bodyMsgDoc.Append("</a>  to login to your doccare account!");
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
                    bodyMsgDoc.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
                    bodyMsgDoc.Append("</a> For FAQ, please visit ");
                    bodyMsgDoc.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>  www.PROFESSORSONLINE.com/faq.");
                    bodyMsgDoc.Append("</a>");
                    bodyMsgDoc.Append("</p>");
                    bodyMsgDoc.Append("</div>");
                    bodyMsgDoc.Append("<div style='float:right; margin-left:10px;width:230px;'>");
                    bodyMsgDoc.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
                    bodyMsgDoc.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>PROFESSORSONLINE helps doctors and patients connect securely anytime, anywhere!");
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

                    DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", tempdatas.UserName, "", "", "PROFESSORSONLINE has a received a secure email for Clinic Appointment.", bodyMsgDoc.ToString(), false, "", 0);


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
                    bodyMsg.Append("<div>Your request for appointment with doctor  on " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", GetLocalTime(ap.APPOINTMENTSTARTTIME, tempdatas.UserName, 0)) + " " + getAbbreiviation(timezone) + " at " + docclinicinfo.INSTITUTIONNAME + " ," + docclinicinfo.CLINICCITY + " has ");
                    bodyMsg.Append("been sent. Once the doctor confirms, you will receive ");
                    bodyMsg.Append("confirmation email.");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<p>Please");
                    bodyMsg.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn 'title='Click' style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");
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
                    bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
                    bodyMsg.Append("</a> For FAQ, please visit ");
                    bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>  www.PROFESSORSONLINE.com/faq.");
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
                    DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", Patientid, "", "", "PROFESSORSONLINE has sent a secure email to your doctor for  Appointment request at the clinic", bodyMsg.ToString(), false, "", 0);
                    */
                }

                Session["ErrorMessage"] = "Clinic Appointment Booked Successfully.Please Check your email for Appointment details.";
                return RedirectToAction("AppoinmnetRestriction");
            }
            else
            {
                Session["ErrorMessage"] = "You can't have This Clinic Appointment as it's already booked";
                return RedirectToAction("AppoinmnetRestriction");
            }


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
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Welcomepage()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult PostPaymentBookYourAppointment()
        {
            BookingAppointmentModelOuter model = new BookingAppointmentModelOuter();
            string Patientid = Convert.ToString(Session["PatientID"]);
            int id = Convert.ToInt32(Session["SlotId"]);
            if (id != 0)
            {


                var tempdata = apUOW.Context.DOCTORSLOTS.FirstOrDefault(f => f.DOCTORSLOTID == id);
                var tempSlotid = apUOW.Context.APPOINTMENTSLOTS.FirstOrDefault(f => f.DOCTORCONFIRMEDSLOTID == id);


                var pdata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);
                var ddata = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);
                model.APPOINTMENTSTARTTIME = tempdata.FROMTIME;
                model.DOCTORSLOTID = id;
                model.APPOINTMENTSTARTTIMEACTUAL = String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", tempdata.FROMTIME) + " " + getAbbreiviation(ddata.CurrenttimeZone);
                model.RateQuatermins = uow.Context.DoctorPayRates.FirstOrDefault(f => f.UserName == ddata.UserName && f.DURATION == 15).RATE;
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
                            temps = temps + " , " + spec.SPECIALITY.ToString();
                        }
                    }
                    if (temps != null)
                    {
                        model.Speciality = temps;
                    }
                }
                var tempDoctor = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == tempdata.UserName);
                var tempPateint = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == Patientid);

            }



            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> PreBookYourAppointment(BookingAppointmentModelOuter model)
        {
            ModelState.AddModelError("", "");

            try
            {
                //if (this.IsCaptchaValid("Captcha is not valid"))
                RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    ModelState.AddModelError("", "Please enter captcha.");
                    return View(model);
                }

                RecaptchaVerificationResult recaptchaResult = await recaptchaHelper.VerifyRecaptchaResponseTaskAsync();

                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha value.");
                    return View(model);
                }
                else
                {
                    if (model.USERSTATE != null || model.COUNTRY != null)
                    {
                        if (model.USERSTATE != null)
                        {
                            var data = Convert.ToString(Session["DOCTORID"]);
                            if (data != "")
                            {
                                var docuserstate = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == data).STATELICENSE;
                                if (model.USERSTATE != docuserstate)
                                {
                                    ModelState.AddModelError("", "You cannot be regisered for different state doctors.");
                                    return View(model);
                                }
                            }
                        }
                        //if (ModelState.IsValid)
                        //{
                            if (model.AGREETERMS)
                            {
                                USERSINFORMATION user = uws.Context.UsersInformations.FirstOrDefault(f => f.UserName == model.UserName);
                                MembershipCreateStatus createStatus = MembershipCreateStatus.UserRejected;
                                if (user == null)
                                    createStatus = account.CreateUser(model.UserName, model.PASSWORDS, model.UserName);

                                if (createStatus == MembershipCreateStatus.Success)
                                {

                                    using (var repo = new PatientInformationRepository(uws))
                                    {
                                        Session["PatientID"] = model.UserName;
                                        user = uws.Context.UsersInformations.FirstOrDefault(f => f.UserName == model.UserName);
                                        USERPASSWORD userpwd = new USERPASSWORD();
                                        StringBuilder bodyMsg = new StringBuilder();
                                        user.PASSWORDHASH = user.PASSWORDHASH;
                                        user.PASSWORDSALT = user.PASSWORDSALT;
                                        user.DateLastActivity = DateTime.UtcNow;
                                        user.DateLastLogin = DateTime.UtcNow;
                                        user.DateLastPasswordChange = DateTime.UtcNow;
                                        user.IsApproved = false;
                                        user.FIRSTNAME = model.PATFIRSTNAME;
                                        user.LASTNAME = model.PATLASTNAME;
                                        user.GENDER = model.GENDER;
                                        user.IsfirstLogin = "true";
                                        //model.DOB = new DateTime(Convert.ToInt32(model.DOBYEAR), Convert.ToInt32(model.DOBMonth), Convert.ToInt32(model.DOBDAY));
                                        //user.DOB = model.DOB;
                                        user.USERTYPE = "P";
                                        //user.STREETADDRESS = model.USERSTREETADDRESS1;
                                        //user.CITY = model.CITY;
                                        user.COUNTRY = model.COUNTRY;
                                        user.USERSTATE = model.USERSTATE;
                                        //user.ZIPCODE = model.ZIPCODE;
                                        user.UserName = model.UserName;
                                        user.Email = model.UserName;
                                        user.SECURITYQESTID1 = model.SECURITYQESTID1;
                                        user.ANSWER1 = model.ANSWER1;
                                        //user.SECURITYQESTID2 = model.SECURITYQESTID2;
                                        //user.ANSWER2 = model.ANSWER2;
                                        user.AGREETERMS = model.AGREETERMS;
                                        user.USERPHOTOFILEPATH = "/Images/profile_doctor.jpg";
                                        user.VERIFICATIONCODE = GenerateRandom.genereteRandomNumber();
                                        user.CREATEDBY = model.UserName;
                                        user.MODIFIEDBY = model.UserName;
                                        user.DateCreated = DateTime.UtcNow;
                                        user.MODIFIEDON = DateTime.UtcNow;
                                        user.REGISTEREDON = DateTime.UtcNow;
                                        //user.CurrenttimeZone = model.CurrenttimeZone;
                                        //user.Salutation = model.Salutation;
                                        user.HITCOUNT = "0";
                                        user.State = State.Modified;

                                        PATIENTINFORMATION patientInformation = new PATIENTINFORMATION();
                                        patientInformation.MODIFIEDBY = model.UserName;
                                        patientInformation.DateCreated = DateTime.UtcNow;
                                        patientInformation.MODIFIEDON = DateTime.UtcNow;
                                        patientInformation.CREATEDBY = model.UserName;
                                        patientInformation.POVEMAIL = user.UserName;
                                        patientInformation.PATIENTTYPE = "P";
                                        patientInformation.State = DOCVIDEO.ObjectState.State.Added;
                                        patientInformation.UserName = user.UserName;

                                        user.PatientInformations.Add(patientInformation);

                                        userpwd.CREATEDBY = model.UserName;
                                        userpwd.MODIFIEDON = DateTime.UtcNow;
                                        userpwd.DateCreated = DateTime.UtcNow;
                                        userpwd.MODIFIEDBY = model.UserName;
                                        userpwd.State = State.Added;
                                        userpwd.UserName = model.UserName;
                                        userpwd.EXPIRYDATE = DateTime.UtcNow.AddMonths(1);
                                        userpwd.PASSWORDHASH = user.PASSWORDHASH;
                                        userpwd.PASSWORDSALT = user.PASSWORDSALT;
                                        userpwd.STATUS = "A";
                                        user.UserPasswords.Add(userpwd);

                                        repo.InsertOrUpdate(user);
                                        uws.Save();
                                        string comment = "Cancel Appointment.";
                                        string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                                        //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.DoctorBookYourAppointmentClinic, 0, 0, model.UserName, data);
            
                                        /*
                                        bodyMsg.Append(" <style>");
                                        bodyMsg.Append("ul.a {list-style-type:circle;color:#d3d3d3;margin-left:20px;margin-bottom:20px;}");
                                        bodyMsg.Append("#logo {background: url(" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Content/Images/doccare_logo.jpg) no-repeat;width: 211px;height: 38px;cursor: pointer;margin:0px;}");
                                        bodyMsg.Append("</style>");
                                        bodyMsg.Append(" <div style='background-color: #5d5d5d; box-shadow:0px 1px 3px 1px #8e8c8c;padding:20px;'>");
                                        bodyMsg.Append("<div style='background-color: #ffffff;width:700px;box-shadow:0px 1px 3px 1px #8e8c8c; font-family: 'Ubuntu', sans-serif;padding-bottom:40px;border:1px solid #aaa;'>");
                                        bodyMsg.Append("<div style='width:100%;background-color:#fff;height:38px;'>");
                                        bodyMsg.Append(" <div id='logo'></div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append(" <div style='background-color:#3fb6d8;width:100%;height:30px;text-align:center;color:#fff;font-size:18px;padding-top:15px;'>");
                                        bodyMsg.Append("<strong>Please confirm your subscription with doccare online</strong>");

                                        bodyMsg.Append(" </div>");
                                        bodyMsg.Append("<div style='padding:20px;border:1px solid #aaa;box-shadow:0px 1px 3px 1px #8e8c8c;background-color: #ffffff;'>");
                                        bodyMsg.Append("<div style='float:right; margin:20px 20px 0px 0px;'>" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                                        bodyMsg.Append("<div style='margin-top:30px;'>");
                                        bodyMsg.Append("<p style='margin:50px 0px 10px 0px;'> Dear,<h3><strong style='text-transform:uppercase; margin-left:2px;' >" + model.FIRSTNAME + " " + model.LASTNAME + "</strong></h3></p>");
                                        bodyMsg.Append("<H3 style='color:#3fb6d8;margin-bottom:20px;'><strong>Thank you for joining doccare online");
                                        bodyMsg.Append("Here are some of the benefits you will Recieve</strong></H3>");
                                        bodyMsg.Append("<ul class='a' style='margin:0px 0px 20px 20px;color:#d3d3d3;list-style-type:circle;'>");
                                        bodyMsg.Append("<li>Doctor will be able to monitor non-urgent issues");
                                        bodyMsg.Append("conveniently and inexpensively  </li><br />");
                                        bodyMsg.Append("<li>There is NO membership free and you can sign up and<br />");
                                        bodyMsg.Append("create an account for FREE   </li><br />");
                                        bodyMsg.Append("<li>Doctors can write your prescription or medication and send ");
                                        bodyMsg.Append("it electronically to the pharmacy's.</li>");
                                        bodyMsg.Append("</ul>");
                                        bodyMsg.Append("<p >");
                                        bodyMsg.Append("<a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn?id=" + user.VERIFICATIONCODE + "'style='color:#0094ff;margin-right:10px;'> Click here </a>");
                                        bodyMsg.Append("to begin setting up your account and talk to a doctor right away.");
                                        bodyMsg.Append("</p>");
                                        bodyMsg.Append("<p style='margin:20px 0px 0px 0px;'>doccare online is an online care system that allows you to comment to patients immediately,");
                                        bodyMsg.Append("whenever and wherever.Our innovative advanced web-based technologies remove traditional");
                                        bodyMsg.Append("barriers to healthcare access.Now that's a new twist on a house call!</p>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<br />");
                                        bodyMsg.Append("<br />");
                                        DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", model.UserName, "", "", "Account Activation", bodyMsg.ToString(), false, "", 0);
                                        */

                                        return RedirectToAction("PreBookYourAppointmentSecond");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "This user already registered as Patient.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Please accept the terms of service.");
                            }
                        //}
                        //else
                        //{

                        //    ModelState.AddModelError("", "Some data are not valid.");
                        //}
                    }
                    else
                    {

                        ModelState.AddModelError("", "Please enter User State or Country.");
                    }

                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });

            }
            return View(model);
        }




        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ClinicPreBookYourAppointment(BookingAppointmentModelOuter model)
        {


            try
            {
                //if (this.IsCaptchaValid("Captcha is not valid"))
                RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

                if (String.IsNullOrEmpty(recaptchaHelper.Response))
                {
                    ModelState.AddModelError("", "Captcha value cannot be empty.");
                    return View(model);
                }

                RecaptchaVerificationResult recaptchaResult = await recaptchaHelper.VerifyRecaptchaResponseTaskAsync();

                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    ModelState.AddModelError("", "Incorrect captcha value.");
                    return View(model);
                }
                else
                {
                    if (model.USERSTATE != null || model.COUNTRY != null)
                    {
                        if (model.USERSTATE != null)
                        {
                            var data = Convert.ToString(Session["DOCTORID"]);
                            //var docuserstate = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == data). USERSTATE;
                            var docuserstate = uow.Context.DoctorsInformations.FirstOrDefault(f => f.UserName == data).STATELICENSE;
                            if (model.USERSTATE != docuserstate)
                            {
                                ModelState.AddModelError("", "You cannot register for different state doctors.");
                                return View(model);
                            }
                        }
                        //if (ModelState.IsValid)
                        //{
                            if (model.AGREETERMS)
                            {
                                USERSINFORMATION user = uws.Context.UsersInformations.FirstOrDefault(f => f.UserName == model.UserName);
                                MembershipCreateStatus createStatus = MembershipCreateStatus.UserRejected;
                                if (user == null)
                                    createStatus = account.CreateUser(model.UserName, model.PASSWORDS, model.UserName);

                                if (createStatus == MembershipCreateStatus.Success)
                                {

                                    using (var repo = new PatientInformationRepository(uws))
                                    {
                                        Session["PatientID"] = model.UserName;
                                        user = uws.Context.UsersInformations.FirstOrDefault(f => f.UserName == model.UserName);
                                        USERPASSWORD userpwd = new USERPASSWORD();
                                        StringBuilder bodyMsg = new StringBuilder();
                                        user.PASSWORDHASH = user.PASSWORDHASH;
                                        user.PASSWORDSALT = user.PASSWORDSALT;
                                        user.DateLastActivity = DateTime.UtcNow;
                                        user.DateLastLogin = DateTime.UtcNow;
                                        user.DateLastPasswordChange = DateTime.UtcNow;
                                        user.IsApproved = false;
                                        user.FIRSTNAME = model.PATFIRSTNAME;
                                        user.LASTNAME = model.PATLASTNAME;
                                        user.GENDER = model.GENDER;
                                        //model.DOB = new DateTime(Convert.ToInt32(model.DOBYEAR), Convert.ToInt32(model.DOBMonth), Convert.ToInt32(model.DOBDAY));
                                        //user.DOB = model.DOB;
                                        user.USERTYPE = "P";
                                        //user.STREETADDRESS = model.USERSTREETADDRESS1;
                                        //user.CITY = model.CITY;
                                        user.COUNTRY = model.COUNTRY;
                                        user.USERSTATE = model.USERSTATE;
                                        //user.ZIPCODE = model.ZIPCODE;
                                        user.UserName = model.UserName;
                                        user.Email = model.UserName;
                                        user.SECURITYQESTID1 = model.SECURITYQESTID1;
                                        user.ANSWER1 = model.ANSWER1;
                                        //user.SECURITYQESTID2 = model.SECURITYQESTID2;
                                        //user.ANSWER2 = model.ANSWER2;
                                        user.AGREETERMS = model.AGREETERMS;
                                        user.USERPHOTOFILEPATH = "/Images/profile_doctor.jpg";
                                        user.VERIFICATIONCODE = GenerateRandom.genereteRandomNumber();
                                        user.CREATEDBY = model.UserName;
                                        user.MODIFIEDBY = model.UserName;
                                        user.DateCreated = DateTime.UtcNow;
                                        user.MODIFIEDON = DateTime.UtcNow;
                                        user.REGISTEREDON = DateTime.UtcNow;
                                        //user.CurrenttimeZone = model.CurrenttimeZone;
                                        //user.Salutation = model.Salutation;
                                        user.HITCOUNT = "0";
                                        user.State = State.Modified;

                                        PATIENTINFORMATION patientInformation = new PATIENTINFORMATION();
                                        patientInformation.MODIFIEDBY = model.UserName;
                                        patientInformation.DateCreated = DateTime.UtcNow;
                                        patientInformation.MODIFIEDON = DateTime.UtcNow;
                                        patientInformation.CREATEDBY = model.UserName;
                                        patientInformation.POVEMAIL = user.UserName;
                                        patientInformation.PATIENTTYPE = "P";
                                        patientInformation.State = DOCVIDEO.ObjectState.State.Added;
                                        patientInformation.UserName = user.UserName;

                                        user.PatientInformations.Add(patientInformation);

                                        userpwd.CREATEDBY = model.UserName;
                                        userpwd.MODIFIEDON = DateTime.UtcNow;
                                        userpwd.DateCreated = DateTime.UtcNow;
                                        userpwd.MODIFIEDBY = model.UserName;
                                        userpwd.State = State.Added;
                                        userpwd.UserName = model.UserName;
                                        userpwd.EXPIRYDATE = DateTime.UtcNow.AddMonths(1);
                                        userpwd.PASSWORDHASH = user.PASSWORDHASH;
                                        userpwd.PASSWORDSALT = user.PASSWORDSALT;
                                        userpwd.STATUS = "A";
                                        user.UserPasswords.Add(userpwd);
                                        repo.InsertOrUpdate(user);
                                        uws.Save();

                                        string comment = "ClinicPreBookYourAppointment.";
                                        string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                                        //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.ClinicPreBookYourAppointment, 0, 0, model.UserName, data);
                                    

                                        /*
                                        bodyMsg.Append(" <style>");
                                        bodyMsg.Append("ul.a {list-style-type:circle;color:#d3d3d3;margin-left:20px;margin-bottom:20px;}");
                                        bodyMsg.Append("#logo {background: url(" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Content/Images/doccare_logo.jpg) no-repeat;width: 211px;height: 38px;cursor: pointer;margin:0px;}");
                                        bodyMsg.Append("</style>");
                                        bodyMsg.Append(" <div style='background-color: #5d5d5d; box-shadow:0px 1px 3px 1px #8e8c8c;padding:20px;'>");
                                        bodyMsg.Append("<div style='background-color: #ffffff;width:700px;box-shadow:0px 1px 3px 1px #8e8c8c; font-family: 'Ubuntu', sans-serif;padding-bottom:40px;border:1px solid #aaa;'>");
                                        bodyMsg.Append("<div style='width:100%;background-color:#fff;height:38px;'>");
                                        bodyMsg.Append(" <div id='logo'></div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append(" <div style='background-color:#3fb6d8;width:100%;height:30px;text-align:center;color:#fff;font-size:18px;padding-top:15px;'>");
                                        bodyMsg.Append("<strong>Please confirm your subscription with doccare online</strong>");

                                        bodyMsg.Append(" </div>");
                                        bodyMsg.Append("<div style='padding:20px;border:1px solid #aaa;box-shadow:0px 1px 3px 1px #8e8c8c;background-color: #ffffff;'>");
                                        bodyMsg.Append("<div style='float:right; margin:20px 20px 0px 0px;'>" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                                        bodyMsg.Append("<div style='margin-top:30px;'>");
                                        bodyMsg.Append("<p style='margin:50px 0px 10px 0px;'> Dear,<h3><strong style='text-transform:uppercase; margin-left:2px;' >" + model.FIRSTNAME + " " + model.LASTNAME + "</strong></h3></p>");
                                        bodyMsg.Append("<H3 style='color:#3fb6d8;margin-bottom:20px;'><strong>Thank you for joining doccare online");
                                        bodyMsg.Append("Here are some of the benefits you will Recieve</strong></H3>");
                                        bodyMsg.Append("<ul class='a' style='margin:0px 0px 20px 20px;color:#d3d3d3;list-style-type:circle;'>");
                                        bodyMsg.Append("<li>Doctor will be able to monitor non-urgent issues");
                                        bodyMsg.Append("conveniently and inexpensively  </li><br />");
                                        bodyMsg.Append("<li>There is NO membership free and you can sign up and<br />");
                                        bodyMsg.Append("create an account for FREE   </li><br />");
                                        bodyMsg.Append("<li>Doctors can write your prescription or medication and send ");
                                        bodyMsg.Append("it electronically to the pharmacy's.</li>");
                                        bodyMsg.Append("</ul>");
                                        bodyMsg.Append("<p >");
                                        bodyMsg.Append("<a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn?id=" + user.VERIFICATIONCODE + "'style='color:#0094ff;margin-right:10px;'> Click here </a>");
                                        bodyMsg.Append("to begin setting up your account and talk to a doctor right away.");
                                        bodyMsg.Append("</p>");
                                        bodyMsg.Append("<p style='margin:20px 0px 0px 0px;'>doccare online is an online care system that allows you to comment to patients immediately,");
                                        bodyMsg.Append("whenever and wherever.Our innovative advanced web-based technologies remove traditional");
                                        bodyMsg.Append("barriers to healthcare access.Now that's a new twist on a house call!</p>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<br />");
                                        bodyMsg.Append("<br />");
                                        DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", model.UserName, "", "", "Account Activation", bodyMsg.ToString(), false, "", 0);

                                        */
                                        return RedirectToAction("ClinicPreBookYourAppointmentSecond");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "This user already registered as Patient.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Please accept the terms of service.");
                            }
                        //}
                        //else
                        //{

                        //    ModelState.AddModelError("", "Some data are not valid.");
                        //}
                    }
                    else
                    {

                        ModelState.AddModelError("", "Please enter User State or Country.");
                    }

                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });

            }
            return View(model);
        }
        [Authorize]
        public ActionResult _ConfirmPayForVisit()
        {
            return View();
        }


        public JsonResult FirstLoginUpdate()
        {
            try
            {
                USERSINFORMATION user = null;


                user = uow.Context.UserInformations.FirstOrDefault(f => f.UserName == User.Identity.Name);
                using (UserRepository repo4 = new UserRepository(uow))
                {
                    user.IsfirstLogin = "false";
                    user.State = State.Modified;
                    repo4.InsertOrUpdateGraph(user);
                    uow.Save();
                }
            }
            catch (Exception)
            {
                return Json(new { msg = "Please try Again" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult SubscriptionInfo()
        {
            if (UsertypeDoc() != "D")
            {
                return RedirectToAction("LogOn", "Account");
            }
            DoctorsInformationEditModel temp = DoctorDisplayData(User.Identity.Name);
            temp.inboxUreadMessageCount = messagecountvalue();
            temp.profilecompleted = ProfilePercent(User.Identity.Name).ToString();
            //using (var uow = new DocSubscriptionUnitOfWork())
            //{
            //    temp.DOCSUBSCRIPTION = uow.Context.DOCSUBSCRIPTIONS.FirstOrDefault(f => f.UserName == User.Identity.Name);
            //    if (temp != null)
            //    {
            //       return RedirectToAction("AccountSettings", "AccountSettings");
            //    }
            //}
            return View(temp);
        }



        [Authorize]
        [HttpPost]
        public ActionResult SubscriptionInfo( DoctorsInformationEditModel model)
        {
           USERSINFORMATION user = null;
              

              //if(model!=null)
              //{
              //    try
              //    {
              //        PaySample samplePay = new PaySample();
              //        PreapprovalResponse response = null;
              //        DateTime subscriptionDate = GetLocalTime(DateTime.UtcNow, model.UserName);

              //        response = samplePay.PreapprovalAPIOperation(Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["SubscriptionRate"]), model.UserName, subscriptionDate);

              //        if (response.error.Count > 0)
              //        {
              //            string message = null;
              //            foreach (var err in response.error)
              //            {
              //                message += err + "/n";
              //            }
              //            ModelState.AddModelError("", message);
              //            return View(model);
              //        }
              //        else
              //        {
              //            if (!string.IsNullOrEmpty(response.preapprovalKey))
              //            {
              //                //make the payment using preapproval key
              //                PayPalModel payModel = new PayPalModel();
              //                payModel.DOCTORID = model.UserName;
              //                payModel.RATE = System.Configuration.ConfigurationManager.AppSettings["SubscriptionRate"];
              //                payModel.PAYPALKEY = response.preapprovalKey;
              //               // payModel.PAYPALID = model.PAYPALEMAIL;
              //                Session["PayModel"] = payModel;

              //                try
              //                {
              //                    using (var uow = new DocSubscriptionUnitOfWork())
              //                    {
              //                        using (var repo = new DocSubscriptionRepository(uow))
              //                        {
              //                            DOCSUBSCRIPTION docSubscritpion = new DOCSUBSCRIPTION();
              //                            docSubscritpion.UserName = model.UserName;
              //                            docSubscritpion.State = State.Added;
              //                            docSubscritpion.PreapprovalKey = response.preapprovalKey;
              //                            docSubscritpion.CREATEDDATE = DateTime.UtcNow;
              //                            docSubscritpion.CREATEDBY = model.UserName;
              //                            docSubscritpion.MODIFIEDDATE = DateTime.UtcNow;
              //                            docSubscritpion.MODIFIEDBY = model.UserName;
              //                            repo.InsertOrUpdate(docSubscritpion);
              //                            uow.Save();
              //                        }
              //                    }
              //                }
              //                catch (Exception)
              //                {

              //                }
              //            }
              //        }
              //    }
              //    catch (Exception ex)
              //    {
              //        throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
              //    }
              //}
              //else
              //{
              //    ModelState.AddModelError("", "error");
              //}

              return View(model);        
        }

        [Authorize]
        [HttpGet]
        public ActionResult PayPalSubscription(string userId, string pstatus, string rate)
        {
            PayPalModel model = new PayPalModel();
            //model.DOCTORID=userId;
            //model.STATUS=pstatus;
            //model.RATE = rate;


            ////make subscription payment

            //if (Session["PayModel"] != null)
            //{
            //    PayPalModel p = Session["PayModel"] as PayPalModel;
            //    if (pstatus.ToUpper().Equals("CANCEL"))
            //    {
            //        try
            //        {
            //            using (var uow = new DocSubscriptionUnitOfWork())
            //            {
            //                using (var repo = new DocSubscriptionRepository(uow))
            //                {
            //                    var temp = uow.Context.DOCSUBSCRIPTIONS.FirstOrDefault(f => f.PreapprovalKey == p.PAYPALKEY);
            //                    if (temp != null)
            //                    {
            //                        repo.Delete(Convert.ToInt32(temp.DOCSUBSCRIPTIONID));

            //                        uow.Save();
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            Session["PayModel"] = null;
            //            //throw;
            //        }

            //    }
            //    else
            //    {
            //        try
            //        {
            //            //make the payment using preapproval key
            //            PaySample makePymt = new PaySample();
            //            PayRequest req = makePymt.PreapprovalPayment(p.PAYPALKEY, p.PAYPALID, p.DOCTORID, Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["SubscriptionRate"]));
            //            PayResponse resp = makePymt.PayAPIOperations(req, true);

            //            if (resp.error.Count > 0)
            //            {
            //                string message = null;
            //                foreach (var err in resp.error)
            //                {
            //                    message += err + "/n";
            //                }
            //                ModelState.AddModelError("", message);
            //            }

            //        }
            //        catch (Exception)
            //        {
            //            Session["PayModel"] = null;
            //           // throw;
            //        }
            //    }
            //    Session["PayModel"] = null;
            //}
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult PayPalIPN()
        {

            try
            {
                
                //Post back to either sandbox or live
                string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                string strLive = "https://www.paypal.com/cgi-bin/webscr";
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strSandbox);
                //Set values for the request back
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] param = Request.BinaryRead(System.Web.HttpContext.Current.Request.ContentLength);
                string strRequest = Encoding.ASCII.GetString(param);
                if (!string.IsNullOrEmpty(strRequest))
                {
                    using (var uow = new PayPalIPNUnitOfWork())
                    {
                        using (var repo = new PayPalIPNServiceRepository(uow))
                        {
                            PAYPALIPN ipn = new PAYPALIPN();
                            ipn.PAYPALRESPONSE = strRequest;
                            ipn.CREATEDBY = "SYSTEM";
                            ipn.CREATEDDATE = DateTime.UtcNow;
                            ipn.MODIFIEDBY = "SYSTEM";
                            ipn.MODIFIEDDATE = DateTime.UtcNow;

                            repo.InsertOrUpdate(ipn);
                            uow.Save();
                        }
                    }
                    string comment = "PayPal IPN Verification.";
                    string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                    //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.PayPalIPNVerification,0,0,data);
                }
            }
            catch (Exception ex)
            {
                
            }

            return View();
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult PayPalIPN()
        //{

        //    try
        //    {

        //        //Post back to either sandbox or live
        //        string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
        //        string strLive = "https://www.paypal.com/cgi-bin/webscr";
        //        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strSandbox);
        //        //Set values for the request back
        //        req.Method = "POST";
        //        req.ContentType = "application/x-www-form-urlencoded";
        //        byte[] param = Request.BinaryRead(System.Web.HttpContext.Current.Request.ContentLength);
        //        string strRequest = Encoding.ASCII.GetString(param);
        //        string strResponse_copy = strRequest;  //Save a copy of the initial info sent by PayPal
        //        strRequest += "&cmd=_notify-validate";
        //        req.ContentLength = strRequest.Length;

        //        //for proxy
        //        //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
        //        //req.Proxy = proxy;
        //        //Send the request to PayPal and get the response
        //        StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
        //        streamOut.Write(strRequest);
        //        streamOut.Close();
        //        StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
        //        string strResponse = streamIn.ReadToEnd();
        //        streamIn.Close();

        //        if (strResponse == "VERIFIED")
        //        {
        //            NameValueCollection nvcResponse = HttpUtility.ParseQueryString(strResponse_copy);

        //            if (nvcResponse.HasKeys())
        //            {
        //                if (nvcResponse.GetKey(0).Equals("payment_request_date"))
        //                {
        //                    #region Doc Payments
        //                    string payment_request_date = nvcResponse["payment_request_date"];
        //                    string fees_payer = nvcResponse["fees_payer"];
        //                    string sender_email = nvcResponse["sender_email"];
        //                    string verify_sign = nvcResponse["verify_sign"];
        //                    string test_ipn = nvcResponse["test_ipn"];
        //                    string transaction_id_for_sender_txn = nvcResponse["transaction[0].id_for_sender_txn"];
        //                    string transaction_receiver = nvcResponse["transaction[0].receiver"];
        //                    string transaction_is_primary_receiver = nvcResponse["transaction[0].is_primary_receiver"];
        //                    string pay_key = nvcResponse["pay_key"];
        //                    string action_type = nvcResponse["action_type"];
        //                    string transactionid = nvcResponse["transaction[0].id"];
        //                    string transactionstatus = nvcResponse["transaction[0].status"];
        //                    string transactionpaymentType = nvcResponse["transaction[0].paymentType"];
        //                    string transactionstatus_for_sender_txn = nvcResponse["transaction[0].status_for_sender_txn"];
        //                    string transactionpending_reason = nvcResponse["transaction[0].pending_reason"];
        //                    string transaction_type = nvcResponse["transaction_type"];
        //                    string transactionamount = nvcResponse["transaction[0].amount"];
        //                    string status = nvcResponse["status"];
        //                    string reverse_all_parallel_payments_on_error = nvcResponse["reverse_all_parallel_payments_on_error"];

        //                    string ipn_notification_url = nvcResponse["ipn_notification_url"];

        //                    string return_url = nvcResponse["return_url"];
        //                    string userName = return_url.Substring(return_url.LastIndexOf("userId=") + 7);

        //                    if (!string.IsNullOrEmpty(userName))
        //                    {

        //                        DocPaymentUnitOfWork dpUOW = new DocPaymentUnitOfWork();
        //                        DocPaymentServiceRepository repo = new DocPaymentServiceRepository(dpUOW);

        //                        var user = dpUOW.Context.UserInformations.FirstOrDefault(w => w.UserName == userName);
        //                        if (user != null)
        //                        {
        //                            var transaction = dpUOW.Context.DOCPAYMENTS.FirstOrDefault(w => w.UserName == user.UserName && w.TRANSACTIONTYPE == transaction_type &&
        //                                w.TRANSACTIONID == transactionid && w.TRANSACTIONAMOUNT == Convert.ToDecimal(transactionamount) && w.SENDEREMAIL == sender_email);

        //                            if (transaction != null)
        //                            {
        //                                //existing transaction

        //                                //check for the status & update if status is not complete
        //                                if (!transaction.STATUS.Equals("COMPLETED"))
        //                                {
        //                                    transaction.STATUS = status;
        //                                    transaction.State = State.Modified;
        //                                    transaction.MODIFIEDBY = userName;
        //                                    transaction.MODIFIEDON = DateTime.UtcNow;

        //                                    repo.InsertOrUpdateGraph(transaction);
        //                                    dpUOW.Save();
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //new transaction
        //                                //Send email and update DB
        //                                DOCPAYMENT docPymt = new DOCPAYMENT();

        //                                // docPymt.PAYMENTREQUESTDATE = Convert.ToDateTime(payment_request_date).ToUniversalTime();
        //                                docPymt.FEESPAYER = fees_payer;
        //                                docPymt.SENDEREMAIL = sender_email;
        //                                docPymt.VERIFYSIGN = verify_sign;
        //                                docPymt.TESTIPN = test_ipn;
        //                                docPymt.TRANSACTIONIDFORSENDERTXN = transaction_id_for_sender_txn;
        //                                docPymt.TRANSACTIONRECEIVER = transaction_receiver;
        //                                docPymt.TRANSACTIONISPRIMARYRECEIVER = transaction_is_primary_receiver;
        //                                docPymt.PAYKEY = pay_key;
        //                                docPymt.ACTIONTYPE = action_type;
        //                                docPymt.TRANSACTIONID = transactionid;
        //                                docPymt.TRANSACTIONSTATUS = transactionstatus;
        //                                docPymt.TRANSACTIONPAYMENTTYPE = transactionpaymentType;
        //                                docPymt.TRANSACTIONSTATUSFORSENDERTXN = transactionstatus_for_sender_txn;
        //                                docPymt.TRANSACTIONPENDINGREASON = transactionpending_reason;
        //                                docPymt.TRANSACTIONTYPE = transaction_type;
        //                                docPymt.TRANSACTIONAMOUNT = Convert.ToDecimal(transactionamount);
        //                                docPymt.STATUS = status;
        //                                docPymt.REVERSEALLPARALLELPAYMENTSONERROR = reverse_all_parallel_payments_on_error;
        //                                docPymt.UserName = userName;

        //                                docPymt.CREATEDBY = userName;
        //                                docPymt.CURRENCYCODE = "USD";
        //                                docPymt.DateCreated = DateTime.UtcNow;
        //                                docPymt.MODIFIEDBY = userName;
        //                                docPymt.MODIFIEDON = DateTime.UtcNow;
        //                                docPymt.State = State.Added;

        //                                repo.InsertOrUpdate(docPymt);
        //                                dpUOW.Save();
        //                            }

        //                            //if (status.Equals("COMPLETED"))
        //                            //{

        //                            //}
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                else if (nvcResponse.GetKey(0).Equals("transaction_subject"))
        //                {
        //                    #region Subscription Payments
        //                    DocSubscriptionUnitOfWork dsUOW = new DocSubscriptionUnitOfWork();
        //                    var payerEmail = nvcResponse["payer_email"];
        //                    var subscriber = dsUOW.Context.DOCSUBSCRIPTIONS.FirstOrDefault(w => w.SenderEmail == payerEmail);
        //                    if (subscriber != null)
        //                    {
        //                        try
        //                        {

        //                            using (var uow = new SubscriptionPaymentUnitOfWork())
        //                            {
        //                                string transactionId = nvcResponse["txn_id"];
        //                                var pymt = uow.Context.SUBSCRIPTIONPAYMENTS.FirstOrDefault(f => f.TxnId == transactionId);

        //                                // if txn_id exists, then check the PaymentStatus, if PaymentStatus == Completed, do not update the record
        //                                //else update the records if nvcResponse["payment_status"]=="Completed"
        //                                if (pymt != null)
        //                                {
        //                                    if (!pymt.PaymentStatus.ToUpper().Equals("COMPLETED") && nvcResponse["payment_status"].ToUpper().Equals("COMPLETED"))
        //                                    {
        //                                        using (var repo = new SubscriptionPaymentRepository(uow))
        //                                        {
        //                                            pymt.TransactionSubject = nvcResponse["transaction_subject"];
        //                                            pymt.PaymentDate = nvcResponse["payment_date"];
        //                                            pymt.TxnType = nvcResponse["txn_type"];
        //                                            pymt.LastName = nvcResponse["last_name"];
        //                                            pymt.ResidenceCountry = nvcResponse["residence_country"];
        //                                            pymt.ItemName = nvcResponse["item_name"];
        //                                            pymt.PaymentGross = nvcResponse["payment_gross"] == null ? 0 : Convert.ToDecimal(nvcResponse["payment_gross"]);
        //                                            pymt.McCurrency = nvcResponse["mc_currency"];
        //                                            pymt.Business = nvcResponse["business"];
        //                                            pymt.PaymentType = nvcResponse["payment_type"];
        //                                            pymt.ProtectionEligibility = nvcResponse["protection_eligibility"];
        //                                            pymt.VerifySign = nvcResponse["verify_sign"];
        //                                            pymt.PayerStatus = nvcResponse["payer_status"];
        //                                            pymt.TestIpn = nvcResponse["test_ipn"];
        //                                            pymt.Tax = nvcResponse["tax"];
        //                                            pymt.PayerEmail = nvcResponse["payer_email"];
        //                                            pymt.Quantity = nvcResponse["quantity"] == null ? 0 : Convert.ToInt32(nvcResponse["quantity"]);
        //                                            pymt.ReceiverEmail = nvcResponse["receiver_email"];
        //                                            pymt.FirstName = nvcResponse["first_name"];
        //                                            pymt.PayerId = nvcResponse["payer_id"];
        //                                            pymt.ReceiverId = nvcResponse["receiver_id"];
        //                                            pymt.ItemNumber = nvcResponse["item_number"];
        //                                            pymt.PaymentStatus = nvcResponse["payment_status"];
        //                                            pymt.PaymentFee = nvcResponse["payment_fee"] == null ? 0 : Convert.ToDecimal(nvcResponse["payment_fee"]);
        //                                            pymt.McFee = nvcResponse["mc_fee"] == null ? 0 : Convert.ToDecimal(nvcResponse["mc_fee"]);
        //                                            pymt.McGross = nvcResponse["mc_gross"] == null ? 0 : Convert.ToDecimal(nvcResponse["mc_gross"]);
        //                                            pymt.Custom = nvcResponse["custom"];
        //                                            pymt.Charset = nvcResponse["charset"];
        //                                            pymt.NotifyVersion = nvcResponse["notify_version"];
        //                                            pymt.IpnTrackId = nvcResponse["ipn_track_id"];
        //                                            //pymt.PendingReason = nvcResponse[""];
        //                                            //pymt.ERRORDETAILS = nvcResponse[""];
        //                                            pymt.UserName = subscriber.UserName;

        //                                            pymt.MODIFIEDDATE = DateTime.UtcNow;
        //                                            pymt.MODIFIEDBY = "SYSTEM";
        //                                            pymt.State = State.Modified;
        //                                            repo.InsertOrUpdate(pymt);
        //                                            uow.Save();
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    // insert new record to the table 
        //                                    SUBSCRIPTIONPAYMENT sp = new SUBSCRIPTIONPAYMENT();
        //                                    using (var repo = new SubscriptionPaymentRepository(uow))
        //                                    {
        //                                        sp.TransactionSubject = nvcResponse["transaction_subject"];
        //                                        sp.PaymentDate = nvcResponse["payment_date"];
        //                                        sp.TxnType = nvcResponse["txn_type"];
        //                                        sp.LastName = nvcResponse["last_name"];
        //                                        sp.ResidenceCountry = nvcResponse["residence_country"];
        //                                        sp.ItemName = nvcResponse["item_name"];
        //                                        sp.PaymentGross = nvcResponse["payment_gross"] == null ? 0 : Convert.ToDecimal(nvcResponse["payment_gross"]);
        //                                        sp.McCurrency = nvcResponse["mc_currency"];
        //                                        sp.Business = nvcResponse["business"];
        //                                        sp.PaymentType = nvcResponse["payment_type"];
        //                                        sp.ProtectionEligibility = nvcResponse["protection_eligibility"];
        //                                        sp.VerifySign = nvcResponse["verify_sign"];
        //                                        sp.PayerStatus = nvcResponse["payer_status"];
        //                                        sp.TestIpn = nvcResponse["test_ipn"];
        //                                        sp.Tax = nvcResponse["tax"];
        //                                        sp.PayerEmail = nvcResponse["payer_email"];
        //                                        sp.Quantity = nvcResponse["quantity"] == null ? 0 : Convert.ToInt32(nvcResponse["quantity"]);
        //                                        sp.ReceiverEmail = nvcResponse["receiver_email"];
        //                                        sp.FirstName = nvcResponse["first_name"];
        //                                        sp.PayerId = nvcResponse["payer_id"];
        //                                        sp.ReceiverId = nvcResponse["receiver_id"];
        //                                        sp.ItemNumber = nvcResponse["item_number"];
        //                                        sp.PaymentStatus = nvcResponse["payment_status"];
        //                                        sp.PaymentFee = nvcResponse["payment_fee"] == null ? 0 : Convert.ToDecimal(nvcResponse["payment_fee"]);
        //                                        sp.McFee = nvcResponse["mc_fee"] == null ? 0 : Convert.ToDecimal(nvcResponse["mc_fee"]);
        //                                        sp.McGross = nvcResponse["mc_gross"] == null ? 0 : Convert.ToDecimal(nvcResponse["mc_gross"]);
        //                                        sp.Custom = nvcResponse["custom"];
        //                                        sp.Charset = nvcResponse["charset"];
        //                                        sp.NotifyVersion = nvcResponse["notify_version"];
        //                                        sp.IpnTrackId = nvcResponse["ipn_track_id"];
        //                                        sp.TxnId = nvcResponse["txn_id"];
        //                                        sp.UserName = subscriber.UserName;
        //                                        sp.CREATEDBY = "SYSTEM";
        //                                        sp.CREATEDDATE = DateTime.UtcNow;
        //                                        sp.MODIFIEDDATE = DateTime.UtcNow;
        //                                        sp.MODIFIEDBY = "SYSTEM";
        //                                        sp.State = State.Added;
        //                                        repo.InsertOrUpdate(sp);
        //                                        uow.Save();
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {


        //                        }
        //                    }
        //                    #endregion
        //                }
        //                else if (nvcResponse.GetKey(0).Equals("max_number_of_payments"))
        //                {
        //                    #region Doc Subscriptions
        //                    using (var uow = new DocSubscriptionUnitOfWork())
        //                    {
        //                        if (nvcResponse["preapproval_key"] == null)
        //                            return View();
        //                        var preapprovalKey = nvcResponse["preapproval_key"];
        //                        var subscriber = uow.Context.DOCSUBSCRIPTIONS.FirstOrDefault(w => w.PreapprovalKey == preapprovalKey);
        //                        if (subscriber != null)
        //                        {
        //                            try
        //                            {
        //                                if (subscriber.Status == null && subscriber.VerifySign == null)
        //                                {
        //                                    using (var repo = new DocSubscriptionRepository(uow))
        //                                    {

        //                                        subscriber.MaxNumberOfPayments = nvcResponse["max_number_of_payments"] == null ? 0 : Convert.ToInt32(nvcResponse["max_number_of_payments"]);
        //                                        subscriber.StartingDate = Convert.ToDateTime(nvcResponse["starting_date"]);
        //                                        subscriber.PinType = nvcResponse["pin_type"];
        //                                        subscriber.MaxAmountPerPayment = nvcResponse["max_amount_per_payment"] == null ? 0 : Convert.ToDecimal(nvcResponse["max_amount_per_payment"]);
        //                                        subscriber.CurrencyCode = nvcResponse["currency_code"];
        //                                        subscriber.SenderEmail = nvcResponse["sender_email"];
        //                                        subscriber.VerifySign = nvcResponse["verify_sign"];
        //                                        subscriber.TestIpn = nvcResponse["test_ipn"];
        //                                        subscriber.DateOfMonth = nvcResponse["date_of_month"] == null ? 0 : Convert.ToInt32(nvcResponse["date_of_month"]);
        //                                        subscriber.CurrentNumberOfPayments = nvcResponse["current_number_of_payments"] == null ? 0 : Convert.ToInt32(nvcResponse["current_number_of_payments"]);
        //                                        subscriber.EndingDate = Convert.ToDateTime(nvcResponse["ending_date"]);
        //                                        subscriber.IsApproved = nvcResponse["approved"] == null ? false : Convert.ToBoolean(nvcResponse["approved"]);
        //                                        subscriber.TransactionType = nvcResponse["transaction_type"];
        //                                        subscriber.DayOfWeek = nvcResponse["day_of_week"];
        //                                        subscriber.Status = nvcResponse["status"];
        //                                        subscriber.CurrentTotalAmountOfAllPayments = nvcResponse["current_total_amount_of_all_payments"] == null ? 0 : Convert.ToDecimal(nvcResponse["current_total_amount_of_all_payments"]);
        //                                        subscriber.CurrentPeriodAttempts = nvcResponse["current_period_attempts"] == null ? 0 : Convert.ToInt32(nvcResponse["current_period_attempts"]);
        //                                        subscriber.Charset = nvcResponse["charset"];
        //                                        subscriber.PaymentPeriod = nvcResponse["payment_period"];
        //                                        subscriber.NotifyVersion = nvcResponse["notify_version"];
        //                                        subscriber.MaxTotalAmountOfAllPayments = nvcResponse["max_total_amount_of_all_payments"] == null ? 0 : Convert.ToDecimal(nvcResponse["max_total_amount_of_all_payments"]);
        //                                        subscriber.MODIFIEDDATE = DateTime.UtcNow;
        //                                        subscriber.MODIFIEDBY = "SYSTEM";
        //                                        subscriber.State = State.Modified;
        //                                        repo.InsertOrUpdate(subscriber);
        //                                        uow.Save();
        //                                    }
        //                                }
        //                            }
        //                            catch (Exception)
        //                            {

        //                            }
        //                        }
        //                    }
        //                    #endregion
        //                }
        //            }
        //            // more checks needed here specially your account number and related stuff
        //        }
        //        else if (strResponse == "INVALID")
        //        {
        //            //log for manual investigation
        //            //send email
        //        }
        //        else
        //        {
        //            //log response/ipn data for manual investigation
        //            // send email
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return View();
        //}
    }
}