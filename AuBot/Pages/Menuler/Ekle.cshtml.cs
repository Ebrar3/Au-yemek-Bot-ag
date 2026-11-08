using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Menuler;

public class EkleModel : PageModel
{
    private readonly AuBotDbContext _context;

    public EkleModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DateOnly Tarih { get; set; }

    [BindProperty]
    public OgunTuru Ogun { get; set; } = OgunTuru.Ogle;

    [BindProperty]
    public bool TatilMi { get; set; }

    [BindProperty]
    public string? Aciklama { get; set; }

    // Her kategorideki seçilen yemek ID'si (key: KategoriId, value: YemekId)
    [BindProperty]
    public Dictionary<int, int?> SeciliYemekler { get; set; } = new();

    // Kategoriler (Sira'ya göre sıralı)
    public List<Kategori> Kategoriler { get; set; } = new();

    // Her kategoriye ait yemekler (dropdown için)
    public Dictionary<int, List<Yemek>> KategoriYemekleri { get; set; } = new();

    // Canlı kalori toplamı için tüm yemekler
    public List<Yemek> TumYemekler { get; set; } = new();

    // Zaten bu tarih+öğünde menü var mı?
    public bool MevcutMenuVar { get; set; }

    public async Task<IActionResult> OnGetAsync(string? tarih = null)
    {
        if (tarih != null && DateOnly.TryParse(tarih, out var t))
            Tarih = t;
        else
            Tarih = DateOnly.FromDateTime(DateTime.Today);

        await YukleAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await YukleAsync();

        // Tarih + Öğün çakışması kontrolü
        MevcutMenuVar = await _context.Menuler
            .AnyAsync(m => m.Tarih == Tarih && m.Ogun == Ogun);

        if (MevcutMenuVar)
        {
            ModelState.AddModelError(string.Empty,
                $"Bu tarih ({Tarih:dd.MM.yyyy}) için {(Ogun == OgunTuru.Ogle ? "Öğle" : "Akşam")} menüsü zaten mevcut. Lütfen mevcut menüyü düzenleyin.");
            return Page();
        }

        if (!TatilMi && SeciliYemekler.All(kv => kv.Value == null))
        {
            ModelState.AddModelError(string.Empty, "En az bir yemek seçmelisiniz.");
            return Page();
        }

        // Yeni menü oluştur
        var yeniMenu = new Menu
        {
            Tarih = Tarih,
            Ogun = Ogun,
            TatilMi = TatilMi,
            Aciklama = Aciklama
        };

        if (!TatilMi)
        {
            int sira = 1;
            foreach (var kat in Kategoriler)
            {
                if (SeciliYemekler.TryGetValue(kat.Id, out var yemekId) && yemekId != null)
                {
                    yeniMenu.MenuYemekleri.Add(new MenuYemek
                    {
                        YemekId = yemekId.Value,
                        Sira = sira
                    });
                }
                sira++;
            }

            // Toplam kalori hesapla
            var yemekIds = yeniMenu.MenuYemekleri.Select(my => my.YemekId).ToList();
            var kaloriler = await _context.Yemekler
                .Where(y => yemekIds.Contains(y.Id))
                .Select(y => y.Kalori)
                .ToListAsync();
            yeniMenu.ToplamKalori = kaloriler.Sum();
        }

        _context.Menuler.Add(yeniMenu);
        await _context.SaveChangesAsync();

        TempData["Basari"] = $"{Tarih:dd.MM.yyyy} tarihi için {(Ogun == OgunTuru.Ogle ? "Öğle" : "Akşam")} menüsü başarıyla oluşturuldu!";
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

        // Her kategori için başlangıç değeri
        foreach (var kat in Kategoriler)
        {
            SeciliYemekler.TryAdd(kat.Id, null);
        }
    }
}
