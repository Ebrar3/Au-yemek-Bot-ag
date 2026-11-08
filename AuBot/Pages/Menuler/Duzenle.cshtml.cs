using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Menuler;

public class DuzenleModel : PageModel
{
    private readonly AuBotDbContext _context;

    public DuzenleModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int MenuId { get; set; }

    [BindProperty]
    public DateOnly Tarih { get; set; }

    [BindProperty]
    public OgunTuru Ogun { get; set; }

    [BindProperty]
    public bool TatilMi { get; set; }

    [BindProperty]
    public string? Aciklama { get; set; }

    [BindProperty]
    public Dictionary<int, int?> SeciliYemekler { get; set; } = new();

    public List<Kategori> Kategoriler { get; set; } = new();
    public Dictionary<int, List<Yemek>> KategoriYemekleri { get; set; } = new();
    public List<Yemek> TumYemekler { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var menu = await _context.Menuler
            .Include(m => m.MenuYemekleri)
                .ThenInclude(my => my.Yemek)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
            return NotFound();

        MenuId = menu.Id;
        Tarih = menu.Tarih;
        Ogun = menu.Ogun;
        TatilMi = menu.TatilMi;
        Aciklama = menu.Aciklama;

        await YukleAsync();

        // Mevcut yemekleri seçili olarak işaretle
        foreach (var my in menu.MenuYemekleri)
        {
            if (my.Yemek?.KategoriId != null)
            {
                SeciliYemekler[my.Yemek.KategoriId.Value] = my.YemekId;
            }
        }

        // Her kategori için başlangıç değeri (eğer yoksa null)
        foreach (var kat in Kategoriler)
        {
            SeciliYemekler.TryAdd(kat.Id, null);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await YukleAsync();

        if (!TatilMi && SeciliYemekler.All(kv => kv.Value == null))
        {
            ModelState.AddModelError(string.Empty, "En az bir yemek seçmelisiniz.");
            return Page();
        }

        var menu = await _context.Menuler
            .Include(m => m.MenuYemekleri)
            .FirstOrDefaultAsync(m => m.Id == MenuId);

        if (menu == null)
            return NotFound();

        // Temel bilgileri güncelle
        menu.Tarih = Tarih;
        menu.Ogun = Ogun;
        menu.TatilMi = TatilMi;
        menu.Aciklama = Aciklama;

        // Mevcut yemekleri temizle
        _context.MenuYemekleri.RemoveRange(menu.MenuYemekleri);

        if (!TatilMi)
        {
            int sira = 1;
            foreach (var kat in Kategoriler)
            {
                if (SeciliYemekler.TryGetValue(kat.Id, out var yemekId) && yemekId != null)
                {
                    menu.MenuYemekleri.Add(new MenuYemek
                    {
                        MenuId = menu.Id,
                        YemekId = yemekId.Value,
                        Sira = sira
                    });
                }
                sira++;
            }

            // Toplam kalori güncelle
            var yemekIds = menu.MenuYemekleri.Select(my => my.YemekId).ToList();
            var kaloriler = await _context.Yemekler
                .Where(y => yemekIds.Contains(y.Id))
                .Select(y => y.Kalori)
                .ToListAsync();
            menu.ToplamKalori = kaloriler.Sum();
        }
        else
        {
            menu.ToplamKalori = null;
        }

        await _context.SaveChangesAsync();

        TempData["Basari"] = $"{Tarih:dd.MM.yyyy} tarihi için {(Ogun == OgunTuru.Ogle ? "Öğle" : "Akşam")} menüsü güncellendi!";
        return RedirectToPage("/Menuler/Index", new { Yil = Tarih.Year, Ay = Tarih.Month });
    }

    private async Task YukleAsync()
    {
        Kategoriler = await _context.Kategoriler
            .OrderBy(k => k.Sira)
            .ThenBy(k => k.Ad)
            .ToListAsync();

        TumYemekler = await _context.Yemekler
            .Include(y => y.Kategori)
            .OrderBy(y => y.KategoriId)
            .ThenBy(y => y.Ad)
            .ToListAsync();

        KategoriYemekleri = TumYemekler
            .GroupBy(y => y.KategoriId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
