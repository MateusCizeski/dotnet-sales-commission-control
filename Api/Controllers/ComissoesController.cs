using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/comissoes")]
    [ApiController]
    public class ComissoesController : ControllerBase
    {
        private readonly IComissaoApplication _comissaoApplication;

        public ComissoesController(IComissaoApplication comissaoApplication)
        {
            _comissaoApplication = comissaoApplication;
        }

        [HttpPut("{id}/pagar")]
        public async Task<IActionResult> MarcarComoPaga([FromRoute] Guid id)
        { 
            await _comissaoApplication.MarcarComoPaga(id);
            return NoContent();
        }

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> MarcarComoCancelada([FromRoute] Guid id)
        {
            await _comissaoApplication.MarcarComoCancelada(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var comissoes = await _comissaoApplication.ListarPaginado(page, pageSize);
            return Ok(comissoes);
        }
    }
}
