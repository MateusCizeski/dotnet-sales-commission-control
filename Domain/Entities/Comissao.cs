using Domain.Enums;

namespace Domain.Entities
{
    public class Comissao
    {
        public Guid Id { get; private set; }
        public Guid InvoiceId { get; private set; }
        public decimal ValorBase { get; private set; }
        public decimal PercentualAplicado { get; private set; }
        public decimal ValorComissao { get; private set; }
        public StatusComissao Status { get; private set; }
        public DateTime DataCalculo { get; private set; }
        public DateTime? DataPagamento { get; private set; }

        public Invoice Invoice { get; private set; }

        protected Comissao() { }

        public Comissao(Guid invoiceId, decimal valorBase, decimal percentual)
        {
            Id = Guid.NewGuid();
            InvoiceId = invoiceId;
            ValorBase = valorBase;
            PercentualAplicado = percentual;
            ValorComissao = Math.Round(valorBase * (percentual / 100), 2);
            Status = StatusComissao.Pendente;
            DataCalculo = DateTime.UtcNow;
        }

        public void MarcarComoPaga()
        {
            Status = StatusComissao.Paga;
            DataPagamento = DateTime.UtcNow;
        }

        public void Cancelar()
        {
            Status = StatusComissao.Cancelada;
        }
    }
}
