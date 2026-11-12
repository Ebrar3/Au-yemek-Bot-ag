using System.Globalization;
using AuBot.Data;
using AuBot.Models;
using AuBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Menuler;

public class ExcelYukleModel : PageModel
{
    private const long MaksimumDosyaBoyutu = 10 * 1024 * 1024; // 10 MB
    private static readonly CultureInfo TrKultur = new("tr-TR");

    private readonly AuBotDbContext _context;
    private readonly ExcelMenuServisi _excelServisi;
    private readonly IWebHostEnvironment _env;

    public ExcelYukleModel(
        AuBotDbContext context,
        ExcelMenuServisi excelServisi,
        IWebHostEnvironment env)
    {
        _context = context;
        _excelServisi = excelServisi;
        _env = env;
    }

    [BindProperty]
    public IFormFile? ExcelDosyasi { get; set; }

    [BindProperty]
    public OgunTuru? SeciliOgun { get; set; }

    public ExcelYuklemeSonuc? Sonuc { get; set; }

    public List<MenuYukleme> YuklemeGecmisi { get; set; } = new();

    public async Task OnGetAsync()
    {
        await GecmisiYukleAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ExcelDosyasi == null || ExcelDosyasi.Length == 0)
        {
            ModelState.AddModelError(nameof(ExcelDosyasi), "Lütfen yüklenecek bir Excel (.xlsx) dosyası seçiniz.");
            await GecmisiYukleAsync();
            return Page();
        }

        var uzanti = Path.GetExtension(ExcelDosyasi.FileName);
        if (!string.Equals(uzanti, ".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(ExcelDosyasi), "Yalnızca .xlsx uzantılı Excel dosyaları desteklenmektedir.");
            await GecmisiYukleAsync();
            return Page();
        }

        if (ExcelDosyasi.Length > MaksimumDosyaBoyutu)
        {
            ModelState.AddModelError(nameof(ExcelDosyasi), "Dosya boyutu en fazla 10 MB olabilir.");
            await GecmisiYukleAsync();
            return Page();
        }

        var yukleyen = User.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(User.Identity.Name)
            ? User.Identity.Name
            : "Yönetici";

        try
        {
            await using var akis = ExcelDosyasi.OpenReadStream();
            var temizDosyaAdi = Path.GetFileName(ExcelDosyasi.FileName);
            Sonuc = await _excelServisi.YukleAsync(akis, temizDosyaAdi, SeciliOgun, yukleyen);
        }
        catch (ExcelFormatHatasi ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        await GecmisiYukleAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostOrnekYukleAsync(string dosyaAdi, OgunTuru ogun)
    {
        var temizAd = Path.GetFileName(dosyaAdi);
        var ornekKlasor = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "OrnekMenuler"));
        var tamYol = Path.Combine(ornekKlasor, temizAd);

        if (!System.IO.File.Exists(tamYol))
        {
            ModelState.AddModelError(string.Empty, $"Örnek menü dosyası bulunamadı: {temizAd}");
            await GecmisiYukleAsync();
            return Page();
        }

        var yukleyen = User.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(User.Identity.Name)
            ? User.Identity.Name
            : "Yönetici (Örnek Menü)";

        try
        {
            await using var akis = System.IO.File.OpenRead(tamYol);
            Sonuc = await _excelServisi.YukleAsync(akis, temizAd, ogun, yukleyen);
        }
        catch (ExcelFormatHatasi ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        await GecmisiYukleAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostSilAsync(int id)
    {
        var kayit = await _context.MenuYuklemeleri.FindAsync(id);
        if (kayit != null)
        {
            _context.MenuYuklemeleri.Remove(kayit);
            await _context.SaveChangesAsync();
            TempData["Basari"] = $"\"{kayit.DosyaAdi}\" yükleme geçmişi kaydı silindi (aktarılan menüler korundu).";
        }

        return RedirectToPage();
    }

    public string AyAdiGetir(int yil, int ay)
    {
        if (ay < 1 || ay > 12 || yil < 2000 || yil > 2100)
            return $"{ay:00}/{yil}";
        return new DateTime(yil, ay, 1).ToString("MMMM yyyy", TrKultur);
    }

    private async Task GecmisiYukleAsync()
    {
        YuklemeGecmisi = await _context.MenuYuklemeleri
            .OrderByDescending(x => x.YuklemeTarihi)
            .ThenByDescending(x => x.Id)
            .Take(25)
            .ToListAsync();
    }
}
