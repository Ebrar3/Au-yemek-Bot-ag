using System.ComponentModel.DataAnnotations;

namespace AuBot.Models;

/// <summary>
/// Yemekhane öğün türleri (1: Öğle, 2: Akşam).
/// </summary>
public enum OgunTuru
{
    [Display(Name = "Öğle Yemeği")]
    Ogle = 1,

    [Display(Name = "Akşam Yemeği")]
    Aksam = 2
}

/// <summary>
/// Belirli bir tarih ve öğün için yayınlanan günlük yemekhane menüsünü temsil eder.
/// </summary>
public class Menu
{
    public int Id { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Menü Tarihi")]
    public DateOnly Tarih { get; set; }

    [Display(Name = "Öğün Türü")]
    public OgunTuru Ogun { get; set; } = OgunTuru.Ogle;

    [Range(0, 10000, ErrorMessage = "Toplam kalori değeri 0 ile 10000 kkal arasında olmalıdır.")]
    [Display(Name = "Toplam Kalori (kkal)")]
    public int? ToplamKalori { get; set; }

    [Display(Name = "Resmi Tatil / Hizmet Dışı")]
    public bool TatilMi { get; set; }

    [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Display(Name = "Menü Yükleme Kaydı")]
    public int? MenuYuklemeId { get; set; }

    public MenuYukleme? MenuYukleme { get; set; }

    public ICollection<MenuYemek> MenuYemekleri { get; set; } = new List<MenuYemek>();
    public ICollection<Degerlendirme> Degerlendirmeler { get; set; } = new List<Degerlendirme>();
}
