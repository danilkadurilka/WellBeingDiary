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
        public string IsActiveText
        {
            get
            {
                if (IsActive) return "Да";
                else return "Нет";
            }

        }

        public TimeSpan IntakeTime { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
