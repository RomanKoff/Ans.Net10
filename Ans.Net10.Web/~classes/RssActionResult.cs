// rev 2026-09-29

using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Кастомный результат действия <see cref="ActionResult"/>, обеспечивающий 
	/// асинхронную сериализацию и отправку новостной ленты в формате RSS 2.0.
	/// </summary>
	/// <remarks>
	/// Данный компонент автоматически устанавливает заголовок ответа Content-Type 
	/// в значение <c>application/rss+xml</c> и выполняет неблокирующую потоковую запись.
	/// </remarks>
	public class RssActionResult
		: ActionResult
	{

		/* properties */


		/// <summary>
		/// Получает или задает объект синдикации новостной ленты.
		/// </summary>
		/// <value>
		/// Экземпляр <see cref="SyndicationFeed"/>, содержащий метаданные ленты и её элементы.
		/// </value>
		public required SyndicationFeed Feed { get; set; }


		/* methods */


		/// <summary>
		/// Асинхронно выполняет обработку результата действия, записывая RSS-ленту в поток HTTP-ответа.
		/// </summary>
		/// <param name="context">
		/// Контекст, в котором выполняется действие, включая HTTP-контекст запроса.
		/// </param>
		/// <returns>
		/// Поток-задача <see cref="Task"/>, представляющая асинхронную операцию выполнения результата.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> равен <see langword="null"/>.
		/// </exception>
		public override async Task ExecuteResultAsync(
			ActionContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			context.HttpContext.Response.ContentType = "application/rss+xml";
			var formatter1 = new Rss20FeedFormatter(Feed);
			await using var writer1 = XmlWriter.Create(
				context.HttpContext.Response.Body,
				new XmlWriterSettings
				{
					Async = true,
					Encoding = Encoding.UTF8,
					Indent = true
				});
			formatter1.WriteTo(writer1);
			await writer1.FlushAsync();
		}

	}

}
