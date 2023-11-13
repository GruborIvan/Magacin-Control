using System.Collections.Generic;

namespace CSS_MagacinControl_App.ViewModels
{
    public class FaktureIdentiViewModel
    {
        public List<FaktureViewModel> FaktureViewModel { get; set; }
        public List<IdentiViewModel> IdentiViewModel { get; set; }
        public Dictionary<string, string> BarcodeToIdentDictionary { get; set; }

        public FaktureIdentiViewModel() { }
    }
}
