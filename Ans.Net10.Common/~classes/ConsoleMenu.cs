// rev 2026-09-25

using System.Runtime.CompilerServices;

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



	/// <summary>
	/// Предоставляет механизмы создания, наполнения и интерактивного отображения текстового меню в консольном интерфейсе.
	/// </summary>
	public class ConsoleMenu
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="ConsoleMenu"/> без заголовка.
		/// </summary>
		public ConsoleMenu()
		{
		}


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="ConsoleMenu"/> с указанным заголовком.
		/// </summary>
		/// <param name="title">Текст заголовка, отображаемый перед списком пунктов меню.</param>
		public ConsoleMenu(
			string title)
			: this()
		{
			Title = title;
		}


		/* properties */


		/// <summary>
		/// Получает или задает текст заголовка меню.
		/// </summary>
		/// <value>
		/// Строка с заголовком меню или <see langword="null"/>, если заголовок не требуется отображать.
		/// </value>
		public string? Title { get; set; }


		/// <summary>
		/// Получает или задает флаг принудительного запроса на выход из цикла меню.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если требуется завершить выполнение текущего цикла меню; в противном случае — <see langword="false"/>. Значение по умолчанию: <see langword="false"/>.
		/// </value>
		public bool IsExitRequested { get; set; } = false;


		/* readonly properties */


		/// <summary>
		/// Возвращает динамический список зарегистрированных пунктов меню.
		/// </summary>
		/// <value>
		/// Коллекция <see cref="List{ConsoleMenuItem}"/>, содержащая все доступные для выбора элементы меню.
		/// </value>
		public List<ConsoleMenuItem> Items { get; } = [];


		/* methods */


		/// <summary>
		/// Добавляет готовый объект пункта меню в общую коллекцию.
		/// </summary>
		/// <param name="item">Экземпляр класса <see cref="ConsoleMenuItem"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(
			ConsoleMenuItem item)
		{
			Items.Add(item);
		}


		/// <summary>
		/// Создает и добавляет новый пункт меню в общую коллекцию по переданным параметрам.
		/// </summary>
		/// <param name="key">Клавиша активации (<see cref="ConsoleKey"/>).</param>
		/// <param name="title">Отображаемый заголовок пункта.</param>
		/// <param name="action">Выполняемый делегат действия (<see cref="Action"/>).</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(
			ConsoleKey key,
			string title,
			Action action)
		{
			Items.Add(new ConsoleMenuItem(key, title, action));
		}


		/// <summary>
		/// Запускает бесконечный интерактивный цикл отрисовки меню и обработки ввода пользователя.
		/// </summary>
		/// <remarks>
		/// Метод сбрасывает флаг <see cref="IsExitRequested"/> в значение <see langword="false"/> перед стартом.
		/// Цикл автоматически прерывается, если пользователь нажимает клавишу <c>Escape</c> или если внутри вызываемого действия <see cref="IsExitRequested"/> устанавливается в <see langword="true"/>.
		/// Ввод клавиш перехватывается без отображения символа на экране (интерцепция).
		/// </remarks>
		public void Run()
		{
			IsExitRequested = false;
			while (!IsExitRequested)
			{
				Console.WriteLine();
				if (!string.IsNullOrEmpty(Title))
					Console.WriteLine($"{Title}:");
				foreach (var item1 in Items)
				{
					string s1 = item1.Key switch
					{
						>= ConsoleKey.A and <= ConsoleKey.Z
							=> item1.Key.ToString(),
						>= ConsoleKey.D0 and <= ConsoleKey.D9
							=> ((int)item1.Key - (int)ConsoleKey.D0).ToString(),
						_ => ((char)item1.Key).ToString()
					};
					Console.WriteLine($"[{s1}] — {item1.Title}");
				}
				Console.WriteLine("---------------------------");
				Console.Write("Введите требуемый пункт или нажмите Esc для выхода: ");
				var key1 = Console.ReadKey(intercept: true);
				Console.WriteLine();
				if (key1.Key == ConsoleKey.Escape)
				{
					IsExitRequested = true;
					break;
				}
				var match1 = Items.FirstOrDefault(i => i.Key == key1.Key);
				match1?.Action();
			}
		}

	}

}
