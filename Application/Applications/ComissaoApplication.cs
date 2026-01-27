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
    }
}
