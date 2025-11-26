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
        private Models.AppContext _context;
        public RegWindow(Models.AppContext appContext)
        {
            _context = appContext;
            InitializeComponent();
            this.DataContext = newUser;
        }
        public bool CheckData()
        {
            if (string.IsNullOrWhiteSpace(newUser.Name))
            {
                MessageBox.Show("Имя не может быть пустым");
                return false;
            }
            if (string.IsNullOrWhiteSpace(newUser.Login))
            {
                MessageBox.Show("Логин пользователя не может быть пустым");
                return false;
            }
            if (_context.Users.Any(u => u.Login == newUser.Login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return false;
            }
            if (string.IsNullOrWhiteSpace(BoxPassword.Password))
            {
                MessageBox.Show("Пароль пользователя не может быть пустым");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(BoxHeight.Text))
            {
                try
                {
                    int height = int.Parse(BoxHeight.Text);
                    if (height < 60 || height > 250)
                    {
                        MessageBox.Show("Введите корректное значение роста (от 60 до 250 см)");
                        return false;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Рост должен быть целым числом");
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(BoxWeight.Text))
            {
                try
                {
                    double weight = double.Parse(BoxWeight.Text);
                    if (weight < 20 || weight > 300)
                    {
                        MessageBox.Show("Введите корректное значение веса (от 20 до 300 кг)");
                        return false;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Вес должен быть числом");
                    return false;
                }
            }
            if (DPBirthDate.SelectedDate.HasValue)
            {
                if (DPBirthDate.SelectedDate.Value > DateTime.Today)
                {
                    MessageBox.Show($"Вы что, из будущего? Дата рождения не может быть позже {DateTime.Today}");
                    return false;
                }
                if (DateTime.Today.Year - DPBirthDate.SelectedDate.Value.Year > 125)
                {
                    MessageBox.Show($"Проверьте введенные данные. Дата рождения пользователя не может быть больше {DateTime.Now.AddYears(-125)}");
                    return false;
                }
            }
            return true;
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
            if (CheckData() == false)
                return;
            try
            {
                if (_context.Users.Any())
                    newUser.Id = _context.Users.Max(u => u.Id) + 1;
                else
                    newUser.Id = 1;
                if (DPBirthDate.SelectedDate.HasValue)
                    newUser.DateOfBirth = DateOnly.FromDateTime(DPBirthDate.SelectedDate.Value);
                newUser.Password = BoxPassword.Password;
                newUser.Height = int.Parse(BoxHeight.Text);
                newUser.Weight = int.Parse(BoxWeight.Text);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                MessageBox.Show("Регистрация завершена!");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла ошибка при регистрации: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
