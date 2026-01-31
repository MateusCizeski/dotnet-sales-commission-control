using Domain.Enums;

namespace Application.DTOs.Invoice
{
    public class InvoiceListDto
    {
        public Guid Id { get; set; }
        public string NumeroInvoice { get; set; }
        public string Cliente { get; set; }
        public DateTime DataEmissao { get; set; }
        public StatusInvoice Status { get; set; }
        public decimal ValorTotal { get; set; }
        public VendedorDto Vendedor { get; set; } = new();
    }

    public class VendedorDto
    {
        public Guid Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
    }
}
