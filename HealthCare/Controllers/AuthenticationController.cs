using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HealthCare.Context;
using HealthCare.Models;

namespace HealthCare.Controllers
{
    public class AuthenticationController : Controller
    {
          PrivacyController privacy= new PrivacyController();

          public static string EncodePassword(string password)
          {
              Byte[] originalBytes;
              Byte[] encodedBytes;
              MD5 a;
              a = new MD5CryptoServiceProvider();
              originalBytes = ASCIIEncoding.Default.GetBytes(password);
              encodedBytes = a.ComputeHash(originalBytes);
              return BitConverter.ToString(encodedBytes);
          }
          public ActionResult Registration()
          {
              ViewBag.Registration = "active";
              return View();
          }



          [HttpPost]
          public ActionResult Registration(Register register)
          {
              using (var db = new HealthContext())
              {
                  register.Email = privacy.Encrypt(register.Email);
                  var e = db.Registrations.Where(c => c.Email == register.Email).ToList().Count;
                  if (e == 0)
                  {
                      register.Password = EncodePassword(register.Password);
                      register.Name = privacy.Encrypt(register.Name);
                      register.Address = privacy.Encrypt(register.Address);
                      register.PhoneNo = privacy.Encrypt(register.PhoneNo);
                      db.Registrations.Add(register);
                      db.SaveChanges();
                  }
                  else
                  {
                      ViewBag.Error = "Already Registered";
                  }
              }
              ViewBag.Yes = '1';
              return View();
          }
       

        public ActionResult Login()
        {
            ViewBag.Login = "active";
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            string password = EncodePassword(login.Password);
            string email = privacy.Encrypt(login.Email);
            using (var db = new HealthContext())
            {
                if (login.Type == 1)
                {
                    var p = db.Registrations.Where(c => c.Email == email && c.Password == password).Select(c => new { c.Id, c.Name }).ToList();
                    if (p.Any())
                    {
                        int i = 0;
                        foreach (var k in p)
                        {
                            i = k.Id;
                            Session["PatientId"] = k.Id;
                            Session["PatientName"] = k.Name;

                        }
                       
                        return RedirectToAction("Index", "Health");
                    }
                    else
                    {
                        ViewBag.Error = "Login Failed";
                    }
                }
                else if (login.Type == 2)
                {
                    var s = db.Doctors.Where(c => c.Email == email && c.Password == password).Select(c => new { c.Id, c.Name }).ToList();
                    if (s.Any())
                    {
                        foreach (var k in s)
                        {
                            Session["DoctorId"] = k.Id;
                            Session["DoctorName"] = k.Name;

                        }
                        return RedirectToAction("Index", "Health");
                    }
                    else
                    {
                        ViewBag.Error = "Login Failed";
                    }
                }
                else if (login.Type == 3)
                {
                    var d = db.Receptionist.Where(c => c.Email == email && c.Password == password).Select(c => new { c.Id, c.Name }).ToList();
                    if (d.Any())
                    {
                        foreach (var k in d)
                        {
                            Session["ReceptionistId"] = k.Id;
                            Session["ReceptionistName"] = k.Name;

                        }
                        return RedirectToAction("Index", "Health");
                    }
                    else
                    {
                        ViewBag.Error = "Login Failed";
                    }
                }
               
               
            }
            return View();
        }




        
        

        public ActionResult PatientLogout()
        {
            Session["PatientId"] = null;
            Session["PatientName"] = null;
            return RedirectToAction("LogIn", "Authentication");
        }
        public ActionResult DoctorLogout()
        {
            Session["DoctorId"] = null;
            Session["DoctorName"] = null;
            return RedirectToAction("LogIn", "Authentication");
        }
        public ActionResult ReceptionistLogout()
        {
            Session["ReceptionistId"] = null;
            Session["ReceptionistName"] = null;
            return RedirectToAction("LogIn", "Authentication");
        }

        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(Admin admin)
        {
            string password = EncodePassword(admin.Password);
            string email = privacy.Encrypt(admin.Email);
            using (var db = new HealthContext())
            {

                var p = db.Admin.Where(c => c.Email == email && c.Password == password).Select(c => new { c.Id, c.Email }).ToList();
                if (p.Any())
                {
                    foreach (var k in p)
                    {
                        Session["AdminId"] = k.Id;
                        Session["AdminEmail"] = k.Email;

                    }
                    return RedirectToAction("Index", "Health");
                }
                else
                {
                    ViewBag.Error = "Login Failed";
                }

            }
            return View();
        }
        public ActionResult AdminLogout()
        {
            Session["AdminId"] = null;
            Session["AdminEmail"] = null;
            return RedirectToAction("AdminLogin", "Authentication");
        }
	}
}