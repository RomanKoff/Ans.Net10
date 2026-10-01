// rev 2026-09-29

namespace Ans.Net10.Web
{

	/// <summary>
	/// Служба для работы с файлами cookie в контексте текущего HTTP-запроса.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="CookiesService"/> с использованием первичного конструктора C#.
	/// </remarks>
	/// <param name="current">Текущий контекст обработки запроса, предоставляющий доступ к <see cref="Microsoft.AspNetCore.Http.HttpContext"/>.</param>
	public class CookiesService(
		CurrentContext current)
	{

		private readonly CurrentContext _current = current;


		/* functions */


		/// <summary>
		/// Проверяет наличие файла cookie с указанным ключом в текущем HTTP-запросе.
		/// </summary>
		/// <param name="key">Уникальное имя (ключ) файла cookie.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если файл cookie с таким именем существует; 
		/// в противном случае, или если HTTP-контекст недоступен — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="key"/> равен <see langword="null"/>.
		/// </exception>
		public bool Has(
			string key)
		{
			ArgumentNullException.ThrowIfNull(key);
			return _current.HttpContext?.Request.Cookies.ContainsKey(key) ?? false;
		}


		/// <summary>
		/// Извлекает строковое значение файла cookie по его ключу.
		/// </summary>
		/// <param name="key">Уникальное имя (ключ) файла cookie.</param>
		/// <returns>
		/// Строковое значение файла cookie, если оно найдено; в противном случае, 
		/// а также при отсутствии HTTP-контекста — <see langword="null"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="key"/> равен <see langword="null"/>.
		/// </exception>
		public string? Get(
			string key)
		{
			ArgumentNullException.ThrowIfNull(key);
			if (_current.HttpContext == null)
				return null;
			return _current.HttpContext.Request.Cookies.TryGetValue(key, out var value1)
				? value1 : null;
		}


		/* methods */


		/// <summary>
		/// Добавляет новый файл cookie или перезаписывает существующий с указанным ключом и значением.
		/// </summary>
		/// <remarks>
		/// Если текущий HTTP-контекст недоступен, операция не выполняет никаких действий.
		/// </remarks>
		/// <param name="key">Уникальное имя (ключ) создаваемого файла cookie.</param>
		/// <param name="value">Строковое содержимое, сохраняемое в файл cookie.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="key"/> или <paramref name="value"/> равен <see langword="null"/>.
		/// </exception>
		public void Append(
			string key,
			string value)
		{
			ArgumentNullException.ThrowIfNull(key);
			ArgumentNullException.ThrowIfNull(value);
			_current.HttpContext?.Response.Cookies.Append(key, value);
		}


		/// <summary>
		/// Удаляет файл cookie с указанным ключом из клиента (браузера).
		/// </summary>
		/// <remarks>
		/// Метод отправляет клиенту специальный заголовок удаления куки. 
		/// Если текущий HTTP-контекст недоступен, операция не выполняет никаких действий.
		/// </remarks>
		/// <param name="key">Уникальное имя (ключ) удаляемого файла cookie.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="key"/> равен <see langword="null"/>.
		/// </exception>
		public void Delete(
			string key)
		{
			ArgumentNullException.ThrowIfNull(key);
			_current.HttpContext?.Response.Cookies.Delete(key);
		}

	}

}
