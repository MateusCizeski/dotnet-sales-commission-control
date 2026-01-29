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
            try
            {
                await _comissaoApplication.MarcarComoPagaAsync(id);
                return NoContent();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> MarcarComoCancelada([FromRoute] Guid id)
        {
            try
            {
                await _comissaoApplication.MarcarComoCanceladaAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var comissoes = await _comissaoApplication.GetAllAsync();

                return Ok(comissoes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
