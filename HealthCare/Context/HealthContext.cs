using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using HealthCare.Models;

namespace HealthCare.Context
{
    public class HealthContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PatientAppointment> Appointment { get; set; }
        public DbSet<DoctorCategory> Specialist { get; set; }
        public DbSet<Register> Registrations { get; set; }
        public DbSet<Receptionist> Receptionist { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Feedbacks> Feedbackses { get; set; }
    }
}