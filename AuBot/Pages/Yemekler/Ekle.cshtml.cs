using System.Globalization;
using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Yemekler;

public class EkleModel : PageModel
{
    private readonly AuBotDbContext _context;

    public EkleModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Yemek YeniYemek { get; set; } = new();

    public SelectList Kategoriler { get; set; } = default!;

    public async Task OnGetAsync()
    {
        await KategorileriDoldurAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await KategorileriDoldurAsync();
            return Page();
        }

        YeniYemek.Ad = YeniYemek.Ad.Trim();
        YeniYemek.AramaAdi = YeniYemek.Ad.ToUpper(new CultureInfo("tr-TR"));

        _context.Yemekler.Add(YeniYemek);
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"\"{YeniYemek.Ad}\" yemeği başarıyla eklendi.";
        return RedirectToPage("Index");
    }

    private async Task KategorileriDoldurAsync()
    {
        var kategoriler = await _context.Kategoriler
            .OrderBy(k => k.Sira)
            .ThenBy(k => k.Ad)
            .ToListAsync();
        Kategoriler = new SelectList(kategoriler, "Id", "Ad");
    }
}
