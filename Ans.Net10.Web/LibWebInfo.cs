// rev 2026-09-28

using Ans.Net10.Common;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Предоставляет информацию о текущей вызывающей сборке библиотеки.
	/// </summary>
	public static class LibWebInfo
	{
		public static string Name => SuppApp.CallingAssembly.Name;
		public static string Version => SuppApp.CallingAssembly.Version;
		public static string FullVersion => SuppApp.CallingAssembly.FullVersion;
		public static string? Description => SuppApp.CallingAssembly.Description;
	}

}
