namespace Application.DTOs.Invoice
{
    public class InvoiceListDto
    {
        public Guid Id { get; set; }
        public string Numero { get; set; }
        public string Cliente { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Status { get; set; }
        public string VendedorNome { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
