// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Специализированный реестр-словарь объектов стилизации HTML-тегов (<see cref="TagStyler"/>), 
	/// поддерживающий быструю инициализацию из коллекции структурированных строк.
	/// </summary>
	/// <remarks>
	/// Наследуется от базового класса <see cref="Dictionary{TKey, TValue}"/> со строковым ключом.
	/// </remarks>
	public class DictTagStylers
		: Dictionary<string, TagStyler>
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictTagStylers"/> и наполняет его данными, 
		/// разбирая массив строк специального формата.
		/// </summary>
		/// <remarks>
		/// Каждая строка набора разбивается методом расширения <c>SplitFix</c> на 3 сегмента с разделителем <c>'|'</c>: 
		/// <list type="number">
		/// <item><description>Имя ключа (уникальный идентификатор стиля в словаре).</description></item>
		/// <item><description>Строка CSS-классов (передаются в конструктор <see cref="TagStyler"/>).</description></item>
		/// <item><description>Строка инлайновых CSS-стилей (передаются в конструктор <see cref="TagStyler"/>).</description></item>
		/// </list>
		/// Если ключ уже присутствует в словаре или строка <paramref name="serialization"/> является пустой / состоит только из пробелов, 
		/// запись автоматически игнорируется. Если передан <see langword="null"/>, инициализируется пустой словарь.
		/// </remarks>
		/// <param name="serialization">Массив строк параметров конфигурации стилей в формате <c>"ИмяКлюча|Классы|Стили"</c>.</param>
		public DictTagStylers(
			params string[] serialization)
			: base(serialization?.Length ?? 0)
		{
			if (serialization == null)
				return;
			foreach (var item1 in serialization)
			{
				if (string.IsNullOrWhiteSpace(item1))
					continue;
				var a1 = item1.SplitFix("|", 3);
				if (!ContainsKey(a1[0]))
					Add(a1[0], new TagStyler(a1[1], a1[2]));
			}
		}

	}

}
