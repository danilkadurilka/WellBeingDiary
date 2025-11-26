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
using LiveCharts;
using LiveCharts.Wpf;
using WellBeingDiary.Models;

namespace WellBeingDiary.Windows
{
    /// <summary>
    /// Логика взаимодействия для ReportMakeWindow.xaml
    /// </summary>
    public partial class ReportMakeWindow : Window
    {
        private User currentUser;
        private Models.AppContext _context;
        public SeriesCollection ReportCollection { get; set; }
        public List<string> Labels { get; set; }

        public ReportMakeWindow(User user, Models.AppContext appContext)
        {
            currentUser = user;
            _context = appContext;
            InitializeComponent();
            DPStartDate.SelectedDate = DateTime.Today.AddDays(-30);
            DPEndDate.SelectedDate = DateTime.Today;
            ComboBoxReportType.SelectedIndex = 0;
            ReportCollection = new SeriesCollection();
            Labels = new List<string>();
            ConfigureAxes();
            DataContext = this;
        }

        public void ConfigureAxes()
        {
            ReportChart.AxisX.Clear();
            ReportChart.AxisX.Add(new Axis { Title = "Дата", Labels = Labels });
            ReportChart.AxisY.Clear();
            ReportChart.AxisY.Add(new Axis { Title = "Значения" });
        }


        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DPStartDate.SelectedDate.HasValue || !DPEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период для формирования отчета!");
                return;
            }
            DateTime startDate = DPStartDate.SelectedDate.Value;
            DateTime endDate = DPEndDate.SelectedDate.Value;
            if (startDate > endDate)
            {
                MessageBox.Show("Начальная дата не может быть больше конечной!");
                return;
            }
            if ((endDate-startDate).TotalDays > 365)
            {
                MessageBox.Show("Период отчета не может превышать 1 год!");
                return;
            }
            List<DailyData>? reportData = _context.DailyData?.Where(d => d.UserId == currentUser.Id && d.Date >= startDate && d.Date <= endDate).OrderBy(d => d.Date).ToList();

            if (reportData == null || reportData.Count == 0)
            {
                BoxReport.Text = "Нет данных за выбранный период.";
                return;
            }
            string reportType = (ComboBoxReportType.SelectedItem as ComboBoxItem)?.Content.ToString();
            string reportText = GenerateReportText(reportType, reportData, startDate, endDate);
            BoxReport.Text = reportText;
            GenerateReportChart(reportType, reportData);
        }

        public string GenerateReportText(string reportType, List<DailyData> data, DateTime startDate, DateTime endDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Тип отчета: {reportType}");
            sb.AppendLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
            sb.AppendLine($"Пользователь: {currentUser.Name}");
            sb.AppendLine();

            if (reportType == "Отчет по самочувствию")
            {
                WellBeingReport(sb, data);
            }
            else if (reportType == "Отчет по давлению")
            {
                BloodPressureReport(sb, data);
            }
            else if (reportType == "Отчет по сну")
            {
                SleepReport(sb, data);
            }
            else if (reportType == "Отчет по двигательной активности")
            {
                ActivityReport(sb, data);
            }
            else if (reportType == "Отчет по весу")
            {
                WeightReport(sb, data);
            }
            sb.AppendLine();
            sb.AppendLine(ReportRecommendations(reportType, data));
            return sb.ToString();
        }

        public void WellBeingReport(StringBuilder sb, List<DailyData> data)
        {
            List<DailyData> validData = data.Where(d => d.WellBeingRating.HasValue).ToList();
            if (validData.Count == 0)
            {
                MessageBox.Show("Нет данных о самочувствии за период");
                return;
            }
            double avgRating = validData.Average(d => d.WellBeingRating.Value);
            sb.AppendLine($"Средняя оценка самочувствия: {avgRating:f1}/5");
            sb.AppendLine();
            sb.AppendLine("Динамика изменений:");
            foreach (DailyData day in validData)
            {
                string rateText = WellBeingText(day.WellBeingRating.Value);
                sb.AppendLine($"{day.Date:dd.MM.yyyy}: {rateText} ({day.WellBeingRating}/5)");
            }
        }

        public void BloodPressureReport(StringBuilder sb, List<DailyData> data)
        {
            List<DailyData> validData = data.Where(d => d.SystolicPressure.HasValue && d.DiastolicPressure.HasValue && d.Pulse.HasValue).ToList();
            if (validData.Count == 0)
            {
                MessageBox.Show("Нет данных о давлении за период");
                return;
            }
            double avgSystolic = validData.Average(d => d.SystolicPressure.Value);
            double avgDiastolic = validData.Average(d => d.DiastolicPressure.Value);
            double avgPulse = validData.Average(d => d.Pulse.Value);
            sb.AppendLine($"Среднее давление и пульс: {avgSystolic:f0}/{avgDiastolic:f0}/{avgPulse:f0}");
            sb.AppendLine();
            List<DailyData> anomalies = validData.Where(d => d.SystolicPressure > 140 || d.DiastolicPressure > 90 || d.Pulse > 80 ||
            d.SystolicPressure < 90 || d.DiastolicPressure < 60 || d.Pulse < 60).ToList();

            if (anomalies.Count > 0)
            {
                sb.AppendLine("Выявлены нарушения!");
                foreach (DailyData anomaly in anomalies)
                {
                    sb.AppendLine($"{anomaly.Date:dd.MM.yyyy}: {anomaly.SystolicPressure}/{anomaly.DiastolicPressure}/{anomaly.Pulse}");
                }
                sb.AppendLine();
            }
            sb.AppendLine("Динамика изменений:");
            foreach (DailyData? day in validData)
            {
                sb.AppendLine($"{day.Date:dd.MM.yyyy}: {day.SystolicPressure}/{day.DiastolicPressure}/{day.Pulse}");
            }
        }

        public void SleepReport(StringBuilder sb, List<DailyData> data)
        {
            List<DailyData> validData = data.Where(d => d.SleepStart.HasValue && d.SleepEnd.HasValue).ToList();
            if (validData.Count == 0)
            {
                MessageBox.Show("Нет данных о сне за период");
                return;
            }
            double avgSleepHours = validData.Average(d => (d.SleepEnd.Value - d.SleepStart.Value).TotalHours);
            double avgSleepQuality = validData.Where(d => d.SleepQuality.HasValue).Average(d => d.SleepQuality.Value);
            sb.AppendLine($"Средняя продолжительность сна: {avgSleepHours:f1} ч");
            sb.AppendLine($"Среднее качество сна: {avgSleepQuality:f1}/5");
            sb.AppendLine();
            sb.AppendLine("Динамика изменений:");
            foreach (DailyData day in validData)
            {
                TimeSpan sleepTime = day.SleepEnd.Value - day.SleepStart.Value;
                string quality = $", качество сна: {day.SleepQuality}/5";
                sb.AppendLine($"{day.Date:dd.MM.yyyy}: {sleepTime.Hours}ч {sleepTime.Minutes}м{quality}");
            }

        }

        public void ActivityReport(StringBuilder sb, List<DailyData> data)
        {
            List<DailyData> validData = data.Where(d => d.StepCount.HasValue).ToList();
            if (validData.Count == 0)
            {
                MessageBox.Show("Нет данных о двигательной активности за период");
                return;
            }
            double avgSteps = validData.Average(d => d.StepCount.Value);
            int totalSteps = validData.Sum(d => d.StepCount.Value);
            sb.AppendLine($"Среднее количество шагов: {avgSteps:f0}");
            sb.AppendLine($"Общее количество шагов: {totalSteps}");
            sb.AppendLine();
            sb.AppendLine("Динамика изменений:");
            foreach (DailyData day in validData)
            {
                sb.AppendLine($"{day.Date:dd.MM.yyyy}: {day.StepCount} шагов");
            }
        }

        public void WeightReport(StringBuilder sb, List<DailyData> data)
        {
            List<DailyData> validData = data.Where(d => d.Weight.HasValue).ToList();
            if (validData.Count == 0)
            {
                MessageBox.Show("Нет данных о весе за период");
                return;
            }
            double firstWeight = validData.OrderBy(d => d.Date).First().Weight.Value;
            double lastWeight = validData.OrderBy(d => d.Date).Last().Weight.Value;
            double weightChange = lastWeight - firstWeight;
            double days = (validData.Max(d => d.Date) - validData.Min(d => d.Date)).TotalDays;
            double changeInDay;
            if (days > 0)
                changeInDay = weightChange / days;
            else
                changeInDay = 0;
            sb.AppendLine($"Начальное значение: {firstWeight} кг");
            sb.AppendLine($"Конечное значение: {lastWeight} кг");
            sb.AppendLine($"{weightChange:+#;-#;0}");
            sb.AppendLine($"Темп изменения: {changeInDay:f2} кг в день");
            sb.AppendLine();
            sb.AppendLine("Динамика изменений:");
            foreach (DailyData day in validData)
            {
                sb.AppendLine($"{day.Date:dd.MM.yyyy}: {day.Weight} кг");
            }

        }

        public string ReportRecommendations(string reportType, List<DailyData> data)
        {
            List<string> recommendations = new();
            if (reportType == "Отчет по самочувствию")
            {
                List<DailyData> wellBeingData = data.Where(d => d.WellBeingRating.HasValue).ToList();
                if (wellBeingData.Any())
                {
                    double avgRating = wellBeingData.Average(d => d.WellBeingRating.Value);
                    if (avgRating < 3)
                        recommendations.Add("Рекомендуется отдых и снижение уровня стресса!");
                    if (avgRating < 2)
                        recommendations.Add("Стоит посетить врача");
                }
            }
            else if (reportType == "Отчет по давлению")
            {
                List<DailyData> pressureData = data.Where(d => d.SystolicPressure.HasValue && d.DiastolicPressure.HasValue && d.Pulse.HasValue).ToList();
                if (pressureData.Any())
                {
                    double avgSystolic = pressureData.Average(d => d.SystolicPressure.Value);
                    double avgDiastolic = pressureData.Average(d => d.DiastolicPressure.Value);
                    double avgPulse = pressureData.Average(d => d.Pulse.Value);
                    if (avgSystolic > 130 || avgDiastolic > 85)
                        recommendations.Add("Рекомендуется снизить потребление соли");
                    if (avgSystolic > 140 || avgDiastolic > 90)
                        recommendations.Add("Проконсультируйтесь с врачом о возможной гипертонии");
                    if (avgSystolic < 100 || avgDiastolic < 60)
                        recommendations.Add("Увеличьте потребление жидкости");
                }
            }
            else if (reportType == "Отчет по сну")
            {
                List<DailyData> sleepData = data.Where(d => d.SleepStart.HasValue && d.SleepEnd.HasValue).ToList();
                if (sleepData.Any())
                { 
                    double avgSleepHours = sleepData.Average(d => (d.SleepEnd.Value - d.SleepStart.Value).TotalHours);
                    if (avgSleepHours < 7)
                        recommendations.Add("Старайтесь спать не менее 7-8 часов в сутки");
                    if (avgSleepHours > 9)
                        recommendations.Add("Избыток сна может приводить к переутомлению");
                }
               
            }
            else if (reportType == "Отчет по двигательной активности")
            {
                List<DailyData> activityData = data.Where(d => d.StepCount.HasValue).ToList();
                if (activityData.Any())
                {
                    double avgSteps = activityData.Average(d => d.StepCount.Value);
                    if (avgSteps < 5000)
                        recommendations.Add("Старайтесь проходить не менее 5000 шагов в день");
                    if (avgSteps < 3000)
                        recommendations.Add("Рекомендуется добавить легкие физические нагрузки");
                    if (avgSteps > 10000)
                        recommendations.Add("Вы - молодец! Продолжайте держать активность на этом уровне");
                }
            }
            else if (reportType == "Отчет по весу")
            {
                List<DailyData> weightData = data.Where(d => d.Weight.HasValue).ToList();
                double firstWeight = weightData.OrderBy(d => d.Date).First().Weight.Value;
                double lastWeight = weightData.OrderBy(d => d.Date).Last().Weight.Value;
                double change = lastWeight - firstWeight;
                if (change < 0)
                    recommendations.Add($"Ваш вес уменьшился на {Math.Abs(change)} кг");
                else if (change > 2)
                    recommendations.Add($"Ваш вес увеличился на {change} кг");
            }
            if (recommendations.Any())
                return string.Join("\n", recommendations);
            else
                return "Рекомендаций нет. У вас все в порядке!";
        }

        public string WellBeingText(int rating)
        {

            if (rating == 1)
            {
                return "Плохое";
            }
            else if (rating == 2)
            {
                return "Удовлетворительное";
            }
            else if (rating == 3)
            {
                return "Нормальное";
            }
            else if (rating == 4)
            {
                return "Хорошее";
            }
            else if (rating == 5)
            {
                return "Отличное";
            }
            else
            {
                return "Не указано";
            }

        }

        public void GenerateReportChart(string reportType, List<DailyData> data)
        {
            ReportCollection.Clear();
            Labels.Clear();
            ConfigureAxes();
            if (reportType == "Отчет по самочувствию")
            {
                List<DailyData> wellBeingData = data.Where(d => d.WellBeingRating.HasValue).OrderBy(d => d.Date).ToList();
                if (wellBeingData.Any())
                {
                    Labels = wellBeingData.Select(d => d.Date.ToString("dd.MM")).ToList();
                    ReportCollection.Add(new LineSeries
                    {
                        Title = "Самочувствие",
                        Values = new ChartValues<int>(wellBeingData.Select(d => d.WellBeingRating.Value)),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });
                }
            }
            else if (reportType == "Отчет по давлению")
            {
                List<DailyData> pressureData = data.Where(d => d.SystolicPressure.HasValue && d.DiastolicPressure.HasValue && d.Pulse.HasValue).OrderBy(d => d.Date).ToList();
                if (pressureData.Any())
                {
                    Labels = pressureData.Select(d => d.Date.ToString("dd.MM")).ToList();
                    ReportCollection.Add(new LineSeries
                    {
                        Title = "Систолическое",
                        Values = new ChartValues<int>(pressureData.Select(d => d.SystolicPressure.Value)),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });
                    ReportCollection.Add(new LineSeries
                    {
                        Title = "Диастолическое",
                        Values = new ChartValues<int>(pressureData.Select(d => d.DiastolicPressure.Value)),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });
                    ReportCollection.Add(new LineSeries
                    {
                        Title = "Пульс",
                        Values = new ChartValues<int>(pressureData.Select(d => d.Pulse.Value)),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });
                }
            }
            else if (reportType == "Отчет по сну")
            {
                List<DailyData> sleepData = data.Where(d => d.SleepStart.HasValue && d.SleepEnd.HasValue).OrderBy(d => d.Date).ToList();
                if (sleepData.Any())
                {
                    Labels = sleepData.Select(d => d.Date.ToString("dd.MM")).ToList();
                    ReportCollection.Add(new LineSeries
                    {
                        Title = "Часы сна",
                        Values = new ChartValues<double>(sleepData.Select(d => (d.SleepEnd.Value - d.SleepStart.Value).TotalHours)),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });
                }

            }
            else if (reportType == "Отчет по двигательной активности")
            {
                List<DailyData> activityData = data.Where(d => d.StepCount.HasValue).OrderBy(d => d.Date).ToList();
                if (activityData.Any())
                {
                    Labels = activityData.Select(d => d.Date.ToString("dd.MM")).ToList();
                    ReportCollection.Add(new ColumnSeries
                    {
                        Title = "Шаги",
                        Values = new ChartValues<int>(activityData.Select(d => d.StepCount.Value))
                    });
                }
            }
            else if (reportType == "Отчет по весу")
            {
                List<DailyData> weightData = data.Where(d => d.Weight.HasValue).OrderBy(d => d.Date).ToList();
                if (weightData.Any())
                {
                    Labels = weightData.Select(d => d.Date.ToString("dd.MM")).ToList();
                    ReportCollection.Add(new LineSeries
                    {
                        Title = "Вес",
                        Values = new ChartValues<double>(weightData.Select(d => d.Weight.Value)),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 8
                    });
                }
            }
            ReportChart.AxisX.Clear();
            ReportChart.AxisX.Add(new Axis
            {
                Title = "Дата",
                Labels = Labels
            });
            ReportChart.Update(true);
        }

        private void SaveReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(BoxReport.Text) || BoxReport.Text.StartsWith("Нет данных"))
            {
                MessageBox.Show("Сначала сгенерируйте отчет!");
                return;
            }
            int newId;
            if (_context.Reports.Any())
            {
                newId = _context.Reports.Max(r => r.Id) + 1;
            }
            else
            {
                newId = 1;
            }
            try
            {
                Report? report = new Report
                {
                    Id = newId,
                    UserId = currentUser.Id,
                    ReportType = (ComboBoxReportType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Общий отчет",
                    StartDate = DPStartDate.SelectedDate ?? DateTime.Today.AddDays(-7),
                    EndDate = DPEndDate.SelectedDate ?? DateTime.Today,
                    ReportData = BoxReport.Text,
                    CreateAt = DateTime.Now
                };

                _context.Reports.Add(report);
                _context.SaveChanges();
                MessageBox.Show("Отчет сохранен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении отчета: {ex.Message}");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBoxReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReportCollection?.Clear();
            Labels?.Clear();
            ConfigureAxes();
        }

    }
}
