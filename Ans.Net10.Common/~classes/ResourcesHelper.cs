// rev 2026-09-25

using Ans.Net10.Common.Resources;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Хелпер для каскадного извлечения и объединения метаданных отображения полей 
	/// из менеджеров локализованных ресурсов (<see cref="ResourceManager"/>).
	/// </summary>
	public class ResourcesHelper
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ResourcesHelper"/>,
		/// дополняя переданные ресурсы базовым ресурсом <see cref="Ans.Net10.Common.Resources.Faces"/>.
		/// </summary>
		/// <param name="resources">
		/// Список дополнительных менеджеров ресурсов для поиска метаданных.
		/// </param>
		public ResourcesHelper(
			params ResourceManager[] resources)
		{
			Resources = resources.GetArrayAdd(Faces.ResourceManager);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает каскадный массив используемых менеджеров ресурсов.
		/// </summary>
		/// <value>
		/// Массив объектов <see cref="ResourceManager"/>, упорядоченный по приоритету поиска.
		/// </value>
		public ResourceManager[] Resources { get; }


		/* functions */


		/// <summary>
		/// Формирует и возвращает каскадный массив менеджеров ресурсов, объединяя переданные ресурсы 
		/// со стандартным базовым менеджером ресурсов локализации интерфейсов полей <see cref="Ans.Net10.Common.Resources.Faces"/>.
		/// </summary>
		/// <param name="resources">Набор дополнительных пользовательских менеджеров ресурсов локализации.</param>
		/// <returns>
		/// Объединенный массив <see cref="ResourceManager"/>, готовый для каскадного поиска метаданных.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ResourceManager[] GetFormResourceManagers(
			params ResourceManager[] resources)
		{
			return [.. resources, Faces.ResourceManager];
		}


		/// <summary>
		/// Каскадно собирает метаданные поля из всех зарегистрированных ресурсов. 
		/// Каждые последующие найденные свойства точечно перезаписывают предыдущие, если они не пусты.
		/// </summary>
		/// <param name="key">Ключ (имя поля) для поиска в ресурсах.</param>
		/// <returns>
		/// Заполненный объект <see cref="CrudFace"/>, содержащий объединенные метаданные поля, 
		/// или <see langword="null"/>, если параметр <paramref name="key"/> равен <see langword="null"/>.
		/// </returns>
		public CrudFace? GetCrudFace(
			string key)
		{
			if (key == null)
				return null;
			string titleRaw1 = string.Empty;
			string shortTitleRaw1 = string.Empty;
			string description1 = string.Empty;
			string sample1 = string.Empty;
			string helpLink1 = string.Empty;
			if (Resources != null)
			{
				foreach (var resource1 in Resources)
				{
					if (resource1 == null)
						continue;
					var s1 = resource1.GetString(key);
					if (string.IsNullOrEmpty(s1))
						continue;
					var parser1 = new StringParser(s1);
					var title1 = parser1.Get(0);
					if (!string.IsNullOrEmpty(title1))
						titleRaw1 = title1;
					var short1 = parser1.Get(1);
					if (!string.IsNullOrEmpty(short1))
						shortTitleRaw1 = short1;
					var desc1 = parser1.Get(2);
					if (!string.IsNullOrEmpty(desc1))
						description1 = desc1;
					var samp1 = parser1.Get(3);
					if (!string.IsNullOrEmpty(samp1))
						sample1 = samp1;
					var help1 = parser1.Get(4);
					if (!string.IsNullOrEmpty(help1))
						helpLink1 = help1;
				}
			}
			return new CrudFace(
				key, titleRaw1, shortTitleRaw1, description1, sample1, helpLink1);
		}

	}

}
