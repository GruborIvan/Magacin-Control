using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.ViewModels.Authentication;
using System.Collections.Generic;
using System.Windows;

namespace CSS_MagacinControl_App
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private string _loggedInUsername;
        public bool IsOpen;

        private readonly IAuthenticationRepository _authenticationRepository;
        private List<UserModel> userModelState;

        public AdminWindow(bool isOpen, IAuthenticationRepository authenticationRepository)
        {
            InitializeComponent();
            _authenticationRepository = authenticationRepository;
            IsOpen = isOpen;
            Initialize_AdminScreen();
        }

        public async void Initialize_AdminScreen()
        {
            UsernameLabel.Content = App.Current.Properties["Username"] as string;

            userModelState = await _authenticationRepository.GetUsersAsync();
            UsersGrid.ItemsSource = null;
            UsersGrid.ItemsSource = userModelState;
        }

        private void DodajNovogKorisnikaButton_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow();
            addUserWindow.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsOpen = false;
        }
    }
}
