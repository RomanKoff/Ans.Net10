// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс реализующий криптографические методы для работы с паролями, токенами, хэшами (SHA-256) и HMAC.
	/// </summary>
	public static class SuppCrypto
	{

		/* consts */


		/// <summary>
		/// Размер генерируемой криптографической соли в байтах (128 бит).
		/// </summary>
		public const int SALT_SIZE = 16; // 128 бит соль

		/// <summary>
		/// Размер генерируемого криптографического хэш-ключа в байтах (256 бит под результирующий хэш).
		/// </summary>
		public const int KEY_SIZE = 32;  // 256 бит под хэш

		/// <summary>
		/// Количество итераций алгоритма PBKDF2, соответствующее стандартам и рекомендациям OWASP и NIST для SHA-256.
		/// </summary>
		public const int ITERATIONS = 600_000; // Рекомендация OWASP / NIST для PBKDF2-SHA256


		/// <summary>
		/// Набор безопасных строчных символов латинского алфавита, исключающий неоднозначные знаки (например, 'l', 'o').
		/// </summary>
		/// <value>Массив символов нижнего регистра.</value>
		public static readonly char[] LOWERCASE_CHARS = "abcdefghjkmnpqrstuvwxyz".ToCharArray();

		/// <summary>
		/// Набор безопасных заглавных символов латинского алфавита, исключающий неоднозначные знаки (например, 'I', 'O').
		/// </summary>
		/// <value>Массив символов верхнего регистра.</value>
		public static readonly char[] UPPERCASE_CHARS = "ABCDEFGHJKMNPQRSTUVWXYZ".ToCharArray();

		/// <summary>
		/// Набор безопасных числовых знаков, исключающий неоднозначные символы '0' и '1'.
		/// </summary>
		/// <value>Массив символов цифр.</value>
		public static readonly char[] DIGITS_CHARS = "23456789".ToCharArray();

		/// <summary>
		/// Набор стандартных специальных символов и знаков пунктуации, рекомендованных для формирования сложных паролей.
		/// </summary>
		/// <value>Массив специальных знаков.</value>
		public static readonly char[] SPECIAL_CHARS = "!@#$%^&*()_+-=[]{}?<>-".ToCharArray();

		/// <summary>
		/// Объединенный алфавитный реестр, содержащий все разрешенные символы для криптографических операций генерации.
		/// </summary>
		/// <value>Полный массив доступных парольных символов.</value>
		public static readonly char[] ALL_CHARS = [
			.. LOWERCASE_CHARS,
			.. UPPERCASE_CHARS,
			.. DIGITS_CHARS,
			.. SPECIAL_CHARS];


		/* functions */


		/// <summary>
		/// Генерирует криптографически стойкий случайный пароль заданной длины с гарантированным вхождением символов из всех четырех групп.
		/// </summary>
		/// <param name="length">Требуемая длина генерируемого пароля (минимальное безопасное значение — 8). По умолчанию равна 16.</param>
		/// <returns>Строка безопасного случайного пароля.</returns>
		/// <exception cref="ArgumentException">Выбрасывается, если переданное значение длины <paramref name="length"/> строго меньше 8 символов.</exception>
		public static string GenerateSecurePassword(
			int length = 16)
		{
			if (length < 8)
				throw new ArgumentException(
					"[Ans.Net10.Common/SuppCrypto.GenerateSecurePassword] To ensure security, the password must be at least 8 characters long.",
					nameof(length));
			var a1 = new char[length];
			a1[0] = LOWERCASE_CHARS[RandomNumberGenerator.GetInt32(LOWERCASE_CHARS.Length)];
			a1[1] = UPPERCASE_CHARS[RandomNumberGenerator.GetInt32(UPPERCASE_CHARS.Length)];
			a1[2] = DIGITS_CHARS[RandomNumberGenerator.GetInt32(DIGITS_CHARS.Length)];
			a1[3] = SPECIAL_CHARS[RandomNumberGenerator.GetInt32(SPECIAL_CHARS.Length)];
			for (int i1 = 4; i1 < length; i1++)
				a1[i1] = ALL_CHARS[RandomNumberGenerator.GetInt32(ALL_CHARS.Length)];
			for (int i1 = length - 1; i1 > 0; i1--)
			{
				int j1 = RandomNumberGenerator.GetInt32(i1 + 1);
				(a1[i1], a1[j1]) = (a1[j1], a1[i1]);
			}
			return new string(a1);
		}


		/// <summary>
		/// Генерирует криптографически стойкий, URL-безопасный токен в формате Base64Url без дополнительных аллокаций памяти в куче.
		/// </summary>
		/// <param name="bytesCount">Количество байт энтропии, определяющее криптостойкость токена (например, 32 байта обеспечивают 256 бит безопасности). По умолчанию равно 32.</param>
		/// <returns>Строка сформированного токена в формате Base64Url, либо <see cref="string.Empty"/>, если значение <paramref name="bytesCount"/> меньше или равно 0.</returns>
		public static string GenerateApiToken(
			int bytesCount = 32)
		{
			if (bytesCount <= 0)
				return string.Empty;
			Span<byte> buffer1 = bytesCount <= 256
				? stackalloc byte[bytesCount]
				: new byte[bytesCount];
			RandomNumberGenerator.Fill(buffer1);
			return Convert.ToBase64String(buffer1)
				.Replace("+", "-")
				.Replace("/", "_")
				.TrimEnd('=');
		}


		/// <summary>
		/// Создает безопасный криптографический хэш открытого пароля на основе алгоритма PBKDF2-SHA256 с автоматическим добавлением соли.
		/// </summary>
		/// <param name="password">Исходная строка открытого пароля для хэширования.</param>
		/// <returns>Строка, содержащая сгенерированную соль и итоговый хэш в кодировке Base64, разделённые точкой и готовые к записи в БД.</returns>
		/// <exception cref="ArgumentException">Выбрасывается, если параметр <paramref name="password"/> пуст или равен <see langword="null"/>.</exception>
		public static string HashPassword(
			string password)
		{
			ArgumentException.ThrowIfNullOrEmpty(
				password, nameof(password));
			var salt1 = RandomNumberGenerator.GetBytes(SALT_SIZE);
			var hash1 = Rfc2898DeriveBytes.Pbkdf2(
				password, salt1, ITERATIONS, HashAlgorithmName.SHA256, KEY_SIZE);
			return $"{Convert.ToBase64String(salt1)}.{Convert.ToBase64String(hash1)}";
		}


		/// <summary>
		/// Проверяет соответствие переданного открытого пароля ранее сохраненному комбинированному хэшу. Защищено от атак по времени (Timing Attacks).
		/// </summary>
		/// <param name="password">Проверяемый открытый текстовый пароль.</param>
		/// <param name="hashedPassword">Ранее сформированная строка хэша в формате "Соль.Хэш".</param>
		/// <returns><see langword="true"/>, если вычисленный хэш пароля совпал с ожидаемым; в противном случае — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentException">Выбрасывается, если любой из параметров пуст или равен <see langword="null"/>.</exception>
		public static bool VerifyPassword(
			string password,
			string hashedPassword)
		{
			ArgumentException.ThrowIfNullOrEmpty(
				password, nameof(password));
			ArgumentException.ThrowIfNullOrEmpty(
				hashedPassword, nameof(hashedPassword));
			var i1 = hashedPassword.IndexOf('.');
			if (i1 == -1 || i1 == 0 || i1 == hashedPassword.Length - 1)
				return false;
			try
			{
				var span1 = hashedPassword.AsSpan();
				var salt1 = Convert.FromBase64String(hashedPassword[..i1]);
				var expectedHash1 = Convert.FromBase64String(hashedPassword[(i1 + 1)..]);
				var actualHash1 = Rfc2898DeriveBytes.Pbkdf2(
					password, salt1, ITERATIONS, HashAlgorithmName.SHA256, KEY_SIZE);
				return CryptographicOperations.FixedTimeEquals(actualHash1, expectedHash1);
			}
			catch (FormatException)
			{
				return false;
			}
		}


		/// <summary>
		/// Вычисляет хэш-код алгоритма SHA-256 для строки и возвращает результат в виде шестнадцатеричной строки в нижнем регистре.
		/// </summary>
		/// <param name="data">Исходная строка данных для хэширования. Допускает значение <see langword="null"/>.</param>
		/// <returns>Строка SHA-256 хэша в нижнем регистре или <see cref="string.Empty"/>, если аргумент <paramref name="data"/> равен <see langword="null"/>.</returns>
		public static string ComputeSha256(
			string? data)
		{
			if (data == null)
				return string.Empty;
			return Convert.ToHexStringLower(
				SHA256.HashData(Encoding.UTF8.GetBytes(data)));
		}


		/// <summary>
		/// Вычисляет хэш-код алгоритма SHA-256 для байтового среза. Выполняется без аллокаций памяти в куче (Zero-allocation).
		/// </summary>
		/// <param name="source">Исходный байтовый срез памяти <see cref="ReadOnlySpan{Byte}"/> для хэширования.</param>
		/// <returns>Массив байт вычисленного хэш-кода.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ComputeSha256(
			ReadOnlySpan<byte> source)
		{
			return SHA256.HashData(source);
		}


		/// <summary>
		/// Потокобезопасно и в буферизованном режиме вычисляет криптографический хэш SHA-256 для переданного потока данных.
		/// </summary>
		/// <param name="stream">Исходный поток данных <see cref="Stream"/>. Допускает значение <see langword="null"/>.</param>
		/// <returns>Массив байт хэш-кода, либо пустой массив, если объект потока равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ComputeSha256(
			Stream? stream)
		{
			if (stream == null)
				return [];
			return SHA256.HashData(stream);
		}


		/// <summary>
		/// Вычисляет код аутентификации сообщения HMAC-SHA256 для строки данных с использованием секретного ключа.
		/// </summary>
		/// <param name="data">Исходная строка данных. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Массив байт секретного криптографического ключа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Строка кода аутентификации в нижнем регистре или <see cref="string.Empty"/>, если данные или ключ не заданы.</returns>
		public static string ComputeHmacSha256(
			string? data,
			byte[]? key)
		{
			if (data == null || key == null)
				return string.Empty;
			var data1 = Encoding.UTF8.GetBytes(data);
			var hash1 = HMACSHA256.HashData(key, data1);
			return Convert.ToHexStringLower(hash1);
		}


		/// <summary>
		/// Вычисляет код аутентификации сообщения HMAC-SHA256 для байтового среза без создания объектов алгоритма в управляемой куче.
		/// </summary>
		/// <param name="source">Исходный байтовый срез данных <see cref="ReadOnlySpan{Byte}"/>.</param>
		/// <param name="key">Байтовый срез секретного криптографического ключа <see cref="ReadOnlySpan{Byte}"/>.</param>
		/// <returns>Массив байт итогового кода аутентификации.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ComputeHmacSha256(
			ReadOnlySpan<byte> source,
			ReadOnlySpan<byte> key)
		{
			return HMACSHA256.HashData(key, source);
		}


		/// <summary>
		/// Вычисляет код аутентификации сообщения HMAC-SHA256 для потока данных с использованием секретного ключа.
		/// </summary>
		/// <param name="stream">Исходный поток данных <see cref="Stream"/>. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Массив байт секретного криптографического ключа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Массив байт кода аутентификации, либо пустой массив при равенстве параметров значению <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ComputeHmacSha256(
			Stream? stream,
			byte[]? key)
		{
			if (stream == null || key == null)
				return [];
			return HMACSHA256.HashData(key, stream);
		}


		/// <summary>
		/// Преобразует строку в безопасный массив байт в кодировке UTF-8.
		/// </summary>
		/// <param name="source">Исходная строка для кодирования. Допускает значение <see langword="null"/>.</param>
		/// <returns>Массив байт UTF-8 или пустой массив, если строка равна <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ToSecureBytes(
			string? source)
		{
			if (source == null)
				return [];
			return Encoding.UTF8.GetBytes(source);
		}


		/// <summary>
		/// Восстанавливает исходную строку из массива байт в кодировке UTF-8.
		/// </summary>
		/// <param name="source">Исходный кодированный массив байт. Допускает значение <see langword="null"/>.</param>
		/// <returns>Декодированная строка, либо <see cref="string.Empty"/>, если массив равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToSecureString(
			byte[]? source)
		{
			if (source == null)
				return string.Empty;
			return Encoding.UTF8.GetString(source);
		}

	}

}
