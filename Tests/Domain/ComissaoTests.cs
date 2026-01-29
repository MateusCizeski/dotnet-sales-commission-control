using Domain.Entities;
using Domain.Enums;
using Xunit;

namespace Tests.Domain
{
    public class ComissaoTests
    {
        [Fact]
        public void CalculoComissao_Correto()
        {
            var comissao = new Comissao(Guid.NewGuid(), 1000m, 10);

            Assert.Equal(100, comissao.ValorComissao);
            Assert.Equal(StatusComissao.Pendente, comissao.Status);
        }

        [Fact]
        public void MarcarComoPaga_AtualizaStatusEData()
        {
            var comissao = new Comissao(Guid.NewGuid(), 1000m, 10);
            comissao.Pagar();

            Assert.Equal(StatusComissao.Paga, comissao.Status);
            Assert.NotNull(comissao.DataPagamento);
        }

        [Theory]
        [InlineData(1000, 12.5, 125)] // 12,5% de 1000 = 125
        [InlineData(999.99, 10, 100)] // 999.99 * 10% = 99.999 → 100
        [InlineData(1234.56, 7.5, 92.59)] // 1234.56 * 7,5% = 92.592 → 92.59
        public void CalculoComissao_ArredondamentoCorreto(decimal valorTotal, decimal percentual, decimal esperado)
        {
            var comissao = new Comissao(Guid.NewGuid(), valorTotal, percentual);

            Assert.Equal(esperado, comissao.ValorComissao);
        }

        [Theory]
        [InlineData(1000, 5, 50)]
        [InlineData(1000, 10, 100)]
        [InlineData(1000, 15, 150)]
        [InlineData(2000, 12.5, 250)]
        public void CalculoComissao_DiferentesPercentuais(decimal valorTotal, decimal percentual, decimal esperado)
        {
            var comissao = new Comissao(Guid.NewGuid(), valorTotal, percentual);

            Assert.Equal(esperado, comissao.ValorComissao);
        }
    }
}
