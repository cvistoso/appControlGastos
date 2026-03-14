using System.ComponentModel.DataAnnotations;

namespace ExpenseControlApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public CategoryType Type { get; set; }

        [StringLength(7)]
        public string Color { get; set; } = "#007bff";

        [StringLength(50)]
        public string? Icon { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public enum CategoryType
    {
        Income = 1,
        Expense = 2
    }
}

