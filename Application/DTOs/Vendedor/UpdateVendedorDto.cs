namespace Application.DTOs.Vendedor
{
    public class UpdateVendedorDto
    {
        public string Nome { get; set; } = string.Empty;
        public decimal PercentualComissao { get; set; }
    }
}
