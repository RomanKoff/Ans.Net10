using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Resources;

namespace Ans.Net10.Web.Forms
{

	public static partial class __e
	{

		/* functions */


		public static FormHelper AppendFormHelper(
			this IHtmlHelper helper,
			string name,
			params ResourceManager[] resources)
		{
			var helper1 = new FormHelper(helper, resources);
			helper.ViewData[name] = helper1;
			return helper1;
		}


		public static FormHelper GetFormHelper(
			this IHtmlHelper helper,
			string name)
		{
			return helper.ViewData[name] as FormHelper;
		}


		public static HtmlString ToHtml(
			this IFormCellControl control,
			bool useTypograf = false)
		{
			return control.ToString().ToHtml(useTypograf);
		}

	}

}
