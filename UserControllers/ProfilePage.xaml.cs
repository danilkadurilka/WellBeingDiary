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
        public ProfilePage(User user)
        {
            InitializeComponent();
            currentUser = user;
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
                MessageBox.Show("Фото обновлено!");
            }
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                currentUser.Name = BoxName.Text;
                currentUser.Gender = ComboBoxGender.Text;
                currentUser.Height = Convert.ToInt32(BoxHeight.Text); 
                currentUser.Weight = Convert.ToInt32(BoxWeight.Text);
                currentUser.Login = BoxLogin.Text;

                if (isVisiblePassword)
                    currentUser.Password = BoxPasswordVisible.Text;
                else if (BoxPassword.Password != "********")
                    currentUser.Password = BoxPassword.Password;

                if (DPBirthDate.SelectedDate.HasValue)
                {
                    currentUser.DateOfBirth = DateOnly.FromDateTime(DPBirthDate.SelectedDate.Value);
                }

                if (!string.IsNullOrEmpty(BoxPassword.Password))
                {
                    currentUser.Password = BoxPassword.Password;
                }

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
