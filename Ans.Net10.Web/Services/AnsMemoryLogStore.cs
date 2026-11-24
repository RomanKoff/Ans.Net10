// rev 2026-11-23

using Serilog.Core;
using Serilog.Events;

namespace Ans.Net10.Web.Services
{

    /// <summary>
    /// Представляет отдельную запись журнала событий в кольцевом буфере памяти.
    /// </summary>
    /// <param name="Id">Уникальный последовательный порядковый номер записи с момента старта приложения.</param>
    /// <param name="Timestamp">Метка времени события с учетом часового пояса.</param>
    /// <param name="Level">Уровень важности события Serilog.</param>
    /// <param name="Message">Отформатированный текст сообщения.</param>
    /// <param name="Exception">Текстовое представление исключения и стека вызовов, либо <see langword="null"/>.</param>
    /// <param name="SourceContext">Имя категории/класса-источника события, либо <see langword="null"/>.</param>
    public sealed record AnsLogEntry(
        long Id,
        DateTimeOffset Timestamp,
        LogEventLevel Level,
        string Message,
        string? Exception,
        string? SourceContext)
    {
        /// <summary>
        /// Возвращает трехбуквенный код уровня логирования (например, <c>"INF"</c>, <c>"WRN"</c>, <c>"ERR"</c>).
        /// </summary>
        public string LevelShortName => Level switch
        {
            LogEventLevel.Verbose => "VRB",
            LogEventLevel.Debug => "DBG",
            LogEventLevel.Information => "INF",
            LogEventLevel.Warning => "WRN",
            LogEventLevel.Error => "ERR",
            LogEventLevel.Fatal => "FTL",
            _ => "UNK"
        };
    }



    /// <summary>
    /// Потокобезопасный кольцевой буфер в памяти для хранения последних записей системного журнала
    /// и их последующего отображения в веб-интерфейсе консоли.
    /// </summary>
    public sealed class AnsMemoryLogStore
    {

        private readonly Lock _syncLock = new();
        private readonly Queue<AnsLogEntry> _entries;
        private readonly int _capacity;
        private long _sequence;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsMemoryLogStore"/> с указанной максимальной емкостью.
        /// </summary>
        /// <param name="capacity">Максимальное количество хранимых записей в буфере (минимум 10).</param>
        public AnsMemoryLogStore(
            int capacity = 1000)
        {
            _capacity = Math.Max(10, capacity);
            _entries = new Queue<AnsLogEntry>(_capacity);
        }


        /* readonly properties */


        /// <summary>
        /// Возвращает текущее количество записей, находящихся в буфере.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncLock)
                    return _entries.Count;
            }
        }


        /// <summary>
        /// Возвращает максимальную емкость кольцевого буфера.
        /// </summary>
        public int Capacity
            => _capacity;


        /* functions */


        /// <summary>
        /// Возвращает снимок записей из буфера с поддержкой инкрементальной выборки и фильтрации по уровню.
        /// </summary>
        /// <param name="afterId">Возвращать только записи с идентификатором строго больше указанного (для фонового обновления).</param>
        /// <param name="minLevel">Опциональный минимальный уровень важности для фильтрации.</param>
        /// <returns>Массив записей журнала <see cref="AnsLogEntry"/> в хронологическом порядке.</returns>
        public AnsLogEntry[] GetEntries(
            long afterId = 0,
            LogEventLevel? minLevel = null)
        {
            lock (_syncLock)
            {
                if (_entries.Count == 0)
                    return [];

                IEnumerable<AnsLogEntry> query1 = _entries;

                if (afterId > 0)
                    query1 = query1.Where(x => x.Id > afterId);

                if (minLevel is not null)
                    query1 = query1.Where(x => x.Level >= minLevel.Value);

                return [.. query1];
            }
        }


        /* methods */


        /// <summary>
        /// Добавляет новую запись в кольцевой буфер, вытесняя самую старую при достижении лимита емкости.
        /// </summary>
        /// <param name="timestamp">Время возникновения события.</param>
        /// <param name="level">Уровень важности события.</param>
        /// <param name="message">Текст сообщения.</param>
        /// <param name="exception">Текст исключения (если есть).</param>
        /// <param name="sourceContext">Категория источника (если есть).</param>
        public void Add(
            DateTimeOffset timestamp,
            LogEventLevel level,
            string message,
            string? exception = null,
            string? sourceContext = null)
        {
            lock (_syncLock)
            {
                if (_entries.Count >= _capacity)
                    _entries.Dequeue();

                var id1 = ++_sequence;
                _entries.Enqueue(new AnsLogEntry(
                    id1,
                    timestamp,
                    level,
                    message,
                    exception,
                    sourceContext));
            }
        }


        /// <summary>
        /// Полностью очищает все накопленные записи в буфере памяти.
        /// </summary>
        public void Clear()
        {
            lock (_syncLock)
            {
                _entries.Clear();
            }
        }

    }



    /// <summary>
    /// Приемник событий Serilog (<see cref="ILogEventSink"/>), транслирующий сообщения в <see cref="AnsMemoryLogStore"/>.
    /// </summary>
    public sealed class AnsMemoryLogSink
        : ILogEventSink
    {

        private readonly AnsMemoryLogStore _store;
        private readonly string[] _excludePaths;
        private readonly IFormatProvider? _formatProvider;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsMemoryLogSink"/>.
        /// </summary>
        /// <param name="store">Экземпляр кольцевого буфера памяти.</param>
        /// <param name="excludePaths">Опциональный список префиксов путей запросов, исключаемых из буфера (для уровней ниже <see cref="LogEventLevel.Warning"/>).</param>
        /// <param name="formatProvider">Опциональный провайдер форматирования сообщений.</param>
        public AnsMemoryLogSink(
            AnsMemoryLogStore store,
            string[]? excludePaths = null,
            IFormatProvider? formatProvider = null)
        {
            ArgumentNullException.ThrowIfNull(store);
            _store = store;
            _excludePaths = excludePaths ?? [];
            _formatProvider = formatProvider;
        }


        /* methods */


        /// <inheritdoc />
        public void Emit(
            LogEvent logEvent)
        {
            ArgumentNullException.ThrowIfNull(logEvent);

            if (logEvent.Level < LogEventLevel.Warning
                && _excludePaths.Length > 0
                && logEvent.Properties.TryGetValue("RequestPath", out var reqPathProp1)
                && reqPathProp1 is ScalarValue { Value: string reqPath1 })
            {
                foreach (var prefix1 in _excludePaths)
                    if (!string.IsNullOrEmpty(prefix1)
                        && reqPath1.StartsWith(prefix1, StringComparison.OrdinalIgnoreCase))
                        return;
            }

            string? sourceContext1 = null;
            if (logEvent.Properties.TryGetValue("SourceContext", out var propValue1)
                && propValue1 is ScalarValue { Value: string strVal1 })
                sourceContext1 = strVal1;

            _store.Add(
                logEvent.Timestamp,
                logEvent.Level,
                logEvent.RenderMessage(_formatProvider),
                logEvent.Exception?.ToString(),
                sourceContext1);
        }

    }

}
