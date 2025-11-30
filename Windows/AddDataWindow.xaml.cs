using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
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
        private Models.AppContext _context;
        private List<SymptomItem> symptomItems;
        public AddDataWindow(User user, Models.AppContext appContext)
        {
            currentUser = user;
            _context = appContext;
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
            List<Symptom> allSymptoms = _context.Symptoms.ToList();
            symptomItems = new List<SymptomItem>();
            foreach (Symptom? symptom in allSymptoms)
            {
                SymptomItem? item = new SymptomItem();
                item.Id = symptom.Id;
                item.Name = symptom.Name;
                item.IsSelected = false;
                symptomItems.Add(item);
            }
            SymptomsItems.ItemsSource = symptomItems;
            DateTime selectedDate = DPDate.SelectedDate ?? DateTime.Today;
            dailyData = _context.DailyData?.FirstOrDefault(d => d.UserId == currentUser.Id && d.Date.Date == selectedDate.Date);
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
                if (!string.IsNullOrEmpty(dailyData.Symptoms))
                {
                    var selectedSymptomIds = dailyData.Symptoms.Split(',')
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(int.Parse)
                        .ToList();

                    foreach (var item in symptomItems)
                    {
                        item.IsSelected = selectedSymptomIds.Contains(item.Id);
                    }
                    SymptomsItems.Items.Refresh();
                }
            }
            else
            {
                dailyData = new DailyData();
                BoxSleepStart.Text = "22:00";
                BoxSleepEnd.Text = "06:00";
                BoxNightAwakening.Text = "0";
                BoxSteps.Text = "0";
            }
        }

        public bool CheckData()
        {
            if(!string.IsNullOrEmpty(BoxSystolic.Text) && !string.IsNullOrEmpty(BoxDiastolic.Text) && !string.IsNullOrEmpty(BoxPulse.Text))
            {
                try 
                {
                    int systolic = int.Parse(BoxSystolic.Text);
                    int diastolic = int.Parse(BoxDiastolic.Text);
                    int pulse = int.Parse(BoxPulse.Text);
                    if (systolic < 60 || systolic > 250)
                    {
                        MessageBox.Show("Систолическое давление должно быть между 60 и 250");
                        return false;
                    }
                    if (diastolic < 40 || diastolic > 150)
                    {
                        MessageBox.Show("Диастолическое давление должно быть между 40 и 150");
                        return false;
                    }
                    if (systolic <= diastolic)
                    {
                        MessageBox.Show("Систолическое давление должно быть больше диастолического");
                        return false;
                    }
                    if (pulse < 30 || pulse > 200)
                    {
                        MessageBox.Show("Пульс должен быть между 30 и 200");
                        return false;
                    }

                }
                catch (FormatException)
                {
                    MessageBox.Show("Давление и пульс должны быть целыми числами");
                    return false;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Превышено допустимое значение!");
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(BoxWeight.Text))
            {
                try
                {
                    double weight = double.Parse(BoxWeight.Text);
                    if (weight < 20 ||  weight > 300)
                    {
                        MessageBox.Show("Вес должен быть между 20 и 300 кг");
                        return false;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Вес должен быть числом");
                    return false;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Превышено допустимое значение!");
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(BoxSleepStart.Text) && !string.IsNullOrEmpty(BoxSleepEnd.Text))
            {
                try
                {
                    TimeSpan sleepStartTime = TimeSpan.Parse(BoxSleepStart.Text);
                    TimeSpan sleepEndTime = TimeSpan.Parse(BoxSleepEnd.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Время сна должно быть указано в формате чч:мм");
                    return false;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Превышено допустимое значение!");
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(BoxNightAwakening.Text))
            {
                try
                {
                    int awake = int.Parse(BoxNightAwakening.Text);
                    if (awake < 0 || awake > 20)
                    {
                        MessageBox.Show("Количество ночных пробуждений не может быть отрицательным или превышать значение 20");
                        return false;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Количество ночных пробуждений должно быть целым числом");
                    return false;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Превышено допустимое значение!");
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(BoxSteps.Text))
            {
                try
                {
                    int steps = int.Parse(BoxSteps.Text);
                    if (steps < 0 || steps > 200000)
                    {
                        MessageBox.Show("Количество шагов не может быть отрицательным или превышать значение 200000");
                        return false;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Количество шагов должно быть целым числом");
                    return false;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("Превышено допустимое значение!");
                    return false;
                }
            }
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckData() == false)
                return;
            try
            {
                dailyData.UserId = currentUser.Id;
                dailyData.Date = DPDate.SelectedDate ?? DateTime.Today;
                dailyData.SystolicPressure = Convert.ToInt32(BoxSystolic.Text);
                dailyData.DiastolicPressure = Convert.ToInt32(BoxDiastolic.Text);
                dailyData.Pulse = Convert.ToInt32(BoxPulse.Text);
                dailyData.Weight = Convert.ToDouble(BoxWeight.Text);
                dailyData.WellBeingRating = ComboBoxWellBeing.SelectedIndex + 1;
                dailyData.Notes = BoxNotes.Text;
                List<int> selectedSymptomIds = symptomItems.Where(s => s.IsSelected).Select(s => s.Id).ToList();
                if (selectedSymptomIds.Count > 0)
                {
                    dailyData.Symptoms = string.Join(",", selectedSymptomIds);
                }

                else
                    dailyData.Symptoms = null;
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
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Неверный формат даты! Введите время сна в формате чч:мм (22:00)");
                        return;
                    }

                }
                dailyData.NightAwake = Convert.ToInt32(BoxNightAwakening.Text);
                dailyData.StepCount = Convert.ToInt32(BoxSteps.Text);
                dailyData.UpdateAt = DateTime.Now;
                if (dailyData.Id == 0)
                {
                    dailyData.CreateAt = DateTime.Now;
                    _context.DailyData.Add(dailyData);
                }

                _context.SaveChanges();
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
            if (!DPDate.SelectedDate.HasValue)
                return;
            if (DPDate.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Нельзя вносить данные за следующие дни");
                DPDate.SelectedDate = DateTime.Today;
                return;
            }
            LoadThisDateData();
            if (DPDate.SelectedDate < DateTime.Today && (dailyData == null || dailyData.Id == 0))
            {
                MessageBox.Show("Запрещено создавать новые данные за предыдущие дни. Можно редактировать только существующие записи");
                DPDate.SelectedDate = DateTime.Today;
                LoadThisDateData();
                return;
            }

        }
    }
}
