using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models;
using CSS_MagacinControl_App.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            // Status filter ComboBox
            StatusComboBox.SelectedValue = _nonSelectedDropdownValue;
            StatusComboBox.ItemsSource = new List<string>() { _nonSelectedDropdownValue ,"U radu", "Završeno" };

            await PullMainScreenData(); 
        }

        private async Task PullMainScreenData()
        {
            DatePickerFilter.SelectedDate = DateTime.UtcNow;
            var filters = Load_SelectedFilters();

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            Setup_BrojeviFakturaCombobox(result.FaktureViewModel);
            ChangeCurrentState(result);
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
                    return;
                }

                ClearFilters();
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

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            await SnimiZaNaknadniZavrsetak(false);

            var result = dialogHandler.GetLogOutDialog();

            if (result == MessageBoxResult.Yes)
            {
                App.Current.Properties["Username"] = String.Empty;
                _authenticationWindow.Show();
                _authenticationWindow.UserBox.Clear();
                _authenticationWindow.PassBox.Clear();
                _authenticationWindow.UserBox.Focus();
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
            // If accidently clicked, save changes.
            if (_identTrackViewModel.FaktureState.Count == 1)
            {
                await SnimiZaNaknadniZavrsetak(false);
            }

            var filters = Load_SelectedFilters();

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);

            Setup_BrojeviFakturaCombobox(result.FaktureViewModel);
        }

        private async void RemoveFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // If accidently clicked, save changes.
            if (_identTrackViewModel.FaktureState.Count == 1)
            {
                await SnimiZaNaknadniZavrsetak(false);
            }

            ClearFilters();
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
                IzvozUCsvButton.Visibility = Visibility.Visible;

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
            IzvozUCsvButton.Visibility = Visibility.Hidden;
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
            ZavrsetakButton.IsEnabled = false;
            var unscannedIdents = _robaService.ValidateIdentScanState(_identTrackViewModel.IdentState);

            if (unscannedIdents.Count != 0)
            {
                dialogHandler.GetNotAllIdentsScannedDialog(unscannedIdents);
                ZavrsetakButton.IsEnabled = true;
                return;
            }

            var selectedFaktura = _identTrackViewModel.FaktureState.First(); // Get selected faktura
            await _robaService.SaveFakturaAndItemsAsync(_identTrackViewModel); // Save faktura and items to database.

            await _robaService.ChangeFakturaStatusToDoneAsync(selectedFaktura.BrojFakture); // Change status of faktura to 'Done'.

            // REFRESH THE VIEW..
            BrojeviFaktureComboBox.SelectedItem = selectedFaktura.BrojFakture;

            var filters = Load_SelectedFilters();
            filters.BrojFakture = "";

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);
            dialogHandler.GetUspesnoSnimljenoZaZavrsetakDialog(selectedFaktura.BrojFakture);

            ZavrsetakButton.IsEnabled = true;
        }

        private async Task SnimiZaNaknadniZavrsetak(bool isUserClicked)
        {
            SnimiZaNaknadniZavrsetakButton.IsEnabled = false;
            await _robaService.SaveFakturaAndItemsAsync(_identTrackViewModel);

            var filters = Load_SelectedFilters();

            // REFRESH THE VIEW..
            if (_identTrackViewModel.FaktureState.Any())
            {
                var selectedFaktura = _identTrackViewModel.FaktureState.First();
                BrojeviFaktureComboBox.SelectedItem = selectedFaktura.BrojFakture;
                filters.BrojFakture = selectedFaktura.BrojFakture;
            }
            else
            {
                filters.BrojFakture = string.Empty;
            }

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);

            if (isUserClicked)
            {
                dialogHandler.GetUspesnoSnimljenoZaNaknadniZavrsetakDialog(_identTrackViewModel.FaktureState.First().BrojFakture);
            }

            SnimiZaNaknadniZavrsetakButton.IsEnabled = true;
        }

        private async void SnimiZaNaknadniZavrsetakButton_Click(object sender, RoutedEventArgs e)
        {
            await SnimiZaNaknadniZavrsetak(isUserClicked: true);
        }

        private void IzvozUCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var faktura = _identTrackViewModel.FaktureState.First();

            _fileParser.PackFaktureToCsvFile(faktura);
        }

        private void ClearFilters()
        {
            _shouldExecuteBrojFakture = false;  // Prevent from 'BrojeviFaktureComboBox_SelectionChanged' call.
            BrojeviFaktureComboBox.Text = "--";
            _shouldExecuteBrojFakture = true;

            DatePickerFilter.Text = String.Empty;
            StatusComboBox.Text = "--";
        }

        protected override async void OnClosed(EventArgs e)
        {
            await SnimiZaNaknadniZavrsetak(false);

            await Task.Delay(10000);
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private async void BarCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string barCode = BarCodeTextBox.Text;
                string brojFakture = _identTrackViewModel.FaktureState.First().BrojFakture;

                if (!_identTrackViewModel.BarcodeToIdentDictionary.ContainsKey(barCode))
                {
                    dialogHandler.GetWrongBarCodeDialog();
                    BarCodeTextBox.Text = String.Empty;
                    return;
                }

                CalculateScannedAmounts(barCode, brojFakture, 1);
                ShowLastScannedArticle(barCode);
                await FlashRowAsync(barCode, brojFakture);
            }
        }

        private async void KolicinaBarKod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) 
                return;

            if (!int.TryParse(KolicinaBarKodTextBox.Text, out int kolicina)) // Ucitavanje i validiranje kolicine.
            {
                KolicinaBarKodTextBox.Text = string.Empty;
                return;
            }

            var barCode = BarCodeTextBox.Text;
            string brojFakture = _identTrackViewModel.FaktureState.First().BrojFakture;
            
            CalculateScannedAmounts(barCode, brojFakture, kolicina);
            ShowLastScannedArticle(barCode, kolicina);
            await FlashRowAsync(barCode, brojFakture);

            // Ovde je logika kad se ucita proizvod..
            KolicinaBarKodTextBox.Text = string.Empty;
            KolicinaBarKodTextBox.IsEnabled = false;
        }

        private void CalculateScannedAmounts(string barkod, string brojFakture, int kolicina)
        {
            string sifraIdenta = _identTrackViewModel.BarcodeToIdentDictionary[barkod];

            var scannedIdent = _identTrackViewModel.IdentState
                    .Where(x => x.SifraIdenta == sifraIdenta)
                    .Where(x => x.BrojFakture == brojFakture)
                    .FirstOrDefault();


            if (scannedIdent != null)
            {
                if (scannedIdent.Oznaka == -1 && String.IsNullOrEmpty(KolicinaBarKodTextBox.Text))
                {
                    KolicinaBarKodTextBox.IsEnabled = true;
                    KolicinaBarKodTextBox.Focus();
                    return;
                }

                if (scannedIdent.PripremljenaKolicina + kolicina > scannedIdent.KolicinaSaFakture)
                {
                    dialogHandler.GetPrimljenaKolicina_VecaOd_Fakturisane();
                    BarCodeTextBox.Text = String.Empty;
                    return;
                }

                scannedIdent.PripremljenaKolicina = scannedIdent.PripremljenaKolicina + kolicina;
                scannedIdent.Razlika = scannedIdent.KolicinaSaFakture - scannedIdent.PripremljenaKolicina;
            }
            else
            {
                dialogHandler.GetWrongBarCodeDialog();
            }

            BarCodeTextBox.Text = String.Empty;
        }

        private async Task FlashRowAsync(string barkod, string brojFakture)
        {
            string sifraIdenta = _identTrackViewModel.BarcodeToIdentDictionary[barkod];

            var scannedIdent = _identTrackViewModel.IdentState
                    .Where(x => x.SifraIdenta == sifraIdenta)
                    .Where(x => x.BrojFakture == brojFakture)
                    .FirstOrDefault();

            if (scannedIdent == null) return;

            scannedIdent.IsRecentlyScanned = true;

            // Optional: bring it into view and/or select it
            Application.Current.Dispatcher.Invoke(() =>
            {
                FaktureIdenti.UpdateLayout();
                FaktureIdenti.ScrollIntoView(scannedIdent);
                var row = (DataGridRow)FaktureIdenti.ItemContainerGenerator.ContainerFromItem(scannedIdent);
                row?.BringIntoView();
            });

            await Task.Delay(2000);
            scannedIdent.IsRecentlyScanned = false;
        }
    
        private void ShowLastScannedArticle(string barCode, int kolicina = 1)
        {
            var scannedIdent = _identTrackViewModel.GetIdentByBarCode(barCode);
            var scannedArticleTextToDisplay = $"{scannedIdent.NazivIdenta}   Kolicina: {kolicina}";
            LastScannedArticleLabel.Content = scannedArticleTextToDisplay;
        }
    }
}