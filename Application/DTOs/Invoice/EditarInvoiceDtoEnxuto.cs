namespace Application.DTOs.Invoice
{
    public class EditarInvoiceDtoEnxuto
    {
        public Guid Id { get; set; }
        public Guid VendedorId { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
