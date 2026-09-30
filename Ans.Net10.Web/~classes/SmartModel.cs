// rev 2026-09-28

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Универсальная составная модель данных (Smart Model), используемая для гибкого маппинга,
	/// построения иерархических структур, хранения динамических параметров и автоматической типографики текста.
	/// </summary>
	public class SmartModel
	{

		/* properties */


		public SmartModel[]? Items { get; set; }
		public SmartModel? Master { get; set; }
		public SmartModel[]? Slaves { get; set; }
		public SmartModel? Parent { get; set; }
		public SmartModel[]? Children { get; set; }
		public SmartModel[]? Data { get; set; }
		public Dictionary<string, SmartModel>? Dict { get; set; }
		public DictTagStylers? Stylers { get; set; }
		public DictString Values { get; set; } = [];
		public Dictionary<string, bool> Flags { get; set; } = [];

		public int Id { get; set; }
		public DateTime? Date { get; set; }
		public DateTime? DateOn { get; set; }
		public DateTime? DateOff { get; set; }
		public string? DateRaw { get; set; }
		public string? DateRawOn { get; set; }
		public string? DateRawOff { get; set; }
		public string? Code { get; set; }
		public string? Number { get; set; }
		public string? Value { get; set; }


		public string? Name
		{
			get;
			set
			{
				field = value;
				NameHtml = value?.ToHtml(true) ?? HtmlString.Empty;
			}
		}

		public string? Title
		{
			get;
			set
			{
				field = value;
				TitleHtml = value?.ToHtml(true) ?? HtmlString.Empty;
			}
		}

		public string? Description
		{
			get;
			set
			{
				field = value;
				DescriptionHtml = value?.ToHtml(true) ?? HtmlString.Empty;
			}
		}

		public string? Place
		{
			get;
			set
			{
				field = value;
				PlaceHtml = value?.ToHtml(true) ?? HtmlString.Empty;
			}
		}


		public string? Image { get; set; }
		public string? Phone { get; set; }
		public string? Phones { get; set; }
		public string? Email { get; set; }
		public string? Emails { get; set; }
		public string? Url { get; set; }
		public string? Link { get; set; }
		public string? Links { get; set; }
		public string? Media { get; set; }
		public string? Medias { get; set; }
		public string? Address { get; set; }
		public string? Addresses { get; set; }
		public string? Face { get; set; }
		public string? Mode { get; set; }
		public string? Type { get; set; }
		public string? Template { get; set; }
		public string? Class { get; set; }
		public string? Style { get; set; }
		public string? Properties { get; set; }
		public string? Tags { get; set; }
		public string? Info { get; set; }
		public string? Remark { get; set; }
		public string? Memo { get; set; }


		/* readonly properties */


		public HtmlString NameHtml { get; private set; } = HtmlString.Empty;
		public HtmlString TitleHtml { get; private set; } = HtmlString.Empty;
		public HtmlString DescriptionHtml { get; private set; } = HtmlString.Empty;
		public HtmlString PlaceHtml { get; private set; } = HtmlString.Empty;


		/* functions */


		/// <summary>
		/// Извлекает стилизатор тега по его уникальному имени.
		/// </summary>
		/// <param name="name">Имя запрашиваемого стилизатора.</param>
		/// <returns>Экземпляр <see cref="TagStyler"/> или <see langword="null"/>, если стилизатор не найден.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TagStyler? GetStyler(
			string name)
		{
			return Stylers != null && Stylers.TryGetValue(name, out var styler1)
				? styler1 : null;
		}


		/// <summary>
		/// Каскадно объединяет и применяет базовый набор стилизаторов к текущей коллекции модели.
		/// Если стилизатор с определенным ключом уже существует, к нему применяются базовые параметры, 
		/// иначе он регистрируется как новый.
		/// </summary>
		/// <param name="stylers">Входящая коллекция базовых стилизаторов.</param>
		public void ApplyBaseStylers(
			DictTagStylers? stylers)
		{
			if (stylers == null)
				return;
			Stylers ??= new DictTagStylers();
			foreach (var incoming1 in stylers)
				if (Stylers.TryGetValue(incoming1.Key, out var existingStyler))
					existingStyler.ApplyBase(incoming1.Value);
				else
					Stylers[incoming1.Key] = incoming1.Value;
		}

	}

}
