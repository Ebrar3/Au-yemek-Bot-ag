using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuBot.Pages.Kategoriler;

public class EkleModel : PageModel
{
    private readonly AuBotDbContext _context;

    public EkleModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Kategori YeniKategori { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        YeniKategori.Ad = YeniKategori.Ad.Trim();
        _context.Kategoriler.Add(YeniKategori);
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"\"{YeniKategori.Ad}\" kategorisi başarıyla eklendi.";
        return RedirectToPage("Index");
    }
}
