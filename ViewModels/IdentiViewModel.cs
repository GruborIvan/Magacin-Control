namespace CSS_MagacinControl_App.ViewModels
{
    public class IdentiViewModel
    {
        public string SifraIdenta { get; set; }
        public string NazivIdenta { get; set; }
        public string Barkod { get; set; }
        public string BrojFakture { get; set; }
        public decimal KolicinaSaFakture { get; set; }
        public decimal PripremljenaKolicina { get; set; }
        public decimal Razlika { get; set; }
        public int Oznaka { get; set; }
        public string OznakaUsluge { get; set; }

        public IdentiViewModel(string sifraIdenta, 
                               string nazivIdenta, 
                               string identBarKod, 
                               string brojFakture, 
                               decimal kolicinaSaFakture, 
                               decimal pripremljenaKolicina, 
                               decimal razlika, 
                               int oznaka, 
                               string oznakaUsluge)
        {
            SifraIdenta = sifraIdenta;
            NazivIdenta = nazivIdenta;
            Barkod = identBarKod;
            BrojFakture = brojFakture;
            KolicinaSaFakture = kolicinaSaFakture;
            PripremljenaKolicina = pripremljenaKolicina;
            Razlika = razlika;
            Oznaka = oznaka;
            OznakaUsluge = oznakaUsluge;
        }

        public IdentiViewModel() 
        {
            
        }
    }
}