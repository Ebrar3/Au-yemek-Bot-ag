using System.Globalization;
using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Menuler;

public class IndexModel : PageModel
{
    private readonly AuBotDbContext _context;

    public IndexModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int? Yil { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Ay { get; set; }

    [BindProperty(SupportsGet = true)]
    public OgunTuru? OgunFiltresi { get; set; }

    public int SeciliYil { get; set; }
    public int SeciliAy { get; set; }
    public string AyBasligi { get; set; } = string.Empty;

    public int OncekiYil { get; set; }
    public int OncekiAy { get; set; }
    public int SonrakiYil { get; set; }
    public int SonrakiAy { get; set; }

    public int BaslangicBosGunSayisi { get; set; }
    public int AyinGunSayisi { get; set; }

    public Dictionary<DateOnly, List<Menu>> GunlukMenuler { get; set; } = new();

    public async Task OnGetAsync()
    {
        var bugun = DateTime.Today;
        SeciliYil = Yil is >= 2020 and <= 2035 ? Yil.Value : bugun.Year;
        SeciliAy = Ay is >= 1 and <= 12 ? Ay.Value : bugun.Month;

        var ayinIlkGunu = new DateOnly(SeciliYil, SeciliAy, 1);
        AyinGunSayisi = DateTime.DaysInMonth(SeciliYil, SeciliAy);
        var ayinSonGunu = new DateOnly(SeciliYil, SeciliAy, AyinGunSayisi);

        var trKultur = new CultureInfo("tr-TR");
        AyBasligi = ayinIlkGunu.ToDateTime(TimeOnly.MinValue).ToString("MMMM yyyy", trKultur);

        // Pazartesi = 0 ... Pazar = 6 olacak şekilde boş hücre sayısı
        BaslangicBosGunSayisi = ((int)ayinIlkGunu.DayOfWeek + 6) % 7;

        var oncekiTarih = ayinIlkGunu.AddMonths(-1);
        OncekiYil = oncekiTarih.Year;
        OncekiAy = oncekiTarih.Month;

        var sonrakiTarih = ayinIlkGunu.AddMonths(1);
        SonrakiYil = sonrakiTarih.Year;
        SonrakiAy = sonrakiTarih.Month;

        IQueryable<Menu> sorgu = _context.Menuler
            .Include(m => m.MenuYemekleri.OrderBy(my => my.Sira))
                .ThenInclude(my => my.Yemek)
                    .ThenInclude(y => y!.Kategori)
            .Where(m => m.Tarih >= ayinIlkGunu && m.Tarih <= ayinSonGunu);

        if (OgunFiltresi != null)
        {
            sorgu = sorgu.Where(m => m.Ogun == OgunFiltresi);
        }

        var menuler = await sorgu
            .OrderBy(m => m.Tarih)
            .ThenBy(m => m.Ogun)
            .ToListAsync();

        GunlukMenuler = menuler
            .GroupBy(m => m.Tarih)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<IActionResult> OnPostSilAsync(int id)
    {
        var menu = await _context.Menuler
            .Include(m => m.MenuYemekleri)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu != null)
        {
            _context.MenuYemekleri.RemoveRange(menu.MenuYemekleri);
            _context.Menuler.Remove(menu);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { Yil = SeciliYil, Ay = SeciliAy });
    }
}
