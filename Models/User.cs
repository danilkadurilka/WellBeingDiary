using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellBeingDiary.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Height { get; set; }
        public double Weight { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string? PhotoPath { get; set; }

        public List<DailyData> DailyData { get; set; } = new();
        public List<Medicine> Medicines { get; set; } = new();
        public List<Report> Reports { get; set; } = new();
    }
}
