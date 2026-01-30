namespace Application.DTOs.Invoice
{
    public class InvoiceEditDto
    {
        public Guid Id { get; set; }
        public string Numero { get; set; } = "";
        public string Cliente { get; set; } = "";
        public DateTime DataEmissao { get; set; }
        public string Status { get; set; } = "";

        public Guid VendedorId { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
