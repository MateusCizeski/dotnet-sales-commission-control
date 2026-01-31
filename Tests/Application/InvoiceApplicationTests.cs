using Application.Applications;
using Application.DTOs.Invoice;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests.Application
{
    public class InvoiceApplicationTests
    {
        private readonly Mock<IInvoiceRepository> _invoiceRepo = new();
        private readonly Mock<IVendedorRepository> _vendedorRepo = new();
        private readonly Mock<IComissaoRepository> _comissaoRepo = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly InvoiceApplication _app;

        public InvoiceApplicationTests()
        {
            _app = new InvoiceApplication(
                _invoiceRepo.Object,
                _vendedorRepo.Object,
                _comissaoRepo.Object,
                _unitOfWork.Object
            );
        }

        [Fact]
        public async Task CriarAsync_ChamaRepositoriosECommit()
        {
            var vendedorId = Guid.NewGuid();
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            _vendedorRepo.Setup(r => r.ListarPorId(vendedorId)).ReturnsAsync(vendedor);

            var dto = new CriarInvoiceDto
            {
                VendedorId = vendedorId,
                ValorTotal = 1000,
                Cliente = "Cliente X",
                CnpjCpfCliente = "12345678909",
                DataEmissao = DateTime.UtcNow
            };

            var invoiceId = await _app.Criar(dto);

            _invoiceRepo.Verify(r => r.Criar(It.IsAny<Invoice>()), Times.Once);
            _comissaoRepo.Verify(r => r.Criar(It.IsAny<Comissao>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Exactly(2));
            Assert.NotEqual(Guid.Empty, invoiceId);
        }

        [Fact]
        public async Task CriarAsync_VendedorNaoEncontrado_LancaException()
        {
            _vendedorRepo.Setup(r => r.ListarPorId(It.IsAny<Guid>())).ReturnsAsync((Vendedor)null);

            var dto = new CriarInvoiceDto
            {
                VendedorId = Guid.NewGuid(),
                ValorTotal = 1000,
                Cliente = "Cliente X",
                CnpjCpfCliente = "12345678909",
                DataEmissao = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<Exception>(() => _app.Criar(dto));
        }
    }
}
