// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Неизменяемая структура-модель данных пагинации, инкапсулирующая полное состояние постраничной навигации
	/// для легкой передачи на интерфейсный слой (UI/DTO) без аллокаций в куче.
	/// </summary>
	/// <param name="CurrentPage">Порядковый номер текущей активной страницы (индексация с 1).</param>
	/// <param name="TotalItems">Общее количество элементов в исходной отфильтрованной коллекции.</param>
	/// <param name="TotalPages">Общее рассчитанное количество доступных страниц.</param>
	/// <param name="SkipItems">Количество элементов, пропущенных с начала выборки для достижения текущей страницы.</param>
	/// <param name="ItemsOnPage">Максимальное количество отображаемых элементов на одной странице.</param>
	/// <param name="StartPage">Начальный номер страницы в текущем отображаемом скользящем диапазоне кнопок навигации.</param>
	/// <param name="PreviousPage">Номер страницы для быстрого блочного перехода назад (на двойной шаг радиуса).</param>
	/// <param name="NextPage">Номер страницы для быстрого блочного перехода вперед (на двойной шаг радиуса).</param>
	/// <param name="EndPage">Конечный номер страницы в текущем отображаемом скользящем диапазоне кнопок навигации.</param>
	/// <param name="ActiveFirstPage">Флаг, указывающий, является ли текущая выбранная страница самой первой.</param>
	/// <param name="ActiveLastPage">Флаг, указывающий, является ли текущая выбранная страница самой последней.</param>
	/// <param name="HasItemsBefore">Признак наличия скрытых страниц слева от видимого диапазона (для отрисовки многоточия "...").</param>
	/// <param name="HasItemsAfter">Признак наличия скрытых страниц справа от видимого диапазона (для отрисовки многоточия "...").</param>
	/// <param name="NotValidIndex">Признак того, что изначально запрошенный индекс страницы вышел за границы диапазона и был скорректирован.</param>
	/// <param name="Offset">Установленный радиус (полуширина) отображения кнопок страниц вокруг текущей в блоке UI.</param>
	public readonly record struct PaginationModel(
		int CurrentPage,
		int TotalItems,
		int TotalPages,
		int SkipItems,
		int ItemsOnPage,
		int StartPage,
		int PreviousPage,
		int NextPage,
		int EndPage,
		bool ActiveFirstPage,
		bool ActiveLastPage,
		bool HasItemsBefore,
		bool HasItemsAfter,
		bool NotValidIndex,
		int Offset)
	{

		/// <summary>
		/// Инициализирует новый экземпляр структуры <see cref="PaginationModel"/> на основе вычисленного состояния живого хелпера.
		/// </summary>
		/// <remarks>
		/// Выполняет копирование всех рассчитанных свойств из объекта вычислений в неизменяемые поля структуры.
		/// </remarks>
		/// <param name="source">Экземпляр хелпера пагинации <see cref="PaginationHelper"/>, содержащий рассчитанные метаданные.</param>
		public PaginationModel(
			PaginationHelper source)
			: this(
				source.CurrentPage,
				source.TotalItems,
				source.TotalPages,
				source.SkipItems,
				source.ItemsOnPage,
				source.StartPage,
				source.PreviousPage,
				source.NextPage,
				source.EndPage,
				source.ActiveFirstPage,
				source.ActiveLastPage,
				source.HasItemsBefore,
				source.HasItemsAfter,
				source.NotValidIndex,
				source.Offset)
		{
		}

	}

}
