// rev 2026-09-27

using Ans.Net10.Common;

namespace Ans.Net10.Psql
{

	/// <summary>
	/// Предоставляет информацию о текущей вызывающей сборке библиотеки.
	/// </summary>
	public static class LibPsqlInfo
	{
		public static string Name => SuppApp.CallingAssembly.Name;
		public static string Version => SuppApp.CallingAssembly.Version;
		public static string FullVersion => SuppApp.CallingAssembly.FullVersion;
		public static string? Description => SuppApp.CallingAssembly.Description;
	}

}
