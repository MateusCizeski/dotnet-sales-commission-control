namespace Application.DTOs.Vendedor
{
    public class CreateVendedorDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty!;
        public decimal PercentualComissao { get; set; }
    }
}
