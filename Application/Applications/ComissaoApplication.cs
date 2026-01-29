using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Applications
{
    public class ComissaoApplication : IComissaoApplication
    {
        private readonly IComissaoRepository _comissaoRepository;

        public ComissaoApplication(IComissaoRepository comissaoRepository)
        {
            _comissaoRepository = comissaoRepository;
        }

        public async Task MarcarComoPagaAsync(Guid id)
        {
            var comissao = await _comissaoRepository.GetByIdAsync(id);

            comissao.Pagar();
            await _comissaoRepository.UpdateAsync(comissao);
        }

        public async Task MarcarComoCanceladaAsync(Guid id)
        {
            var comissao = await _comissaoRepository.GetByIdAsync(id);

            comissao.Cancelar();
            await _comissaoRepository.UpdateAsync(comissao);
        }
    }
}
