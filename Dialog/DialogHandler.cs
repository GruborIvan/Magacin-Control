using CSS_MagacinControl_App.ViewModels;
using CSS_PA_Otprema.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CSS_MagacinControl_App.Dialog
{
    public class DialogHandler
    {
        public DialogHandler() 
        {

        }

        // Neispravan fajl! Barkod moze pripadati samo jednom artiklu - ERROR
        public MessageBoxResult GetBarcodeAlreadyAssignedToIdentDialog(List<CsvParseErrorModel> errorModel)
        {
            string errorContents = string.Empty;

            foreach (var error in errorModel)
            {
                string singleErrorStr = $"\n  Barkod  {error.Value}  se dodeljuje {error.Count} identa: \n  Nazivi idenata:  \n";
                foreach(var barkod in error.NaziviIdenta)
                {
                    singleErrorStr += $"      {barkod}\n";
                }   
                
                errorContents += singleErrorStr ;
            }

            return MessageBox.Show(
                messageBoxText: $" Pokušaj da se dodeli isti barkod različitim identima! \n {errorContents}",
                caption: "Neispravan fajl!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Pokretanje - Nije moguće uspostaviti vezu sa bazom podataka!
        public MessageBoxResult GetDatabaseNotAccessibleDialog(string ex)
        {
            return MessageBox.Show(
                messageBoxText: $" Nije moguće uspostaviti vezu sa bazom podataka!\n Proverite kredencijale i pokušajte ponovo\n \n {ex}",
                caption: "Greška prilikom konekcije!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Kreiranje novog korisnika - Nisu popunjena sva polja.
        public MessageBoxResult GetNotAllFieldsFilled()
        {
            return MessageBox.Show(
                messageBoxText: $" Nisu popunjena sva polja!",
                caption: "Neuspešno dodavanje korisnika!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Kreiranje novog korisnika - Korisnicko ime vec zauzeto.
        public MessageBoxResult GetUsernameAlreadyTakenDialog()
        {
            return MessageBox.Show(
                messageBoxText: $" Korisničko ime već postoji u sistemu!",
                caption: "Neuspešno dodavanje korisnika!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Kreiranje novog korisnika - Ne poklapaju se sifra i ponovljena sifra.
        public MessageBoxResult GetRazliciteSifreDialog()
        {
            return MessageBox.Show(
                messageBoxText: $" Lozinke se ne poklapaju!",
                caption: "Neuspešno dodavanje korisnika!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Kreiranje novog korisnika - Ne poklapaju se sifra i ponovljena sifra.
        public MessageBoxResult GetRazliciteSifre_PromenaSifreDialog()
        {
            return MessageBox.Show(
                messageBoxText: $"  Unesene lozinke se ne poklapaju! \n  Unesite ponovo!",
                caption: " Neuspešna promena lozinke!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Ucitana faktura vec postoji! - ERROR
        public MessageBoxResult GetFakturaAlreadyExistDialog()
        {
            return MessageBox.Show(
                messageBoxText: $" Faktura koju pokušavate da učitate već postoji!",
                caption: "Greška",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Not all idents scanned - ERROR
        public MessageBoxResult GetNotAllIdentsScannedDialog(List<IdentiViewModel> failList)
        {
            var items = string.Join("\n   ", failList.Select(x => x.NazivIdenta));

            return MessageBox.Show(
                messageBoxText: $"  Nisu skenirani svi identi! \n  Završite skeniranje ili snimite za naknadni završetak. \n \n  Preostali identi: \n   {items}",
                caption: "Greška",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Barcode scan - WRONG CODE
        public void GetWrongBarCodeDialog(Window window)
        {
            var customErrorDialog = new CustomErrorDialog(window, "Skenirani bar kod ne odgovara nijednoj stavci!", "Skenirani bar kod ne odgovara nijednoj stavci, pokusajte da skenirate drugi proizvod.");
            customErrorDialog.ShowDialog();
        }

        // Pogresan broj fajlova ucitano.. Moguce 1 ili 3.
        public MessageBoxResult GetWrongFileNumberSelectDialog(int numberOfSelectedFiles)
        {
            return MessageBox.Show(
                messageBoxText: $"Greška prilikom učitavanja fajlova. \n Moguće je učitati 1 ili 3 fajlova. \n Učitano je {numberOfSelectedFiles} fajlova.",
                caption: "Greška",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Error prilikom ucitavanja fajlova.
        public MessageBoxResult GetErrorWhileLoadingFilesDialog()
        {
            return MessageBox.Show(
                messageBoxText: "Greška prilikom učitavanja fajlova.",
                caption: "Greška",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Logout - Are you sure you want to Logout?
        public MessageBoxResult GetLogOutDialog()
        {
            return MessageBox.Show(
                messageBoxText: "  Da li ste sigurni da želite da se odjavite sa sistema?",
                caption: "Odjava",
                button: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Admin - Save changes to user account?
        public MessageBoxResult GetSaveUserChangesDialog()
        {
            return MessageBox.Show(
                messageBoxText: " Da li želite da sačuvate izmene korisničkog naloga?",
                caption: "Čuvanje izmena?",
                button: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question,
                defaultResult: MessageBoxResult.Yes
            );
        }

        public MessageBoxResult GetFailedUsernamePassDialog()
        {
            return MessageBox.Show(
                messageBoxText: " Neuspešna prijava na sistem.",
                caption: "Neuspešno logovanje",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Warning,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Kad korisnik uspesno promeni lozinku naloga.
        public MessageBoxResult GetUspesnoPromenjenaLozinkaKorisnikaDialog()
        {
            return MessageBox.Show(
                messageBoxText: $" Uspešno promenjena lozinka naloga!",
                caption: "Sačuvano!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Information,
                defaultResult: MessageBoxResult.Yes
            );
        }

        #region SAVE_PROGRESS

        // Kad korisnik uspesno snimi zavrsetak.
        public MessageBoxResult GetUspesnoSnimljenoZaZavrsetakDialog(string brojFakture)
        {
            return MessageBox.Show(
                messageBoxText: $" Faktura {brojFakture} je uspešno snimljena. \n  Status: Završeno.",
                caption: "Snimljeno!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Information,
                defaultResult: MessageBoxResult.Yes
            );
        }

        // Kad korisnik uspesno snimi za naknadni zavrsetak.
        public MessageBoxResult GetUspesnoSnimljenoZaNaknadniZavrsetakDialog(string brojFakture)
        {
            return MessageBox.Show(
                messageBoxText: $" Faktura {brojFakture} je uspešno snimljena za naknadni završetak. \n",
                caption: "Snimljeno!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Information,
                defaultResult: MessageBoxResult.Yes
            );
        }

        #endregion
    }
}