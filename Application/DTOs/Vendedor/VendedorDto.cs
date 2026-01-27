namespace Application.DTOs.Vendedor
{
    public class VendedorDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public decimal PercentualComissao { get; set; }
        public bool Ativo { get; set; }
    }
}
