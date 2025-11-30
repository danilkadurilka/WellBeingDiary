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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WellBeingDiary.Models;

namespace WellBeingDiary.UserControllers
{
    /// <summary>
    /// Логика взаимодействия для MainPageContent.xaml
    /// </summary>
    public partial class MainPageContent : UserControl
    {
        private User currentUser;
        private DailyData selectedData;
        private DateTime currentDate = DateTime.Today;
        private Models.AppContext _context;
        public MainPageContent(User user, Models.AppContext appContext)
        {
            currentUser = user;
            _context = appContext;
            InitializeComponent();
            DatePickerControl.SelectedDate = currentDate;
            LoadData(); 
        }
        private void DatePickerControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePickerControl.SelectedDate.HasValue)
            { 
                currentDate = DatePickerControl.SelectedDate.Value;
                LoadData();
            }
        }
        public void LoadData()
        {
            selectedData = _context.DailyData?.FirstOrDefault(d => d.UserId == currentUser.Id && d.Date.Date == currentDate.Date);
            if (selectedData == null)
                selectedData = new DailyData();
            UpdateDataDisplay();
            LoadMedicines();
        }
        public void UpdateDataDisplay()
        {

            if (selectedData.SystolicPressure.HasValue && selectedData.DiastolicPressure.HasValue)
            {
                string pulse = string.Empty;
                if (selectedData.Pulse.HasValue)
                {
                    pulse = $"{selectedData.Pulse}";
                }
                BoxBloodPressure.Text = $"{selectedData.SystolicPressure}/{selectedData.DiastolicPressure}/{pulse}";
            }
            else { BoxBloodPressure.Text = "Не указано"; }
                

            if (selectedData.Weight.HasValue)
            {
                BoxWeight.Text = $"{selectedData.Weight} кг";
            }
            else { BoxWeight.Text = "Не указан"; }
                

            if (selectedData.WellBeingRating.HasValue)
            {
                int? rating = selectedData.WellBeingRating.Value;
                string ratingText;

                if (rating == 1)
                {
                    ratingText = "Плохое";
                }
                else if (rating == 2)
                {
                    ratingText = "Удовлетворительное";
                }
                else if (rating == 3)
                {
                    ratingText = "Нормальное";
                }
                else if (rating == 4)
                {
                    ratingText = "Хорошее";
                }
                else if (rating == 5)
                {
                    ratingText = "Отличное";
                }
                else
                {
                    ratingText = "Не указано";
                }

                BoxWellBeing.Text = $"{ratingText} ({rating}/5)";
            }
            else { BoxWellBeing.Text = "Не указано"; }
            if (!string.IsNullOrEmpty(selectedData.Notes))
                BoxNotes.Text = selectedData.Notes;
            else
                BoxNotes.Text = "Заметок нет.\nМожете заполнить :)";
            if (!string.IsNullOrEmpty(selectedData.Symptoms))
            {
                try
                {
                    List<int> symptomIds = selectedData.Symptoms.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
                    List<string> symptomNames = _context.Symptoms.Where(s => symptomIds.Contains(s.Id)).Select(s => s.Name).ToList();

                    if (symptomNames.Count > 0)
                    {
                        BoxSymptoms.Text = string.Join("\n", symptomNames);
                    }
                    else
                    {
                        BoxSymptoms.Text = "Нет симптомов";
                    }
                }
                catch (Exception ex)
                {
                    BoxSymptoms.Text = "Ошибка загрузки симптомов";
                    Console.WriteLine($"Ошибка при загрузке симптомов: {ex.Message}");
                }
            }
            else
            {
                BoxSymptoms.Text = "Нет симптомов";
            }


            if (selectedData.SleepStart.HasValue && selectedData.SleepEnd.HasValue)
            {
                TimeSpan sleepTime = selectedData.SleepEnd.Value - selectedData.SleepStart.Value;
                string quality = string.Empty;
                if (selectedData.SleepQuality.HasValue)
                {
                    quality = $", качество сна: {selectedData.SleepQuality}/5";
                }
                BoxSleep.Text = $"{sleepTime.Hours}ч {sleepTime.Minutes}м{quality}";
            }
            else { BoxSleep.Text = "Не указан"; }

            if (selectedData.StepCount.HasValue)
            { 
                BoxSteps.Text = $"{selectedData.StepCount} шагов";
            }
            else { BoxSteps.Text = "Не указано"; }
        }
        public void LoadMedicines()
        {
            List<Medicine>? medicines = _context.Medicines?.Where(m => m.UserId == currentUser.Id && m.IsActive && (m.StartDate.Date <= currentDate.Date) 
                                        && (m.EndDate.Value.Date >= currentDate.Date)).ToList();

            if (medicines != null && medicines.Any())
            {
                ItemsMedicines.ItemsSource = medicines;
                TextNoMedicines.Visibility = Visibility.Collapsed;
            }
            else
            {
                ItemsMedicines.ItemsSource = null;
                TextNoMedicines.Visibility = Visibility.Visible;
            }
        }

        
    }
}
