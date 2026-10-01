// rev 2026-10-01

namespace Ans.Net10.Web
{

	/// <summary>
	/// Базовый класс для специализированных компонентов Razor Tag Helper в экосистеме Ans,
	/// обеспечивающий автоматическое внедрение и доступ к текущему контексту приложения.
	/// </summary>
	/// <param name="current">Экземпляр текущего контекста приложения <see cref="CurrentContext"/>.</param>
	public class _AnsTagHelper_Base(
		CurrentContext current)
		: _TagHelper_Base
	{
		/// <summary>
		/// Предоставляет доступ к текущему контексту выполнения HTTP-запроса, сессии и служб приложения.
		/// </summary>
		public readonly CurrentContext Current = current;
	}

}
