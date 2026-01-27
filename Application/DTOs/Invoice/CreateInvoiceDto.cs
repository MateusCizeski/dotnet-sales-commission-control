using Domain.Enums;

namespace Application.DTOs.Invoice
{
    public class CreateInvoiceDto
    {
        public Guid VendedorId { get; set; }
        public decimal ValorTotal { get; set; }
        public string NumeroInvoice { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Cliente { get; set; }
        public string CnpjCpfCliente { get; set; }
        public StatusInvoice Status { get; set; }
        public string? Observacoes { get; set; }
    }
}
