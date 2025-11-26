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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WellBeingDiary.Models;

namespace WellBeingDiary.UserControllers
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : UserControl
    {
        private User currentUser;
        private bool isVisiblePassword = false;
        private string realPassword = string.Empty;
        private Models.AppContext _context;
        public ProfilePage(User user, Models.AppContext appContext)
        {
            currentUser = user;
            _context = appContext;
            InitializeComponent();
            LoadUserData();
        }
        public void LoadUserData() 
        {
            BoxName.Text = currentUser.Name;
            ComboBoxGender.Text = currentUser.Gender;
            BoxHeight.Text = currentUser.Height.ToString();
            BoxWeight.Text = currentUser.Weight.ToString();
            BoxLogin.Text = currentUser.Login;
            realPassword = currentUser.Password;
            BoxPassword.Password = "********";
            BoxPasswordVisible.Text = realPassword;
            isVisiblePassword = false;
            ViewPasswordButton.Content = "Показать";
            BoxPasswordVisible.Visibility = Visibility.Collapsed;
            BoxPassword.Visibility = Visibility.Visible;


            if (currentUser.DateOfBirth != default)
            {
                DPBirthDate.SelectedDate = new DateTime(
                    currentUser.DateOfBirth.Year,
                    currentUser.DateOfBirth.Month,
                    currentUser.DateOfBirth.Day);
            }
        }

        private void ChangePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                currentUser.PhotoPath = openFileDialog.FileName;
                _context.SaveChanges();
                MessageBox.Show("Фото обновлено!");
            }
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (string.IsNullOrWhiteSpace(BoxName.Text))
                {
                    MessageBox.Show("Имя не может быть пустым");
                    return;
                }
                if (string.IsNullOrWhiteSpace(BoxLogin.Text))
                {
                    MessageBox.Show("Логин пользователя не может быть пустым");
                    return;
                }
                if (BoxLogin.Text != currentUser.Login && _context.Users.Any(u => u.Login == BoxLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует");
                    return;
                }

                try
                {
                    int height = int.Parse(BoxHeight.Text);
                    if (height < 60 || height > 250)
                    {
                        MessageBox.Show("Введите корректное значение роста (от 60 до 250 см)");
                        return;
                    }
                    currentUser.Height = height;

                } 
                catch(FormatException)
                {
                    MessageBox.Show("Рост должен быть целым числом");
                    return;
                }
                try
                {
                    double weight = double.Parse(BoxWeight.Text);
                    if (weight < 20 || weight > 300)
                    {
                        MessageBox.Show("Введите корректное значение веса (от 20 до 300 кг)");
                        return;
                    }
                    currentUser.Weight = weight;
                }
                catch (FormatException)
                {
                    MessageBox.Show("Вес должен быть числом");
                    return;
                }
                currentUser.Name = BoxName.Text;
                currentUser.Gender = ComboBoxGender.Text;
                currentUser.Login = BoxLogin.Text;

                if (isVisiblePassword)
                    currentUser.Password = BoxPasswordVisible.Text;
                else if (BoxPassword.Password != "********")
                    currentUser.Password = BoxPassword.Password;

                if (DPBirthDate.SelectedDate.HasValue)
                {
                    currentUser.DateOfBirth = DateOnly.FromDateTime(DPBirthDate.SelectedDate.Value);
                }

                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены");
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Возникла ошибка при сохранении: " + ex.Message);
            }
        }

        private void ViewPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isVisiblePassword)
            {
                BoxPasswordVisible.Text = realPassword;
                BoxPassword.Visibility = Visibility.Collapsed;
                BoxPasswordVisible.Visibility = Visibility.Visible;
                ViewPasswordButton.Content = "Скрыть";
            }
            else
            {
                if (BoxPasswordVisible.Text != realPassword)
                    realPassword = BoxPasswordVisible.Text;
                BoxPassword.Password = "********";
                BoxPasswordVisible.Visibility = Visibility.Collapsed;
                BoxPassword.Visibility = Visibility.Visible;
                ViewPasswordButton.Content = "Показать";
            }
            isVisiblePassword = !isVisiblePassword;
        }
    }
}
