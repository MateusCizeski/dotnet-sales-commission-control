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

        [HttpPut("{id:guid}/pagar")]
        public async Task<IActionResult> MarcarComoPaga(Guid id)
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
    }
}
