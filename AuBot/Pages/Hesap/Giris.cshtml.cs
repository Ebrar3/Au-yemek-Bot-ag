using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuBot.Pages.Hesap;

/// <summary>
/// Kullanıcı giriş sayfası. E-posta ve şifre ile kimlik doğrulama yapılır.
/// </summary>
public class GirisModel : PageModel
{
    private readonly SignInManager<IdentityUser> _girisYoneticisi;

    public GirisModel(SignInManager<IdentityUser> girisYoneticisi)
    {
        _girisYoneticisi = girisYoneticisi;
    }

    [BindProperty]
    public GirisGirisModeli GirisModeli { get; set; } = new();

    // Demo sayfasında gösterilecek örnek hesap bilgileri
    public List<DemoHesap> DemoHesaplari { get; } = new()
    {
        new DemoHesap { Rol = "Yönetici", Email = "admin@aubot.edu.tr", Sifre = "Admin123" },
        new DemoHesap { Rol = "Öğrenci",  Email = "ogrenci@aubot.edu.tr", Sifre = "Ogrenci123" }
    };

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
            return Page();

        var sonuc = await _girisYoneticisi.PasswordSignInAsync(
            GirisModeli.Email,
            GirisModeli.Sifre,
            GirisModeli.BeniHatirla,
            lockoutOnFailure: false);

        if (sonuc.Succeeded)
        {
            TempData["Basari"] = "Başarıyla giriş yaptınız. Hoş geldiniz!";
            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre. Lütfen tekrar deneyin.");
        return Page();
    }

    public class GirisGirisModeli
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = string.Empty;

        [Display(Name = "Beni Hatırla")]
        public bool BeniHatirla { get; set; }
    }

    public record DemoHesap
    {
        public string Rol { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Sifre { get; init; } = string.Empty;
    }
}
