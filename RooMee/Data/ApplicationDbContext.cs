using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RooMee.Models;

namespace RooMee.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Profil> Profiller => Set<Profil>();
    public DbSet<Ilan> Ilanlar => Set<Ilan>();
    public DbSet<IlanFoto> IlanFotolari => Set<IlanFoto>();
    public DbSet<Favori> Favoriler => Set<Favori>();
    public DbSet<Sikayet> Sikayetler => Set<Sikayet>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Her kullanıcının tek profili olur; kullanıcı silinince profili de silinir
        builder.Entity<Profil>()
            .HasOne(p => p.Kullanici).WithOne(u => u.Profil)
            .HasForeignKey<Profil>(p => p.KullaniciId)
            .OnDelete(DeleteBehavior.Cascade);

        // Kullanıcı silinince ilanları da silinir
        builder.Entity<Ilan>()
            .HasOne(i => i.Kullanici).WithMany(u => u.Ilanlar)
            .HasForeignKey(i => i.KullaniciId)
            .OnDelete(DeleteBehavior.Cascade);

        // Aynı ilan bir kişi tarafından iki kez favorilenemez
        builder.Entity<Favori>()
            .HasIndex(f => new { f.KullaniciId, f.IlanId }).IsUnique();
        builder.Entity<Favori>()
            .HasOne(f => f.Kullanici).WithMany()
            .HasForeignKey(f => f.KullaniciId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Favori>()
            .HasOne(f => f.Ilan).WithMany()
            .HasForeignKey(f => f.IlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Sikayet>()
            .HasOne(s => s.SikayetEden).WithMany()
            .HasForeignKey(s => s.SikayetEdenId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Sikayet>()
            .HasOne(s => s.SikayetEdilenKullanici).WithMany()
            .HasForeignKey(s => s.SikayetEdilenKullaniciId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Sikayet>()
            .HasOne(s => s.Ilan).WithMany()
            .HasForeignKey(s => s.IlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}