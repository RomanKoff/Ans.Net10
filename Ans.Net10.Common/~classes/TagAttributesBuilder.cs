// rev 2026-09-25

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Строитель и контейнер свободных HTML-атрибутов (например, id, data-*, href, disabled) для HTML-тегов.
	/// </summary>
	public class TagAttributesBuilder
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="TagAttributesBuilder"/>.
		/// </summary>
		public TagAttributesBuilder()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagAttributesBuilder"/>
		/// и разбирает переданную строку атрибутов.
		/// </summary>
		/// <param name="serialization">
		/// Строка сериализованных атрибутов (например, <c>id="main" data-id="12" disabled</c>).
		/// </param>
		public TagAttributesBuilder(
			string? serialization)
			: this()
		{
			Append(serialization);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает внутренний словарь уникальных атрибутов и их значений.
		/// </summary>
		/// <value>
		/// Экземпляр <see cref="Dictionary{TKey, TValue}"/>, где ключом является имя HTML-атрибута, 
		/// а значением — его содержимое (или <see langword="null"/> для флаговых атрибутов типа <c>disabled</c>).
		/// </value>
		public Dictionary<string, string?> Items { get; } = [];


		/* methods */


		/// <summary>
		/// Разбирает и добавляет HTML-атрибуты из строки сериализации. 
		/// Существующие в контейнере атрибуты при совпадении имен перезаписываются новыми значениями.
		/// </summary>
		/// <param name="serialization">
		/// Строка с перечислением HTML-атрибутов, разделенных пробелами.
		/// </param>
		public void Append(
			string? serialization)
		{
			if (string.IsNullOrEmpty(serialization))
				return;
			foreach (var item1 in _getItems(serialization))
			{
				var (key1, value1) = _getParts(item1);
				Items[key1] = value1;
			}
		}


		/// <summary>
		/// Добавляет HTML-атрибуты из строки сериализации только в том случае, если переданное логическое условие истинно.
		/// </summary>
		/// <param name="check">Логический флаг (условие), управляющий добавлением атрибутов.</param>
		/// <param name="serialization">Строка с перечислением HTML-атрибутов, разделенных пробелами.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendIf(
			bool check,
			string? serialization)
		{
			if (check)
				Append(serialization);
		}


		/* functions */


		/// <summary>
		/// Формирует готовую строку HTML-атрибутов, предназначенную для рендеринга непосредственно внутри открывающего HTML-тега.
		/// </summary>
		/// <returns>
		/// Строка атрибутов вида <c> id="val" disabled</c> (всегда начинается со знака пробела), 
		/// либо пустая строка <see cref="string.Empty"/>, если в коллекции отсутствуют элементы.
		/// </returns>
		public override string ToString()
		{
			if (Items.Count == 0)
				return string.Empty;
			var sb1 = new StringBuilder();
			foreach (var item1 in Items)
			{
				sb1.Append(' ');
				sb1.Append(item1.Key);
				if (item1.Value != null)
				{
					sb1.Append("=\"");
					sb1.Append(item1.Value);
					sb1.Append('"');
				}
			}
			return sb1.ToString();
		}


		/* privates */


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string[] _getItems(
			string serialization)
		{
			return serialization.Split(
				' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		}


		private static (string key, string? value) _getParts(
			string serialization)
		{
			var i1 = serialization.LastIndexOf('=');
			if (i1 == -1)
				return (serialization, null);
			var key1 = serialization[..i1].Trim();
			var value1 = serialization[(i1 + 1)..].Trim().Trim('"');
			return (key1, value1);
		}

	}

}
