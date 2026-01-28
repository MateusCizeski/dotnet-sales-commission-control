using Application.DTOs.Vendedor;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/vendedores")]
    [ApiController]
    public class VendedoresController : ControllerBase
    {
        private readonly IVendedorApplication _vendedorApplication;

        public VendedoresController(IVendedorApplication vendedorApplication)
        {
            _vendedorApplication = vendedorApplication;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CreateVendedorDto dto)
        {
            try
            {
                var id = await _vendedorApplication.CriarAsync(dto);

                return Ok(id);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VendedorDto>>> ObterTodos()
        {
            try
            {
                var vendedores = await _vendedorApplication.ObterTodosAsync();

                return Ok(vendedores);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendedorDto>> ObterPorId([FromRoute] Guid id)
        {
            try
            {
                var vendedor = await _vendedorApplication.ObterPorIdAsync(id);

                return Ok(vendedor);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/inativar")]
        public async Task<IActionResult> Inativar([FromRoute] Guid id)
        {
            try
            {
                await _vendedorApplication.InativarAsync(id);
                return NoContent();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/remover")]
        public async Task<IActionResult> Remover([FromRoute] Guid id)
        {
            try
            {
                await _vendedorApplication.Remover(id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
