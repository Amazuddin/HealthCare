using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HealthCare.Context;
using HealthCare.Models;
using Newtonsoft.Json;
using Rotativa.MVC;

namespace HealthCare.Controllers
{
    public class HealthController : Controller
    {
        PrivacyController privacy = new PrivacyController();
        //
        // GET: /Health/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DoctorInformation()
        {
            if (Session["PatientId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.DoctorInformation = "active";
            List<DoctorCategory> categories;
            using (var ctx = new HealthContext())
            {
                categories = ctx.Specialist.ToList();
            }
            ViewBag.Category = categories;
            return View();
        }

        public JsonResult GetAllDoctorInfoById(int id)
        {
            List<Doctor> doctor = new List<Doctor>();
            using (var ctx = new HealthContext())
            {
                var a = ctx.Doctors.Where(s => s.CategoryId == id).Select(c => new { c.Image, c.Name, c.Degree, c.Fees, c.Schedule }); ;
                foreach (var k in a)
                {
                    Doctor doc = new Doctor();
                    doc.Name = privacy.Decrypt(k.Name);
                    doc.Degree = privacy.Decrypt(k.Degree);
                    doc.Image = k.Image;
                    doc.Fees = k.Fees;
                    doc.Schedule = k.Schedule;
                    doctor.Add(doc);
                }
            }
            return Json(doctor);
        }

        public ActionResult Appointment()
        {
            if (Session["PatientId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.Appointment = "active";
            List<DoctorCategory> categories;
            using (var ctx = new HealthContext())
            {
                categories = ctx.Specialist.ToList();
            }

            ViewBag.Category = categories;
            return View();
        }

        public JsonResult GetAllDoctorNameById(int id)
        {
            List<Doctor> doctors = new List<Doctor>();
            using (var ctx = new HealthContext())
            {
                var a = ctx.Doctors.Where(s => s.CategoryId == id);
                foreach (var k in a)
                {
                    Doctor doc = new Doctor();
                    doc.Id = k.Id;
                    doc.Name = privacy.Decrypt(k.Name);
                    doctors.Add(doc);
                }
            }
            return Json(doctors);
        }

        [HttpPost]
        public ActionResult Appointment(PatientAppointment patientAppointment)
        {
            ViewBag.Appointment = "active";
            patientAppointment.PatientId = Convert.ToInt32(Session["PatientId"]);
            List<DoctorCategory> specialists;
            using (var db = new HealthContext())
            {
                db.Appointment.Add(patientAppointment);
                db.SaveChanges();
                specialists = db.Specialist.ToList();
            }
            ViewBag.Category = specialists;
            ViewBag.Error = '1';
            return View();
        }

        public ActionResult AppointmentSchedule()
        {
            if (Session["ReceptionistId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.AppointmentSchedule = "active";
            List<DoctorCategory> specialists;
            using (var db = new HealthContext())
            {
                specialists = db.Specialist.ToList();
            }
            ViewBag.Category = specialists;
            return View();
        }

        public JsonResult GetAllPatientByDoctorId(int id, string date)
        {
            List<PatientAppointment> patients = new List<PatientAppointment>();
            using (var db = new HealthContext())
            {
                var appointments = db.Appointment.Where(s => s.DoctorId == id && s.Date == date).OrderBy(i => i.Id).ToList();
                foreach (var item in appointments)
                {
                    PatientAppointment patient = new PatientAppointment();
                    patient.Id = item.Id;
                    patient.PatientName = item.PatientName;
                    patient.Number = item.Number;
                    patient.Email = item.Email;
                    patient.Gender = item.Gender;
                    patient.IsAccepted = item.IsAccepted;
                    patients.Add(patient);
                }
            }
            return Json(patients);
        }

        public JsonResult ChangeApproval(int id, int value, int docId, string date)
        {
            string serialNo;
            using (var ctx = new HealthContext())
            {
                var numberOfApproval = ctx.Appointment.Count(i => i.DoctorId == docId && i.Date == date && i.SerialNumber != null);
                serialNo = "SL#"+ (numberOfApproval + 1);
                PatientAppointment appintment = ctx.Appointment.Single(c => c.Id == id);
                if (appintment.SerialNumber == null)
                {
                    appintment.IsAccepted = value;
                    appintment.SerialNumber = serialNo;
                    ctx.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = "Serial Number Already Exists!";
                }
            }
            return Json('1');
        }

        //**************************patient profile******************************//

        public ActionResult PatientProfile()
        {
            if (Session["PatientId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.PatientProfile = "active";
            Register patient = new Register();
            int patientid = Convert.ToInt32(Session["PatientId"]);
            using (var db = new HealthContext())
            {
                var u = db.Registrations.Where(k => k.Id == patientid).Select(c => new { c.Name, c.Age, c.Address, c.PhoneNo });
                foreach (var j in u)
                {
                    patient.Name = privacy.Decrypt(j.Name);
                    patient.Address = privacy.Decrypt(j.Address);
                    patient.Age = j.Age;
                    patient.PhoneNo = privacy.Decrypt(j.PhoneNo);
                }
                ViewBag.Patient = patient;
                return View();
            }

        }

        public ActionResult PatientProfileUpdate()
        {
            if (Session["PatientId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.PatientProfileUpdate = "active";
            Register patient = new Register();
            int patientid = Convert.ToInt32(Session["PatientId"]);
            using (var db = new HealthContext())
            {
                var u = db.Registrations.Where(k => k.Id == patientid).Select(c => new { c.Name, c.Age, c.Address, c.PhoneNo });
                foreach (var j in u)
                {
                    patient.Name = privacy.Decrypt(j.Name);
                    patient.Address = privacy.Decrypt(j.Address);
                    patient.Age = j.Age;
                    patient.PhoneNo = privacy.Decrypt(j.PhoneNo);
                }
                ViewBag.Patient = patient;
                return View();
            }
        }
        public ActionResult Update(Register patient)
        {
            int id = Convert.ToInt32(Session["PatientId"]);
            string useremail = privacy.Encrypt(patient.Email);
            using (var db = new HealthContext())
            {
                Register pa = db.Registrations.Single(e => e.Id == id);
                if (pa.Email == useremail)
                {
                    pa.Name = privacy.Encrypt(patient.Name);
                    pa.Age = patient.Age;
                    pa.Address = privacy.Encrypt(patient.Address);
                    pa.PhoneNo = privacy.Encrypt(patient.PhoneNo);
                    db.SaveChanges();
                    return RedirectToAction("PatientProfile", "Health");
                }

                return RedirectToAction("PatientProfile", "Health");

            }
        }
        public ActionResult Notification()
        {
            if (Session["PatientId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.Notification = "active";
            int id = Convert.ToInt32(Session["PatientId"]);
            List<DoctorAppointmentView> appointmentList = new List<DoctorAppointmentView>();

            using (var db = new HealthContext())
            {
                //List<PatientAppointment> lappoint = (from a in db.Appointment
                //                                     where a.PatientId == id && a.IsSeen == 0
                //                                     select a).ToList();
                //foreach (PatientAppointment p in lappoint)
                //{
                //    p.IsSeen = 1;
                //}
                //db.SaveChanges();

                //Session["PatientNotification"] = 0;

                var q = (from ab in db.Doctors
                         join p in db.Specialist on ab.CategoryId equals p.Id
                         join a in db.Appointment on ab.Id equals a.DoctorId
                         where a.DoctorId == ab.Id && a.PatientId == id
                         select new
                         {
                             appointmentDate = a.Date,
                             appointmentName = ab.Name,
                             appointmentSpecialist = p.Title,
                             aApproval = a.IsAccepted,
                             appointmentSerialNumber = a.SerialNumber
                         });
                foreach (var d in q)
                {
                    DoctorAppointmentView ap = new DoctorAppointmentView();
                    ap.DoctorName = privacy.Decrypt(d.appointmentName);
                    ap.Date = d.appointmentDate;
                    ap.Specialist = d.appointmentSpecialist;
                    ap.Approval = d.aApproval;
                    ap.SerialNumber = d.appointmentSerialNumber;
                    appointmentList.Add(ap);
                }
            }
            return View(appointmentList);
        }

        //**********************reception profile***********************//


        public ActionResult ReceptionistProfile()
        {
            if (Session["ReceptionistId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.ReceptionistProfile = "active";
            Receptionist receptionist = new Receptionist();
            int receptionistid = Convert.ToInt32(Session["ReceptionistId"]);
            using (var db = new HealthContext())
            {
                var q = db.Receptionist.Where(k => k.Id == receptionistid).Select(c => new { c.Name, c.Address, c.PhoneNo });
                foreach (var j in q)
                {
                    receptionist.Name = privacy.Decrypt(j.Name);
                    receptionist.Address = privacy.Decrypt(j.Address);
                    receptionist.PhoneNo = privacy.Decrypt(j.PhoneNo);
                }
                ViewBag.Receptionist = receptionist;
                return View();
            }

        }
        public ActionResult ReceptionistProfileUpdate()
        {
            if (Session["ReceptionistId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.ReceptionistProfileUpdate = "active";
            Receptionist receptionist = new Receptionist();
            int receptionistid = Convert.ToInt32(Session["ReceptionistId"]);
            using (var db = new HealthContext())
            {
                var q = db.Receptionist.Where(k => k.Id == receptionistid).Select(c => new { c.Name, c.Address, c.PhoneNo });
                foreach (var j in q)
                {
                    receptionist.Name = privacy.Decrypt(j.Name);
                    receptionist.Address = privacy.Decrypt(j.Address);
                    receptionist.PhoneNo = privacy.Decrypt(j.PhoneNo);
                }
                ViewBag.Receptionist = receptionist;
                return View();
            }
        }
        public ActionResult UpdateReceptionist(Receptionist receptionist)
        {
            int id = Convert.ToInt32(Session["ReceptionistId"]);
            string useremail = privacy.Encrypt(receptionist.Email);
            using (var db = new HealthContext())
            {
                Receptionist pa = db.Receptionist.Single(e => e.Id == id);
                if (pa.Email == useremail)
                {
                    pa.Name = privacy.Encrypt(receptionist.Name);
                    pa.Address = privacy.Encrypt(receptionist.Address);
                    pa.PhoneNo = privacy.Encrypt(receptionist.PhoneNo);
                    db.SaveChanges();
                    return RedirectToAction("ReceptionistProfile", "Health");
                }
                return RedirectToAction("ReceptionistProfile", "Health");
            }
        }


        //*************************Doctor profile*****************************//

        public ActionResult DoctorProfile()
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.DoctorProfile = "active";
            Doctor doctor = new Doctor();
            int doctorid = Convert.ToInt32(Session["DoctorId"]);
            using (var db = new HealthContext())
            {
                var q = db.Doctors.Where(k => k.Id == doctorid).Select(c => new { c.Name, c.Degree, c.Fees });
                foreach (var j in q)
                {
                    doctor.Name = privacy.Decrypt(j.Name);
                    doctor.Degree = privacy.Decrypt(j.Degree);
                    doctor.Fees = j.Fees;
                }
                ViewBag.Doctor = doctor;
                return View();
            }
        }

        public ActionResult DoctorProfileUpdate()
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.DoctorProfileUpdate = "active";
            Doctor doctor = new Doctor();
            int doctorid = Convert.ToInt32(Session["DoctorId"]);
            using (var db = new HealthContext())
            {
                var q = db.Doctors.Where(k => k.Id == doctorid).Select(c => new { c.Name, c.Degree, c.Fees });
                foreach (var j in q)
                {
                    doctor.Name = privacy.Decrypt(j.Name);
                    doctor.Degree = privacy.Decrypt(j.Degree);
                    doctor.Fees = j.Fees;
                }
                ViewBag.Doctor = doctor;
                return View();
            }
        }

        public ActionResult UpdateDoctor(Doctor doctor)
        {
            int id = Convert.ToInt32(Session["DoctorId"]);
            string useremail = privacy.Encrypt(doctor.Email);
            using (var db = new HealthContext())
            {
                Doctor pa = db.Doctors.Single(e => e.Id == id);
                if (pa.Email == useremail)
                {
                    pa.Name = privacy.Encrypt(doctor.Name);
                    pa.Degree = privacy.Encrypt(doctor.Degree);
                    pa.Fees = doctor.Fees;
                    db.SaveChanges();
                    return RedirectToAction("DoctorProfile", "Health");
                }
                return RedirectToAction("DoctorProfile", "Health");
            }
        }

        public ActionResult Feedback()
        {
            if (Session["PatientId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.Feedback = "active";
            return View();
        }

        [HttpPost]
        public ActionResult Feedback(Feedbacks feedbacks)
        {
            using (var db = new HealthContext())
            {
                db.Feedbackses.Add(feedbacks);
                db.SaveChanges();
            }
            ViewBag.Yes = '1';
            return View();
        }

        public ActionResult Graph()
        {
            if (Session["ReceptionistId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.Graph = "active";
            int[] ar = new int[3];
            for (int i = 0; i <= 2; i++)
                ar[i] = 0;
            int size = 1;
            using (var ctx = new HealthContext())
            {
                string sql = "SELECT Review, COUNT(*) as name FROM Feedbacks GROUP BY Review";
                SqlConnection cn = new SqlConnection(ctx.Database.Connection.ConnectionString);
                SqlCommand command = new SqlCommand(sql, cn);
                cn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int a = Convert.ToInt32(reader["name"]);
                        ar[size - 1] = a;
                        size = size + 1;
                    }
                }
                cn.Close();
            }
            string dataStr = JsonConvert.SerializeObject(ar, Formatting.None);
            ViewBag.Data = dataStr;
            return View();

        }

        public ActionResult Prescription()
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            ViewBag.Prescription = "active";
            return View();
        }
        public JsonResult GetAllPatient(string date)
        {
            List<AppointmentView> pt = new List<AppointmentView>();
            int doc = Convert.ToInt32(Session["DoctorId"]);
            using (var ctx = new HealthContext())
            {
                var data = from a in ctx.Appointment
                           join p in ctx.Registrations
                               on a.PatientId equals p.Id
                           where a.DoctorId == doc && a.Date == date
                           select new
                           {
                               pappointmentId = a.Id,
                               patientName = p.Name,
                               patientAge = p.Age,
                           };
                foreach (var dc in data)
                {
                    AppointmentView p = new AppointmentView();
                    p.AppointmentId = dc.pappointmentId;
                    p.PatientName = privacy.Decrypt(dc.patientName);
                    p.Age = dc.patientAge;
                    pt.Add(p);
                }
            }
            return Json(pt);
        }

        [HttpPost]
        public ActionResult Prescription(Prescription prescription, int AppointmentId)
        {
            prescription.DocId = Convert.ToInt32(Session["DoctorId"]);
            ViewBag.Prescription = "active";
            string searchdate = DateTime.Now.ToString("MM/dd/yyyy");
            string pdfname;
            pdfname = DateTime.Now.ToString("dd-MM-yyyy") + "_" + Guid.NewGuid() + ".pdf";
            using (var db = new HealthContext())
            {
                var presdoc = from ap in db.Appointment
                              join p in db.Registrations
                                  on ap.PatientId equals p.Id
                              join d in db.Doctors
                                  on ap.DoctorId equals d.Id
                              where ap.Id == AppointmentId
                              select new
                              {
                                  PatientName = p.Name,
                                  PatientAge = p.Age
                              };
                foreach (var v in presdoc)
                {
                    prescription.PatientName = privacy.Decrypt(v.PatientName);
                    prescription.PatientAge = v.PatientAge;
                }
                PatientAppointment app = db.Appointment.Single(c => c.Id == AppointmentId && c.DoctorId == prescription.DocId
                    && c.Date == searchdate);
                app.Prescription = "Prescriptions/" + pdfname;
                db.SaveChanges();
            }
            var printpdf = new ActionAsPdf("MakePdf", prescription)
            {
                FileName = pdfname,
                //RotativaOptions = new Rotativa.Core.DriverOptions()
                //{
                //    PageSize = Rotativa.Core.Options.Size.A5,
                //    PageOrientation = Rotativa.Core.Options.Orientation.Landscape
                //}
            };
            
            string path = Server.MapPath("~/Prescriptions");
            string a = Path.Combine(path, pdfname);
            var byteArray = printpdf.BuildPdf(ControllerContext);
            var fileStream = new FileStream(a, FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();

            return printpdf;
        }

        public ActionResult MakePdf(Prescription prescription)
        {
            DoctorView doctor = new DoctorView();
            //string email = privacy.Encrypt(prescription.PatientEmail);
            using (var db = new HealthContext())
            {
                var q =
                (from a in db.Doctors
                 join p in db.Specialist on a.CategoryId equals p.Id
                 where a.Id == prescription.DocId
                 select new
                 {
                     dId = a.Id,
                     dName = a.Name,
                     dQualification = a.Degree,
                     dspecialist = p.Title
                 });
                foreach (var j in q)
                {

                    doctor.DocName = privacy.Decrypt(j.dName);
                    doctor.DocSpecialist = j.dspecialist;
                    doctor.DocDegree = privacy.Decrypt(j.dQualification);
                }
                ViewBag.Doctor = doctor;
                return View(prescription);
            }
        }

	}
}