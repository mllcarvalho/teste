namespace Oficina.Common.Application.Helper
{
    public static class EnumConverter
    {
        public static T ConvertToEnum<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse<T>(value, true, out var result))
                return result;

            throw new ArgumentException($"Valor inválido para enum {typeof(T).Name}: {value}");
        }
    }
}
