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
        private Models.AppContext _context;
        public MainPageWindow(User user, Models.AppContext appContext)
        {
            currentUser = user;
            _context = appContext;
            InitializeComponent();
            DataContext = this;
            ShowUserImage();
            ShowMainPage();
        }
        public void ShowUserImage()
        {
            if (!string.IsNullOrEmpty(currentUser.PhotoPath))
            {
                try
                {
                    string path = currentUser.PhotoPath;
                    if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
                    {
                        path = "Images/none.png";
                    }
                    if (System.IO.File.Exists(path))
                    {
                        BitmapImage bitmap = new();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(path), UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        UserImage.Source = bitmap;
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка инициализации изображения");
                }
            }
        }
        public void ShowMainPage()
        {
           MainContent.Content = new MainPageContent(currentUser, _context);
        }

        private void ViewProfileButton_Click(object sender, RoutedEventArgs e)
        {
           MainContent.Content = new ProfilePage(currentUser, _context);
        }

        private void MainPageButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMainPage();
        }

        private void AddDataButton_Click(object sender, RoutedEventArgs e)
        {
            Window addDataWindow = new AddDataWindow(currentUser, _context);
            if (addDataWindow.ShowDialog() == true)
                ShowMainPage();
        }

        private void MedControlButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MedicineControlPage(currentUser, _context);
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            Window reportWindow = new ReportMakeWindow(currentUser, _context);
            reportWindow.ShowDialog();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
