using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Vendedor
{
    public class UpdateVendedorDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Range(0, 15, ErrorMessage = "O percentual deve estar entre 0% e 15%")]
        public decimal PercentualComissao { get; set; }
    }
}
