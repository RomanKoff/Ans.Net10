// rev 2026-11-01

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Ans.Net10.Web.Nodes
{

	/// <summary>
	/// Фоновое инфраструктурное решение (Триггер), отслеживающее изменения в Views/Nodes
	/// и координирующее безопасное перестроение карты в NodesMapper с защитой от шторма событий.
	/// </summary>
	public sealed partial class NodesWatcherService
		: BackgroundService
	{

		private const int _debounceDelayMs = 800;
		private const int _timerIntervalMs = 200;
		private const int _maxRetryAttempts = 5;

		private readonly NodesMapper _mapper;
		private readonly ILogger<NodesWatcherService> _logger;
		private readonly Lock _syncLock = new();

		private FileSystemWatcher? _watcher;
		private long _lastEventTimestamp;
		private bool _hasUnprocessedEvents;

		private bool _isRefreshing;
		private bool _isPending;
		private int _retryCount;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodesWatcherService"/>.
		/// </summary>
		/// <param name="mapper">Исполнитель карты путей.</param>
		/// <param name="logger">Служба ведения системных логов.</param>
		public NodesWatcherService(
			NodesMapper mapper,
			ILogger<NodesWatcherService> logger)
		{
			ArgumentNullException.ThrowIfNull(mapper);
			ArgumentNullException.ThrowIfNull(logger);
			_mapper = mapper;
			_logger = logger;
		}


		/* methods */


		/// <inheritdoc />
		protected override async Task ExecuteAsync(
			CancellationToken stoppingToken)
		{
			// Первичный прогрев карты при старте приложения
			bool isSuccess1 = await _mapper.InitializeAsync(stoppingToken);
			if (!isSuccess1)
			{
				_log.LogInitialRefreshFailed(_logger);
			}

			if (!Directory.Exists(_mapper.NodesPath))
			{
				_log.LogWatchDirectoryNotFound(_logger, _mapper.NodesPath);
				return;
			}

			// Настройка кроссплатформенного FileSystemWatcher
			_watcher = new FileSystemWatcher(_mapper.NodesPath)
			{
				IncludeSubdirectories = true,
				NotifyFilter =
					NotifyFilters.FileName
					| NotifyFilters.DirectoryName
					| NotifyFilters.LastWrite
					| NotifyFilters.Size
			};

			_watcher.Created += _onFileSystemChanged;
			_watcher.Deleted += _onFileSystemChanged;
			_watcher.Changed += _onFileSystemChanged;
			_watcher.Renamed += _onFileSystemChanged;

			_watcher.EnableRaisingEvents = true;
			_log.LogWatcherStarted(_logger, _mapper.NodesPath);

			using var periodicTimer1 = new PeriodicTimer(TimeSpan.FromMilliseconds(_timerIntervalMs));

			try
			{
				while (await periodicTimer1.WaitForNextTickAsync(stoppingToken))
				{
					bool isShouldRefresh1 = false;

					using (_syncLock.EnterScope())
					{
						if (_hasUnprocessedEvents
							&& !_isRefreshing
							&& _retryCount < _maxRetryAttempts)
						{
							long elapsedMs1 = (Stopwatch.GetTimestamp() - _lastEventTimestamp) * 1000 / Stopwatch.Frequency;
							if (elapsedMs1 >= _debounceDelayMs)
							{
								isShouldRefresh1 = true;
								_hasUnprocessedEvents = false;
							}
						}
					}

					if (isShouldRefresh1)
					{
						_log.LogTriggeringMapper(_logger);
						await _processRefreshLoopAsync();
					}
				}
			}
			catch (OperationCanceledException)
			{
				// Нормальное завершение работы
			}
		}


		/// <inheritdoc />
		public override void Dispose()
		{
			if (_watcher is not null)
			{
				_watcher.EnableRaisingEvents = false;
				_watcher.Created -= _onFileSystemChanged;
				_watcher.Deleted -= _onFileSystemChanged;
				_watcher.Changed -= _onFileSystemChanged;
				_watcher.Renamed -= _onFileSystemChanged;
				_watcher.Dispose();
			}
			base.Dispose();
		}


		/* privates */


		private void _onFileSystemChanged(
			object sender,
			FileSystemEventArgs e)
		{
			_log.LogFileSystemEventDetected(_logger, e.ChangeType, e.Name ?? string.Empty);

			using (_syncLock.EnterScope())
			{
				if (_retryCount >= _maxRetryAttempts)
				{
					_log.LogEmergencyStopActive(_logger);
					return;
				}

				_lastEventTimestamp = Stopwatch.GetTimestamp();
				_hasUnprocessedEvents = true;

				if (_isRefreshing)
				{
					_isPending = true;
					_log.LogRefreshPending(_logger);
				}
			}
		}


		private async Task _processRefreshLoopAsync()
		{
			while (true)
			{
				// Изолированная синхронная установка флагов перед await
				_startRefresh();

				bool isSuccess1 = await _mapper.InitializeAsync(CancellationToken.None);

				// Изолированная синхронная обработка результатов после await
				bool isPending1 = _endRefresh(isSuccess1);

				if (!isPending1)
				{
					_log.LogRefreshLoopFinished(_logger);
					break;
				}
			}
		}


		private void _startRefresh()
		{
			using (_syncLock.EnterScope())
			{
				_isRefreshing = true;
				_isPending = false;
			}
		}


		private bool _endRefresh(
	bool isSuccess)
		{
			using (_syncLock.EnterScope())
			{
				_isRefreshing = false;

				if (isSuccess)
				{
					_retryCount = 0;
				}
				else
				{
					_retryCount++;
					_log.LogRetryAttemptFailed(_logger, _retryCount, _maxRetryAttempts);

					if (_retryCount >= _maxRetryAttempts)
					{
						_log.LogCriticalFailureAlarm(_logger, _maxRetryAttempts);
						// Прерываем цикл, так как мы вошли в аварийный стоп
						return false;
					}

					_isPending = true;

					// Запускаем ожидание в фоновом потоке, не блокируя текущий
					_ = Task.Delay(_debounceDelayMs * _retryCount).ContinueWith(_ =>
					{
						using (_syncLock.EnterScope())
						{
							// Принудительно взводим флаг для обработки на следующем тике таймера
							_hasUnprocessedEvents = true;
						}
					}, TaskScheduler.Default);
				}

				return _isPending;
			}
		}


		/* logs */


		private static partial class _log
		{
			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "[NodesWatcher] Не удалось выполнить первоначальный прогрев карты представлений.")]
			public static partial void LogInitialRefreshFailed(
				ILogger logger);

			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "[NodesWatcher] Каталог для мониторинга не существует: {Path}. Служба отключена.")]
			public static partial void LogWatchDirectoryNotFound(
				ILogger logger, string path);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[NodesWatcher] Мониторинг запущен для папки: {Path}")]
			public static partial void LogWatcherStarted(
				ILogger logger, string path);

			[LoggerMessage(
				Level = LogLevel.Debug,
				Message = "[NodesWatcher] Обнаружено событие ОС [{ChangeType}] для: {Name}")]
			public static partial void LogFileSystemEventDetected(
				ILogger logger, WatcherChangeTypes changeType, string name);

			[LoggerMessage(
				Level = LogLevel.Debug,
				Message = "[NodesWatcher] Маппер занят. Событие отложено во флаг Pending.")]
			public static partial void LogRefreshPending(
				ILogger logger);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[NodesWatcher] Затишье наступило. Передаем команду исполнителю.")]
			public static partial void LogTriggeringMapper(
				ILogger logger);

			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "[NodesWatcher] Попытка пересчета карты провалилась ({Current}/{Max}). Сдвигаем таймер.")]
			public static partial void LogRetryAttemptFailed(
				ILogger logger, int current, int max);

			[LoggerMessage(
				Level = LogLevel.Critical,
				Message = "КРИТИЧЕСКАЯ АВАРИЯ: Достигнут лимит из {Max} неудачных попыток обновления карты. Автоматический Self-Healing остановлен! Требуется вмешательство администратора.")]
			public static partial void LogCriticalFailureAlarm(
				ILogger logger, int max);

			[LoggerMessage(
				Level = LogLevel.Warning,
				Message = "[NodesWatcher] Запросы на изменение игнорируются, так как система находится в критической аварии.")]
			public static partial void LogEmergencyStopActive(
				ILogger logger);

			[LoggerMessage(
				Level = LogLevel.Information,
				Message = "[NodesWatcher] Конвейер актуализации успешно завершен, система стабильна.")]
			public static partial void LogRefreshLoopFinished(
				ILogger logger);
		}

	}

}
