using Ans.Net10.Common;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;

namespace Ans.Net10.Web.TagHelpers
{

	/*
	 * <app-dict-value name="" key="" />
	 * <app-address-value key="" mode="0|1|2|3" />
	 * <app-smart-address value="" />
	 * <app-value key="" />
	 */



	/// <summary>
	/// Отображает значение ключа {key}
	/// из словаря appsettings.json/Ans.Net10.Web/Dicts/{name}
	/// </summary>
	[HtmlTargetElement("app-dict-value", TagStructure = TagStructure.WithoutEndTag)]
	public class AppDictValueTagHelper(
		CurrentContext current)
		: _TagHelper_Base
	{

		private readonly CurrentContext _current = current;

		private const string Name_AttributeName = "name";
		private const string Key_AttributeName = "key";


		/* properties */


		/// <summary>
		/// Имя словаря
		/// </summary>
		[Required]
		[HtmlAttributeName(Name_AttributeName)]
		public string Name { get; set; }


		/// <summary>
		/// Ключ в словаре
		/// </summary>
		[Required]
		[HtmlAttributeName(Key_AttributeName)]
		public string Key { get; set; }


		/* methods */


		public override void Process(
			TagHelperContext context,
			TagHelperOutput output)
		{
			base.Process(context, output);
			output.TagName = null;
			if (string.IsNullOrEmpty(Name))
				throw new Exception(
					$"<app-dict-value/> 'name' attribute is required!");
			if (string.IsNullOrEmpty(Key))
				throw new Exception(
					$"<app-dict-value/> 'key' attribute is required!");
			output.Content.AppendHtml(_current.App.GetDictValueHtml(Name, Key));
		}

	}



	/// <summary>
	/// Отображает значение адреса {key}[{mode}]
	/// из словаря appsettings.json/Ans.Net10.Web/Addresses
	/// </summary>
	[HtmlTargetElement("app-address-value", TagStructure = TagStructure.WithoutEndTag)]
	public class AppAddressValueTagHelper(
		CurrentContext current)
		: _TagHelper_Base
	{

		private readonly CurrentContext _current = current;

		private const string Key_AttributeName = "key";
		private const string Mode_AttributeName = "mode";


		/* properties */


		[Required]
		[HtmlAttributeName(Key_AttributeName)]
		public string Key { get; set; }


		[HtmlAttributeName(Mode_AttributeName)]
		public int Mode { get; set; } = 0;


		/* methods */


		public override void Process(
			TagHelperContext context,
			TagHelperOutput output)
		{
			base.Process(context, output);
			output.TagName = null;
			if (string.IsNullOrEmpty(Key))
				throw new Exception(
					$"<app-address-value/> 'key' attribute is required!");
			if (Mode > 3)
				Mode = 0;
			output.Content.AppendHtml(_current.App.GetAddressSetHtml(Key)[Mode]);
		}

	}



	/// <summary>
	/// Отображает адрес с автозаменой
	/// по словарю appsettings.json/Ans.Net10.Web/Addresses
	/// </summary>
	[HtmlTargetElement("app-smart-address", TagStructure = TagStructure.WithoutEndTag)]
	public class AppAddressTagHelper(
		CurrentContext current)
		: _TagHelper_Base
	{

		private readonly CurrentContext _current = current;

		private const string Value_AttributeName = "value";


		/* properties */


		[Required]
		[HtmlAttributeName(Value_AttributeName)]
		public string Value { get; set; }


		/* methods */


		public override void Process(
			TagHelperContext context,
			TagHelperOutput output)
		{
			base.Process(context, output);
			output.TagName = null;
			if (string.IsNullOrEmpty(Value))
				throw new Exception(
					$"<app-smart-address/> 'value' attribute is required!");
			output.Content.AppendHtml(
				SuppValues.GetSubstitutionAddress(
					Value, _current.App.GetAddressesShort())
						.ToHtml(true));
		}

	}



	/// <summary>
	/// Отображает значение ключа {key}
	/// из словаря appsettings.json/Ans.Net10.Web/Values
	/// </summary>
	[HtmlTargetElement("app-value", TagStructure = TagStructure.WithoutEndTag)]
	public class AppValueTagHelper(
		CurrentContext current)
		: _TagHelper_Base
	{

		private readonly CurrentContext _current = current;

		private const string Key_AttributeName = "key";


		/* properties */


		[Required]
		[HtmlAttributeName(Key_AttributeName)]
		public string Key { get; set; }


		/* methods */


		public override void Process(
			TagHelperContext context,
			TagHelperOutput output)
		{
			base.Process(context, output);
			output.TagName = null;
			if (string.IsNullOrEmpty(Key))
				throw new Exception(
					$"<app-value/> key attribute is required!");
			output.Content.AppendHtml(_current.App.GetValueHtml(Key));
		}

	}

}
