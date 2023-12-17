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
                        BrojFakture = brojFakture,
                        OznakaUsluge = lineDataItems[4]
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

            var fileStream = new FileStream(fileName, FileMode.Open);

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

                    if (kolicine.Where(x => x.SifraIdenta.Equals(kolicinaItem.SifraIdenta)).Any())
                    {
                        kolicine.Where(x => x.SifraIdenta.Equals(kolicinaItem.SifraIdenta)).FirstOrDefault().Kolicina += kolicinaItem.Kolicina;
                    }
                    else
                    {
                        kolicine.Add(kolicinaItem);
                    }
                }
            }



            return kolicine;
        }

        private string ConvertDateToRightFormat(string oldDate)
        {
            if (oldDate.Contains('.'))
            {
                // Fix double digit day & month.
                string[] dateParts = oldDate.Split('.');
                dateParts[0] = int.Parse(dateParts[0]) < 10 ? $"0{dateParts[0]}" : dateParts[0];
                dateParts[1] = int.Parse(dateParts[1]) < 10 ? $"0{dateParts[1]}" : dateParts[1];

                //Form date string.
                return $"{dateParts[0]}/{dateParts[1]}/{dateParts[2]}";
            }
            else
            {
                return oldDate;
            }
        }

        public void CreateOutputDirectoryIfNotExists(string outputPath)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
        }

        public void CorrectAmountsForServices(List<IdentiViewModel> identi)
        {
            foreach (var ident in identi)
            {
                if (ident.OznakaUsluge.StartsWith("7"))
                {
                    ident.KolicinaSaFakture = 0;
                    ident.PripremljenaKolicina = 0;
                    ident.Razlika = 0;
                }
            }
        }

        public List<IdentiViewModel> FilterOutDuplicates(List<IdentiViewModel> identi)
        {
            var filteredList = new List<IdentiViewModel>();
            var sifreIdenata = identi.Select(x => x.SifraIdenta);

            foreach (var sifraIdenta in sifreIdenata)
            {
                if (!filteredList.Where(x => x.SifraIdenta == sifraIdenta).Any())
                {
                    filteredList.Add(identi.Where(x => x.SifraIdenta == sifraIdenta).First());
                }
            }

            return filteredList;
        }
    }
}