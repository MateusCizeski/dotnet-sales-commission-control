using Domain.Enums;
using Domain.Exceptions;

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

        public void Pagar()
        {
            if (Status == StatusComissao.Cancelada)
            {
                throw new DomainException("Não é possível pagar comissão cancelada.");
            }

            Status = StatusComissao.Paga;
            DataPagamento = DateTime.UtcNow;
        }

        public void Cancelar()
        {
            Status = StatusComissao.Cancelada;
        }

        public void AtualizarValores(decimal valorBase, decimal percentual, decimal valorComissao)
        {
            ValorBase = valorBase;
            PercentualAplicado = percentual;
            ValorComissao = valorComissao;
            DataCalculo = DateTime.UtcNow;
            Status = StatusComissao.Pendente;
        }
    }
}
