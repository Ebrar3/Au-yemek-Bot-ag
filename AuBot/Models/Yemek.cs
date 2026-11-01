using System.ComponentModel.DataAnnotations;

namespace AuBot.Models;

public class Yemek
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Yemek adı zorunludur.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Yemek adı 2 ile 150 karakter arasında olmalıdır.")]
    [Display(Name = "Yemek Adı")]
    public string Ad { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Arama Adı (Büyük Harf)")]
    public string AramaAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lütfen bir kategori seçiniz.")]
    [Display(Name = "Kategori")]
    public int? KategoriId { get; set; }

    public Kategori? Kategori { get; set; }

    [Range(0, 3000, ErrorMessage = "Kalori değeri 0 ile 3000 kkal arasında olmalıdır.")]
    [Display(Name = "Kalori (kkal)")]
    public int? Kalori { get; set; }

    public List<MenuYemek> MenuYemekleri { get; set; } = new();
    public List<Degerlendirme> Degerlendirmeler { get; set; } = new();
}
