// rev 2026-09-25

using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Строитель и контейнер инлайновых CSS-стилей (атрибута style) для HTML-тегов.
	/// </summary>
	public class TagStylesBuilder
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="TagStylesBuilder"/>.
		/// </summary>
		public TagStylesBuilder()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagStylesBuilder"/>
		/// и разбирает переданную строку стилей.
		/// </summary>
		/// <param name="cssStyles">Строка CSS-стилей (например, <c>"color:red;padding:10px;"</c>).</param>
		public TagStylesBuilder(
			string? cssStyles)
			: this()
		{
			Append(cssStyles);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает внутренний словарь уникальных CSS-свойств и их значений.
		/// </summary>
		/// <value>
		/// Экземпляр <see cref="Dictionary{TKey, TValue}"/>, где ключом является системное имя CSS-свойства (например, <c>"color"</c>), 
		/// а значением — установленный для него параметр (например, <c>"red"</c>).
		/// </value>
		public Dictionary<string, string> Items { get; } = [];


		/* methods */


		/// <summary>
		/// Добавляет CSS-стили из другого словаря, только если имя CSS-свойства полностью отсутствует в текущей коллекции. 
		/// Существующие в контейнере свойства не перезаписываются.
		/// </summary>
		/// <param name="dict">Словарь CSS-свойств и их параметров, планируемый к импорту.</param>
		public void ApplyOriginal(
			Dictionary<string, string>? dict)
		{
			if (dict == null)
				return;
			foreach (var item1 in dict)
				Items.TryAdd(item1.Key, item1.Value);
		}


		/// <summary>
		/// Разбирает строку CSS-стилей и добавляет только те свойства, которые еще не были зарегистрированы в текущей коллекции.
		/// Существующие в контейнере свойства не перезаписываются.
		/// </summary>
		/// <param name="cssStyles">Строка CSS-стилей, элементы которой разделены точкой с запятой.</param>
		public void ApplyOriginal(
			string? cssStyles)
		{
			if (string.IsNullOrEmpty(cssStyles))
				return;
			ApplyOriginal(_getDict(cssStyles));
		}


		/// <summary>
		/// Перезаписывает или добавляет CSS-стили из другого словаря в текущую коллекцию.
		/// При совпадении имен CSS-свойств старые параметры заменяются новыми.
		/// </summary>
		/// <param name="dict">Словарь CSS-свойств и их параметров для слияния.</param>
		public void Append(
			Dictionary<string, string>? dict)
		{
			if (dict == null)
				return;
			foreach (var item1 in dict)
				Items[item1.Key] = item1.Value;
		}


		/// <summary>
		/// Разбирает и добавляет инлайновые CSS-стили из строки в текущую коллекцию. 
		/// Существующие в контейнере свойства при совпадении имен перезаписываются новыми значениями.
		/// </summary>
		/// <param name="cssStyles">Строка CSS-стилей, элементы которой разделены точкой с запятой.</param>
		public void Append(
			string? cssStyles)
		{
			if (string.IsNullOrEmpty(cssStyles))
				return;
			Append(_getDict(cssStyles));
		}


		/// <summary>
		/// Добавляет инлайновые CSS-стили из строки в текущую коллекцию только в том случае, если переданное логическое условие истинно.
		/// </summary>
		/// <param name="check">Логический флаг (условие), управляющий добавлением инлайновых стилей.</param>
		/// <param name="cssStyles">Строка CSS-стилей, элементы которой разделены точкой с запятой.</param>
		public void AppendIf(
			bool check,
			string? cssStyles)
		{
			if (check)
				Append(cssStyles);
		}


		/* functions */


		/// <summary>
		/// Формирует итоговую валидную строку инлайнового стиля для последующей вставки в HTML-тег.
		/// </summary>
		/// <returns>
		/// Готовая строка инлайновых стилей вида <c>"prop1:val1;prop2:val2;"</c>, 
		/// либо пустая строка <see cref="string.Empty"/>, если в коллекции строителя отсутствуют элементы.
		/// </returns>
		public override string ToString()
		{
			if (Items.Count == 0)
				return string.Empty;
			var sb1 = new StringBuilder();
			foreach (var item1 in Items)
			{
				sb1.Append(item1.Key);
				sb1.Append(':');
				sb1.Append(item1.Value);
				sb1.Append(';');
			}
			return sb1.ToString();
		}


		/* privates */


		private static string[] _getItems(
			string cssStyles)
		{
			return cssStyles.Split(
				';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		}


		private static (string? key, string? value) _getParts(
			string cssStyle)
		{
			var parts1 = cssStyle.Split(':');
			return parts1.Length == 2
				? (parts1[0].Trim(), parts1[1].Trim())
				: (null, null);
		}


		private static Dictionary<string, string> _getDict(
			string cssStyles)
		{
			var dict1 = new Dictionary<string, string>();
			foreach (var item1 in _getItems(cssStyles))
			{
				var (key1, value1) = _getParts(item1);
				if (key1 != null && value1 != null)
					dict1[key1] = value1;
			}
			return dict1;
		}

	}

}
