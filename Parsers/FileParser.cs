using CSS_MagacinControl_App.Dialog;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models.CsvModels;
using CSS_MagacinControl_App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Parsers
{
    public class FileParser : IFileParser
    {
        private readonly string _identiFileExtension = "_code.txt";
        private readonly string _kolicineFileExtension = "_item.txt";
        private DialogHandler _dialogHandler;

        private readonly IFileParserRepository _fileParserRepository;

        public FileParser(IFileParserRepository fileParserRepository)
        {
            _fileParserRepository = fileParserRepository;
            _dialogHandler = new DialogHandler();
        }

        public async Task<FaktureIdentiViewModel> ReadDataFromCsvFilesAsync(List<string> fileNames)
        {
            try
            {
                var identiFileName = fileNames.Where(x => x.EndsWith(_identiFileExtension)).FirstOrDefault();
                var faktureFileName = identiFileName.Substring(0, identiFileName.Length - 9) + ".txt";
                var kolicineFileName = fileNames.Where(x => x.EndsWith(_kolicineFileExtension)).FirstOrDefault();

                var faktureViewModel = await _fileParserRepository.ReadFakturaHeaderFromCsvFileAsync(faktureFileName);
                var identiViewModel = await _fileParserRepository.ReadIdentiFromCsvFileAsync(identiFileName, kolicineFileName, faktureViewModel.BrojFakture);

                var barCodeToNazivIdenta = identiViewModel
                                           .Select(o => new BarkodMap(o.Barkod, o.NazivIdenta))
                                           .Distinct()
                                           .ToList();

                var brojPonavljanjaIstogIdenta = barCodeToNazivIdenta
                                        .GroupBy(x => x.Barkod)
                                        .Select(group => new CsvParseErrorModel
                                        {
                                            Value = group.Key,
                                            Count = group.Count(),
                                            NaziviIdenta = barCodeToNazivIdenta.Where(x => x.Barkod == group.Key)
                                                                               .Select(x => x.NazivIdenta)
                                                                               .ToList()
                                        })
                                        .Where(x => x.Count > 1)
                                        .ToList();

                // Handle error in file.
                if (brojPonavljanjaIstogIdenta.Count > 0)
                {
                    _dialogHandler.GetBarcodeAlreadyAssignedToIdentDialog(brojPonavljanjaIstogIdenta);
                    return null;
                }

                var barKodMapDictionary = barCodeToNazivIdenta.ToDictionary(x => x.Barkod, x => x.NazivIdenta);

                // UNCOMMENT LATER.
                //fileNames.ForEach(fileName => { File.Delete(fileName); });

                return new FaktureIdentiViewModel()
                {
                    FaktureViewModel = new List<FaktureViewModel>() { faktureViewModel },
                    IdentiViewModel = identiViewModel,
                    BarcodeToIdentDictionary = barKodMapDictionary
                };
            }
            catch (Exception e)
            {
                _dialogHandler.GetErrorWhileLoadingFilesDialog();
                return null;
            }
        }

        public List<string> ValidateFileNames(List<string> fileNames)
        {
            if (fileNames.Count == 1)
            {
                var fileName = fileNames[0];
                
                var baseFileName = fileName.Substring(0,fileName.Length - 4);

                string itemFileName = baseFileName + _kolicineFileExtension;
                string codeFileName = baseFileName + _identiFileExtension;
                return new List<string> { fileName, codeFileName, itemFileName };
            }
            else if (fileNames.Count == 3)
            {
                return fileNames;
            }
            else
            {
                _dialogHandler.GetWrongFileNumberSelectDialog(fileNames.Count);
                return null;
            }
        }
    }
}