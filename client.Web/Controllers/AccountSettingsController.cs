using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOCVIDEO.Domain;
using client.Web.Models;
using UserService.Repositories.Disconnected;
using DOCVIDEO.ObjectState;
using System.Text;
using DOCVIDEO.UserServiceRepoUOW;
//using DOCVIDEO.BOL;
//using DOCVIDEO.DAL;

namespace client.Web.Controllers
{
    public class AccountSettingsController : Controller
    {
        private readonly DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
        public DoctorInformationController control = new DoctorInformationController();
        //
        // GET: /AccountSettings/
          [Authorize]
        public ActionResult AccountSettings()
        {
            DoctorsInformationEditModel model = control.DoctorDisplayData(User.Identity.Name);
            model.DOCSUBSCRIPTION = new DOCSUBSCRIPTION();
            using (var uow = new DocSubscriptionUnitOfWork())
            {
                model.DOCSUBSCRIPTION = uow.Context.DOCSUBSCRIPTIONS.FirstOrDefault(f => f.UserName == User.Identity.Name);
            }
            return View(model);
        }
          [Authorize]
          public JsonResult getClinicInformation(DoctorsInformationEditModel model)
          {
              var data=uow.Context.DocWorkInstitutions.SingleOrDefault(f=>f.UserName==model.UserName);
              return Json(new { msg = data.INSTITUTIONNAME + "," + data.STREETADDRESS1 + "," + data.STREETADDRESS2 + "," + data.CLINICUSERSTATE }, JsonRequestBehavior.AllowGet);
          }
          [Authorize]
        public ActionResult AccountSettingPatient()
        {
           
            return View();
        }

          [Authorize]
          public ActionResult DoctorsInformatonViews(string id,int mode)
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

          public ActionResult AccountAdmin()
          {
              string[] approveddoctorslist = null;
              var data = uow.Context.UserInformations.Where(f => f.IsApproved == true && f.USERTYPE=="D").Select(s => new { s.UserName  });
              string tempdata = null;
              foreach (var temp in data)
              {
                  if (tempdata == null)
                  {
                      tempdata = temp.UserName;
                  }
                  else
                  {
                      tempdata = tempdata + " , " + temp.UserName;
                  }

              }

              if (tempdata != null)
              {
                  approveddoctorslist = tempdata.Split(',');
              }
              else
              {
                  approveddoctorslist = null;
              }

              ViewBag.ApprovedDoctors = approveddoctorslist;

              DoctorsInformationEditModel model = control.DoctorDisplayData(User.Identity.Name);
              return View(model);
          }
          public JsonResult GetDoctors(string id,string value=null)
          {
              JsonResult result = new JsonResult();
              if (value != null)
              {
                  value.Trim();
              }
              uow.Context.Configuration.ProxyCreationEnabled = false;
              if (id == "Default")
              {
                  var data = uow.Context.UserInformations
                           .Join(uow.Context.DoctorsInformations, ui => ui.UserName, di => di.UserName,
                          (ui, di) => new { ui.Salutation, ui.Comment, ui.MODIFIEDBY, ui.DateCreated, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.UserName, di.LICENSE, di.STATELICENSE }).Where(f => f.USERTYPE != "A" )
                          .ToList(); result.Data = data;
              }
              else if (id == "NPI")
              {
                  if (value != null)
                  {
                      var data = uow.Context.UserInformations
                          .Join(uow.Context.DoctorsInformations, ui => ui.UserName, di => di.UserName,
                         (ui, di) => new { ui.Salutation, ui.Comment, ui.MODIFIEDBY, ui.DateCreated, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.UserName, di.LICENSE, di.STATELICENSE }).Where(f => f.USERTYPE != "A" && f.LICENSE == value)
                         .ToList(); result.Data = data;
                  }
              }
              else if (id == "FirstName")
              {
                  if (value != null)
                  {
                      var data = uow.Context.UserInformations
                          .Join(uow.Context.DoctorsInformations, ui => ui.UserName, di => di.UserName,
                         (ui, di) => new { ui.Salutation, ui.Comment, ui.MODIFIEDBY, ui.DateCreated, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.UserName, di.LICENSE, di.STATELICENSE }).Where(f => f.USERTYPE != "A" && f.FIRSTNAME == value)
                         .ToList(); result.Data = data;
                  }
              }
              else if (id == "Lastname")
              {
                  if (value != null)
                  {
                      var data = uow.Context.UserInformations
                          .Join(uow.Context.DoctorsInformations, ui => ui.UserName, di => di.UserName,
                         (ui, di) => new { ui.Salutation, ui.Comment, ui.MODIFIEDBY, ui.DateCreated, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.UserName, di.LICENSE, di.STATELICENSE }).Where(f => f.USERTYPE != "A" && f.LASTNAME == value)
                         .ToList(); result.Data = data;
                  }
              }
              

              result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
              return result;
          }
           
        [AllowAnonymous]
          public JsonResult GetAllDoctors()
          {
              JsonResult result = new JsonResult();
              uow.Context.Configuration.ProxyCreationEnabled = false;

              var data = uow.Context.UserInformations
                   .Join(uow.Context.Doctorspecialities, ui => ui.UserName, ds => ds.UserName,
                  (ui, ds) => new { ui.Salutation,ui.USERPHOTOFILEPATH, ui.IsApproved, ui.USERTYPE, ui.FIRSTNAME, ui.LASTNAME, ui.USERSTATE, ui.UserName, ui.GENDER, ui.CITY, ui.ZIPCODE, ds.SPECIALITY }).                                  
                  Where(f => f.USERTYPE == "D" && f.IsApproved==true)
                          .ToList(); result.Data = data;
            


              result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
              return result;
          }
        [AllowAnonymous]
        public JsonResult GetAllFilteredDoctors()
        {
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;
            var tempspecialities=Convert.ToString(Session["DOCFIRSTNAME"]);
            if(tempspecialities != " ")
            {

                var data = uow.Context.UserInformations.Where(f => f.USERTYPE == "D" && f.IsApproved == true && f.FIRSTNAME == tempspecialities)
                        .ToList(); result.Data = data;
            }else{
                result.Data=null;
            }


            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
          public ActionResult GetDoctorsUpdate(DoctorsInformationEditModel model)
          {
              //string header = null;
              DOCVIDEO.UserServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
              var request = uow.Context.UserInformations.SingleOrDefault(s => s.UserName == model.UserName);
              string comment = "Update Doctor Info.";
              string data = string.Format("<Data><Info><![CDATA[{0}]]></Info></Data>", comment);
              //DOCVIDEO.DAL.Event.EventPublisher.PublishEvent((int)DocVideoEvents.GetDoctorsUpdate, Convert.ToInt32(model.DOCTORSLOTID), 0, model.UserName, data);
      
             /* StringBuilder bodyMsg = new StringBuilder();
                if (!request.IsApproved)
              {
                bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                bodyMsg.Append("<div>");
                bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='docare logo'/>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<div style='padding:10px;'>");
                bodyMsg.Append("<div style='text-align:right;'>Date" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'>Congratulations! Dear, " + model.FIRSTNAME + " " + model.LASTNAME + "</h2>");
                bodyMsg.Append("<div style='float:left;width:490px;'> ");
                bodyMsg.Append("<div>");
                bodyMsg.Append(" Your information has been verified. Your account is ");
                bodyMsg.Append("Now active! ");
                bodyMsg.Append("<p> ");
                bodyMsg.Append("Please  <a href='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "Account/LogOn?id=" + request.VERIFICATIONCODE + " 'title='Click'  style='color: #1072B5;'>Click here  </a>   to login to your docare account!");
                bodyMsg.Append("</a> to login and create your profile and subscribe ");
                bodyMsg.Append("To PROFESSORSONLINE.com.");
                bodyMsg.Append("</p>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<div style='color:#1072B5;margin:10px 0px -10px 0px;'>");
                bodyMsg.Append("<strong>Benefits of signing up");
                bodyMsg.Append("</strong>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<div style='font-style: italic;font-weight: bold;line-height: 20px;'>");
                bodyMsg.Append("<ul>");
                bodyMsg.Append("<li>Effective way to provide quality healthcare to patients");
                bodyMsg.Append("</li>");
                bodyMsg.Append("<li>Market physicians’ services online");
                bodyMsg.Append("</li>");
                bodyMsg.Append("<li>Access to new local and foreign patients. ");
                bodyMsg.Append("</li>");
                bodyMsg.Append("<li>Payment at time of service");
                bodyMsg.Append("</li>");
                bodyMsg.Append("<li>No insurance paperwork");
                bodyMsg.Append("</li>");
                bodyMsg.Append("<li>Flexibility for scheduling: online or in office");
                bodyMsg.Append("</li>");
                bodyMsg.Append("<li>Minimal support staff");
                bodyMsg.Append("</li>");
                bodyMsg.Append("</ul>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>");
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
                bodyMsg.Append("</strong>");
                bodyMsg.Append("This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
                bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
                bodyMsg.Append("</strong>");
                bodyMsg.Append("</div>");
                bodyMsg.Append("</div>");

                header = "Congratulations! Your information has been verified. See your patients, anytime, anywhere!";
              }
              else
              {
                    bodyMsg.Append("<div style='width:750px;background-color: #fff;color:#4d4d4d;border:1px solid #999;font-size:14px;font-family: serif;letter-spacing: 0.02em;'>");
                    bodyMsg.Append("<div style='height:50px;border-bottom:1px solid #aaa;background: #D3D0D0;padding:8px;'>");
                    bodyMsg.Append("<div>");
                    bodyMsg.Append("<img src='" + System.Configuration.ConfigurationManager.AppSettings["server_url"] + "/content/images/logo.png' title='doccare_logo' alt='docare logo'/>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='padding:10px;'>");
                    bodyMsg.Append("<div style='text-align:right;'>Date:" + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", DateTime.Now) + "</div>");
                    bodyMsg.Append("<h2 style='color:#1072B5;font-style: italic;'>Dear, " + model.FIRSTNAME + " " + model.LASTNAME + "</h2>");
                    bodyMsg.Append("<div style='float:left;width:490px;'>");
                    bodyMsg.Append("<div>");
                    bodyMsg.Append("Thank you for signing up with docare. ");
                    bodyMsg.Append("<br/>");
                    bodyMsg.Append("At docare, we take patient care seriously. Only board certified ");
                    bodyMsg.Append("Doctors are allowed access to docare. Unfortunately, based on the ");
                    bodyMsg.Append("Information you provided, we were not able to verify your credentials.");
                    bodyMsg.Append("<p>Please contact us at ");
                    bodyMsg.Append("<a href='drreject@PROFESSORSONLINE.com ' title='Click' style='color: #1072B5;'> drreject@PROFESSORSONLINE.com.");
                    bodyMsg.Append("</a> so we can disucss your Account.");
                    bodyMsg.Append("</p>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='color:#1072B5;margin:10px 0px -10px 0px;'>");
                    bodyMsg.Append("<strong>Benefits of signing up");
                    bodyMsg.Append("</strong>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='font-style: italic;font-weight: bold;line-height: 20px;'>");
                    bodyMsg.Append("<ul>");
                    bodyMsg.Append("<li>	Access to doctors’ profiles and quality e-medicine");
                    bodyMsg.Append("</li>");
                    bodyMsg.Append("<li>	No travel time");
                    bodyMsg.Append("</li>");
                    bodyMsg.Append("<li>	Minimal wait time ");
                    bodyMsg.Append("</li>");
                    bodyMsg.Append("<li>	No insurance paperwork");
                    bodyMsg.Append("</li>");
                    bodyMsg.Append("<li>	Electronic scheduling");
                    bodyMsg.Append("</li>");
                    bodyMsg.Append("<li>	Avoid waiting rooms");
                    bodyMsg.Append("</li>");
                    bodyMsg.Append("<li>	Eliminate time off from work.");
                    bodyMsg.Append("</li>");
                    bodyMsg.Append("</ul>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<div style='color: #1072B5;text-align: center;font-size: 19px;font-style: italic;'>");
                    bodyMsg.Append("<strong>See your Doctors, anytime, anywhere!");
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
                    bodyMsg.Append("</strong> This message is intended for the use of the person or entity to which it is addressed and may contain information that is privileged and confidential, the disclosure of which is governed by applicable law. If the reader of this message is not the intended recipient, or the employee or agent responsible to deliver it to the intended recipient, you are hereby notified that any dissemination, distribution or copying of this information is STRICTLY PROHIBITED. If you have received this message by error, please notify us immediately and destroy the related message. You, the recipient, are obligated to maintain it in a safe, secure and confidential manner. Re-disclosure without appropriate member authorization or as permitted by law is prohibited. Unauthorized re-disclosure or failure to maintain confidentiality");
                    bodyMsg.Append("<strong>could subject you to penalties described in Federal and State law.");
                    bodyMsg.Append("</strong>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("</div>");
                    header = "Information regarding your docare account";
                   
              }
              */
            
              JsonResult result = new JsonResult();
              try
              {
                
                  using (var repo = new UserRepository(uow))
                  {
                      request.Comment = model.Comment;
                      request.MODIFIEDBY = User.Identity.Name;
                      request.IsApproved = model.IsApproved;                      
                      request.State = State.Modified;
                      repo.InsertOrUpdate(request);
                      uow.Save();
                  }

                //  DOCVIDEO.Utility.MailUtility.SendEmail("info@PROFESSORSONLINE.com", request.UserName , "", "", header, bodyMsg.ToString(), false, "", 0);
                                         


              }
              catch (Exception ex)
              {
                  //throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
              }

              result.Data = "Success";
              result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
              return result;

          }
          [HttpPost]
          [Authorize]
          public JsonResult Userapproved(DoctorsInformationEditModel model)
          {
              string[] approveddoctorslist = null;
              var data = uow.Context.UserInformations.Where(f => f.IsApproved == true && f.USERTYPE == "D").Select(s => new { s.UserName });
              string tempdata = null;
              foreach (var temp in data)
              {
                  if (tempdata == null)
                  {
                      tempdata = temp.UserName;
                  }
                  else
                  {
                      tempdata = tempdata + " , " + temp.UserName;
                  }

              }

              if (tempdata != null)
              {
                  approveddoctorslist = tempdata.Split(',');
              }
              else
              {
                  approveddoctorslist = null;
              }

              ViewBag.ApprovedDoctors = approveddoctorslist;
              try
              {
                  if (model.UserApproved != null)
                  {
                      for (int i = 0; i < model.UserApproved.Length; i++)
                      {
                          DOCVIDEO.UserServiceRepoUOW.UnitOfWork luow2 = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                          UserRepository repo3 = new UserRepository(luow2);

                          var temp=model.UserApproved[i];
                          USERSINFORMATION Userdata = luow2.Context.UserInformations.SingleOrDefault(f => f.UserName == temp);
                          Userdata.IsApproved = true;
                          luow2.Save();
                          luow2 = null;
                          repo3 = null;
                      }
                  }                  

              }
              catch (Exception ex)
              {
                  return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
              }

              return Json(new { msg = "Successfully Approved" }, JsonRequestBehavior.AllowGet);
          }

          [HttpPost]
          [Authorize]
          public JsonResult Userdisapproved(DoctorsInformationEditModel model)
          {
              string[] approveddoctorslist = null;
              var data = uow.Context.UserInformations.Where(f => f.IsApproved == true && f.USERTYPE == "D").Select(s => new { s.UserName });
              string tempdata = null;
              foreach (var temp in data)
              {
                  if (tempdata == null)
                  {
                      tempdata = temp.UserName;
                  }
                  else
                  {
                      tempdata = tempdata + " , " + temp.UserName;
                  }

              }

              if (tempdata != null)
              {
                  approveddoctorslist = tempdata.Split(',');
              }
              else
              {
                  approveddoctorslist = null;
              }

              ViewBag.ApprovedDoctors = approveddoctorslist;
              try
              {
                if (model.UserDisApproved != null)
                  {
                      for (int i = 0; i < model.UserDisApproved.Length; i++)
                      {
                          DOCVIDEO.UserServiceRepoUOW.UnitOfWork luow2 = new DOCVIDEO.UserServiceRepoUOW.UnitOfWork();
                          UserRepository repo3 = new UserRepository(luow2);

                          var temp = model.UserDisApproved[i];
                          USERSINFORMATION Userdata = luow2.Context.UserInformations.SingleOrDefault(f => f.UserName == temp);
                          Userdata.IsApproved = false;
                          luow2.Save();
                          luow2 = null;
                          repo3 = null;

                      }

                  }

              }
              catch (Exception ex)
              {
                  return Json(new { msg = "Failure" }, JsonRequestBehavior.AllowGet);
              }

              return Json(new { msg = "Successfullly DisApproved" }, JsonRequestBehavior.AllowGet);
          }

          [Authorize]
          public JsonResult GetSubscriptionPayments(string userName)
          {
              JsonResult result = new JsonResult();
              SubscriptionPaymentUnitOfWork spUOW = new SubscriptionPaymentUnitOfWork();
              spUOW.Context.Configuration.ProxyCreationEnabled = false;
              try
              {
                  var pymts = spUOW.Context.SUBSCRIPTIONPAYMENTS.Where(w => w.UserName == userName).ToList();
                  result.Data = pymts;
              }
              catch (Exception )
              {
                  //throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
                  result.Data = "Failure";
              }

              result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
              return result;
          }
    }
}
