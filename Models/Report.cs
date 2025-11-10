using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellBeingDiary.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string ReportType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ReportData { get; set; }
        public string? Summary { get; set; }
        public string? Recommends { get; set; }

        public DateTime CreateAt { get; set; }
    }
}