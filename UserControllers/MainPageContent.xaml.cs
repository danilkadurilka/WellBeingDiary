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
        private DailyData todayData;
        private DateTime currentDate = DateTime.Today;
        public MainPageContent(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadData();
        }
        public void LoadData()
        {
            todayData = Models.AppContext.DailyData?.FirstOrDefault(d => d.UserId == currentUser.Id && d.Date.Date == currentDate);
            if (todayData == null)
                todayData = new DailyData();
            UpdateDataDisplay();
            LoadMedicines();
        }
        public void UpdateDataDisplay()
        {
            if (todayData.SystolicPressure.HasValue && todayData.DiastolicPressure.HasValue)
            {
                string pulse = string.Empty;
                if (todayData.Pulse.HasValue)
                {
                    pulse = $"{todayData.Pulse}";
                }
                BoxBloodPressure.Text = $"{todayData.SystolicPressure}/{todayData.DiastolicPressure}/{pulse}";
            }
            else { BoxBloodPressure.Text = "Не указано"; }
                

            if (todayData.Weight.HasValue)
            {
                BoxWeight.Text = $"{todayData.Weight} кг";
            }
            else { BoxWeight.Text = "Не указан"; }
                

            if (todayData.WellBeingRating.HasValue)
            {
                int? rating = todayData.WellBeingRating.Value;
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


            if (todayData.SleepStart.HasValue && todayData.SleepEnd.HasValue)
            {
                TimeSpan sleepTime = todayData.SleepEnd.Value - todayData.SleepStart.Value;
                string quality = string.Empty;
                if (todayData.SleepQuality.HasValue)
                {
                    quality = $", качество сна: {todayData.SleepQuality}/5";
                }
                BoxSleep.Text = $"{sleepTime.Hours}ч {sleepTime.Minutes}м{quality}";
            }
            else { BoxWellBeing.Text = "Не указан"; }

            if (todayData.StepCount.HasValue)
            { 
                BoxSteps.Text = $"{todayData.StepCount} шагов";
            }
            else { BoxSteps.Text = "Не указано"; }
        }
        public void LoadMedicines()
        {
            var medicines = Models.AppContext.Medicines?.Where(m => m.UserId == currentUser.Id && m.IsActive).ToList(); //var - это DbSet? List? 

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
