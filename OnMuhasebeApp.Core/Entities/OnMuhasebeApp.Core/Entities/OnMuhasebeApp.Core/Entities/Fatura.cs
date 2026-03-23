using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnMuhasebeApp.Core.Entities
{
    public class Fatura
    {
        public int Id { get; set; }
        public int CariId { get; set; }
        
        // --- XML'den ve ekrandan gelecek veriler ---
        public string FaturaNo { get; set; }
        
        [NotMapped]
        public string SirketAdi { get; set; } // GİB'den okunan şirket ünvanı için ekledik
        public decimal ToplamTutar { get; set; }
        
        public DateTime Tarih { get; set; }
        public string FaturaTuru { get; set; } // Örn: Alış, Satış

        // --- Navigation Properties (Veritabanı İlişkileri) ---
        public Cari Cari { get; set; }
        public ICollection<FaturaSatiri> FaturaSatirlari { get; set; } = new List<FaturaSatiri>();
    }
}