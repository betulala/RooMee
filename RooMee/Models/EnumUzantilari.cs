using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RooMee.Models;

public static class EnumUzantilari
{
    // OdaTipi.TekKisilik -> "Tek kişilik oda"
    public static string GorunenAd(this Enum deger)
    {
        var alan = deger.GetType().GetField(deger.ToString());
        return alan?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? deger.ToString();
    }
}