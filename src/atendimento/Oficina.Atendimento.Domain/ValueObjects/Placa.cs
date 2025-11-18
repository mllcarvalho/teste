namespace Oficina.Atendimento.Domain.ValueObjects
{
    public class Placa
    {
        public string Numero { get; }

        public Placa(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero) || !PlacaValida(numero))
                throw new ArgumentException("Placa inválida.");

            Numero = numero;
        }

        private static bool PlacaValida(string numero)
        {
            // Formato antigo: LLL-NNNN (ex: ABC-1234)
            var formatoAntigo = @"^[A-Z]{3}-\d{4}$";
            // Formato Mercosul: LLLNLNN (ex: ABC1D23)
            var formatoMercosul = @"^[A-Z]{3}\d{1}[A-Z]{1}\d{2}$";
            return System.Text.RegularExpressions.Regex.IsMatch(numero, formatoAntigo)
                || System.Text.RegularExpressions.Regex.IsMatch(numero, formatoMercosul);
        }
    }
}
