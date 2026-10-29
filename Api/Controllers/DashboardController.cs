using Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize(Roles = Roles.AdminOrSeller)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardQueries _dashboardQueries;

        public DashboardController(IDashboardQueries dashboardQueries)
        {
            _dashboardQueries = dashboardQueries;
        }

        /// <summary>Invoice and commission totals for the filters. A Seller user always gets only their own numbers.</summary>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(DashboardSummary), StatusCodes.Status200OK)]
        public async Task<ActionResult<DashboardSummary>> GetSummary([FromQuery] DashboardFilter filter, CancellationToken cancellationToken)
        {
            return Ok(await _dashboardQueries.GetSummaryAsync(filter, cancellationToken));
        }
    }
}
