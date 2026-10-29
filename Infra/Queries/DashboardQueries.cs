using Application.Abstractions;
using Application.Queries;
using Domain.Entities;
using Domain.Enums;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infra.Queries
{
    /// <summary>
    /// Dashboard totals computed by the database (COUNT, SUM, GROUP BY, TOP 5).
    /// Only small result rows travel to the API — never the invoices themselves.
    /// </summary>
    public class DashboardQueries : IDashboardQueries
    {
        private readonly PortalDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly TimeProvider _timeProvider;

        public DashboardQueries(PortalDbContext context, ICurrentUser currentUser, TimeProvider timeProvider)
        {
            _context = context;
            _currentUser = currentUser;
            _timeProvider = timeProvider;
        }

        public async Task<DashboardSummary> GetSummaryAsync(DashboardFilter filter, CancellationToken cancellationToken = default)
        {
            IQueryable<Invoice> invoices = ApplyFilter(_context.Invoices.AsNoTracking(), filter);

            var byStatus = await invoices
                .GroupBy(i => i.Status)
                .Select(g => new { Status = g.Key, Count = g.Count(), Amount = g.Sum(i => i.TotalAmount) })
                .ToListAsync(cancellationToken);

            DateTime since = _timeProvider.GetUtcNow().UtcDateTime.AddDays(-30);
            int last30Days = await invoices.CountAsync(i => i.IssueDate >= since, cancellationToken);

            var commissionsByStatus = await invoices
                .Select(i => i.Commission)
                .GroupBy(c => c.Status)
                .Select(g => new { Status = g.Key, Amount = g.Sum(c => c.Amount) })
                .ToListAsync(cancellationToken);

            // Cancelled commissions do not count in the ranking.
            var topSellers = await invoices
                .Where(i => i.Commission.Status != CommissionStatus.Cancelled)
                .GroupBy(i => new { i.SellerId, i.Seller!.FullName })
                .Select(g => new TopSeller
                {
                    SellerId = g.Key.SellerId,
                    FullName = g.Key.FullName,
                    TotalCommission = g.Sum(i => i.Commission.Amount)
                })
                .OrderByDescending(t => t.TotalCommission)
                .Take(5)
                .ToListAsync(cancellationToken);

            return new DashboardSummary
            {
                TotalInvoices = byStatus.Sum(s => s.Count),
                PendingInvoices = byStatus.Where(s => s.Status == InvoiceStatus.Pending).Sum(s => s.Count),
                ApprovedInvoices = byStatus.Where(s => s.Status == InvoiceStatus.Approved).Sum(s => s.Count),
                CancelledInvoices = byStatus.Where(s => s.Status == InvoiceStatus.Cancelled).Sum(s => s.Count),
                ApprovedAmount = byStatus.Where(s => s.Status == InvoiceStatus.Approved).Sum(s => s.Amount),
                InvoicesLast30Days = last30Days,
                PendingCommissions = commissionsByStatus.Where(c => c.Status == CommissionStatus.Pending).Sum(c => c.Amount),
                PaidCommissions = commissionsByStatus.Where(c => c.Status == CommissionStatus.Paid).Sum(c => c.Amount),
                TopSellers = topSellers
            };
        }

        private IQueryable<Invoice> ApplyFilter(IQueryable<Invoice> invoices, DashboardFilter filter)
        {
            // A Seller user always sees only their own numbers, whatever SellerId the request sends.
            Guid? sellerId = _currentUser.IsAdmin ? filter.SellerId : _currentUser.SellerId ?? Guid.Empty;

            if (filter.StartDate.HasValue)
            {
                DateTime start = filter.StartDate.Value.Date;
                invoices = invoices.Where(i => i.IssueDate >= start);
            }

            if (filter.EndDate.HasValue)
            {
                // "< next day" keeps invoices issued during the last day (IssueDate may have a time part).
                DateTime endExclusive = filter.EndDate.Value.Date.AddDays(1);
                invoices = invoices.Where(i => i.IssueDate < endExclusive);
            }

            if (sellerId.HasValue)
            {
                invoices = invoices.Where(i => i.SellerId == sellerId.Value);
            }

            if (filter.Status.HasValue)
            {
                invoices = invoices.Where(i => i.Status == filter.Status.Value);
            }

            return invoices;
        }
    }
}
