using Application.DTOs.Dashboard;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardApplication _dashboardApplication;

        public DashboardController(IDashboardApplication dashboardApplication)
        {
            _dashboardApplication = dashboardApplication;
        }

        [HttpGet("invoices/summary")]
        public async Task<ActionResult<InvoiceSummaryDto>> ResumoInvoices([FromQuery] ObterResumoDto dto)
        {
            try
            {
                var resumo = await _dashboardApplication.ObterResumoInvoicesAsync(dto);

                return Ok(resumo);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
