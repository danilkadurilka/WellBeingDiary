using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellBeingDiary.Models
{
    public class Medicine
    {   
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        
        public string MedicineName { get; set; }
        public string? Dose {  get; set; }
        public string? Instructions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        public List<MedicineSchedule> Schedules { get; set; } = new();
        public List<MedicineIntake> Intakes { get; set; } = new();

        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
