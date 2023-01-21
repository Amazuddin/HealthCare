using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.Models
{
    public class DoctorAppointmentView
    {
        public string DoctorName { get; set; }
        public string Date { get; set; }
        public string Specialist { get; set; }
        public string Prescription { get; set; }
        public int Approval { get; set; }
        public string SerialNumber { get; set; }
    }
}