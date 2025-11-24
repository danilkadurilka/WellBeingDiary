using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellBeingDiary.Models
{
    public class DailySymptom
    {
        public int Id { get; set; }
        public int DailyDataId { get; set; }
        public DailyData DailyData { get; set; }
        public int SymptomId { get; set; }
        public Symptom Symptom { get; set; }

    }
}
