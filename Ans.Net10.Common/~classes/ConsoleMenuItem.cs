// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Представляет отдельный пункт консольного меню, связывающий горячую клавишу, отображаемый заголовок и выполняемое действие.
	/// </summary>
	/// <param name="key">Клавиша клавиатуры (<see cref="ConsoleKey"/>), при нажатии на которую срабатывает данный пункт меню.</param>
	/// <param name="title">Текстовый заголовок пункта меню, отображаемый на экране.</param>
	/// <param name="action">Делегат действия (<see cref="Action"/>), выполняемый при активации пункта.</param>
	public class ConsoleMenuItem(
		ConsoleKey key,
		string title,
		Action action)
	{
		/// <summary>
		/// Возвращает клавишу активации пункта меню.
		/// </summary>
		public ConsoleKey Key { get; } = key;

		/// <summary>
		/// Возвращает текстовый заголовок пункта меню.
		/// </summary>
		public string Title { get; } = title;

		/// <summary>
		/// Возвращает делегат действия, выполняемый при выборе этого пункта.
		/// </summary>
		public Action Action { get; } = action;
	}

}
