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
        private void btnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new System.Uri(openFileDialog.FileName));
                imgUserPhoto.Source = bitmap;
                newUser.PhotoPath = openFileDialog.FileName;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrEmpty(newUser.Name) || string.IsNullOrEmpty(newUser.Login))
            {
                MessageBox.Show("Заполните обязательные поля");
                return;
            }

            // Установка ID и добавление пользователя
            newUser.Id = Models.AppContext.Users.Count > 0 ? Models.AppContext.Users.Max(u => u.Id) + 1 : 1;

            // Преобразование даты рождения
            if (dpBirthDate.SelectedDate.HasValue)
            {
                newUser.DateOfBirth = DateOnly.FromDateTime(dpBirthDate.SelectedDate.Value);
            }

            // Установка пароля
            var passwordBox = txtPassword;
            newUser.Password = passwordBox.Password;

            Models.AppContext.Users.Add(newUser);

            MessageBox.Show("Регистрация завершена!");

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
