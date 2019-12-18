using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DOCVIDEO.Domain;
using DOCVIDEO.ObjectState;
using DOCVIDEO.UserServiceBoundedContext;
using DOCVIDEO.UserServiceRepoUOW;
using DOCVIDEO.HealthHistoryServiceRepoUOW.Repositories.Disconnected;
using DOCVIDEO.Utility;
using UserService.Repositories.Disconnected;
using DOCVIDEO.ErrorLoggingContext;
using System.IO;
using client.Web.Models;
using  DOCVIDEO.UserServiceRepoUOW.Disconnected;
//using DOCVIDEO.BOL;
//using DOCVIDEO.DAL;

namespace client.Web.Controllers
{
    public class MessageController : Controller
    {
        //
        // GET: /Message/
        private readonly MessageUnitOfWork muw = new MessageUnitOfWork();
        private readonly MembershipUnitOfWork muow = new MembershipUnitOfWork();
         private AccountMembershipService account = new AccountMembershipService();
         public DoctorInformationController control = new DoctorInformationController();
         private readonly DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();
         public static string Speciality;
         public static string GENDER;
         public static string ZIPCODE;

         public string Usertype()
         {
             var data = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
             return data.USERTYPE;
         }

         [Authorize]
         public ActionResult DoctorsInformatonViews(string id, int mode)
         {

             if (id != null)
             {
                 Session["DoctorId"] = id;
                 Session["mode"] = mode;
             }
             else
             {
                 string temp = Convert.ToString(Session["DoctorId"]);
                 if (temp != "")
                 {
                     id = temp;
                 }
             }
             return RedirectToAction("DoctorsInformatonViews", "PatientProfile");
         }
         [Authorize]
         public ActionResult DoctorsRatingsViews(string id)
         {

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
             return RedirectToAction("DoctorsRatingsViews", "PatientProfile");
         }
        [Authorize]
        [HttpPost]
         public ActionResult UpdateMailStatus(MESSAGE msg)
         {
             StringBuilder bodyMsg = new StringBuilder();
             JsonResult result = new JsonResult();
             try
             {
                 var user = uow.Context.UsersInformations.SingleOrDefault(f=>f.UserName==User.Identity.Name);
                 var request = muw.Context.Messages.SingleOrDefault(s => s.MESSAGEID == msg.MESSAGEID );
                 if (request.MESSAGESTATUS == "UnRead")
                 {
                     using (var repo = new MessageRepository(muw))
                     {
                         request.MESSAGESTATUS = "Read";
                         request.State = State.Modified;
                         repo.InsertOrUpdate(request);
                         muw.Save();
                         bodyMsg.Append("<div id='displaymessage'>");

                         bodyMsg.Append("<div class='popupcontent'>");
                         bodyMsg.Append("<h3> Message</h3>");
                         bodyMsg.Append(" <div class='row'><b>From :</b> <span class='left'> " + request.SENDMESSAGEFROM + "</span></div>");
                         bodyMsg.Append("<div class='row'><b>Subject :</b> <span class='left'> " + request.MESSAGESUBJECT + "</span></div>");
                         bodyMsg.Append("<div class='row'><b>Message : </b><span class='left'> " + request.MESSAGEBODY + "</span></div>");
                         bodyMsg.Append("</div>");
                         bodyMsg.Append("</div>");
                         if (user.USERTYPE == "D")
                         {
                             bodyMsg.Append(" <a class='button cancel cancelreply' href='/DoctorInformation/Index?sendto=" + request.SENDMESSAGEFROM + "&MessageBody=" + request.MESSAGEBODY + "'>Reply</a>");
                         }
                         else if (user.USERTYPE == "P")
                         {
                             bodyMsg.Append(" <a class='button cancel' href='/Message/Index?id=" + request.SENDMESSAGEFROM + "&MessageBody=" + request.MESSAGEBODY + "'>Reply</a>");
                         }
                         return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                     }
                 }
                 else
                 {
                     bodyMsg.Append("<div id='displaymessage'>");
                     bodyMsg.Append("<div class='popupcontent'>");
                     bodyMsg.Append("<h3> Message</h3>");
                     bodyMsg.Append(" <div class='row'><b>From :</b> <span class='left'> " + request.SENDMESSAGEFROM + "</span></div>");
                     bodyMsg.Append("<div class='row'><b>Subject :</b>  <span class='left'> " + request.MESSAGESUBJECT + "</span></div>");
                     bodyMsg.Append("<div class='row'><b>Message :</b>  <span class='left bodyMessageScroll'> " + request.MESSAGEBODY + "</span></div>");
                     bodyMsg.Append("</div>");
                     bodyMsg.Append("</div>");
                     if (user.USERTYPE == "D")
                     {
                         bodyMsg.Append(" <a class='button cancel replyButton'  href='/DoctorInformation/Index?sendto=" + request.SENDMESSAGEFROM + "&MessageBody=" + request.MESSAGEBODY + "'>Reply</a>");
                     }
                     else if (user.USERTYPE == "P")
                     {
                         bodyMsg.Append(" <a class='button cancel replyButton' href='/Message/Index?id=" + request.SENDMESSAGEFROM + "&MessageBody=" + request.MESSAGEBODY + "'>Reply</a>");
                     }
                    
                     return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                 }

             }
             catch (Exception ex)
             {
                 result.Data = "Please Try Again";
             }
             result.Data = "Success";
             result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
             return result;

         }



        [HttpGet]
        [Authorize]
        public ActionResult Index(string id, string MessageBody)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
           
            string data = Convert.ToString(Session["SendTo"]);
            if (id == null && data !="")
            {
                id = Session["SendTo"].ToString();
            }
            string MessageBodylocal = Convert.ToString(Session["MessageBody"]);
            if (MessageBody == null && MessageBodylocal != "")
            {
                MessageBody = Session["MessageBody"].ToString();
            }
            MESSAGE model = new MESSAGE();
            if (id != null)
            {
                model.SENDMESSAGETO = id;
            }
            if (MessageBody != null)
            {
                model.MESSAGEBODY = MessageBody;
            }
            Session["SendTo"] = null;
            _advancequicksearch();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(MESSAGE temp)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
           
           try
            {
                if (ModelState.IsValid)
                {
                    if (temp.SENDMESSAGETO != null)
                    {
                        if (temp.MESSAGESUBJECT != null)
                        {
                           
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
                                message.MESSAGESTATUSTO = "ACTIVE";
                                message.MESSAGETYPE = "Ordinary";
                                message.MESSAGESTATUS = "UnRead";
                                message.MODIFIEDON = DateTime.UtcNow;
                                message.SENDDATE = DateTime.UtcNow;
                                message.State = DOCVIDEO.ObjectState.State.Added;
                                repo.InsertOrUpdate(message);
                                muw.Save();
                               
                                ModelState.AddModelError("", "Message Sent Successfully");
                                string comment = "Send Secure Message Email.";
                                string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
                                //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.SendSecureMessageEmail, 0, 0, temp.SENDMESSAGETO, data);
                       
                               /*
                                var doctor = uow.Context.UsersInformations.SingleOrDefault(f => f.UserName == temp.SENDMESSAGETO);
                                if (doctor != null)
                                {
                                   
                                        bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                                        bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                                        bodyMsg.Append("<div>");
                                        bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='docare logo'/>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='padding:10px;'>");
                                        bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                                        bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'>Dear,  " +doctor.Salutation  + doctor.FIRSTNAME + " " + doctor.LASTNAME + "");
                                        bodyMsg.Append("</h2>");
                                        bodyMsg.Append("<div style='float:left;width:440px;'>");
                                        bodyMsg.Append("<p>");
                                        bodyMsg.Append("Your patient has send you a secure email. Please login to docare to ");
                                        bodyMsg.Append("Review and respond the email.");
                                        bodyMsg.Append("</p>");
                                        bodyMsg.Append("<div>Please ");
                                        bodyMsg.Append("<p>");
                                        bodyMsg.Append(" <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn 'title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your docare account!");
                                        bodyMsg.Append("</a> ");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 25px;font-style: italic;margin:100px 0px;'>");
                                        bodyMsg.Append("<strong>See your patients, anytime, anywhere!");
                                        bodyMsg.Append("</strong>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<p>If you have questions about docare online, please email us at");
                                        bodyMsg.Append("<a href='mailto:helpdesk@PROFESSORSONLINE.com.' style='color:#1072B5;'>  helpdesk@PROFESSORSONLINE.com.");
                                        bodyMsg.Append("</a> For FAQ, please visit ");
                                        bodyMsg.Append("<a href=' www.PROFESSORSONLINE.com/faq' title='PROFESSORSONLINE' style='color:#1072B5;'>  www.PROFESSORSONLINE.com/faq.");
                                        bodyMsg.Append("</a>");
                                        bodyMsg.Append("</p>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='float:right; margin-left:10px;width:230px;'>");
                                        bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/amy_inner.png' alt='amy_image'/>");
                                        bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>docare online helps doctors and patients connect securely anytime, anywhere!");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='clear:both;'>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("<div style='font-size:12px;background:#D3D0D0;padding: 10px;clear:both;margin-top: 10px;'>");
                                        bodyMsg.Append("<strong>Disclaimer:");
                                        bodyMsg.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality ");
                                        bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
                                        bodyMsg.Append("</strong>");
                                        bodyMsg.Append("</div>");
                                        bodyMsg.Append("</div>");

                                    DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", temp.SENDMESSAGETO, "", "", "docare online has a secure email from your patient.", bodyMsg.ToString(), false, "", 0);
                                }  
                                */
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error :Message Subject is Required.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error :Send Message to Required.");
                    }
                  }
                }               
             catch (Exception ex)
             {
                 ModelState.AddModelError("", "Error :Please Retry.");
             }
           
            return View(temp);
          }

        public JsonResult GetSentItem()
        {
            JsonResult result = new JsonResult();
            muw.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = muw.Context.Messages.Join(muw.Context.Userinformations, me => me.SENDMESSAGETO, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME,ul.Salutation, ul.LASTNAME, me.SENDBY, me.MESSAGESTATUSFROM, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT })
                        .Where(f => f.SENDBY == User.Identity.Name && f.MESSAGESTATUSFROM == "ACTIVE").ToList();
            }
            catch
            {
                result.Data = null;
            }
         
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult GetFilteredSentItem(string id)
        {
            JsonResult result = new JsonResult();
            muw.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (id == "All")
                {
                    result.Data = muw.Context.Messages.Join(muw.Context.Userinformations, me => me.SENDMESSAGETO, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME,ul.Salutation, ul.LASTNAME, me.SENDBY, me.MESSAGESTATUSFROM, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT })
                        .Where(f => f.SENDBY == User.Identity.Name && f.MESSAGESTATUSFROM == "ACTIVE").OrderByDescending(f => f.MESSAGEID).ToList();
                }
                else
                {
                    result.Data = muw.Context.Messages.Join(muw.Context.Userinformations, me => me.SENDMESSAGETO, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME,ul.Salutation, ul.LASTNAME, me.SENDBY, me.MESSAGESTATUSFROM, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT })
                        .Where(f => f.SENDBY == User.Identity.Name && f.MESSAGESTATUSFROM == "ACTIVE" && f.MESSAGESTATUS == id).OrderByDescending(f => f.MESSAGEID).ToList();
                }
            }
            catch
            {
                result.Data = null;
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public JsonResult GetFilteredSentItemDoc(string id)
        {
            JsonResult result = new JsonResult();
            muw.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (id == "All")
                {
                    result.Data = muw.Context.Messages.Join(muw.Context.Userinformations, me => me.SENDMESSAGETO, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME,ul.Salutation, ul.LASTNAME, me.SENDBY, me.MESSAGESTATUSTO, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT })
                        .Where(f => f.SENDBY == User.Identity.Name && f.MESSAGESTATUSTO == "ACTIVE").OrderByDescending(f=>f.MESSAGEID).ToList();
                }
                else
                {
                    result.Data = muw.Context.Messages.Join(muw.Context.Userinformations, me => me.SENDMESSAGETO, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME,ul.Salutation, ul.LASTNAME, me.SENDBY, me.MESSAGESTATUSTO, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT })
                        .Where(f => f.SENDBY == User.Identity.Name && f.MESSAGESTATUSTO == "ACTIVE" && f.MESSAGESTATUS == id).OrderByDescending(f => f.MESSAGEID).ToList();
                }
            }
            catch
            {
                result.Data = null;
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult Getdrpdwn()
        {
            JsonResult result = new JsonResult();
            result.Data = muw.Context.Messages.Select(s => new { s.MESSAGESTATUS }).Distinct().ToList();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

       [HttpGet]
       [Authorize]
        public ActionResult SentItems()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
           
            _advancequicksearch();
            return View();
        }

       [HttpPost]
       [Authorize]
       public ActionResult SentItems(MESSAGE temp)
       {
           if (Usertype() != "P")
           {
               return RedirectToAction("LogOn", "Account");
           }
           
           
           return View(temp);
       }
        [HttpPost]
        [Authorize]
        public JsonResult DeleteMessage(HealthHistoryModel model)
        {
            try
            {
                if (model.MessageDeleteIds != null)
                {
                    for (int i = 0; i < model.MessageDeleteIds.Length; i++)
                    {
                        MessageUnitOfWork muw = new MessageUnitOfWork();
                        var repo = new MessageRepository(muw);
                        var temp = model.MessageDeleteIds[i];
                        var data = muw.Context.Messages.SingleOrDefault(f => f.MESSAGEID == temp);
                        data.MESSAGESTATUSFROM = "DELETE";
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(data);
                        muw.Save();
                        repo = null;
                        muw = null;

                    }
                    
                }
                else
                {
                    return Json(new { msg = "Error :No message Selected" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Error :Please Try Again" }, JsonRequestBehavior.AllowGet);               
            }
            return Json(new { msg = "Message  Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public JsonResult DeleteMessageDoc(HealthHistoryModel model)
        {
            try
            {
                if (model.MessageDeleteIds != null)
                {
                    for (int i = 0; i < model.MessageDeleteIds.Length; i++)
                    {
                        MessageUnitOfWork muw = new MessageUnitOfWork();
                        var repo = new MessageRepository(muw);
                        var temp = model.MessageDeleteIds[i];
                        var data = muw.Context.Messages.SingleOrDefault(f => f.MESSAGEID == temp);
                        data.MESSAGESTATUSTO = "DELETE";
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        repo.InsertOrUpdate(data);
                        muw.Save();
                        repo = null;
                        muw = null;

                    }
                    return Json(new { msg = "Message  Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msg = "Error : No message Selected" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = "Error : Please Try Again" }, JsonRequestBehavior.AllowGet); 
            }
           
        }
        [HttpGet]
        [Authorize]
        public ActionResult AdvancedQuickSearch()
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
           
            _advancequicksearch();
            return View();
        }
        [HttpPost]
        [Authorize]
        public ActionResult AdvancedQuickSearch(MESSAGE model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
           
            MESSAGE modely = new MESSAGE();
            Speciality = modely.ProviderType;
            GENDER = modely.SearchGender;
            ZIPCODE = modely.SEARCHZIPCODE;
           
            return View(model);
        }

       
        [Authorize]
        public void _advancequicksearch()
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
                    tempdata = tempdata + " , " + temp.DOCTORID;
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

        }

        [HttpPost]
        [Authorize]
        public ActionResult QuickSearchOne(MESSAGE model)
        {

            Speciality = model.ProviderType;
            GENDER = model.SearchGender;
            ZIPCODE = model.SEARCHZIPCODE;
            return Json(new { status = "Success", type = "save" }, "text/plain");
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

       
        public JsonResult GetInbox()
        {
            JsonResult result = new JsonResult();
            muw.Context.Configuration.ProxyCreationEnabled = false;
            result.Data = muw.Context.Messages.Join(muw.Context.Userinformations, me => me.SENDMESSAGETO, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME,ul.Salutation, ul.LASTNAME, me.SENDMESSAGETO, me.MESSAGESTATUSTO, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT })
                        .Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUSTO == "ACTIVE").OrderByDescending(f => f.MESSAGEID).ToList();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public JsonResult GetFilteredInbox(string id)
        {
            JsonResult result = new JsonResult();
            muw.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (id == "All")
                {
                    result.Data = muw.Context.Messages
                         .Join(muw.Context.Userinformations, me => me.SENDMESSAGEFROM, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME, ul.Salutation, ul.LASTNAME, me.SENDMESSAGEFROM, me.MESSAGEBODY, me.SENDMESSAGETO, me.MESSAGESTATUSFROM, me.MESSAGEID, me.MESSAGESUBJECT }).
                        Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUSFROM == "ACTIVE").OrderByDescending(f => f.MESSAGEID).ToList();
                }
                else
                {
                    result.Data = muw.Context.Messages.
                        Join(muw.Context.Userinformations, me => me.SENDMESSAGEFROM, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME, ul.Salutation, ul.LASTNAME, me.SENDMESSAGEFROM, me.MESSAGEBODY, me.SENDMESSAGETO, me.MESSAGESTATUSFROM, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT }).
                        Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUSFROM == "ACTIVE" && f.MESSAGESTATUS == id).OrderByDescending(f => f.MESSAGEID).ToList();
                }
            }
            catch
            {
                result.Data = null;
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public JsonResult GetFilteredInboxDoc(string id)
        {
            JsonResult result = new JsonResult();
            muw.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (id == "All")
                {
                    result.Data = muw.Context.Messages.
                        Join(muw.Context.Userinformations, me => me.SENDMESSAGEFROM, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME,ul.Salutation, ul.LASTNAME,me.SENDMESSAGEFROM, me.SENDMESSAGETO,me.MESSAGEBODY, me.MESSAGESTATUSTO, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT })
                        .Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUSTO == "ACTIVE").OrderByDescending(f=>f.MESSAGEID).ToList();
                }
                else
                {
                    result.Data = muw.Context.Messages.Join(muw.Context.Userinformations, me => me.SENDMESSAGEFROM, ul => ul.UserName,
                   (me, ul) => new { ul.FIRSTNAME, ul.Salutation, ul.LASTNAME, me.SENDMESSAGEFROM, me.MESSAGEBODY, me.SENDMESSAGETO, me.MESSAGESTATUSTO, me.MESSAGESTATUS, me.MESSAGEID, me.MESSAGESUBJECT }).Where(f => f.SENDMESSAGETO == User.Identity.Name && f.MESSAGESTATUSTO == "ACTIVE" && f.MESSAGESTATUS == id).OrderByDescending(f => f.MESSAGEID).ToList();
                }
            }
            catch
            {
                result.Data = null;
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        [HttpGet]
        [Authorize]
        public ActionResult Inbox()
        {
            MESSAGE temp = new MESSAGE();
            return View(temp);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Inbox(MESSAGE temp)
        {
           
            return View(temp);
        }
        
}
}
