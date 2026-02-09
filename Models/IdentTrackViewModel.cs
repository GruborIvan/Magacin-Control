using CSS_MagacinControl_App.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace CSS_MagacinControl_App.Models
{
    public class IdentTrackViewModel
    {
        public List<FaktureViewModel> FaktureState { get; set; }
        public List<IdentiViewModel> IdentState { get; set; }
        public Dictionary<string, string> BarcodeToIdentDictionary { get; set; }
    }
}