using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models;
using CSS_MagacinControl_App.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSS_MagacinControl_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRobaService _robaService;
        private readonly IFileParser _fileParser;
        private readonly IAuthenticationRepository _authenticationRepository;
        private DialogHandler dialogHandler;

        private AuthenticationWindow _authenticationWindow;
        private AdminWindow _adminWindow;

        private readonly string _nonSelectedDropdownValue = "--";

        private bool _isAdminWindowOpen = false;
        private bool _shouldExecuteBrojFakture = true;

        private readonly IdentTrackViewModel _identTrackViewModel;

        public MainWindow(AuthenticationWindow authWindow, IRobaService robaService, IFileParser fileParser, IAuthenticationRepository authenticationRepository)
        {
            InitializeComponent();

            _robaService = robaService;
            _fileParser = fileParser;
            _authenticationWindow = authWindow;
            _authenticationRepository = authenticationRepository;
            _adminWindow = new AdminWindow(_isAdminWindowOpen, _authenticationRepository);
            dialogHandler = new DialogHandler();

            _identTrackViewModel = new IdentTrackViewModel()
            {
                FaktureState = new List<FaktureViewModel>(),
                IdentState = new List<IdentiViewModel>(),
                BarcodeToIdentDictionary = new Dictionary<string, string>()
            };

            SetupMainWindowScreen();
        }

        private async Task SetupMainWindowScreen()
        {
            // Setup admin button visibility.
            Adjust_AdminButtonVisibility();

            // Setup BrojeviFakture dropdown.
            var brojeviFaktura = new List<string>() { _nonSelectedDropdownValue };
            brojeviFaktura.AddRange(await _robaService.GetBrojeviFaktureAsync());

            BrojeviFaktureComboBox.ItemsSource = brojeviFaktura;
            BrojeviFaktureComboBox.SelectedItem = brojeviFaktura.FirstOrDefault();

            // Setup broj faktura label
            UkupnoFakturaLabel.Content = brojeviFaktura.Count();

            // Status filter ComboBox
            StatusComboBox.SelectedValue = _nonSelectedDropdownValue;
            StatusComboBox.ItemsSource = new List<string>() { _nonSelectedDropdownValue ,"U radu", "Završeno" };
        }

        private async void BrojeviFaktureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if we have prevented the handler from executing.
            if (!_shouldExecuteBrojFakture)
                return;

            var filters = Load_SelectedFilters();
            
            filters.BrojFakture = (sender as ComboBox).SelectedItem as string;

            if (filters.BrojFakture == "--")
                filters.BrojFakture = String.Empty;


            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);
        }

        private async void UcitajButton_Click(object sender, RoutedEventArgs e)
        {
            (OpenFileDialog dialog,bool result) = InitilizeOpenFileDialog();
            FileNameTextBox.Text = String.Empty;

            if (result == true)
            {
                var fileNames = dialog.FileNames.ToList();

                fileNames = _fileParser.ValidateFileNames(fileNames);

                if (fileNames == null) return; // If filenames are not good.

                var csvDataViewModel = await _fileParser.ReadDataFromCsvFilesAsync(fileNames);

                if (csvDataViewModel == null) return; // If fails during loading.

                if (await _robaService.CheckIfFakturaExists(csvDataViewModel.FaktureViewModel.First().BrojFakture))
                {
                    dialogHandler.GetFakturaAlreadyExistDialog();
                }

                ChangeCurrentState(csvDataViewModel); // Change the new state on the screen.

                // Set brojFakture dropbox
                var brojFakture = csvDataViewModel.FaktureViewModel.First().BrojFakture;

                FileNameTextBox.Text = brojFakture;
                BarCodeTextBox.Focus();
            }
        }

        private (OpenFileDialog, bool) InitilizeOpenFileDialog()
        {
            // Create OpenFileDialog 
            OpenFileDialog dialog = new OpenFileDialog()
            {
                DefaultExt = ".csv",
                Multiselect = true,
                Title = "Import fakture i stavki"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dialog.ShowDialog();

            return (dialog, result ?? false);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = dialogHandler.GetLogOutDialog();

            if (result == MessageBoxResult.Yes)
            {
                App.Current.Properties["Username"] = String.Empty;
                _authenticationWindow.Show();
                _authenticationWindow.UserBox.Clear();
                _authenticationWindow.PassBox.Clear();
                this.Hide();
            }
        }

        private void AdminPreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_adminWindow.IsOpen)
                return;

            // Open admin preferences window.
            _adminWindow = new AdminWindow(true, _authenticationRepository);
            _adminWindow.Show();
        }

        private async void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var filters = Load_SelectedFilters();

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);

            Setup_BrojeviFakturaCombobox(result.FaktureViewModel);
        }

        private async void RemoveFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _shouldExecuteBrojFakture = false;  // Prevent from 'BrojeviFaktureComboBox_SelectionChanged' call.
            BrojeviFaktureComboBox.Text = "--";  
            _shouldExecuteBrojFakture = true;

            DatePickerFilter.Text = String.Empty;
            StatusComboBox.Text = "--";

            var filters = Load_SelectedFilters();
            filters.BrojFakture = String.Empty;

            if (filters.BrojFakture == "--")
                filters.BrojFakture = String.Empty;

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);

            Setup_BrojeviFakturaCombobox(result.FaktureViewModel);
        }

        private void Adjust_AdminButtonVisibility()
        {
            bool isAdmin = (bool)App.Current.Properties["IsAdmin"];

            if (isAdmin)
            {
                AdminPreferencesButton.Visibility = Visibility.Visible;
            }
            else
            {
                AdminPreferencesButton.Visibility = Visibility.Hidden;
            }
        }

        private FilterModel Load_SelectedFilters()
        {
            return new FilterModel()
            {
                PocetniDatum = DatePickerFilter.Text,
                StatusFakture = StatusComboBox.Text == "--" ? "" : StatusComboBox.Text
            };
        }

        private void FaktureGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                BrojeviFaktureComboBox.Text = (grid.SelectedItems[0] as FaktureViewModel).BrojFakture;
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            string barCode = BarCodeTextBox.Text;
            string brojFakture = _identTrackViewModel.FaktureState.First().BrojFakture;

            if (!_identTrackViewModel.BarcodeToIdentDictionary.ContainsKey(barCode))
            {
                dialogHandler.GetWrongBarCodeDialog();
                BarCodeTextBox.Text = String.Empty;
                return;
            }

            string identName = _identTrackViewModel.BarcodeToIdentDictionary[barCode];

            // CHANGE SCAN LOGIC
            // GET IDENT NAME BY BARCODE.
            var scannedIdent = _identTrackViewModel.IdentState
                                .Where(x => x.NazivIdenta == identName)
                                .Where(x => x.BrojFakture == brojFakture)
                                .FirstOrDefault();

            if (scannedIdent != null)
            {
                // Provera da li je spakovano vise nego sto je na fakturi.
                if ((scannedIdent.KolicinaSaFakture == scannedIdent.PripremljenaKolicina) && scannedIdent.Razlika == 0)
                {
                    dialogHandler.GetPrimljenaKolicina_VecaOd_Fakturisane();
                    BarCodeTextBox.Text = String.Empty;
                    return;
                }

                scannedIdent.PripremljenaKolicina = scannedIdent.PripremljenaKolicina + 1;
                scannedIdent.Razlika = scannedIdent.KolicinaSaFakture - scannedIdent.PripremljenaKolicina;

                FaktureIdenti.ItemsSource = null;
                FaktureIdenti.ItemsSource = _identTrackViewModel.IdentState;
            }
            else
            {
                // Show dialog for Wrong Barcode scanned.
                dialogHandler.GetWrongBarCodeDialog();
            }

            BarCodeTextBox.Text = String.Empty;
        }

        private void ChangeCurrentState(FaktureIdentiViewModel faktureIdentiViewModel)
        {
            FaktureGrid.ItemsSource = faktureIdentiViewModel.FaktureViewModel;

            var brojFaktureFirst = faktureIdentiViewModel.FaktureViewModel.Count > 0
                                    ? faktureIdentiViewModel.FaktureViewModel.First().BrojFakture
                                    : String.Empty;
            
            FaktureIdenti.ItemsSource = faktureIdentiViewModel.IdentiViewModel.Where(x => x.BrojFakture.Equals(brojFaktureFirst));

            _identTrackViewModel.FaktureState = faktureIdentiViewModel.FaktureViewModel;
            _identTrackViewModel.IdentState = faktureIdentiViewModel.IdentiViewModel;
            _identTrackViewModel.BarcodeToIdentDictionary = faktureIdentiViewModel.BarcodeToIdentDictionary;

            // Set ukupnoFakturaLabel
            UkupnoFakturaLabel.Content = faktureIdentiViewModel.FaktureViewModel.Count();

            if (faktureIdentiViewModel.FaktureViewModel.Count == 1) 
            {
                if (faktureIdentiViewModel.FaktureViewModel[0].Status == "U radu")
                {
                    ScanCanvas.Visibility = Visibility.Visible;
                    SnimiEndCanvas.Visibility = Visibility.Visible;
                    BarCodeTextBox.Focus();
                    return;
                }
            }

            ScanCanvas.Visibility = Visibility.Hidden;
            SnimiEndCanvas.Visibility = Visibility.Hidden;
        }

        private void Setup_BrojeviFakturaCombobox(List<FaktureViewModel> fakture)
        {
            // Setup brojevi faktura dropdown.
            var brojeviFaktura = new List<string>() { _nonSelectedDropdownValue };
            brojeviFaktura.AddRange(fakture.Select(x => x.BrojFakture));
            BrojeviFaktureComboBox.ItemsSource = brojeviFaktura;

            // Setup broj faktura label
            UkupnoFakturaLabel.Content = fakture.Count;
        }

        private async void ZavrsetakButton_Click(object sender, RoutedEventArgs e)
        {
            if (_robaService.ValidateIdentScanState(_identTrackViewModel.IdentState).Count != 0)
            {
                dialogHandler.GetNotAllIdentsScannedDialog(_identTrackViewModel.IdentState);
                return;
            }

            var selectedFaktura = _identTrackViewModel.FaktureState.First();
            selectedFaktura.Status = "Završeno";
            await _robaService.SaveFakturaAndItemsAsync(_identTrackViewModel);

            // REFRESH THE VIEW..
            BrojeviFaktureComboBox.SelectedItem = selectedFaktura.BrojFakture;

            var filters = Load_SelectedFilters();
            filters.BrojFakture = "";

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);
            dialogHandler.GetUspesnoSnimljenoZaZavrsetakDialog(selectedFaktura.BrojFakture);
        }

        private async void SnimiZaNaknadniZavrsetakButton_Click(object sender, RoutedEventArgs e)
        {
            await _robaService.SaveFakturaAndItemsAsync(_identTrackViewModel);

            // REFRESH THE VIEW..
            var selectedFaktura = _identTrackViewModel.FaktureState.First();
            BrojeviFaktureComboBox.SelectedItem = selectedFaktura.BrojFakture;

            var filters = Load_SelectedFilters();
            filters.BrojFakture = selectedFaktura.BrojFakture;

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);

            dialogHandler.GetUspesnoSnimljenoZaNaknadniZavrsetakDialog(_identTrackViewModel.FaktureState.First().BrojFakture);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}