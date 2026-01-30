namespace Application.DTOs.Invoice
{
    public class UpdateInvoiceDto
    {
        public Guid Id { get; set; }
        public Guid VendedorId { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
