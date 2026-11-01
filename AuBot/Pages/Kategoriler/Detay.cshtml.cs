using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Kategoriler;

public class DetayModel : PageModel
{
    private readonly AuBotDbContext _context;

    public DetayModel(AuBotDbContext context)
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
}
