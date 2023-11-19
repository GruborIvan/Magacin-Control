using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.ViewModels.Authentication;
using System;
using System.Windows;

namespace CSS_MagacinControl_App
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private AdminWindow adminWindow;

        public AddUserWindow(IAuthenticationRepository authenticationRepository, AdminWindow parentAdminWindow)
        {
            InitializeComponent();
            _authenticationRepository = authenticationRepository;
            adminWindow = parentAdminWindow;
        }

        private async void KreirajKorisnikaButton_Click(object sender, RoutedEventArgs e)
        {
            var korisnik = new UserModel()
            {
                Id = Guid.NewGuid(),
                Username = UsernameTextBox.Text,
                Name = ImeTextBox.Text,
                Surname = PrezimeTextBox.Text,
                IsAdmin = (bool)AdminCheckBox.IsChecked,
                Password = PasswordBox.Password,
            };

            if (!await _authenticationRepository.ValidateNewUserAsync(korisnik, PasswordRepeatBox.Password))
            {
                return;
            }

            await _authenticationRepository.AddNewUserAsync(korisnik);

            adminWindow.Initialize_AdminScreen();
            this.Close();
        }
    }
}