using ApiBase.Application.ApplicationGuid;
using ApiBase.Controller.BaseGuid;
using Application.Queries;
using Application.Requests;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Queries (GET list / GET by id) come from ApiBase; a Seller user only sees their own invoices.
    /// Commands (create, update, approve, cancel) go through IInvoiceService and the domain: Admin only.
    /// </summary>
    [ApiController]
    [Route("api/invoices")]
    [Authorize(Roles = Roles.AdminOrSeller)]
    public class InvoicesController : GuidController<IApplicationGuid<InvoiceView>, InvoiceView>
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IApplicationGuid<InvoiceView> queries, IInvoiceService invoiceService, ILogger<InvoicesController> logger)
            : base(queries, logger)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>Creates the invoice and its commission (seller's current rate) in one transaction.</summary>
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var id = await _invoiceService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        /// <summary>Changes seller and total of a pending invoice and recalculates the commission.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
        {
            await _invoiceService.UpdateAsync(id, request, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/approve")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
        {
            await _invoiceService.ApproveAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>Cancels the invoice and its pending commission (not allowed when the commission is paid).</summary>
        [HttpPost("{id:guid}/cancel")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            await _invoiceService.CancelAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
