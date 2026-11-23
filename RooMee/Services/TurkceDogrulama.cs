using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace RooMee.Services;

// Hata mesajı yazılmamış tüm doğrulama kurallarına Türkçe mesaj verir.
// Örn. [Required] tek başına kullanılırsa "The Email field is required." yerine "E-posta alanı zorunludur." çıkar.
public class TurkceDogrulamaMesajlari : IValidationMetadataProvider, IDisplayMetadataProvider
{
    // [Display(Name=...)] yazılmamış alanlar için Türkçe adlar (hesap sayfalarındaki Email, Password vb.)
    private static readonly Dictionary<string, string> AlanAdlari = new()
    {
        ["Email"] = "E-posta",
        ["NewEmail"] = "Yeni e-posta",
        ["Password"] = "Şifre",
        ["ConfirmPassword"] = "Şifre tekrar",
        ["OldPassword"] = "Mevcut şifre",
        ["NewPassword"] = "Yeni şifre",
        ["Code"] = "Kod",
        ["RecoveryCode"] = "Kurtarma kodu",
        ["TwoFactorCode"] = "Doğrulama kodu",
        ["RememberMe"] = "Beni hatırla",
        ["RememberMachine"] = "Bu cihazı hatırla",
        ["PhoneNumber"] = "Telefon numarası",
        ["Username"] = "Kullanıcı adı",
    };

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        if (context.Key.MetadataKind != ModelMetadataKind.Property) return;
        if (context.DisplayMetadata.DisplayName != null) return;
        if (context.Key.Name != null && AlanAdlari.TryGetValue(context.Key.Name, out var ad))
            context.DisplayMetadata.DisplayName = () => ad;
    }

    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        foreach (var attr in context.ValidationMetadata.ValidatorMetadata.OfType<ValidationAttribute>())
        {
            if (!string.IsNullOrEmpty(attr.ErrorMessage) || attr.ErrorMessageResourceName != null)
                continue;

            attr.ErrorMessage = attr switch
            {
                RequiredAttribute => "{0} alanı zorunludur.",
                StringLengthAttribute s when s.MinimumLength > 0 => "{0} en az {2}, en fazla {1} karakter olmalı.",
                StringLengthAttribute => "{0} en fazla {1} karakter olabilir.",
                MaxLengthAttribute => "{0} en fazla {1} karakter olabilir.",
                MinLengthAttribute => "{0} en az {1} karakter olmalı.",
                RangeAttribute => "{0} {1} ile {2} arasında olmalı.",
                EmailAddressAttribute => "Geçerli bir e-posta adresi gir.",
                PhoneAttribute => "Geçerli bir telefon numarası gir.",
                UrlAttribute => "Geçerli bir web adresi gir.",
                CompareAttribute => "{0} ile {1} aynı değil.",
                RegularExpressionAttribute => "{0} geçerli biçimde değil.",
                _ => attr.ErrorMessage
            };
        }
    }

    // Form değerleri okunurken çıkan (örn. sayı alanına harf yazılması) mesajlar
    public static void ModelBaglamaMesajlari(MvcOptions o)
    {
        var m = o.ModelBindingMessageProvider;
        m.SetAttemptedValueIsInvalidAccessor((deger, alan) => $"'{deger}' değeri {alan} için geçerli değil.");
        m.SetMissingBindRequiredValueAccessor(alan => $"{alan} değeri girilmedi.");
        m.SetMissingKeyOrValueAccessor(() => "Değer zorunludur.");
        m.SetMissingRequestBodyRequiredValueAccessor(() => "İstek boş gönderilemez.");
        m.SetNonPropertyAttemptedValueIsInvalidAccessor(deger => $"'{deger}' değeri geçerli değil.");
        m.SetNonPropertyUnknownValueIsInvalidAccessor(() => "Girilen değer geçerli değil.");
        m.SetNonPropertyValueMustBeANumberAccessor(() => "Bu alana bir sayı girilmeli.");
        m.SetUnknownValueIsInvalidAccessor(alan => $"{alan} için girilen değer geçerli değil.");
        m.SetValueIsInvalidAccessor(deger => $"'{deger}' değeri geçerli değil.");
        m.SetValueMustBeANumberAccessor(alan => $"{alan} alanına bir sayı girilmeli.");
        m.SetValueMustNotBeNullAccessor(deger => $"'{deger}' değeri geçerli değil.");
    }
}
