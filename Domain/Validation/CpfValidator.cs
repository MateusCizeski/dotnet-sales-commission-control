using System.Text.RegularExpressions;

namespace Domain.Validation
{
    public static class CpfValidator
    {
        public static bool CpfIsValid(string cpf)
        {
            cpf = Regex.Replace(cpf, "[^0-9]", "");

            if (cpf.Length != 11)
            {
                return false;
            }


            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }

            var numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += numbers[i] * (10 - i);
            }

            int result = sum % 11;
            int digit1 = result < 2 ? 0 : 11 - result;

            if (numbers[9] != digit1)
            {
                return false;
            }

            sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += numbers[i] * (11 - i);
            }

            result = sum % 11;
            int digit2 = result < 2 ? 0 : 11 - result;

            return numbers[10] == digit2;
        }
    }
}
