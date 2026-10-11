using Microsoft.AspNetCore.Html;
using System.Text;

namespace Ans.Net10.Web
{

	public class MetaData(
		CurrentContext current)
	{

		/* properties */


		public string Description { get; set; }
		public string Keywords { get; set; }
		public string Og_SiteName { get; set; }
		public string Og_Title { get; set; }
		public string Og_Description { get; set; }
		public string Og_Image { get; set; }
		public string Og_Url { get; set; }
		public string Og_Type { get; set; }


		/* functions */


		public HtmlString Render()
		{	
			Og_SiteName ??= current.Site.Title;
			Og_Title ??= current.Page.ParentsTitles;
			Og_Description ??= Description;
			Og_Url ??= current.Request.AbsoluteUrl;
			Og_Type ??= "website";
			var sb1 = new StringBuilder();

			if (!string.IsNullOrEmpty(Description))
				sb1.Append($@"
	<meta name=""description"" content=""{Description}""/>");

			if (!string.IsNullOrEmpty(Keywords))
				sb1.Append($@"
	<meta name=""keywords"" content=""{Keywords}""/>");

			if (!string.IsNullOrEmpty(Og_SiteName))
				sb1.Append($@"
	<meta property=""og:site_name"" content=""{Og_SiteName}""/>");

			if (!string.IsNullOrEmpty(Og_Title))
				sb1.Append($@"
	<meta property=""og:title"" content=""{Og_Title}""/>");

			if (!string.IsNullOrEmpty(Og_Description))
				sb1.Append($@"
	<meta property=""og:description"" content=""{Og_Description}""/>");

			if (!string.IsNullOrEmpty(Og_Image))
				sb1.Append($@"
	<meta property=""og:image"" content=""{Og_Image}""/>");

			if (!string.IsNullOrEmpty(Og_Url))
				sb1.Append($@"
	<meta property=""og:url"" content=""{Og_Url}""/>");

			if (!string.IsNullOrEmpty(Og_Type))
				sb1.Append($@"
	<meta property=""og:type"" content=""{Og_Type}""/>");

			return sb1.ToString().ToHtml();
		}

	}

}
