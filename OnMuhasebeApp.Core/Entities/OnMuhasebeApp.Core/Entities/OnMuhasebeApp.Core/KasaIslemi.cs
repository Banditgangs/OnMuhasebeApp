namespace OnMuhasebeApp.Core.Entities
{
    public class KasaIslemi
    {
        public int Id { get; set; }
        public int? CariId { get; set; } // Cari bağımsız genel gider/gelir olabilme ihtimali için nullable
        public DateTime Tarih { get; set; }
        public decimal Tutar { get; set; }
        public string IslemTuru { get; set; } // Örn: Tahsilat, Ödeme
        public string Aciklama { get; set; }

        // Navigation Properties
        public Cari Cari { get; set; }
    }
}