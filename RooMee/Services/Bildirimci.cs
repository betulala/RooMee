using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using RooMee.Models;

namespace RooMee.Services;

// Kullanıcılara e-posta bildirimleri gönderir.
// Kullanıcı profilinden bildirimleri kapattıysa hiçbir şey gönderilmez.
// Gönderim hatası sayfayı bozmasın diye hatalar sadece günlüğe yazılır.
public class Bildirimci
{
    private readonly IEmailSender _eposta;
    private readonly ILogger<Bildirimci> _log;

    public Bildirimci(IEmailSender eposta, ILogger<Bildirimci> log)
    {
        _eposta = eposta;
        _log = log;
    }

    public Task YeniMesajAsync(ApplicationUser alici, ApplicationUser gonderen, string metin, string konusmaAdresi) =>
        GonderAsync(alici, $"RooMee - {gonderen.KisaAd} sana mesaj gönderdi",
            $"<p><b>{Kodla(gonderen.KisaAd)}</b> sana yeni bir mesaj gönderdi:</p>" +
            $"<blockquote style='border-left:3px solid #1f6f63;padding-left:10px;color:#444'>{Kodla(Kisalt(metin, 300))}</blockquote>" +
            Buton("Mesajı oku", konusmaAdresi));

    public Task IlanKaldirildiAsync(ApplicationUser sahip, string ilanBaslik, string neden) =>
        GonderAsync(sahip, "RooMee - İlanın yayından kaldırıldı",
            $"<p><b>\"{Kodla(ilanBaslik)}\"</b> başlıklı ilanın, kullanıcı şikayeti incelendikten sonra yayından kaldırıldı.</p>" +
            $"<p>Şikayet nedeni: {Kodla(neden)}</p>" +
            "<p>Bir yanlışlık olduğunu düşünüyorsan bu e-postayı yanıtlayarak bize ulaşabilirsin.</p>");

    public Task YeniIlanlarAsync(ApplicationUser alici, string aramaAdi, List<Ilan> ilanlar, Func<Ilan, string> ilanAdresi, string aramaAdresi)
    {
        var liste = string.Join("", ilanlar.Take(5).Select(i =>
            $"<li><a href='{ilanAdresi(i)}'>{Kodla(i.Baslik)}</a> — {Kodla(i.Ilce)}, {Kodla(i.Sehir)} · {i.Kira:N0} ₺</li>"));
        return GonderAsync(alici, $"RooMee - \"{aramaAdi}\" aramana uyan yeni ilan",
            $"<p>Kaydettiğin <b>{Kodla(aramaAdi)}</b> aramasına uyan yeni ilan var:</p><ul>{liste}</ul>" +
            Buton("Tüm sonuçları gör", aramaAdresi));
    }

    private async Task GonderAsync(ApplicationUser alici, string konu, string govde)
    {
        if (!alici.EpostaBildirim || string.IsNullOrEmpty(alici.Email)) return;
        var html =
            $"<div style='font-family:Segoe UI,Arial,sans-serif;max-width:560px'>" +
            $"<p>Merhaba {Kodla(alici.Ad)},</p>{govde}" +
            "<hr style='border:none;border-top:1px solid #e3e0da;margin:24px 0'/>" +
            "<p style='font-size:12px;color:#5b6672'>Bu bildirimleri almak istemiyorsan profilindeki " +
            "\"E-posta bildirimleri\" seçeneğini kapatabilirsin.</p></div>";
        try
        {
            await _eposta.SendEmailAsync(alici.Email, konu, html);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Bildirim e-postası gönderilemedi: {Konu}", konu);
        }
    }

    private static string Buton(string yazi, string adres) =>
        $"<p><a href='{adres}' style='display:inline-block;background:#ff7a59;color:#fff;padding:10px 18px;border-radius:8px;text-decoration:none;font-weight:bold'>{yazi}</a></p>";

    private static string Kodla(string? s) => WebUtility.HtmlEncode(s ?? "");
    private static string Kisalt(string s, int n) => s.Length <= n ? s : s[..n] + "…";
}
