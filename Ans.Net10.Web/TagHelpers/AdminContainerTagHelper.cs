// rev 2026-10-02

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/// <summary>
	/// Асинхронный Tag Helper для создания условного контейнера <c>&lt;admin-container&gt;</c>,
	/// который отображает свое содержимое только пользователям из административной IP-подсети.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="AdminContainerTagHelper"/> с внедрением контекста.
	/// </remarks>
	/// <param name="current">Текущий оркестровый контекст выполнения HTTP-запроса.</param>
	[HtmlTargetElement("admin-container", Attributes = _ATTR_FREE)]
	public class AdminContainerTagHelper(
		CurrentContext current)
		: _AnsTagHelper_Base(current)
	{

		private const string _ATTR_FREE = "free";


		/* attributes */


		/// <summary>
		/// Получает или задает признак «свободного» контейнера.
		/// Если равен <see langword="true"/>, контент выводится без дополнительной визуальной обертки.
		/// Значение по умолчанию: <see langword="false"/>.
		/// </summary>
		/// <value>Логический флаг скрытия декоративного контейнера div.</value>
		[HtmlAttributeName(_ATTR_FREE)]
		public bool IsFree { get; set; } = false;


		/* methods */


		/// <summary>
		/// Асинхронно обрабатывает логику контейнера, проверяя сетевой статус клиента
		/// и полностью уничтожая вывод для неавторизованных IP-адресов.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> или <paramref name="output"/> равен <see langword="null"/>.
		/// </exception>
		/// <remarks>
		/// Метод использует службу <see cref="NetworkService"/> для проверки прав. Если клиент не является 
		/// администратором, контент полностью очищается, предотвращая утечку данных. Если доступ разрешен 
		/// и параметр <see cref="IsFree"/> равен <see langword="false"/>, содержимое оборачивается в тег <c>div</c> 
		/// с предупреждающим фоном Bootstrap.
		/// </remarks>
		public override async Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			UseAutoContent = true;
			output.TagName = null;
			if (!Current.Network.IsAdmin)
			{
				output.SuppressOutput();
				output.Content.Clear();
				return;
			}
			if (!IsFree)
			{
				output.TagMode = TagMode.StartTagAndEndTag;
				output.TagName = "div";
				Class = "bg-warning-subtle";
			}
			await base.ProcessAsync(context, output);
		}

	}

}
