using Domain.Entities;
using Domain.Exceptions;
using Domain.Validation;
using Xunit;

namespace Tests.Domain
{
    public class VendedorValidationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CriarVendedor_NomeInvalido_DeveLancarDomainException(string nome)
        {
            var ex = Assert.Throws<DomainException>(() =>  new Vendedor(nome, "12345678909", "teste@teste.com", 10));
            Assert.Equal("Nome completo é obrigatório", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("11111111111")]
        public void CriarVendedor_CpfInvalido_DeveLancarDomainException(string cpf)
        {
            var ex = Assert.Throws<DomainException>(() => new Vendedor("Nome", cpf, "teste@teste.com", 10));
            Assert.Equal("CPF inválido", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("emailinvalido")]
        public void CriarVendedor_EmailInvalido_DeveLancarDomainException(string email)
        {
            var ex = Assert.Throws<DomainException>(() => new Vendedor("Nome", "12345678909", email, 10));
            Assert.Equal("Email inválido", ex.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(16)]
        public void CriarVendedor_PercentualInvalido_DeveLancarDomainException(decimal percentual)
        {
            var ex = Assert.Throws<DomainException>(() => new Vendedor("Nome", "12345678909", "teste@teste.com", percentual));
            Assert.Equal("Percentual deve estar entre 0% e 15%", ex.Message);
        }

        [Fact]
        public void CriarVendedor_Valido_DeveCriarComSucesso()
        {
            var vendedor = new Vendedor("Nome Teste", "12345678909", "teste@teste.com", 10);

            Assert.NotNull(vendedor);
            Assert.Equal("Nome Teste", vendedor.NomeCompleto);
            Assert.Equal("12345678909", vendedor.Cpf);
            Assert.Equal("teste@teste.com", vendedor.Email);
            Assert.Equal(10, vendedor.PercentualComissao);
            Assert.True(vendedor.Ativo);
        }

        [Fact]
        public void AtualizarDados_Valido_DeveAtualizar()
        {
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            vendedor.AtualizarDados("Nome Atualizado", 12);

            Assert.Equal("Nome Atualizado", vendedor.NomeCompleto);
            Assert.Equal(12, vendedor.PercentualComissao);
        }

        [Fact]
        public void Inativar_DeveSetarAtivoFalse()
        {
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            vendedor.Inativar();

            Assert.False(vendedor.Ativo);
        }

        [Fact]
        public void Ativar_DeveSetarAtivoTrue()
        {
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            vendedor.Inativar();
            vendedor.Ativar();

            Assert.True(vendedor.Ativo);
        }

        [Fact]
        public void GarantirQueEstaAtivo_VendedorInativo_DeveLancarDomainException()
        {
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            vendedor.Inativar();

            var ex = Assert.Throws<DomainException>(() => vendedor.GarantirQueEstaAtivo());
            Assert.Equal("Vendedor inativo", ex.Message);
        }

        [Theory]
        [InlineData("12345678909")]
        public void CpfValido_DeveRetornarTrue(string cpf)
        {
            Assert.True(CpfValidator.CpfIsValid(cpf));
        }

        [Theory]
        [InlineData("11111111111")]
        [InlineData("")]
        public void CpfInvalido_DeveRetornarFalse(string cpf)
        {
            Assert.False(CpfValidator.CpfIsValid(cpf));
        }

        [Theory]
        [InlineData("email@teste.com")]
        public void EmailValido_DeveRetornarTrue(string email)
        {
            Assert.True(EmailValidator.EmailIsValid(email));
        }

        [Theory]
        [InlineData("emailinvalido")]
        [InlineData("")]
        public void EmailInvalido_DeveRetornarFalse(string email)
        {
            Assert.False(EmailValidator.EmailIsValid(email));
        }
    }
}
