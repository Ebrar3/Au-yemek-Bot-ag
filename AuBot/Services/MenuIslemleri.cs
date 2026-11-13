using AuBot.Models;

namespace AuBot.Services;

/// <summary>
/// Menü oluşturma/güncelleme ve Excel menü aktarımı sırasında kullanılan ortak işlemler.
/// </summary>
public static class MenuIslemleri
{
    /// <summary>
    /// Menüdeki yemekleri verilen listeyle eşitler (N–N ara tablo: MenuYemekleri).
    /// Listede olmayanları siler, olanların sırasını günceller, yenileri ekler.
    /// </summary>
    public static void YemekleriEsitle(Menu menu, IReadOnlyList<Yemek> yemekler)
    {
        var silinecekler = menu.MenuYemekleri
            .Where(my => !yemekler.Any(y => y.Id != 0 && y.Id == my.YemekId))
            .ToList();

        foreach (var my in silinecekler)
            menu.MenuYemekleri.Remove(my);

        for (var i = 0; i < yemekler.Count; i++)
        {
            var yemek = yemekler[i];
            var mevcut = yemek.Id != 0
                ? menu.MenuYemekleri.FirstOrDefault(my => my.YemekId == yemek.Id)
                : null;

            if (mevcut is not null)
                mevcut.Sira = i + 1;
            else
                menu.MenuYemekleri.Add(new MenuYemek { Yemek = yemek, Sira = i + 1 });
        }
    }

    /// <summary>
    /// Tüm yemeklerin kalorisi biliniyorsa toplamı döndürür; biri bile bilinmiyorsa null.
    /// </summary>
    public static int? ToplamKaloriHesapla(IReadOnlyCollection<Yemek> yemekler)
    {
        if (yemekler.Count == 0 || yemekler.Any(y => y.Kalori is null))
            return null;
        return yemekler.Sum(y => y.Kalori!.Value);
    }

    public static string OgunAdi(OgunTuru ogun) => ogun switch
    {
        OgunTuru.Aksam => "Akşam",
        OgunTuru.Vejetaryen => "Vejetaryen",
        _ => "Öğle"
    };
}
