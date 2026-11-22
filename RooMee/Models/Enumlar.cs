using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

public enum OdaTipi
{
    [Display(Name = "Tek kişilik oda")] TekKisilik,
    [Display(Name = "Ortak oda")] OrtakOda,
    [Display(Name = "Ev bütünü")] EvButunu
}

public enum SikayetDurumu
{
    Beklemede,
    Incelendi,
    Reddedildi
}