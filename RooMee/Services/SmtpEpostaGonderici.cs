using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace RooMee.Services;

public class EpostaAyarlari
{
    public string Sunucu { get; set; } = "";
    public int Port { get; set; } = 587;
    public string Kullanici { get; set; } = "";
    public string Sifre { get; set; } = "";
    public string GonderenEposta { get; set; } = "";
    public string GonderenAd { get; set; } = "RooMee";
}

public class SmtpEpostaGonderici : IEmailSender
{
    private readonly EpostaAyarlari _ayar;
    private readonly ILogger<SmtpEpostaGonderici> _log;

    public SmtpEpostaGonderici(IOptions<EpostaAyarlari> ayar, ILogger<SmtpEpostaGonderici> log)
    {
        _ayar = ayar.Value;
        _log = log;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Test hesapları (@roomee.test gibi .test uzantılı adresler) gerçek değildir, e-posta gönderilmez.
        // SMTP ayarı yapılmamışsa da e-posta sadece günlüğe yazılır.
        if (email.EndsWith(".test", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(_ayar.Sunucu))
        {
            _log.LogInformation("E-posta gönderilmedi (test adresi ya da SMTP ayarı yok): {Alici} - {Konu}", email, subject);
            return;
        }

        using var mesaj = new MailMessage
        {
            From = new MailAddress(_ayar.GonderenEposta, _ayar.GonderenAd),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mesaj.To.Add(email);

        using var istemci = new SmtpClient(_ayar.Sunucu, _ayar.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_ayar.Kullanici, _ayar.Sifre)
        };

        await istemci.SendMailAsync(mesaj);
        _log.LogInformation("E-posta gönderildi: {Alici} - {Konu}", email, subject);
    }
}