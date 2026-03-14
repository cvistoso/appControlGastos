using Microsoft.EntityFrameworkCore;
using ExpenseControlApp.Data;
using ExpenseControlApp.Models.DTOs;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace ExpenseControlApp.Services
{
    public class ReportService : IReportService
    {
        private readonly ExpenseDbContext _context;

        public ReportService(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GeneratePdfReportAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await GetTransactionsForReportAsync(userId, startDate, endDate);
            var user = await _context.Users.FindAsync(userId);

            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Title
            var title = new Paragraph("Expense Control Report")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(18)
                .SetBold()
                .SetMarginBottom(20);
            document.Add(title);

            // User info
            var userInfo = $"User: {user?.FirstName} {user?.LastName} ({user?.Email})";
            document.Add(new Paragraph(userInfo).SetFontSize(12));
            document.Add(new Paragraph($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}").SetFontSize(12));
            document.Add(new Paragraph(" "));

            // Summary
            var totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            var totalExpenses = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            var balance = totalIncome - totalExpenses;

            document.Add(new Paragraph("Summary:").SetBold().SetFontSize(12));
            document.Add(new Paragraph($"Total Income: ${totalIncome:N2}").SetFontSize(12));
            document.Add(new Paragraph($"Total Expenses: ${totalExpenses:N2}").SetFontSize(12));
            document.Add(new Paragraph($"Balance: ${balance:N2}").SetFontSize(12));
            document.Add(new Paragraph(" "));

            // Transactions table
            var table = new Table(6, true).UseAllAvailableWidth();
            
            // Headers
            var headers = new[] { "Description", "Date", "Type", "Category", "Amount", "Payment Method" };
            foreach (var header in headers)
            {
                var cell = new Cell()
                    .Add(new Paragraph(header).SetBold())
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);
                table.AddCell(cell);
            }

            // Data rows
            foreach (var transaction in transactions.OrderByDescending(t => t.Date))
            {
                table.AddCell(new Cell().Add(new Paragraph(transaction.Description)));
                table.AddCell(new Cell().Add(new Paragraph(transaction.Date.ToString("yyyy-MM-dd"))));
                table.AddCell(new Cell().Add(new Paragraph(transaction.Type)));
                table.AddCell(new Cell().Add(new Paragraph(transaction.CategoryName)));
                table.AddCell(new Cell().Add(new Paragraph($"${transaction.Amount:N2}")));
                table.AddCell(new Cell().Add(new Paragraph(transaction.PaymentMethod ?? "")));
            }

            document.Add(table);
            document.Close();

            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateExcelReportAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await GetTransactionsForReportAsync(userId, startDate, endDate);
            var user = await _context.Users.FindAsync(userId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Expense Report");

            // Title
            worksheet.Cell("A1").Value = "Expense Control Report";
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Font.FontSize = 16;
            worksheet.Range("A1:F1").Merge();

            // User info
            worksheet.Cell("A3").Value = $"User: {user?.FirstName} {user?.LastName} ({user?.Email})";
            worksheet.Cell("A4").Value = $"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}";

            // Summary
            var totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            var totalExpenses = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            var balance = totalIncome - totalExpenses;

            worksheet.Cell("A6").Value = "Summary:";
            worksheet.Cell("A6").Style.Font.Bold = true;
            worksheet.Cell("A7").Value = $"Total Income: ${totalIncome:N2}";
            worksheet.Cell("A8").Value = $"Total Expenses: ${totalExpenses:N2}";
            worksheet.Cell("A9").Value = $"Balance: ${balance:N2}";

            // Headers
            var headers = new[] { "Description", "Date", "Type", "Category", "Amount", "Payment Method" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(11, i + 1).Value = headers[i];
                worksheet.Cell(11, i + 1).Style.Font.Bold = true;
                worksheet.Cell(11, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Data
            int row = 12;
            foreach (var transaction in transactions.OrderByDescending(t => t.Date))
            {
                worksheet.Cell(row, 1).Value = transaction.Description;
                worksheet.Cell(row, 2).Value = transaction.Date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 3).Value = transaction.Type;
                worksheet.Cell(row, 4).Value = transaction.CategoryName;
                worksheet.Cell(row, 5).Value = transaction.Amount;
                worksheet.Cell(row, 6).Value = transaction.PaymentMethod ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsForReportAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= startDate.ToUniversalTime() && t.Date <= endDate.ToUniversalTime())
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    Date = t.Date,
                    Notes = t.Notes,
                    Location = t.Location,
                    PaymentMethod = t.PaymentMethod,
                    IsRecurring = t.IsRecurring,
                    RecurringFrequency = t.RecurringFrequency,
                    NextRecurringDate = t.NextRecurringDate,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name,
                    CategoryColor = t.Category.Color,
                    CategoryIcon = t.Category.Icon
                })
                .ToListAsync();
        }
    }
}
