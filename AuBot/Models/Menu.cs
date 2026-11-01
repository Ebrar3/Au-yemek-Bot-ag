namespace AuBot.Models
{
    public enum OgunTuru
    {
        Ogle = 1,
        Aksam = 2
    }

    public class Menu
    {
        public int Id { get; set; }
        public DateOnly Tarih { get; set; }
        public OgunTuru Ogun { get; set; } = OgunTuru.Ogle;
        public int? ToplamKalori { get; set; }
        public bool TatilMi { get; set; }
        public string? Aciklama { get; set; }

        public ICollection<MenuYemek> MenuYemekleri { get; set; } = new List<MenuYemek>();
    }
}
