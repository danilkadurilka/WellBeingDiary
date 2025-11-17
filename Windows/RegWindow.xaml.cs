using Microsoft.Win32;
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
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        private User newUser = new User();
        public RegWindow()
        {
            InitializeComponent();
            this.DataContext = newUser;
        }
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new System.Uri(openFileDialog.FileName));
                ImgUserPhoto.Source = bitmap;
                newUser.PhotoPath = openFileDialog.FileName;
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(newUser.Name) || string.IsNullOrEmpty(newUser.Login))
            {
                MessageBox.Show("Заполните обязательные поля");
                return;
            }

            newUser.Id = Models.AppContext.Users.Count > 0 ? Models.AppContext.Users.Max(u => u.Id) + 1 : 1;

            if (DPBirthDate.SelectedDate.HasValue)
            {
                newUser.DateOfBirth = DateOnly.FromDateTime(DPBirthDate.SelectedDate.Value);
            }

            var passwordBox = BoxPassword;
            newUser.Password = passwordBox.Password;

            Models.AppContext.Users.Add(newUser);

            MessageBox.Show("Регистрация завершена!");

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
