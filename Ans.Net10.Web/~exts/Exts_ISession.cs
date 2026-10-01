// rev 2026-09-30

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="ISession"/>, обеспечивающие строго типизированное 
	/// сохранение и извлечение произвольных объектов посредством JSON-сериализации.
	/// </summary>
	public static partial class _e_ISession
	{

		/* methods */


		/// <summary>
		/// Сериализует переданный объект в строку формата JSON и сохраняет его в сессии по указанному ключу.
		/// </summary>
		/// <typeparam name="T">Тип сохраняемого объекта.</typeparam>
		/// <param name="session">Текущий контекст сессии <see cref="ISession"/>.</param>
		/// <param name="key">Уникальный строковый ключ для записи в сессию.</param>
		/// <param name="value">Объект для сериализации и сохранения.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="session"/> или <paramref name="key"/> равен <see langword="null"/>.</exception>
		public static void Set<T>(
			this ISession session,
			string key,
			T value)
		{
			ArgumentNullException.ThrowIfNull(session);
			ArgumentException.ThrowIfNullOrEmpty(key);
			session.SetString(key, JsonSerializer.Serialize(value));
		}


		/* functions */


		/// <summary>
		/// Извлекает из сессии строковое значение по указанному ключу и десериализует его в объект заданного типа.
		/// </summary>
		/// <typeparam name="T">Тип, в который преобразуются данные из JSON.</typeparam>
		/// <param name="session">Текущий контекст сессии <see cref="ISession"/>.</param>
		/// <param name="key">Уникальный строковый ключ для поиска в сессии.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/>, либо <see langword="null"/>, если ключ отсутствует, сессия пуста или значение содержит некорректный формат.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="session"/> или <paramref name="key"/> равен <see langword="null"/>.</exception>
		public static T? Get<T>(
			this ISession session,
			string key)
		{
			ArgumentNullException.ThrowIfNull(session);
			ArgumentException.ThrowIfNullOrEmpty(key);
			var value1 = session.GetString(key);
			return string.IsNullOrWhiteSpace(value1)
				? default
				: JsonSerializer.Deserialize<T>(value1);
		}

	}

}
