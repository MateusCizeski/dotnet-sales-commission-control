using Domain.Entities;

namespace Domain.Services
{
    public class ComissaoService
    {
        public Comissao Calcular(Invoice invoice, Vendedor vendedor)
        {
            if (!vendedor.PodeReceberCommissao())
            {
                throw new Exception("Vendedor inativo não pode receber comissão.");
            }

            return new Comissao(
                invoice.Id,
                invoice.ValorTotal,
                vendedor.PercentualComissao
            );
        }
    }
}
