namespace Application.DTOs.Invoice
{
    public class UpdateInvoiceDto
    {
        public Guid Id { get; set; }
        public Guid vendedorId { get; set; }
    }
}
