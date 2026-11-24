// rev 2026-11-23

using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace Ans.Net10.Web.Compilation
{

    /// <summary>
    /// Провайдер компилятора представлений MVC, предоставляющий экземпляр <see cref="AnsRazorViewCompiler"/> 
    /// в конвейер рендеринга ASP.NET Core.
    /// </summary>
    public sealed class AnsRazorViewCompilerProvider
        : IViewCompilerProvider
    {

        private readonly AnsRazorViewCompiler _compiler;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsRazorViewCompilerProvider"/>.
        /// </summary>
        /// <param name="compiler">Экземпляр рантайм-компилятора представлений.</param>
        public AnsRazorViewCompilerProvider(
            AnsRazorViewCompiler compiler)
        {
            ArgumentNullException.ThrowIfNull(compiler);
            _compiler = compiler;
        }


        /* functions */


        /// <summary>
        /// Возвращает активный экземпляр компилятора Razor-представлений.
        /// </summary>
        /// <returns>Экземпляр <see cref="IViewCompiler"/>.</returns>
        public IViewCompiler GetCompiler()
        {
            return _compiler;
        }

    }

}