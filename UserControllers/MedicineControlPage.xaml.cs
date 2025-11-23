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
using WellBeingDiary.Windows;

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
            Window medicineEditWindow = new MedicineEditWindow(currentUser);
            if (medicineEditWindow.ShowDialog() == true)
                LoadMedicines();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Medicine selectedMedicine = (Medicine)MedicinesListBox.SelectedItem;
            if (MedicinesListBox.SelectedItem == selectedMedicine)
            {
                Window medicineEditWindow = new MedicineEditWindow(currentUser, selectedMedicine);
                if (medicineEditWindow.ShowDialog() == true)
                    LoadMedicines();
            }
            else
            {
                MessageBox.Show("Выберите лекарство для редактирования!");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Medicine selectedMedicine = (Medicine)MedicinesListBox.SelectedItem;
            if (MedicinesListBox.SelectedItem == selectedMedicine)
            {
                var result = MessageBox.Show("Удалить это лекарство?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result.ToString() == "Yes")
                {
                    Models.AppContext.Medicines?.Remove(selectedMedicine);
                    LoadMedicines();
                }
            }
            else
            {
                MessageBox.Show("Выберите лекарство для удаления!");
            }
        }
    }
}
