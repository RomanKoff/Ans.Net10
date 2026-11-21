// rev 2026-11-21

using Microsoft.AspNetCore.Html;
using System.Net;
using System.Text;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Управляющий контейнер SEO-метаданных и разметки Open Graph для секции <c>&lt;head&gt;</c> текущей веб-страницы.
    /// </summary>
    /// <param name="current">Текущий оркестровый контекст HTTP-запроса.</param>
    public sealed class MetaData(
        CurrentContext current)
    {

        private readonly CurrentContext _current = current
            ?? throw new ArgumentNullException(nameof(current));


        /* properties */


        /// <summary>
        /// Получает или задает признак автоматической генерации канонического URL (<c>link rel="canonical"</c>)
        /// на основе чистого пути запроса без query-параметров. Значение по умолчанию: <see langword="true"/>.
        /// </summary>
        public bool UseAutoCanonical { get; set; } = true;


        /// <summary>
        /// Получает или задает краткое описание содержимого страницы (<c>meta name="description"</c>).
        /// </summary>
        public string? Description { get; set; }


        /// <summary>
        /// Получает или задает ключевые слова страницы (<c>meta name="keywords"</c>).
        /// </summary>
        public string? Keywords { get; set; }


        /// <summary>
        /// Получает или задает директивы индексирования для поисковых роботов (<c>meta name="robots"</c>, например, <c>"noindex, nofollow"</c>).
        /// </summary>
        public string? Robots { get; set; }


        /// <summary>
        /// Получает или задает канонический URL-адрес страницы (<c>link rel="canonical"</c>).
        /// </summary>
        public string? CanonicalUrl { get; set; }


        /// <summary>
        /// Получает или задает дату и время первичной публикации статьи (<c>article:published_time</c> в формате ISO 8601).
        /// </summary>
        public DateTimeOffset? Article_PublishedTime { get; set; }


        /// <summary>
        /// Получает или задает дату и время последнего обновления статьи (<c>article:modified_time</c> в формате ISO 8601).
        /// </summary>
        public DateTimeOffset? Article_ModifiedTime { get; set; }


        /// <summary>
        /// Получает или задает название рубрики или раздела публикации (<c>article:section</c>).
        /// </summary>
        public string? Article_Section { get; set; }


        /// <summary>
        /// Получает или задает массив ключевых тегов публикации (<c>article:tag</c>).
        /// </summary>
        public string[]? Article_Tags { get; set; }


        /// <summary>
        /// Получает или задает цвет верхней панели браузера и системной строки состояния на мобильных устройствах (<c>meta name="theme-color"</c>, например, <c>"#0d6efd"</c>).
        /// </summary>
        public string? ThemeColor { get; set; }


        /// <summary>
        /// Получает или задает поддерживаемые страницей цветовые схемы интерфейса (<c>meta name="color-scheme"</c>, например, <c>"light dark"</c> или <c>"only light"</c>).
        /// </summary>
        public string? ColorScheme { get; set; }


        /// <summary>
        /// Получает или задает политику автоматического распознавания данных мобильными браузерами (<c>meta name="format-detection"</c>, например, <c>"telephone=no"</c>).
        /// </summary>
        public string? FormatDetection { get; set; }


        /// <summary>
        /// Получает или задает политику передачи HTTP-заголовка <c>Referer</c> при переходах со страницы (<c>meta name="referrer"</c>).
        /// </summary>
        public string? Referrer { get; set; }


        /// <summary>
        /// Получает или задает путь или URL-адрес специализированного превью-изображения для ВКонтакте (<c>vk:image</c>).
        /// Поддерживает префиксы ресурсов (<c>site:</c>, <c>node:</c>, <c>page:</c>).
        /// Рекомендуемый размер для ленты ВК — 537×240 или 1074×480 пикселей (пропорция ~2.2:1).
        /// </summary>
        public string? Vk_Image { get; set; }


        /// <summary>
        /// Получает или задает название сайта для протокола Open Graph (<c>og:site_name</c>).
        /// По умолчанию инициализируется из заголовка сайта (<c>Current.Site.Title</c>).
        /// </summary>
        public string? Og_SiteName { get; set; }


        /// <summary>
        /// Получает или задает заголовок страницы для протокола Open Graph (<c>og:title</c>).
        /// По умолчанию вычисляется на основе заголовка текущей страницы, узла или сайта.
        /// </summary>
        public string? Og_Title { get; set; }


        /// <summary>
        /// Получает или задает описание страницы для протокола Open Graph (<c>og:description</c>).
        /// Если не задано явно, наследует значение свойства <see cref="Description"/>.
        /// </summary>
        public string? Og_Description { get; set; }


        /// <summary>
        /// Получает или задает путь или URL-адрес превью-изображения для протокола Open Graph (<c>og:image</c>).
        /// Поддерживает префиксы ресурсов (<c>site:</c>, <c>node:</c>, <c>page:</c>).
        /// Рекомендуемое соотношение сторон — 1.91:1 (например, 1200×630 пикселей), чтобы картинка не обрезалась в мессенджерах.
        /// </summary>
        public string? Og_Image { get; set; }


        /// <summary>
        /// Получает или задает ширину изображения Open Graph в пикселях (<c>og:image:width</c>).
        /// </summary>
        public int? Og_ImageWidth { get; set; }


        /// <summary>
        /// Получает или задает высоту изображения Open Graph в пикселях (<c>og:image:height</c>).
        /// </summary>
        public int? Og_ImageHeight { get; set; }


        /// <summary>
        /// Получает или задает канонический URL объекта для протокола Open Graph (<c>og:url</c>).
        /// По умолчанию инициализируется из <see cref="RequestData.AbsoluteUrl"/>.
        /// </summary>
        public string? Og_Url { get; set; }


        /// <summary>
        /// Получает или задает тип контента для протокола Open Graph (<c>og:type</c>).
        /// Значение по умолчанию: <c>"website"</c>.
        /// </summary>
        public string? Og_Type { get; set; }


        /* methods */


        /// <summary>
        /// Запрещает поисковым роботам индексировать текущую страницу и переходить по ссылкам на ней,
        /// устанавливая свойство <see cref="Robots"/> в значение <c>"noindex, nofollow"</c>.
        /// </summary>
        public void RobotsOff()
        {
            Robots = "noindex, nofollow";
        }


        /// <summary>
        /// Формирует канонический URL (<see cref="CanonicalUrl"/>) на основе текущего пути запроса
        /// с опциональной строкой значимых query-параметров (например, <c>"page=2"</c> или <c>"year=2026"</c>).
        /// </summary>
        /// <param name="queryString">Строка значимых параметров запроса без ведущего знака вопроса, либо <see langword="null"/> для чистого URL.</param>
        public void SetCanonical(
            string? queryString = null)
        {
            string base1 = _getCleanCurrentUrl();
            CanonicalUrl = string.IsNullOrWhiteSpace(queryString)
                ? base1
                : $"{base1}?{queryString.TrimStart('?')}";
        }


        /// <summary>
        /// Отключает автоматическую и ручную генерацию тега <c>link rel="canonical"</c> для текущей страницы.
        /// </summary>
        public void CanonicalOff()
        {
            UseAutoCanonical = false;
            CanonicalUrl = null;
        }


        /// <summary>
        /// Устанавливает тип Open Graph (<see cref="Og_Type"/>) в значение <c>"article"</c> и опционально заполняет метаданные публикации.
        /// </summary>
        /// <param name="publishedTime">Дата и время публикации статьи.</param>
        /// <param name="modifiedTime">Дата и время последнего изменения статьи.</param>
        /// <param name="section">Название рубрики публикации.</param>
        /// <param name="tags">Набор тегов публикации.</param>
        public void SetArticle(
            DateTimeOffset? publishedTime = null,
            DateTimeOffset? modifiedTime = null,
            string? section = null,
            params string[]? tags)
        {
            Og_Type = "article";
            if (publishedTime is not null)
                Article_PublishedTime = publishedTime;
            if (modifiedTime is not null)
                Article_ModifiedTime = modifiedTime;
            if (!string.IsNullOrWhiteSpace(section))
                Article_Section = section;
            if (tags is { Length: > 0 })
                Article_Tags = tags;
        }


        /// <summary>
        /// Отключает автоматическое преобразование числовых последовательностей (артикулов, индексов, кодов) 
        /// в кликабельные телефонные ссылки на мобильных устройствах (<c>format-detection="telephone=no"</c>).
        /// </summary>
        public void DisableTelephoneDetection()
        {
            FormatDetection = "telephone=no";
        }


        /* functions */


        /// <summary>
        /// Формирует готовую HTML-разметку всех заполненных SEO и Open Graph метатегов для вставки в <c>&lt;head&gt;</c>.
        /// </summary>
        /// <returns>Безопасная строка разметки <see cref="HtmlString"/>.</returns>
        public HtmlString Render()
        {
            var sb1 = new StringBuilder();

            _appendMetaName(sb1, "description", Description);
            _appendMetaName(sb1, "keywords", Keywords);
            _appendMetaName(sb1, "robots", Robots);
            _appendMetaName(sb1, "theme-color", ThemeColor);
            _appendMetaName(sb1, "color-scheme", ColorScheme);
            _appendMetaName(sb1, "format-detection", FormatDetection);
            _appendMetaName(sb1, "referrer", Referrer);

            string? effectiveCanonical1 = CanonicalUrl;
            if (string.IsNullOrWhiteSpace(effectiveCanonical1)
                && UseAutoCanonical
                && _current.HttpContext.Response.StatusCode == 200)
                effectiveCanonical1 = _getCleanCurrentUrl();

            if (!string.IsNullOrWhiteSpace(effectiveCanonical1))
                _appendLine(sb1, $"""<link rel="canonical" href="{_encode(effectiveCanonical1)}" />""");

            string? vkImage1 = _resolveOgImage(Vk_Image ?? _current.Options.Meta?.DefaultVkImage);
            _appendMetaProperty(sb1, "vk:image", vkImage1);

            bool useDefaultOgImage1 = string.IsNullOrWhiteSpace(Og_Image);
            string? ogImage1 = _resolveOgImage(useDefaultOgImage1
                ? _current.Options.Meta?.DefaultOgImage
                : Og_Image);
            int? ogImageWidth1 = useDefaultOgImage1
                ? (Og_ImageWidth ?? _current.Options.Meta?.DefaultOgImageWidth)
                : Og_ImageWidth;
            int? ogImageHeight1 = useDefaultOgImage1
                ? (Og_ImageHeight ?? _current.Options.Meta?.DefaultOgImageHeight)
                : Og_ImageHeight;

            string? ogSiteName1 = Og_SiteName ?? _current.Site.Title;
            string? ogTitle1 = Og_Title ?? _getDefaultOgTitle();
            string? ogDescription1 = Og_Description ?? Description;
            string? ogUrl1 = Og_Url ?? effectiveCanonical1 ?? _current.Request.AbsoluteUrl;
            string? ogType1 = Og_Type ?? "website";

            _appendMetaProperty(sb1, "og:site_name", ogSiteName1);
            _appendMetaProperty(sb1, "og:title", ogTitle1);
            _appendMetaProperty(sb1, "og:description", ogDescription1);

            if (!string.IsNullOrEmpty(ogImage1))
            {
                _appendMetaProperty(sb1, "og:image", ogImage1);
                if (ogImageWidth1 is > 0)
                    _appendMetaProperty(sb1, "og:image:width", ogImageWidth1.Value.ToString());
                if (ogImageHeight1 is > 0)
                    _appendMetaProperty(sb1, "og:image:height", ogImageHeight1.Value.ToString());
            }

            _appendMetaProperty(sb1, "og:url", ogUrl1);
            _appendMetaProperty(sb1, "og:type", ogType1);

            if (Article_PublishedTime is not null)
                _appendMetaProperty(sb1, "article:published_time", Article_PublishedTime.Value.ToString("O"));

            if (Article_ModifiedTime is not null)
                _appendMetaProperty(sb1, "article:modified_time", Article_ModifiedTime.Value.ToString("O"));

            _appendMetaProperty(sb1, "article:section", Article_Section);

            if (Article_Tags is { Length: > 0 })
            {
                foreach (var tag1 in Article_Tags)
                    _appendMetaProperty(sb1, "article:tag", tag1);
            }

            return new HtmlString(sb1.ToString());
        }


        /* privates */


        private string _getCleanCurrentUrl()
        {
            return $"{_current.Host.BaseUrl}{_current.Request.RelativeUrl}";
        }


        private string? _getDefaultOgTitle()
        {
            if (!string.IsNullOrWhiteSpace(_current.Page.Title))
                return _current.Page.Title;
            if (!string.IsNullOrWhiteSpace(_current.Node.Title))
                return _current.Node.Title;
            return _current.Site.Title;
        }


        private string? _resolveOgImage(
            string? image)
        {
            if (string.IsNullOrWhiteSpace(image))
                return null;
            string resolved1 = _current.GetResUrl(image);
            if (resolved1.StartsWith('/'))
                return $"{_current.Host.BaseUrl}{resolved1}";
            return resolved1;
        }


        private static void _appendMetaName(
            StringBuilder sb,
            string name,
            string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;
            _appendLine(sb, $"""<meta name="{name}" content="{_encode(content)}" />""");
        }


        private static void _appendMetaProperty(
            StringBuilder sb,
            string property,
            string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;
            _appendLine(sb, $"""<meta property="{property}" content="{_encode(content)}" />""");
        }


        private static void _appendLine(
            StringBuilder sb,
            string tag)
        {
            if (sb.Length > 0)
                sb.AppendLine().Append('\t');
            sb.Append(tag);
        }


        private static string _encode(
            string value)
        {
            return WebUtility.HtmlEncode(value.Trim());
        }

    }

}
