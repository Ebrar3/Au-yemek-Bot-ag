using System.Globalization;
using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages;

public class IndexModel : PageModel
{
    private readonly AuBotDbContext _context;

    public IndexModel(AuBotDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public DateOnly? SeciliTarih { get; set; }

    [BindProperty(SupportsGet = true)]
    public OgunTuru? AktifOgun { get; set; }

    public DateOnly GosterilenTarih { get; set; }
    public string FormatliTarih { get; set; } = string.Empty;
    public bool BugunMu { get; set; }

    public Menu? OgleMenusu { get; set; }
    public Menu? AksamMenusu { get; set; }
    public Menu? VejetaryenMenusu { get; set; }

    public List<DateOnly> YakinMenuTarihleri { get; set; } = new();

    public async Task OnGetAsync()
    {
        await OrnekMenuleriKontrolEtAsync();

        var bugun = DateOnly.FromDateTime(DateTime.Today);
        GosterilenTarih = SeciliTarih ?? bugun;
        BugunMu = GosterilenTarih == bugun;

        var trKultur = new CultureInfo("tr-TR");
        FormatliTarih = GosterilenTarih.ToDateTime(TimeOnly.MinValue)
            .ToString("dd MMMM yyyy, dddd", trKultur);

        // Varsayılan sekme: saat 15:00'ten sonraysa Akşam, öncesinde Öğle
        if (AktifOgun == null)
        {
            AktifOgun = DateTime.Now.Hour >= 15 ? OgunTuru.Aksam : OgunTuru.Ogle;
        }

        var gununMenuleri = await _context.Menuler
            .Include(m => m.MenuYemekleri.OrderBy(my => my.Sira))
                .ThenInclude(my => my.Yemek)
                    .ThenInclude(y => y!.Kategori)
            .Where(m => m.Tarih == GosterilenTarih)
            .ToListAsync();

        OgleMenusu = gununMenuleri.FirstOrDefault(m => m.Ogun == OgunTuru.Ogle);
        AksamMenusu = gununMenuleri.FirstOrDefault(m => m.Ogun == OgunTuru.Aksam);
        VejetaryenMenusu = gununMenuleri.FirstOrDefault(m => m.Ogun == OgunTuru.Vejetaryen);

        // Eğer seçili öğünde menü yoksa ama diğer öğünde varsa onu aktif sekme yap
        if (AktifOgun == OgunTuru.Aksam && AksamMenusu == null)
        {
            AktifOgun = OgleMenusu != null ? OgunTuru.Ogle : VejetaryenMenusu != null ? OgunTuru.Vejetaryen : OgunTuru.Aksam;
        }
        else if (AktifOgun == OgunTuru.Ogle && OgleMenusu == null)
        {
            AktifOgun = AksamMenusu != null ? OgunTuru.Aksam : VejetaryenMenusu != null ? OgunTuru.Vejetaryen : OgunTuru.Ogle;
        }
        else if (AktifOgun == OgunTuru.Vejetaryen && VejetaryenMenusu == null)
        {
            AktifOgun = OgleMenusu != null ? OgunTuru.Ogle : AksamMenusu != null ? OgunTuru.Aksam : OgunTuru.Vejetaryen;
        }

        YakinMenuTarihleri = await _context.Menuler
            .Select(m => m.Tarih)
            .Distinct()
            .OrderBy(t => t)
            .Take(10)
            .ToListAsync();
    }

    /// <summary>
    /// Veritabanında henüz hiç menü kaydı yoksa, ana sayfa ve takvim görünümünün
    /// test edilebilmesi için mevcut örnek yemeklerle başlangıç menüleri oluşturur.
    /// </summary>
    private async Task OrnekMenuleriKontrolEtAsync()
    {
        if (await _context.Menuler.AnyAsync())
        {
            return;
        }

        var yemekIdler = await _context.Yemekler
            .OrderBy(y => y.Id)
            .Select(y => y.Id)
            .ToListAsync();

        if (yemekIdler.Count < 8)
        {
            return;
        }

        var bugun = DateOnly.FromDateTime(DateTime.Today);
        var yarin = bugun.AddDays(1);

        var bugunOgle = new Menu
        {
            Tarih = bugun,
            Ogun = OgunTuru.Ogle,
            ToplamKalori = 1383,
            TatilMi = false,
            MenuYemekleri = new List<MenuYemek>
            {
                new() { YemekId = yemekIdler[0], Sira = 1 },
                new() { YemekId = yemekIdler[1], Sira = 2 },
                new() { YemekId = yemekIdler[2], Sira = 3 },
                new() { YemekId = yemekIdler[3], Sira = 4 }
            }
        };

        var bugunAksam = new Menu
        {
            Tarih = bugun,
            Ogun = OgunTuru.Aksam,
            ToplamKalori = 1574,
            TatilMi = false,
            MenuYemekleri = new List<MenuYemek>
            {
                new() { YemekId = yemekIdler[4], Sira = 1 },
                new() { YemekId = yemekIdler[5], Sira = 2 },
                new() { YemekId = yemekIdler[6], Sira = 3 },
                new() { YemekId = yemekIdler[7], Sira = 4 }
            }
        };

        var yarinOgle = new Menu
        {
            Tarih = yarin,
            Ogun = OgunTuru.Ogle,
            ToplamKalori = 1574,
            TatilMi = false,
            MenuYemekleri = new List<MenuYemek>
            {
                new() { YemekId = yemekIdler[4], Sira = 1 },
                new() { YemekId = yemekIdler[5], Sira = 2 },
                new() { YemekId = yemekIdler[6], Sira = 3 },
                new() { YemekId = yemekIdler[7], Sira = 4 }
            }
        };

        var yarinAksam = new Menu
        {
            Tarih = yarin,
            Ogun = OgunTuru.Aksam,
            ToplamKalori = 1383,
            TatilMi = false,
            MenuYemekleri = new List<MenuYemek>
            {
                new() { YemekId = yemekIdler[0], Sira = 1 },
                new() { YemekId = yemekIdler[1], Sira = 2 },
                new() { YemekId = yemekIdler[2], Sira = 3 },
                new() { YemekId = yemekIdler[3], Sira = 4 }
            }
        };

        _context.Menuler.AddRange(bugunOgle, bugunAksam, yarinOgle, yarinAksam);
        await _context.SaveChangesAsync();
    }
}
