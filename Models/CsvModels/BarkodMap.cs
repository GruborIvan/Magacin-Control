namespace CSS_MagacinControl_App.Models.CsvModels
{
    public class BarkodMap
    {
        public string Barkod { get; set; }
        public string SifraIdenta { get; set; }

        public BarkodMap(string barkod, string sifraIdenta)
        {
            Barkod = barkod;
            SifraIdenta = sifraIdenta;
        }
    }
}