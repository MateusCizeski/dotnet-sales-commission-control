using Application.DTOs.Invoice;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/invoices")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceApplication _invoiceApplication;

        public InvoicesController(IInvoiceApplication invoiceApplication)
        {
            _invoiceApplication = invoiceApplication;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarInvoiceDto dto)
        {
            var id = await _invoiceApplication.Criar(dto);
            return CreatedAtAction(nameof(ListarPorId), new { id }, new { id });
        }

        [HttpPut("{id}/aprovar")]
        public async Task<IActionResult> Aprovar([FromRoute] Guid id)
        {
            await _invoiceApplication.Aprovar(id);
            return NoContent();
        }

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> Cancelar([FromRoute] Guid id)
        {
            await _invoiceApplication.Cancelar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar([FromRoute] Guid id, [FromBody] EditarInvoiceDtoEnxuto dto)
        {
            await _invoiceApplication.Editar(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ListarPaginado([FromQuery] Guid? vendedorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            var result = await _invoiceApplication.ListarPaginado(vendedorId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListarPorId([FromRoute] Guid id)
        {
            var invoice = await _invoiceApplication.ListarPorId(id);
            return Ok(invoice);
        }
    }
}
