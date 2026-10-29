using ApiBase.Controller.BaseGuid;
using Application.Queries;
using Application.Requests;
using Application.Services;
using Infra.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// GET /api/sellers (filter, sort, page, fields) and GET /api/sellers/{id} come from ApiBase's GuidController.
    /// Sellers contain personal data (CPF, email): Admin only.
    /// </summary>
    [ApiController]
    [Route("api/sellers")]
    [Authorize(Roles = Roles.Admin)]
    public class SellersController : GuidController<ISellerQueries, SellerView>
    {
        private readonly ISellerService _sellerService;

        public SellersController(ISellerQueries queries, ISellerService sellerService, ILogger<SellersController> logger)
            : base(queries, logger)
        {
            _sellerService = sellerService;
        }

        /// <summary>Active sellers (id and name) for dropdowns.</summary>
        [HttpGet("options")]
        [ProducesResponseType(typeof(List<SellerOptionView>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
        {
            return Ok(await Application.GetActiveOptionsAsync(cancellationToken));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSellerRequest request, CancellationToken cancellationToken)
        {
            var id = await _sellerService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSellerRequest request, CancellationToken cancellationToken)
        {
            await _sellerService.UpdateAsync(id, request, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            await _sellerService.ActivateAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>Sellers with registered commissions cannot be deactivated.</summary>
        [HttpPost("{id:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            await _sellerService.DeactivateAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>Sellers with registered commissions cannot be deleted.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _sellerService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
