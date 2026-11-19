// rev 2026-11-19

namespace Ans.Net10.Common
{

    /// <summary>
    /// Методы расширения для стандартного ответа <see cref="HttpResponseMessage"/>.
    /// </summary>
    public static partial class Exts_HttpResponseMessage
    {

        /// <summary>
        /// Извлекает HTTP-заголовки ответа и преобразует их в регистронезависимый словарь.
        /// </summary>
        /// <param name="message">Экземпляр HTTP-ответа <see cref="HttpResponseMessage"/>.</param>
        /// <returns>Словарь <see cref="Dictionary{TKey, TValue}"/>, где ключом является имя заголовка, а значением — массив его строковых значений.</returns>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="message"/> равен <see langword="null"/>.</exception>
        public static Dictionary<string, string[]> GetHeadersDictionary(
            this HttpResponseMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return message.Headers.ToDictionary(
                x => x.Key,
                x => x.Value.ToArray(),
                StringComparer.OrdinalIgnoreCase);
        }

    }

}
