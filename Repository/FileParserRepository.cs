using AutoMapper;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models.CsvModels;
using CSS_MagacinControl_App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Repository
{
    public class FileParserRepository : IFileParserRepository
    {
        private readonly string _sep = "\t";
        private readonly IMapper _mapper;


        public FileParserRepository(IMapper mapper) 
        {
            _mapper = mapper;
        }

        public async Task<FaktureViewModel> ReadFakturaHeaderFromCsvFileAsync(string fileName)
        {
            string fakturaTextContent = await File.ReadAllTextAsync(fileName);

            string[] objData = fakturaTextContent.Split(_sep.ToCharArray());

            var fakturaCsv = new FakturaCsv()
            {
                BrojFakture = objData[0],
                DatumFakture = ConvertDateToRightFormat(objData[1]),
                SifraKupca = objData[2],
                NazivKupca = objData[3],
            };

            var faktura = _mapper.Map<FaktureViewModel>(fakturaCsv);

            return faktura;
        }

        public async Task<List<IdentiViewModel>> ReadIdentiFromCsvFileAsync(string identiFileName, string kolicineFileName, string brojFakture)
        {
            string line;
            var identi = new List<IdentiViewModel>();

            var kolicineList = await ReadKolicineFromCsvFileAsync(kolicineFileName);

            FileStream fileStream = new FileStream(identiFileName, FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] lineDataItems = line.Split(_sep.ToCharArray());

                    var identCsv = new IdentiCsv()
                    {
                        SifraIdenta = lineDataItems[0],
                        Barkod = lineDataItems[1],
                        NazivIdenta = lineDataItems[2],
                        Oznaka = Convert.ToInt32(lineDataItems[3]),
                        BrojFakture = brojFakture
                    };

                    var identViewModel = _mapper.Map<IdentiViewModel>(identCsv);
                    identViewModel.KolicinaSaFakture = kolicineList.Where(x => x.SifraIdenta == identCsv.SifraIdenta).FirstOrDefault().Kolicina;
                    identViewModel.Razlika = identViewModel.KolicinaSaFakture - identViewModel.PripremljenaKolicina;

                    identi.Add(identViewModel);
                }
            }

            return identi;
        }

        private async Task<List<KolicineCsv>> ReadKolicineFromCsvFileAsync(string fileName)
        {
            string line;
            var kolicine = new List<KolicineCsv>();

            FileStream fileStream = new FileStream(fileName, FileMode.Open);

            using (StreamReader reader = new StreamReader(fileStream))
            {
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] lineDataItems = line.Split(_sep.ToCharArray());

                    var kolicinaItem = new KolicineCsv()
                    {
                        SifraIdenta = lineDataItems[0],
                        Kolicina = Convert.ToInt32(lineDataItems[1]),
                    };

                    kolicine.Add(kolicinaItem);
                }
            }

            return kolicine;
        }

        private string ConvertDateToRightFormat(string oldDate)
        {
            string[] dateParts = oldDate.Split('.');
            DateTime date = new DateTime(
                Convert.ToInt32(dateParts[2]),
                Convert.ToInt32(dateParts[1]),
                Convert.ToInt32(dateParts[0])
            );

            return date.ToString("dd/MM/yyyy");
        }

        public void CreateOutputDirectoryIfNotExists(string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
        }
    }
}
