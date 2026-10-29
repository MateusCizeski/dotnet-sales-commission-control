using ApiBase.Application.ApplicationGuid;
using ApiBase.Controller.BaseGuid;
using Application.Queries;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/commissions")]
    [Authorize(Roles = Roles.AdminOrSeller)]
    public class CommissionsController : GuidController<IApplicationGuid<CommissionView>, CommissionView>
    {
        private readonly ICommissionService _commissionService;

        public CommissionsController(IApplicationGuid<CommissionView> queries, ICommissionService commissionService, ILogger<CommissionsController> logger)
            : base(queries, logger)
        {
            _commissionService = commissionService;
        }

        [HttpPost("{id:guid}/pay")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Pay(Guid id, CancellationToken cancellationToken)
        {
            await _commissionService.PayAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/cancel")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            await _commissionService.CancelAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
