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
        public async Task<IActionResult> Criar([FromBody] CreateInvoiceDto dto)
        {
            var id = await _invoiceApplication.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id }, new { id });
        }

        [HttpPut("{id}/aprovar")]
        public async Task<IActionResult> Aprovar([FromRoute] Guid id)
        {
            await _invoiceApplication.AprovarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> Cancelar([FromRoute] Guid id)
        {
            await _invoiceApplication.CancelarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateInvoiceDto dto)
        {
            await _invoiceApplication.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] Guid? vendedorId)
        {
            var invoices = await _invoiceApplication.ObterTodosDtoAsync(vendedorId);
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            var invoice = await _invoiceApplication.ObterPorIdAsync(id);
            return Ok(invoice);
        }
    }
}
