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
            try
            {
                var id = await _invoiceApplication.CriarAsync(dto);

                return Ok(id);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/aprovar")]
        public async Task<IActionResult> Aprovar([FromRoute] Guid id)
        {
            try
            {
                await _invoiceApplication.AprovarAsync(id);

                return NoContent();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateInvoiceDto dto)
        {
            try
            {
                await _invoiceApplication.UpdateAsync(dto);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
