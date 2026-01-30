using Domain.Enums;
using Domain.Exceptions;
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
            ValidarVendedor(vendedor);
            ValidarCliente(cliente);
            ValidarCnpjCpfCliente(cnpjCpfCliente);
            ValidarValorTotal(valorTotal);
            ValidarDataEmissao(dataEmissao);
            ValidarObservacoes(observacoes);

            Id = Guid.NewGuid();
            NumeroInvoice = string.IsNullOrWhiteSpace(numero) ? GerarNumero() : numero;
            DataEmissao = dataEmissao;
            Vendedor = vendedor;
            VendedorId = vendedor.Id;
            Cliente = cliente;
            CnpjCpfCliente = cnpjCpfCliente;
            ValorTotal = valorTotal;
            Observacoes = observacoes;
            Status = StatusInvoice.Pendente;

            CriarComissao();
        }

        private string GerarNumero()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6]}";
        }

        private void CriarComissao()
        {
            Comissao = new Comissao(Id, ValorTotal, Vendedor.PercentualComissao);
        }

        public void Aprovar() => Status = StatusInvoice.Aprovada;

        public void Cancelar() => Status = StatusInvoice.Cancelada;

        public void AlterarVendedor(Vendedor? novoVendedor)
        {
            GarantirPodeAlterar();
            ValidarVendedor(novoVendedor);

            Vendedor = novoVendedor;
            VendedorId = novoVendedor.Id;
            Comissao = new Comissao(Id, ValorTotal, novoVendedor.PercentualComissao);
        }

        public void AlterarValorTotal(decimal valorTotal)
        {
            GarantirPodeAlterar();
            ValidarValorTotal(valorTotal);

            ValorTotal = valorTotal;
            Comissao = new Comissao(Id, ValorTotal, Vendedor.PercentualComissao);
        }

        private void GarantirPodeAlterar()
        {
            if (Status == StatusInvoice.Aprovada)
            {
                throw new DomainException("Invoice aprovada não pode ser alterada.");
            }
        }

        private static void ValidarVendedor(Vendedor vendedor)
        {
            if (vendedor == null)
            {
                throw new DomainException("Vendedor é obrigatório.");
            }

            if (!vendedor.Ativo)
            {
                throw new DomainException("Vendedor inativo não pode receber comissões.");
            }
        }

        private static void ValidarCliente(string cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente))
            {
                throw new DomainException("Cliente é obrigatório.");
            }

            if (cliente.Length > 200)
            {
                throw new DomainException("Cliente deve ter no máximo 200 caracteres.");
            }
        }

        private static void ValidarCnpjCpfCliente(string cnpjCpfCliente)
        {
            if (string.IsNullOrWhiteSpace(cnpjCpfCliente))
            {
                throw new DomainException("CPF/CNPJ do cliente é obrigatório.");
            }

            if (!CpfValidator.CpfIsValid(cnpjCpfCliente) && !CnpjValidator.CnpjIsValid(cnpjCpfCliente))
            {
                throw new DomainException("CPF ou CNPJ do cliente inválido.");
            }
        }

        private static void ValidarValorTotal(decimal valorTotal)
        {
            if (valorTotal <= 0)
            {
                throw new DomainException("Valor total da Invoice deve ser maior que zero.");
            }
        }

        private static void ValidarDataEmissao(DateTime dataEmissao)
        {
            if (dataEmissao == default)
            {
                throw new DomainException("Data de emissão é obrigatória.");
            }
        }

        private static void ValidarObservacoes(string? observacoes)
        {
            if (!string.IsNullOrEmpty(observacoes) && observacoes.Length > 500)
            {
                throw new DomainException("Observações devem ter no máximo 500 caracteres.");
            }
        }
    }
}
