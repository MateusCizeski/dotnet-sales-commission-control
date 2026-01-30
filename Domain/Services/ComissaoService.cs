using Domain.Entities;

namespace Domain.Services
{
    public class ComissaoService
    {
        public Comissao Calcular(Invoice invoice, Vendedor vendedor)
        {
            vendedor.GarantirQueEstaAtivo();

            return new Comissao(
                invoice.Id,
                invoice.ValorTotal,
                vendedor.PercentualComissao
            );
        }
    }
}
