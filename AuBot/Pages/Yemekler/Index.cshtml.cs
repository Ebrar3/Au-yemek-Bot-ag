using System.Globalization;
using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Yemekler;

public class IndexModel : PageModel
{
    private readonly AuBotDbContext _context;

    public IndexModel(AuBotDbContext context)
    {
        _context = context;
    }

    public List<Yemek> Yemekler { get; set; } = new();
    public SelectList Kategoriler { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? Arama { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? KategoriId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Siralama { get; set; }

    public async Task OnGetAsync()
    {
        IQueryable<Yemek> sorgu = _context.Yemekler.Include(y => y.Kategori);

        if (!string.IsNullOrWhiteSpace(Arama))
        {
            var arananBuyuk = Arama.Trim().ToUpper(new CultureInfo("tr-TR"));
            sorgu = sorgu.Where(y => y.Ad.Contains(Arama) || y.AramaAdi.Contains(arananBuyuk));
        }

        if (KategoriId != null)
        {
            sorgu = sorgu.Where(y => y.KategoriId == KategoriId);
        }

        sorgu = Siralama switch
        {
            "kalori_artan" => sorgu.OrderBy(y => y.Kalori).ThenBy(y => y.Ad),
            "kalori_azalan" => sorgu.OrderByDescending(y => y.Kalori).ThenBy(y => y.Ad),
            _ => sorgu.OrderBy(y => y.Ad)
        };

        Yemekler = await sorgu.ToListAsync();

        var kategoriler = await _context.Kategoriler
            .OrderBy(k => k.Sira)
            .ThenBy(k => k.Ad)
            .ToListAsync();
        Kategoriler = new SelectList(kategoriler, "Id", "Ad");
    }
}
