using CSS_MagacinControl_App.ViewModels;
using System.Collections.Generic;

namespace CSS_MagacinControl_App.Models.CsvModels
{
    public class FullModelCsv
    {
        public FaktureViewModel Faktura { get; set; }
        public List<IdentiCsv> Identi { get; set; }
        public List<KolicineCsv> Kolicine { get; set; }

        public FullModelCsv()
        {
            Identi = new List<IdentiCsv>();
            Kolicine = new List<KolicineCsv>();
        }
    }
}