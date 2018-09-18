using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTTokenIdentity.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      #region "Seed Data"

      builder.Entity<ApplicationRole>().HasData(
        new { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
        new { Id = "2", Name = "Customer", NormalizedName = "CUSTOMER" }
      );

      #endregion
    }
  }
}