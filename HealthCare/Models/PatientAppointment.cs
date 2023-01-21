using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.Models
{
    public class PatientAppointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int Number { get; set; } /*mobile number*/
        public string Email { get; set; }
        public string Date { get; set; }
        public string SpecialistId { get; set; }
        public int DoctorId { get; set; }
        public string Gender { get; set; }
        public int IsAccepted { get; set; }
        public int IsSeen { get; set; }
        public string Prescription { get; set; }
        public string SerialNumber { get; set; }
    }
}