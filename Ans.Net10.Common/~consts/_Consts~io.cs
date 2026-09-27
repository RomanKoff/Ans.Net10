// rev 2026-09-25

using System.Collections.Frozen;

namespace Ans.Net10.Common
{

	/*
	 * SuppCulture.AddCodePagesSupport() || Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
	 */



	public static partial class _Consts
	{

		/// <summary>
		/// Список имен файлов, зарезервированных операционной системой Windows и запрещенных для использования.
		/// </summary>
		public static readonly string[] FORBIDDEN_FILE_NAMES =
		[
			"con", "prn", "aux", "nul",
			"com0", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
			"lpt0", "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9"
		];


		/// <summary>
		/// Дефолтный объект информации о контенте для неопознанных бинарных данных.
		/// </summary>
		public static readonly ContentInfo CONTENTINFO_BIN
			= new("*", "application/octet-stream", ContentGroupEnum.Bin);


		/// <summary>
		/// Замороженный (Frozen) оптимизированный словарь сопоставления расширений файлов и метаданных типов контента.
		/// </summary>
		/// <remarks>
		/// Ключом словаря является расширение файла (обязательно включая ведущую точку, например, <c>".jpg"</c>). 
		/// Поиск по ключам производится в регистронезависимом режиме (<see cref="StringComparer.OrdinalIgnoreCase"/>).
		/// </remarks>
		/// <value>
		/// Оптимизированный для чтения словарь <see cref="FrozenDictionary{TKey, TValue}"/>, сопоставляющий 
		/// строковые расширения с объектами <see cref="ContentInfo"/>.
		/// </value>
		public static FrozenDictionary<string, ContentInfo> CONTENTINFOS => field ??= new ContentInfo[]
		{
			// Archive
			new(".apk", "application/vnd.android.package-archive", ContentGroupEnum.Archive),
			new(".gtar", "application/x-gtar", ContentGroupEnum.Archive),
			new(".gz", "application/x-gzip", ContentGroupEnum.Archive),
			new(".tar", "application/x-tar", ContentGroupEnum.Archive),
			new(".tgz", "application/x-compressed", ContentGroupEnum.Archive),
			new(".z", "application/x-compress", ContentGroupEnum.Archive),
			new(".zip", "application/zip", ContentGroupEnum.Archive),
			// Document
			new(".accdb", "application/msaccess", ContentGroupEnum.Document),
			new(".ai", "application/postscript", ContentGroupEnum.Document),
			new(".doc", "application/msword", ContentGroupEnum.Document),
			new(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ContentGroupEnum.Document),
			new(".dot", "application/msword", ContentGroupEnum.Document),
			new(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template", ContentGroupEnum.Document),
			new(".dvi", "application/x-dvi", ContentGroupEnum.Document),
			new(".eps", "application/postscript", ContentGroupEnum.Document),
			new(".hlp", "application/winhlp", ContentGroupEnum.Document),
			new(".latex", "application/x-latex", ContentGroupEnum.Document),
			new(".mdb", "application/x-msaccess", ContentGroupEnum.Document),
			new(".mpp", "application/vnd.ms-project", ContentGroupEnum.Document),
			new(".pdf", "application/pdf", ContentGroupEnum.Document),
			new(".pot", "application/vnd.ms-powerpoint", ContentGroupEnum.Document),
			new(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template", ContentGroupEnum.Document),
			new(".pps", "application/vnd.ms-powerpoint", ContentGroupEnum.Document),
			new(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow", ContentGroupEnum.Document),
			new(".ppt", "application/vnd.ms-powerpoint", ContentGroupEnum.Document),
			new(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation", ContentGroupEnum.Document),
			new(".ps", "application/postscript", ContentGroupEnum.Document),
			new(".pub", "application/x-mspublisher", ContentGroupEnum.Document),
			new(".rtf", "application/rtf", ContentGroupEnum.Document),
			new(".tex", "application/x-tex", ContentGroupEnum.Document),
			new(".wcm", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wdb", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wks", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wps", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wri", "application/x-mswrite", ContentGroupEnum.Document),
			new(".xla", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xlc", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xlm", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xls", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ContentGroupEnum.Document),
			new(".xlt", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template", ContentGroupEnum.Document),
			new(".xlw", "application/vnd.ms-excel", ContentGroupEnum.Document),
			// Image                        	   
			new(".bmp", "image/bmp", ContentGroupEnum.Image, isWebImage: true),
			new(".cmx", "image/x-cmx", ContentGroupEnum.Image),
			new(".cod", "image/cis-cod", ContentGroupEnum.Image),
			new(".gif", "image/gif", ContentGroupEnum.Image, isWebImage: true),
			new(".ico", "image/x-icon", ContentGroupEnum.Image),
			new(".ief", "image/ief", ContentGroupEnum.Image),
			new(".jfif", "image/pipeg", ContentGroupEnum.Image),
			new(".jpe", "image/jpeg", ContentGroupEnum.Image, isWebImage: true, isJpeg: true),
			new(".jpeg", "image/jpeg", ContentGroupEnum.Image, isWebImage: true, isJpeg: true),
			new(".jpg", "image/jpeg", ContentGroupEnum.Image, isWebImage: true, isJpeg: true),
			new(".pbm", "image/x-portable-bitmap", ContentGroupEnum.Image, isWebImage: true),
			new(".pgm", "image/x-portable-graymap", ContentGroupEnum.Image),
			new(".png", "image/png", ContentGroupEnum.Image, isWebImage: true),
			new(".pnm", "image/x-portable-anymap", ContentGroupEnum.Image),
			new(".ppm", "image/x-portable-pixmap", ContentGroupEnum.Image),
			new(".ras", "image/x-cmu-raster", ContentGroupEnum.Image),
			new(".rgb", "image/x-rgb", ContentGroupEnum.Image),
			new(".svg", "image/svg+xml", ContentGroupEnum.Image, isWebImage: true),
			new(".svgz", "image/svg+xml", ContentGroupEnum.Image, isWebImage: true),
			new(".tif", "image/tiff", ContentGroupEnum.Image, isWebImage: true),
			new(".tiff", "image/tiff", ContentGroupEnum.Image, isWebImage: true),
			new(".wbmp", "image/vnd.wap.wbmp", ContentGroupEnum.Image),
			new(".webp", "image/webp", ContentGroupEnum.Image, isWebImage: true),
			new(".xbm", "image/x-xbitmap", ContentGroupEnum.Image),
			new(".xpm", "image/x-xpixmap", ContentGroupEnum.Image),
			new(".xwd", "image/x-xwindowdump", ContentGroupEnum.Image),
			// Text                         	   
			new(".323", "text/h323", ContentGroupEnum.Text),
			new(".bas", "text/plain", ContentGroupEnum.Text),
			new(".c", "text/plain", ContentGroupEnum.Text),
			new(".cs", "text/plain", ContentGroupEnum.Text),
			new(".cshtml", "text/plain", ContentGroupEnum.Text),
			new(".css", "text/css", ContentGroupEnum.Text),
			new(".csv", "text/csv", ContentGroupEnum.Text),
			new(".etx", "text/x-setext", ContentGroupEnum.Text),
			new(".h", "text/plain", ContentGroupEnum.Text),
			new(".htc", "text/x-component", ContentGroupEnum.Text),
			new(".htm", "text/html", ContentGroupEnum.Text),
			new(".html", "text/html", ContentGroupEnum.Text),
			new(".htt", "text/webviewhtml", ContentGroupEnum.Text),
			new(".js", "application/javascript", ContentGroupEnum.Text),
			new(".json", "application/json", ContentGroupEnum.Text),
			new(".less", "text/css", ContentGroupEnum.Text),
			new(".rss", "application/rss+xml", ContentGroupEnum.Text),
			new(".rtx", "text/richtext", ContentGroupEnum.Text),
			new(".sass", "text/css", ContentGroupEnum.Text),
			new(".scss", "text/css", ContentGroupEnum.Text),
			new(".sct", "text/scriptlet", ContentGroupEnum.Text),
			new(".shtml", "text/html", ContentGroupEnum.Text),
			new(".stm", "text/html", ContentGroupEnum.Text),
			new(".tsv", "text/tab-separated-values", ContentGroupEnum.Text),
			new(".txt", "text/plain", ContentGroupEnum.Text),
			new(".uls", "text/iuls", ContentGroupEnum.Text),
			new(".vb", "text/plain", ContentGroupEnum.Text),
			new(".vcf", "text/x-vcard", ContentGroupEnum.Text),
			new(".xml", "application/xml", ContentGroupEnum.Text),
			// Audio                        	   
			new(".aif", "audio/x-aiff", ContentGroupEnum.Audio),
			new(".aifc", "audio/x-aiff", ContentGroupEnum.Audio),
			new(".aiff", "audio/x-aiff", ContentGroupEnum.Audio),
			new(".au", "audio/basic", ContentGroupEnum.Audio),
			new(".flac", "audio/flac", ContentGroupEnum.Audio),
			new(".m3u", "audio/x-mpegurl", ContentGroupEnum.Audio),
			new(".m4a", "audio/mp4", ContentGroupEnum.Audio),
			new(".mid", "audio/mid", ContentGroupEnum.Audio),
			new(".mp3", "audio/mpeg", ContentGroupEnum.Audio),
			new(".ogg", "audio/ogg", ContentGroupEnum.Audio),
			new(".ra", "audio/x-pn-realaudio", ContentGroupEnum.Audio),
			new(".ram", "audio/x-pn-realaudio", ContentGroupEnum.Audio),
			new(".rmi", "audio/mid", ContentGroupEnum.Audio),
			new(".snd", "audio/basic", ContentGroupEnum.Audio),
			new(".wav", "audio/x-wav", ContentGroupEnum.Audio),
			// Video                        	   
			new(".asf", "video/x-ms-asf", ContentGroupEnum.Video),
			new(".asr", "video/x-ms-asf", ContentGroupEnum.Video),
			new(".asx", "video/x-ms-asf", ContentGroupEnum.Video),
			new(".avi", "video/x-msvideo", ContentGroupEnum.Video),
			new(".f4v", "video/mp4", ContentGroupEnum.Video),
			new(".flv", "video/x-flv", ContentGroupEnum.Video),
			new(".lsf", "video/x-la-asf", ContentGroupEnum.Video),
			new(".lsx", "video/x-la-asf", ContentGroupEnum.Video),
			new(".mkv", "video/x-matroska", ContentGroupEnum.Video),
			new(".mov", "video/quicktime", ContentGroupEnum.Video),
			new(".movie", "video/x-sgi-movie", ContentGroupEnum.Video),
			new(".mp2", "video/mpeg", ContentGroupEnum.Video),
			new(".mp4", "video/mp4", ContentGroupEnum.Video),
			new(".mpa", "video/mpeg", ContentGroupEnum.Video),
			new(".mpe", "video/mpeg", ContentGroupEnum.Video),
			new(".mpeg", "video/mpeg", ContentGroupEnum.Video),
			new(".mpg", "video/mpeg", ContentGroupEnum.Video),
			new(".mpv2", "video/mpeg", ContentGroupEnum.Video),
			new(".ogv", "video/ogg", ContentGroupEnum.Video),
			new(".qt", "video/quicktime", ContentGroupEnum.Video),
			new(".webm", "video/webm", ContentGroupEnum.Video),
			// Fonts                        	   
			new(".eot", "application/vnd.ms-fontobject", ContentGroupEnum.Font),
			new(".otf", "font/otf", ContentGroupEnum.Font),
			new(".ttf", "font/ttf", ContentGroupEnum.Font),
			new(".woff", "font/woff", ContentGroupEnum.Font),
			new(".woff2", "font/woff2", ContentGroupEnum.Font)
		}.ToFrozenDictionary(x => x.Extension, StringComparer.OrdinalIgnoreCase);

	}

}
