// rev 2026-10-23

namespace Ans.Net10.Web
{

	/// <summary>
	/// Облегченное декларативное определение контентной страницы, применяемое для программных инъекций.
	/// </summary>
	public sealed class PagemapDef
		: _MapDef_Base
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PagemapDef"/>.
		/// </summary>
		/// <param name="target">Строка назначения страницы (ЧПУ-сегмент или путь).</param>
		/// <param name="face">Человекочитаемое название страницы.</param>
		/// <param name="hasNoIndex">Признак отключения индексации для страницы и её потомков.</param>
		/// <param name="childPages">Массив вложенных дочерних страниц.</param>
		/// <param name="display">Декларативное правило отображения элемента ("показ|скрытие").</param>
		/// <param name="passthroughs">Декларативное правило задающих сквозное поведение элемента ("сквозной|обычный").</param>
		public PagemapDef(
			string target,
			string? face,
			bool hasNoIndex = false,
			PagemapDef[]? childPages = null,
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
		/// <returns>Массив объектов <see cref="PagemapDef"/> либо <see langword="null"/>.</returns>
		public PagemapDef[]? ChildPages { get; }
	}

}