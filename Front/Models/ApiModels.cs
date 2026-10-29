using System.ComponentModel.DataAnnotations;

namespace Front.Models
{
    // The Front's own copies of the API contract. It does not reference the backend projects:
    // the two applications only share the HTTP/JSON contract.

    public enum InvoiceStatus { Pending = 1, Approved = 2, Cancelled = 3 }

    public enum CommissionStatus { Pending = 1, Paid = 2, Cancelled = 3 }

    /// <summary>List response of the ApiBase endpoints: { "content": [...], "total": 42 }.</summary>
    public class PageResult<T>
    {
        public List<T> Content { get; set; } = [];
        public int Total { get; set; }
    }

    public record LoginResponse(string AccessToken, DateTime ExpiresAt, string Role, Guid? SellerId);

    public class SellerItem
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public decimal CommissionRate { get; set; }
        public bool IsActive { get; set; }
    }

    public class SellerOption
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class SellerSummary
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class InvoiceItem
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public Guid SellerId { get; set; }
        public SellerSummary? Seller { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
    }

    public class CommissionItem
    {
        public Guid Id { get; set; }
        public CommissionInvoice? Invoice { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public CommissionStatus Status { get; set; }
        public DateTime CalculatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class CommissionInvoice
    {
        public string Number { get; set; } = string.Empty;
        public SellerSummary? Seller { get; set; }
        public InvoiceStatus Status { get; set; }
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
        public string FullName { get; set; } = string.Empty;
        public decimal TotalCommission { get; set; }
    }

    // ---------- Forms (validated in the browser and again by the API) ----------

    public class LoginForm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class SellerForm
    {
        [Required, StringLength(200), Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$|^\d{11}$", ErrorMessage = "Use 123.456.789-09 or 12345678909.")]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Range(0, 15), Display(Name = "Commission rate (%)")]
        public decimal CommissionRate { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }
    }

    public class SellerEditForm
    {
        [Required, StringLength(200), Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Range(0, 15), Display(Name = "Commission rate (%)")]
        public decimal CommissionRate { get; set; }
    }

    public class InvoiceForm
    {
        [Required, Display(Name = "Seller")]
        public Guid? SellerId { get; set; }

        [Range(0.01, double.MaxValue), Display(Name = "Total amount")]
        public decimal TotalAmount { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Issue date")]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Required, StringLength(200), Display(Name = "Customer")]
        public string CustomerName { get; set; } = string.Empty;

        [Required, StringLength(18), Display(Name = "Customer CPF/CNPJ")]
        public string CustomerTaxId { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class InvoiceEditForm
    {
        [Required, Display(Name = "Seller")]
        public Guid? SellerId { get; set; }

        [Range(0.01, double.MaxValue), Display(Name = "Total amount")]
        public decimal TotalAmount { get; set; }
    }
}
