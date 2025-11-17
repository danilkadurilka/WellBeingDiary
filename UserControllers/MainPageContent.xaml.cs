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
            
        }
        public void LoadMedicines()
        {

        }

    }
}
