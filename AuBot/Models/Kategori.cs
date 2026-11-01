using System.ComponentModel.DataAnnotations;

namespace AuBot.Models;

public class Kategori
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Kategori adı 2 ile 60 karakter arasında olmalıdır.")]
    [Display(Name = "Kategori Adı")]
    public string Ad { get; set; } = string.Empty;

    /// <summary>
    /// Excel menüsündeki sütun sırası (1: Çorba, 2: Ana Yemek, 3: Yardımcı Yemek, 4: Tatlı/Salata/İçecek).
    /// </summary>
    [Required(ErrorMessage = "Menü sütun sırası zorunludur.")]
    [Range(1, 10, ErrorMessage = "Sıra değeri 1 ile 10 arasında olmalıdır.")]
    [Display(Name = "Menü Sütun Sırası")]
    public int? Sira { get; set; } = 1;

    public List<Yemek> Yemekler { get; set; } = new();
}
