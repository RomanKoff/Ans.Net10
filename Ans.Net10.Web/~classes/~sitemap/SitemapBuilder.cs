// rev 2026-11-10

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

        private readonly List<NodemapItem> _topNodes = [];
        private readonly SemaphoreSlim _syncSemaphore = new(1, 1);


        /* consts */


        /// <summary>
        /// Имя конфигурационного файла структуры информационных узлов.
        /// </summary>
        public const string MAPNODES_FILENAME = "_map-nodes.xml";


        /// <summary>
        /// Имя конфигурационного файла структуры страниц контента внутри узла.
        /// </summary>
        public const string MAPPAGES_FILENAME = "_map-pages.xml";



        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SitemapBuilder"/> с вычислением базового пути к файлам карт.
        /// </summary>
        /// <param name="env">Окружение веб-приложения для определения корневого каталога.</param>
        /// <param name="options">Конфигурационные параметры навигации и путей к XML-файлам.</param>
        /// <param name="logger">Экземпляр логгера для записи событий сборки структуры.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если один из переданных параметров равен <see langword="null"/>.</exception>
        public SitemapBuilder(
            IWebHostEnvironment env,
            LibWebOptions options,
            ILogger<SitemapBuilder> logger)
        {
            ArgumentNullException.ThrowIfNull(env);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;
            if (options.XmlSitemap is not null)
                SitemapPath = Path.Combine(env.ContentRootPath, options.XmlSitemap.Path).Replace('\\', '/');
            TopNodes = _topNodes.AsReadOnly();
        }


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
        /// Асинхронно загружает, парсит и полностью перестраивает структуру карты сайта из XML-файлов конфигурации.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
        /// <returns>Значение <see langword="true"/>, если перестроение графа и индексов завершилось успехом; иначе — <see langword="false"/> (ошибка I/O или парсинга).</returns>
        public async ValueTask<bool> InitializeAsync(
            CancellationToken cancellationToken = default)
        {
            await _syncSemaphore
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
            try
            {
                _log.BuildStarted(_logger);
                string mapnodesFilename1 = Path.Combine(SitemapPath, MAPNODES_FILENAME);
                if (!File.Exists(mapnodesFilename1))
                {
                    _log.MainConfigNotFound(_logger, mapnodesFilename1);
                    return false;
                }

                NodesXmlDto? nodesDto1;
                try
                {
                    nodesDto1 = await SuppXml
                        .GetSoftObjectFromXmlFileAsync<NodesXmlDto>(mapnodesFilename1)
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
                {
                    _log.MainConfigDeserializationError(_logger, mapnodesFilename1, ex.Message, ex);
                    return false;
                }

                if (nodesDto1?.Items is null)
                {
                    _log.MainConfigEmptyOrInvalid(_logger, mapnodesFilename1);
                    return false;
                }

                var tempTopNodes1 = new List<NodemapItem>();
                await _parseNodesXmlAsync(
                    nodesDto1.Items, tempTopNodes1, cancellationToken)
                    .ConfigureAwait(false);

                _topNodes.Clear();
                _topNodes.AddRange(tempTopNodes1);
                _buildIndexes();

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
        /// <param name="nodemapItemDef">Декларативное описание добавляемого узла.</param>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="nodemapItemDef"/> равен <see langword="null"/>.</exception>
        public void AddTopNodeDef(
            NodemapItemDef nodemapItemDef)
        {
            ArgumentNullException.ThrowIfNull(nodemapItemDef);
            var node1 = new NodemapItem(nodemapItemDef);
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
        /// Программно вставляет дочерний узел в уже существующий родительский узел карты сайта.
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
                parentNode1.AddChildNode(childNode);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Программно вставляет контентную страницу в существующий информационный узел графа.
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
                {
                    string mappagesFilename1 = Path.Combine(SitemapPath, nodeItem1.Name, MAPPAGES_FILENAME);
                    if (File.Exists(mappagesFilename1))
                    {
                        try
                        {
                            var pagesDto1 = await SuppXml
                                .GetSoftObjectFromXmlFileAsync<PagesXmlDto>(mappagesFilename1)
                                .ConfigureAwait(false);

                            if (pagesDto1?.Items is { Count: > 0 })
                                _parsePagesXml(pagesDto1.Items, nodeItem1, null);
                        }
                        catch (Exception ex) when (ex is not OperationCanceledException)
                        {
                            // Логируем ошибку конкретного файла, но не прерываем сборку всего графа
                            _log.PageConfigDeserializationError(_logger, mappagesFilename1, ex.Message, ex);
                        }
                    }
                }
                targetList.Add(nodeItem1);
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
            if (!string.IsNullOrEmpty(node.Name))
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


        /* logs */


        /// <summary>
        /// Вложенный класс генерации логов с использованием Source Generator.
        /// </summary>
        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[SitemapBuilder] Запущено перестроение карты сайта.")]
            public static partial void BuildStarted(
                ILogger logger);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "[SitemapBuilder] Перестроение карты сайта представлений завершено успешно. Загружено элементов: {nodesIndexCount} узлов, {pagesIndexCount} страниц.")]
            public static partial void BuildCompleted(
                ILogger logger, int nodesIndexCount, int pagesIndexCount);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[SitemapBuilder] Ошибка при перестроении карты сайта: {message}")]
            public static partial void BuildFailed(
                ILogger logger, string message, Exception ex);

            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "[SitemapBuilder] Главный конфигурационный файл узлов не найден по пути: '{path}'.")]
            public static partial void MainConfigNotFound(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "[SitemapBuilder] Главный конфигурационный файл '{path}' пуст или содержит некорректную структуру.")]
            public static partial void MainConfigEmptyOrInvalid(
                ILogger logger, string path);

            [LoggerMessage(
                Level = LogLevel.Error,
                Message = "[SitemapBuilder] Критическая ошибка десериализации файла страниц '{path}': {message}")]
            public static partial void PageConfigDeserializationError(
                ILogger logger, string path, string message, Exception ex);

            [LoggerMessage(
                Level = LogLevel.Critical,
                Message = "[SitemapBuilder] Не удалось десериализовать главный конфигурационный файл узлов '{path}': {message}")]
            public static partial void MainConfigDeserializationError(
                ILogger logger, string path, string message, Exception ex);
        }

    }

}
