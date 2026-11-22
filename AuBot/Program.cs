using AuBot.Data;
using AuBot.Models;
using AuBot.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

static string? KlasorBul(string baslangicDizini, string hedefKlasor)
{
    var dizin = new DirectoryInfo(baslangicDizini);
    for (var i = 0; i < 6 && dizin != null; i++, dizin = dizin.Parent)
    {
        var aday = Path.Combine(dizin.FullName, hedefKlasor);
        if (Directory.Exists(aday))
            return aday;
    }
    return null;
}

var contentRoot = Directory.GetCurrentDirectory();
if (!Directory.Exists(Path.Combine(contentRoot, "wwwroot")))
{
    var bulunanWwwroot = KlasorBul(AppContext.BaseDirectory, "wwwroot")
                         ?? KlasorBul(contentRoot, Path.Combine("AuBot", "wwwroot"));
    if (bulunanWwwroot != null)
    {
        contentRoot = Directory.GetParent(bulunanWwwroot)!.FullName;
    }
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot,
    WebRootPath = Path.Combine(contentRoot, "wwwroot")
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuBotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuBotDb")));

// ASP.NET Core Identity — Yönetici ve Öğrenci rolleri için kimlik doğrulama sistemi
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Şifre gereksinimleri (akademik ortama uygun, esnek tutuldu)
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // E-posta onayı zorunlu değil (demo amaçlı kapalı)
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AuBotDbContext>();

// Servisler: ClosedXML Excel menü ayrıştırma servisi
builder.Services.AddScoped<ExcelMenuServisi>();

var app = builder.Build();

// Uygulama başlarken bekleyen EF Core migration'larını uygula ve OrnekMenuler Excel dosyalarını içe aktar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuBotDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2714 || ex.Number == 1913 || ex.Number == 3728)
    {
        // Yerel veritabanında eski şema/tablo çakışması varsa sıfırlayıp güncel migration'ları temiz şekilde uygula
        db.Database.EnsureDeleted();
        db.Database.Migrate();
    }

    // --- Identity: Rolleri ve varsayılan kullanıcıları oluştur ---
    var rolYoneticisi = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var kullaniciYoneticisi = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Yönetici ve Öğrenci rollerini oluştur (yoksa)
    string[] roller = { "Yonetici", "Ogrenci" };
    foreach (var rol in roller)
    {
        if (!await rolYoneticisi.RoleExistsAsync(rol))
            await rolYoneticisi.CreateAsync(new IdentityRole(rol));
    }

    // Varsayılan Yönetici hesabını oluştur (yoksa)
    const string yoneticiEmail = "admin@aubot.edu.tr";
    const string yoneticiSifre = "Admin123";
    if (await kullaniciYoneticisi.FindByEmailAsync(yoneticiEmail) == null)
    {
        var yonetici = new IdentityUser
        {
            UserName = yoneticiEmail,
            Email = yoneticiEmail,
            EmailConfirmed = true
        };
        var sonuc = await kullaniciYoneticisi.CreateAsync(yonetici, yoneticiSifre);
        if (sonuc.Succeeded)
            await kullaniciYoneticisi.AddToRoleAsync(yonetici, "Yonetici");
    }

    // Varsayılan Öğrenci hesabını oluştur (yoksa)
    const string ogrenciEmail = "ogrenci@aubot.edu.tr";
    const string ogrenciSifre = "Ogrenci123";
    if (await kullaniciYoneticisi.FindByEmailAsync(ogrenciEmail) == null)
    {
        var ogrenci = new IdentityUser
        {
            UserName = ogrenciEmail,
            Email = ogrenciEmail,
            EmailConfirmed = true
        };
        var sonuc = await kullaniciYoneticisi.CreateAsync(ogrenci, ogrenciSifre);
        if (sonuc.Succeeded)
            await kullaniciYoneticisi.AddToRoleAsync(ogrenci, "Ogrenci");
    }

    // OrnekMenuler Excel dosyalarını içe aktar (zaten yüklenmemişse)
    var excelServisi = scope.ServiceProvider.GetRequiredService<ExcelMenuServisi>();
    var ornekKlasor = KlasorBul(app.Environment.ContentRootPath, "OrnekMenuler")
                      ?? KlasorBul(AppContext.BaseDirectory, "OrnekMenuler")
                      ?? Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "OrnekMenuler"));

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

// Identity için kimlik doğrulama middleware'i (UseAuthorization'dan önce olmalı)
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
