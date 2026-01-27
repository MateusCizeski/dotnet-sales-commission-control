namespace Domain.Validation
{
    public class VendedorValidation
    {
        public static void ValidarVendedor(string nomeCompleto, string cpf, string email, decimal percentualComissao)
        {
            ValidarNomeCompleto(nomeCompleto);
            ValidarCpf(cpf);
            ValidarEmail(email);
            ValidarPercentualComissao(percentualComissao);
        }

        private static void ValidarNomeCompleto(string nomeCompleto)
        {
            if(string.IsNullOrWhiteSpace(nomeCompleto))
            {
                throw new ArgumentNullException("Nome completo é obrigatório.");
            }

            if(nomeCompleto.Length > 200)
            {
                throw new ArgumentException("Nome completo deve ter no máximo 200 caracteres.");
            }
        }

        private static void ValidarCpf(string cpf)
        {
            if(string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("CPF é obrigatório.");
            }

            if(!CpfValidator.CpfIsValid(cpf))
            {
                throw new ArgumentException("CPF inválido.");
            }
        }

        private static void ValidarEmail(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email é obrigatório.");
            }

            if(!EmailValidator.EmailIsValid(email))
            {
                throw new ArgumentException("Email inválido.");
            }
        }

        private static void ValidarPercentualComissao(decimal percentualComissao)
        {
            if(percentualComissao < 0 || percentualComissao > 15)
            {
                throw new ArgumentException("Percentual de comissão deve estar entre 0% e 15%.");
            }
        }
    }
}
