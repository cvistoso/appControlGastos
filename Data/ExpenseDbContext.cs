using Microsoft.EntityFrameworkCore;
using ExpenseControlApp.Models;

namespace ExpenseControlApp.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => new { e.Name, e.Type }).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Transaction configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Salary", Description = "Monthly salary income", Type = CategoryType.Income, Color = "#28a745", Icon = "fas fa-money-bill-wave" },
                new Category { Id = 2, Name = "Freelance", Description = "Freelance work income", Type = CategoryType.Income, Color = "#17a2b8", Icon = "fas fa-laptop-code" },
                new Category { Id = 3, Name = "Investment", Description = "Investment returns", Type = CategoryType.Income, Color = "#6f42c1", Icon = "fas fa-chart-line" },
                new Category { Id = 4, Name = "Food & Dining", Description = "Food and restaurant expenses", Type = CategoryType.Expense, Color = "#fd7e14", Icon = "fas fa-utensils" },
                new Category { Id = 5, Name = "Transportation", Description = "Transport and fuel costs", Type = CategoryType.Expense, Color = "#20c997", Icon = "fas fa-car" },
                new Category { Id = 6, Name = "Housing", Description = "Rent, mortgage, utilities", Type = CategoryType.Expense, Color = "#dc3545", Icon = "fas fa-home" },
                new Category { Id = 7, Name = "Healthcare", Description = "Medical and health expenses", Type = CategoryType.Expense, Color = "#e83e8c", Icon = "fas fa-heartbeat" },
                new Category { Id = 8, Name = "Entertainment", Description = "Movies, games, hobbies", Type = CategoryType.Expense, Color = "#6c757d", Icon = "fas fa-gamepad" },
                new Category { Id = 9, Name = "Shopping", Description = "Clothing and personal items", Type = CategoryType.Expense, Color = "#ffc107", Icon = "fas fa-shopping-bag" },
                new Category { Id = 10, Name = "Education", Description = "Courses, books, training", Type = CategoryType.Expense, Color = "#6610f2", Icon = "fas fa-graduation-cap" }
            );

            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@expensecontrol.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}

