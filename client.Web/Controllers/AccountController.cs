 using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
using DOCVIDEO.Domain;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using client.Web.Models;
using DOCVIDEO.UserServiceRepoUOW;
using DOCVIDEO.Utility;
using UserService.Repositories.Disconnected;
using DOCVIDEO.ErrorLoggingContext;
using System.Data.SqlClient;
using DOCVIDEO.ObjectState;
using PatientInformationService.Repositories.Disconnected;
//using DOCVIDEO.BOL;
//using DOCVIDEO.DAL;

namespace client.Web.Controllers
{
    public class AccountController : Controller {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        private readonly UnitOfWork uow = new UnitOfWork();
        private readonly DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uw = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();
        private AccountMembershipService account = new AccountMembershipService();
        public static DateTime logintime;
        protected override void Initialize(RequestContext requestContext) {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegistrationSuccess()
        {

            return View();
        }
         [AllowAnonymous]
        public ActionResult LogOn(int id=0)
        {
            var errormessage = Convert.ToString(Session["LogInError"]);
            if (id == 1)
            {
                ModelState.AddModelError("", "Your account is now active.Please Login with the new Password now");
                return View();
            }
            if (errormessage != "")
            { ModelState.AddModelError("", "Please Relogin"); }
            if (id >0)
            {
                var data = uow.Context.UserInformations.SingleOrDefault(f => f.VERIFICATIONCODE == id);

                if (id == 2)
                {
                    ModelState.AddModelError("", "Please Relogin");
                    return View();
                }

                if (data != null)
                {
                    if (data.IsApproved == false)
                    {
                        using (var repo = new UserRepository(uow))
                        {

                            data.PASSWORDHASH = data.PASSWORDHASH;
                            data.PASSWORDSALT = data.PASSWORDSALT;
                            data.DateLastActivity = DateTime.UtcNow;
                            data.DateLastLogin = DateTime.UtcNow;
                            data.DateLastPasswordChange = DateTime.UtcNow;
                            data.IsApproved = true;
                            data.CREATEDBY = User.Identity.Name;
                            data.MODIFIEDBY = User.Identity.Name;
                            data.DateCreated = DateTime.UtcNow;
                            data.MODIFIEDON = DateTime.UtcNow;
                            data.REGISTEREDON = DateTime.UtcNow;
                            data.State = State.Modified;
                            repo.InsertOrUpdate(data);
                            uow.Save();
                            ModelState.AddModelError("", "Your account is now active.Please Login now");
                        }
                    }
                }
            }
            
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            Session["Gridresult"] = null;
            try
            {
               
                    if (ModelState.IsValid)
                    {
                        var data = uow.Context.UserInformations.SingleOrDefault(f => f.UserName == model.UserName);
                        if (data == null)
                        {
                            ModelState.AddModelError("", "The user name not found.");
                            return View(model);
                        }
                        if (data.IsApproved)
                        {
                            if (MembershipService.ValidateUser(model.UserName, model.Password))
                            {

                                if (data != null)
                                {
                                    if (data.USERTYPE == "D")
                                    {
                                        FormsService.SignIn(model.UserName, model.RememberMe);
                                        if (Url.IsLocalUrl(returnUrl))
                                        {
                                            return Redirect(returnUrl);
                                        }
                                        else
                                        {
                                        Session["DoctorId"] = User.Identity.Name;
                                            UserLoginsUnitOfWork uluow = new UserLoginsUnitOfWork();
                                            using (var repo = new UserLoginsServiceRepository(uluow))
                                            {
                                                USERLOGIN temp = new USERLOGIN();
                                                temp.LOGINTIME = DateTime.UtcNow;
                                                temp.DateCreated = DateTime.UtcNow;
                                                temp.CREATEDBY = model.UserName;
                                                temp.UserName = model.UserName;
                                                temp.STATUS = "A";
                                                temp.MODIFIEDBY = model.UserName;
                                                temp.MODIFIEDON = DateTime.UtcNow;
                                                temp.State = State.Added;
                                                repo.InsertOrUpdate(temp);
                                                uluow.Save();
                                            }
                                            if (data.IsfirstLogin.Trim()=="false")
                                            {
                                                return RedirectToAction("Dashboard", "DoctorInformation");
                                            }
                                            else
                                            {
                                                return RedirectToAction("Welcomepage", "DoctorInformation");
                                                 
                                            }
                                        }
                                    }
                                    else if (data.USERTYPE == "P")
                                    {
                                        FormsService.SignIn(model.UserName, model.RememberMe);
                                        if (Url.IsLocalUrl(returnUrl))
                                        {
                                            return Redirect(returnUrl);
                                        }
                                        else
                                        {
                                        Session["PatientId"] = User.Identity.Name;
                                        UserLoginsUnitOfWork uluow = new UserLoginsUnitOfWork();
                                            using (var repo = new UserLoginsServiceRepository(uluow))
                                            {
                                                USERLOGIN temp = new USERLOGIN();
                                                temp.LOGINTIME = DateTime.UtcNow;
                                                temp.DateCreated = DateTime.UtcNow;
                                                temp.CREATEDBY = model.UserName;
                                                temp.UserName = model.UserName;
                                                temp.STATUS = "A";
                                                temp.MODIFIEDBY = model.UserName;
                                                temp.MODIFIEDON = DateTime.UtcNow;
                                                temp.State = State.Added;
                                                repo.InsertOrUpdate(temp);
                                                uluow.Save();
                                            }
                                            if (data.IsfirstLogin.Trim() == "false")
                                            {
                                                return RedirectToAction("Dashboard", "PatientProfile");
                                            }
                                            else
                                            {
                                                return RedirectToAction("Welcomepage", "PatientProfile");

                                            }
                                            

                                        }
                                    }
                                    else if (data.USERTYPE == "A")
                                    {
                                        FormsService.SignIn(model.UserName, model.RememberMe);
                                        if (Url.IsLocalUrl(returnUrl))
                                        {
                                            return Redirect(returnUrl);
                                        }
                                        return RedirectToAction("AccountAdmin", "AccountSettings");
                                    }



                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Please Activate your account from your Email-id doccare registration message.");
                        }
                    }                               
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }
     
        [HttpPost]
        [AllowAnonymous]
        public JsonResult LogOnOuter(LogOnModel model, string returnUrl)
        {
            Session["Gridresult"] = null;
            try
            {

                if (ModelState.IsValid)
                {
                    var data = uow.Context.UserInformations.SingleOrDefault(f => f.UserName == model.UserName);
                    if (data == null)
                    {
                        return Json(new { msg = "The user name not found" }, JsonRequestBehavior.AllowGet);                     
                        
                    }
                    if (data.IsApproved)
                    {
                        if (MembershipService.ValidateUser(model.UserName, model.Password))
                        {

                            if (data != null)
                            {
                                if (data.USERTYPE == "P")
                                {
                                    FormsService.SignIn(model.UserName, model.RememberMe);
                                  
                                    if(true)
                                    {
                                        UserLoginsUnitOfWork uluow = new UserLoginsUnitOfWork();
                                        using (var repo = new UserLoginsServiceRepository(uluow))
                                        {
                                            Session["PatientID"] = model.UserName;
                                            USERLOGIN temp = new USERLOGIN();
                                            temp.LOGINTIME = DateTime.UtcNow;
                                            temp.DateCreated = DateTime.UtcNow;
                                            temp.CREATEDBY = model.UserName;
                                            temp.UserName = model.UserName;
                                            temp.STATUS = "A";
                                            temp.MODIFIEDBY = model.UserName;
                                            temp.MODIFIEDON = DateTime.UtcNow;
                                            temp.State = State.Added;
                                            repo.InsertOrUpdate(temp);
                                            uluow.Save();
                                        }

                                        

                                    }
                                }
                                else
                                {
                                    return Json(new { msg = "Account type not valid" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            return Json(new { msg = "The user name or password provided is incorrect." }, JsonRequestBehavior.AllowGet);                          
                        }
                    }
                    else
                    {
                        return Json(new { msg = "Please Activate your account from your Email-id doccare registration message." }, JsonRequestBehavior.AllowGet);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });                
            }
            return Json(new { msg = "success" }, JsonRequestBehavior.AllowGet);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************
        [AllowAnonymous]
        public ActionResult LogOff() {

            UserLoginsUnitOfWork uluow = new UserLoginsUnitOfWork();
            using (var repo = new UserLoginsServiceRepository(uluow))
            {
                USERLOGIN temp = uluow.Context.UserLogins.Where(f => f.UserName == User.Identity.Name ).OrderByDescending(f => f.LOGINTIME).FirstOrDefault();
                if (temp != null)
                {
                    temp.LOGOUTTIME = DateTime.UtcNow;
                    temp.STATUS = "A";
                    temp.MODIFIEDBY = User.Identity.Name;
                    temp.MODIFIEDON = DateTime.UtcNow;
                    temp.State = State.Modified;
                    repo.InsertOrUpdate(temp);
                    uluow.Save();
                }
            }

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

       

      

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

         [Authorize]
        public ActionResult _changepassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();      
        }
        [Authorize]
        [HttpPost]
        public ActionResult _changepassword(ChangePasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                    {
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************
         [AllowAnonymous]
        public ActionResult ChangePasswordSuccess() {
            return View();
        }

        // **************************************
        // URL: /Account/ForgotPassword
        // **************************************
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {           
            return View();
        }
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            USERSINFORMATION user = new USERSINFORMATION();          
            string tempPassword = RandomString(10);
           // StringBuilder bodyMsg = new StringBuilder();
            try
            {
                user = uow.Context.UserInformations.SingleOrDefault(u => u.UserName == model.UserName && u.SECURITYQESTID1==model.SECURITYQESTID1 && u.ANSWER1==model.ANSWER1 );
                if (user != null)
                {
                    if (account.ResetPassword(model.UserName, tempPassword))
                    {
                        string comment = "PassWord Reset.";
                        string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                        ////DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.ForgotPassword, 0, 0, model.UserName + "|" + tempPassword, data);
                        /*
                        bodyMsg.Append("<div style='width: 750px; background-color: #fff; color: #4d4d4d; border: 1px solid #999; font-size: 14px; font-family: serif; letter-spacing: 0.02em;'>");
                        bodyMsg.Append("<div style='height: 50px; border-bottom: 1px solid #aaa; background: #D3D0D0; padding: 8px;'>");
                        bodyMsg.Append("<div>");
                        bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='doccare logo' /></div>");
                        bodyMsg.Append("</div>");
                        bodyMsg.Append("<div style='padding: 10px;'>");
                        bodyMsg.Append("<div style='text-align: right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                        bodyMsg.Append("<h2 style='color: #1072B5; font-style: italic;'>Dear," + user.FIRSTNAME + " " + user.LASTNAME + "</h2>");
                        bodyMsg.Append("<div style='float: left; width: 490px;'>");
                        bodyMsg.Append("<div>Thanks here is new password for signing up with doccare. <br/>");
                        bodyMsg.Append("<strong>Please find temporary password : " + tempPassword + "  </strong><br/>");
                        bodyMsg.Append("doccare online is an online care system that allows patients to connect with doctors immediately, whenever and wherever.");
                        bodyMsg.Append("Please  <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn' title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your doccare account!");
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
                        bodyMsg.Append(" If you have questions about doccare online, please email us at");
                        bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color: #1072B5;'>  helpdesk@PROFESSORSONLINE.com.</a> For FAQ, please visit <a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color: #1072B5;'> www.PROFESSORSONLINE.com/faq.</a>");
                        bodyMsg.Append("</p>");
                        bodyMsg.Append("</div>");
                        bodyMsg.Append("<div style='float: right; margin-left: 10px; width: 230px;'>");
                        bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image' />");
                        bodyMsg.Append("<div style='color: #1072B5; text-align: center; font-size: 19px; font-style: italic;'>doccare online helps doctors and patients connect securely anytime, anywhere!</div>");
                        bodyMsg.Append("</div>");
                        bodyMsg.Append("<div style='clear: both;'></div>");
                        bodyMsg.Append("</div>");
                        bodyMsg.Append("<div style='font-size: 12px; background: #D3D0D0; padding: 10px; clear: both; margin-top: 10px;'>");
                        bodyMsg.Append("<strong>Disclaimer:</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality <strong>could subject you to penalties described in Federal and State law.</strong>");
                        bodyMsg.Append("</div>");
                        bodyMsg.Append("</div>");
                      
                        MailUtility.SendEmail("info@PROFESSORSONLINE.com", user.Email, "", "", "Temporary Password", bodyMsg.ToString() , false, "", 0);
                        */
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email-Id or Question Answer are not Correct.");
                }
            }
            catch (Exception ex)
            {
                throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }


            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = account.MinPasswordLength;
            return View();
        }

         [Authorize]
        public ActionResult PasswordReset(string id)
        {
            try
            {
                string tempPassword = RandomString(10);
                var tempUser = uow.Context.UserInformations.Find(id);
                if (tempUser != null)
                {


                    if (account.ResetPassword(id, tempPassword))
                    {
                        //MailUtility.SendEmail("info@PROFESSORSONLINE.com", tempUser.Email, "", "", "Temporary Password", "Hi," + tempUser.UserName + "<br/>Please find temporary password :" + tempPassword, false, "", 0);

                        return RedirectToAction("ResetPasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The Username is incorrect.");
                }



                // If we got this far, something failed, redisplay form
                ViewBag.PasswordLength = account.MinPasswordLength;
            }
            catch (Exception ex)
            {
                //throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            return View();
        }
          [Authorize]
        public ActionResult ChangePassword(string id)
        {
            return RedirectToAction("ChangePassword", "Account", new { id = id });
        }
          [Authorize]
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }



    }
}
