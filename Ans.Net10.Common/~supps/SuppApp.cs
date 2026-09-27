// rev 2026-09-26

using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс реализующий свойства и методы для получения метаданных и путей текущего приложения.
	/// </summary>
	public static partial class SuppApp
	{

		/* readonly properties */


		/// <summary>
		/// Возвращает главную (входную) сборку приложения.
		/// </summary>
		/// <value>Объект <see cref="Assembly"/>, представляющий исполняемый файл приложения, либо <see langword="null"/>.</value>
		public static Assembly? EntryAssembly
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Assembly.GetEntryAssembly();
		}


		/// <summary>
		/// Возвращает сборку, которая вызвала текущий метод.
		/// </summary>
		/// <value>Объект <see cref="Assembly"/> вызывающей сборки, либо <see langword="null"/>.</value>
		public static Assembly? CallingAssembly
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Assembly.GetCallingAssembly();
		}


		/// <summary>
		/// Возвращает полный путь к текущему рабочему каталогу приложения.
		/// </summary>
		/// <value>Строка, содержащая абсолютный путь к активному каталогу операционной системы.</value>
		public static string CurrentDirectory
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Environment.CurrentDirectory;
		}


		/// <summary>
		/// Возвращает базовый каталог, используемый сборщиком подсистем для поиска сборок (каталог запуска бинарных файлов).
		/// </summary>
		/// <value>Строка, содержащая путь к директории, из которой было запущено приложение.</value>
		public static string BaseDirectory
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => AppDomain.CurrentDomain.BaseDirectory;
		}


		/// <summary>
		/// Возвращает предполагаемый путь к каталогу проекта Visual Studio. 
		/// Вычисляется как подъем на 4 уровня вверх от базового каталога сборки. Результат кэшируется.
		/// </summary>
		/// <value>Строка с абсолютным путем к директории проекта (.csproj).</value>
		public static string VSProjectPath
			=> field ??= _safeGetParent(BaseDirectory, 4);


		/// <summary>
		/// Возвращает имя каталога проекта Visual Studio. Результат кэшируется.
		/// </summary>
		/// <value>Строка, содержащая чистое имя папки проекта.</value>
		public static string VSProjectName
			=> field ??= _getLastPathSegment(VSProjectPath);


		/// <summary>
		/// Возвращает предполагаемый путь к каталогу решения (Solution) Visual Studio. 
		/// Вычисляется как подъем на 5 уровней вверх от базового каталога сборки. Результат кэшируется.
		/// </summary>
		/// <value>Строка с абсолютным путем к директории корня решения (.sln).</value>
		public static string VSSolutionPath
			=> field ??= _safeGetParent(BaseDirectory, 5);


		/// <summary>
		/// Возвращает имя каталога решения (Solution) Visual Studio. Результат кэшируется.
		/// </summary>
		/// <value>Строка, содержащая чистое имя корневой папки решения.</value>
		public static string VSSolutionName
			=> field ??= _getLastPathSegment(VSSolutionPath);


		/* privates */


		private static readonly char[] _SEP_DIR_CHARS = ['/', '\\'];
		private static readonly SearchValues<char> _SEP_DIR = SearchValues.Create(_SEP_DIR_CHARS);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string _getLastPathSegment(
		string path)
		{
			if (string.IsNullOrEmpty(path))
				return string.Empty;
			var span1 = path.AsSpan().TrimEnd(_SEP_DIR_CHARS);
			var i1 = span1.LastIndexOfAny(_SEP_DIR);
			if (i1 == -1)
				return span1.ToString();
			var result1 = span1[(i1 + 1)..];
			return result1.Contains(':')
				? string.Empty : result1.ToString();
		}


		private static string _safeGetParent(
			string path,
			int levels)
		{
			if (string.IsNullOrEmpty(path))
				return string.Empty;
			string s1 = path;
			for (int i1 = 0; i1 < levels; i1++)
			{
				string? parent1 = Path.GetDirectoryName(s1);
				if (string.IsNullOrEmpty(parent1))
					return s1;
				s1 = parent1;
			}
			return s1;
		}

	}

}
