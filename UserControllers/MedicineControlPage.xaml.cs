using Microsoft.EntityFrameworkCore;
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
        private Models.AppContext _context;
        public MedicineControlPage(User user, Models.AppContext appContext)
        {
            currentUser = user;
            _context = appContext;
            InitializeComponent();
            LoadMedicines();
        }
        public void LoadMedicines()
        {
            List<Medicine>? medicines = _context.Medicines?.Where(m => m.UserId == currentUser.Id).ToList();
            MedicinesListBox.ItemsSource = medicines;           
        }

        private void AddMedicineButton_Click(object sender, RoutedEventArgs e)
        {
            Window medicineEditWindow = new MedicineEditWindow(currentUser, _context);
            if (medicineEditWindow.ShowDialog() == true)
                LoadMedicines();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Medicine selectedMedicine = (Medicine)MedicinesListBox.SelectedItem;
            if (selectedMedicine != null)
            {
                Window medicineEditWindow = new MedicineEditWindow(currentUser, _context, selectedMedicine);
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
            if (selectedMedicine != null)
            {
                var result = MessageBox.Show("Удалить это лекарство?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result.ToString() == "Yes")
                {
                    try
                    {
                        _context.Medicines.Remove(selectedMedicine);
                        _context.SaveChanges();
                        LoadMedicines();
                        MessageBox.Show("Лекарство удалено");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите лекарство для удаления!");
            }
        }
    }
}
