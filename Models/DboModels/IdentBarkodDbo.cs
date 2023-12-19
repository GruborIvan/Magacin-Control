using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSS_MagacinControl_App.Models.DboModels
{
    [Table("_css_IdentBarKod")]
    public class IdentBarkodDbo
    {
        public Guid Id { get; set; }
        public string SifraIdenta { get; set; }
        public string BarkodIdenta { get; set; }

        public IdentBarkodDbo(Guid id, string sifraIdenta, string barkodIdenta)
        {
            Id = id;
            SifraIdenta = sifraIdenta;
            BarkodIdenta = barkodIdenta;
        }
    }
}