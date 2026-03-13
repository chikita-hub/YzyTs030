using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProPla.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<ProductPlanning> ProductPlannings => Set<ProductPlanning>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductPlanning>(entity =>
        {
            entity.ToTable("m_productplanning", "proplaweb");
            entity.HasKey(x => x.ProductPlanningId);

            entity.Property(x => x.ProductPlanningId)
                .HasColumnName("productplanningid")
                .HasMaxLength(10);
            entity.Property(x => x.ProductCategory)
                .HasColumnName("productcategory")
                .HasMaxLength(3);
            entity.Property(x => x.ProductPerson)
                .HasColumnName("productperson")
                .HasMaxLength(30);
            entity.Property(x => x.ProductNameOfficial)
                .HasColumnName("productnameofficial")
                .HasMaxLength(1000);
            entity.Property(x => x.ProductNameRuby)
                .HasColumnName("productnameruby")
                .HasMaxLength(1000);
            entity.Property(x => x.ProductNameOfficialE)
                .HasColumnName("productnameofficiale")
                .HasMaxLength(1000);
            entity.Property(x => x.ProductNameOmit1)
                .HasColumnName("productnameomit1")
                .HasMaxLength(1000);
            entity.Property(x => x.ProductNameReceipt)
                .HasColumnName("productnamereceipt")
                .HasMaxLength(1000);
            entity.Property(x => x.ProductReleaseStatus)
                .HasColumnName("productreleasestatus")
                .HasMaxLength(3);
            entity.Property(x => x.ProductTestReleaseDate)
                .HasColumnName("producttestreleasedate")
                .HasMaxLength(1000);
            entity.Property(x => x.ProductReleaseDate)
                .HasColumnName("productreleasedate")
                .HasMaxLength(1000);
            entity.Property(x => x.ProductAnniversary)
                .HasColumnName("productanniversary")
                .HasMaxLength(10);
            entity.Property(x => x.ProductRenewalHistory)
                .HasColumnName("productrenewalhistory")
                .HasMaxLength(1000);
            entity.Property(x => x.TrademarkAcquisitionList)
                .HasColumnName("trademarkacquisitionlist")
                .HasMaxLength(3000);
        });

    }
}
public class ProductPlanning
{
    public string ProductPlanningId { get; set; } = string.Empty;

    public string? ProductCategory { get; set; }

    public string? ProductPerson { get; set; }

    public string? ProductNameOfficial { get; set; }

    public string? ProductNameRuby { get; set; }

    public string? ProductNameOfficialE { get; set; }

    public string? ProductNameOmit1 { get; set; }

    public string? ProductNameReceipt { get; set; }

    public string? ProductReleaseStatus { get; set; }

    public string? ProductTestReleaseDate { get; set; }

    public string? ProductReleaseDate { get; set; }

    public string? ProductAnniversary { get; set; }

    public string? ProductRenewalHistory { get; set; }

    public string? TrademarkAcquisitionList { get; set; }
}
