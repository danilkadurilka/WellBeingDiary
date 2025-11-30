using Microsoft.EntityFrameworkCore;
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
            currentUser = user;
            _context = appContext;
            InitializeComponent();
            
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
                BoxIntakeTime.Text = "08:00";
                BoxMonday.IsChecked = true;
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
                if(DPEndDate.SelectedDate.Value < DPStartDate.SelectedDate.Value)
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
            if (!(BoxMonday.IsChecked == true || BoxTuesday.IsChecked == true || BoxWednesday.IsChecked == true || BoxThursday.IsChecked == true ||
                BoxFriday.IsChecked == true || BoxSaturday.IsChecked == true || BoxSunday.IsChecked == true))
            {
                MessageBox.Show("Выберите хотя бы один день для приема");
                return false;
            }
            return true;
        }
        public bool ShouldTakeOnDay(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Monday) return BoxMonday.IsChecked == true;
            if (dayOfWeek == DayOfWeek.Tuesday) return BoxTuesday.IsChecked == true;
            if (dayOfWeek == DayOfWeek.Wednesday) return BoxWednesday.IsChecked == true;
            if (dayOfWeek == DayOfWeek.Thursday) return BoxThursday.IsChecked == true;
            if (dayOfWeek == DayOfWeek.Friday) return BoxFriday.IsChecked == true;
            if (dayOfWeek == DayOfWeek.Saturday) return BoxSaturday.IsChecked == true;
            if (dayOfWeek == DayOfWeek.Sunday) return BoxSunday.IsChecked == true;
            return false;
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
            BoxMonday.IsChecked = editMedicine.Monday;
            BoxTuesday.IsChecked = editMedicine.Tuesday;
            BoxWednesday.IsChecked = editMedicine.Wednesday;
            BoxThursday.IsChecked = editMedicine.Thursday;
            BoxFriday.IsChecked = editMedicine.Friday;
            BoxSaturday.IsChecked = editMedicine.Saturday;
            BoxSunday.IsChecked = editMedicine.Sunday;
            BoxIntakeTime.Text = editMedicine.IntakeTime.ToString(@"hh\:mm");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckMedicineData() == false)
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
                editMedicine.IntakeTime = TimeSpan.Parse(BoxIntakeTime.Text);
                editMedicine.Monday = BoxMonday.IsChecked ?? false;
                editMedicine.Tuesday = BoxTuesday.IsChecked ?? false;
                editMedicine.Wednesday = BoxWednesday.IsChecked ?? false;
                editMedicine.Thursday = BoxThursday.IsChecked ?? false;
                editMedicine.Friday = BoxFriday.IsChecked ?? false;
                editMedicine.Saturday = BoxSaturday.IsChecked ?? false;
                editMedicine.Sunday = BoxSunday.IsChecked ?? false;
                editMedicine.UpdateAt = DateTime.Now;
                if (!isEditing)
                {
                    _context.Medicines.Add(editMedicine);
                }
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
