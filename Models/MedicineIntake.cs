using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellBeingDiary.Models
{
    public class MedicineIntake
    {   
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
        public DateTime IntakeDateTime { get; set; }

        public bool IsTaken { get; set; }  
        public DateTime? TakenDateTime { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
