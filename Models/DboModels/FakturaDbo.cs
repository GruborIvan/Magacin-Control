using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSS_MagacinControl_App.Models.DboModels
{
    [Table("_css_RobaZaPakovanje_hd")]
    public class FakturaDbo
    {
        [Key]
        public string BrojFakture { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatumFakture { get; set; }
        public string SifraKupca { get; set; }
        public string NazivKupca { get; set; }
        public string StatusFakture { get; set; }
        public ICollection<IdentDbo> RobaZaPakovanjeItems { get; set; }

        public FakturaDbo(string brojFakture, DateTime datumFakture, string sifraKupca, string nazivKupca, string statusFakture)
        {
            BrojFakture = brojFakture;
            DatumFakture = datumFakture;
            SifraKupca = sifraKupca;
            NazivKupca = nazivKupca;
            StatusFakture = statusFakture;
        }

        public FakturaDbo() { }
    }
}