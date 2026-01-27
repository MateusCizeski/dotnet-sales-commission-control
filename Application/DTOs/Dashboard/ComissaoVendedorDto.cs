namespace Application.DTOs.Dashboard
{
    public class ComissaoVendedorDto
    {
        public Guid VendedorId { get; set; }
        public string NomeVendedor { get; set; } = string.Empty;
        public decimal TotalComissoes { get; set; }
        public decimal TotalPendentes { get; set; }
        public decimal TotalPagas { get; set; }
        public List<ComissaoDetalheDto> Comissoes { get; set; } = new();
    }

    public class ComissaoDetalheDto
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal ValorComissao { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Paga { get; set; }
    }
}
