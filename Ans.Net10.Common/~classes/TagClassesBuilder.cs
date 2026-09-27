// rev 2026-09-25

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Строитель и контейнер для группировки, слияния и генерации CSS-классов HTML-тегов.
	/// </summary>
	public class TagClassesBuilder
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="TagClassesBuilder"/>.
		/// </summary>
		public TagClassesBuilder()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagClassesBuilder"/>
		/// и наполняет его из строки классов.
		/// </summary>
		/// <param name="cssClasses">Строка CSS-классов, разделенная пробелами.</param>
		public TagClassesBuilder(
			string? cssClasses)
			: this()
		{
			Append(cssClasses);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает внутренний словарь сгруппированных префиксов классов и их суффиксов.
		/// </summary>
		/// <value>
		/// Экземпляр <see cref="Dictionary{TKey, TValue}"/>, где ключом является базовый префикс класса 
		/// (включая дефис, например <c>"btn-"</c>), а значением — массив суффиксов (например <c>["primary", "lg"]</c>).
		/// </value>
		public Dictionary<string, string[]> Items { get; } = [];


		/* methods */


		/// <summary>
		/// Добавляет элементы из другого словаря классов, только если
		/// префикс класса полностью отсутствует в текущей коллекции. Существующие префиксы не изменяются.
		/// </summary>
		/// <param name="dict">Словарь префиксов и суффиксов CSS-классов, планируемый к добавлению.</param>
		public void ApplyOriginal(
			Dictionary<string, string[]>? dict)
		{
			if (dict == null)
				return;
			foreach (var item1 in dict)
				Items.TryAdd(item1.Key, item1.Value);
		}


		/// <summary>
		/// Разбирает строку CSS-классов и добавляет только те из них, чьи базовые
		/// префиксы отсутствуют в текущей коллекции. Существующие префиксы не изменяются.
		/// </summary>
		/// <param name="cssClasses">Строка CSS-классов, разделенная пробелами.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyOriginal(
			string? cssClasses)
		{
			if (string.IsNullOrEmpty(cssClasses))
				return;
			ApplyOriginal(_getDict(cssClasses));
		}


		/// <summary>
		/// Добавляет или перезаписывает элементы из другого словаря классов в текущую коллекцию.
		/// При совпадении префиксов старые суффиксы заменяются новыми.
		/// </summary>
		/// <param name="dict">Словарь префиксов и суффиксов CSS-классов для слияния.</param>
		public void Append(
			Dictionary<string, string[]>? dict)
		{
			if (dict == null)
				return;
			foreach (var item1 in dict)
				Items[item1.Key] = item1.Value;
		}


		/// <summary>
		/// Разбирает и добавляет CSS-классы из строки в текущую коллекцию. 
		/// Существующие в контейнере базовые префиксы при совпадении перезаписываются новыми значениями.
		/// </summary>
		/// <param name="cssClasses">Строка CSS-классов, разделенная пробелами.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string? cssClasses)
		{
			if (string.IsNullOrEmpty(cssClasses))
				return;
			Append(_getDict(cssClasses));
		}


		/// <summary>
		/// Добавляет CSS-классы в текущую коллекцию только в том случае, если переданное логическое условие истинно.
		/// </summary>
		/// <param name="check">Логический флаг (условие), управляющий добавлением классов.</param>
		/// <param name="cssClasses">Строка CSS-классов, разделенная пробелами.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendIf(
			bool check,
			string? cssClasses)
		{
			if (check)
				Append(cssClasses);
		}


		/* functions */


		/// <summary>
		/// Собирает все сгруппированные и добавленные CSS-классы в единую валидную строку, готовую для подстановки в атрибут <c>class</c>.
		/// </summary>
		/// <returns>
		/// Полная строка классов, разделенная пробелами, или <see langword="null"/>, если коллекция строителя пуста.
		/// </returns>
		public override string? ToString()
		{
			if (Items.Count == 0)
				return null;
			var sb1 = new StringBuilder();
			foreach (var item1 in Items)
				foreach (var value1 in item1.Value)
				{
					if (sb1.Length > 0)
						sb1.Append(' ');
					sb1.Append(item1.Key);
					sb1.Append(value1);
				}
			return sb1.ToString();
		}


		/* privates */


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string[] _getItems(
			string cssClasses)
		{
			return cssClasses.Split(
				' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		}


		private static (string key, string value) _getParts(
			string cssClass)
		{
			var i1 = cssClass.LastIndexOf('-');
			if (i1 == -1 || i1 == cssClass.Length - 1)
				return (cssClass, string.Empty);
			i1++;
			return (cssClass[..i1], cssClass[i1..]);
		}


		private static Dictionary<string, string[]> _getDict(
			string cssClasses)
		{
			var dict1 = new Dictionary<string, string[]>();
			foreach (var item1 in _getItems(cssClasses))
			{
				var (key1, value1) = _getParts(item1);
				if (dict1.TryGetValue(key1, out var existingArray1))
					dict1[key1] = existingArray1.GetArrayAdd(value1);
				else
					dict1[key1] = [value1];
			}
			return dict1;
		}

	}

}
