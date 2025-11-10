using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellBeingDiary.Models
{
    public static class AppContext
    {
        public static List<User> Users { get; set; } = new();
        public static List<DailyData> DailyData { get; set; }
        public static List<Medicine> Medicines { get; set; }
        public static List<MedicineSchedule> MedicinesSchedule { get; set; }
        public static List<MedicineIntake> MedicineIntakes { get; set; }
        public static List<Report> Reports { get; set; }
        public static List<Symptom> Symptoms { get; set; }
        public static List<DailySymptom> DailySymptoms { get; set; }
    }
}