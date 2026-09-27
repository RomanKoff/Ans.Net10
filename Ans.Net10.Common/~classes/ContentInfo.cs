// rev 2026-09-25

using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень категорий (групп) типов контента.
	/// </summary>
	public enum ContentGroupEnum
	{
		/// <summary>
		/// Архивные файлы (например: zip, rar, tar).
		/// </summary>
		Archive,

		/// <summary>
		/// Аудиофайлы (например: mp3, wav, ogg).
		/// </summary>
		Audio,

		/// <summary>
		/// Бинарные и исполняемые файлы (например: exe, dll, bin).
		/// </summary>
		Bin,

		/// <summary>
		/// Файлы исходного кода (например: cs, js, cpp, html).
		/// </summary>
		Code,

		/// <summary>
		/// Документы и офисные форматы (например: docx, pdf, xlsx).
		/// </summary>
		Document,

		/// <summary>
		/// Файлы шрифтов (например: ttf, otf, woff).
		/// </summary>
		Font,

		/// <summary>
		/// Изображения и графические файлы (например: png, jpeg, svg).
		/// </summary>
		Image,

		/// <summary>
		/// Простые текстовые файлы (например: txt, log, csv).
		/// </summary>
		Text,

		/// <summary>
		/// Видеофайлы (например: mp4, mkv, avi).
		/// </summary>
		Video,
	}



	/// <summary>
	/// Предоставляет структурированную информацию о типе контента, 
	/// включая MIME-тип, расширение файла и категорию контента.
	/// </summary>
	public class ContentInfo
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ContentInfo"/> с заданными параметрами метаданных контента.
		/// </summary>
		/// <param name="extension">Расширение файла в нижнем регистре, включая точку (например, <c>".jpg"</c>).</param>
		/// <param name="contentType">Строковое представление стандартного MIME-типа (например, <c>"image/jpeg"</c>).</param>
		/// <param name="group">Группа контента из перечисления <see cref="ContentGroupEnum"/>, к которой относится данный тип.</param>
		/// <param name="isWebImage">Флаг, указывающий, является ли изображение оптимизированным и безопасным для использования в Web (например, png, jpeg, gif, webp). Значение по умолчанию: <see langword="false"/>.</param>
		/// <param name="isJpeg">Флаг, указывающий, является ли изображение форматом JPEG (jpg, jpeg). Значение по умолчанию: <see langword="false"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ContentInfo(
			string extension,
			string contentType,
			ContentGroupEnum group,
			bool isWebImage = false,
			bool isJpeg = false)
		{
			Extension = extension;
			ContentType = contentType;
			Group = group;
			IsImage = group == ContentGroupEnum.Image;
			IsWebImage = isWebImage;
			IsJpeg = isJpeg;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает расширение файла, связанное с этим типом контента.
		/// </summary>
		/// <value>Строковое значение расширения, содержащее ведущую точку.</value>
		public string Extension { get; }


		/// <summary>
		/// Возвращает MIME-тип контента.
		/// </summary>
		/// <value>Строковое представление MIME-типа в формате <c>"тип/подтип"</c>.</value>
		public string ContentType { get; }


		/// <summary>
		/// Возвращает группу контента (категорию файла).
		/// </summary>
		/// <value>Одно из значений перечисления <see cref="ContentGroupEnum"/>.</value>
		public ContentGroupEnum Group { get; }


		/// <summary>
		/// Возвращает значение, указывающее, относится ли текущий контент к группе изображений.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если свойство <see cref="Group"/> имеет значение <see cref="ContentGroupEnum.Image"/>; в противном случае — <see langword="false"/>.
		/// </value>
		public bool IsImage { get; }


		/// <summary>
		/// Возвращает значение, указывающее, является ли изображение стандартным и оптимизированным для использования в Web.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если формат поддерживается веб-браузерами напрямую без плагинов; в противном случае — <see langword="false"/>.
		/// </value>
		public bool IsWebImage { get; }


		/// <summary>
		/// Возвращает значение, указывающее, является ли изображение форматом JPEG.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если текущий контент представляет собой изображение JPEG-формата; в противном случае — <see langword="false"/>.
		/// </value>
		public bool IsJpeg { get; }


		/// <summary>
		/// Возвращает строго типизированный объект заголовка типа медиа, созданный на основе строки <see cref="ContentType"/>.
		/// </summary>
		/// <remarks>
		/// Свойство использует ленивую инициализацию. Если формат строки <see cref="ContentType"/> некорректен или не может быть распознан, 
		/// возвращается дефолтный заголовок общего бинарного потока <c>"application/octet-stream"</c>.
		/// </remarks>
		/// <value>Объект класса <see cref="MediaTypeHeaderValue"/>, используемый в HTTP-заголовках.</value>
		public MediaTypeHeaderValue MediaType
		{
			get
			{
				_mediaType ??= MediaTypeHeaderValue.TryParse(ContentType, out var parsed1)
					? parsed1
					: new MediaTypeHeaderValue("application/octet-stream");
				return _mediaType;
			}
		}
		private MediaTypeHeaderValue? _mediaType;


		/* functions */


		/// <summary>
		/// Возвращает строковое представление текущего объекта информации о контенте с перечислением ключевых параметров.
		/// </summary>
		/// <returns>Форматированная диагностическая строка, содержащая группу, MIME-тип и значения логических флагов контента.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return $"[{Group}] '{ContentType}' IsImage:{IsImage} IsWebImage:{IsWebImage} IsJpeg:{IsJpeg}";
		}

	}

}
