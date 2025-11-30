using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellBeingDiary.Models;

namespace WellBeingDiary.Models
{
    public class DailyData
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        public int? SystolicPressure { get; set; }
        public int? DiastolicPressure { get; set; }
        public int? Pulse { get; set; }

        public int? WellBeingRating { get; set; }
        public string? Symptoms { get; set; }
        public string? Notes { get; set; }

        public int? StepCount { get; set; }

        public DateTime? SleepStart { get; set; }
        public DateTime? SleepEnd { get; set; }
        public int? SleepQuality { get; set; }
        public int? NightAwake { get; set; }

        public double? Weight { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;

    }
}
