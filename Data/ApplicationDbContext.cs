using Lumen_Merch_Store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Data;

 public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Universe> Universes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Role> CustomRoles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Налаштування для enum
            builder.Entity<Order>()
                .Property(e => e.Status)
                .HasConversion<string>();

            // Налаштування зв'язків
            builder.Entity<Product>()
                .HasOne(p => p.Universe)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UniverseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductSize>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSizes)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Size)
                .WithMany(ps => ps.OrderItems)
                .HasForeignKey(oi => oi.SizeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed дані для ролей
            builder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );

            // Перейменування таблиць Identity для відповідності схемі БД
            builder.Entity<ApplicationUser>().ToTable("users");
            builder.Entity<IdentityRole<int>>().ToTable("AspNetRoles");
            builder.Entity<IdentityUserRole<int>>().ToTable("AspNetUserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("AspNetUserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("AspNetUserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("AspNetUserTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("AspNetRoleClaims");

            // Перейменування колонок для users таблиці
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone");
                entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
                entity.Property(e => e.PasswordHash).HasColumnName("password");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Перейменування колонок для інших таблиць
            ConfigureTableNames(builder);
        }

        private void ConfigureTableNames(ModelBuilder builder)
        {
            builder.Entity<Universe>().ToTable("universes");
            builder.Entity<Category>().ToTable("categories");
            builder.Entity<Product>().ToTable("products");
            builder.Entity<ProductSize>().ToTable("product_sizes");
            builder.Entity<Role>().ToTable("roles");
            builder.Entity<Order>().ToTable("orders");
            builder.Entity<OrderItem>().ToTable("order_items");
            builder.Entity<Favorite>().ToTable("favorites");

            // Налаштування назв колонок для відповідності схемі БД
            ConfigureColumnNames(builder);
        }

        private void ConfigureColumnNames(ModelBuilder builder)
        {
            // Universe
            builder.Entity<Universe>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");
            });

            // Category
            builder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");
            });

            // Product
            builder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UniverseId).HasColumnName("universe_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.ShortDescription).HasColumnName("short_description");
                entity.Property(e => e.FullDescription).HasColumnName("full_description");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.Stock).HasColumnName("stock");
            });

            // ProductSize
            builder.Entity<ProductSize>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Size).HasColumnName("size");
                entity.Property(e => e.Stock).HasColumnName("stock");
            });

            // Role
            builder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
            });

            // Order
            builder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Total).HasColumnName("total");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // OrderItem
            builder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.SizeId).HasColumnName("size_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.Price).HasColumnName("price");
            });

            // Favorite
            builder.Entity<Favorite>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });
        }
    }