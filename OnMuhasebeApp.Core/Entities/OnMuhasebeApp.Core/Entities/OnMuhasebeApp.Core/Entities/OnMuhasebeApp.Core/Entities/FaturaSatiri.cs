namespace OnMuhasebeApp.Core.Entities
{
    public class FaturaSatiri
    {
        public int Id { get; set; }
        public int FaturaId { get; set; }
        public string UrunAdi { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal KdvOrani { get; set; }
        public decimal SatirToplami { get; set; }

        // Navigation Properties
        public Fatura Fatura { get; set; }
    }
}