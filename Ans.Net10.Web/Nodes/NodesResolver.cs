using Ans.Net10.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;

namespace Ans.Net10.Web.Nodes
{

	/// <summary>
	/// Представляет детерминированный результат разрешения виртуального URL-пути запроса 
	/// в физическое Razor-представление внутри контентной структуры CMS.
	/// </summary>
	/// <param name="IsMatched">Возвращает значение, указывающее, было ли найдено подходящее представление.</param>
	/// <param name="NodeName">Имя (идентификатор) определенного информационного узла.</param>
	/// <param name="PagePath">Логический относительный путь к странице внутри узла.</param>
	/// <param name="ViewPath">Полный канонический виртуальный путь к файлу представления, готовый для Razor Engine (например, ~/Views/Nodes/...).</param>
	/// <param name="IsStartPage">Возвращает значение, указывающее, является ли страница индексной (start.cshtml).</param>
	public readonly record struct NodesResolveResult(
		bool IsMatched,
		string? NodeName,
		string? PagePath,
		string? ViewPath,
		bool IsStartPage);



	/// <summary>
	/// Высокопроизводительный реактивный сервис анализа и разрешения ЧПУ-маршрутов CMS, 
	/// обеспечивающий мгновенное сопоставление запросов с картой Razor-представлений в памяти.
	/// </summary>
	public sealed partial class NodesResolver
		: IDisposable
	{

		private readonly IWebHostEnvironment _env;
		private readonly ILogger<NodesResolver> _logger;
		private readonly string _physicalRootPath;
		private readonly FileSystemWatcher _watcher1;
		private readonly Lock _lock1 = new();

		private FrozenDictionary<string, string> _viewFilesMap;
		private CancellationTokenSource? _debounceCts1;
		private bool _isInitialized;


		/* consts */


		/// <summary>
		/// Базовый виртуальный каталог размещения контента CMS.
		/// </summary>
		public const string BASE_NODES_DIR = "Views/Nodes";

		/// <summary>
		/// Расширение файлов представлений Razor.
		/// </summary>
		public const string VIEW_EXTENSION = ".cshtml";

		/// <summary>
		/// Маска фильтрации для поиска файлов представлений.
		/// </summary>
		public const string VIEW_FILTER = $"*{VIEW_EXTENSION}";

		/// <summary>
		/// Имя индексной (стартовой) страницы по умолчанию.
		/// </summary>
		public const string START_PAGE_NAME = "start";

		/// <summary>
		/// Системное имя главного контентного узла по умолчанию.
		/// </summary>
		public const string MAIN_NODE_NAME = "_main";


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodesResolver"/>.
		/// </summary>
		/// <param name="env">Среда выполнения веб-приложения.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если один из входящих параметров равен <see langword="null"/>.</exception>
		public NodesResolver(
			IWebHostEnvironment env,
			ILogger<NodesResolver> logger)
		{
			_env = env ?? throw new ArgumentNullException(nameof(env));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_physicalRootPath = Path.Combine(_env.ContentRootPath, BASE_NODES_DIR);
			_viewFilesMap = FrozenDictionary<string, string>.Empty;
			_watcher1 = new FileSystemWatcher(_physicalRootPath)
			{
				Filter = VIEW_FILTER,
				IncludeSubdirectories = true,
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size
			};
			_watcher1.Created += _onMapFileChanged;
			_watcher1.Deleted += _onMapFileChanged;
			_watcher1.Renamed += _onMapFileRenamed;
			_watcher1.Error += _onWatcherError;
			_watcher1.EnableRaisingEvents = true;
		}


		/* properties */


		/// <summary>
		/// Возвращает текущую, полностью скомпилированную карту соответствия путей в памяти.
		/// </summary>
		public FrozenDictionary<string, string> ViewFilesMap => _viewFilesMap;


		/* methods */


		/// <summary>
		/// Выполняет первоначальное асинхронное сканирование файлов контента, строит карту 
		/// представлений в памяти и активирует систему реактивного отслеживания изменений.
		/// </summary>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, представляющая операцию инициализации сервиса.</returns>
		public async Task InitializeAsync(
			CancellationToken cancellationToken = default)
		{
			if (_isInitialized)
				return;
			using (_lock1.EnterScope())
			{
				if (_isInitialized)
					return;
				if (!Directory.Exists(_physicalRootPath))
				{
					_log.BaseDirectoryNotFound(_logger, _physicalRootPath);
					_isInitialized = true;
					return;
				}
			}
			_log.StartingIndexing(_logger, _physicalRootPath);
			await _rebuildCacheMapAsync(cancellationToken);
			_isInitialized = true;
		}


		/// <inheritdoc />
		public void Dispose()
		{
			_watcher1.EnableRaisingEvents = false;
			_watcher1.Created -= _onMapFileChanged;
			_watcher1.Deleted -= _onMapFileChanged;
			_watcher1.Renamed -= _onMapFileRenamed;
			_watcher1.Error -= _onWatcherError;
			_watcher1.Dispose();
			using (_lock1.EnterScope())
			{
				_debounceCts1?.Cancel();
				_debounceCts1?.Dispose();
			}
		}


		/* functions */


		/// <summary>
		/// Выполняет мгновенное и детерминированное разрешение виртуального пути запроса 
		/// в физическое представление на основе матрицы приоритетов в памяти приложения.
		/// </summary>
		/// <param name="rawPath">Сырой относительный URL-путь входящего HTTP-запроса.</param>
		/// <returns>Структура <see cref="NodesResolveResult"/>, содержащая результаты анализа пути.</returns>
		/// <exception cref="InvalidOperationException">Вызывается, если метод вызван до инициализации сервиса.</exception>
		public NodesResolveResult Resolve(
			string? rawPath)
		{
			if (!_isInitialized)
				throw new InvalidOperationException(
					"[Ans.Net10.Web.Nodes] Сервис анализа путей не был инициализирован. Вызовите InitializeAsync() перед использованием.");

			string cleanPath1 = (rawPath ?? string.Empty).CollapseSpaces();
			var pathSpan1 = cleanPath1.AsSpan().Trim('/');

			// МАТРИЦА ПРИОРИТЕТОВ 1: Корень сайта (Пустой запрос)
			if (pathSpan1.IsEmpty)
			{
				string defaultKey1 = $"{MAIN_NODE_NAME}/{START_PAGE_NAME}{VIEW_EXTENSION}";
				if (_viewFilesMap.TryGetValue(defaultKey1, out var physicalView1))
				{
					return new NodesResolveResult(
						IsMatched: true,
						NodeName: MAIN_NODE_NAME,
						PagePath: START_PAGE_NAME,
						ViewPath: $"~/{BASE_NODES_DIR}/{physicalView1}",
						IsStartPage: true);
				}
				return default;
			}

			string searchPath1 = cleanPath1.ToLowerInvariant();
			var searchSpan1 = searchPath1.AsSpan().Trim('/');
			int firstSlashIndex1 = searchSpan1.IndexOf('/');

			// МАТРИЦА ПРИОРИТЕТОВ 2: Односегментный запрос (name1)
			if (firstSlashIndex1 == -1)
			{
				string name1 = searchSpan1.ToString();

				// Приоритет 2.1 (Кастомный Узел): Проверяем name1/start.cshtml
				string option21 = $"{name1}/{START_PAGE_NAME}{VIEW_EXTENSION}";
				if (_viewFilesMap.TryGetValue(option21, out var physicalView21))
				{
					return new NodesResolveResult(
						IsMatched: true,
						NodeName: name1,
						PagePath: START_PAGE_NAME,
						ViewPath: $"~/{BASE_NODES_DIR}/{physicalView21}",
						IsStartPage: true);
				}

				// Приоритет 2.2 (Страница в _main): Проверяем _main/name1.cshtml
				string option22 = $"{MAIN_NODE_NAME}/{name1}{VIEW_EXTENSION}";
				if (_viewFilesMap.TryGetValue(option22, out var physicalView22))
				{
					return new NodesResolveResult(
						IsMatched: true,
						NodeName: MAIN_NODE_NAME,
						PagePath: name1,
						ViewPath: $"~/{BASE_NODES_DIR}/{physicalView22}",
						IsStartPage: false);
				}

				// Приоритет 2.3 (Раздел в _main): Проверяем _main/name1/start.cshtml
				string option23 = $"{MAIN_NODE_NAME}/{name1}/{START_PAGE_NAME}{VIEW_EXTENSION}";
				if (_viewFilesMap.TryGetValue(option23, out var physicalView23))
				{
					return new NodesResolveResult(
						IsMatched: true,
						NodeName: MAIN_NODE_NAME,
						PagePath: $"{name1}/{START_PAGE_NAME}",
						ViewPath: $"~/{BASE_NODES_DIR}/{physicalView23}",
						IsStartPage: true);
				}

				return default;
			}

			// МАТРИЦА ПРИОРИТЕТОВ 3: Многосегментный запрос (name1/name2/...+)
			string nodeSegment1 = searchSpan1[..firstSlashIndex1].ToString();
			string pageSegment1 = searchSpan1[(firstSlashIndex1 + 1)..].ToString();
			string fullSearchPath1 = searchSpan1.ToString();

			// Приоритет 3.1 (Страница кастомного узла): Проверяем name1/name2.cshtml
			string option31 = $"{nodeSegment1}/{pageSegment1}{VIEW_EXTENSION}";
			if (_viewFilesMap.TryGetValue(option31, out var physicalView31))
			{
				return new NodesResolveResult(
					IsMatched: true,
					NodeName: nodeSegment1,
					PagePath: pageSegment1,
					ViewPath: $"~/{BASE_NODES_DIR}/{physicalView31}",
					IsStartPage: false);
			}

			// Приоритет 3.2 (Раздел кастомного узла): Проверяем name1/name2/start.cshtml
			string option32 = $"{nodeSegment1}/{pageSegment1}/{START_PAGE_NAME}{VIEW_EXTENSION}";
			if (_viewFilesMap.TryGetValue(option32, out var physicalView32))
			{
				return new NodesResolveResult(
					IsMatched: true,
					NodeName: nodeSegment1,
					PagePath: $"{pageSegment1}/{START_PAGE_NAME}",
					ViewPath: $"~/{BASE_NODES_DIR}/{physicalView32}",
					IsStartPage: true);
			}

			// Приоритет 3.3 (Страница узла _main): Проверяем _main/name1/name2.cshtml
			string option33 = $"{MAIN_NODE_NAME}/{fullSearchPath1}{VIEW_EXTENSION}";
			if (_viewFilesMap.TryGetValue(option33, out var physicalView33))
			{
				return new NodesResolveResult(
					IsMatched: true,
					NodeName: MAIN_NODE_NAME,
					PagePath: fullSearchPath1,
					ViewPath: $"~/{BASE_NODES_DIR}/{physicalView33}",
					IsStartPage: false);
			}

			// Приоритет 3.4 (Раздел узла _main): Проверяем _main/name1/name2/start.cshtml
			string option34 = $"{MAIN_NODE_NAME}/{fullSearchPath1}/{START_PAGE_NAME}{VIEW_EXTENSION}";
			if (_viewFilesMap.TryGetValue(option34, out var physicalView34))
			{
				return new NodesResolveResult(
					IsMatched: true,
					NodeName: MAIN_NODE_NAME,
					PagePath: $"{fullSearchPath1}/{START_PAGE_NAME}",
					ViewPath: $"~/{BASE_NODES_DIR}/{physicalView34}",
					IsStartPage: true);
			}

			return default;
		}


		/* privates */


		private async Task _rebuildCacheMapAsync(
			CancellationToken cancellationToken)
		{
			var enumerationOptions1 = new EnumerationOptions
			{
				RecurseSubdirectories = true,
				MatchCasing = MatchCasing.CaseInsensitive,
				AttributesToSkip = FileAttributes.System | FileAttributes.Hidden
			};
			var tempDict1 = new Dictionary<string, string>(StringComparer.Ordinal);
			await Task.Run(() =>
			{
				foreach (var filePath1 in Directory.EnumerateFiles(
					_physicalRootPath, VIEW_FILTER, enumerationOptions1))
				{
					cancellationToken.ThrowIfCancellationRequested();
					string relativePath1 = Path.GetRelativePath(_physicalRootPath, filePath1)
						.Replace(Path.DirectorySeparatorChar, '/');
					tempDict1[relativePath1.ToLowerInvariant()] = relativePath1;
				}
			}, cancellationToken);
			Interlocked.Exchange(ref _viewFilesMap, tempDict1.ToFrozenDictionary(StringComparer.Ordinal));
		}


		private void _onMapFileChanged(
			object sender,
			FileSystemEventArgs e)
		{
			_log.FileChangedInfo(_logger, e.Name, e.ChangeType);
			_scheduleRebuild();
		}


		private void _onMapFileRenamed(
			object sender,
			RenamedEventArgs e)
		{
			_log.FileRenamedInfo(_logger, e.OldName, e.Name);
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
			lock (_lock1)
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
					_log.RebuildStartedInfo(_logger);
					await _rebuildCacheMapAsync(token1);
					_log.RebuildSuccessInfo(_logger);
				}
				catch (OperationCanceledException)
				{
					// Штатное поведение при поступлении новых файловых событий (дебаунс)
				}
				catch (Exception ex1)
				{
					_log.RebuildError(_logger, ex1);
				}
			}, CancellationToken.None);
		}


		/* logs */


		private static partial class _log
		{
			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "[Nodes] Базовый каталог представлений узлов не найден по пути: {Path}")]
			public static partial void BaseDirectoryNotFound(ILogger logger, string path);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[Nodes] Запуск асинхронной индексации представлений CMS из: {Path}")]
			public static partial void StartingIndexing(ILogger logger, string path);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[Nodes] Файловая система зафиксировала изменения представлений: {FileName} ({ChangeType}). Запуск таймера обновления...")]
			public static partial void FileChangedInfo(ILogger logger, string? fileName, WatcherChangeTypes changeType);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[Nodes] Файл представлений переименован: {OldName} -> {NewName}. Запуск таймера обновления...")]
			public static partial void FileRenamedInfo(ILogger logger, string? oldName, string? newName);

			[LoggerMessage(
				Level = LogLevel.Error,
				Message = "[Nodes] Зафиксирован сбой в работе FileSystemWatcher в модуле резолвера контента.")]
			public static partial void WatcherError(ILogger logger, Exception ex);

			[LoggerMessage(
				Level = LogLevel.Error,
				Message = "[Nodes] Ошибка при автоматическом фоновом пересчете плоской карты контента CMS.")]
			public static partial void RebuildError(ILogger logger, Exception ex);

			[LoggerMessage(
				Level = LogLevel.Debug,
				Message = "[Nodes] Начало фонового пересчета и построения Frozen-карты контента...")]
			public static partial void RebuildStartedInfo(ILogger logger);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[Nodes] Карта представлений CMS успешно обновлена в памяти и переиндексирована.")]
			public static partial void RebuildSuccessInfo(ILogger logger);
		}

	}

}
