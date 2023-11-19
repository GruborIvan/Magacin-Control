using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.ViewModels.Authentication;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CSS_MagacinControl_App
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public bool IsOpen;

        private readonly IAuthenticationRepository _authenticationRepository;
        public ObservableCollection<UserModel> userModelState;
        private readonly DialogHandler dialogHandler;

        public AdminWindow(bool isOpen, IAuthenticationRepository authenticationRepository)
        {
            InitializeComponent();
            _authenticationRepository = authenticationRepository;
            IsOpen = isOpen;
            dialogHandler = new DialogHandler();
            Initialize_AdminScreen();
        }

        public async Task Initialize_AdminScreen()
        {
            UsernameLabel.Content = App.Current.Properties["Username"] as string;

            userModelState = new ObservableCollection<UserModel>(
                await _authenticationRepository.GetUsersAsync()
            );

            UsersGrid.ItemsSource = userModelState;
        }

        private void DodajNovogKorisnikaButton_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new (_authenticationRepository, this);
            addUserWindow.Show();
        }

        private void ChangePassword_ButtonClick(object sender, RoutedEventArgs e)
        {
            Guid userChangeIndentifier = (((FrameworkElement)sender).DataContext as UserModel).Id;

            var changePasswordWindow = new ChangeUserPasswordWindow(userChangeIndentifier, _authenticationRepository);
            changePasswordWindow.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            IsOpen = false;
        }

        private async void UsersGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var changedUser = await _authenticationRepository.FindChangedUserAsync(userModelState.ToList());

            if (changedUser != null)
            {
                var result = dialogHandler.GetSaveUserChangesDialog();

                if (result == MessageBoxResult.Yes)
                {
                    await _authenticationRepository.SaveUserChangesAsync(changedUser);
                }
            }

            await Initialize_AdminScreen();
        }
    }
}
