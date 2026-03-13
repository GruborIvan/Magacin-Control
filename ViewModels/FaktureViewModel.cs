namespace CSS_MagacinControl_App.ViewModels
{
    public class FaktureViewModel
    {
        public string BrojFakture { get; set; }
        public System.DateTime DatumFakture { get; set; }
        public string SifraKupca { get; set; }
        public string NazivKupca { get; set; }
        public string Magacioner { get; set; }
        public string Status { get; set; }

        public FaktureViewModel(string brojFakture, System.DateTime datumFakture, string sifraKupca, string nazivKupca, string magacioner, string status)
        {
            BrojFakture = brojFakture;
            DatumFakture = datumFakture;
            SifraKupca = sifraKupca;
            NazivKupca = nazivKupca;
            Magacioner = magacioner;
            Status = status;
        }

        public FaktureViewModel() { }
    }
}