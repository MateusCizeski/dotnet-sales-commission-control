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
        public async Task<ActionResult<InvoiceSummaryDto>> ResumoInvoices()
        {
            try
            {
                var resumo = await _dashboardApplication.ObterResumoInvoicesAsync();

                return Ok(resumo);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("comissoes/by-vendedor/{vendedorId}")]
        public async Task<ActionResult<ComissaoVendedorDto>> ComissoesPorVendedor([FromRoute] Guid vendedorId)
        {
            try
            {
                var dto = await _dashboardApplication.ObterComissoesPorVendedorAsync(vendedorId);

                return Ok(dto);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
