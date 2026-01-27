using Domain.Entities;

namespace Domain.Validation
{
    public static class InvoiceValidation
    {
        public static void ValidarInvoice(Vendedor vendedor, string cliente, string cnpjCpfCliente, decimal valorTotal, DateTime dataEmissao, string? observacoes)
        {
            ValidarVendedor(vendedor);
            ValidarCliente(cliente);
            ValidarCnpjCpfCliente(cnpjCpfCliente);
            ValidarValorTotal(valorTotal);
            ValidarDataEmissao(dataEmissao);
            ValidarObservacoes(observacoes);
        }

        private static void ValidarVendedor(Vendedor vendedor)
        {
            if(vendedor == null)
            {
                throw new ArgumentException("Vendedor é obrigatório.");
            }

            if(!vendedor.Ativo)
            {
                throw new ArgumentException("Vendedor inativo não pode receber comissões.");
            }
        }

        private static void ValidarCliente(string cliente)
        {
            if(string.IsNullOrWhiteSpace(cliente))
            {
                throw new ArgumentException("Cliente é obrigatório.");
            }

            if (cliente.Length > 200)
            {
                throw new ArgumentException("Cliente deve ter no máximo 200 caracteres.");
            }
        }

        private static void ValidarCnpjCpfCliente(string cnpjCpfCliente)
        {
            if (string.IsNullOrWhiteSpace(cnpjCpfCliente))
            {
                throw new ArgumentException("CPF/CNPJ do cliente é obrigatório.");
            }

            if (!CpfValidator.CpfIsValid(cnpjCpfCliente) && !CnpjValidator.CnpjIsValid(cnpjCpfCliente))
            {
                throw new ArgumentException("CPF ou CNPJ do cliente inválido.");
            }
        }

        private static void ValidarValorTotal(decimal valorTotal)
        {
            if (valorTotal <= 0)
            {
                throw new ArgumentException("Valor total da Invoice deve ser maior que zero.");
            }
        }

        private static void ValidarDataEmissao(DateTime dataEmissao)
        {
            if (dataEmissao == default)
            {
                throw new ArgumentException("Data de emissão é obrigatória.");
            }
        }

        private static void ValidarObservacoes(string? observacoes)
        {
            if (!string.IsNullOrEmpty(observacoes) && observacoes.Length > 500)
            {
                throw new ArgumentException("Observações devem ter no máximo 500 caracteres.");
            }
        }
    }
}
