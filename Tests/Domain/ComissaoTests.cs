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
            comissao.MarcarComoPaga();

            Assert.Equal(StatusComissao.Paga, comissao.Status);
            Assert.NotNull(comissao.DataPagamento);
        }
    }
}
