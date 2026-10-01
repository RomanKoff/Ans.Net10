// rev 2026-09-30

using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Хелпер буферизированной записи логов в файл порциями для минимизации и автоматизации 
	/// асинхронных дисковых операций ввода-вывода.
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
		/// <exception cref="ArgumentException">Вызывается, если путь <paramref name="filename"/> пуст.</exception>
		public TinyLogWriterHelper(
			string filename,
			bool rewrite,
			int length = 300)
		{
			ArgumentException.ThrowIfNullOrEmpty(filename);
			_init();
			Filename = filename;
			Length = length;
			if (rewrite)
				File.WriteAllText(Filename, string.Empty);
		}


		/* properties */


		/// <summary>
		/// Возвращает или задает лимит количества порций записей в буфере до автосохранения.
		/// </summary>
		/// <value>
		/// Числовое значение лимита операций добавления, после превышения которого автоматически вызывается асинхронный метод сброса на диск.
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
		/// Добавляет текст в буфер лога. Синхронная операция для обеспечения максимального быстродействия и потокобезопасности буфера.
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
		/// <param name="template">Шаблон строки форматирования.</param>
		/// <param name="templateArgs">Массив аргументов для подстановки в шаблон строки.</param>
		public void AppendLine(
			string template,
			params object[] templateArgs)
		{
			_sb.AppendFormat(template, templateArgs).AppendLine();
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
		/// Принудительно и асинхронно сбросить все накопленные в буфере логи на диск (в конец файла) и очистить буфер без блокировки вызывающего потока.
		/// </summary>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Временная задача <see cref="ValueTask"/>, представляющая асинхронную операцию записи.</returns>
		public async ValueTask SaveAsync(
			CancellationToken cancellationToken = default)
		{
			if (_sb.Length == 0)
				return;
			await SuppIO.FileWriteAsync(Filename, _sb.ToString(), EncodingsEnum.UTF8, FileMode.Append, cancellationToken);
			_clearBuffer();
		}


		/* privates */


		private void _init()
		{
			_sb = new StringBuilder();
			_count = 0;
		}


		private void _clearBuffer()
		{
			_sb.Clear();
			_count = 0;
		}


		private void _test()
		{
			_count++;
			if (_count > Length)
				Task.Run(async () => await SaveAsync(CancellationToken.None));
		}

	}

}
