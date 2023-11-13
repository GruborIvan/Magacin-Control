namespace CSS_MagacinControl_App.Models.CsvModels
{
    public class BarkodMap
    {
        public string Barkod { get; set; }
        public string NazivIdenta { get; set; }

        public BarkodMap(string barkod, string nazivIdenta)
        {
            Barkod = barkod;
            NazivIdenta = nazivIdenta;
        }
    }
}