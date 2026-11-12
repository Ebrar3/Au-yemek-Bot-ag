using System.ComponentModel.DataAnnotations;

namespace AuBot.Models;

/// <summary>
/// Yönetici panelinden yapılan aylık Excel menü yüklemelerinin geçmişini ve ayrıştırma uyarılarını tutar.
/// </summary>
public class MenuYukleme
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dosya adı zorunludur.")]
    [StringLength(260)]
    [Display(Name = "Dosya Adı")]
    public string DosyaAdi { get; set; } = string.Empty;

    [Range(1, 12)]
    [Display(Name = "Ay")]
    public int Ay { get; set; }

    [Range(2020, 2035)]
    [Display(Name = "Yıl")]
    public int Yil { get; set; }

    [Display(Name = "Öğün Türü")]
    public OgunTuru Ogun { get; set; }

    [StringLength(450)]
    [Display(Name = "Yükleyen Kullanıcı")]
    public string? YukleyenKullaniciId { get; set; }

    [Display(Name = "Yükleme Tarihi")]
    public DateTime YuklemeTarihi { get; set; } = DateTime.Now;

    [Display(Name = "Eklenen Menü Sayısı")]
    public int EklenenMenuSayisi { get; set; }

    [Display(Name = "Güncellenen Menü Sayısı")]
    public int GuncellenenMenuSayisi { get; set; }

    [Display(Name = "Tatil Gün Sayısı")]
    public int TatilGunSayisi { get; set; }

    [Display(Name = "Yeni Yemek Sayısı")]
    public int YeniYemekSayisi { get; set; }

    /// <summary>
    /// Excel ayrıştırma sırasında tespit edilen hatalı tarih veya kalori uyumsuzluğu uyarıları (satır satır).
    /// </summary>
    [Display(Name = "Uyarılar")]
    public string? Uyarilar { get; set; }

    public ICollection<Menu> Menuler { get; set; } = new List<Menu>();
}
