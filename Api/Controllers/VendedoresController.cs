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
        public async Task<IActionResult> Criar([FromBody] CriarVendedorDto dto)
        {
            var id = await _vendedorApplication.Criar(dto);
            return Ok(id);
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IReadOnlyList<VendedorDto>>> Listar()
        {
            var vendedores = await _vendedorApplication.Listar();
            return Ok(vendedores);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VendedorDto>>> ListarPaginado([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var vendedores = await _vendedorApplication.ListarPaginado(page, pageSize);
            return Ok(vendedores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendedorDto>> ListarPorId([FromRoute] Guid id)
        {

            var vendedor = await _vendedorApplication.ListarPorId(id);
            return Ok(vendedor);

        }

        [HttpPut("{id}/inativar")]
        public async Task<IActionResult> Inativar([FromRoute] Guid id)
        {
            await _vendedorApplication.Inativar(id);
            return NoContent();
        }

        [HttpPut("{id}/ativar")]
        public async Task<IActionResult> Ativar([FromRoute] Guid id)
        {
            await _vendedorApplication.Ativar(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar([FromRoute] Guid id, [FromBody] EditarVendedorDto dto)
        {

            await _vendedorApplication.Editar(id, dto);
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
