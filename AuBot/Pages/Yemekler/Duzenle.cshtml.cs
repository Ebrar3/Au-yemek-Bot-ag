using System.Globalization;
using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Yemekler;

public class DuzenleModel : PageModel
{
    private readonly AuBotDbContext _context;

    public DuzenleModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Yemek DuzenlenenYemek { get; set; } = new();

    public SelectList Kategoriler { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var yemek = await _context.Yemekler.FindAsync(id);
        if (yemek == null)
        {
            return NotFound();
        }

        DuzenlenenYemek = yemek;
        await KategorileriDoldurAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await KategorileriDoldurAsync();
            return Page();
        }

        var yemek = await _context.Yemekler.FindAsync(id);
        if (yemek == null)
        {
            return NotFound();
        }

        yemek.Ad = DuzenlenenYemek.Ad.Trim();
        yemek.AramaAdi = yemek.Ad.ToUpper(new CultureInfo("tr-TR"));
        yemek.KategoriId = DuzenlenenYemek.KategoriId;
        yemek.Kalori = DuzenlenenYemek.Kalori;
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"\"{yemek.Ad}\" yemeği güncellendi.";
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
