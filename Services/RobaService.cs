﻿using CSS_MagacinControl_App.Interfaces;
using CSS_MagacinControl_App.Models;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSS_MagacinControl_App.Services
{
    public class RobaService : IRobaService
    {
        private readonly IRobaRepository _robaRepository;

        public RobaService(IRobaRepository robaRepository)
        {
            _robaRepository = robaRepository;
        }

        public async Task SaveFaktura(FaktureIdentiViewModel faktureIdenti)
        {
            await _robaRepository.SaveFakturaAsync(faktureIdenti.FaktureViewModel.First());
        }

        public async Task<IEnumerable<string>> GetBrojeviFaktureAsync()
        {
            return await _robaRepository.GetBrojeviFakturaAsync();
        }

        public async Task<FaktureIdentiViewModel> GetFilteredFaktureAsync(FilterModel filter)
        {
            return await _robaRepository.GetFilteredFakturaDataAsync(filter);
        }

        public List<IdentiViewModel> ValidateIdentScanState(List<IdentiViewModel> identi)
        {
            List<IdentiViewModel> failList = new List<IdentiViewModel>();

            foreach (var ident in identi)
            {
                if (ident.PripremljenaKolicina != ident.KolicinaSaFakture)
                {
                    failList.Add(ident);
                }
            }

            return failList;
        }

        public async Task<bool> CheckIfFakturaExists(string brojFakture)
        {
            return await _robaRepository.CheckIfFakturaAlreadyExistsAsync(brojFakture);
        }

        public async Task<string> GetNazivIdentaByBarcodeAsync(string enteredBarcode)
        {
            return await _robaRepository.GetNazivIdentaByBarcodeAsync(enteredBarcode);
        }

        public async Task SaveFakturaAndItemsAsync(IdentTrackViewModel dataModel)
        {
            bool fakturaAlreadyExists = await _robaRepository.SaveFakturaAsync(dataModel.FaktureState.First());

            if (!fakturaAlreadyExists)
            {
                await _robaRepository.SaveIdentiAsync(dataModel.IdentState);

                List<IdentBarkodDbo> barkodIdentRelations = new List<IdentBarkodDbo>();
                foreach (var kvp in dataModel.BarcodeToIdentDictionary)
                {
                    IdentBarkodDbo idenBarcodeDbo = new IdentBarkodDbo
                    (
                        Guid.NewGuid(),
                        kvp.Value,
                        kvp.Key
                    );
                    barkodIdentRelations.Add(idenBarcodeDbo);
                }

                await _robaRepository.SaveIdentBarcodeRelationAsync(barkodIdentRelations);
            }
            else
            {
                await _robaRepository.UpdateIdentiAsync(dataModel.IdentState);
            }
        }
    }
}