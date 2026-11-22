namespace RooMee.Models;

public class IlanFoto
{
    public int Id { get; set; }
    public int IlanId { get; set; }
    public Ilan? Ilan { get; set; }
    public string Yol { get; set; } = "";
}