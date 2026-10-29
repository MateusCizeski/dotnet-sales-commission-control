using System.ComponentModel.DataAnnotations;

namespace Application.Requests
{
    // Input models of the commands. DataAnnotations give a fast 400 at the API boundary;
    // the domain entities still validate everything (they are the source of truth).

    public class CreateSellerRequest
    {
        [Required, StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>Brazilian individual tax ID: 123.456.789-09 or 12345678909.</summary>
        [Required, RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$|^\d{11}$", ErrorMessage = "CPF must be 123.456.789-09 or 12345678909.")]
        public string Cpf { get; set; } = string.Empty;

        [Range(0, 15)]
        public decimal CommissionRate { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }
    }

    public class UpdateSellerRequest
    {
        [Required, StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Range(0, 15)]
        public decimal CommissionRate { get; set; }
    }

    public class CreateInvoiceRequest
    {
        [Required]
        public Guid SellerId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [Required, StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>Customer CPF or CNPJ (formatted or digits only).</summary>
        [Required, StringLength(18)]
        public string CustomerTaxId { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateInvoiceRequest
    {
        [Required]
        public Guid SellerId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }
    }

    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public record LoginResponse(string AccessToken, DateTime ExpiresAt, string Role, Guid? SellerId);
}
