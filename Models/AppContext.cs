using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellBeingDiary.Models
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<DailyData> DailyData { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=WellBeingDiary.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<DailyData>().HasIndex(d => new { d.UserId, d.Date }).IsUnique();
            modelBuilder.Entity<Symptom>().HasData(
                new Symptom { Id = 1, Name = "Головная боль" },
                new Symptom { Id = 2, Name = "Ломота в суставах" },
                new Symptom { Id = 3, Name = "Боль в животе" },
                new Symptom { Id = 4, Name = "Повышенная температура тела" },
                new Symptom { Id = 5, Name = "Кашель" },
                new Symptom { Id = 6, Name = "Одышка" },
                new Symptom { Id = 7, Name = "Насморк" },
                new Symptom { Id = 8, Name = "Отечность" },
                new Symptom { Id = 9, Name = "Головокружение" },
                new Symptom { Id = 10, Name = "Диарея" },
                new Symptom { Id = 11, Name = "Рвота" },
                new Symptom { Id = 12, Name = "Запор" });
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Login = "test", Password = "test", Name = "Тестовый", Gender = "Мужской", Height = 180, Weight = 75, DateOfBirth = new DateOnly(1999, 4, 1) });
        }
    }
}