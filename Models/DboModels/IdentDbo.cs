using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSS_MagacinControl_App.Models.DboModels
{
    [Table("_css_RobaZaPakovanje_item")]
    public class IdentDbo
    {
        public Guid Id { get; set; }
        public string SifraIdenta { get; set; }
        public FakturaDbo RobaZaPakovanje { get; set; }

        [ForeignKey("RobaZaPakovanje")]
        public string BrojFakture { get; set; } 
        public string NazivIdenta { get; set; }
        public int KolicinaSaFakture { get; set; }
        public int PrimljenaKolicina { get; set; }

        public IdentDbo(Guid id, string sifraIdenta, string brojFakture, string nazivIdenta, int kolicinaSaFakture, int primljenaKolicina)
        {
            Id = id;
            SifraIdenta = sifraIdenta;
            BrojFakture = brojFakture;
            NazivIdenta = nazivIdenta;
            KolicinaSaFakture = kolicinaSaFakture;
            PrimljenaKolicina = primljenaKolicina;
        }

        public IdentDbo() { }
    }
}