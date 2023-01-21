using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCare.Models
{
    public class Feedbacks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Review { get; set; }
        public string Message { get; set; }
    }
}