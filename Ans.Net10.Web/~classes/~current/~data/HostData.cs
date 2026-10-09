namespace Ans.Net10.Web
{

	public class HostData
	{

		/* ctor */


		public HostData(
			CurrentContext current)
		{
			BaseUrl = current.HttpContext.GetBaseUrl();
			VirtualPath = current.HttpContext.GetVirtualPath();
			ApplicationUrl = $"{BaseUrl}{VirtualPath}";
		}


		/* readonly properties */


		public string BaseUrl { get; }
		public string VirtualPath { get; }
		public string ApplicationUrl { get; }

	}

}
