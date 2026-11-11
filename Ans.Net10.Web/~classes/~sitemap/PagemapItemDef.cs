// rev 2026-11-05

namespace Ans.Net10.Web
{

	/// <summary>
	/// Облегченное декларативное определение контентной страницы, применяемое для программных инъекций.
	/// </summary>
	public sealed class PagemapItemDef
		: _MapItemDef_Base
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PagemapItemDef"/>.
		/// </summary>
		/// <param name="target">Строка назначения страницы (ЧПУ-сегмент или путь).</param>
		/// <param name="face">Человекочитаемое название страницы.</param>
		/// <param name="hasNoIndex">Признак отключения индексации для страницы и её потомков.</param>
		/// <param name="childPages">Массив вложенных дочерних страниц.</param>
		/// <param name="display">Декларативное правило отображения элемента ("показ|скрытие").</param>
		/// <param name="passthroughs">Декларативное правило задающих сквозное поведение элемента ("сквозной|обычный").</param>
		public PagemapItemDef(
			string target,
			string? face,
			bool hasNoIndex = false,
			PagemapItemDef[]? childPages = null,
			string? display = null,
			string? passthroughs = null)
		{
			Target = target;
			Face = face;
			HasNoIndex = hasNoIndex;
			ChildPages = childPages;
			Display = display;
			Passthroughs = passthroughs;
		}

		/// <summary>
		/// Получает оригинальную строку назначения страницы.
		/// </summary>
		/// <returns>Строковый идентификатор назначения.</returns>
		public string Target { get; }

		/// <summary>
		/// Получает человекочитаемое наименование страницы.
		/// </summary>
		/// <returns>Название страницы либо <see langword="null"/>.</returns>
		public string? Face { get; }

		/// <summary>
		/// Получает значение, указывающее на отключение индексации потомков.
		/// </summary>
		/// <returns><see langword="true"/>, если индексация отключена; иначе — <see langword="false"/>.</returns>
		public bool HasNoIndex { get; }

		/// <summary>
		/// Получает массив вложенных дочерних страниц.
		/// </summary>
		/// <returns>Массив объектов <see cref="PagemapItemDef"/> либо <see langword="null"/>.</returns>
		public PagemapItemDef[]? ChildPages { get; }
	}

}