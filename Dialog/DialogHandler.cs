using CSS_MagacinControl_App.ViewModels;
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
                messageBoxText: $" Pokušaj da se dodeli isti barkod dva različitim identima! \n {errorContents}",
                caption: "Neispravan file!",
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
        public MessageBoxResult GetWrongBarCodeDialog()
        {
            return MessageBox.Show(
                messageBoxText: "Skenirani bar kod ne odgovara nijednoj stavci.",
                caption: "Greška",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Error,
                defaultResult: MessageBoxResult.Yes
            );
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
                messageBoxText: "  Are you sure that you want to Log Out?",
                caption: "Log out",
                button: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question,
                defaultResult: MessageBoxResult.Yes
            );
        }

        public MessageBoxResult GetFailedUsernamePassDialog()
        {
            return MessageBox.Show(
                messageBoxText: " Login failed for user.",
                caption: "Login failed",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Warning,
                defaultResult: MessageBoxResult.Yes
            );
        }

        public MessageBoxResult GetPrimljenaKolicina_VecaOd_Fakturisane()
        {
            return MessageBox.Show(
                messageBoxText: " Pripremljena količina je veća od količine sa fakture. \n Upakovano je već koliko je potrebno.",
                caption: "Greška prilikom skeniranja!",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Warning,
                defaultResult: MessageBoxResult.Yes
            );
        }

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
    }
}