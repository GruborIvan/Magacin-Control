using System.Collections.Generic;

namespace CSS_MagacinControl_App.ViewModels
{
    public class SingleFakturaViewModel
    {
        public FaktureViewModel Faktura {  get; set; }
        public IEnumerable<IdentiViewModel> Identi { get; set; }

        public SingleFakturaViewModel()
        {

        }
    }
}