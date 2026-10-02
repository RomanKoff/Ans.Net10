using Ans.Net10.Common;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Ans.Net10.Web.TagHelpers
{

	/*
	 * <ans-table col-classes="" col-styles="">...</ans-table>
	 */



	[HtmlTargetElement("ans-table", TagStructure = TagStructure.NormalOrSelfClosing)]
	public partial class AnsTable_TagHelper(
		CurrentContext current)
		: TagHelper
	{

		private const string ColClasses_AttributeName = "col-classes";
		private const string ColStyles_AttributeName = "col-styles";


		/* properties */


		[HtmlAttributeName(ColClasses_AttributeName)]
		public string ColClassesData { get; set; }

		[HtmlAttributeName(ColStyles_AttributeName)]
		public string ColStylesData { get; set; }

		public string Class { get; set; }


		/* methods */


		public override void Process(
			TagHelperContext context,
			TagHelperOutput output)
		{
			base.Process(context, output);
			var lines1 = _getLines(output.GetChildContent());
			var thead1 = lines1.First().Split('|');
			var tbody1 = lines1.Skip(1)
				.Select(x => x.SplitFix("|", thead1.Length));
			var classes1 = (ColClassesData ?? "")
				.SplitFix("|", thead1.Length)
				.Select(x => x.Make(" class=\"{0}\""))
				.ToArray();
			var styles1 = (ColStylesData ?? "")
				.SplitFix("|", thead1.Length)
				.Select(x => x.Make(" style=\"{0}\""))
				.ToArray();
			var sb1 = new StringBuilder();
			sb1.AppendLine($"<table class=\"{Class ?? current.App.GetValue("css-table1")}\">");
			sb1.Append("<thead><tr>");
			foreach (var col1 in thead1)
				sb1.Append("<th>" + col1 + "</th>");
			sb1.Append("</tr></thead>");
			sb1.AppendLine("<tbody>");
			foreach (var row1 in tbody1)
			{
				sb1.Append("<tr>");
				for (int i1 = 0; i1 < thead1.Length; i1++)
					sb1.Append($"<td{classes1[i1]}{styles1[i1]}>{row1[i1].ToHtml(true)}</td>");
				sb1.AppendLine("</tr>");
			}
			sb1.AppendLine("</tbody>");
			sb1.AppendLine("</table>");
			output.Content.AppendHtml(sb1.ToString());
		}


		/* privates */


		private static IEnumerable<string> _getLines(
			string data)
		{
			return data
				.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)
				.Select(x => x.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("//"));
		}

	}

}
