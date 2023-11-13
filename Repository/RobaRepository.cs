using AutoMapper;
using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Repository
{
    public class RobaRepository : IRobaRepository
    {
        public AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public RobaRepository(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<string>> GetBrojeviFakturaAsync()
        {
            return await _dbContext.RobaZaPakovanje.Select(x => x.BrojFakture).ToArrayAsync();
        }

        public async Task<FaktureViewModel> GetSingleFakturaDataAsync(string brojFakture)
        {
            var fakturaDbo = await _dbContext.RobaZaPakovanje.Where(x => x.BrojFakture == brojFakture).FirstOrDefaultAsync();

            return _mapper.Map<FaktureViewModel>(fakturaDbo);
        }

        public async Task<IEnumerable<IdentiViewModel>> GetIdentiForFakturaAsync(string brojFakture)
        {
            var fakturaItemsDbo = await _dbContext.RobaZaPakovanjeItem.Where(x => x.BrojFakture == brojFakture).ToListAsync();

            return _mapper.Map<IEnumerable<IdentiViewModel>>(fakturaItemsDbo);
        }

        public async Task<FaktureIdentiViewModel> GetFilteredFakturaDataAsync(FilterModel filter)
        {
            var query = from faktura in _dbContext.RobaZaPakovanje
                        select faktura;

            // Filter query by 'BrojFakture'.
            if (!String.IsNullOrEmpty(filter.BrojFakture))
                query = query.Where(x => x.BrojFakture == filter.BrojFakture);

            // Try to filter query by date => LATER

            if (!String.IsNullOrEmpty(filter.PocetniDatum))
            {
                var date = DateTime.Parse(filter.PocetniDatum);
                query = query.Where(x => x.DatumFakture >= date);
            }

            // Filter query by 'Status fakture'.
            if (!String.IsNullOrEmpty(filter.StatusFakture))
                query = query.Where(x => x.StatusFakture == filter.StatusFakture);

            var result = await query
                .Include(x => x.RobaZaPakovanjeItems)
                .ToListAsync();

            var identDbos = result.Select(x => x.RobaZaPakovanjeItems)
                .SelectMany(x => x)
                .ToList();

            var barkodIdentDbo = await _dbContext.IdentBarkod.ToListAsync();

            var barCodeIdentDict = barkodIdentDbo.ToDictionary(x => x.BarkodIdenta, x => x.NazivIdenta);

            var fakture = _mapper.Map<List<FaktureViewModel>>(result);

            var idents = _mapper.Map<List<IdentiViewModel>>(identDbos);

            return new FaktureIdentiViewModel()
            {
                FaktureViewModel = fakture,
                IdentiViewModel = idents,
                BarcodeToIdentDictionary = barCodeIdentDict,
            };
        }

        public async Task<bool> SaveFakturaAsync(FaktureViewModel faktura)
        {
            var checkFaktura = await _dbContext.RobaZaPakovanje.FindAsync(faktura.BrojFakture);

            if (checkFaktura == null)
            {
                var fakturaDbo = _mapper.Map<FakturaDbo>(faktura);

                await _dbContext.AddAsync(fakturaDbo);
                await _dbContext.SaveChangesAsync();
                return false;
            }

            return true;
        }

        public async Task<bool> CheckIfFakturaAlreadyExistsAsync(string brojFakture)
        {
            var faktura = await _dbContext.RobaZaPakovanje.Where(x => x.BrojFakture == brojFakture).ToListAsync();
            return faktura.Count > 0;
        }

        public async Task<string> GetNazivIdentaByBarcodeAsync(string enteredBarcode)
        {
            var identFilter = await _dbContext.IdentBarkod
                              .Where(x => x.BarkodIdenta == enteredBarcode)
                              .ToListAsync();

            if (identFilter.Count() == 0)
                return null;

            return identFilter.First().NazivIdenta;
        }

        public async Task SaveIdentiAsync(List<IdentiViewModel> identi)
        {
            var identiListDbo = _mapper.Map<IEnumerable<IdentDbo>>(identi);

            await _dbContext.RobaZaPakovanjeItem.AddRangeAsync(identiListDbo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveIdentBarcodeRelationAsync(List<IdentBarkodDbo> identBarkodRelations)
        {
            var identBarCodes = _dbContext.IdentBarkod.Select(x => x.BarkodIdenta).ToList();

            var addList = identBarkodRelations.Where(x => !identBarCodes.Contains(x.BarkodIdenta)).ToList();

            await _dbContext.IdentBarkod.AddRangeAsync(addList);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateIdentiAsync(List<IdentiViewModel> identi)
        {
            var identiListDbo = _mapper.Map<IEnumerable<IdentDbo>>(identi);
            List<IdentDbo> identiList = _dbContext.RobaZaPakovanjeItem.ToList();

            foreach (var ident in identiList)
            {
                var changedIdent = identiListDbo.Where(x => x.SifraIdenta == ident.SifraIdenta)
                                         .Where(y => y.NazivIdenta == ident.NazivIdenta)
                                         .ToList()
                                         .FirstOrDefault();

                if (changedIdent != null)
                {
                    ident.PrimljenaKolicina = changedIdent.PrimljenaKolicina;
                    _dbContext.Entry(ident).State = EntityState.Modified;
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task ChangeFakturaStatusToDoneAsync(string brojFakture)
        {
            var checkFaktura = await _dbContext.RobaZaPakovanje.FindAsync(brojFakture);
            checkFaktura.StatusFakture = "Završeno";

            _dbContext.Entry<FakturaDbo>(checkFaktura).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}