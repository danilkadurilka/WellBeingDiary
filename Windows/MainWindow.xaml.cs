using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WellBeingDiary.Models;

namespace WellBeingDiary.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeBasicData();
        }
        public void InitializeBasicData()
        {
            if (Models.AppContext.Users.Count == 0)
            {
                Models.AppContext.Users.Add(new User
                {
                    Id = 1,
                    Login = "test",
                    Password = "test",
                    Name = "Test User",
                    Gender = "Мужской",
                    Height = 180,
                    Weight = 75,
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });
            }

            Models.AppContext.Symptoms = new List<Symptom>
            {
                new Symptom { Id = 1, Name = "Головная боль", Description = "Боль в области головы" },
                new Symptom { Id = 2, Name = "Тошнота", Description = "Чувство тошноты" },
                new Symptom { Id = 3, Name = "Головокружение", Description = "Потеря равновесия" },
                new Symptom { Id = 4, Name = "Слабость", Description = "Общая слабость организма" },
                new Symptom { Id = 5, Name = "Температура", Description = "Повышенная температура тела" }
            };
            Models.AppContext.DailyData = new List<DailyData>();
            Models.AppContext.Medicines = new List<Medicine>();
            Models.AppContext.MedicinesSchedule = new List<MedicineSchedule>();
            Models.AppContext.MedicineIntakes = new List<MedicineIntake>();
            Models.AppContext.Reports = new List<Report>();
            Models.AppContext.DailySymptoms = new List<DailySymptom>();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = BoxLogin.Text;
            string password = BoxPassword.Password;

            var user = Models.AppContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                MainPageWindow mainWindow = new MainPageWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new RegWindow();
            regWindow.Show();
            this.Close();
        }

    }
}
