using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Degree { get; set; }
        public int Fees { get; set; }
        public string Schedule { get; set; }
        public int CategoryId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}