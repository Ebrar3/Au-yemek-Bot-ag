using System.ComponentModel.DataAnnotations;

namespace AuBot.Models;

/// <summary>
/// Öğrencilerin bir menüdeki yemeğe verdiği 1–5 arası puanı ve yorumunu temsil eder.
/// </summary>
public class Degerlendirme
{
    public int Id { get; set; }

    [Display(Name = "Kullanıcı")]
    public string? KullaniciId { get; set; }

    [Display(Name = "Menü")]
    public int MenuId { get; set; }
    public Menu? Menu { get; set; }

    [Display(Name = "Yemek")]
    public int YemekId { get; set; }
    public Yemek? Yemek { get; set; }

    [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
    [Display(Name = "Puan (1-5)")]
    public int Puan { get; set; }

    [StringLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir.")]
    [Display(Name = "Yorum")]
    public string? Yorum { get; set; }

    [Display(Name = "Değerlendirme Tarihi")]
    public DateTime Tarih { get; set; } = DateTime.UtcNow;

    [Display(Name = "Onay Durumu")]
    public bool Onaylandi { get; set; } = true;
}
