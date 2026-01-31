using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Invoice
{
    public class CreateInvoiceDto
    {
        [Required(ErrorMessage = "Vendedor é obrigatório.")]
        public Guid VendedorId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor total deve ser maior que zero.")]
        public decimal ValorTotal { get; set; }
        public DateTime DataEmissao { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório.")]
        public string Cliente { get; set; }

        [Required(ErrorMessage = "CPF/CNPJ do cliente é obrigatório.")]
        public string CnpjCpfCliente { get; set; }
        public StatusInvoice Status { get; set; }
        public string? Observacoes { get; set; }
    }
}
