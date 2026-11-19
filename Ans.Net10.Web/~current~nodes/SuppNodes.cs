// rev 2026-11-19

namespace Ans.Net10.Web
{

    /// <summary>
    /// Вспомогательные константы и пути подсистемы карты сайта и информационных узлов.
    /// </summary>
    public static class SuppNodes
    {
        /// <summary>
        /// Имя корневого каталога представлений приложения.
        /// </summary>
        public const string VIEWS_ROOT = "Views";

        /// <summary>
        /// Имя подкаталога информационных узлов внутри каталога представлений.
        /// </summary>
        public const string NODES_SUBDIR = "Nodes";

        /// <summary>
        /// Относительный путь к корневому каталогу представлений информационных узлов (с учетом разделителя текущей ОС).
        /// </summary>
        public static readonly string NODES_ROOT = Path.Combine(VIEWS_ROOT, NODES_SUBDIR);

        /// <summary>
        /// Относительный префикс подкаталога информационных узлов с завершающим разделителем каталогов текущей ОС.
        /// </summary>
        public static readonly string NODES_SUBDIR_PREFIX_SLASH = $"{NODES_SUBDIR}{Path.DirectorySeparatorChar}";

        /// <summary>
        /// Имя конфигурационного файла структуры информационных узлов.
        /// </summary>
        public const string MAPNODES_FILENAME = "_map-nodes.xml";

        /// <summary>
        /// Имя конфигурационного файла структуры страниц контента внутри узла.
        /// </summary>
        public const string MAPPAGES_FILENAME = "_map-pages.xml";

        /// <summary>
        /// Системное имя корневого информационного узла по умолчанию.
        /// </summary>
        public const string INDEX_NODE_NAME = "_main";

        /// <summary>
        /// Системное имя индексной (стартовой) страницы по умолчанию.
        /// </summary>
        public const string INDEX_PAGE_NAME = "start";

        /// <summary>
        /// Расширение XML-файлов конфигурации карты сайта.
        /// </summary>
        public const string MAP_EXTENSION = ".xml";

        /// <summary>
        /// Расширение файлов Razor-представлений.
        /// </summary>
        public const string VIEW_EXTENSION = ".cshtml";

        /// <summary>
        /// Маска поиска файлов Razor-представлений.
        /// </summary>
        public const string VIEW_FILTER = $"*{VIEW_EXTENSION}";
    }

}
