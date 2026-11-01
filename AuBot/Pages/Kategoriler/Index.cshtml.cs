using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Kategoriler;

public class IndexModel : PageModel
{
    private readonly AuBotDbContext _context;

    public IndexModel(AuBotDbContext context)
    {
        _context = context;
    }

    public List<Kategori> Kategoriler { get; set; } = new();

    public async Task OnGetAsync()
    {
        Kategoriler = await _context.Kategoriler
            .Include(k => k.Yemekler)
            .OrderBy(k => k.Sira)
            .ThenBy(k => k.Ad)
            .ToListAsync();
    }
}
