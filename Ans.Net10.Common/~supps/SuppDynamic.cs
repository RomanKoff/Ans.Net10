// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/*
	 * For Razor use ViewData.Get...
	 */



	/// <summary>
	/// Вспомогательный класс для работы с динамическими объектами (<see langword="dynamic"/>).
	/// </summary>
	/// <remarks>
	/// Для контекста Razor рекомендуется использовать специализированные методы расширения вида <c>ViewData.Get...</c>.
	/// </remarks>
	public static class SuppDynamic
	{

		/* functions */


		/// <summary>
		/// Проверяет наличие свойства с указанным именем у динамического объекта.
		/// </summary>
		/// <remarks>
		/// Метод корректно обрабатывает как объекты, реализующие <see cref="IDictionary{TKey, TValue}"/> 
		/// (например, <see cref="System.Dynamic.ExpandoObject"/>), так и стандартные анонимные или строго типизированные объекты через механизмы рефлексии.
		/// </remarks>
		/// <param name="item">Динамический объект для проверки. Не должен быть равен <see langword="null"/>.</param>
		/// <param name="propertyName">Имя искомого свойства.</param>
		/// <returns>
		/// <see langword="true"/>, если свойство с именем <paramref name="propertyName"/> найдено у объекта; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasProperty(
			dynamic item,
			string propertyName)
		{
			if (item is IDictionary<string, object> dict1)
				return dict1.ContainsKey(propertyName);
			return item.GetType().GetProperty(propertyName) != null;
		}

	}

}
