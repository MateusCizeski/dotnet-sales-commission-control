using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Vendedor
{
    public class CriarVendedorDto
    {
        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome não pode ter mais de 200 caracteres")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$|^\d{11}$", ErrorMessage = "CPF inválido. Use 123.456.789-09 ou 12345678909")]
        public string Cpf { get; set; } = string.Empty!;

        [Required(ErrorMessage = "Percentual de comissão é obrigatório")]
        [Range(0, 15, ErrorMessage = "Percentual de comissão deve ser entre 0 e 15%")]
        public decimal PercentualComissao { get; set; }

        public string? Telefone { get; set; }
    }
}
