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
    /// Логика взаимодействия для MedicineEditWindow.xaml
    /// </summary>
    public partial class MedicineEditWindow : Window
    {
        private User currentUser;
        private Medicine editMedicine;
        private bool isEditing = false;
        public MedicineEditWindow(User user, Medicine medicine = null)
        {
            InitializeComponent();
            currentUser = user;
            if (medicine != null)
            {
                editMedicine = medicine;
                isEditing = true;
                LoadMedicineData();
            }
            else 
            {
                editMedicine = new Medicine();
                DPStartDate.SelectedDate = DateTime.Today;
            }
        }

        public void LoadMedicineData()
        {
            BoxMedicineName.Text = editMedicine.MedicineName;
            BoxDose.Text = editMedicine.Dose;
            BoxInstructions.Text = editMedicine.Instructions;
            DPStartDate.SelectedDate = editMedicine.StartDate;
            if (editMedicine.EndDate.HasValue)
                DPEndDate.SelectedDate = editMedicine.EndDate.Value;
            BoxIsActive.IsChecked = editMedicine.IsActive;
            if (editMedicine.Schedules.Any())
            {
                MedicineSchedule schedule = editMedicine.Schedules[0];
                BoxMonday.IsChecked = schedule.Monday;
                BoxTuesday.IsChecked = schedule.Tuesday;
                BoxWednesday.IsChecked = schedule.Wednesday;
                BoxThursday.IsChecked = schedule.Thursday;
                BoxFriday.IsChecked = schedule.Friday;
                BoxSaturday.IsChecked = schedule.Saturday;
                BoxSunday.IsChecked = schedule.Sunday;
                BoxIntakeTime.Text = schedule.IntakeTime.ToString(@"hh\:mm");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(BoxMedicineName.Text))
            {
                MessageBox.Show("Введите название лекарства!");
                return;
            }

            try
            {
                editMedicine.MedicineName = BoxMedicineName.Text;
                editMedicine.Dose = BoxDose.Text;
                editMedicine.Instructions = BoxInstructions.Text;
                editMedicine.StartDate = DPStartDate.SelectedDate ?? DateTime.Today;
                editMedicine.EndDate = DPEndDate.SelectedDate;
                editMedicine.IsActive = BoxIsActive.IsChecked ?? true;
                editMedicine.UserId = currentUser.Id;

                if (isEditing == false)
                {
                    //editMedicine.Id???
                    Models.AppContext.Medicines?.Add(editMedicine);
                }
                else
                {
                    var oldSchedules = Models.AppContext.MedicinesSchedule?.Where(s => s.MedicineId == editMedicine.Id).ToList(); //var? 
                    if (oldSchedules !=  null)
                    {
                        foreach (var schedule in oldSchedules)
                        {
                            Models.AppContext.MedicinesSchedule?.Remove(schedule);
                        }
                    }

                }
                try 
                {
                    TimeSpan intakeTime = TimeSpan.Parse(BoxIntakeTime.Text);
                    MedicineSchedule schedule = new()
                    {
                        //id?
                        MedicineId = editMedicine.Id,
                        IntakeTime = intakeTime,
                        Monday = BoxMonday.IsChecked ?? false,
                        Tuesday = BoxTuesday.IsChecked ?? false,
                        Wednesday = BoxWednesday.IsChecked ?? false,
                        Thursday = BoxThursday.IsChecked ?? false,
                        Friday = BoxFriday.IsChecked ?? false,
                        Saturday = BoxSaturday.IsChecked ?? false,
                        Sunday = BoxSunday.IsChecked ?? false
                    };
                    Models.AppContext.MedicinesSchedule?.Add(schedule);
                }
                
                catch (OverflowException)
                {
                    MessageBox.Show("Неверное значение времени");
                    return;
                }
                this.DialogResult = true;
                this.Close();
        }

            catch (Exception ex)
            {
                MessageBox.Show($"Возникла ошибка в момент сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
