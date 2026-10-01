// rev 2026-10-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный словарь, поддерживающий механизм строковой сериализации и десериализации,
	/// где ключами являются строки, а значениями — безопасные HTML-строки <see cref="HtmlString"/>.
	/// </summary>
	/// <remarks>
	/// Позволяет хранить предопределенные фрагменты разметки с опциональной встроенной типографикой.
	/// </remarks>
	public class DictHtml
		: _Dict_Proto<string, HtmlString>
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="DictHtml"/>.
		/// </summary>
		public DictHtml()
			: base()
		{
		}


		/// <summary>
		/// Инициализирует экземпляр класса <see cref="DictHtml"/> на основе перечисляемой коллекции строк сериализации.
		/// </summary>
		/// <param name="serialization">Коллекция строк в формате сериализации словаря.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="serialization"/> равен <see langword="null"/>.</exception>
		public DictHtml(
			IEnumerable<string>? serialization)
			: base(serialization)
		{
		}


		/// <summary>
		/// Инициализирует экземпляр класса <see cref="DictHtml"/> на основе массива строк сериализации.
		/// </summary>
		/// <param name="serialization">Массив строк в формате сериализации словаря.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="serialization"/> равен <see langword="null"/>.</exception>
		public DictHtml(
			params string[]? serialization)
			: base(serialization)
		{
		}


		/// <summary>
		/// Инициализирует экземпляр класса <see cref="DictHtml"/> на основе одной плоской строки сериализации.
		/// </summary>
		/// <param name="serialization">Строка в формате сериализации словаря.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="serialization"/> равен <see langword="null"/>.</exception>
		public DictHtml(
			string? serialization)
			: base(serialization)
		{
		}


		/* overrides */


		/// <inheritdoc />
		public override string StringToKey(
			string key)
		{
			return key ?? string.Empty;
		}


		/// <inheritdoc />
		public override HtmlString StringToValue(
			string value)
		{
			if (value == null)
				return HtmlString.Empty;
			return value.ToHtml(UseTypograf);
		}


		/// <inheritdoc />
		public override string KeyToString(
			string key)
		{
			return key ?? string.Empty;
		}


		/// <inheritdoc />
		public override string ValueToString(
			HtmlString value)
		{
			return value?.ToString() ?? string.Empty;
		}


		/* properties */


		/// <summary>
		/// Определяет, требуется ли применять правила типографики при преобразовании входящих строк в объекты <see cref="HtmlString"/>.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если автоматическое форматирование текста включено; в противном случае — <see langword="false"/>. По умолчанию: <see langword="true"/>.
		/// </value>
		public bool UseTypograf { get; set; } = true;

	}

}
