using Microsoft.AspNetCore.Identity;
using RooMee.Models;

namespace RooMee.Data;

public static class Yoneticiler
{
    // Yönetici rolü yoksa oluşturur ve verilen e-postadaki kullanıcıya atar
    public static async Task YoneticiYapAsync(IServiceProvider servisler, string? eposta)
    {
        if (string.IsNullOrWhiteSpace(eposta))
        {
            Console.WriteLine("Kullanım: dotnet run --project RooMee -- --yonetici-yap ornek@mail.com");
            return;
        }

        using var kapsam = servisler.CreateScope();
        var roller = kapsam.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var kullanicilar = kapsam.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await RolOlusturAsync(roller);

        var k = await kullanicilar.FindByEmailAsync(eposta.Trim());
        if (k == null)
        {
            Console.WriteLine($"Bu e-postayla kayıtlı kullanıcı bulunamadı: {eposta}");
            return;
        }
        if (await kullanicilar.IsInRoleAsync(k, Roller.Yonetici))
        {
            Console.WriteLine($"{k.Ad} {k.Soyad} zaten yönetici.");
            return;
        }

        var sonuc = await kullanicilar.AddToRoleAsync(k, Roller.Yonetici);
        Console.WriteLine(sonuc.Succeeded
            ? $"{k.Ad} {k.Soyad} artık yönetici. Değişikliğin görünmesi için çıkış yapıp tekrar giriş yapmalı."
            : "Hata: " + string.Join(", ", sonuc.Errors.Select(e => e.Description)));
    }

    // Sunucu ayarındaki "IlkYonetici" e-postası kayıtlıysa ve henüz yönetici değilse yönetici yapar.
    // eposta verilirse sadece o kişi için kontrol eder (girişte kullanılır). Yönetici yapıldıysa true döner.
    public static async Task<bool> IlkYoneticiyiKontrolEtAsync(IServiceProvider servisler, string? eposta)
    {
        var ilk = servisler.GetRequiredService<IConfiguration>()["IlkYonetici"]?.Trim();
        if (string.IsNullOrEmpty(ilk)) return false;
        if (eposta != null && !string.Equals(eposta.Trim(), ilk, StringComparison.OrdinalIgnoreCase)) return false;

        using var kapsam = servisler.CreateScope();
        var roller = kapsam.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var kullanicilar = kapsam.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var k = await kullanicilar.FindByEmailAsync(ilk);
        if (k == null || await kullanicilar.IsInRoleAsync(k, Roller.Yonetici)) return false;

        await RolOlusturAsync(roller);
        return (await kullanicilar.AddToRoleAsync(k, Roller.Yonetici)).Succeeded;
    }

    public static async Task RolOlusturAsync(RoleManager<IdentityRole> roller)
    {
        if (!await roller.RoleExistsAsync(Roller.Yonetici))
            await roller.CreateAsync(new IdentityRole(Roller.Yonetici));
    }
}
