using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Yemekler;

public class DetayModel : PageModel
{
    private readonly AuBotDbContext _context;

    public DetayModel(AuBotDbContext context)
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
}
