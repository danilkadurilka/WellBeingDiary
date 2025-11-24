using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
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
    /// Логика взаимодействия для AddDataWindow.xaml
    /// </summary>
    public partial class AddDataWindow : Window
    {
        private User currentUser;
        private DailyData dailyData;
        public AddDataWindow(User user)
        {
            currentUser = user;
            InitializeComponent();
            DPDate.SelectedDate = DateTime.Today;
            LoadThisDateData();
        }

        public void LoadThisDateData()
        {
            if (currentUser == null)
            {
                MessageBox.Show("Ошибка! Данные пользователя не загрузились");
                return;
            }
            DateTime selectedDate = DPDate.SelectedDate ?? DateTime.Today;
            dailyData = Models.AppContext.DailyData?.
                FirstOrDefault(d => d.UserId == currentUser.Id && d.Date.Date == selectedDate.Date);
            if (dailyData != null)
            {
                BoxSystolic.Text = dailyData.SystolicPressure?.ToString();
                BoxDiastolic.Text = dailyData.DiastolicPressure?.ToString();
                BoxPulse.Text = dailyData.Pulse?.ToString();
                BoxWeight.Text = dailyData.Weight?.ToString();
                if (dailyData.WellBeingRating.HasValue)
                    ComboBoxWellBeing.SelectedIndex = dailyData.WellBeingRating.Value - 1;
                BoxNotes.Text = dailyData.Notes;
                if (dailyData.SleepStart.HasValue)
                    BoxSleepStart.Text = dailyData.SleepStart.Value.ToString("HH:mm");
                if (dailyData.SleepEnd.HasValue)
                    BoxSleepEnd.Text = dailyData.SleepEnd.Value.ToString("HH:mm");
                if (dailyData.SleepQuality.HasValue)
                    ComboBoxSleepQuality.SelectedIndex = dailyData.SleepQuality.Value - 1;
                BoxNightAwakening.Text = dailyData.NightAwake?.ToString();
                BoxSteps.Text = dailyData.StepCount?.ToString();

            }
            else
            {
                dailyData = new DailyData();
                BoxSleepStart.Text = "22:00";
                BoxSleepEnd.Text = "06:00";
                BoxNightAwakening.Text = "0";
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dailyData.UserId = currentUser.Id;
                dailyData.Date = DPDate.SelectedDate ?? DateTime.Today;
                dailyData.SystolicPressure = Convert.ToInt32(BoxSystolic.Text);
                dailyData.DiastolicPressure = Convert.ToInt32(BoxDiastolic.Text);
                dailyData.Pulse = Convert.ToInt32(BoxPulse.Text);
                dailyData.Weight = Convert.ToInt32(BoxWeight.Text);
                dailyData.WellBeingRating = ComboBoxWellBeing.SelectedIndex + 1;
                dailyData.Notes = BoxNotes.Text;
                if (!string.IsNullOrEmpty(BoxSleepStart.Text) && !string.IsNullOrEmpty(BoxSleepEnd.Text)) 
                {
                    try
                    {
                        TimeSpan sleepStartTime = TimeSpan.Parse(BoxSleepStart.Text);
                        TimeSpan sleepEndTime = TimeSpan.Parse(BoxSleepEnd.Text);
                        DateTime selectedDate = DPDate.SelectedDate ?? DateTime.Today;
                        DateTime sleepStart = selectedDate.Date + sleepStartTime;
                        DateTime sleepEnd = selectedDate.Date + sleepEndTime;
                        if (sleepEnd <= sleepStart)
                            sleepEnd = sleepEnd.AddDays(1);
                        dailyData.SleepStart = sleepStart;
                        dailyData.SleepEnd = sleepEnd;
                        dailyData.SleepQuality = ComboBoxSleepQuality.SelectedIndex + 1;
                        dailyData.NightAwake = Convert.ToInt32(BoxNightAwakening.Text);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Неверный формат даты! Введите время сна в формате чч:мм (22:00)");
                        return;
                    }

                }
                dailyData.StepCount = Convert.ToInt32(BoxSteps.Text);
                dailyData.UpdateAt = DateTime.Now;
                dailyData.CreateAt = DateTime.Now;
                Models.AppContext.DailyData?.Add(dailyData);
                MessageBox.Show("Данные успешно внесены!");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При внесении данных возникла ошибка: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult=false;
            this.Close();
        }

        private void DPDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadThisDateData();
        }
    }
}
