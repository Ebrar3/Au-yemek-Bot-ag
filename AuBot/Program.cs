using AuBot.Data;
using AuBot.Models;
using AuBot.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuBotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuBotDb")));

// Servisler: ClosedXML Excel menü ayrıştırma servisi
builder.Services.AddScoped<ExcelMenuServisi>();

var app = builder.Build();

// Uygulama başlarken bekleyen EF Core migration'larını uygula ve OrnekMenuler Excel dosyalarını içe aktar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuBotDbContext>();
    db.Database.Migrate();

    var excelServisi = scope.ServiceProvider.GetRequiredService<ExcelMenuServisi>();
    var ornekKlasor = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "OrnekMenuler"));

    var baslangicDosyalari = new (string DosyaAdi, OgunTuru Ogun)[]
    {
        ("Ekim_2026_Ogle_Menusu.xlsx", OgunTuru.Ogle),
        ("Ekim_2026_Aksam_Menusu.xlsx", OgunTuru.Aksam),
        ("Kasim_2026_Ogle_Menusu.xlsx", OgunTuru.Ogle),
        ("Kasim_2026_Aksam_Menusu.xlsx", OgunTuru.Aksam),
        ("Kasim_2026_Vejetaryen_Menusu.xlsx", OgunTuru.Vejetaryen)
    };

    foreach (var (dosyaAdi, ogun) in baslangicDosyalari)
    {
        var tamYol = Path.Combine(ornekKlasor, dosyaAdi);
        if (File.Exists(tamYol) && !await db.MenuYuklemeleri.AnyAsync(x => x.DosyaAdi == dosyaAdi))
        {
            using var akis = File.OpenRead(tamYol);
            await excelServisi.YukleAsync(akis, dosyaAdi, ogun, "Sistem (Başlangıç)");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
