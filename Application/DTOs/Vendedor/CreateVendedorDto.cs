namespace Application.DTOs.Vendedor
{
    public class CreateVendedorDto
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty!;
        public decimal PercentualComissao { get; set; }
        public string? Telefone { get; set; }
    }
}
