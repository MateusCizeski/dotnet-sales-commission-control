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
            var id = await _vendedorApplication.CriarAsync(dto);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VendedorDto>>> ObterTodos()
        {
            var vendedores = await _vendedorApplication.ObterTodosAsync();
            return Ok(vendedores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendedorDto>> ObterPorId([FromRoute] Guid id)
        {

            var vendedor = await _vendedorApplication.ObterPorIdAsync(id);
            return Ok(vendedor);

        }

        [HttpPut("{id}/inativar")]
        public async Task<IActionResult> Inativar([FromRoute] Guid id)
        {
            await _vendedorApplication.InativarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/ativar")]
        public async Task<IActionResult> Ativar([FromRoute] Guid id)
        {
            await _vendedorApplication.AtivarAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar([FromRoute] Guid id, [FromBody] UpdateVendedorDto dto)
        {

            await _vendedorApplication.Atualizar(id, dto);
            return NoContent();
        }

        [HttpPut("{id}/remover")]
        public async Task<IActionResult> Remover([FromRoute] Guid id)
        {
            await _vendedorApplication.Remover(id);
            return NoContent();
        }
    }
}
