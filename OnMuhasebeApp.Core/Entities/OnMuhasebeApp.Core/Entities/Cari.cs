namespace OnMuhasebeApp.Core.Entities
{
    public class Cari
    {
        public int Id { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNo { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }

        // Navigation Properties
        public ICollection<Fatura> Faturalar { get; set; } = new List<Fatura>();
        public ICollection<KasaIslemi> KasaIslemleri { get; set; } = new List<KasaIslemi>();
    }
}