using Domain.Entities;
using Domain.Validation;
using Xunit;

namespace Tests.Domain
{
    public class InvoiceValidationTests
    {
        private Vendedor vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);

        [Fact]
        public void ValorTotalNegativo_DeveLancarErro()
        {
            Assert.Throws<ArgumentException>(() =>
                InvoiceValidation.ValidarInvoice(vendedor, "Cliente", "12345678909", 0, DateTime.UtcNow, null)
            );
        }

        [Fact]
        public void ClienteVazio_DeveLancarErro()
        {
            Assert.Throws<ArgumentException>(() =>
                InvoiceValidation.ValidarInvoice(vendedor, "", "12345678909", 100, DateTime.UtcNow, null)
            );
        }

        [Fact]
        public void CnpjCpfInvalido_DeveLancarErro()
        {
            Assert.Throws<ArgumentException>(() =>
                InvoiceValidation.ValidarInvoice(vendedor, "Cliente", "00000000000", 100, DateTime.UtcNow, null)
            );
        }
    }
}
