using Domain.Enums;
using Domain.Validation;
using System.Drawing;

namespace Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; private set; }
        public string NumeroInvoice { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public Guid VendedorId { get; private set; }
        public string Cliente { get; private set; }
        public string CnpjCpfCliente { get; private set; }
        public decimal ValorTotal { get; private set; }
        public StatusInvoice Status { get; private set; }
        public string? Observacoes { get; private set; }

        public Vendedor Vendedor { get; private set; }
        public Comissao Comissao { get; private set; }

        protected Invoice() { }

        public Invoice(Vendedor vendedor, string numero, DateTime dataEmissao, string cliente, string cnpjCpfCliente, decimal valorTotal, string? observacoes = null)
        {
            InvoiceValidation.ValidarInvoice(vendedor, cliente, cnpjCpfCliente, valorTotal, dataEmissao, observacoes);

            Id = Guid.NewGuid();
            Vendedor = vendedor;
            NumeroInvoice = numero;
            DataEmissao = dataEmissao;
            VendedorId = vendedor.Id;
            Cliente = cliente;
            CnpjCpfCliente = cnpjCpfCliente;
            ValorTotal = valorTotal;
            Observacoes = observacoes;
            Status = StatusInvoice.Pendente;

            CriarComissao();
        }

        private void CriarComissao()
        {
            Comissao = new Comissao(Id, ValorTotal, Vendedor.PercentualComissao);
        }

        public void Aprovar()
        {
            Status = StatusInvoice.Aprovada;
        }

        public void Cancelar()
        {
            Status = StatusInvoice.Cancelada;
        }

        public void AlterarVendedor(Guid vendedorId)
        {
            VendedorId = vendedorId;
        }

        public void AlterarValorTotal(decimal valorToal)
        {
            if (valorToal <= 0)
            {
                throw new Exception("Valor da invoice deve ser maior que zero");
            }

            ValorTotal = valorToal;
        }
    }
}
