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

    if (!await db.Menuler.AnyAsync(m => m.Tarih.Year == 2026 && m.Tarih.Month == 10))
    {
        var excelServisi = scope.ServiceProvider.GetRequiredService<ExcelMenuServisi>();
        var ornekKlasor = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "OrnekMenuler"));
        var ogleDosya = Path.Combine(ornekKlasor, "Ekim_2026_Ogle_Menusu.xlsx");
        var aksamDosya = Path.Combine(ornekKlasor, "Ekim_2026_Aksam_Menusu.xlsx");

        if (File.Exists(ogleDosya))
        {
            using var akis = File.OpenRead(ogleDosya);
            await excelServisi.YukleAsync(akis, Path.GetFileName(ogleDosya), OgunTuru.Ogle);
        }

        if (File.Exists(aksamDosya))
        {
            using var akis = File.OpenRead(aksamDosya);
            await excelServisi.YukleAsync(akis, Path.GetFileName(aksamDosya), OgunTuru.Aksam);
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
