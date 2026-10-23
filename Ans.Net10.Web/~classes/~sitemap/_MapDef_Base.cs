// rev 2026-10-23

namespace Ans.Net10.Web
{

	public class _MapDef_Base
	{
		/// <summary>
		/// Получает или задает декларативное правило отображения элемента ("показ|скрытие").
		/// </summary>
		/// <returns>Строка правила либо <see langword="null"/>.</returns>
		public string? Display { get; internal set; }

		/// <summary>
		/// Получает или задает декларативное правило сквозного прохода элемента ("включение|исключение").
		/// </summary>
		/// <returns>Строка правила либо <see langword="null"/>.</returns>
		public string? Passthroughs { get; internal set; }
	}

}