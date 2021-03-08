using Bakery.Server.Models;
using Bakery.Shared.DataModel;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Bakery.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<Sweet> Sweets { get; set; }
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RecipeIngredient>().HasKey(x => new { x.IngredientId, x.SweetId });
        }
        
        public DbSet<Bakery.Shared.DataModel.Ingredient> Ingredient { get; set; }

        public DbSet<Bakery.Shared.DataModel.RecipeIngredient> RecipeIngredient { get; set; }
    }
}
