// rev 2026-10-07

using Ans.Net10.Common;
using MimeKit;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Служба отправки уведомлений и системных сообщений, автоматизирующая рендеринг 
	/// Razor-шаблонов и их последующую асинхронную доставку по электронной почте.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="SendService"/> с использованием первичного конструктора C#.
	/// </remarks>
	/// <param name="current">Текущий оркестровый контекст обработки запроса.</param>
	public class SendService(
		CurrentContext current)
	{

		private readonly CurrentContext _current
			= current
				?? throw new ArgumentNullException(nameof(current));


		/* methods */


		/// <summary>
		/// Асинхронно формирует электронное письмо на основе Razor-представления и отправляет его адресату.
		/// </summary>
		/// <remarks>
		/// Метод полностью асинхронный и не блокирует потоки ввода-вывода (I/O threads) веб-сервера.
		/// </remarks>
		/// <param name="name">Отображаемое имя главного получателя письма.</param>
		/// <param name="address">Электронный почтовый адрес главного получателя.</param>
		/// <param name="subject">Тема (заголовок) отправляемого письма.</param>
		/// <param name="viewName">Имя или полный путь к Razor-представлению, используемому в качестве HTML-шаблона тела письма.</param>
		/// <param name="model">Объект доменной модели данных, передаваемый внутрь Razor-шаблона для рендеринга.</param>
		/// <param name="bcc">Опциональный список адресов скрытой копии (BCC), разделенных точкой с запятой (;).</param>
		/// <returns>
		/// Поток-задача, представляющая асинхронную операцию подготовки и отправки письма.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если любой из обязательных строковых параметров или параметр <paramref name="model"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Вызывается, если переданный адрес имеет некорректный формат или является пустой строкой.
		/// </exception>
		public async Task EmailAsync(
			string name,
			string address,
			string subject,
			string viewName,
			object model,
			string? bcc = null)
		{
			ArgumentNullException.ThrowIfNull(name);
			ArgumentNullException.ThrowIfNull(address);
			ArgumentNullException.ThrowIfNull(subject);
			ArgumentNullException.ThrowIfNull(viewName);
			ArgumentNullException.ThrowIfNull(model);
			if (string.IsNullOrWhiteSpace(address))
				throw new ArgumentException("[SendService] Адрес получателя не может быть пустым.", nameof(address));
			var toAddress1 = new MailboxAddress(name, address);
			var htmlContent1 = await _current.ViewRender.RenderViewToStringAsync(viewName, model);
			var message1 = new MailMessageModel
			{
				To = toAddress1,
				Subject = subject,
				ContentHtml = htmlContent1
			};
			if (!string.IsNullOrEmpty(bcc))
			{
				var bccAddresses1 = bcc
					.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					.Where(addr => !string.IsNullOrWhiteSpace(addr))
					.Select(addr => new MailboxAddress(string.Empty, addr));
				message1.Bcc = [.. bccAddresses1];
			}
			await _current.Mailer.SendAsync(message1);
		}

	}

}
