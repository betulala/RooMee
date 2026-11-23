using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        // Ana sayfada gösterilecek en yeni ilanlar
        var sonIlanlar = await _db.Ilanlar
            .Where(i => i.Aktif)
            .Include(i => i.Kullanici)
            .Include(i => i.Fotolar)
            .OrderByDescending(i => i.OlusturmaTarihi)
            .Take(6)
            .ToListAsync();

        ViewBag.Sehirler = await _db.Ilanlar
            .Where(i => i.Aktif)
            .Select(i => i.Sehir)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        return View(sonIlanlar);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
