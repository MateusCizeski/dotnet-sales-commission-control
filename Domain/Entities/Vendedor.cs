using Domain.Validation;

namespace Domain.Entities
{
    public class Vendedor
    {
        public Guid Id { get; private set; }
        public string NomeCompleto { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get; private set; }
        public string? Telefone { get; private set; }
        public decimal PercentualComissao { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }

        protected Vendedor() { }

        public Vendedor(string nomeCompleto, string cpf, string email, decimal percentualComissao, string? telefone = null)
        {
            VendedorValidation.ValidarVendedor(nomeCompleto, cpf, email, percentualComissao);

            Id = Guid.NewGuid();
            NomeCompleto = nomeCompleto;
            Cpf = cpf;
            Email = email;
            PercentualComissao = percentualComissao;
            Telefone = telefone;
            Ativo = true;
            DataCadastro = DateTime.UtcNow;
        }

        public void Inativar()
        {
            Ativo = false;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public bool PodeReceberCommissao()
        {
            return Ativo; 
        }
    }
}
