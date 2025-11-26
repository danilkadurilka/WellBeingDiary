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
        private Models.AppContext _context;
        public MedicineEditWindow(User user, Models.AppContext appContext, Medicine medicine = null)
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
        public bool CheckMedicineData()
        {
            if (string.IsNullOrWhiteSpace(BoxMedicineName.Text)) 
            {
                MessageBox.Show("Название лекарства не может быть пустым");
                return false;
            }
            if (DPStartDate.SelectedDate.HasValue && DPStartDate.SelectedDate.Value > DateTime.Today.AddDays(1))
            {
                MessageBox.Show("Дата начала приема не может быть в будущем");
                return false;
            }
            if (DPEndDate.SelectedDate.HasValue && DPStartDate.SelectedDate.HasValue)
            {
                if(DPEndDate.SelectedDate.Value < DPEndDate.SelectedDate.Value)
                {
                    MessageBox.Show("Дата окончания приема не может быть раньше даты начала");
                    return false;
                }
            }
            if (string.IsNullOrWhiteSpace(BoxIntakeTime.Text))
            {
                MessageBox.Show("Введите время приема");
                return false;
            }
            try
            {
                TimeSpan intakeTime = TimeSpan.Parse(BoxIntakeTime.Text);
            }
            catch (Exception ex)
            { 
                MessageBox.Show($"Возникла ошибка: {ex.Message}");
                return false;
            }
            if (BoxMonday.IsChecked == false || BoxTuesday.IsChecked == false || BoxWednesday.IsChecked == false || BoxThursday.IsChecked == false ||
                BoxFriday.IsChecked == false || BoxSaturday.IsChecked == false || BoxSunday.IsChecked == false)
            {
                MessageBox.Show("Выберите хотя бы один день для приема");
                return false;
            }
            return true;
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
            MedicineSchedule? schedule = _context.MedicinesSchedule.FirstOrDefault(s => s.MedicineId == editMedicine.Id);
            if (schedule != null)
            {
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
            if (CheckMedicineData())
                return;

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
                    if (_context.Medicines.Any())
                        editMedicine.Id = _context.Medicines.Max(m => m.Id) + 1;
                    else
                        editMedicine.Id = 1;
                    
                }
                TimeSpan intakeTime = TimeSpan.Parse(BoxIntakeTime.Text);

                int newId;
                if (_context.MedicinesSchedule.Any())
                {
                    newId = _context.MedicinesSchedule.Max(ms => ms.Id) + 1;
                }
                else
                {
                    newId = 1;
                }
                MedicineSchedule schedule = new()
                {
                    Id = newId,
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
                _context.MedicinesSchedule.Add(schedule);
                _context.SaveChanges();
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
