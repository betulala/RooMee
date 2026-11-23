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
    await db.Users.Where(u => u.TwoFactorEnabled).ExecuteUpdateAsync(s => s.SetProperty(u => u.TwoFactorEnabled, false));
}
catch (Exception ex)
{
    // Veritabanına o an ulaşılamıyorsa uygulama yine açılsın
    app.Logger.LogWarning(ex, "İki adımlı doğrulama kapatılamadı (veritabanına ulaşılamadı).");
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
