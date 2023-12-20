using CSS_MagacinControl_App.Models;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Interfaces
{
    public interface IRobaRepository
    {
        Task<IEnumerable<string>> GetBrojeviFakturaAsync();
        Task<FaktureViewModel> GetSingleFakturaDataAsync(string brojFakture);
        Task<IEnumerable<IdentiViewModel>> GetIdentiForFakturaAsync(string brojFakture);
        Task<FaktureIdentiViewModel> GetFilteredFakturaDataAsync(FilterModel filter);
        Task<bool> SaveFakturaAsync(FaktureViewModel faktura);
        Task SaveIdentiAsync(List<IdentiViewModel> identi);
        Task UpdateIdentiAsync(List<IdentiViewModel> identi, string brojFakture);
        Task SaveIdentBarcodeRelationAsync(List<IdentBarkodDbo> identBarkodRelations);
        Task<bool> CheckIfFakturaAlreadyExistsAsync(string brojFakture);
        Task<string> GetSifraIdentaByBarcodeAsync(string enteredBarcode);
        Task ChangeFakturaStatusToDoneAsync(string brojFakture);
    }
}