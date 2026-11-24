// rev 2026-11-24

namespace Ans.Net10.Web
{

    /// <summary>
    /// Определяет контракт поставщика программных элементов карты сайта, 
    /// позволяющий библиотекам и модулям регистрировать собственные информационные узлы и страницы при сборке <see cref="SitemapBuilder"/>.
    /// </summary>
    public interface ISitemapContributor
    {
        /// <summary>
        /// Выполняет программное добавление или модификацию узлов и страниц в переданном строителе карты сайта.
        /// </summary>
        /// <param name="sitemap">Активный экземпляр строителя карты сайта <see cref="SitemapBuilder"/>.</param>
        void ConfigureSitemap(
            SitemapBuilder sitemap);
    }



    /// <summary>
    /// Встроенный поставщик системных узлов и сервисных страниц библиотеки <c>Ans.Net10.Web</c> (<c>/Areas/Ans/Pages</c>).
    /// </summary>
    internal sealed class AnsSitemapContributor
        : ISitemapContributor
    {
        /// <inheritdoc />
        public void ConfigureSitemap(
            SitemapBuilder sitemap)
        {
            ArgumentNullException.ThrowIfNull(sitemap);

            sitemap.AddTopNodeDef(
                new NodemapDef("ans", "CMS", display: "none", passthroughs: "|all", topPages:
                [
                    new PagemapDef("system", "Система", childPages:
                    [
                        new PagemapDef("console", "Серверная консоль"),
                        new PagemapDef("compile", "Сборка представлений и моделей"),
                        new PagemapDef("pages", "Проверка страниц")
                    ])
                ])
            );
        }
    }

}
