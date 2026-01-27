using Domain.Enums;

namespace Application.DTOs.Invoice
{
    public class CreateInvoiceDto
    {
        public Guid VendedorId { get; set; }
        public decimal ValorTotal { get; set; }
        public string NumeroInvoice { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public string Cliente { get; private set; }
        public string CnpjCpfCliente { get; private set; }
        public StatusInvoice Status { get; private set; }
        public string? Observacoes { get; private set; }
    }
}
