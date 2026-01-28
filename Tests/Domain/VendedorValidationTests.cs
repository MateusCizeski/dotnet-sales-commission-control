using Domain.Validation;
using Xunit;

namespace Tests.Domain
{
    public class VendedorValidationTests
    {
        [Theory]
        [InlineData("12345678909")] // CPF válido
        public void CpfValido_DeveRetornarTrue(string cpf)
        {
            Assert.True(CpfValidator.CpfIsValid(cpf));
        }

        [Theory]
        [InlineData("11111111111")] // CPF inválido
        [InlineData("")] // vazio
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

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(15)]
        public void PercentualValido_DevePassar(decimal percentual)
        {
            var ex = Record.Exception(() =>
                VendedorValidation.ValidarVendedor("Nome", "12345678909", "teste@teste.com", percentual)
            );
            Assert.Null(ex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(16)]
        public void PercentualInvalido_DeveLancarErro(decimal percentual)
        {
            Assert.Throws<ArgumentException>(() =>
                VendedorValidation.ValidarVendedor("Nome", "12345678909", "teste@teste.com", percentual)
            );
        }
    }
}
