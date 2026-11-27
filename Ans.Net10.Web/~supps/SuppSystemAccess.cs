// rev 2026-11-27

using System.Security.Cryptography;
using System.Text;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Предоставляет криптографические методы генерации и проверки персональных ключей системного доступа (PBKDF2-SHA256).
    /// </summary>
    public static class SuppSystemAccess
    {

        private const int SALT_SIZE = 16;
        private const int HASH_SIZE = 32;
        private const int ITERATIONS = 100_000;
        private const string PREFIX = "PBKDF2";


        /// <summary>
        /// Генерирует криптостойкий случайный персональный ключ доступа в формате URL-safe Base64.
        /// </summary>
        /// <param name="byteLength">Длина случайной последовательности в байтах (по умолчанию <c>24</c>).</param>
        /// <returns>Строка сгенерированного ключа доступа.</returns>
        public static string GenerateKey(
            int byteLength = 24)
        {
            var bytes1 = RandomNumberGenerator.GetBytes(byteLength);
            return Convert.ToBase64String(bytes1)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }


        /// <summary>
        /// Вычисляет необратимый криптографический хэш PBKDF2-SHA256 для указанного ключа доступа.
        /// </summary>
        /// <param name="key">Исходный ключ доступа.</param>
        /// <returns>Строка хэша в формате <c>PBKDF2$100000$&lt;salt&gt;$&lt;hash&gt;</c>.</returns>
        /// <exception cref="ArgumentException">Вызывается, если ключ пустой или состоит из пробелов.</exception>
        public static string HashKey(
            string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            var salt1 = RandomNumberGenerator.GetBytes(SALT_SIZE);
            var hash1 = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(key),
                salt1,
                ITERATIONS,
                HashAlgorithmName.SHA256,
                HASH_SIZE);
            return $"{PREFIX}${ITERATIONS}${Convert.ToBase64String(salt1)}${Convert.ToBase64String(hash1)}";
        }


        /// <summary>
        /// Выполняет безопасную проверку соответствия введенного ключа сохраненному хэшу PBKDF2-SHA256.
        /// </summary>
        /// <param name="key">Проверяемый ключ доступа.</param>
        /// <param name="storedHash">Сохраненная строка хэша из конфигурации.</param>
        /// <returns><see langword="true"/>, если ключ соответствует хэшу; иначе — <see langword="false"/>.</returns>
        public static bool VerifyKey(
            string? key,
            string? storedHash)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            var parts1 = storedHash.Split('$');
            if (parts1.Length != 4 || !string.Equals(parts1[0], PREFIX, StringComparison.Ordinal))
                return false;

            if (!int.TryParse(parts1[1], out var iterations1) || iterations1 < 10_000)
                return false;

            try
            {
                var salt1 = Convert.FromBase64String(parts1[2]);
                var expectedHash1 = Convert.FromBase64String(parts1[3]);
                var actualHash1 = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(key),
                    salt1,
                    iterations1,
                    HashAlgorithmName.SHA256,
                    expectedHash1.Length);
                return CryptographicOperations.FixedTimeEquals(actualHash1, expectedHash1);
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }

}
