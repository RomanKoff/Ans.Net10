// rev 2026-11-05

namespace Ans.Net10.Web
{

	/// <summary>
	/// Базовый класс для облегченных декларативных определений элементов карт сайта.
	/// </summary>
	public class _MapItemDef_Base
	{
		/// <summary>
		/// Получает или задает декларативное правило отображения элемента ("показ|скрытие").
		/// </summary>
		/// <returns>Строка правила видимости либо <see langword="null"/>.</returns>
		public string? Display { get; internal set; }

		/// <summary>
		/// Получает или задает декларативное правило сквозного прохода элемента ("включение|исключение").
		/// </summary>
		/// <returns>Строка правила прозрачности либо <see langword="null"/>.</returns>
		public string? Passthroughs { get; internal set; }
	}

}