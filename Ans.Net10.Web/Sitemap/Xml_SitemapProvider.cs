using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Ans.Net10.Web.Sitemap
{

	/// <summary>
	/// Определяет контракт для поставщика данных карты сайта.
	/// </summary>
	public interface ISitemapProvider
	{
	}



	public sealed partial class Xml_SitemapProvider
		: ISitemapProvider,
		IDisposable
	{

		private const string _NODES_PATH = "Views/Nodes";
		private const string _MAP_NODES_FILE = "_map-nodes.xml";
		private const string _MAP_PAGES_FILE = "_map-pages.xml";

		private readonly IWebHostEnvironment _env;
		private readonly ILogger<Xml_SitemapProvider> _logger;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Xml_SitemapProvider"/>.
		/// </summary>
		public Xml_SitemapProvider(
			IWebHostEnvironment env,
			ILogger<Xml_SitemapProvider> logger)
		{
			_env = env;
			_logger = logger;
		}


		/* functions */





		/* methods */


		/// <inheritdoc />
		public void Dispose()
		{
			//_watcher.Dispose();
			//_cacheLock.Dispose();
			//GC.SuppressFinalize(this);
		}


		/* privates */



		

		/* logs */


		//private static partial class _log
		//{
		//	[LoggerMessage(
		//		EventId = 1001,
		//		Level = LogLevel.Information,
		//		Message = "Запущена асинхронная компиляция карт хранилища...")]
		//	public static partial void MapCompileRun(ILogger logger);

		//	[LoggerMessage(
		//		Level = LogLevel.Warning,
		//		Message = "Глобальная карта узлов не найдена: {Path}")]
		//	public static partial void MapNotFound(ILogger logger, string? path);

		//	[LoggerMessage(
		//		Level = LogLevel.Information,
		//		Message = "Асинхронная компиляция карт хранилища успешно завершена.")]
		//	public static partial void MapCompileSuccess(ILogger logger);

		//	[LoggerMessage(
		//		Level = LogLevel.Critical,
		//		Message = "Критическая ошибка при асинхронной сборке карт навигации. {@Exception}")]
		//	public static partial void MapCompileError(ILogger logger, Exception exception);

		//	[LoggerMessage(
		//		Level = LogLevel.Information,
		//		Message = "Файловое событие реактивности: {name}. Пересчет индексов...")]
		//	public static partial void FileChanged(ILogger logger, string? name);
		//}

	}



	/* XML DTO */



	[XmlRoot("nodes", Namespace = "urn:ans:net10:web:map-nodes")]
	public sealed class NodesXmlDto
	{
		[XmlElement("item")]
		public List<NodeItemXmlDto>? Items { get; set; }
	}



	public sealed class NodeItemXmlDto
	{
		[XmlElement("item")]
		public List<NodeItemXmlDto>? ChildItems { get; set; }

		[XmlAttribute("target")]
		public string? Target { get; set; }

		[XmlAttribute("face")]
		public string? Face { get; set; }
	}



	[XmlRoot("pages", Namespace = "urn:ans:net10:web:map-pages")]
	public sealed class PagesXmlDto
	{
		[XmlElement("item")]
		public List<PageItemXmlDto>? Items { get; set; }
	}



	public sealed class PageItemXmlDto
	{
		[XmlElement("item")]
		public List<PageItemXmlDto>? ChildItems { get; set; }

		[XmlAttribute("target")]
		public string? Target { get; set; }

		[XmlAttribute("face")]
		public string? Face { get; set; }

		[XmlAttribute("start")]
		public bool HasString { get; set; }
	}

}
