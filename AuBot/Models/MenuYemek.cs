using System.ComponentModel.DataAnnotations;

namespace AuBot.Models;

/// <summary>
/// Menü ile Yemek arasındaki çoka-çok (N–N) ilişkiyi kuran ara tablo modeli.
/// </summary>
public class MenuYemek
{
    public int MenuId { get; set; }
    public Menu? Menu { get; set; }

    public int YemekId { get; set; }
    public Yemek? Yemek { get; set; }

    /// <summary>
    /// Yemeğin o günkü menü içerisindeki sırası (1: Çorba, 2: Ana Yemek, 3: Yardımcı Yemek, 4: Tatlı/Salata).
    /// </summary>
    [Range(1, 10)]
    [Display(Name = "Menü Sırası")]
    public int Sira { get; set; }
}
