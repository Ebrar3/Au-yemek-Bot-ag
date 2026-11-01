namespace AuBot.Models
{
    public class Kategori
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public int? Sira { get; set; }

        public ICollection<Yemek> Yemekler { get; set; } = new List<Yemek>();
    }
}
