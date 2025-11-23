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
using WellBeingDiary.UserControllers;

namespace WellBeingDiary.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainPageWindow.xaml
    /// </summary>
    public partial class MainPageWindow : Window
    {
        public User currentUser { get; set; }
        public MainPageWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            this.DataContext = this;
            ShowMainPage();
        }
        public void ShowMainPage()
        {
           MainContent.Content = new MainPageContent(currentUser);
        }

        private void ViewProfileButton_Click(object sender, RoutedEventArgs e)
        {
           MainContent.Content = new ProfilePage(currentUser);
        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMainPage();
        }

        private void AddDataButton_Click(object sender, RoutedEventArgs e)
        {
            //Window addDataWindow = new AddDataWindow(currentUser);
            //if (addDataWindow.ShowDialog() == true)
            //    ShowMainPage();
        }

        private void MedControlButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MedicineControlPage(currentUser);
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            //Window reportWindow = new ReportMakeWindow(currentUser);
            //reportWindow.ShowDialog();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
