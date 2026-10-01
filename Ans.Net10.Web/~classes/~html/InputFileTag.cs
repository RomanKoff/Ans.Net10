// rev 2026-10-01

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации поля выбора файла 
	/// <c>&lt;input type="file" /&gt;</c> со встроенным самозакрывающимся режимом рендеринга.
	/// </summary>
	public class InputFileTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="InputFileTag"/>, формируя атрибуты идентификатора, имени и типа поля выбора файла.
		/// </summary>
		/// <param name="name">Системное имя и уникальный идентификатор HTML-элемента (атрибуты <c>id</c> и <c>name</c>).</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> равен <see langword="null"/>.</exception>
		public InputFileTag(
			string name)
			: base("input", TagRenderMode.SelfClosing)
		{
			ArgumentNullException.ThrowIfNull(name);
			Name = name;
			MergeAttribute("id", Name);
			MergeAttribute("name", Name);
			MergeAttribute("type", "file");
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя и идентификатор поля выбора файла.
		/// </summary>
		public string Name { get; }

	}

}
