using System.Text;
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

namespace WellBeingDiary
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Логика взаимодействия для MainWindow.xaml
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            if (Models.AppContext.Users.Count == 0)
            {
                Models.AppContext.Users.Add(new User
                {
                    Id = 1,
                    Login = "test",
                    Password = "test",
                    Name = "Test",
                    Gender = "Мужской",
                    Height = 180,
                    Weight = 75,
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });
            }
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = BoxLogin.Text;
            string password = BoxPassword.Password;

            var user = Models.AppContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                MainPageWindow mainWindow = new MainPageWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new RegWindow();
            regWindow.Show();
            this.Close();
        }
    }
}
