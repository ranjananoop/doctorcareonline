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
using DOCVIDEO.HealthHistoryServiceBoundedContext;
using DOCVIDEO.HealthHistoryServiceRepoUOW;
using DOCVIDEO.HealthHistoryServiceRepoUOW.Repositories.Disconnected;
using System.IO;
using client.Web.Models;
using DOCVIDEO.ErrorLoggingContext;
using DOCVIDEO.UserServiceRepoUOW;



namespace client.Web.Controllers
{
    public class HealthHistoryController : Controller
    {
        //
        // GET: /HealthHistory/
        public static string ALLERGYFILEPATH ;
        public static string REPORTPATH ;
        public static string Speciality;
        public static string GENDER;
        public static string ZIPCODE;
        private readonly DOCVIDEO.HealthHistoryServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.HealthHistoryServiceRepoUOW.UnitOfWork();
        private readonly AllergyUnitOfWork uwrk = new AllergyUnitOfWork();
        private readonly MedicationUnitOfWork uw = new MedicationUnitOfWork();
        private readonly ImmunizationUnitOfWork uwk = new ImmunizationUnitOfWork();
        private readonly ProcedureUnitOfWork uwork = new ProcedureUnitOfWork();
        public DoctorInformationController control = new DoctorInformationController();
        private readonly AppointmentStatusUnitOfWork asUOW = new AppointmentStatusUnitOfWork();
        private readonly DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork uow1 = new DOCVIDEO.PatientInformationServiceRepoUOW.UnitOfWork();

        public string Usertype()
        {
            var data = uow1.Context.UsersInformations.SingleOrDefault(f => f.UserName == User.Identity.Name);
            return data.USERTYPE;
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult DisplayField(HealthHistoryModel HealthHistoryModel)
        {
            StringBuilder bodyMsg = new StringBuilder();
            JsonResult result = new JsonResult();
            try
            {
                if (HealthHistoryModel.flag == 1)
                {
                    var request = uow.Context.PatientReports.SingleOrDefault(s => s.PATIENTREPORTID == HealthHistoryModel.ID);
                    bodyMsg.Append("<div style='font-size:14px;'>");
                    bodyMsg.Append("<div'>");
                    bodyMsg.Append("<h3 style='padding: 20px 10px 10px 20px;color: #3AA3FF;width: 600px;border-bottom: 1px solid #999;margin-bottom: 10px;font-size: 20px;'> Reports</h3>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("<div style='padding-left: 20px;font-size: 14px;min-height: 75px;padding-bottom: 20px;'>");
                    bodyMsg.Append(" <div style='font-weight:bold;'><span style='width:140px;float:left;'>Report Name:</span> <span style='color:#4d4d4d;float:left;font-weight:normal;'> " + request.REPORTNAME + "<br /></span></div> </br>");
                    bodyMsg.Append("<div style='font-weight:bold;'><span style='width:140px;float:left;'>Comments :</span> <p style='color:#4d4d4d;font-weight:normal;float:left;word-wrap:break-word;width:400px;float: left;'> " + request.NOTES + "</p></div> </br>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<br />");
                    return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else if (HealthHistoryModel.flag == 2)
                {
                    var request = uwork.Context.Procedures.SingleOrDefault(s => s.PROCEDUREID == HealthHistoryModel.ID);
                    bodyMsg.Append("<div style=';font-size:14px;'>");
                    bodyMsg.Append("<div '>");
                    bodyMsg.Append("<h3 style='padding: 20px 10px 10px 20px;color: #3AA3FF;width: 600px;border-bottom: 1px solid #999;margin-bottom: 10px;font-size: 20px;'>Procedure Reports</h3>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("<div style='padding-left: 20px;font-size: 14px;min-height: 75px;padding-bottom: 20px;margin-left:0px;'>");
                    bodyMsg.Append(" <div ><span style='width:140px;float:left;'><b>Report Name:</b> </span><span style='color:#4d4d4d;font-weight:normal;float:left;'> " + request.PROCEDUREREPORTNAME + "<br /></span></div> </br>");
                    bodyMsg.Append("<div ><span style='width:140px;float:left;'><b>Procedure Date</b> :</span> <span style='color:#4d4d4d;font-weight:normal;float:left;'> " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", request.PROCEDUREDATE) + "</span></div> </br>");
                    bodyMsg.Append("<div ><span style='width:140px;float:left;'><b>Comments</b> : </span><p style='color:#4d4d4d;font-weight:normal;word-wrap:break-word;width:420px;float: left;'> " + request.PROCEDURENOTES + "</p></div> </br>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<br />");
                    return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else if (HealthHistoryModel.flag == 3)
                {
                    var request = uwrk.Context.Allergies.SingleOrDefault(s => s.ALLERGYID == HealthHistoryModel.ID);
                    bodyMsg.Append("<div style=';font-size:14px;'>");
                    bodyMsg.Append("<div>");
                    bodyMsg.Append("<h3 style='padding: 20px 10px 10px 20px;color: #3AA3FF;width: 600px;border-bottom: 1px solid #999;margin-bottom: 10px;font-size: 20px;'>Allergy Report </h3>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("<div style='padding-left: 20px;font-size: 14px;min-height: 75px;padding-bottom: 20px;margin-left:0px;'>");
                    bodyMsg.Append(" <div ><div style='width:140px;float:left;'><b>Report Name:</b></div> <span style='color:#4d4d4d;font-weight:normal;float:left;'> " + request.ALLERGYREPORTNAME + "<br /></span></div> </br>");
                    bodyMsg.Append("<div ><div style='width:140px;float:left;'><b>Comments :</b> </div><p style='color:#4d4d4d;font-weight:normal;word-wrap:break-word;width:420px;float:left;'> " + request.COMMENT + "</p</div> </br>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<br />");
                    return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else if (HealthHistoryModel.flag == 4)
                {
                    var request = uwk.Context.Immunizations.SingleOrDefault(s => s.IMMUNIZATIONID == HealthHistoryModel.ID);
                    bodyMsg.Append("<div style='font-size:14px;'>");
                    bodyMsg.Append("<div >");
                    bodyMsg.Append("<h3 style='padding: 20px 10px 10px 20px;color: #3AA3FF;width: 600px;border-bottom: 1px solid #999;margin-bottom: 10px;font-size: 20px;'>Immunization Reports</h3>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("<div style='padding-left: 20px;font-size: 14px;min-height: 75px;padding-bottom: 20px;margin-left:0px;'>");
                    bodyMsg.Append(" <div ><span style='width:140px;float:left;'><b>Report Name:</span> <span style='color:#4d4d4d;font-weight:normal;float:left;'> " + request.IMMUNIZATIONNAME + "<br /></span></div> </br>");
                    //bodyMsg.Append("<div ><span style='width:140px;float:left;'><b>Date:</b></span> <span style='color:#4d4d4d;float:left;font-weight: normal; " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", request.IMMUNEDATE) + "</span></div> </br>");
                    bodyMsg.Append("<div ><span style='width:140px;float:left;clear:both;'><b>Comments:</b></span> <span style='color:#4d4d4d;float:left;font-weight:normal;word-wrap:break-word;width:420px;'> " + request.COMMENT + "</span></div> </br>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<br />");
                    return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else if (HealthHistoryModel.flag == 5)
                {
                    var request = uw.Context.Medications.SingleOrDefault(s => s.MEDICATIONID == HealthHistoryModel.ID);
                    bodyMsg.Append("<div style='font-size:14px;'>");
                    bodyMsg.Append("<div >");
                    bodyMsg.Append("<h3 style='padding: 20px 10px 10px 20px;color: #3AA3FF;width: 600px;border-bottom: 1px solid #999;margin-bottom: 10px;font-size: 20px;'>Medication Reports</h3>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("<div style='padding-left: 20px;font-size: 14px;min-height: 75px;padding-bottom: 20px;margin-left:0px;'>");
                    bodyMsg.Append(" <div ><div style='width:140px;float:left;'><b>Medication Name:</b></div> <span style='color:#4d4d4d;font-weight:normal;float:left;'> " + request.MEDICATIONNAME + "<br /></span></div> </br>");
                    bodyMsg.Append("<div ><div style='width:140px;float:left;'><b>Prescribed Date:</b></div> <span style='color:#4d4d4d;font-weight:normal;word-wrap:break-word;float:left;'> " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", request.PRESCRIBEDDATE) + "</span></div> </br>");
                    bodyMsg.Append("<div ><div style='width:140px;float:left;'><b>Stopped Date:</b> </div><span style='color:#4d4d4d;font-weight:normal;word-wrap:break-word;width:450px;float:left;'> " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", request.STOPPEDDATE) + "</span></div> </br>");
                    bodyMsg.Append("<div ><div style='width:140px;float:left;'><b>Doctor Name:</b></div> <span style='color:#4d4d4d;font-weight:normal;word-wrap:break-word;width:450px;float:left;'> " + request.DOCTORID + "</span></div> </br>");
                    bodyMsg.Append("<div ><div style='width:140px;float:left;'><b>Comments:</b> </div><p style='color:#4d4d4d;font-weight:normal;word-wrap:break-word;width:420px;float:left;'> " + request.COMMENT + "</p></div> </br>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("</div>");
                    bodyMsg.Append("<br />");
                    return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else if (HealthHistoryModel.flag == 6)
                {
                    var request = asUOW.Context.CancelledAppointments
                    .Join(asUOW.Context.Appointments, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                    (ui, ds) => new { ui.CANCELDATE, ui.REASONTOCANCEL,ds.DOCTORID }).Where(f => f.DOCTORID == HealthHistoryModel.DOCTORID);
                    bodyMsg.Append("<div style='background-color: #fff;padding:20px; height:auto;font-size:16px; z-index:9999999999999 !important;width:150px'>");
                    bodyMsg.Append("<div id='displaycancelledappointmnets'style='background-color: #10C8FE;height:auto;font-size:15px;color:#fff;padding: 5px 0px 5px 25px; z-index:999999999999 !important;margin-top: -38px;margin-left: -20px;width: inherit;'>");
                    bodyMsg.Append("<h3>Cancelled Appointments</h3>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append("<div style='background-color: #fff;padding:20px;padding-bottom:00px;font-size:15px;text-align: left;width: 100px;margin-left: -20px; z-index:999999999999 !important;'>");
                    if (request != null)
                    {
                        foreach (var temp in request)
                        {

                            bodyMsg.Append(" <div style='font-weight:bold;color:#1071b5;background-color: #fff; z-index:999999999999 !important;'>Cancelled Date: <span style='color:#4d4d4d; z-index:999999999999 !important;margin-left: 6px;font-weight:normal;'> " + String.Format("{0:dddd, MMMM d, yyyy HH:mm:ss tt}", temp.CANCELDATE) + "<br /></span></div> </br>");
                            bodyMsg.Append("<div style='font-weight:bold;color:#1071b5;background-color: #fff; z-index:999999999999 !important;'>Reason to cancel: <span style='color:#4d4d4d;margin-left: 10px; z-index:999999999999 !important;font-weight:normal;word-wrap:break-word;'> " + temp.REASONTOCANCEL + "</span></div> </br>");
                            //bodyMsg.Append("<br style='background-color: #fff; z-index:9999999 !important;'/>");
                        }
                        bodyMsg.Append("</div>");
                    }
                    return Json(new { msg = bodyMsg.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else if (HealthHistoryModel.flag == 7)
                {
                    var request = asUOW.Context.DoctorNotes
                    .Join(asUOW.Context.Appointments, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                    (ui, ds) => new { ui.DOCTORNOTESID,ui.NOTES, ds.APPOINTMENTID }).Where(f => f.APPOINTMENTID == HealthHistoryModel.ID).ToList();

                    var requestchat = asUOW.Context.Chatmessages
                  .Join(asUOW.Context.Appointments, ui => ui.APPOINTMENTID, ds => ds.APPOINTMENTID,
                  (ui, ds) => new { ui.MESSAGEFROM, ui.CHATTEXT, ds.APPOINTMENTID }).Where(f => f.APPOINTMENTID == HealthHistoryModel.ID).ToList();


                    bodyMsg.Append("<div id='displaymessage' >");
                    bodyMsg.Append("<div class='popupcontent' >");
                    //bodyMsg.Append("<h3 style='padding-bottom: 7px;font-size: 22px !important;margin-bottom: 31px;'>Appointments notes and chat history</h3>");                   
                    bodyMsg.Append("<div >");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append(" </div>");
                    bodyMsg.Append(" </div>");
                    
                    if (request != null)
                    {
                       
                        foreach (var temp in request)
                        {
                            bodyMsg.Append("<div style='margin: 0px 0px 0px 0px;clear:both;text-align:justify;'><b style='width:100px;margin:10px 20px 0px 20px;clear:both;'>Notes:</b>" + temp.NOTES + "</div><br/> ");
                         
                           
                           
                        }
                        
                    }

                    if (requestchat != null)
                    {
                        foreach (var temp in requestchat)
                        {

                            bodyMsg.Append("<div style='margin: 0px 0px 0px 0px;clear:both;text-align:justify;'><b style='width:100px;margin:10px 20px 0px 20px;color:#10c8fe'>" + temp.MESSAGEFROM + ": </b>" + temp.CHATTEXT + "</div> </br>");
                           
                        }
                       
                    }
                    if (requestchat.Count==0 && request.Count ==0)
                    {
                        bodyMsg.Append("<div style='clear: both;margin: 15px 0px 20px 21px;'>No  Record found </div> </br>");
                         
                    }
                    bodyMsg.Append("</div>");
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
        [Authorize]
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

        [HttpGet]
        [Authorize]
        public ActionResult upload(string tabid="")
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            
            _procedure();
            _allergies();           
            _immunization();
             _uploadreport();
             HealthHistoryModel model = new HealthHistoryModel();
             string tabindex = Convert.ToString(Session["tabindex"]);
             string message = Convert.ToString(Session["Message"]);
             if (tabid != "")
             {
                 model.tabindex = tabid;
             }
             if (tabid == "")
             {
                 model.tabindex = "tab1";
             }
              if (tabindex != "")
             {
                 model.tabindex = tabindex;
             }
            

             if (message != null)
                 model.MessageUpdateStatus = message;
             else
             {
                 model.MessageUpdateStatus = "";
             }
            Session["Message"] = null;
            Session["tabindex"] = null;
             return View(model);
        }

        public ActionResult Save1(IEnumerable<HttpPostedFileBase> Uploads)
        {
            // Users = User.Identity.Name;
            string UserName = User.Identity.Name;
            StringBuilder fileNames = new StringBuilder();
            try
            {
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(UserName));

                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(UserName));
                }
                //  The Name of the Upload component is "files"
                if (Uploads != null)
                {
                    foreach (var file in Uploads)
                    {

                        var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        var fileNamewithoutExt = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                        var fileName = Path.GetFileName(fileNamewithoutExt + "_" + DateTime.UtcNow.ToString("MMddyyyyhhmmss") + ext);


                        var physicalPath = Path.Combine(Server.MapPath("/HealthHistory/" + UserName), fileName);
                        fileNames.Append("/HealthHistory/" + UserName + "/" + fileName);
                        file.SaveAs(physicalPath);

                        ALLERGYFILEPATH = fileNames.ToString();


                    }
                }
            }
            catch (Exception ex)
            {
                 throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }



            return Json(new { status = fileNames.ToString(), type = "save" }, "text/plain");

        }


        public ActionResult Remove1(string[] fileNames)
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

                        if (System.IO.File.Exists(physicalPath))
                        {
                            //     The files are not actually removed in this demo
                            System.IO.File.Delete(physicalPath);
                        }

                        ALLERGYFILEPATH = null;
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


        [HttpPost]
        [Authorize]
        public ActionResult upload(HealthHistoryModel model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }

            return View(model);
        }

        public JsonResult GetUploadReport()
        {
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                 result.Data = uow.Context.PatientReports.Where(f => f.UserName == User.Identity.Name && f.REPORTSTATUS=="ACTIVE")
                .Select(s=>new {s.PATIENTREPORTID,s.REPORTNAME,s.NOTES,s.REPORTPATH}).OrderByDescending(f=>f.PATIENTREPORTID).ToList();
            }catch(Exception e)
            {
                result.Data =null;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        public ActionResult GetUploadReportID(PATIENTREPORT patientReport)
        {
            JsonResult result = new JsonResult();
            try
            {

                var request = uow.Context.PatientReports.SingleOrDefault(s => s.PATIENTREPORTID == patientReport.PATIENTREPORTID);
                using (var repo = new HealthHistoryRepository(uow))
                 {
                    request.REPORTNAME = patientReport.REPORTNAME;
                    request.NOTES = patientReport.NOTES;
                       request.State = State.Modified;
                       repo.InsertOrUpdate(request);
                       uow.Save();
                        }                   
                 
                
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
        public ActionResult QuickSearchOne(HealthHistoryModel model)
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
        public ActionResult AdvancedQuickSearch(HealthHistoryModel model)
        {
            if (Usertype() != "P")
            {
                return RedirectToAction("LogOn", "Account");
            }
            HealthHistoryModel modely = _advancequicksearch(model);
            Speciality = modely.ProviderType;
            GENDER = modely.SearchGender;
            ZIPCODE = modely.SEARCHZIPCODE;
           
            return View(model);
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

        }
    
        public ActionResult GetAllergyRep(ALLERGY allergy)
        {
            JsonResult result = new JsonResult();
            try
            {

                var request = uwrk.Context.Allergies.SingleOrDefault(s => s.ALLERGYID == allergy.ALLERGYID);
                using (var repo = new AllergyRepository(uwrk))
                {
                    request.ALLERGYREPORTNAME = allergy.ALLERGYREPORTNAME;
                    request.COMMENT = allergy.COMMENT;
                    request.State = State.Modified;
                    repo.InsertOrUpdate(request);
                    uwrk.Save();
                }


            }
            catch (Exception ex)
            {
                //throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            result.Data = "Success";
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }


        public ActionResult GetImmuneRep(IMMUNIZATION immune)
        {
            JsonResult result = new JsonResult();
            try
            {

                var request = uwk.Context.Immunizations.SingleOrDefault(s => s.IMMUNIZATIONID == immune.IMMUNIZATIONID);
                using (var repo = new ImmunizationRepository(uwk))
                {
                    request.IMMUNIZATIONNAME = immune.IMMUNIZATIONNAME;
                    request.COMMENT = immune.COMMENT;
                    request.State = State.Modified;
                    repo.InsertOrUpdate(request);
                    uwk.Save();
                }


            }
            catch (Exception ex)
            {
                //throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            result.Data = "Success";
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        public ActionResult GetMedicationeRep(MEDICATION medication)
        {
            JsonResult result = new JsonResult();
            try
            {

                var request = uw.Context.Medications.SingleOrDefault(s => s.MEDICATIONID == medication.MEDICATIONID);
                using (var repo = new MedicationRepository(uw))
                {
                    request.MEDICATIONNAME = medication.MEDICATIONNAME;
                    request.COMMENT = medication.COMMENT;
                    request.State = State.Modified;
                    repo.InsertOrUpdate(request);
                    uw.Save();
                }


            }
            catch (Exception ex)
            {
                //throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
            }
            result.Data = "Success";
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

        }

        public ActionResult GetProcedureRep(PROCEDURE procedure)
        {
            JsonResult result = new JsonResult();
            try
            {

                var request = uwork.Context.Procedures.SingleOrDefault(s => s.PROCEDUREID == procedure.PROCEDUREID);
                using (var repo = new ProcedureRepository(uwork))
                {
                    request.PROCEDUREREPORTNAME = procedure.PROCEDUREREPORTNAME;
                    request.PROCEDUREDATE = procedure.PROCEDUREDATE;
                    request.State = State.Modified;
                    repo.InsertOrUpdate(request);
                    uwork.Save();
                }


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
        public HealthHistoryModel _advancequicksearch(HealthHistoryModel model)
        {
           
            Speciality = model.ProviderType;
            GENDER = model.SearchGender;
            ZIPCODE = model.SEARCHZIPCODE;
            return model;
        }
        public ActionResult Save(IEnumerable<HttpPostedFileBase> Upload)
        {
           // Users = User.Identity.Name;
            string UserName = User.Identity.Name;
            StringBuilder fileNames = new StringBuilder();
            try
            {
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(UserName));

                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(UserName));
                }
                //  The Name of the Upload component is "files"
                if (Upload != null)
                {
                    foreach (var file in Upload)
                    {
                       
                        var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        var fileNamewithoutExt = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                        var fileName = Path.GetFileName(fileNamewithoutExt + "_" + DateTime.UtcNow.ToString("MMddyyyyhhmmss") + ext);


                        var physicalPath = Path.Combine(Server.MapPath("/HealthHistory/" + UserName), fileName);
                        fileNames.Append("/HealthHistory/" + UserName + "/" + fileName);
                        file.SaveAs(physicalPath);
                       
                          REPORTPATH = fileNames.ToString();
                        
                     
                    }
                }
            }
            catch (Exception ex)
            {
                // throw new CustomException("Exception:- Project: {0} \n Error Message: {1} ", ex.InnerException, new Object[] { ex.Source, ex.Message });
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
                       
                        if (System.IO.File.Exists(physicalPath))
                        {
                            //     The files are not actually removed in this demo
                            System.IO.File.Delete(physicalPath);
                        }
                        REPORTPATH = null;
                       
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

        [HttpGet]
        [Authorize]
        public void _uploadreport()
        {
            
        }
        [HttpPost]
        [Authorize]
        public JsonResult _uploadreport(HealthHistoryModel model)
        {
            try
            {
                if (model.REPORTNAME != null)
                {
                    if (REPORTPATH != null)
                    {
                        using (HealthHistoryRepository repo2 = new HealthHistoryRepository(uow))
                        {
                            PATIENTREPORT dw = new PATIENTREPORT();

                            dw.State = State.Added;
                            dw.CREATEDBY = User.Identity.Name;
                            dw.DateCreated = DateTime.UtcNow;
                            dw.MODIFIEDBY = User.Identity.Name;
                            dw.MODIFIEDON = DateTime.UtcNow;
                            dw.REPORTNAME = model.REPORTNAME;
                            dw.REPORTSTATUS = "Active";
                            dw.DOCTORREPORTSTATUS = "Active";
                            dw.NOTES = model.NOTES;
                            dw.UserName = User.Identity.Name;
                            model.PATIENTREPORTID = dw.PATIENTREPORTID;
                            dw.REPORTPATH = REPORTPATH;
                            repo2.InsertOrUpdate(dw);
                            uow.Save();
                            Session["tabindex"] = "tab1";
                            REPORTPATH = null;
                        }
                        
                     
                    }
                    else
                    {
                        Session["tabindex"] = "tab1";
                        Session["Message"] = "Error :Please Upload a Report";
                        return Json(new { msg = "Error :Please Upload a Report" }, JsonRequestBehavior.AllowGet);                       
                    }
                }
                else
                {
                    Session["tabindex"] = "tab1";
                    Session["Message"] = "Error :Report Name  Required";
                    return Json(new { msg = "Error :Report Name Required" }, JsonRequestBehavior.AllowGet);                
                }
            }
            catch (Exception ex)
            {
                Session["tabindex"] = "tab1";
                Session["Message"] = "Error :Please Try Again";
                REPORTPATH = null;
            }
            Session["tabindex"] = "tab1";
            Session["Message"] = "Upload Report saved successfully";
            REPORTPATH = null;
            return Json(new { msg = "Upload Report saved successfully" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMedicationReport()
        {
            JsonResult result = new JsonResult();
            uw.Context.Configuration.ProxyCreationEnabled = false;
            result.Data = uw.Context.Medications.Where(f => f.UserName == User.Identity.Name && f.REPORTSTATUS == "ACTIVE").OrderByDescending(f=>f.MEDICATIONID).ToList();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public JsonResult GetUploadReportDocPateint()
        {
            string PateintId = Convert.ToString(Session["PateintID"]);
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uow.Context.PatientReports.Where(f => f.UserName == PateintId && f.REPORTSTATUS == "ACTIVE")
               .Select(s => new { s.PATIENTREPORTID, s.REPORTNAME, s.NOTES, s.REPORTPATH }).OrderByDescending(f => f.PATIENTREPORTID).ToList();
            }
            catch (Exception e)
            {
                result.Data = null;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult GetMedicationReportDocPateint()
        {
            string PateintId = Convert.ToString(Session["PateintID"]);
            JsonResult result = new JsonResult();
            uw.Context.Configuration.ProxyCreationEnabled = false;
            result.Data = uw.Context.Medications.Where(f => f.UserName == PateintId && f.REPORTSTATUS == "ACTIVE").OrderByDescending(f => f.MEDICATIONID).ToList();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [HttpGet]
        [Authorize]
        public void _medication()
        {
         
        }

        [HttpPost]
        [Authorize]
        public ActionResult _medication(HealthHistoryModel  model)
        {
             try
            {
                if (model.MEDICATIONNAME != null)
                {
                    if (model.DOCTORID != null)
                    {
                        using (MedicationRepository repo = new MedicationRepository(uw))
                        {
                            MEDICATION dw = new MEDICATION();

                            dw.State = State.Added;
                            dw.CREATEDBY = User.Identity.Name;
                            dw.DateCreated = DateTime.UtcNow;
                            dw.MODIFIEDBY = User.Identity.Name;
                            dw.MODIFIEDON = DateTime.UtcNow;
                            dw.MEDICATIONNAME = model.MEDICATIONNAME;
                            dw.REPORTSTATUS = "Active";
                            dw.PRESCRIBEDDATE = model.PRESCRIBEDDATE;
                            dw.STOPPEDDATE = model.STOPPEDDATE;
                            dw.COMMENT = model.COMMENT;
                            dw.DOCTORID = model.DOCTORID;
                            dw.UserName = User.Identity.Name;
                            repo.InsertOrUpdate(dw);
                            uw.Save();

                           
                        }
                    }
                    else
                    {
                        Session["tabindex"] = "tab2";
                        Session["Message"] = "Error :Doctor Name  Required";
                        return Json(new { msg = "Error :Doctor Name  Required" }, JsonRequestBehavior.AllowGet);
                       
                    }                   
                }
                else
                {
                    Session["tabindex"] = "tab2";
                    Session["Message"] = "Error :Medication Name  Required";
                    return Json(new { msg = "Error :Medication Name  Required" }, JsonRequestBehavior.AllowGet);                   
                  
                }
             } 
              
                
            
            catch (Exception ex)
            {
                Session["tabindex"] = "tab2";
                Session["Message"] = "Error :Please Try Again";
            }

             Session["tabindex"] = "tab2";
             Session["Message"] = "Medication Report  saved successfully";
             return Json(new { msg = "Medication Report saved successfully" }, JsonRequestBehavior.AllowGet);

           }
        public JsonResult GetUploadReportDoc()
        {
            JsonResult result = new JsonResult();
            uow.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uow.Context.Appointments
                .Join(uow.Context.PatientReports,
                ap => ap.PATIENTID, pr => pr.UserName,
                (ap, pr) => new { pr.PATIENTREPORTID, pr.DateCreated, pr.REPORTNAME, pr.NOTES, pr.REPORTPATH, pr.DOCTORREPORTSTATUS, ap.DOCTORID }).Where(f => f.DOCTORID == User.Identity.Name && f.DOCTORREPORTSTATUS == "Active").Distinct().OrderByDescending(f => f.PATIENTREPORTID).ToList();
                
              
            }
            catch (Exception e)
            {
                result.Data = null;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

      

        public JsonResult GetAllergyReportDoc()
        {
            JsonResult result = new JsonResult();
            uwrk.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uwrk.Context.Appointments
                .Join(uwrk.Context.Allergies,
                ap => ap.PATIENTID, al => al.UserName,
                (ap, al) => new { al.ALLERGYFILEPATH, al.DateCreated, al.ALLERGYID, al.ALLERGYREPORTNAME, al.COMMENT, al.DOCTORREPORTSTATUS, ap.DOCTORID }).Where(f => f.DOCTORID == User.Identity.Name && f.DOCTORREPORTSTATUS == "Active").Distinct().OrderByDescending(f => f.ALLERGYID).ToList();
                
            }
            catch (Exception e)
            {
                result.Data = null;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult GetAllergyReportDocPateint()
        {
            string PateintId = Convert.ToString(Session["PateintID"]);
            JsonResult result = new JsonResult();
            uwrk.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uwrk.Context.Allergies.Where(f => f.UserName == PateintId && f.REPORTSTATUS == "ACTIVE").OrderByDescending(f => f.ALLERGYID).ToList();
            }
            catch (Exception e)
            {
                result.Data = null;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        public JsonResult GetAllergyReport()
        {
            JsonResult result = new JsonResult();
            uwrk.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uwrk.Context.Allergies.Where(f => f.UserName == User.Identity.Name && f.REPORTSTATUS == "ACTIVE").OrderByDescending(f=>f.ALLERGYID).ToList();
            }
            catch (Exception e)
            {
                result.Data = null;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }


        [HttpGet]
        [Authorize]
        public void _allergies()
        {
           
        }
       
        [HttpPost]
        [Authorize]
        public JsonResult _allergies(HealthHistoryModel model)
        {
            
            try
            {
                if (model.ALLERGYREPORTNAME != null)
                {
                    if (ALLERGYFILEPATH != null)
                    {
                        using (AllergyRepository repo2 = new AllergyRepository(uwrk))
                        {
                            ALLERGY dw = new ALLERGY();

                            dw.CREATEDBY = User.Identity.Name;
                            dw.DateCreated = DateTime.UtcNow;
                            dw.MODIFIEDBY = User.Identity.Name;
                            dw.MODIFIEDON = DateTime.UtcNow;
                            dw.ALLERGYREPORTNAME = model.ALLERGYREPORTNAME;
                            dw.REPORTSTATUS = "Active";
                            dw.DOCTORREPORTSTATUS = "Active";
                            dw.COMMENT = model.COMMENT;
                            dw.ALLERGYFILEPATH = model.ALLERGYFILEPATH;
                            dw.UserName = User.Identity.Name;
                            dw.ALLERGYFILEPATH = ALLERGYFILEPATH;
                            repo2.InsertOrUpdate(dw);
                            uwrk.Save();
                            ALLERGYFILEPATH = null;
                        }
                    }
                    else
                    {
                        Session["tabindex"] = "tab4";
                        Session["Message"] = "Error :Please Upload Allergy Report";
                        return Json(new { msg = "Error :Please Upload Allergy Report" }, JsonRequestBehavior.AllowGet);
                    }
                     
                }
                else
                {
                    Session["tabindex"] = "tab4";
                    Session["Message"] = "Error :Allergy Name  Required";
                    return Json(new { msg = "Error :Allergy Name Required" }, JsonRequestBehavior.AllowGet);
                }
            }



            catch (Exception ex)
            {
                Session["tabindex"] = "tab4";
                Session["Message"] = "Error :Please Try Again";
              ALLERGYFILEPATH = null;
            }
            Session["tabindex"] = "tab4";
            Session["Message"] = "Allergy Report  saved successfully";
            ALLERGYFILEPATH = null;
            return Json(new { msg = "Allergy Report saved successfully" }, JsonRequestBehavior.AllowGet); 
         }

        public JsonResult GetProcedureReport()
        {
            JsonResult result = new JsonResult();
            uwork.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uwork.Context.Procedures.Where(f => f.UserName == User.Identity.Name && f.REPORTSTATUS == "ACTIVE").OrderByDescending(f=>f.PROCEDUREID).ToList();
            }
            catch (Exception e)
            {
                result.Data = null;
            }
           
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public JsonResult GetProcedureReportDocPatient()
        {
            string PateintId = Convert.ToString(Session["PateintID"]);
            JsonResult result = new JsonResult();
            uwork.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uwork.Context.Procedures.Where(f => f.UserName  == PateintId && f.REPORTSTATUS == "ACTIVE").OrderByDescending(f => f.PROCEDUREID).ToList();
            }
            catch (Exception e)
            {
                result.Data = null;
            }

            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [HttpGet]
        [Authorize]
        public void _procedure()
        {
            
        }
        [HttpPost]
        [Authorize]
        public JsonResult _procedure(HealthHistoryModel model)
        {
            try
            {
                if (model.PROCEDUREREPORTNAME != null)
                {
                    using (ProcedureRepository repo3 = new ProcedureRepository(uwork))
                    {
                        PROCEDURE dw = new PROCEDURE();

                        dw.CREATEDBY =  User.Identity.Name;
                        dw.DateCreated = DateTime.UtcNow;
                        dw.MODIFIEDBY =  User.Identity.Name;
                        dw.MODIFIEDON = DateTime.UtcNow;
                        dw.PROCEDUREREPORTNAME = model.PROCEDUREREPORTNAME;
                        dw.PROCEDURENOTES = model.PROCEDURENOTES;
                        dw.PROCEDUREDATE = model.PROCEDUREDATE;
                        dw.REPORTSTATUS = "Active";
                        dw.UserName =  User.Identity.Name;                       
                        repo3.InsertOrUpdate(dw);
                        uwork.Save();
                    }
                     model.MessageUpdateStatus = "Record Inserted";
                    
                     Session["tabindex"] = "tab5";
                     Session["Message"] = "Procedure Report   saved successfully";
                     return Json(new { msg = "Procedure Report  saved successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session["tabindex"] = "tab5";
                    Session["Message"] = "Error :Procedure  Name  Required";
                    return Json(new { msg = "Error :Procedure  Name  Required" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Session["tabindex"] = "tab5";
                Session["Message"] = "Error :Please Try Again";
                return Json(new { msg = "Error :Please Retry after sometimes" }, JsonRequestBehavior.AllowGet);
            }
         
           
        }

        public JsonResult GetImmuneReportDocPateint()
        {
            string PateintId = Convert.ToString(Session["PateintID"]);
            JsonResult result = new JsonResult();
            uwk.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uwk.Context.Immunizations.Where(f => f.UserName == PateintId && f.REPORTSTATUS == "ACTIVE").OrderByDescending(f => f.IMMUNIZATIONID).ToList();
            }
            catch (Exception e)
            {
                result.Data = null;
            }
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public JsonResult GetImmuneReport()
        {
            JsonResult result = new JsonResult();
            uwk.Context.Configuration.ProxyCreationEnabled = false;
            try
            {
                result.Data = uwk.Context.Immunizations.Where(f => f.UserName == User.Identity.Name&& f.REPORTSTATUS == "ACTIVE").OrderByDescending(f=>f.IMMUNIZATIONID).ToList();
            }
            catch (Exception e)
            {
                result.Data = null;
            }           
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [HttpGet]
        [Authorize]
        public void _immunization()
        {
           
        }
        [HttpPost]
        [Authorize]
        public JsonResult _immunization(HealthHistoryModel model)
        {
            try
            {
                if (model.IMMUNIZATIONNAME != null)
                {

                    using (ImmunizationRepository repo1 = new ImmunizationRepository(uwk))
                    {

                        IMMUNIZATION dw = new IMMUNIZATION();

                        dw.CREATEDBY =  User.Identity.Name;
                        dw.DateCreated = DateTime.UtcNow;
                        dw.MODIFIEDBY =  User.Identity.Name;
                        dw.MODIFIEDON = DateTime.UtcNow;
                        dw.IMMUNIZATIONNAME = model.IMMUNIZATIONNAME;
                        dw.IMMUNEDATE = model.IMMUNEDATE;
                        dw.REPORTSTATUS = "Active";
                        dw.COMMENT = model.COMMENT;
                        dw.UserName =  User.Identity.Name;
                        repo1.InsertOrUpdate(dw);
                        uwk.Save();
                    }
                }
                else
                {
                    Session["tabindex"] = "tab3";
                    Session["Message"] = "Error :Immunization Name  Required";
                    return Json(new { msg = "Error :Immunization Name  Required" }, JsonRequestBehavior.AllowGet);
                }
            }



            catch (Exception ex)
            {
                Session["tabindex"] = "tab3";
                Session["Message"] = "Error :Please Try Again";
            }

            Session["tabindex"] = "tab3";
            Session["Message"] = "Immunization Report  saved successfully";
            return Json(new { msg = "Immunization Report  saved successfully" }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [Authorize]
        public JsonResult DeleteRepoMessageDoc(HealthHistoryModel model)
        {
            try
            {
                if (model.PatientRepoDeleteIDs != null)
                {
                    for (int i = 0; i < model.PatientRepoDeleteIDs.Length; i++)
                    {
                        DOCVIDEO.HealthHistoryServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.HealthHistoryServiceRepoUOW.UnitOfWork();
                        var repo = new HealthHistoryRepository(uow);
                        var temp = model.PatientRepoDeleteIDs[i];
                        var data = uow.Context.PatientReports.SingleOrDefault(f => f.PATIENTREPORTID == temp);
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        data.DOCTORREPORTSTATUS = "DELETE";
                        repo.InsertOrUpdate(data);
                        uow.Save();
                        repo = null;
                        uow = null;

                    }
                }
                else
                {
                    return Json(new { msg = "Error : No Upload Report Selected" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                
                 return Json(new { msg = "Please Retry" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Report deleted successfully" }, JsonRequestBehavior.AllowGet);
           
        }
        [HttpPost]
        [Authorize]
        public ActionResult DeleteRepoMessage(HealthHistoryModel model)
        {
            try
            {
                if (model.PatientRepoDeleteIDs!=null)
                {
                    for (int i = 0; i < model.PatientRepoDeleteIDs.Length; i++)
                    {
                        DOCVIDEO.HealthHistoryServiceRepoUOW.UnitOfWork uow = new DOCVIDEO.HealthHistoryServiceRepoUOW.UnitOfWork();
                        var repo = new HealthHistoryRepository(uow);
                        var temp = model.PatientRepoDeleteIDs[i];
                        var data = uow.Context.PatientReports.SingleOrDefault(f => f.PATIENTREPORTID == temp);
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        data.REPORTSTATUS = "DELETE";
                        data.DOCTORREPORTSTATUS = "DELETE";
                        repo.InsertOrUpdate(data);
                        uow.Save();
                        repo = null;
                        uow = null;
                        Session["tabindex"] = "tab1";
                        Session["Message"] = "Report deleted successfully";
                    }
                }
                else
                {
                    Session["tabindex"] = "tab1";
                    Session["Message"] = "Error :No Reports are there for delete";
                }

            }
            catch (Exception ex)
            {
                Session["tabindex"] = "tab1";
                Session["Message"] = "Please Try Again";
            }

            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        [Authorize]
        public JsonResult DeleteAllergyReportDoc(HealthHistoryModel model)
        {
            try
            {
                if (model.AllergyDeleteIDs != null)
                {
                    for (int i = 0; i < model.AllergyDeleteIDs.Length; i++)
                    {
                        AllergyUnitOfWork uwrk = new AllergyUnitOfWork();
                        var repo = new AllergyRepository(uwrk);
                        var temp = model.AllergyDeleteIDs[i];
                        var data = uwrk.Context.Allergies.SingleOrDefault(f => f.ALLERGYID == temp);
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        data.DOCTORREPORTSTATUS = "DELETE";
                        repo.InsertOrUpdate(data);
                        uwrk.Save();
                        repo = null;
                        uwrk = null;

                    }
                }
                else
                {
                    return Json(new { msg = "Error : No Allergy Report Selected" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = "Error : Please Try Again";
                return Json(new { msg = "Error : Please Retry" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { msg = "Allery Report deleted successfully" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public ActionResult DeleteAllergyReport(HealthHistoryModel model)
        {
            try
            {
                if (model.AllergyDeleteIDs != null)
                {
                    for (int i = 0; i < model.AllergyDeleteIDs.Length; i++)
                    {
                        AllergyUnitOfWork uwrk = new AllergyUnitOfWork();
                        var repo = new AllergyRepository(uwrk);
                        var temp = model.AllergyDeleteIDs[i];
                        var data = uwrk.Context.Allergies.SingleOrDefault(f => f.ALLERGYID == temp);
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        data.REPORTSTATUS = "DELETE";
                        data.DOCTORREPORTSTATUS = "DELETE";
                        repo.InsertOrUpdate(data);
                        uwrk.Save();
                        repo = null;
                        uwrk = null;
                        Session["tabindex"] = "tab4";
                        Session["Message"] = "Allergy Report deleted successfully";

                    }
                }
                else
                {
                    Session["tabindex"] = "tab4";
                    Session["Message"] = "Error : No Allergy Report to delete";
                }
            }
            catch (Exception ex)
            {
                Session["tabindex"] = "tab4";
                Session["Message"] = "Error :Please Try Again";
            }
            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public ActionResult DeleteImmuneRepo(HealthHistoryModel model)
        {
            try
            {
                if(model.ImmuneDeleteIDs!=null)
                {
                for (int i = 0; i < model.ImmuneDeleteIDs.Length; i++)
                {
                    ImmunizationUnitOfWork uwk = new ImmunizationUnitOfWork();
                    var repo = new ImmunizationRepository(uwk);
                    var temp = model.ImmuneDeleteIDs[i];
                    var data = uwk.Context.Immunizations.SingleOrDefault(f => f.IMMUNIZATIONID == temp);
                    data.State = DOCVIDEO.ObjectState.State.Modified;
                    data.REPORTSTATUS = "DELETE";                   
                    repo.InsertOrUpdate(data);
                    uwk.Save();
                    repo = null;
                    uwk = null;
                    Session["tabindex"] = "tab3";
                    Session["Message"] = "Immunization Report deleted successfully";

                }
                }
                else
                {
                    Session["tabindex"] = "tab3";
                    Session["Message"] = "Error :No Immunization Report to delete";
                }
            }
            catch (Exception ex)
            {
                Session["tabindex"] = "tab3";
                Session["Message"] = "Error :Please Try Again";
            }
            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public ActionResult DeleteMedicationRepo(HealthHistoryModel model)
        {
            try
            {
                if (model.MedicationDeleteIDs != null)
                {
                    for (int i = 0; i < model.MedicationDeleteIDs.Length; i++)
                    {
                        MedicationUnitOfWork uw = new MedicationUnitOfWork();
                        var repo = new MedicationRepository(uw);
                        var temp = model.MedicationDeleteIDs[i];
                        var data = uw.Context.Medications.SingleOrDefault(f => f.MEDICATIONID == temp);
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        data.REPORTSTATUS = "DELETE";
                        repo.InsertOrUpdate(data);
                        uw.Save();
                        repo = null;
                        uw = null;
                        Session["tabindex"] = "tab2";
                        Session["Message"] = "Medication Report deleted successfully";

                    }
                }
                else
                {
                    Session["tabindex"] = "tab2";
                    Session["Message"] = "Error :No Medication Report to delete";
                }
            }
            catch (Exception ex)
            {
                Session["tabindex"] = "tab2";
                Session["Message"] = "Error :Please Try Again";
            }
            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Authorize]
        public ActionResult DeleteProcedureRepo(HealthHistoryModel model)
        {
            try
            {
                if (model.ProcedureDeleteIDs != null)
                {
                    for (int i = 0; i < model.ProcedureDeleteIDs.Length; i++)
                    {
                        ProcedureUnitOfWork uwork = new ProcedureUnitOfWork();
                        var repo = new ProcedureRepository(uwork);
                        var temp = model.ProcedureDeleteIDs[i];
                        var data = uwork.Context.Procedures.SingleOrDefault(f => f.PROCEDUREID == temp);
                        data.State = DOCVIDEO.ObjectState.State.Modified;
                        data.REPORTSTATUS = "DELETE";
                        repo.InsertOrUpdate(data);
                        uwork.Save();
                        repo = null;
                        uwork = null;
                        Session["tabindex"] = "tab5";
                        Session["Message"] = "Porcedure Report deleted successfully";
                    }
                }
                else
                {
                    Session["tabindex"] = "tab5";
                    Session["Message"] = "Error :No Porcedure Report to delete";
                }
            }
            catch (Exception ex)
            {
                Session["tabindex"] = "tab5";
                Session["Message"] = "Error :Please Try Again";
            }
            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
        }
    }

}
