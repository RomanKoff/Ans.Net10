// rev 2026-09-26

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{


	/// <summary>
	/// Вспомогательный класс для работы с консольным пользовательским интерфейсом (Console UI).
	/// </summary>
	public static class SuppConsole
	{

		private static readonly string _lineV = "-".MakeRepeats(80);
		private static readonly string _lineW = "=".MakeRepeats(80);

		private static int _cursorLeft;
		private static int _cursorTop;
		private static int _counter;


		/// <summary>
		/// Выводит в консоль оформленный заголовок начала работы приложения с кастомным названием.
		/// </summary>
		/// <param name="title">Отображаемый заголовок приложения.</param>
		public static void AppStart(
			string title)
		{
			Console.WriteLine();
			WriteColor(ConsoleColor.Green, _lineW);
			Console.WriteLine();
			WriteColor(ConsoleColor.Green, title);
			Console.WriteLine();
			WriteColor(ConsoleColor.Green, _lineW);
			Console.WriteLine();
			Console.WriteLine();
		}


		/// <summary>
		/// Выводит в консоль оформленный заголовок начала работы приложения, автоматически используя имя вызывающей сборки.
		/// </summary>
		public static void AppStart()
		{
			AppStart(Assembly.GetCallingAssembly().GetName().Name ?? "App");
		}


		/// <summary>
		/// Выводит финальную строку и ожидает нажатия любой клавиши перед закрытием консоли.
		/// </summary>
		public static void AppEnd()
		{
			Console.WriteLine();
			WriteColor(ConsoleColor.Green, _lineW);
			Console.WriteLine();
			Console.WriteLine(Resources.Common.Text_PressAnyKeyToExit);
			_ = Console.ReadKey(true);
			Console.WriteLine();
		}


		/// <summary>
		/// Выводит в консоль заголовок начала крупной логической секции.
		/// </summary>
		/// <param name="title">Название запускаемой секции.</param>
		public static void SectionStart(
			string title)
		{
			Console.WriteLine();
			WriteColor(ConsoleColor.Yellow, _lineV);
			Console.WriteLine();
			WriteColor(ConsoleColor.Yellow, title);
			Console.WriteLine();
			WriteColor(ConsoleColor.Yellow, _lineV);
			Console.WriteLine();
			Console.WriteLine();
		}


		/// <summary>
		/// Выводит в консоль заголовок начала подразделения или локальной части.
		/// </summary>
		/// <param name="title">Название подраздела.</param>
		public static void PartStart(
			string title)
		{
			Console.WriteLine();
			WriteColor(ConsoleColor.DarkYellow, $"--- {title} ---");
			Console.WriteLine();
			Console.WriteLine();
		}


		/// <summary>
		/// Очищает буфер ввода консоли от накопленных, но не обработанных нажатий клавиш.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InputClear()
		{
			while (Console.KeyAvailable)
				_ = Console.ReadKey(true);
		}


		/// <summary>
		/// Очищает буфер ввода и считывает информацию о нажатой клавише пользователя без вывода её на экран.
		/// </summary>
		/// <returns>Объект <see cref="ConsoleKeyInfo"/> с метаданными нажатой клавиши.</returns>
		public static ConsoleKeyInfo ReadKey()
		{
			InputClear();
			return Console.ReadKey(true);
		}


		/*--- CURSOR ---*/


		/// <summary>
		/// Запоминает во внутренней переменной текущие координаты курсора в буфере консоли.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CursorSavePos()
		{
			_cursorLeft = Console.CursorLeft;
			_cursorTop = Console.CursorTop;
		}


		/// <summary>
		/// Восстанавливает позицию курсора на координаты, зафиксированные при последнем вызове <see cref="CursorSavePos"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CursorRestorePos()
		{
			Console.SetCursorPosition(_cursorLeft, _cursorTop);
		}


		/// <summary>
		/// Смещает позицию курсора в крайнее левое положение (индекс 0) текущей строки.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CursorToBeginLine()
		{
			Console.SetCursorPosition(0, Console.CursorTop);
		}


		/*--- COUNTER ---*/


		/// <summary>
		/// Инициализирует счетчик прогресса и сохраняет текущую точку возврата курсора.
		/// </summary>
		/// <param name="counter">Начальное числовое значение счетчика.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetCounter(
			int counter)
		{
			CursorSavePos();
			_counter = counter;
		}


		/// <summary>
		/// Увеличивает значение счетчика на единицу и обновляет его визуальное отображение на экране на прежней позиции.
		/// </summary>
		public static void CounterUp()
		{
			_counter++;
			CursorRestorePos();
			Console.Write(_counter);
			Console.WriteLine(" ");
		}


		/// <summary>
		/// Уменьшает значение счетчика на единицу и обновляет его визуальное отображение на экране на прежней позиции.
		/// </summary>
		public static void CounterDown()
		{
			_counter--;
			CursorRestorePos();
			Console.Write(_counter);
			Console.WriteLine(" ");
		}


		/*--- WRITE ---*/


		/// <summary>
		/// Формирует строку для вывода на основе коллекции элементов, оборачивая строковые типы в кавычки.
		/// </summary>
		/// <typeparam name="T">Тип элементов в коллекции.</typeparam>
		/// <param name="items">Исходная последовательность элементов. Допускает значение <see langword="null"/>.</param>
		/// <returns>Строковое перечисление элементов в квадратных скобках, разделенное запятыми.</returns>
		public static string ArrToText<T>(
			IEnumerable<T>? items)
		{
			if (items == null)
				return string.Empty;
			var f1 = typeof(T) == typeof(string) ? "\"{0}\"" : null;
			return items.MakeFromCollection(x => x?.ToString(), "[{0}]", f1, ", ");
		}


		/// <summary>
		/// Полностью очищает текущую строку консоли, заполняя её пробелами с использованием высокопроизводительного буфера.
		/// </summary>
		public static void ClearLine()
		{
			CursorToBeginLine();
			int width = Console.BufferWidth;
			if (width <= 256)
			{
				Span<char> buffer = stackalloc char[width];
				buffer.Fill('\x20');
				Console.Write((ReadOnlySpan<char>)buffer);
			}
			else
			{
				string bufferStr = string.Create(width, '\x20', static (span, space) => span.Fill(space));
				Console.Write(bufferStr);
			}
			CursorToBeginLine();
		}


		/// <summary>
		/// Выводит в консоль разделительную линию из дефисов.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BreakLine()
		{
			Console.WriteLine(_lineV);
		}


		/// <summary>
		/// Выводит в консоль жирную разделительную линию из знаков равенства.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BreakLineW()
		{
			Console.WriteLine(_lineW);
		}


		/// <summary>
		/// Выводит текст в консоль с изменением цвета шрифта и последующим автоматическим восстановлением системной палитры.
		/// </summary>
		/// <param name="fgColor">Целевой цвет текста.</param>
		/// <param name="text">Выводимое строковое сообщение.</param>
		public static void WriteColor(
			ConsoleColor fgColor,
			string text)
		{
			var save1 = Console.ForegroundColor;
			Console.ForegroundColor = fgColor;
			Console.Write(text);
			Console.ForegroundColor = save1;
		}


		/// <summary>
		/// Выводит текст цветным шрифтом, если логическое условие верно; в противном случае выводит текст стандартным цветом.
		/// </summary>
		/// <param name="expression">Флаг применения кастомного окрашивания текста.</param>
		/// <param name="fgColor">Кастомный цвет текста.</param>
		/// <param name="text">Выводимое строковое сообщение.</param>
		public static void WriteColor(
			bool expression,
			ConsoleColor fgColor,
			string text)
		{
			if (expression)
				WriteColor(fgColor, text);
			else
				Console.Write(text);
		}


		/// <summary>
		/// Выводит структурированную пару "название: значение" с опциональным цветовым выделением значения.
		/// </summary>
		/// <param name="title">Название выводимого параметра.</param>
		/// <param name="value">Значение параметра (примитив или сложный объект).</param>
		/// <param name="fgColor">Цвет шрифта для вывода значения. Если <see langword="null"/> — используется зеленый.</param>
		public static void WriteLineParam(
			string title,
			object? value,
			ConsoleColor? fgColor = null)
		{
			var save1 = Console.ForegroundColor;
			Console.Write($"{title}: ");
			Console.ForegroundColor = fgColor ?? ConsoleColor.Green;
			if (value != null)
				Console.Write(SuppReflection.GetCSharpValue(value, true));
			Console.ForegroundColor = save1;
			Console.WriteLine();
		}


		/// <summary>
		/// Выводит общее количество элементов словаря и построчно логирует его записи через кастомную функцию.
		/// </summary>
		/// <typeparam name="T">Тип значений в словаре. Должен быть ссылочным (<see langword="class"/>).</typeparam>
		/// <param name="title">Общий заголовок выводимого блока данных.</param>
		/// <param name="dictionary">Словарь с данными.</param>
		/// <param name="itemLog">Делегат функции формирования строки для каждой записи KeyValuePair.</param>
		public static void WriteDict<T>(
			string title,
			IDictionary<string, T> dictionary,
			Func<KeyValuePair<string, T>, string> itemLog)
			where T : class
		{
			int count1 = dictionary.Count;
			Console.WriteLine($"{title}: {count1}");
			foreach (var item1 in dictionary)
				Console.WriteLine(itemLog(item1));
			Console.WriteLine();
		}


		/// <summary>
		/// Выводит заголовок, рассчитывает количество элементов последовательности, логирует их и выполняет заданное действие над каждым объектом.
		/// </summary>
		/// <typeparam name="T">Тип элементов коллекции. Должен быть ссылочным (<see langword="class"/>).</typeparam>
		/// <param name="title">Заголовок выводимого списка.</param>
		/// <param name="items">Последовательность элементов.</param>
		/// <param name="itemLog">Функция логирования элемента.</param>
		/// <param name="itemAction">Опциональное действие, выполняемое над элементом в цикле обхода.</param>
		public static void WriteItems<T>(
			string title,
			IEnumerable<T> items,
			Func<T, string> itemLog,
			Action<T> itemAction)
			where T : class
		{
			int count1 = items switch
			{
				ICollection<T> c => c.Count,
				IReadOnlyCollection<T> rc => rc.Count,
				_ => items.Count()
			};
			Console.WriteLine($"{title}: {count1}");
			foreach (var item1 in items)
			{
				Console.WriteLine(itemLog(item1));
				itemAction?.Invoke(item1);
			}
			Console.WriteLine();
		}


		/// <summary>
		/// Выводит текст в текущую позицию консоли, сохраняя и моментально восстанавливая координаты курсора ("замороженный" вывод).
		/// </summary>
		/// <param name="template">Шаблон строки вывода.</param>
		/// <param name="args">Параметры форматирования.</param>
		public static void WriteFreeze(
			string template,
			params object[] args)
		{
			CursorSavePos();
			Console.Write(template, args);
			CursorRestorePos();
		}


		/*--- MENU ---*/


		/// <summary>
		/// Инициализирует и запускает интерактивное текстовое меню в консоли на основе переданного списка элементов.
		/// </summary>
		/// <param name="items">Коллекция пунктов меню.</param>
		public static void MakeMenu(
			IEnumerable<ConsoleMenuItem> items)
		{
			var menu1 = new ConsoleMenu();
			foreach (var item1 in items)
				menu1.Add(item1);
			menu1.Run();
		}


		/// <summary>
		/// Выводит вопрос в консоль и ожидает от пользователя подтверждения в формате Да/Нет (клавиши Y/N).
		/// </summary>
		/// <param name="query">Текст отображаемого вопроса.</param>
		/// <returns><see langword="true"/>, если пользователь нажал 'y' или 'Y'; иначе — <see langword="false"/>.</returns>
		public static bool GetCase(
			string query)
		{
			Console.Write($"{query} (y|n)? ");
			InputClear();
			var key1 = Console.ReadKey(true);
			Console.WriteLine();
			return key1.KeyChar is 'y' or 'Y';
		}

	}

}
