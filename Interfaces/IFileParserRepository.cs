using CSS_MagacinControl_App.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Interfaces
{
    public interface IFileParserRepository
    {
        Task<FaktureViewModel> ReadFakturaHeaderFromCsvFileAsync(string fileName);
        Task<List<IdentiViewModel>> ReadIdentiFromCsvFileAsync(string identiFileName, string kolicineFileName, string brojFakture);
        void CreateOutputDirectoryIfNotExists(string outputPath);
        void CorrectAmountsForServices(List<IdentiViewModel> identi);
        List<IdentiViewModel> FilterOutDuplicates(List<IdentiViewModel> identiViewModel);
    }
}