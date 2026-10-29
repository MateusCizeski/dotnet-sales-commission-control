using Domain.Enums;

namespace Application.Queries
{
    /// <summary>
    /// Dashboard totals. Implemented in Infra with COUNT/SUM/GROUP BY in the database —
    /// the Application layer does not know EF Core.
    /// For a Seller user, the implementation always restricts to that seller.
    /// </summary>
    public interface IDashboardQueries
    {
        Task<DashboardSummary> GetSummaryAsync(DashboardFilter filter, CancellationToken cancellationToken = default);
    }

    public class DashboardFilter
    {
        /// <summary>First issue date included (date only).</summary>
        public DateTime? StartDate { get; set; }

        /// <summary>Last issue date included (date only; the whole day counts).</summary>
        public DateTime? EndDate { get; set; }

        public Guid? SellerId { get; set; }
        public InvoiceStatus? Status { get; set; }
    }

    public class DashboardSummary
    {
        public int TotalInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public int ApprovedInvoices { get; set; }
        public int CancelledInvoices { get; set; }
        public decimal ApprovedAmount { get; set; }
        public int InvoicesLast30Days { get; set; }
        public decimal PendingCommissions { get; set; }
        public decimal PaidCommissions { get; set; }
        public List<TopSeller> TopSellers { get; set; } = [];
    }

    public class TopSeller
    {
        public Guid SellerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal TotalCommission { get; set; }
    }
}
