using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Kategoriler;

public class SilModel : PageModel
{
    private readonly AuBotDbContext _context;

    public SilModel(AuBotDbContext context)
    {
        _context = context;
    }

    public Kategori SeciliKategori { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var kategori = await _context.Kategoriler
            .Include(k => k.Yemekler)
            .FirstOrDefaultAsync(k => k.Id == id);

        if (kategori == null)
        {
            return NotFound();
        }

        SeciliKategori = kategori;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var kategori = await _context.Kategoriler
            .Include(k => k.Yemekler)
            .FirstOrDefaultAsync(k => k.Id == id);

        if (kategori == null)
        {
            return NotFound();
        }

        if (kategori.Yemekler.Any())
        {
            TempData["Hata"] = $"\"{kategori.Ad}\" kategorisine bağlı yemekler olduğu için silinemedi.";
            return RedirectToPage("Index");
        }

        _context.Kategoriler.Remove(kategori);
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"\"{kategori.Ad}\" kategorisi silindi.";
        return RedirectToPage("Index");
    }
}
