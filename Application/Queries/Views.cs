using ApiBase.Domain.View;
using Domain.Enums;

namespace Application.Queries
{
    // Read models returned by the list/detail endpoints (ApiBase projects the entities into them in SQL).
    // Properties are matched by name with the entity; nested views become JOINs.
    // Only what is declared here is returned and can be filtered or sorted.

    public class SellerView : IdGuidView
    {
        public string FullName { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public decimal CommissionRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>Small item for dropdowns.</summary>
    public class SellerOptionView
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class SellerSummaryView
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class InvoiceView : IdGuidView
    {
        public string Number { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public Guid SellerId { get; set; }
        public SellerSummaryView? Seller { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerTaxId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? Notes { get; set; }
        public InvoiceCommissionView? Commission { get; set; }
    }

    public class InvoiceCommissionView
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public CommissionStatus Status { get; set; }
    }

    public class CommissionView : IdGuidView
    {
        public Guid InvoiceId { get; set; }
        public CommissionInvoiceView? Invoice { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public CommissionStatus Status { get; set; }
        public DateTime CalculatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class CommissionInvoiceView
    {
        public string Number { get; set; } = string.Empty;
        public Guid SellerId { get; set; }
        public SellerSummaryView? Seller { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
