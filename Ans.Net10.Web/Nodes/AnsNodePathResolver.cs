using Ans.Net10.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Ans.Net10.Web.Nodes
{

	/// <summary>
	/// Определяет контракт сервиса для инициализации карты физических представлений 
	/// и асинхронного сопоставления входящих URL-запросов с узлами и страницами CMS.
	/// </summary>
	public interface INodePathResolver
	{
		/// <summary>
		/// Асинхронно сканирует базовый каталог представлений, формируя 
		/// первоначальную карту путей в оперативной памяти приложения.
		/// </summary>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, представляющая процесс асинхронной инициализации.</returns>
		Task InitializeAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Асинхронно анализирует входящий относительный URL-путь запроса и, на основе матрицы 
		/// приоритетов фреймворка, сопоставляет его с конкретным Узлом и Страницей.
		/// </summary>
		/// <param name="rawPath">Сырой относительный путь запроса, полученный из URL (например, "dep1/about/contacts").</param>
		/// <returns>
		/// Результат сопоставления в виде объекта <see cref="NodePageResolveResult"/>, 
		/// либо <see langword="null"/>, если совпадений в карте физических представлений не найдено.
		/// </returns>
		Task<NodePageResolveResult?> ResolveAsync(string? rawPath);
	}



	/// <summary>
	/// Реализация сервиса анализа путей, обеспечивающая асинхронную индексацию структуры 
	/// каталогов Razor-представлений в оперативной памяти и реактивное отслеживание изменений.
	/// </summary>
	public sealed partial class AnsNodePathResolver
		: INodePathResolver,
		IDisposable
	{

		private const string _BASE_NODES_DIR = "Views/Nodes";
		private const string _VIEW_EXTENSION = ".cshtml";
		private const string _START_PAGE_NAME = "start";
		private const string _MAIN_NODE_NAME = "_main";

		private readonly IWebHostEnvironment _env;
		private readonly ILogger<AnsNodePathResolver> _logger;
		private readonly ConcurrentDictionary<string, byte> _viewFilesMap;
		private FileSystemWatcher? _watcher;
		private string _physicalRootPath;
		private bool _isInitialized;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AnsNodePathResolver"/> 
		/// с внедрением необходимых инфраструктурных зависимостей платформы.
		/// </summary>
		/// <param name="env">Среда веб-хостинга для вычисления корневых путей приложения.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если один из входящих параметров равен <see langword="null"/>.
		/// </exception>
		public AnsNodePathResolver(
			IWebHostEnvironment env,
			ILogger<AnsNodePathResolver> logger)
		{
			_env = env ?? throw new ArgumentNullException(nameof(env));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_viewFilesMap = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
			_physicalRootPath = string.Empty;
		}


		/* methods */


		/// <inheritdoc />
		public async Task InitializeAsync(
			CancellationToken cancellationToken = default)
		{
			if (_isInitialized)
				return;

			_physicalRootPath = Path.Combine(_env.ContentRootPath, _BASE_NODES_DIR)
				.Replace(Path.DirectorySeparatorChar, '/');

			if (!Directory.Exists(_physicalRootPath))
			{
				_log.BaseDirectoryNotFound(_logger, _physicalRootPath);
				_isInitialized = true;
				return;
			}

			_log.StartingIndexing(_logger, _physicalRootPath);

			var enumerationOptions1 = new EnumerationOptions
			{
				RecurseSubdirectories = true,
				MatchCasing = MatchCasing.CaseInsensitive,
				AttributesToSkip = FileAttributes.System | FileAttributes.Hidden
			};

			await Task.Run(() =>
			{
				foreach (var filePath1 in Directory.EnumerateFiles(
					_physicalRootPath, $"*{_VIEW_EXTENSION}", enumerationOptions1))
				{
					cancellationToken.ThrowIfCancellationRequested();
					_registerFileInMap(filePath1);
				}
			}, cancellationToken);

			_setupFileSystemWatcher();
			_isInitialized = true;
		}


		/// <inheritdoc />
		public void Dispose()
		{
			if (_watcher != null)
			{
				_watcher.EnableRaisingEvents = false;
				_watcher.Dispose();
			}
		}


		/* functions */


		/// <inheritdoc />
		public Task<NodePageResolveResult?> ResolveAsync(
			string? rawPath)
		{
			if (!_isInitialized)
				throw new InvalidOperationException(
					"[Ans.Net10.Web.Nodes] Сервис анализа путей не был инициализирован. Вызовите InitializeAsync() перед использованием.");

			string cleanPath1 = (rawPath ?? string.Empty).CollapseSpaces();

			// ШАГ 1: Если пришел пустой запрос (корень сайта)
			if (string.IsNullOrEmpty(cleanPath1))
			{
				string defaultView1 = $"{_MAIN_NODE_NAME}/{_START_PAGE_NAME}{_VIEW_EXTENSION}";
				if (_viewFilesMap.ContainsKey(defaultView1))
					return Task.FromResult<NodePageResolveResult?>(
						new NodePageResolveResult(
							_MAIN_NODE_NAME,
							$"~/{_BASE_NODES_DIR}/{defaultView1}",
							_START_PAGE_NAME,
							true));

				return Task.FromResult<NodePageResolveResult?>(null);
			}

			var pathSpan1 = cleanPath1.AsSpan().Trim('/');
			int firstSlashIndex1 = pathSpan1.IndexOf('/');

			// ШАГ 2: Если запрос состоит из одного имени (нет слэшей в пути)
			if (firstSlashIndex1 == -1)
			{
				string name1 = pathSpan1.ToString();

				// Приоритет 2.1: Проверяем кастомный узел (name1/start.cshtml)
				string option1 = $"{name1}/{_START_PAGE_NAME}{_VIEW_EXTENSION}";
				if (_viewFilesMap.ContainsKey(option1))
					return Task.FromResult<NodePageResolveResult?>(
						new NodePageResolveResult(
							name1,
							$"~/{_BASE_NODES_DIR}/{option1}",
							_START_PAGE_NAME,
							true));

				// Приоритет 2.2: Проверяем страницу в _main (_main/name1.cshtml)
				string option2 = $"{_MAIN_NODE_NAME}/{name1}{_VIEW_EXTENSION}";
				if (_viewFilesMap.ContainsKey(option2))
					return Task.FromResult<NodePageResolveResult?>(
						new NodePageResolveResult(
							_MAIN_NODE_NAME,
							$"~/{_BASE_NODES_DIR}/{option2}",
							name1,
							false));

				// Приоритет 2.3: Проверяем подраздел в _main (_main/name1/start.cshtml)
				string option3 = $"{_MAIN_NODE_NAME}/{name1}/{_START_PAGE_NAME}{_VIEW_EXTENSION}";
				if (_viewFilesMap.ContainsKey(option3))
					return Task.FromResult<NodePageResolveResult?>(
						new NodePageResolveResult(
							_MAIN_NODE_NAME,
							$"~/{_BASE_NODES_DIR}/{option3}",
							$"{name1}/{_START_PAGE_NAME}",
							true));

				return Task.FromResult<NodePageResolveResult?>(null);
			}

			// ШАГ 3: Если запрос — это многосегментный путь (содержит слэши)
			string nodeSegment1 = pathSpan1[..firstSlashIndex1].ToString();
			string pageSegment1 = pathSpan1[(firstSlashIndex1 + 1)..].ToString();

			// Приоритет 3.1: Страница кастомного узела (name1/name2.cshtml)
			string pathOption1 = $"{nodeSegment1}/{pageSegment1}{_VIEW_EXTENSION}";
			if (_viewFilesMap.ContainsKey(pathOption1))
				return Task.FromResult<NodePageResolveResult?>(
					new NodePageResolveResult(
						nodeSegment1,
						$"~/{_BASE_NODES_DIR}/{pathOption1}",
						pageSegment1,
						false));

			// Приоритет 3.2: Раздел кастомного узела (name1/name2/start.cshtml)
			string pathOption2 = $"{nodeSegment1}/{pageSegment1}/{_START_PAGE_NAME}{_VIEW_EXTENSION}";
			if (_viewFilesMap.ContainsKey(pathOption2))
				return Task.FromResult<NodePageResolveResult?>(
					new NodePageResolveResult(
						nodeSegment1,
						$"~/{_BASE_NODES_DIR}/{pathOption2}",
						$"{pageSegment1}/{_START_PAGE_NAME}",
						true));

			// Приоритет 3.3: Страница узла _main (_main/name1/name2.cshtml)
			string pathOption3 = $"{_MAIN_NODE_NAME}/{cleanPath1}{_VIEW_EXTENSION}";
			if (_viewFilesMap.ContainsKey(pathOption3))
				return Task.FromResult<NodePageResolveResult?>(
					new NodePageResolveResult(
						_MAIN_NODE_NAME,
						$"~/{_BASE_NODES_DIR}/{pathOption3}",
						cleanPath1,
						false));

			// Приоритет 3.4: Раздел узла _main (_main/name1/name2/start.cshtml)
			string pathOption4 = $"{_MAIN_NODE_NAME}/{cleanPath1}/{_START_PAGE_NAME}{_VIEW_EXTENSION}";
			if (_viewFilesMap.ContainsKey(pathOption4))
				return Task.FromResult<NodePageResolveResult?>(
					new NodePageResolveResult(
						_MAIN_NODE_NAME,
						$"~/{_BASE_NODES_DIR}/{pathOption4}",
						$"{cleanPath1}/{_START_PAGE_NAME}",
						true));

			return Task.FromResult<NodePageResolveResult?>(null);
		}


		/* private methods */


		private void _registerFileInMap(
			string physicalFilePath)
		{
			string relativePath1 = Path.GetRelativePath(_physicalRootPath, physicalFilePath)
				.Replace(Path.DirectorySeparatorChar, '/');
			_viewFilesMap.TryAdd(relativePath1, 0);
		}


		private void _removeFileFromMap(
			string physicalFilePath)
		{
			string relativePath1 = Path.GetRelativePath(_physicalRootPath, physicalFilePath)
				.Replace(Path.DirectorySeparatorChar, '/');
			_viewFilesMap.TryRemove(relativePath1, out _);
		}


		private void _setupFileSystemWatcher()
		{
			_watcher = new FileSystemWatcher(_physicalRootPath, $"*{_VIEW_EXTENSION}")
			{
				IncludeSubdirectories = true,
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size
			};
			_watcher.Created += (s, e) =>
			{
				if (e.FullPath != null)
				{
					_registerFileInMap(e.FullPath);
					_log.FileAddedToMap(_logger, e.Name);
				}
			};
			_watcher.Deleted += (s, e) =>
			{
				if (e.FullPath != null)
				{
					_removeFileFromMap(e.FullPath);
					_log.FileRemovedFromMap(_logger, e.Name);
				}
			};
			_watcher.Renamed += (s, e) =>
			{
				if (e.OldFullPath != null && e.FullPath != null)
				{
					_removeFileFromMap(e.OldFullPath);
					_registerFileInMap(e.FullPath);
					_log.FileRenamedInMap(_logger, e.OldName, e.Name);
				}
			};
			_watcher.EnableRaisingEvents = true;
		}


		private static partial class _log
		{

			[LoggerMessage(
				EventId = 1,
				Level = LogLevel.Warning,
				Message = "[Nodes] Базовый каталог представлений узлов не найден по пути: {Path}")]
			public static partial void BaseDirectoryNotFound(
				ILogger logger,
				string path);

			[LoggerMessage(
				EventId = 2,
				Level = LogLevel.Information,
				Message = "[Nodes] Запуск асинхронной индексации представлений CMS из: {Path}")]
			public static partial void StartingIndexing(
				ILogger logger,
				string path);

			[LoggerMessage(
				EventId = 3,
				Level = LogLevel.Debug,
				Message = "[Nodes] Файл добавлен в карту CMS: {File}")]
			public static partial void FileAddedToMap(
				ILogger logger,
				string? file);

			[LoggerMessage(
				EventId = 4,
				Level = LogLevel.Debug,
				Message = "[Nodes] Файл удален из карты CMS: {File}")]
			public static partial void FileRemovedFromMap(
				ILogger logger,
				string? file);

			[LoggerMessage(
				EventId = 5,
				Level = LogLevel.Debug,
				Message = "[Nodes] Файл переименован в карте CMS: {Old} -> {New}")]
			public static partial void FileRenamedInMap(
				ILogger logger,
				string? old,
				string? @new);

		}

	}

}