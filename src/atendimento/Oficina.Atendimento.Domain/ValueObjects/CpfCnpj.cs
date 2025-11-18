using System.Text.RegularExpressions;

namespace Oficina.Atendimento.Domain.ValueObjects
{
    public class CpfCnpj
    {
        public string Numero { get; }

        public CpfCnpj(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("CPF/CNPJ inválido.");

            var cleaned = Regex.Replace(numero, @"\D", "", RegexOptions.None, TimeSpan.FromSeconds(1));

            if (cleaned.Length == 11)
            {
                if (!CpfValido(cleaned))
                    throw new ArgumentException("CPF inválido.");
            }
            else if (cleaned.Length == 14)
            {
                if (!CnpjValido(cleaned))
                    throw new ArgumentException("CNPJ inválido.");
            }
            else
            {
                throw new ArgumentException("Formato de CPF/CNPJ inválido.");
            }

            Numero = cleaned;
        }

        private static bool CpfValido(string cpf)
        {
            // Rejeita CPFs com todos os dígitos iguais (ex: 00000000000)
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Pesos para os cálculos
            Span<int> mult1 = stackalloc int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            Span<int> mult2 = stackalloc int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Calcula o primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (cpf[i] - '0') * mult1[i];

            int resto = sum % 11;
            int dig1 = resto < 2 ? 0 : 11 - resto;

            // Calcula o segundo dígito verificador usando os 9 dígitos + o dig1 calculado
            sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (cpf[i] - '0') * mult2[i];

            sum += dig1 * mult2[9];

            resto = sum % 11;
            int dig2 = resto < 2 ? 0 : 11 - resto;

            // Verifica se os dois dígitos batem com os informados
            return cpf[9] - '0' == dig1 && cpf[10] - '0' == dig2;
        }


        private static bool CnpjValido(string cnpj)
        {
            // Rejeita CNPJs com todos os dígitos iguais
            if (cnpj.All(c => c == cnpj[0]))
                return false;

            // Pesos pré-definidos com stackalloc para evitar heap allocation
            Span<int> mult1 = stackalloc int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            Span<int> mult2 = stackalloc int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Calcula primeiro dígito verificador
            int sum = 0;
            for (int i = 0; i < 12; i++)
                sum += (cnpj[i] - '0') * mult1[i];

            int resto = sum % 11;
            int dig1 = resto < 2 ? 0 : 11 - resto;

            // Calcula segundo dígito verificador
            sum = 0;
            for (int i = 0; i < 13; i++)
                sum += (cnpj[i] - '0') * mult2[i];

            resto = sum % 11;
            int dig2 = resto < 2 ? 0 : 11 - resto;

            return cnpj[12] - '0' == dig1 && cnpj[13] - '0' == dig2;
        }

        public override string ToString() => Numero;
    }
}
