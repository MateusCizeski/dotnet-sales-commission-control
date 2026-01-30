using Domain.Enums;

namespace Application.DTOs.Comissao
{
    public class ComissaoListDto
    {
        public Guid Id { get; set; }
        public string NumeroInvoice { get; set; } = string.Empty;
        public string VendedorNome { get; set; } = string.Empty;
        public decimal ValorBase { get; set; }
        public decimal PercentualAplicado { get; set; }
        public decimal ValorComissao { get; set; }
        public StatusComissao Status { get; set; }
        public DateTime DataCalculo { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}
