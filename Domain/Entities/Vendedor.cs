using Domain.Exceptions;
using Domain.Validation;
using System.Text.RegularExpressions;

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
            if (nomeCompleto == null) throw new DomainException("Nome completo é obrigatório");
            if (cpf == null) throw new DomainException("CPF inválido");
            if (email == null) throw new DomainException("Email inválido");

            cpf = NormalizarCpf(cpf);
            ValidarNome(nomeCompleto);
            ValidarCpf(cpf);
            ValidarEmail(email);
            ValidarPercentual(percentualComissao);

            Id = Guid.NewGuid();
            NomeCompleto = nomeCompleto;
            Cpf = cpf;
            Email = email;
            PercentualComissao = percentualComissao;
            Telefone = telefone;
            Ativo = true;
            DataCadastro = DateTime.UtcNow;
        }

        public void AtualizarDados(string nome, decimal percentual)
        {
            ValidarNome(nome);
            ValidarPercentual(percentual);

            NomeCompleto = nome;
            PercentualComissao = percentual;
        }

        public void Inativar() => Ativo = false;

        public void Ativar() => Ativo = true;

        public void GarantirQueEstaAtivo()
        {
            if (!Ativo)
            {
                throw new DomainException("Vendedor inativo");
            }
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new DomainException("Nome completo é obrigatório");
            }

            if (nome.Length > 200)
            {
                throw new DomainException("Nome completo excede 200 caracteres");
            }
        }

        private static void ValidarCpf(string cpf)
        {
            if (!CpfValidator.CpfIsValid(cpf))
            {
                throw new DomainException("CPF inválido");
            }
        }

        private static void ValidarEmail(string email)
        {
            if (!EmailValidator.EmailIsValid(email))
            {
                throw new DomainException("Email inválido");
            }
        }

        private static void ValidarPercentual(decimal percentual)
        {
            if (percentual < 0 || percentual > 15)
            {
                throw new DomainException("Percentual deve estar entre 0% e 15%");
            }
        }

        private static string NormalizarCpf(string cpf)
        {
            return Regex.Replace(cpf, "[^0-9]", "");
        }
    }
}
