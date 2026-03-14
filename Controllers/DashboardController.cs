using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseControlApp.Models.DTOs;
using ExpenseControlApp.Services;

namespace ExpenseControlApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboardData(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var dashboardData = await _dashboardService.GetDashboardDataAsync(userId.Value, startDate, endDate);
            return Ok(dashboardData);
        }

        [HttpGet("monthly-trends")]
        public async Task<ActionResult<IEnumerable<MonthlyTrendDto>>> GetMonthlyTrends(
            [FromQuery] int months = 12)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var trends = await _dashboardService.GetMonthlyTrendsAsync(userId.Value, months);
            return Ok(trends);
        }

        [HttpGet("projections")]
        public async Task<ActionResult<IEnumerable<ProjectionDto>>> GetCashFlowProjections(
            [FromQuery] int months = 6)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var projections = await _dashboardService.GetCashFlowProjectionsAsync(userId.Value, months);
            return Ok(projections);
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
