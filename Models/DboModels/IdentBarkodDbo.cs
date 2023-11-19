using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSS_MagacinControl_App.Models.DboModels
{
    [Table("_css_IdentBarKod")]
    public class IdentBarkodDbo
    {
        public Guid Id { get; set; }
        public string NazivIdenta { get; set; }
        public string BarkodIdenta { get; set; }
        [Timestamp]
        public byte[] Timestamp { get; set; }

        public IdentBarkodDbo(Guid id, string nazivIdenta, string barkodIdenta)
        {
            Id = id;
            NazivIdenta = nazivIdenta;
            BarkodIdenta = barkodIdenta;
        }
    }
}