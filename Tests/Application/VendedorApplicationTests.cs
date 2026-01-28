using Application.Applications;
using Application.DTOs.Vendedor;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tests.Application
{
    public class VendedorApplicationTests
    {
        private readonly Mock<IVendedorRepository> _vendedorRepoMock;
        private readonly Mock<IComissaoRepository> _comissaoRepoMock;
        private readonly VendedorApplication _app;

        public VendedorApplicationTests()
        {
            _vendedorRepoMock = new Mock<IVendedorRepository>();
            _comissaoRepoMock = new Mock<IComissaoRepository>();
            _app = new VendedorApplication(_vendedorRepoMock.Object, _comissaoRepoMock.Object);
        }

        [Fact]
        public async Task CriarAsync_ChamaAddAsync()
        {
            var dto = new CreateVendedorDto { NomeCompleto = "Teste", Cpf = "12345678909", Email = "teste@teste.com", PercentualComissao = 10 };

            await _app.CriarAsync(dto);

            _vendedorRepoMock.Verify(r => r.AddAsync(It.IsAny<Vendedor>()), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDto()
        {
            _vendedorRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Vendedor> { new Vendedor("Nome", "12345678909", "teste@teste.com", 10) });

            var result = await _app.ObterTodosAsync();

            Assert.Single(result);
            Assert.Equal("Nome", result[0].Nome);
        }
    }
}
