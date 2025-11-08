using System.ComponentModel.DataAnnotations;

namespace ExpenseControlApp.Models.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public string? Notes { get; set; }

        public string? Location { get; set; }

        public string? PaymentMethod { get; set; }

        public bool IsRecurring { get; set; } = false;

        public string? RecurringFrequency { get; set; }

        public DateTime? NextRecurringDate { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }
        public string? CategoryColor { get; set; }
        public string? CategoryIcon { get; set; }
    }

    public class CreateTransactionRequest
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public string? Notes { get; set; }

        public string? Location { get; set; }

        public string? PaymentMethod { get; set; }

        public bool IsRecurring { get; set; } = false;

        public string? RecurringFrequency { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}

