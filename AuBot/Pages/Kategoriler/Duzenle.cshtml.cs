using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuBot.Pages.Kategoriler;

public class DuzenleModel : PageModel
{
    private readonly AuBotDbContext _context;

    public DuzenleModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Kategori DuzenlenenKategori { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var kategori = await _context.Kategoriler.FindAsync(id);
        if (kategori == null)
        {
            return NotFound();
        }

        DuzenlenenKategori = kategori;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var kategori = await _context.Kategoriler.FindAsync(id);
        if (kategori == null)
        {
            return NotFound();
        }

        kategori.Ad = DuzenlenenKategori.Ad.Trim();
        kategori.Sira = DuzenlenenKategori.Sira;
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"\"{kategori.Ad}\" kategorisi güncellendi.";
        return RedirectToPage("Index");
    }
}
