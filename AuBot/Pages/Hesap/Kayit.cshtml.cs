using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuBot.Pages.Hesap;

/// <summary>
/// Yeni kullanıcı kayıt sayfası. Kayıt olan kullanıcıya otomatik "Ogrenci" rolü atanır.
/// </summary>
public class KayitModel : PageModel
{
    private readonly UserManager<IdentityUser> _kullaniciYoneticisi;
    private readonly SignInManager<IdentityUser> _girisYoneticisi;

    public KayitModel(UserManager<IdentityUser> kullaniciYoneticisi,
                      SignInManager<IdentityUser> girisYoneticisi)
    {
        _kullaniciYoneticisi = kullaniciYoneticisi;
        _girisYoneticisi = girisYoneticisi;
    }

    [BindProperty]
    public KayitGirisModeli GirisModeli { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var yeniKullanici = new IdentityUser
        {
            UserName = GirisModeli.Email,
            Email = GirisModeli.Email,
            EmailConfirmed = true
        };

        var sonuc = await _kullaniciYoneticisi.CreateAsync(yeniKullanici, GirisModeli.Sifre);

        if (sonuc.Succeeded)
        {
            // Kayıt olan her yeni kullanıcıya varsayılan olarak Öğrenci rolü verilir
            await _kullaniciYoneticisi.AddToRoleAsync(yeniKullanici, "Ogrenci");
            await _girisYoneticisi.SignInAsync(yeniKullanici, isPersistent: false);

            TempData["Basari"] = "Hesabınız başarıyla oluşturuldu. Hoş geldiniz!";
            return RedirectToPage("/Index");
        }

        // Hata mesajlarını ModelState'e ekle
        foreach (var hata in sonuc.Errors)
            ModelState.AddModelError(string.Empty, hata.Description);

        return Page();
    }

    public class KayitGirisModeli
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Sifre), ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre Tekrar")]
        public string SifreTekrar { get; set; } = string.Empty;
    }
}
