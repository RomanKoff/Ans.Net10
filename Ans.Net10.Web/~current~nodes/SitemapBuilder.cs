// rev 2026-11-19

using Ans.Net10.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Строитель и контейнер графа навигации, обеспечивающий индексацию и быстрый
    /// регистронезависимый поиск узлов и страниц карты сайта в оперативной памяти.
    /// </summary>
    public sealed partial class SitemapBuilder
    {

        private readonly ILogger<SitemapBuilder> _logger;
        private readonly ISitemapContributor[] _contributors;
        private readonly List<NodemapItem> _topNodes = [];
        private readonly SemaphoreSlim _syncSemaphore = new(1, 1);


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SitemapBuilder"/> с вычислением базового пути к файлам карт.
        /// </summary>
        /// <param name="env">Окружение веб-приложения для определения корневого каталога.</param>
        /// <param name="options">Конфигурационные параметры навигации и путей к XML-файлам.</param>
        /// <param name="logger">Экземпляр логгера для записи событий сборки структуры.</param>
        /// <param name="contributors">Опциональная коллекция модульных поставщиков программных узлов карты сайта.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если один из обязательных параметров равен <see langword="null"/>.</exception>
        public SitemapBuilder(
            IWebHostEnvironment env,
            LibWebOptions options,
            ILogger<SitemapBuilder> logger,
            IEnumerable<ISitemapContributor>? contributors = null)
        {
            ArgumentNullException.ThrowIfNull(env);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;
            _contributors = contributors is not null
                ? [.. contributors]
                : [];
            SitemapPath = Path.Combine(env.ContentRootPath, SuppNodes.NODES_ROOT);
            TopNodes = _topNodes.AsReadOnly();
        }


        /* properties */


        /// <summary>
        /// Получает или задает делегат для программного дополнения карты сайта при каждой её инициализации.
        /// </summary>
        public Action<SitemapBuilder>? CustomBuilder { get; set; }


        /* readonly properties */


        /// <summary>
        /// Получает полный физический путь к корневому каталогу с XML-файлами карты сайта.
        /// </summary>
        public string SitemapPath { get; } = string.Empty;


        /// <summary>
        /// Получает коллекцию корневых информационных узлов карты сайта.
        /// </summary>
        /// <returns>Коллекция только для чтения, содержащая корневые узлы <see cref="NodemapItem"/>.</returns>
        public IReadOnlyCollection<NodemapItem> TopNodes { get; }


        /// <summary>
        /// Получает индексный словарь информационных узлов карты сайта.
        /// </summary>
        /// <returns>Оптимизированный для чтения замороженный словарь, сопоставляющий системные имена с объектами <see cref="NodemapItem"/>.</returns>
        public FrozenDictionary<string, NodemapItem> NodesIndex { get; private set; }
            = FrozenDictionary<string, NodemapItem>.Empty;


        /// <summary>
        /// Получает индексный словарь информационных страниц карты сайта.
        /// </summary>
        /// <returns>Оптимизированный для чтения замороженный словарь, сопоставляющий полные ЧПУ-маршруты с объектами <see cref="PagemapItem"/>.</returns>
        public FrozenDictionary<string, PagemapItem> PagesIndex { get; private set; }
            = FrozenDictionary<string, PagemapItem>.Empty;


        /* methods */


        /// <summary>
        /// Асинхронно загружает, парсит и полностью перестраивает структуру карты сайта из XML-файлов и программных правил.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Значение <see langword="true"/>, если перестроение графа и индексов завершилось успехом; иначе — <see langword="false"/>.</returns>
        public async ValueTask<bool> InitializeAsync(
            CancellationToken cancellationToken = default)
        {
            await _syncSemaphore
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
            try
            {
                _log.BuildStarted(_logger);

                var tempTopNodes1 = new List<NodemapItem>();
                bool hasXmlNodes1 = false;

                if (SitemapPath is { Length: > 0 })
                {
                    string mapnodesFilename1 = Path.Combine(SitemapPath, SuppNodes.MAPNODES_FILENAME);
                    if (File.Exists(mapnodesFilename1))
                    {
                        NodesXmlDto? nodesDto1 = null;
                        try
                        {
                            nodesDto1 = await _readXmlWithRetryAsync<NodesXmlDto>(
                                mapnodesFilename1, cancellationToken)
                                .ConfigureAwait(false);
                        }
                        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
                        {
                            _log.MainConfigDeserializationError(_logger, mapnodesFilename1, ex.Message, ex);
                        }

                        if (nodesDto1 is not null)
                        {
                            if (nodesDto1.Items is not null)
                            {
                                await _parseNodesXmlAsync(
                                    nodesDto1.Items, tempTopNodes1, cancellationToken)
                                    .ConfigureAwait(false);
                                hasXmlNodes1 = true;
                            }
                            else
                            {
                                _log.MainConfigEmptyOrInvalid(_logger, mapnodesFilename1);
                            }
                        }
                    }
                    else
                    {
                        _log.MainConfigNotFoundProgrammaticMode(_logger, mapnodesFilename1);
                    }
                }

                if (hasXmlNodes1 && !_containsNodeRecursive(tempTopNodes1, SuppNodes.INDEX_NODE_NAME))
                {
                    var mainNode1 = new NodemapItem(SuppNodes.INDEX_NODE_NAME, string.Empty);
                    await _loadNodePagesAsync(mainNode1, cancellationToken)
                        .ConfigureAwait(false);
                    tempTopNodes1.Insert(0, mainNode1);
                }

                _topNodes.Clear();
                _topNodes.AddRange(tempTopNodes1);
                _buildIndexes();

                if (_contributors.Length > 0 || CustomBuilder is not null)
                {
                    foreach (var contributor1 in _contributors)
                        contributor1.ConfigureSitemap(this);

                    CustomBuilder?.Invoke(this);
                    _buildIndexes();
                }

                _log.BuildCompleted(_logger, NodesIndex.Count, PagesIndex.Count);
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
            {
                _log.BuildFailed(_logger, ex.Message, ex);
                return false;
            }
            finally
            {
                _syncSemaphore.Release();
            }
        }


        /// <summary>
        /// Программно преобразует и добавляет корневой узел на основе его декларативного описания в конец коллекции.
        /// </summary>
        /// <param name="nodemapDef">Декларативное описание добавляемого узла.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="nodemapDef"/> равен <see langword="null"/>.</exception>
        public void AddTopNodeDef(
            NodemapDef nodemapDef)
        {
            ArgumentNullException.ThrowIfNull(nodemapDef);
            var node1 = new NodemapItem(nodemapDef);
            AddTopNode(node1);
        }


        /// <summary>
        /// Программно добавляет корневой информационный узел в карту сайта.
        /// </summary>
        /// <param name="node">Добавляемый информационный узел <see cref="NodemapItem"/>.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="node"/> равен <see langword="null"/>.</exception>
        public void AddTopNode(
            NodemapItem node)
        {
            ArgumentNullException.ThrowIfNull(node);
            _topNodes.Add(node);
        }


        /// <summary>
        /// Вставляет информационный узел в коллекцию корневых элементов по строго указанному порядковому индексу.
        /// </summary>
        /// <param name="index">Порядковый индекс позиции вставки (начиная с 0).</param>
        /// <param name="node">Вставляемый информационный узел.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="node"/> равен <see langword="null"/>.</exception>
        public void InsertTopNode(
            int index,
            NodemapItem node)
        {
            ArgumentNullException.ThrowIfNull(node);
            _topNodes.Insert(index, node);
        }


        /// <summary>
        /// Программно вставляет или дополняет дочерний узел в уже существующий родительский узел карты сайта.
        /// </summary>
        /// <param name="parentTarget">Идентификатор или путь целевого родительского узла.</param>
        /// <param name="childNode">Новый дочерний узел <see cref="NodemapItem"/>.</param>
        /// <returns>Значение <see langword="true"/>, если родительский узел найден и инъекция успешна; иначе — <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="childNode"/> равен <see langword="null"/>.</exception>
        public bool InjectChildNode(
            string parentTarget,
            NodemapItem childNode)
        {
            ArgumentNullException.ThrowIfNull(childNode);
            if (NodesIndex.TryGetValue(parentTarget, out var parentNode1))
            {
                if (childNode.Name is { Length: > 0 }
                    && NodesIndex.TryGetValue(childNode.Name, out var existingChild1))
                {
                    _mergeNodeRecursive(existingChild1, childNode);
                    return true;
                }
                parentNode1.AddChildNode(childNode);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Программно вставляет или дополняет контентную страницу в существующий информационный узел графа.
        /// </summary>
        /// <param name="nodeTarget">Идентификатор информационного узла, в который добавляется страница.</param>
        /// <param name="page">Новая страница контента <see cref="PagemapItem"/>.</param>
        /// <returns>Значение <see langword="true"/>, если целевой узел найден и страница добавлена; иначе — <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="page"/> равен <see langword="null"/>.</exception>
        public bool InjectPageToNode(
            string nodeTarget,
            PagemapItem page)
        {
            ArgumentNullException.ThrowIfNull(page);
            if (NodesIndex.TryGetValue(nodeTarget, out var node1))
            {
                if (page.Path is { Length: > 0 }
                    && PagesIndex.TryGetValue(page.Path, out var existingPage1))
                {
                    _mergePageRecursive(existingPage1, page);
                    return true;
                }
                node1.AddTopPage(page);
                return true;
            }
            return false;
        }


        /* functions */


        /// <summary>
        /// Возвращает информационный узел из индекса по его системному имени.
        /// </summary>
        /// <param name="nodeName">Системное имя искомого информационного узла.</param>
        /// <returns>Найденный экземпляр <see cref="NodemapItem"/> или <see langword="null"/>, если узел не найден или имя пусто.</returns>
        public NodemapItem? GetNode(
            string nodeName)
        {
            NodesIndex.TryGetValue(nodeName, out var node1);
            return node1;
        }


        /// <summary>
        /// Возвращает контентную страницу из индекса по её полному составному пути.
        /// </summary>
        /// <param name="combinedPath">Составной ЧПУ-путь страницы в формате "ИмяУзла/ПутьСтраницы".</param>
        /// <returns>Найденный экземпляр <see cref="PagemapItem"/> или <see langword="null"/>, если страница не найдена.</returns>
        public PagemapItem? GetPage(
            string combinedPath)
        {
            PagesIndex.TryGetValue(combinedPath, out var page1);
            return page1;
        }


        /* privates */


        private async Task _parseNodesXmlAsync(
            List<NodeItemXmlDto> dtos,
            List<NodemapItem> targetList,
            CancellationToken cancellationToken)
        {
            foreach (var dto1 in dtos)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var nodeItem1 = new NodemapItem(
                    dto1.Target, dto1.Face, dto1.Display, dto1.Passthroughs);
                if (dto1.ChildItems is { Count: > 0 })
                {
                    var tempChildren1 = new List<NodemapItem>();
                    await _parseNodesXmlAsync(
                        dto1.ChildItems, tempChildren1, cancellationToken)
                        .ConfigureAwait(false);
                    foreach (var child1 in tempChildren1)
                        nodeItem1.AddChildNode(child1);
                }
                if (nodeItem1.Type is NodemapTypesEnum.Node && nodeItem1.Name is { Length: > 0 })
                    await _loadNodePagesAsync(nodeItem1, cancellationToken)
                        .ConfigureAwait(false);
                targetList.Add(nodeItem1);
            }
        }


        private async Task _loadNodePagesAsync(
            NodemapItem nodeItem,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(SitemapPath) || nodeItem.Name is not { Length: > 0 })
                return;

            string mappagesFilename1 = Path.Combine(SitemapPath, nodeItem.Name, SuppNodes.MAPPAGES_FILENAME);
            if (!File.Exists(mappagesFilename1))
                return;

            try
            {
                var pagesDto1 = await _readXmlWithRetryAsync<PagesXmlDto>(
                    mappagesFilename1, cancellationToken)
                    .ConfigureAwait(false);

                if (pagesDto1?.Items is { Count: > 0 })
                    _parsePagesXml(pagesDto1.Items, nodeItem, null);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _log.PageConfigDeserializationError(_logger, mappagesFilename1, ex.Message, ex);
            }
        }


        private static void _parsePagesXml(
            List<PageItemXmlDto> dtos,
            NodemapItem nodeItem,
            PagemapItem? parentPage)
        {
            foreach (var dto1 in dtos)
            {
                var pageItem1 = new PagemapItem(
                    nodeItem, dto1.Target, dto1.Face, dto1.HasNoIndex, dto1.Display, dto1.Passthroughs);
                if (parentPage is null)
                    nodeItem.AddTopPage(pageItem1);
                else
                    parentPage.AddChildPage(pageItem1);
                if (dto1.ChildItems is { Count: > 0 })
                    _parsePagesXml(dto1.ChildItems, nodeItem, pageItem1);
            }
        }


        private void _buildIndexes()
        {
            var nodesDict1 = new Dictionary<string, NodemapItem>(StringComparer.OrdinalIgnoreCase);
            var pagesDict1 = new Dictionary<string, PagemapItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var node1 in _topNodes)
                _indexNodeRecursive(node1, nodesDict1, pagesDict1);
            NodesIndex = nodesDict1
                .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
            PagesIndex = pagesDict1
                .Where(x => x.Key is not { Length: 0 })
                .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        }


        private static void _indexNodeRecursive(
            NodemapItem node,
            Dictionary<string, NodemapItem> nodesDict,
            Dictionary<string, PagemapItem> pagesDict)
        {
            if (node.Name is { Length: > 0 })
                nodesDict[node.Name] = node;
            foreach (var page1 in node._AllTopPages)
                _indexPageRecursive(page1, pagesDict);
            foreach (var childNode1 in node.ChildNodes)
                _indexNodeRecursive(childNode1, nodesDict, pagesDict);
        }


        private static void _indexPageRecursive(
            PagemapItem page,
            Dictionary<string, PagemapItem> pagesDict)
        {
            if (page.Path is not null)
                pagesDict[page.Path] = page;
            foreach (var childPage1 in page.ChildPages)
                _indexPageRecursive(childPage1, pagesDict);
        }


        private static async Task<T?> _readXmlWithRetryAsync<T>(
            string filename,
            CancellationToken cancellationToken)
            where T : class
        {
            const int maxAttempts1 = 3;
            for (int attempt1 = 1; ; attempt1++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    return await SuppXml
                        .GetSoftObjectFromXmlFileAsync<T>(filename)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is IOException or InvalidOperationException && attempt1 < maxAttempts1)
                {
                    await Task.Delay(150 * attempt1, cancellationToken).ConfigureAwait(false);
                }
            }
        }


        private static bool _containsNodeRecursive(
            IEnumerable<NodemapItem> nodes,
            string nodeName)
        {
            foreach (var node1 in nodes)
            {
                if (string.Equals(node1.Name, nodeName, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (node1.ChildNodes is { Count: > 0 }
                    && _containsNodeRecursive(node1.ChildNodes, nodeName))
                    return true;
            }
            return false;
        }


        private static void _mergeNodeRecursive(
            NodemapItem target,
            NodemapItem source)
        {
            foreach (var page1 in source._AllTopPages)
            {
                var existingPage1 = target._AllTopPages
                    .FirstOrDefault(x => string.Equals(x.Path, page1.Path, StringComparison.OrdinalIgnoreCase));
                if (existingPage1 is not null)
                    _mergePageRecursive(existingPage1, page1);
                else
                    target.AddTopPage(page1);
            }
            foreach (var child1 in source.ChildNodes)
            {
                var existingChild1 = target.ChildNodes
                    .FirstOrDefault(x => string.Equals(x.Name, child1.Name, StringComparison.OrdinalIgnoreCase));
                if (existingChild1 is not null)
                    _mergeNodeRecursive(existingChild1, child1);
                else
                    target.AddChildNode(child1);
            }
        }


        private static void _mergePageRecursive(
            PagemapItem target,
            PagemapItem source)
        {
            foreach (var childPage1 in source.ChildPages)
            {
                var existingChild1 = target.ChildPages
                    .FirstOrDefault(x => string.Equals(x.Path, childPage1.Path, StringComparison.OrdinalIgnoreCase));
                if (existingChild1 is not null)
                    _mergePageRecursive(existingChild1, childPage1);
                else
                    target.AddChildPage(childPage1);
            }
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[SitemapBuilder] Источник узлов '{path}' не найден. SitemapBuilder формируется из программной конфигурации.")]
            public static partial void MainConfigNotFoundProgrammaticMode(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[SitemapBuilder] Запущено перестроение SitemapBuilder.")]
            public static partial void BuildStarted(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[SitemapBuilder] Перестроение SitemapBuilder завершено успешно. Загружено элементов: {nodesIndexCount} узлов, {pagesIndexCount} страниц.")]
            public static partial void BuildCompleted(
                ILogger logger, int nodesIndexCount, int pagesIndexCount);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[SitemapBuilder] Ошибка при перестроении SitemapBuilder: {message}")]
            public static partial void BuildFailed(
                ILogger logger, string message, Exception ex);

            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "[SitemapBuilder] Источник узлов '{path}' пуст или содержит некорректную структуру.")]
            public static partial void MainConfigEmptyOrInvalid(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[SitemapBuilder] Критическая ошибка десериализации источник страниц '{path}': {message}")]
            public static partial void PageConfigDeserializationError(
                ILogger logger, string path, string message, Exception ex);

            [LoggerMessage(
                Level = LogLevel.Critical,
                Message = "[SitemapBuilder] Не удалось десериализовать источник узлов '{path}': {message}")]
            public static partial void MainConfigDeserializationError(
                ILogger logger, string path, string message, Exception ex);
        }

    }

}
