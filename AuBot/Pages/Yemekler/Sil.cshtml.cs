using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Yemekler;

public class SilModel : PageModel
{
    private readonly AuBotDbContext _context;

    public SilModel(AuBotDbContext context)
    {
        _context = context;
    }

    public Yemek SeciliYemek { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var yemek = await _context.Yemekler
            .Include(y => y.Kategori)
            .FirstOrDefaultAsync(y => y.Id == id);

        if (yemek == null)
        {
            return NotFound();
        }

        SeciliYemek = yemek;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var yemek = await _context.Yemekler.FindAsync(id);
        if (yemek == null)
        {
            return NotFound();
        }

        _context.Yemekler.Remove(yemek);
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"\"{yemek.Ad}\" yemeği silindi.";
        return RedirectToPage("Index");
    }
}
