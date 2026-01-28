using Application.Applications;
using Application.DTOs.Invoice;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;
using Moq;
using Xunit;

namespace Tests.Application
{
    public class InvoiceApplicationTests
    {
        private readonly Mock<IInvoiceRepository> _invoiceRepo = new();
        private readonly Mock<IVendedorRepository> _vendedorRepo = new();
        private readonly Mock<IComissaoRepository> _comissaoRepo = new();
        private readonly InvoiceApplication _app;
        private readonly ComissaoService _comissaoService = new();

        public InvoiceApplicationTests()
        {
            _app = new InvoiceApplication(_invoiceRepo.Object, _vendedorRepo.Object, _comissaoRepo.Object, _comissaoService);
        }

        [Fact]
        public async Task CriarAsync_ChamaRepositorios()
        {
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            _vendedorRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(vendedor);

            var dto = new CreateInvoiceDto
            {
                VendedorId = Guid.NewGuid(),
                ValorTotal = 1000,
                Cliente = "Cliente X",
                CnpjCpfCliente = "12345678909",
                DataEmissao = DateTime.UtcNow
            };

            await _app.CriarAsync(dto);

            _invoiceRepo.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once);
            _comissaoRepo.Verify(r => r.AddAsync(It.IsAny<Comissao>()), Times.Once);
        }
    }
}
