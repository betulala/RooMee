using RooMee.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;   // e-posta onaylanmadan giriş yok
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false; // sembol zorunlu değil
    options.Lockout.MaxFailedAccessAttempts = 5;     // 5 hatalı denemede
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // 10 dk kilit
})
.AddRoles<IdentityRole>()
.AddSignInManager<RooMee.Services.RooMeeGirisYoneticisi>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<RooMee.Services.TurkceIdentityHatalari>();
// Doğrulama ve form mesajları Türkçe (MVC sayfaları ve hesap sayfaları için)
builder.Services.AddControllersWithViews();
builder.Services.Configure<MvcOptions>(o =>
{
    o.ModelMetadataDetailsProviders.Add(new RooMee.Services.TurkceDogrulamaMesajlari());
    RooMee.Services.TurkceDogrulamaMesajlari.ModelBaglamaMesajlari(o);
});

// Tarih, sayı ve para biçimleri Türkçe (23.11.2026, 5.000 gibi)
var turkce = new System.Globalization.CultureInfo("tr-TR");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = turkce;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = turkce;

builder.Services.Configure<RooMee.Services.EpostaAyarlari>(builder.Configuration.GetSection("Eposta"));
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, RooMee.Services.SmtpEpostaGonderici>();
// Oturum çerezleri dakikada bir kontrol edilir; askıya alınan ya da silinen hesabın oturumu hemen kapanır
builder.Services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromMinutes(1));
builder.Services.AddScoped<RooMee.Services.Bildirimci>();
builder.Services.AddScoped<RooMee.Services.AramaUyarilari>();
builder.Services.AddSingleton<RooMee.Services.Yuklemeler>();
var app = builder.Build();

// Geliştirme için örnek veri: dotnet run --project RooMee -- --test-verisi
if (args.Contains("--test-verisi"))
{
    await RooMee.Data.TestVerisi.YukleAsync(app.Services);
    return;
}

// Bir kullanıcıyı yönetici yap: dotnet run --project RooMee -- --yonetici-yap ornek@mail.com
var yoneticiIndex = Array.IndexOf(args, "--yonetici-yap");
if (yoneticiIndex >= 0)
{
    var eposta = yoneticiIndex + 1 < args.Length ? args[yoneticiIndex + 1] : null;
    await RooMee.Data.Yoneticiler.YoneticiYapAsync(app.Services, eposta);
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// İki adımlı doğrulama (doğrulama uygulaması) kullanılmıyor; güvenlik öğrenci e-postasına gönderilen
// kodla sağlanıyor. Daha önce açan olduysa kapat, giriş yapamaz hale gelmesin.
try
{
    using var kapsam = app.Services.CreateScope();
    var db = kapsam.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Bekleyen veritabanı değişikliklerini (migration) otomatik uygula; Update-Database'e gerek kalmaz.
    // Azure SQL'in ücretsiz veritabanı uykudan uyanırken ilk bağlantılar zaman aşımına uğrayabilir;
    // bu yüzden yaklaşık 2 dakika boyunca tekrar denenir.
    var azureMi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
    var enFazlaDeneme = azureMi ? 8 : 1;
    for (var deneme = 1; ; deneme++)
    {
        try
        {
            await db.Database.MigrateAsync();
            break;
        }
        catch (Exception ex) when (deneme < enFazlaDeneme)
        {
            app.Logger.LogWarning(ex, "Veritabanına bağlanılamadı (deneme {Deneme}/{EnFazla}), 15 sn sonra tekrar denenecek.", deneme, enFazlaDeneme);
            await Task.Delay(TimeSpan.FromSeconds(15));
        }
    }
    await db.Users.Where(u => u.TwoFactorEnabled).ExecuteUpdateAsync(s => s.SetProperty(u => u.TwoFactorEnabled, false));
    // "IlkYonetici" ayarındaki e-posta kayıtlıysa yönetici yap (kayıtlı değilse ilk girişinde yapılır)
    await RooMee.Data.Yoneticiler.IlkYoneticiyiKontrolEtAsync(app.Services, null);
}
catch (Exception ex)
{
    // Veritabanına o an ulaşılamıyorsa uygulama yine açılsın
    app.Logger.LogError(ex, "Açılışta veritabanı hazırlanamadı (migration / ilk yönetici). Bağlantı metnini kontrol et.");
}

app.UseHttpsRedirection();

// Kaldırılan iki adımlı doğrulama sayfaları (Identity'nin hazır İngilizce sayfaları açılmasın)
string[] kaldirilanSayfalar =
{
    "/Identity/Account/Manage/TwoFactorAuthentication", "/Identity/Account/Manage/EnableAuthenticator",
    "/Identity/Account/Manage/Disable2fa", "/Identity/Account/Manage/GenerateRecoveryCodes",
    "/Identity/Account/Manage/ResetAuthenticator", "/Identity/Account/Manage/ShowRecoveryCodes",
    "/Identity/Account/LoginWith2fa", "/Identity/Account/LoginWithRecoveryCode",
};
app.Use(async (context, next) =>
{
    var yol = context.Request.Path.Value ?? "";
    if (kaldirilanSayfalar.Any(s => yol.Equals(s, StringComparison.OrdinalIgnoreCase)))
    {
        context.Response.Redirect(yol.Contains("/Manage/") ? "/Identity/Account/Manage" : "/Identity/Account/Login");
        return;
    }
    await next();
});
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Kullanıcıların yüklediği fotoğraflar (/uploads/...). Azure'da kalıcı veri klasöründen sunulur.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        app.Services.GetRequiredService<RooMee.Services.Yuklemeler>().Kok),
    RequestPath = "/uploads"
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
