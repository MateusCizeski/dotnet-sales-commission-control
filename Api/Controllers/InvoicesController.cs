using Application.DTOs.Invoice;
using Application.Interfaces;
using Domain.Entities;
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

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> Cancelar([FromRoute] Guid id)
        {
            try
            {
                await _invoiceApplication.CancelarAsync(id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateInvoiceDto dto)
        {
            try
            {
                await _invoiceApplication.UpdateAsync(id, dto);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] Guid? vendedorId)
        {
            try
            {
               var invoices = await _invoiceApplication.GetAllAsync(vendedorId);

                return Ok(invoices);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            try
            {
                var invoices = await _invoiceApplication.ObterPorIdAsync(id);

                return Ok(invoices);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
