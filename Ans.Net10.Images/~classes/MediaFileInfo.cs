using Ans.Net10.Common;
using ImageMagick;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Images
{

	/// <summary>
	/// Перечень стандартных категорий адаптивных размеров графических объектов (изображений).
	/// </summary>
	public enum ImageSizeEnum
		: int
	{
		/// <summary>
		/// Размер не определен или неизвестен.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Экстра-малый размер (ориентир базовой стороны до 256 пикселей).
		/// </summary>
		Extrasmall = 1,

		/// <summary>
		/// Малый размер (ориентир базовой стороны до 576 пикселей).
		/// </summary>
		Small = 2,

		/// <summary>
		/// Средний размер (ориентир базовой стороны до 768 пикселей).
		/// </summary>
		Medium = 3,

		/// <summary>
		/// Большой размер (ориентир базовой стороны до 992 пикселей).
		/// </summary>
		Large = 4,

		/// <summary>
		/// Экстра-большой размер (ориентир базовой стороны от 1200 пикселей).
		/// </summary>
		Extralarge = 5
	}



	/// <summary>
	/// Предоставляет комплексную структурированную информацию о медиафайле, 
	/// включая его MIME-тип, параметры контейнера и геометрические характеристики изображений.
	/// </summary>
	public class MediaFileInfo
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="MediaFileInfo"/> на основе 
		/// существующего информационного объекта файла.
		/// </summary>
		/// <param name="info">
		/// Информационный объект целевого файла системы <see cref="System.IO.FileInfo"/>.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="info"/> равен <see langword="null"/>.
		/// </exception>
		public MediaFileInfo(
			FileInfo info)
		{
			ArgumentNullException.ThrowIfNull(info);
			FileInfo = info;
			Length = info.Length;
			ContentInfo = SuppIO.GetContentInfoFromExtension(Extension);
			Mimetype = ContentInfo.ContentType;
			Format = string.Empty;
			if (ContentInfo.IsWebImage)
			{
				try
				{
					MagickImageInfo = new MagickImageInfo(info.FullName);
					Format = MagickImageInfo.Format.ToString().ToLowerInvariant();
					var inferredMime1 = $"image/{Format}";
					if (Mimetype == "image/jpeg" && inferredMime1 != "image/jpeg")
					{
						Mimetype = Common._Consts.CONTENTINFO_BIN.ContentType;
						IsInvalidImage = true;
					}
					else
					{
						IsWebImage = true;
						IsJpeg = Format == "jpeg";
						Mimetype = inferredMime1;
						ResizeHelper = new ImageResizeHelper(MagickImageInfo.Width, MagickImageInfo.Height);
						_calcSize();
					}
				}
				catch (Exception)
				{
					Mimetype = Common._Consts.CONTENTINFO_BIN.ContentType;
					IsInvalidImage = true;
				}
			}
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="MediaFileInfo"/> по указанному пути к файлу.
		/// </summary>
		/// <param name="path">
		/// Абсолютный или относительный путь к исследуемому файлу на диске.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Вызывается, если параметр <paramref name="path"/> пуст или содержит некорректные символы.
		/// </exception>
		public MediaFileInfo(
			string path)
			: this(new FileInfo(path))
		{
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системный информационный объект файла.
		/// </summary>
		public FileInfo FileInfo { get; }


		/// <summary>
		/// Возвращает метаданные типа контента, определенные по расширению файла.
		/// </summary>
		public ContentInfo ContentInfo { get; }


		/// <summary>
		/// Возвращает низкоуровневые метаданные заголовка изображения движка Magick.NET. 
		/// Равен <see langword="null"/>, если файл не является валидным веб-изображением.
		/// </summary>
		public MagickImageInfo? MagickImageInfo { get; }


		/// <summary>
		/// Возвращает хелпер расчёта пропорций и изменения размеров изображения. 
		/// Равен <see langword="null"/>, если файл не является веб-изображением.
		/// </summary>
		public ImageResizeHelper? ResizeHelper { get; }


		/// <summary>
		/// Возвращает размер файла в байтах.
		/// </summary>
		public long Length { get; }


		/// <summary>
		/// Возвращает строго определенный MIME-тип контента (например, "image/webp").
		/// </summary>
		public string Mimetype { get; private set; }


		/// <summary>
		/// Возвращает строковое представление формата графического файла в нижнем регистре (например, "png", "webp").
		/// </summary>
		public string Format { get; private set; }


		/// <summary>
		/// Возвращает признак того, является ли файл стандартным оптимизированным изображением для Web.
		/// </summary>
		public bool IsWebImage { get; private set; }


		/// <summary>
		/// Возвращает признак того, относится ли текущее изображение к формату JPEG.
		/// </summary>
		public bool IsJpeg { get; private set; }


		/// <summary>
		/// Возвращает признак того, что файл задекларирован как изображение, но его структура повреждена или невалидна.
		/// </summary>
		public bool IsInvalidImage { get; private set; }


		/// <summary>
		/// Возвращает максимально доступную категорию адаптивного размера для текущего изображения.
		/// </summary>
		public ImageSizeEnum MaxSize { get; private set; }


		/// <summary>
		/// Возвращает числовой индекс максимального адаптивного размера изображения (от 1 до 5).
		/// </summary>
		public int SizeIndex { get; private set; }


		/// <summary>
		/// Возвращает признак возможности генерации миниатюры малого размера (Small).
		/// </summary>
		public bool HasSmall { get; private set; }


		/// <summary>
		/// Возвращает признак возможности генерации миниатюры среднего размера (Medium).
		/// </summary>
		public bool HasMedium { get; private set; }


		/// <summary>
		/// Возвращает признак возможности генерации миниатюры большого размера (Large).
		/// </summary>
		public bool HasLarge { get; private set; }


		/// <summary>
		/// Возвращает признак возможности генерации миниатюры экстра-большого размера (Extralarge).
		/// </summary>
		public bool HasExtralarge { get; private set; }


		/// <summary>
		/// Возвращает полный абсолютный путь к файлу.
		/// </summary>
		public string FullName
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FileInfo.FullName;
		}


		/// <summary>
		/// Возвращает имя файла с расширением.
		/// </summary>
		public string Name
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FileInfo.Name;
		}


		/// <summary>
		/// Возвращает чистое имя файла без расширения.
		/// </summary>
		public string NameWithoutExtension
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Path.GetFileNameWithoutExtension(Name);
		}


		/// <summary>
		/// Возвращает расширение файла (включая ведущую точку).
		/// </summary>
		public string Extension
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FileInfo.Extension;
		}


		/// <summary>
		/// Возвращает полный путь к каталогу, в котором расположен файл.
		/// </summary>
		public string DirectoryPath
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => FileInfo.DirectoryName ?? string.Empty;
		}


		/// <summary>
		/// Возвращает фактическую ширину изображения в пикселях. Если файл не является изображением, возвращает 0.
		/// </summary>
		public uint Width
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ResizeHelper?.Width ?? 0;
		}


		/// <summary>
		/// Возвращает фактическую высоту изображения в пикселях. Если файл не является изображением, возвращает 0.
		/// </summary>
		public uint Height
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ResizeHelper?.Height ?? 0;
		}


		/// <summary>
		/// Возвращает пространственную ориентацию графического объекта на основе соотношения сторон.
		/// </summary>
		public ImageOrientationEnum Orientation
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ResizeHelper?.Orientation ?? ImageOrientationEnum.Unknown;
		}


		/// <summary>
		/// Возвращает коэффициент соотношения сторон изображения (Ширина / Высота).
		/// </summary>
		public float Ratio
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ResizeHelper?.Ratio ?? 0f;
		}


		/// <summary>
		/// Возвращает признак того, что геометрическая форма изображения близка к квадрату.
		/// </summary>
		public bool IsNearSquare
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ResizeHelper?.IsNearSquare ?? false;
		}


		/* privates */


		private void _calcSize()
		{
			var minDimension1 = Math.Min(Width, Height);
			MaxSize = ImageSizeEnum.Extralarge;
			SizeIndex = 5;
			HasSmall = true;
			HasMedium = true;
			HasLarge = true;
			HasExtralarge = true;
			if (minDimension1 >= Common._Consts.SIZE_XL)
				return;
			HasExtralarge = false;
			MaxSize = ImageSizeEnum.Large;
			SizeIndex = 4;
			if (minDimension1 >= Common._Consts.SIZE_LG)
				return;
			HasLarge = false;
			MaxSize = ImageSizeEnum.Medium;
			SizeIndex = 3;
			if (minDimension1 >= Common._Consts.SIZE_MD)
				return;
			HasMedium = false;
			MaxSize = ImageSizeEnum.Small;
			SizeIndex = 2;
			if (minDimension1 >= Common._Consts.SIZE_SM)
				return;
			HasSmall = false;
			MaxSize = ImageSizeEnum.Extrasmall;
			SizeIndex = 1;
		}

	}

}
