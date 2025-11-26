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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Models.AppContext _context;
        public MainWindow()
        {
            InitializeComponent();
            InitializeDB();
        }
        public void InitializeDB()
        {
            try
            {
                _context = new Models.AppContext();
                _context.Database.EnsureCreated();
                if (!_context.Users.Any(u => u.Login == "test")) 
                {
                    _context.Users.Add(new User { Id = 1, Login = "test", Password = "test", Name = "Тестовый", Gender = "Мужской", Height = 180, Weight = 75, DateOfBirth = new DateOnly(1999, 4, 1) });
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации БД: {ex.Message}");
            }
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = BoxLogin.Text;
            string password = BoxPassword.Password;

            try
            {
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Введите логин и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    MainPageWindow mainWindow = new MainPageWindow(user, _context);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}");
            }

            
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new RegWindow(_context);
            regWindow.Show();
            this.Close();
        }

    }
}
