using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DOCVIDEO.Domain;
using DOCVIDEO.Utility;
using client.Web.Models;
using DOCVIDEO.ObjectState;
using DOCVIDEO.BaseDataLayer;
using DOCVIDEO.UserServiceRepoUOW;
using UserService.Repositories.Disconnected;
using System.Web.Security;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using DOCVIDEO.ErrorLoggingContext;
using DOCVIDEO.PatientInformationServiceBoundedContext;
using DOCVIDEO.PatientInformationServiceRepoUOW;
using PatientInformationService.Repositories.Disconnected;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Web.Routing;
//using DOCVIDEO.BOL;
//using DOCVIDEO.DAL;
namespace client.Web.Controllers
{
    public static class GenerateRandom
    {
        public static int genereteRandomNumber()
        {
            Random rnd = new Random();

            int card = rnd.Next(9999); // creates a number between 0 and 9999
            return card;
        }
    }

    public class RegistrationController : Controller
    {
        //
        // GET: /Registration/
        private readonly DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uw = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();
        private readonly DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
        private readonly SpecialityUnitOfWork suow = new SpecialityUnitOfWork();
        private readonly MembershipUnitOfWork muow = new MembershipUnitOfWork();
        private AccountMembershipService account = new AccountMembershipService();
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }


        public static DateTime logintime;
        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }


        [AllowAnonymous]
        public ActionResult PatientSignUp()
        {
            Session["Gridresult"] = null;
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            PatientRegistrationModel model = new PatientRegistrationModel();
            return View(model);
        }

        // GET: /Registration/DoctorSignUp
        [HttpGet]
        [AllowAnonymous]
        public ActionResult DoctorSignUp()
        {
            Session["Gridresult"] = null;
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            //double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.Now)) ? 1 : 0);
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            DoctorsRegistrationModel model = new DoctorsRegistrationModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult PatientSignUp(PatientRegistrationModel model)
        {
            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(timezone);

            //double offSetHours = tzi.BaseUtcOffset.TotalHours + ((tzi.IsDaylightSavingTime(DateTime.Now)) ? 1 : 0);
            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;
            try
            {
                //if (this.IsCaptchaValid("Captcha is not valid"))
               // RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

                //if (String.IsNullOrEmpty(recaptchaHelper.Response))
                //{
                //    ModelState.AddModelError("", "Error :Captcha value cannot be empty.");
                //    return View(model);
                //}

               // RecaptchaVerificationResult recaptchaResult = await recaptchaHelper.VerifyRecaptchaResponseTaskAsync();

                //if (recaptchaResult != RecaptchaVerificationResult.Success)
                //{
                //    ModelState.AddModelError("", "Error :Incorrect captcha value.");
                //    return View(model);
                //}
                //else
                //{
                    if (model.USERSTATE != null || model.COUNTRY != null)
                    {
                        if (ModelState.IsValid)
                        {
                            if (model.AGREETERMS)
                            {
                                USERSINFORMATION user = uw.Context.UsersInformations.SingleOrDefault(f => f.UserName == model.UserName);
                                MembershipCreateStatus createStatus = MembershipCreateStatus.UserRejected;
                                if (user == null)
                                    createStatus = account.CreateUser(model.UserName, model.PASSWORD, model.UserName);

                                if (createStatus == MembershipCreateStatus.Success)
                                {

                                    using (var repo = new PatientInformationRepository(uw))
                                    {
                                        user = uw.Context.UsersInformations.SingleOrDefault(f => f.UserName == model.UserName);
                                        USERPASSWORD userpwd = new USERPASSWORD();
                                        StringBuilder bodyMsg = new StringBuilder();
                                        user.PASSWORDHASH = user.PASSWORDHASH;
                                        user.PASSWORDSALT = user.PASSWORDSALT;
                                        user.DateLastActivity = DateTime.UtcNow;
                                        user.DateLastLogin = DateTime.UtcNow;
                                        user.DateLastPasswordChange = DateTime.UtcNow;
                                        user.IsApproved = false;
                                        user.IsfirstLogin = "true";
                                        user.FIRSTNAME = model.FIRSTNAME;
                                        user.LASTNAME = model.LASTNAME;
                                        user.GENDER = model.GENDER;
                                        //model.DOB = new DateTime(Convert.ToInt32(model.DOBYEAR), Convert.ToInt32(model.DOBMonth), Convert.ToInt32(model.DOBDAY));
                                        //user.DOB = model.DOB;
                                        user.USERTYPE = "P";
                                        //user.STREETADDRESS = model.USERSTREETADDRESS1;
                                        //user.CITY = model.CITY;
                                        user.COUNTRY = model.COUNTRY;
                                        if (model.COUNTRY != null && model.COUNTRY == "United States")
                                        {
                                            user.USERSTATE = model.USERSTATE;
                                        }
                                        //user.ZIPCODE = model.ZIPCODE;
                                        user.UserName = model.UserName;
                                        user.Email = model.UserName;
                                        user.SECURITYQESTID1 = model.SECURITYQESTID1;
                                        user.ANSWER1 = model.ANSWER1;
                                        //user.SECURITYQESTID2 = model.SECURITYQESTID2;
                                        //user.ANSWER2 = model.ANSWER2;
                                        user.AGREETERMS = true;
                                        user.USERPHOTOFILEPATH = "/Images/profile_doctor.jpg";
                                        user.VERIFICATIONCODE = GenerateRandom.genereteRandomNumber();
                                        user.CREATEDBY = User.Identity.Name;
                                        user.MODIFIEDBY = User.Identity.Name;
                                        user.DateCreated = DateTime.UtcNow;
                                        user.MODIFIEDON = DateTime.UtcNow;
                                        user.REGISTEREDON = DateTime.UtcNow;
                                        //user.CurrenttimeZone = model.CurrenttimeZone;
                                        //user.Salutation = model.Salutation;
                                        user.HITCOUNT = "0";
                                        user.State = State.Modified;

                                        PATIENTINFORMATION patientInformation = new PATIENTINFORMATION();
                                        patientInformation.MODIFIEDBY = User.Identity.Name;
                                        patientInformation.DateCreated = DateTime.UtcNow;
                                        patientInformation.MODIFIEDON = DateTime.UtcNow;
                                        patientInformation.CREATEDBY = User.Identity.Name;
                                        patientInformation.POVEMAIL = user.UserName;
                                        patientInformation.PATIENTTYPE = "P";
                                        patientInformation.State = DOCVIDEO.ObjectState.State.Added;
                                        patientInformation.UserName = user.UserName;

                                        user.PatientInformations.Add(patientInformation);

                                        userpwd.CREATEDBY = User.Identity.Name;
                                        userpwd.MODIFIEDON = DateTime.UtcNow;
                                        userpwd.DateCreated = DateTime.UtcNow;
                                        userpwd.MODIFIEDBY = User.Identity.Name;
                                        userpwd.State = State.Added;
                                        userpwd.UserName = model.UserName;
                                        userpwd.EXPIRYDATE = DateTime.UtcNow.AddMonths(1);
                                        userpwd.PASSWORDHASH = user.PASSWORDHASH;
                                        userpwd.PASSWORDSALT = user.PASSWORDSALT;
                                        userpwd.STATUS = "A";
                                        user.UserPasswords.Add(userpwd);

                                        repo.InsertOrUpdate(user);
                                        uw.Save();
                                        string comment = "Patient SignUp.";
                                        string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                                        //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.PatientSignUp, 0, 0, model.UserName, data);
                       
                                        /*

                                        bodyMsg.Append("<div style='width: 750px; background-color: #fff; color: #4d4d4d; border: 1px solid #999; font-size: 14px; font-family: serif; letter-spacing: 0.02em;'>");
                                        bodyMsg.Append("<div style='height: 50px; border-bottom: 1px solid #aaa; background: #D3D0D0; padding: 8px;'>");
                                        bodyMsg.Append("<div>");
                                        bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='docare logo' /></div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='padding: 10px;'>");
                                        bodyMsg.Append("<div style='text-align: right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                                        bodyMsg.Append("<h2 style='color: #1072B5; font-style: italic;'>Dear," + model.FIRSTNAME + " " + model.LASTNAME + "</h2>");
                                        bodyMsg.Append("<div style='float: left; width: 490px;'>");
                                        bodyMsg.Append("<div>Thank you for signing up with docare. <br/>");
                                        bodyMsg.Append("docare online is an online care system that allows patients to connect with doctors immediately, whenever and wherever.");
                                        bodyMsg.Append("Please  <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn?id=" + user.VERIFICATIONCODE + " 'title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your docare account!");
                                        bodyMsg.Append("Account.");
                                        bodyMsg.Append(" </p>");
                                        bodyMsg.Append("</div>");
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
                                        bodyMsg.Append("<div style='color: #1072B5; text-align: center; font-size: 19px; font-style: italic;'>");
                                        bodyMsg.Append("<strong>See your Doctors, anytime, anywhere!</strong>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<p>");
                                        bodyMsg.Append(" If you have questions about docare online, please email us at");
                                        bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color: #1072B5;'>  helpdesk@PROFESSORSONLINE.com.</a> For FAQ, please visit <a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color: #1072B5;'> www.PROFESSORSONLINE.com/faq.</a>");
                                        bodyMsg.Append("</p>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='float: right; margin-left: 10px; width: 230px;'>");
                                        bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image' />");
                                        bodyMsg.Append("<div style='color: #1072B5; text-align: center; font-size: 19px; font-style: italic;'>docare online helps doctors and patients connect securely anytime, anywhere!</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='clear: both;'></div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='font-size: 12px; background: #D3D0D0; padding: 10px; clear: both; margin-top: 10px;'>");
                                        bodyMsg.Append("<strong>Disclaimer:</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality <strong>could subject you to penalties described in Federal and State law.</strong>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");

                                        //Mail to pateint
                                        DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", model.UserName, "", "", "Welcome to docare online", bodyMsg.ToString(), false, "", 0);
                                        */
                                        return RedirectToAction("RegistrationSuccess", "Account");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Error :This user is already registered.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error : Please accept the terms of service.");
                            }
                        }
                        else
                        {

                            ModelState.AddModelError("", "Error: Some data are not valid.");
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("", "Error: Please enter User State or Country.");
                    }

                //}
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });

            }
            return View(model);
        }



        //POST: /Registration/DoctorSignUp
        [HttpPost]
        [AllowAnonymous]
        public ActionResult DoctorSignUp(DoctorsRegistrationModel model)
        {

            ReadOnlyCollection<TimeZoneInfo> tz = TimeZoneInfo.GetSystemTimeZones();
            List<SelectListItem> items = new List<SelectListItem>();
            DOCTORSINFORMATION docInformation = new DOCTORSINFORMATION();
            USERPASSWORD userpwd = new USERPASSWORD();
            DOCTORSTATUS docstatus = new DOCTORSTATUS();
            //StringBuilder bodyMsg = new StringBuilder();
            //StringBuilder bodyMsgAdmin = new StringBuilder();

            foreach (TimeZoneInfo tzi in tz)
            {

                items.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id });

            }
            ViewBag.CurrenttimeZone = items;



            try
            {
                //if (this.IsCaptchaValid("Captcha is not valid"))
               //RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

                //if (String.IsNullOrEmpty(recaptchaHelper.Response))
                //{
                //    ModelState.AddModelError("", "Error :Captcha answer cannot be empty.");
                //    return View(model);
                //}

                //RecaptchaVerificationResult recaptchaResult = await recaptchaHelper.VerifyRecaptchaResponseTaskAsync();

                //if (recaptchaResult != RecaptchaVerificationResult.Success)
                //{
                //    ModelState.AddModelError("", "Error :Incorrect captcha answer.");
                //    return View(model);
                //}
                //else
                //{
                    if (ModelState.IsValid)
                    {

                        if (model.AGREETERMS)
                        {
                            USERSINFORMATION user = uow.Context.UserInformations.SingleOrDefault(f => f.UserName == model.UserName);
                            MembershipCreateStatus createStatus = MembershipCreateStatus.UserRejected;
                            if (user == null)
                                createStatus = account.CreateUser(model.UserName, model.PASSWORD, model.UserName);
                            if (createStatus == MembershipCreateStatus.Success)
                            {

                                using (UserRepository repo = new UserRepository(uow))
                                {

                                    user = uow.Context.UserInformations.SingleOrDefault(f => f.UserName == model.UserName);

                                    user.PASSWORDHASH = user.PASSWORDHASH;
                                    user.PASSWORDSALT = user.PASSWORDSALT;
                                    user.DateLastActivity = DateTime.UtcNow;
                                    user.DateLastLogin = DateTime.UtcNow;
                                    user.DateLastPasswordChange = DateTime.UtcNow;
                                    user.IsApproved = false;
                                    user.FIRSTNAME = model.FIRSTNAME;
                                    user.LASTNAME = model.LASTNAME;
                                    user.GENDER = model.GENDER;
                                    user.IsfirstLogin = "true";
                                    //model.DOB = new DateTime(Convert.ToInt32(model.DOBYEAR), Convert.ToInt32(model.DOBMonth), Convert.ToInt32(model.DOBDAY));
                                    //user.DOB = model.DOB;
                                    user.USERTYPE = "D";
                                    user.COUNTRY = "United States";
                                    //user.STREETADDRESS = model.USERSTREETADDRESS1;
                                    //user.CITY = model.CITY;
                                    //user.USERSTATE = model.USERSTATE;
                                    //user.ZIPCODE = model.ZIPCODE;
                                    user.UserName = model.UserName;
                                    user.Email = model.UserName;
                                    user.SECURITYQESTID1 = model.SECURITYQESTID1;
                                    user.ANSWER1 = model.ANSWER1;
                                    //user.SECURITYQESTID2 = model.SECURITYQESTID2;
                                    //user.ANSWER2 = model.ANSWER2;
                                    user.AGREETERMS = true;
                                    user.VERIFICATIONCODE = GenerateRandom.genereteRandomNumber();
                                    user.CREATEDBY = model.UserName;
                                    user.MODIFIEDBY = model.UserName;
                                    user.DateCreated = DateTime.UtcNow;
                                    user.MODIFIEDON = DateTime.UtcNow;
                                    user.USERPHOTOFILEPATH = "/Images/profile_doctor.jpg";
                                    user.REGISTEREDON = DateTime.UtcNow;
                                    user.AverageRating = Convert.ToDecimal(0);
                                    //user.CurrenttimeZone = model.CurrenttimeZone;
                                    //user.Salutation = model.Salutation;
                                    user.HITCOUNT = "0";
                                    user.State = State.Modified;

                                    docstatus.DateCreated = DateTime.UtcNow;
                                    docstatus.LICENSEEXPIRYDATE = DateTime.UtcNow;//model.LICENSEEXPIRESON;
                                    docstatus.MODIFIEDBY = model.UserName;
                                    docstatus.MODIFIEDON = DateTime.UtcNow;
                                    docstatus.State = State.Added;
                                    docstatus.STATUS = true;
                                    docstatus.UserName = model.UserName;
                                    docstatus.CREATEDBY = model.UserName;


                                    docInformation.LICENSE = model.LICENSE;
                                    docInformation.STATELICENSE = "India";// model.STATELICENSE;
                                    docInformation.LICENSEEXPIRESON = model.LICENSEEXPIRESON;
                                    //docInformation.PAYPALEMAIL = model.PAYPALEMAIL;
                                    docInformation.CREATEDBY = model.UserName;
                                    docInformation.MODIFIEDBY = model.UserName;
                                    docInformation.DateCreated = DateTime.UtcNow;
                                    docInformation.MODIFIEDON = DateTime.UtcNow;
                                    docInformation.UserName = model.UserName;
                                    docInformation.State = State.Added;


                                    user.DoctorInformations.Add(docInformation);
                                    user.DoctorStatuses.Add(docstatus);

                                    if (model.Languages != null)
                                    {
                                        for (int i = 0; i < model.Languages.Length; i++)
                                        {
                                            USERSLANGUAGE UserLanguages = new USERSLANGUAGE();
                                            UserLanguages.CREATEDBY = User.Identity.Name;
                                            UserLanguages.MODIFIEDON = DateTime.UtcNow;
                                            UserLanguages.DateCreated = DateTime.UtcNow;
                                            UserLanguages.MODIFIEDBY = User.Identity.Name;
                                            UserLanguages.State = State.Added;
                                            UserLanguages.UserName = model.UserName;
                                            UserLanguages.LANGUAGEKEYID = model.Languages[i];
                                            user.Userlanguages.Add(UserLanguages);
                                        }
                                    }
                                    if (model.Specialities != null)
                                    {
                                        string temp = null;
                                        foreach (var data in model.Specialities)
                                        {
                                            if (temp == null)
                                            {
                                                temp = data;
                                            }
                                            else
                                            {
                                                temp = temp + "," + data;
                                            }
                                        }

                                        var datas = suow.Context.DoctorsSpecialities.SingleOrDefault(f => f.UserName == model.UserName);
                                        if (datas == null)
                                        {

                                            SpecialityUnitOfWork suow2 = new SpecialityUnitOfWork();
                                            DoctorSpecialitiesServiceRepository repo5 = new DoctorSpecialitiesServiceRepository(suow2);


                                            DOCTORSPECIALITY speciality = new DOCTORSPECIALITY();
                                            speciality.CREATEDBY = model.UserName;
                                            speciality.MODIFIEDON = DateTime.UtcNow;
                                            speciality.DateCreated = DateTime.UtcNow;
                                            speciality.MODIFIEDBY = model.UserName;
                                            speciality.State = State.Added;
                                            speciality.UserName = model.UserName;
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
                                                datas.MODIFIEDBY = model.UserName;
                                                datas.State = State.Modified;
                                                datas.SPECIALITY = temp;
                                                repo2.InsertOrUpdate(datas);
                                                suow.Save();
                                            }
                                        }
                                    }

                                    PayRateUnitOfWork puow = new PayRateUnitOfWork();
                                    DoctorPayRateServiceRepository repo1 = new DoctorPayRateServiceRepository(puow);

                                    DOCTORPAYRATE DoctorPayrate = new DOCTORPAYRATE();
                                    DoctorPayrate.CREATEDBY = User.Identity.Name;
                                    DoctorPayrate.MODIFIEDON = DateTime.UtcNow;
                                    DoctorPayrate.DateCreated = DateTime.UtcNow;
                                    DoctorPayrate.MODIFIEDBY = User.Identity.Name;
                                    DoctorPayrate.State = State.Added;
                                    DoctorPayrate.RATE = 0;
                                    DoctorPayrate.UserName = model.UserName;
                                    DoctorPayrate.DURATION = 15;
                                    DoctorPayrate.ACTIVEFROM = DateTime.UtcNow;
                                    repo1.InsertOrUpdate(DoctorPayrate);
                                    puow.Save();


                                    userpwd.CREATEDBY = User.Identity.Name;
                                    userpwd.MODIFIEDON = DateTime.UtcNow;
                                    userpwd.DateCreated = DateTime.UtcNow;
                                    userpwd.MODIFIEDBY = User.Identity.Name;
                                    userpwd.State = State.Added;
                                    userpwd.UserName = model.UserName;
                                    userpwd.EXPIRYDATE = DateTime.UtcNow.AddMonths(1);
                                    userpwd.PASSWORDHASH = user.PASSWORDHASH;
                                    userpwd.PASSWORDSALT = user.PASSWORDSALT;
                                    userpwd.STATUS = "A";
                                    user.UserPasswords.Add(userpwd);

                                    repo.InsertOrUpdate(user);
                                    uow.Save();
                                     string comment = "DoctorSignUp.";
                                     string dataDcotor = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                                    //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.ForgotPassword, 0, 0, model.UserName, data);
                                     //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.DoctorSignUp, 0, 0, model.UserName, dataDcotor);
                                     //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.DoctorSignUpAdmin, 0, 0, model.UserName, dataDcotor);
                                    /*
                                    bodyMsg.Append("<div style='width:750px;background-color:#fff;color:#4d4d4d;border:1pxsolid#999;font-size:14px;font-family:serif;letter-spacing:0.02em;'>");
                                    bodyMsg.Append("<div style='height:50px;border-bottom:1pxsolid#aaa;background:#D3D0D0;padding:8px;'>");
                                    bodyMsg.Append("<div>");
                                    bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png'title='doccare_logo'alt='doccarelogo'/>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("<div style='padding:10px;'>");
                                    bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                                    bodyMsg.Append("<h2 style='color:#1072B5;font-style:italic;'>Congratulations! Dr." + model.FIRSTNAME + " " + model.LASTNAME + "</h2>");
                                    bodyMsg.Append("<div style='float:left;width:490px;'>");
                                    bodyMsg.Append("<div>docare online has received your information.We are in the process");
                                    bodyMsg.Append(" Of verifying your credentials. Once we have verified your information");
                                    bodyMsg.Append(" we will send you details to log in to the site.After you login,you will");
                                    bodyMsg.Append("be able to create your profile and subscribe to see your patients");
                                    bodyMsg.Append("via PROFESSORSONLINE.");
                                    bodyMsg.Append("</div>");
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
                                    bodyMsg.Append("<div style='color:#1072B5;text-align:center;font-size:19px;font-style:italic;'>");
                                    bodyMsg.Append("<strong>See your patients,anytime,anywhere!");
                                    bodyMsg.Append("</strong>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("<p>");
                                    bodyMsg.Append("If you have questions about PROFESSORSONLINE,please email us at");
                                    bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.'style='color:#1072B5;'>  helpdesk@PROFESSORSONLINE.com.");
                                    bodyMsg.Append("</a>For FAQ,please visit");
                                    bodyMsg.Append("<ahref=' www.PROFESSORSONLINE.com/faq'title='PROFESSORSONLINE'style='color:#1072B5;'> www.PROFESSORSONLINE.com/faq.");
                                    bodyMsg.Append("</a>");
                                    bodyMsg.Append("</p>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("<div style='float:right;margin-left:10px;width:230px;'>");
                                    bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png'alt='amy_image'/>");
                                    bodyMsg.Append("<div style='color:#1072B5;text-align:center;font-size:19px;font-style:italic;'>");
                                    bodyMsg.Append("docare online helps doctors and patients connect securely anytime,anywhere!");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("<div style='clear:both;'>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("<div style='font-size:12px;background:#D3D0D0;padding:10px;clear:both;margin-top:10px;'>");
                                    bodyMsg.Append("<strong>Disclaimer:");
                                    bodyMsg.Append("</strong>This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential,the disclosure of which is governed by applicable law.If the reader of this message is not the intended recipient,or the employee or agent responsible to deliver it to the intended recipient,you are here by notified that any dissemination,distribution or copying of this information is STRICTLY PROHIBITED.If you have received this message by error,please not if you have received this message by error, please notify us immediately and destroy the related message.You,the recipient,are obligated to maintain it in a safe,secure and confidential manner.Re-disclosure without appropriate member authorization or as permitted by law is prohibited.Unauthorized re-disclosure or failure to maintain confidentiality");
                                    bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
                                    bodyMsg.Append("</strong>");
                                    bodyMsg.Append("</div>");
                                    bodyMsg.Append("</div>");


                                    //






                                    bodyMsgAdmin.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                                    bodyMsgAdmin.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                                    bodyMsgAdmin.Append("<div>");
                                    bodyMsgAdmin.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='docare logo'/>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("<div style='padding:10px;'>");
                                    bodyMsgAdmin.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                                    bodyMsgAdmin.Append("<h2 style='color:#1072B5;font-style: italic;'>Dear, Admin");
                                    bodyMsgAdmin.Append("</h2>");
                                    bodyMsgAdmin.Append("<div style='float:left;width:490px;'> ");
                                    bodyMsgAdmin.Append("<div style='color: #1072B5;text-align: center;font-size: 25px;font-style: italic;margin:100px 0px;'>");
                                    bodyMsgAdmin.Append("<strong>New Doctor has signed up at Doc Care. Please login to ");
                                    bodyMsgAdmin.Append("<a href='#' style='color: #1072B5;'> www.PROFESSORSONLINE.com/admin");
                                    bodyMsgAdmin.Append("</a> to verify doctor.");
                                    bodyMsgAdmin.Append("</strong>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("<p>If you have questions about docare online, please email us at");
                                    bodyMsgAdmin.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'> helpdesk@PROFESSORSONLINE.com");
                                    bodyMsgAdmin.Append("</a> For FAQ, please visit ");
                                    bodyMsgAdmin.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'> www.PROFESSORSONLINE.com/faq.");
                                    bodyMsgAdmin.Append("</a>");
                                    bodyMsgAdmin.Append("</p>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("<div style='float:right; margin-left:10px;width:230px;'>");
                                    bodyMsgAdmin.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
                                    bodyMsgAdmin.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>docare online helps doctors and patients connect securely anytime, anywhere!");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("<div style='clear:both;'>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("<div style='font-size:12px;background:#D3D0D0;padding: 10px;clear:both;margin-top: 10px;'>");
                                    bodyMsgAdmin.Append("<strong>Disclaimer:");
                                    bodyMsgAdmin.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality ");
                                    bodyMsgAdmin.Append("<strong>could subject you to penalties described in Federal and State law.");
                                    bodyMsgAdmin.Append("</strong>");
                                    bodyMsgAdmin.Append("</div>");
                                    bodyMsgAdmin.Append("</div>");

                                    DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", model.UserName, "", "", "Welcome to docare online", bodyMsg.ToString(), false, "", 0);
                                    DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", "anoopranjan.1983@gmail.com", "", "", "Please verify New Doctor immediately", bodyMsgAdmin.ToString(), false, "", 0);
                                    */
                                    return RedirectToAction("RegistrationSuccess", "Account");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error :This user is already registered .");
                            }
                        }

                        else
                        {
                            ModelState.AddModelError("", "Error :Please accept the terms of service.");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some data are not valid.");
                    }
               // }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });

            }
            return View(model);
        }

    }
}
