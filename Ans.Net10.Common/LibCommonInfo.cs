// rev 2026-09-26

namespace Ans.Net10.Common
{

	/// <summary>
	/// Предоставляет информацию о текущей вызывающей сборке библиотеки.
	/// </summary>
	public static class LibCommonInfo
	{
		public static string Name => SuppApp.CallingAssembly.Name;
		public static string Version => SuppApp.CallingAssembly.Version;
		public static string FullVersion => SuppApp.CallingAssembly.FullVersion;
		public static string? Description => SuppApp.CallingAssembly.Description;
	}

}
