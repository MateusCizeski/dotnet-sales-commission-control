using System.Text.RegularExpressions;

namespace Domain.Validation
{
    public static class CnpjValidator
    {
        public static bool CnpjIsValid(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = Regex.Replace(cnpj, "[^0-9]", "");

            if (cnpj.Length != 14)
                return false;

            if (cnpj.Distinct().Count() == 1)
                return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var numeros = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

            int soma = 0;
            for (int i = 0; i < 12; i++)
                soma += numeros[i] * multiplicador1[i];

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (numeros[12] != digito1)
                return false;

            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += numeros[i] * multiplicador2[i];

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return numeros[13] == digito2;
        }
    }
}
