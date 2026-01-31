using Domain.Entities;
using Domain.Exceptions;
using Domain.Validation;
using Xunit;

namespace Tests.Domain
{
    public class InvoiceValidationTests
    {
        private Vendedor _vendedorAtivo = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);

        [Fact]
        public void ValorTotalZero_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() => new Invoice(_vendedorAtivo, "INV-0001", DateTime.UtcNow, "Cliente", "12345678909", 0));
        }

        [Fact]
        public void ClienteVazio_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() =>
                new Invoice(_vendedorAtivo, "INV-0001", DateTime.UtcNow, "", "12345678909", 100)
            );
        }

        [Fact]
        public void CnpjCpfInvalido_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() =>
                new Invoice(_vendedorAtivo, "INV-0001", DateTime.UtcNow, "Cliente", "00000000000", 100)
            );
        }

        [Fact]
        public void VendedorInativo_DeveLancarDomainException()
        {
            var vendedorInativo = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);

            vendedorInativo.Inativar();

            Assert.Throws<DomainException>(() =>
                new Invoice(vendedorInativo, "INV-0001", DateTime.UtcNow, "Cliente", "12345678909", 100)
            );
        }

        [Fact]
        public void InvoiceValida_DeveCriarComissaoAutomaticamente()
        {
            var invoice = new Invoice(_vendedorAtivo, "INV-0001", DateTime.UtcNow, "Cliente", "12345678909", 1000);

            Assert.NotNull(invoice.Comissao);
            Assert.Equal(1000, invoice.Comissao.ValorBase);
            Assert.Equal(10, invoice.Comissao.PercentualAplicado);
        }
    }
}
