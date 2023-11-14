using CSS_MagacinControl_App.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Interfaces
{
    public interface IFileParser
    {
        Task<FaktureIdentiViewModel> ReadDataFromCsvFilesAsync(List<string> fileNames);
        List<string> ValidateFileNames(List<string> fileNames);
        void PackFaktureToCsvFile(FaktureViewModel faktura);
    }
}