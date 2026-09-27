// rev 2026-09-25

using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Хелпер буферизированной записи логов в файл порциями для минимизации дисковых операций ввода-вывода.
	/// </summary>
	public class TinyLogWriterHelper
	{

		private StringBuilder _sb = null!;
		private int _count;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TinyLogWriterHelper"/>.
		/// </summary>
		/// <param name="filename">Полный путь к файлу лога.</param>
		/// <param name="rewrite">Если установлено значение <see langword="true"/>, целевой файл лога будет предварительно очищен.</param>
		/// <param name="length">Максимальное количество записей (порций) в буфере до автоматического сброса на диск.</param>
		public TinyLogWriterHelper(
			string filename,
			bool rewrite,
			int length = 300)
		{
			_init();
			Filename = filename;
			Length = length;
			if (rewrite)
				SuppIO.FileWrite(Filename, string.Empty);
		}


		/* properties */


		/// <summary>
		/// Возвращает или задает лимит количества порций записей в буфере до автосохранения.
		/// </summary>
		/// <value>
		/// Числовое значение лимита операций добавления, после превышения которого автоматически вызывается метод <see cref="Save"/>.
		/// </value>
		public int Length { get; set; }


		/// <summary>
		/// Возвращает полный путь к целевому файлу лога.
		/// </summary>
		/// <value>
		/// Строка, содержащая абсолютный или относительный путь к лог-файлу, переданный при инициализации.
		/// </value>
		public string Filename { get; private set; }


		/* methods */


		/// <summary>
		/// Добавляет текст в буфер лога.
		/// </summary>
		/// <param name="text">Добавляемая текстовая строка. Если передано значение <see langword="null"/>, буфер не изменяется.</param>
		public void Append(
			string? text)
		{
			_sb.Append(text);
			_test();
		}


		/// <summary>
		/// Форматирует и добавляет текст в буфер лога без промежуточных строковых аллокаций.
		/// </summary>
		/// <param name="template">Шаблон строки форматирования (содержит маркеры вида {0}, {1} и т.д.).</param>
		/// <param name="templateArgs">Массив аргументов для подстановки в шаблон строки.</param>
		public void Append(
			string template,
			params object[] templateArgs)
		{
			_sb.AppendFormat(template, templateArgs);
			_test();
		}


		/// <summary>
		/// Добавляет строку с текстом и символом переноса строки в буфер лога.
		/// </summary>
		/// <param name="text">Добавляемая текстовая строка. Если передано значение <see langword="null"/>, в буфер записывается только перенос строки.</param>
		public void AppendLine(
			string? text)
		{
			_sb.AppendLine(text);
			_test();
		}


		/// <summary>
		/// Форматирует и добавляет строку с переносом строки в буфер лога.
		/// </summary>
		/// <param name="template">Шаблон строки форматирования (содержит маркеры вида {0}, {1} и т.д.).</param>
		/// <param name="templateArgs">Массив аргументов для подстановки в шаблон строки.</param>
		public void AppendLine(
			string template,
			params object[] templateArgs)
		{
			_sb
				.AppendFormat(template, templateArgs)
				.AppendLine();
			_test();
		}


		/// <summary>
		/// Форматирует текст, добавляет его в буфер лога и дублирует вывод в стандартную консоль.
		/// </summary>
		/// <param name="template">Шаблон строки форматирования.</param>
		/// <param name="templateArgs">Массив аргументов для подстановки в шаблон строки.</param>
		public void AppendLog(
			string template,
			params object[] templateArgs)
		{
			var s1 = string.Format(template, templateArgs);
			Append(s1);
			Console.Write(s1);
		}


		/// <summary>
		/// Форматирует текст, добавляет строку в буфер лога с переносом и дублирует вывод в консоль.
		/// </summary>
		/// <param name="template">Шаблон строки форматирования.</param>
		/// <param name="args">Массив аргументов для подстановки в шаблон строки.</param>
		public void AppendLineLog(
			string template,
			params object[] args)
		{
			var s1 = string.Format(template, args);
			AppendLine(s1);
			Console.WriteLine(s1);
		}


		/// <summary>
		/// Принудительно сбрасывает все накопленные в буфере логи на диск (в конец файла) и очищает буфер.
		/// </summary>
		/// <remarks>
		/// Запись осуществляется в режиме <see cref="System.IO.FileMode.Append"/>. Если на момент вызова буфер пуст, обращение к диску не производится.
		/// </remarks>
		public void Save()
		{
			if (_sb.Length == 0)
				return;
			SuppIO.FileWrite(Filename, _sb.ToString(), mode: FileMode.Append);
			_init();
		}


		/* privates */


		private void _init()
		{
			_sb = new StringBuilder();
			_count = 0;
		}


		private void _test()
		{
			_count++;
			if (_count > Length)
				Save();
		}

	}

}
