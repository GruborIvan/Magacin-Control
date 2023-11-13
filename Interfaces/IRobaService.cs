using CSS_MagacinControl_App.Models;
using CSS_MagacinControl_App.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Interfaces
{
    public interface IRobaService
    {
        Task<IEnumerable<string>> GetBrojeviFaktureAsync();
        Task<FaktureIdentiViewModel> GetFilteredFaktureAsync(FilterModel filter);
        List<IdentiViewModel> ValidateIdentScanState(List<IdentiViewModel> identi);
        Task SaveFaktura(FaktureIdentiViewModel faktureIdenti);
        Task<bool> CheckIfFakturaExists(string brojFakture);
        Task<string> GetNazivIdentaByBarcodeAsync(string enteredBarcode);
        Task SaveFakturaAndItemsAsync(IdentTrackViewModel dataModel);
    }
}