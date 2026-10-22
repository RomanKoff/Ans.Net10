// rev 2026-10-22

using Ans.Net10.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Определяет контракт поставщика данных и управления жизненным циклом карты сайта.
	/// </summary>
	public interface ISitemapProvider
	{
		/// <summary>
		/// Получает актуальный, полностью проиндексированный экземпляр сборщика карты сайта.
		/// </summary>
		/// <value>Экземпляр <see cref="SitemapBuilder"/>, содержащий текущий граф навигации.</value>
		/// <returns>Текущий проиндексированный граф <see cref="SitemapBuilder"/>.</returns>
		SitemapBuilder Current { get; }

		/// <summary>
		/// Асинхронно выполняет первичную инициализацию поставщика и запускает процессы отслеживания изменений.
		/// </summary>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, представляющая асинхронную операцию инициализации.</returns>
		Task InitializeAsync(CancellationToken cancellationToken = default);
	}



	/// <summary>
	/// Фиктивная пустая реализация поставщика карты сайта, используемая в качестве 
	/// Null-object паттерна, когда функционал реактивной карты сайта отключен в конфигурации.
	/// </summary>
	public sealed class Fake_SitemapProvider
		: ISitemapProvider
	{
		/// <inheritdoc />
		public SitemapBuilder Current
			=> new();

		/// <inheritdoc />
		public Task InitializeAsync(CancellationToken cancellationToken = default)
			=> Task.CompletedTask;
	}




	/// <summary>
	/// Реализация файлового XML-поставщика карты сайта, осуществляющая реактивный 
	/// мониторинг каталога и атомарное асинхронное обновление навигационного графа в памяти.
	/// </summary>
	public sealed partial class Xml_SitemapProvider
		: ISitemapProvider,
		IDisposable
	{

		private const string _MAPNODES_FILENAME = "_map-nodes.xml";
		private const string _MAPPAGES_FILENAME = "_map-pages.xml";

		private readonly IWebHostEnvironment _env;
		private readonly ILogger<Xml_SitemapProvider> _logger;
		private readonly string _rootFolder1;
		private readonly FileSystemWatcher _watcher1;
		private readonly Lock _lock1 = new();

		private SitemapBuilder _currentBuilder1;
		private CancellationTokenSource? _debounceCts1;
		private bool _isInitialized;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Xml_SitemapProvider"/>.
		/// </summary>
		/// <param name="env">Среда выполнения веб-приложения.</param>
		/// <param name="configuration">Конфигурация приложения для извлечения настроек.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="env"/> или <paramref name="logger"/> равен <see langword="null"/>.</exception>
		public Xml_SitemapProvider(
			IWebHostEnvironment env,
			IConfiguration configuration,
			ILogger<Xml_SitemapProvider> logger)
		{
			var options1 = AppSettingsFactory.GetOptions<LibWebOptions>(configuration);
			SourcePath = options1.XmlSitemap?.Path ?? string.Empty;
			_env = env ?? throw new ArgumentNullException(nameof(env));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_rootFolder1 = Path.Combine(_env.ContentRootPath, SourcePath);
			_currentBuilder1 = new SitemapBuilder();

			_watcher1 = new FileSystemWatcher(_rootFolder1)
			{
				Filter = "_map-*.xml",
				IncludeSubdirectories = true,
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
			};

			_watcher1.Created += _onMapFileChanged;
			_watcher1.Changed += _onMapFileChanged;
			_watcher1.Deleted += _onMapFileChanged;
			_watcher1.Renamed += _onMapFileChanged;
			_watcher1.Error += _onWatcherError;
		}


		/* readonly properties */


		/// <inheritdoc />
		public SitemapBuilder Current
			=> _currentBuilder1;


		/// <summary>
		/// Получает настроенный относительный путь к каталогу с XML-картами.
		/// </summary>
		/// <returns>Строка относительного пути к каталогу ресурсов.</returns>
		public string SourcePath { get; }


		/* methods */


		/// <inheritdoc />
		public async Task InitializeAsync(
			CancellationToken cancellationToken = default)
		{
			if (_isInitialized)
				return;
			using (_lock1.EnterScope())
			{
				if (_isInitialized)
					return;
				if (!Directory.Exists(_rootFolder1))
				{
					_log.FolderNotFoundWarning(_logger, _rootFolder1);
					_isInitialized = true;
					return;
				}
			}
			try
			{
				await _rebuildSitemapAsync();
				_watcher1.EnableRaisingEvents = true;
				_log.ProviderInitialized(_logger, _rootFolder1);
				_isInitialized = true;
			}
			catch (Exception ex1)
			{
				_log.InitializationCriticalError(_logger, ex1);
				throw;
			}
		}


		/// <inheritdoc />
		public void Dispose()
		{
			_watcher1.EnableRaisingEvents = false;
			_watcher1.Created -= _onMapFileChanged;
			_watcher1.Changed -= _onMapFileChanged;
			_watcher1.Deleted -= _onMapFileChanged;
			_watcher1.Renamed -= _onMapFileChanged;
			_watcher1.Error -= _onWatcherError;
			_watcher1.Dispose();
			using (_lock1.EnterScope())
			{
				_debounceCts1?.Cancel();
				_debounceCts1?.Dispose();
			}
		}


		/* privates */


		private async Task _loadSitemapDataAsync(
			SitemapBuilder builder)
		{
			string nodesFile1 = Path.Combine(_rootFolder1, _MAPNODES_FILENAME);
			if (File.Exists(nodesFile1))
			{
				NodesXmlDto? nodesDto1 = null;
				try
				{
					nodesDto1 = await SuppXml.GetSoftObjectFromXmlFileAsync<NodesXmlDto>(nodesFile1, null, null);
				}
				catch (Exception ex1)
				{
					_log.NodesFileParsingError(_logger, nodesFile1, ex1);
				}
				if (nodesDto1 is not null && nodesDto1.Items is not null)
					foreach (var itemDto1 in nodesDto1.Items)
					{
						var node1 = _parseNodeRecursive(itemDto1);
						builder.AddTopNode(node1);
					}
			}
			bool hasMainNode1 = _hasNodeWithNameRecursive(
				builder.TopNodes, LibWebConsts.INDEX_NODE_NAME);
			if (!hasMainNode1)
			{
				var defaultMainNode1 = new NodemapItem(LibWebConsts.INDEX_NODE_NAME, string.Empty);
				builder.InsertTopNode(0, defaultMainNode1);
			}
			var tasks1 = new List<Task>();
			_processAllNodesFlat(builder.TopNodes, node1 =>
			{
				if (node1.Type == NodemapTypesEnum.Node && node1.Name is not null)
					tasks1.Add(_loadPagesForNodeAsync(node1));
			});
			await Task.WhenAll(tasks1);
		}


		private async Task _loadPagesForNodeAsync(
			NodemapItem node)
		{
			string pagesFile1 = Path.Combine(_rootFolder1, node.Name!, _MAPPAGES_FILENAME);
			if (!File.Exists(pagesFile1))
				return;
			PagesXmlDto? pagesDto1 = null;
			try
			{
				pagesDto1 = await SuppXml.GetSoftObjectFromXmlFileAsync<PagesXmlDto>(pagesFile1, null, null);
			}
			catch (Exception ex1)
			{
				_log.PagesFileParsingError(_logger, pagesFile1, ex1);
			}
			if (pagesDto1 is not null && pagesDto1.Items is not null)
				foreach (var pageDto1 in pagesDto1.Items)
					_parsePageRecursive(pageDto1, node, null);
		}


		private static void _processAllNodesFlat(
			IEnumerable<NodemapItem> nodes,
			Action<NodemapItem> action)
		{
			foreach (var node1 in nodes)
			{
				action(node1);
				_processAllNodesFlat(node1.ChildNodes, action);
			}
		}


		private static bool _hasNodeWithNameRecursive(
			IEnumerable<NodemapItem> nodes,
			string name)
		{
			foreach (var node1 in nodes)
			{
				if (string.Equals(node1.Name, name, StringComparison.OrdinalIgnoreCase))
					return true;
				if (_hasNodeWithNameRecursive(node1.ChildNodes, name))
					return true;
			}
			return false;
		}


		private static NodemapItem _parseNodeRecursive(
			NodeItemXmlDto nodeDto)
		{
			string target1 = nodeDto.Target ?? string.Empty;
			string face1 = string.IsNullOrWhiteSpace(nodeDto.Face)
				? target1 : nodeDto.Face;
			var node1 = new NodemapItem(
				target1, face1,
				nodeDto.TagHides, nodeDto.TagShows, nodeDto.TagPassthroughs);
			if (nodeDto.ChildItems is not null)
				foreach (var childDto1 in nodeDto.ChildItems)
				{
					var childNode1 = _parseNodeRecursive(childDto1);
					node1.AddChildNode(childNode1);
				}
			return node1;
		}


		private static void _parsePageRecursive(
			PageItemXmlDto pageDto,
			NodemapItem node,
			PagemapItem? parentPage)
		{
			string target1 = pageDto.Target;
			string face1 = pageDto.Face ?? string.Empty;
			var page1 = new PagemapItem(
				node, target1, face1, pageDto.HasNoIndex,
				pageDto.TagHides, pageDto.TagShows, pageDto.TagPassthroughs);
			if (parentPage is null)
				node.AddTopPage(page1);
			else
				parentPage.AddChildPage(page1);
			if (page1.Type == PagemapTypesEnum.Page && pageDto.ChildItems is not null)
				foreach (var childDto1 in pageDto.ChildItems)
					_parsePageRecursive(childDto1, node, page1);
		}


		private void _onMapFileChanged(
			object sender,
			FileSystemEventArgs e)
		{
			_log.FileChangedInfo(_logger, e.Name, e.ChangeType);
			_scheduleRebuild();
		}


		private void _onWatcherError(
			object sender,
			ErrorEventArgs e)
		{
			_log.WatcherError(_logger, e.GetException());
			_scheduleRebuild();
		}


		private void _scheduleRebuild()
		{
			CancellationTokenSource cts1;
			using (_lock1.EnterScope())
			{
				_debounceCts1?.Cancel();
				_debounceCts1?.Dispose();
				_debounceCts1 = new CancellationTokenSource();
				cts1 = _debounceCts1;
			}
			var token1 = cts1.Token;
			Task.Run(async () =>
			{
				try
				{
					await Task.Delay(500, token1);
					await _rebuildSitemapAsync();
				}
				catch (OperationCanceledException)
				{
					// Пропускаем — сработал дебаунс (пришло новое изменение файла)
				}
				catch (Exception ex1)
				{
					_log.RebuildError(_logger, ex1);
				}
			}, token1);
		}


		private async Task _rebuildSitemapAsync()
		{
			_log.RebuildStartedInfo(_logger);
			var newBuilder1 = new SitemapBuilder();
			await _loadSitemapDataAsync(newBuilder1);
			Interlocked.Exchange(ref _currentBuilder1, newBuilder1);
			_log.RebuildSuccessInfo(_logger);
		}


		/* logs */


		private static partial class _log
		{
			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "Xml_SitemapProvider успешно инициализирован. Запущено отслеживание изменений в {Path}")]
			public static partial void ProviderInitialized(ILogger logger, string path);

			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "Каталог карт сайта не найден по пути: {Path}. Мониторинг изменений отключен.")]
			public static partial void FolderNotFoundWarning(ILogger logger, string path);

			[LoggerMessage(
				Level = LogLevel.Critical,
				Message = "Критическая ошибка при первичной инициализации карты сайта.")]
			public static partial void InitializationCriticalError(ILogger logger, Exception ex);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "Зафиксировано изменение файла карты сайта: {FileName} ({ChangeType}). Запуск таймера обновления...")]
			public static partial void FileChangedInfo(ILogger logger, string? fileName, WatcherChangeTypes changeType);

			[LoggerMessage(
				Level = LogLevel.Error,
				Message = "Зафиксирован сбой в работе FileSystemWatcher. Запуск принудительной ревизии карты сайта...")]
			public static partial void WatcherError(ILogger logger, Exception ex);

			[LoggerMessage(
				Level = LogLevel.Error,
				Message = "Ошибка при автоматическом фоновом пересчете карты сайта.")]
			public static partial void RebuildError(ILogger logger, Exception ex);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "Начало фонового пересчета и индексации карты сайта...")]
			public static partial void RebuildStartedInfo(ILogger logger);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "Карта сайта успешно обновлена в памяти приложения.")]
			public static partial void RebuildSuccessInfo(ILogger logger);

			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "Ошибка при разборе главного файла узлов '{FilePath}'. Этот файл был проигнорирован.")]
			public static partial void NodesFileParsingError(ILogger logger, string filePath, Exception ex);

			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "Ошибка при разборе файла страниц '{FilePath}'. Карта страниц этого узла была проигнорирована.")]
			public static partial void PagesFileParsingError(ILogger logger, string filePath, Exception ex);
		}

	}

}
