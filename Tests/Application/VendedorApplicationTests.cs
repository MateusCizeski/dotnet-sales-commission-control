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
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VendedorApplication _app;

        public VendedorApplicationTests()
        {
            _vendedorRepoMock = new Mock<IVendedorRepository>();
            _comissaoRepoMock = new Mock<IComissaoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            _app = new VendedorApplication(_vendedorRepoMock.Object, _comissaoRepoMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CriarAsync_ChamaAddAsyncECommitAsync()
        {
            var dto = new CreateVendedorDto
            {
                NomeCompleto = "Teste",
                Cpf = "12345678909",
                Email = "teste@teste.com",
                PercentualComissao = 10
            };

            var id = await _app.CriarAsync(dto);

            _vendedorRepoMock.Verify(r => r.AddAsync(It.IsAny<Vendedor>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDto()
        {
            _vendedorRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Vendedor>
                {
                    new Vendedor("Nome", "12345678909", "teste@teste.com", 10)
                });

            var result = await _app.ObterTodosAsync();

            Assert.Single(result);
            Assert.Equal("Nome", result[0].Nome);
        }

        [Fact]
        public async Task Atualizar_Existente_DeveChamarUpdateECommit()
        {
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            _vendedorRepoMock.Setup(r => r.GetByIdAsync(vendedor.Id)).ReturnsAsync(vendedor);

            var dto = new UpdateVendedorDto { Nome = "Nome Atualizado", PercentualComissao = 12 };

            await _app.Atualizar(vendedor.Id, dto);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal("Nome Atualizado", vendedor.NomeCompleto);
            Assert.Equal(12, vendedor.PercentualComissao);
        }

        [Fact]
        public async Task Atualizar_Inexistente_DeveLancarApplicationException()
        {
            _vendedorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vendedor?)null);
            var dto = new UpdateVendedorDto { Nome = "Nome", PercentualComissao = 10 };

            await Assert.ThrowsAsync<ApplicationException>(() => _app.Atualizar(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task Inativar_ComComissao_DeveLancarApplicationException()
        {
            var vendedorId = Guid.NewGuid();
            _vendedorRepoMock.Setup(r => r.GetByIdAsync(vendedorId)).ReturnsAsync(new Vendedor("Nome", "12345678909", "teste@teste.com", 10));
            _comissaoRepoMock.Setup(c => c.ExisteComissaoParaVendedor(vendedorId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ApplicationException>(() => _app.InativarAsync(vendedorId));
        }

        [Fact]
        public async Task Inativar_SemComissao_DeveChamarUpdateECommit()
        {
            var vendedorId = Guid.NewGuid();
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);

            _vendedorRepoMock.Setup(r => r.GetByIdAsync(vendedorId)).ReturnsAsync(vendedor);
            _comissaoRepoMock.Setup(c => c.ExisteComissaoParaVendedor(vendedorId)).ReturnsAsync(false);

            await _app.InativarAsync(vendedorId);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            Assert.False(vendedor.Ativo);
        }

        [Fact]
        public async Task Ativar_DeveChamarUpdateECommit()
        {
            var vendedorId = Guid.NewGuid();
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            vendedor.Inativar();

            _vendedorRepoMock.Setup(r => r.GetByIdAsync(vendedorId)).ReturnsAsync(vendedor);

            await _app.AtivarAsync(vendedorId);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            Assert.True(vendedor.Ativo);
        }

        [Fact]
        public async Task Remover_ComComissao_DeveLancarApplicationException()
        {
            var vendedorId = Guid.NewGuid();
            _vendedorRepoMock.Setup(r => r.GetByIdAsync(vendedorId)).ReturnsAsync(new Vendedor("Nome", "12345678909", "teste@teste.com", 10));
            _comissaoRepoMock.Setup(c => c.ExisteComissaoParaVendedor(vendedorId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ApplicationException>(() => _app.Remover(vendedorId));
        }

        [Fact]
        public async Task Remover_SemComissao_DeveChamarRemoveECommit()
        {
            var vendedorId = Guid.NewGuid();
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);

            _vendedorRepoMock.Setup(r => r.GetByIdAsync(vendedorId)).ReturnsAsync(vendedor);
            _comissaoRepoMock.Setup(c => c.ExisteComissaoParaVendedor(vendedorId)).ReturnsAsync(false);

            await _app.Remover(vendedorId);

            _vendedorRepoMock.Verify(r => r.RemoveAsync(vendedor), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterPorId_Existente_DeveRetornarDto()
        {
            var vendedor = new Vendedor("Nome", "12345678909", "teste@teste.com", 10);
            _vendedorRepoMock.Setup(r => r.GetByIdAsync(vendedor.Id)).ReturnsAsync(vendedor);

            var result = await _app.ObterPorIdAsync(vendedor.Id);

            Assert.Equal("Nome", result.Nome);
            Assert.Equal(vendedor.Cpf, result.Documento);
        }

        [Fact]
        public async Task ObterPorId_Inexistente_DeveLancarApplicationException()
        {
            _vendedorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vendedor?)null);

            await Assert.ThrowsAsync<ApplicationException>(() => _app.ObterPorIdAsync(Guid.NewGuid()));
        }
    }
}
