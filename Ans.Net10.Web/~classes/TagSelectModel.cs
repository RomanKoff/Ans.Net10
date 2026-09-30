// rev 2026-09-28

namespace Ans.Net10.Web
{

	/// <summary>
	/// Композитная модель данных, инкапсулирующая полный список доступных опций и информацию 
	/// о текущем выбранном элементе для HTML-компонента <c>&lt;select&gt;</c>.
	/// </summary>
	public class TagSelectModel
	{
		/// <summary>
		/// Получает или задает текущий выбранный элемент списка. 
		/// Может содержать <see langword="null"/>, если ни одна опция не выбрана.
		/// </summary>
		public TagOptionModel? Selected { get; set; }

		/// <summary>
		/// Получает или задает перечисляемую коллекцию всех доступных элементов (опций) списка выбора.
		/// </summary>
		public IEnumerable<TagOptionModel> Options { get; set; } = [];
	}

}
