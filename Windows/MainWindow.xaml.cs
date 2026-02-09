using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models;
using CSS_MagacinControl_App.ViewModels;
using CSS_PA_Otprema.Windows;
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
        private DialogHandler _dialogHandler;

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
            _dialogHandler = new DialogHandler();

            _identTrackViewModel = new IdentTrackViewModel()
            {
                FaktureState = new List<FaktureViewModel>(),
                IdentState = new List<IdentiViewModel>(),
                BarcodeToIdentDictionary = new Dictionary<string, string>()
            };

            InitializeAsync();
        }

        #region SCREEN INITIALIZATION & SETUP

        private async Task InitializeAsync()
        {
            Adjust_AdminButtonVisibility();
            InitializeStatusComboBox();

            DatePickerFilter.SelectedDate = DateTime.UtcNow;

            var filters = Load_SelectedFilters();

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            Setup_BrojeviFakturaDropdown(result.FaktureViewModel);
            ChangeCurrentState(result);
        }

        private void InitializeStatusComboBox()
        {
            StatusComboBox.SelectedValue = _nonSelectedDropdownValue;
            StatusComboBox.ItemsSource = new List<string> { _nonSelectedDropdownValue, "U radu", "Završeno" };
        }

        private void Setup_BrojeviFakturaDropdown(List<FaktureViewModel> fakture)
        {
            var brojeviFaktura = new List<string>() { _nonSelectedDropdownValue };
            brojeviFaktura.AddRange(fakture.Select(x => x.BrojFakture));
            BrojeviFaktureComboBox.ItemsSource = brojeviFaktura;

            UkupnoFakturaLabel.Content = fakture.Count;
        }

        #endregion

        #region FILE_LOADING

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
                    _dialogHandler.GetFakturaAlreadyExistDialog();
                    return;
                }

                ClearFilters();
                ChangeCurrentState(csvDataViewModel);
                
                var brojFakture = csvDataViewModel.FaktureViewModel.First().BrojFakture;

                FileNameTextBox.Text = brojFakture;
                FocusOnBarCodeScanTextBox();
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

        #endregion

        #region FILTERS

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

            Setup_BrojeviFakturaDropdown(result.FaktureViewModel);
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

            Setup_BrojeviFakturaDropdown(result.FaktureViewModel);
        }

        private async void BrojeviFaktureDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_shouldExecuteBrojFakture)
                return;

            var filters = Load_SelectedFilters();

            filters.BrojFakture = (sender as ComboBox).SelectedItem as string;

            if (filters.BrojFakture == "--")
                filters.BrojFakture = String.Empty;


            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);
        }

        private FilterModel Load_SelectedFilters()
        {
            return new FilterModel()
            {
                PocetniDatum = DatePickerFilter.Text,
                StatusFakture = StatusComboBox.Text == "--" ? "" : StatusComboBox.Text
            };
        }

        private void ClearFilters()
        {
            _shouldExecuteBrojFakture = false;
            BrojeviFaktureComboBox.Text = "--";
            _shouldExecuteBrojFakture = true;

            DatePickerFilter.Text = String.Empty;
            StatusComboBox.Text = "--";
        }

        #endregion

        private void FaktureGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                    FocusOnBarCodeScanTextBox();
                    return;
                }
            }

            ScanCanvas.Visibility = Visibility.Hidden;
            SnimiEndCanvas.Visibility = Visibility.Hidden;
            IzvozUCsvButton.Visibility = Visibility.Hidden;
        }

        #region SAVE_PROGRESS

        private async void ZavrsetakButton_Click(object sender, RoutedEventArgs e)
        {
            ZavrsetakButton.IsEnabled = false;
            var unscannedIdents = _robaService.ValidateIdentScanState(_identTrackViewModel.IdentState);

            if (unscannedIdents.Count != 0)
            {
                _dialogHandler.GetNotAllIdentsScannedDialog(unscannedIdents);
                ZavrsetakButton.IsEnabled = true;
                FocusOnBarCodeScanTextBox();
                return;
            }

            var selectedFaktura = _identTrackViewModel.FaktureState.First(); 
            await _robaService.SaveFakturaAndItemsAsync(_identTrackViewModel); 

            await _robaService.ChangeFakturaStatusToDoneAsync(selectedFaktura.BrojFakture);

            BrojeviFaktureComboBox.SelectedItem = selectedFaktura.BrojFakture;

            var filters = Load_SelectedFilters();
            filters.BrojFakture = "";

            var result = await _robaService.GetFilteredFaktureAsync(filters);

            ChangeCurrentState(result);
            _dialogHandler.GetUspesnoSnimljenoZaZavrsetakDialog(selectedFaktura.BrojFakture);

            BarCodeTextBox.Text = string.Empty;
            RemoveLastScannedArticleLabel();
            ZavrsetakButton.IsEnabled = true;
        }

        private async Task SnimiZaNaknadniZavrsetak(bool isUserClicked)
        {
            SnimiZaNaknadniZavrsetakButton.IsEnabled = false;
            await _robaService.SaveFakturaAndItemsAsync(_identTrackViewModel);

            var filters = Load_SelectedFilters();

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
                _dialogHandler.GetUspesnoSnimljenoZaNaknadniZavrsetakDialog(_identTrackViewModel.FaktureState.First().BrojFakture);
            }

            RemoveLastScannedArticleLabel();
            SnimiZaNaknadniZavrsetakButton.IsEnabled = true;
        }

        private async void SnimiZaNaknadniZavrsetakButton_Click(object sender, RoutedEventArgs e)
        {
            await SnimiZaNaknadniZavrsetak(isUserClicked: true);
        }

        private void RemoveLastScannedArticleLabel()
        {
            LastScannedArticleLabel.Content = string.Empty;
        }

        #endregion

        #region BARCODE_SCAN

        private async void BarCodeScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (!TryGetScanContext(out var barCode, out var brojFakture, out _))
                return;

            if (!TryProcessScannedAmounts(barCode, brojFakture, 1))
                return;

            ShowLastScannedArticle(barCode, brojFakture);
            await FlashRowAsync(barCode, brojFakture);
        }

        private async void BarCodeScan_WithAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) 
                return;

            if (!int.TryParse(KolicinaBarKodTextBox.Text, out int kolicina))
            {
                KolicinaBarKodTextBox.Text = string.Empty;
                return;
            }

            if (!TryGetScanContext(out var barCode, out var brojFakture, out _))
                return;

            if (TryProcessScannedAmounts(barCode, brojFakture, kolicina))
            {
                ShowLastScannedArticle(barCode, brojFakture, kolicina);
                await FlashRowAsync(barCode, brojFakture);
            }

            KolicinaBarKodTextBox.Text = string.Empty;
            KolicinaBarKodTextBox.IsEnabled = false;
        }

        private bool TryGetScanContext(out string barCode, out string brojFakture, out IdentiViewModel scannedIdent)
        {
            barCode = BarCodeTextBox.Text;
            brojFakture = _identTrackViewModel.FaktureState.First().BrojFakture;

            if (!TryGetIdentByBarCode(barCode, brojFakture, out scannedIdent))
            {
                _dialogHandler.GetWrongBarCodeDialog(this);
                BarCodeTextBox.Text = string.Empty;
                return false;
            }

            return true;
        }

        private bool TryGetIdentByBarCode(string barCode, string brojFakture, out IdentiViewModel scannedIdent)
        {
            scannedIdent = null;

            if (!_identTrackViewModel.BarcodeToIdentDictionary.TryGetValue(barCode, out var sifraIdenta))
                return false;

            scannedIdent = _identTrackViewModel.IdentState
                .FirstOrDefault(x => x.SifraIdenta == sifraIdenta && x.BrojFakture == brojFakture);

            return scannedIdent != null;
        }

        private bool TryProcessScannedAmounts(string barCode, string brojFakture, int kolicina)
        {
            if (!TryGetIdentByBarCode(barCode, brojFakture, out var scannedIdent))
            {
                _dialogHandler.GetWrongBarCodeDialog(this);

                BarCodeTextBox.Text = string.Empty;
                return false;
            }

            if (scannedIdent.Oznaka == -1 && String.IsNullOrEmpty(KolicinaBarKodTextBox.Text))
            {
                KolicinaBarKodTextBox.IsEnabled = true;
                KolicinaBarKodTextBox.Focus();
                return false;
            }

            if (scannedIdent.PripremljenaKolicina + kolicina > scannedIdent.KolicinaSaFakture)
            {
                var customErrorDialog = new CustomErrorDialog(this, "Greška prilikom skeniranja!", "Pripremljena količina je veća od količine sa fakture. \n Upakovano je već koliko je potrebno.");
                customErrorDialog.ShowDialog();

                BarCodeTextBox.Text = String.Empty;
                FocusOnBarCodeScanTextBox();
                return false;
            }

            scannedIdent.PripremljenaKolicina = scannedIdent.PripremljenaKolicina + kolicina;
            scannedIdent.Razlika = scannedIdent.KolicinaSaFakture - scannedIdent.PripremljenaKolicina;
            BarCodeTextBox.Text = String.Empty;
            FocusOnBarCodeScanTextBox();
            return true;
        }

        private void FocusOnBarCodeScanTextBox()
        {
            BarCodeTextBox.Focus();
        }

        private async Task FlashRowAsync(string barCode, string brojFakture)
        {
            if (!TryGetIdentByBarCode(barCode, brojFakture, out var scannedIdent))
            {
                _dialogHandler.GetWrongBarCodeDialog(this);
                BarCodeTextBox.Text = string.Empty;
                return;
            }

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    scannedIdent.IsRecentlyScanned = true;

                    FaktureIdenti.UpdateLayout();
                    FaktureIdenti.ScrollIntoView(scannedIdent);

                    if (FaktureIdenti.ItemContainerGenerator.ContainerFromItem(scannedIdent) is DataGridRow row)
                    {
                        row.BringIntoView();
                    }
                });

                await Task.Delay(2000);
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    scannedIdent.IsRecentlyScanned = false;
                });
            }
        }
    
        private void ShowLastScannedArticle(string barCode, string brojFakture, int kolicina = 1)
        {
            if (!TryGetIdentByBarCode(barCode, brojFakture, out var scannedIdent))
                return;

            var scannedArticleTextToDisplay = $"{scannedIdent.NazivIdenta}   Kolicina: {kolicina}";
            LastScannedArticleLabel.Content = scannedArticleTextToDisplay;
        }

        #endregion

        #region AUTHENTICATION

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            await SnimiZaNaknadniZavrsetak(false);

            var result = _dialogHandler.GetLogOutDialog();

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

            _adminWindow = new AdminWindow(true, _authenticationRepository);
            _adminWindow.Show();
        }

        private void Adjust_AdminButtonVisibility()
        {
            bool isAdmin = (bool)App.Current.Properties["IsAdmin"];

            AdminPreferencesButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion

        private void IzvozUCsvButton_Click(object sender, RoutedEventArgs e)
        {
            var faktura = _identTrackViewModel.FaktureState.First();

            _fileParser.PackFaktureToCsvFile(faktura);

            Application.Current.Shutdown();
        }

        protected override async void OnClosed(EventArgs e)
        {
            await SnimiZaNaknadniZavrsetak(false);

            await Task.Delay(10000);
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}