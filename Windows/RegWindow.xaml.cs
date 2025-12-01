using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        private string selectedImagePath = string.Empty;
        public RegWindow(Models.AppContext appContext)
        {
            _context = appContext;
            InitializeComponent();
        }
        public bool CheckData()
        {
            if (string.IsNullOrWhiteSpace(BoxName.Text))
            {
                MessageBox.Show("Имя не может быть пустым");
                return false;
            }
            if (string.IsNullOrWhiteSpace(BoxLogin.Text))
            {
                MessageBox.Show("Логин пользователя не может быть пустым");
                return false;
            }
            if (_context.Users.Any(u => u.Login == BoxLogin.Text))
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
            openFileDialog.Title = "Выберите файл:";

            if (openFileDialog.ShowDialog() == true)
            {
                selectedImagePath = openFileDialog.FileName;
                newUser.PhotoPath = selectedImagePath;
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(selectedImagePath), UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ImgUserPhoto.Source = bitmap;
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckData() == false)
                return;
            try
            {
                newUser.Name = BoxName.Text;
                newUser.Login = BoxLogin.Text;
                newUser.Password = BoxPassword.Password;
                if (ComboBoxGender.SelectedItem ==  null)
                {
                    MessageBox.Show("Выберите пол");
                    return;
                }
                newUser.Gender = (ComboBoxGender.SelectedItem as ComboBoxItem)?.Tag?.ToString();
                if (!string.IsNullOrWhiteSpace(BoxHeight.Text))
                    newUser.Height = int.Parse(BoxHeight.Text);
                if (!string.IsNullOrWhiteSpace(BoxWeight.Text))
                    newUser.Weight = double.Parse(BoxWeight.Text);
                if (DPBirthDate.SelectedDate.HasValue)
                    newUser.DateOfBirth = DateOnly.FromDateTime(DPBirthDate.SelectedDate.Value);
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    string extension = System.IO.Path.GetExtension(selectedImagePath);
                    string fileName = $"{newUser.Id}_{newUser.Login}{extension}";
                    string imagePath = "../Images/" + fileName;
                    System.IO.File.Copy(selectedImagePath, imagePath, true);
                    newUser.PhotoPath = imagePath;
                }
                else
                    newUser.PhotoPath = "../Images/none.png";
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
