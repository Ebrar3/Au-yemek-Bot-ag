namespace AuBot.Models
{
    public class Yemek
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string AramaAdi { get; set; } = string.Empty;

        public int? KategoriId { get; set; }
        public Kategori? Kategori { get; set; }

        public int? Kalori { get; set; }

        public ICollection<MenuYemek> MenuYemekleri { get; set; } = new List<MenuYemek>();
    }
}
