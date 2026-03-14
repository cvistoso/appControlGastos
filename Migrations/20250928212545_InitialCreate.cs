using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseControlApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                    RecurringFrequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NextRecurringDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedAt", "Description", "Icon", "IsActive", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "#28a745", new DateTime(2025, 9, 28, 21, 25, 44, 664, DateTimeKind.Utc).AddTicks(7180), "Monthly salary income", "fas fa-money-bill-wave", true, "Salary", 1 },
                    { 2, "#17a2b8", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(260), "Freelance work income", "fas fa-laptop-code", true, "Freelance", 1 },
                    { 3, "#6f42c1", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(270), "Investment returns", "fas fa-chart-line", true, "Investment", 1 },
                    { 4, "#fd7e14", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(270), "Food and restaurant expenses", "fas fa-utensils", true, "Food & Dining", 2 },
                    { 5, "#20c997", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(270), "Transport and fuel costs", "fas fa-car", true, "Transportation", 2 },
                    { 6, "#dc3545", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(270), "Rent, mortgage, utilities", "fas fa-home", true, "Housing", 2 },
                    { 7, "#e83e8c", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(270), "Medical and health expenses", "fas fa-heartbeat", true, "Healthcare", 2 },
                    { 8, "#6c757d", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(270), "Movies, games, hobbies", "fas fa-gamepad", true, "Entertainment", 2 },
                    { 9, "#ffc107", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(280), "Clothing and personal items", "fas fa-shopping-bag", true, "Shopping", 2 },
                    { 10, "#6610f2", new DateTime(2025, 9, 28, 21, 25, 44, 665, DateTimeKind.Utc).AddTicks(280), "Courses, books, training", "fas fa-graduation-cap", true, "Education", 2 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastLoginAt", "LastName", "PasswordHash", "Role" },
                values: new object[] { 1, new DateTime(2025, 9, 28, 21, 25, 44, 923, DateTimeKind.Utc).AddTicks(8020), "admin@expensecontrol.com", "Admin", true, null, "User", "$2a$11$u.YKGtptfDsS4QYHZxdqP.jh.5ES/KqiOprYezcypdlp9nrO28U8e", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_Type",
                table: "Categories",
                columns: new[] { "Name", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
