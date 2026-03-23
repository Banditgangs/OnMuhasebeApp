using System;
using System.Linq;
using System.Xml.Linq;
using OnMuhasebeApp.Core.Entities;

namespace OnMuhasebeApp.Infrastructure.Services
{
    public class InvoiceService
    {
        // Sabit yol yerine, dışarıdan (UI'dan) gelen dosya yolunu alıyoruz
        public Fatura FaturayiOku(string dosyaYolu)
        {
            try
            {
                // XML dosyasını yüklüyoruz
                XDocument rapor = XDocument.Load(dosyaYolu);
                
                // UBL TR Standart Namespace (İsim Uzayı) Tanımları
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

                // ? işareti ve ?? ile Null (boş) gelme ihtimaline karşı programı çökmeden koruyoruz
                string faturaNo = rapor.Root?.Element(cbc + "ID")?.Value ?? "Fatura No Bulunamadı";
                
                string sirketAdi = rapor.Descendants(cac + "AccountingSupplierParty")
                                        .Descendants(cbc + "Name")
                                        .FirstOrDefault()?.Value ?? "Şirket Adı Bulunamadı";

                string tutarString = rapor.Descendants(cbc + "PayableAmount").FirstOrDefault()?.Value ?? "0";
                
                // Noktalı gelen tutarı (örn: 1500.50), C# ondalık formatına güvenli şekilde çeviriyoruz
                decimal.TryParse(tutarString.Replace(".", ","), out decimal toplamTutar);

                // Okunan verileri Fatura nesnesi olarak paketleyip geriye döndürüyoruz
                return new Fatura
                {
                    FaturaNo = faturaNo,
                    SirketAdi = sirketAdi,
                    ToplamTutar = toplamTutar
                };
            }
            catch (Exception ex)
            {
                // Eğer seçilen dosya geçerli bir XML değilse bu hata fırlatılır
                throw new Exception("GİB e-Fatura XML formatı okunamadı!\nDetay: " + ex.Message);
            }
        }
    }
}