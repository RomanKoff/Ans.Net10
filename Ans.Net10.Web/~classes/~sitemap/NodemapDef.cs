// rev 2026-10-23

namespace Ans.Net10.Web
{

	/// <summary>
	/// Облегченное декларативное определение информационного узла, применяемое для программных инъекций.
	/// </summary>
	public sealed class NodemapDef
		: _MapDef_Base
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodemapDef"/>.
		/// </summary>
		/// <param name="target">Строка назначения (ЧПУ-идентификатор, внутренний или внешний URL).</param>
		/// <param name="face">Человекочитаемое наименование узла.</param>
		/// <param name="childNodes">Массив дочерних узлов верхнего уровня.</param>
		/// <param name="topPages">Массив корневых страниц, принадлежащих узлу.</param>
		/// <param name="display">Декларативное правило отображения элемента ("показ|скрытие").</param>
		/// <param name="passthroughs">Декларативное правило задающих сквозное поведение элемента ("сквозной|обычный").</param>
		public NodemapDef(
			string? target,
			string face,
			NodemapDef[]? childNodes = null,
			PagemapDef[]? topPages = null,
			string? display = null,
			string? passthroughs = null)
		{
			Target = target;
			Face = face;
			ChildNodes = childNodes;
			TopPages = topPages;
			Display = display;
			Passthroughs = passthroughs;
		}

		/// <summary>
		/// Получает строку назначения узла.
		/// </summary>
		/// <returns>Строка назначения либо <see langword="null"/>.</returns>
		public string? Target { get; }

		/// <summary>
		/// Получает отображаемое человекочитаемое наименование узла.
		/// </summary>
		/// <returns>Строка заголовка узла.</returns>
		public string Face { get; }

		/// <summary>
		/// Получает массив описаний дочерних узлов.
		/// </summary>
		/// <returns>Массив объектов <see cref="NodemapDef"/> либо <see langword="null"/>.</returns>
		public NodemapDef[]? ChildNodes { get; }

		/// <summary>
		/// Получает массив описаний корневых страниц узла.
		/// </summary>
		/// <returns>Массив объектов <see cref="PagemapDef"/> либо <see langword="null"/>.</returns>
		public PagemapDef[]? TopPages { get; }
	}

}