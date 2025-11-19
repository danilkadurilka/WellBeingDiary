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
    /// Логика взаимодействия для MedicineControlPage.xaml
    /// </summary>
    public partial class MedicineControlPage : UserControl
    {
        private User currentUser;
        public MedicineControlPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadMedicines();
        }

        public void LoadMedicines()
        {
            var medicines = Models.AppContext.Medicines?.Where(m => m.UserId == currentUser.Id).ToList();
            MedicinesListBox.ItemsSource = medicines;
        }

        private void AddMedicineButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
