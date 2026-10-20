// rev 2026-10-17

namespace Ans.Net10.Web
{

	/// <summary>
	/// Описывает профиль текущего узла.
	/// </summary>
	public sealed class NodeProfile(
		CurrentContext current)
		: _Profile_Proto(current)
	{

		/// <summary>
		/// Получает или задает связанный элемент карты сайта для текущего информационного узла (Node).
		/// </summary>
		/// <remarks>
		/// При установке элемента автоматически инициализирует свойства <see cref="_Profile_Proto.Title"/> 
		/// и <see cref="_Profile_Proto.ShortTitle"/> значениями из метаданных карты сайта.
		/// </remarks>
		/// <value>
		/// Объект <see cref="NodemapItem"/>, содержащий навигационную структуру узла, 
		/// или <see langword="null"/>, если узел не привязан к структуре.
		/// </value>
		public NodemapItem? MapItem
		{
			get;
			set
			{
				field = value;
				if (value is not null)
				{
					Title = value.Title;
					ShortTitle = value.ShortTitle;
				}
			}
		}

	}

}
