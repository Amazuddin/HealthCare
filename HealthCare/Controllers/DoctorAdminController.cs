using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HealthCare.Models;
using HealthCare.Context;

namespace HealthCare.Controllers
{
    public class DoctorAdminController : Controller
    {
        private HealthContext db = new HealthContext();
        PrivacyController privacy=new PrivacyController();
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

        // GET: /DoctorAdmin/
        public ActionResult Index()
        {

            List<Doctor> doctors = db.Doctors.ToList();
            foreach (Doctor doc in doctors)
            {
                
                doc.Name = privacy.Decrypt(doc.Name);
                doc.Degree = privacy.Decrypt(doc.Degree);
                doc.Email = privacy.Decrypt(doc.Email);
            }
            return View(doctors);
        }

        // GET: /DoctorAdmin/Details/5
       

        // GET: /DoctorAdmin/Create
        public ActionResult Create()
        {
            ViewBag.Specialist = db.Specialist.ToList();
            return View();
        }

        // POST: /DoctorAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Degree,Fees,Schedule,CategoryId,Email,Password")] Doctor doctor, HttpPostedFileBase Image, int categoryId)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {

                    try
                    {
                        string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                        string uploadUrl = Server.MapPath("~/picture");
                        Image.SaveAs(Path.Combine(uploadUrl, fileName));
                        doctor.Image = "picture/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                doctor.CategoryId = categoryId;
                doctor.Name = privacy.Encrypt(doctor.Name);
                doctor.Email = privacy.Encrypt(doctor.Email);
                doctor.Degree = privacy.Encrypt(doctor.Degree);
                doctor.Schedule = doctor.Schedule;
                doctor.Password = EncodePassword(doctor.Password);
                db.Doctors.Add(doctor);
                db.SaveChanges();
            }

            return RedirectToAction("Create", new { message = "Doctor Successfully Appointed" });
        }

        // GET: /DoctorAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.DoctorCategory = db.Specialist.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            doctor.Name = privacy.Decrypt(doctor.Name);
            doctor.Email = privacy.Decrypt(doctor.Email);
            doctor.Degree = privacy.Decrypt(doctor.Degree);
            return View(doctor);
        }

        // POST: /DoctorAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image,Degree,Fees,Schedule,CategoryId,Email,Password")] Doctor doctor, HttpPostedFileBase Image, int categoryId, string pastImage)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {

                    try
                    {
                        string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                        string uploadUrl = Server.MapPath("~/picture");
                        Image.SaveAs(Path.Combine(uploadUrl, fileName));
                        doctor.Image = "picture/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    doctor.Image = pastImage;
                }
                doctor.CategoryId = categoryId;
                doctor.Name = privacy.Encrypt(doctor.Name);
                doctor.Email = privacy.Encrypt(doctor.Email);
                doctor.Degree = privacy.Encrypt(doctor.Degree);
                doctor.Schedule = doctor.Schedule;
                doctor.Password = EncodePassword(doctor.Password);
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: /DoctorAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            doctor.Name = privacy.Decrypt(doctor.Name);
            doctor.Degree = privacy.Decrypt(doctor.Degree);
            doctor.Email = privacy.Decrypt(doctor.Email);
            return View(doctor);
        }

        // POST: /DoctorAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
