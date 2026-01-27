namespace Application.DTOs.Comissao
{
    public class ComissaoDto
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal ValorBase { get; set; }
        public decimal Percentual { get; set; }
        public decimal ValorComissao { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
