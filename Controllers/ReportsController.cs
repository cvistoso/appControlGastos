using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseControlApp.Services;

namespace ExpenseControlApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GeneratePdfReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var pdfBytes = await _reportService.GeneratePdfReportAsync(userId.Value, startDate, endDate);
            return File(pdfBytes, "application/pdf", $"expense-report-{startDate:yyyy-MM-dd}-{endDate:yyyy-MM-dd}.pdf");
        }

        [HttpGet("excel")]
        public async Task<IActionResult> GenerateExcelReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var excelBytes = await _reportService.GenerateExcelReportAsync(userId.Value, startDate, endDate);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"expense-report-{startDate:yyyy-MM-dd}-{endDate:yyyy-MM-dd}.xlsx");
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
