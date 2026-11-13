using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuBot.Pages.Hesap;

/// <summary>
/// Kullanıcı çıkış işlemi. POST ile çağrılır, oturumu kapatır ve ana sayfaya yönlendirir.
/// </summary>
public class CikisModel : PageModel
{
    private readonly SignInManager<IdentityUser> _girisYoneticisi;

    public CikisModel(SignInManager<IdentityUser> girisYoneticisi)
    {
        _girisYoneticisi = girisYoneticisi;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _girisYoneticisi.SignOutAsync();
        TempData["Basari"] = "Oturumunuz başarıyla kapatıldı. Görüşmek üzere!";
        return RedirectToPage("/Index");
    }
}
