// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Хелпер для детального расчёта диапазонов страниц, смещений выборки данных (Skip/Take)
	/// и навигационных состояний интерфейса постраничного вывода (Pagination UI).
	/// </summary>
	public class PaginationHelper
	{

		private int _currentPage = 1;


		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PaginationHelper"/> и сразу вычисляет все навигационные свойства.
		/// </summary>
		/// <remarks>
		/// Параметр <paramref name="offset"/> принудительно ограничивается диапазоном <c>[1..9]</c> с помощью метода <c>SuppMath.GetRestrict</c>. 
		/// Некорректные отрицательные значения или нули автоматически выравниваются в минимально допустимые границы.
		/// </remarks>
		/// <param name="itemsOnPage">Количество отображаемых элементов на одной странице. Если меньше 1, устанавливается значение 1.</param>
		/// <param name="totalItems">Общее количество доступных элементов в коллекции. Если меньше 0, устанавливается значение 0.</param>
		/// <param name="currentPage">Порядковый номер текущей активной страницы. Индексация начинается с 1. Значение по умолчанию: 1.</param>
		/// <param name="offset">Радиус (полуширина) отображения соседних страниц вокруг текущей в блоке интерфейса. Значение по умолчанию: 4.</param>
		public PaginationHelper(
			int itemsOnPage,
			int totalItems,
			int currentPage = 1,
			int offset = 4)
		{
			if (itemsOnPage < 1)
				itemsOnPage = 1;
			if (totalItems < 0)
				totalItems = 0;
			Offset = SuppMath.GetRestrict(offset, 1, 9);
			ItemsOnPage = itemsOnPage;
			TotalItems = totalItems;
			TotalPages = (int)Math.Ceiling(TotalItems / (double)ItemsOnPage);
			CurrentPage = currentPage;
		}


		/* properties */


		/// <summary>
		/// Получает или задает номер текущей активной страницы. 
		/// При изменении автоматически выполняет сквозной пересчет всех связанных навигационных диапазонов и флагов.
		/// </summary>
		/// <remarks>
		/// Если устанавливаемое значение меньше 1, оно принудительно сбрасывается в 1, а свойство <see cref="NotValidIndex"/> принимает значение <see langword="true"/>. 
		/// Если значение превышает <see cref="TotalPages"/>, оно принудительно ограничивается максимальной страницей, а <see cref="NotValidIndex"/> также переходит в <see langword="true"/>.
		/// </remarks>
		/// <value>Целочисленный номер текущей страницы.</value>
		public int CurrentPage
		{
			get => _currentPage;
			set
			{
				_currentPage = value;
				if (_currentPage < 1)
				{
					_currentPage = 1;
					NotValidIndex = true;
				}
				if (TotalPages > 0 && _currentPage > TotalPages)
				{
					_currentPage = TotalPages;
					NotValidIndex = true;
				}
				SkipItems = ItemsOnPage * (_currentPage - 1);
				if (_currentPage < Offset + 1)
				{
					StartPage = 1;
					EndPage = Offset * 2 + 1;
				}
				else if (_currentPage > TotalPages - Offset - 1)
				{
					StartPage = TotalPages - Offset * 2;
					EndPage = TotalPages;
				}
				else
				{
					StartPage = _currentPage - Offset;
					EndPage = _currentPage + Offset;
				}
				if (StartPage < 1)
					StartPage = 1;
				if (EndPage > TotalPages)
					EndPage = TotalPages;
				ActiveFirstPage = (_currentPage == 1);
				ActiveLastPage = (_currentPage == TotalPages);
				HasItemsBefore = (StartPage > 1);
				HasItemsAfter = (EndPage < TotalPages);
				PreviousPage = _currentPage - (2 * Offset);
				if (PreviousPage < 1)
					PreviousPage = 1;
				NextPage = _currentPage + (2 * Offset);
				if (NextPage > TotalPages)
					NextPage = TotalPages;
			}
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает количество элементов, отображаемых на одной странице.
		/// </summary>
		/// <value>Целочисленный фиксированный размер страницы.</value>
		public int ItemsOnPage { get; }


		/// <summary>
		/// Возвращает установленный радиус (полуширину) отображения кнопок страниц вокруг текущей в блоке навигации.
		/// </summary>
		/// <value>Целочисленное значение в диапазоне от 1 до 9.</value>
		public int Offset { get; }


		/// <summary>
		/// Возвращает общее количество элементов в исходной коллекции.
		/// </summary>
		/// <value>Целочисленное общее количество записей.</value>
		public int TotalItems { get; }


		/// <summary>
		/// Возвращает общее рассчитанное количество страниц, вычисленное как <see cref="Math.Ceiling(double)"/> от деления общего числа элементов на размер страницы.
		/// </summary>
		/// <value>Целочисленное общее количество страниц.</value>
		public int TotalPages { get; }


		/// <summary>
		/// Возвращает количество элементов, которые необходимо пропустить в SQL- или LINQ-запросах для достижения текущей страницы.
		/// </summary>
		/// <remarks>
		/// Рассчитывается как: <c>ItemsOnPage * (CurrentPage - 1)</c>. Соответствует параметру <c>Skip</c>.
		/// </remarks>
		/// <value>Целочисленное количество пропускаемых записей.</value>
		public int SkipItems { get; private set; }


		/// <summary>
		/// Возвращает номер страницы для быстрого блочного перехода назад (на двойной радиус <see cref="Offset"/>).
		/// </summary>
		/// <value>Номер целевой страницы. Гарантированно не меньше 1.</value>
		public int PreviousPage { get; private set; }


		/// <summary>
		/// Возвращает номер страницы для быстрого блочного перехода вперед (на двойной радиус <see cref="Offset"/>).
		/// </summary>
		/// <value>Номер целевой страницы. Гарантированно не больше <see cref="TotalPages"/>.</value>
		public int NextPage { get; private set; }


		/// <summary>
		/// Возвращает начальный номер страницы в текущем отображаемом скользящем диапазоне (интервале) навигационного интерфейса.
		/// </summary>
		/// <value>Минимальный номер страницы в блоке видимых кнопок.</value>
		public int StartPage { get; private set; }


		/// <summary>
		/// Возвращает конечный номер страницы в текущем отображаемом скользящем диапазоне (интервале) навигационного интерфейса.
		/// </summary>
		/// <value>Максимальный номер страницы в блоке видимых кнопок.</value>
		public int EndPage { get; private set; }


		/// <summary>
		/// Возвращает признак того, является ли текущая выбранная страница самой первой страницей (началом списка).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если <see cref="CurrentPage"/> равен 1; в противном случае — <see langword="false"/>.</value>
		public bool ActiveFirstPage { get; private set; }


		/// <summary>
		/// Возвращает признак того, является ли текущая выбранная страница самой последней доступной страницей (концом списка).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если <see cref="CurrentPage"/> равен <see cref="TotalPages"/>; в противном случае — <see langword="false"/>.</value>
		public bool ActiveLastPage { get; private set; }


		/// <summary>
		/// Возвращает признак наличия невидимых страниц слева, расположенных до начала текущего отображаемого диапазона.
		/// </summary>
		/// <remarks>
		/// Используется в UI шаблонах для принятия решения об отрисовке левого разделителя-многоточия (<c>"..."</c>).
		/// </remarks>
		/// <value>Значение <see langword="true"/>, если <see cref="StartPage"/> строго больше 1; иначе — <see langword="false"/>.</value>
		public bool HasItemsBefore { get; private set; }


		/// <summary>
		/// Возвращает признак наличия невидимых страниц справа, расположенных после окончания текущего отображаемого диапазона.
		/// </summary>
		/// <remarks>
		/// Используется в UI шаблонах для принятия решения об отрисовке правого разделителя-многоточия (<c>"..."</c>).
		/// </remarks>
		/// <value>Значение <see langword="true"/>, если <see cref="EndPage"/> строго меньше <see cref="TotalPages"/>; иначе — <see langword="false"/>.</value>
		public bool HasItemsAfter { get; private set; }


		/// <summary>
		/// Возвращает значение, указывающее, что переданный в конструктор или свойство изначальный индекс страницы вышел за границы диапазона и был скорректирован.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если произошла принудительная автокоррекция индекса; в противном случае — <see langword="false"/>.</value>
		public bool NotValidIndex { get; private set; }


		/* functions */


		/// <summary>
		/// Формирует детальную строковую диагностическую информацию о текущем математическом состоянии и флагах пагинатора для отладки.
		/// </summary>
		/// <returns>Форматированная текстовая строка, содержащая размеры страниц, текущий срез диапазона в формате <c>[Start-Current-End]</c> и вычисленные смещения.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			var before1 = HasItemsBefore ? $"🡠{PreviousPage} " : string.Empty;
			var after1 = HasItemsAfter ? $" {NextPage}🡢" : string.Empty;
			return $"Items: {ItemsOnPage}/{TotalItems}  Pages: {TotalPages} {before1}[{StartPage}-{CurrentPage}-{EndPage}]{after1}  FirstPage:{ActiveFirstPage}  LastPage:{ActiveLastPage}  Skip:{SkipItems}";
		}

	}

}
