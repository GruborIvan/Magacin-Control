namespace CSS_MagacinControl_App.Models
{
    public class FilterModel
    {
        public string BrojFakture {  get; set; }
        public string PocetniDatum { get; set; }
        public string StatusFakture { get; set; }

        public FilterModel(string brojFakture, string pocetniDatum, string statusFakture)
        {
            BrojFakture = brojFakture;
            PocetniDatum = pocetniDatum;
            StatusFakture = statusFakture;
        }

        public FilterModel() { }
    }
}