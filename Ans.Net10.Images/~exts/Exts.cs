// rev 2026-10-11

using Ans.Net10.Common;
using ImageMagick;

namespace Ans.Net10.Images
{

	/// <summary>
	/// Методы расширения для обработки, масштабирования, кадрирования
	/// и сохранения графических объектов библиотеки Magick.NET.
	/// </summary>
	public static partial class Exts
	{

		/* functions */


		/// <summary>
		/// Возвращает инициализированный экземпляр хелпера масштабирования 
		/// на основе текущих геометрических размеров изображения.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <returns>
		/// Новый объект <see cref="ImageResizeHelper"/> с заполненными параметрами ширины и высоты.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		public static ImageResizeHelper GetResizer(
			this MagickImage image)
		{
			ArgumentNullException.ThrowIfNull(image);
			return new ImageResizeHelper(image.Width, image.Height);
		}


		/// <summary>
		/// Пропорционально масштабирует изображение таким образом, чтобы оно гарантированно 
		/// и полностью вписалось внутрь указанной прямоугольной рамки.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="width">
		/// Максимально допустимая ширина целевой рамки в пикселях.
		/// </param>
		/// <param name="height">
		/// Максимально допустимая высота целевой рамки в пикселях.
		/// </param>
		/// <param name="noIncrease">
		/// Если установлено значение <see langword="true"/>, исходно маленькое изображение 
		/// не будет увеличиваться (апскейлиться), если размеры рамки больше самого изображения.
		/// </param>
		/// <param name="useMaskBackground">
		/// Признак необходимости наложения шахматной текстуры (CHECKERBOARD) в качестве подложки 
		/// под изображение для визуального обозначения областей прозрачности.
		/// </param>
		/// <returns>
		/// Новый объект измененного изображения, реализующий интерфейс <see cref="IMagickImage"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		public static IMagickImage GetResizeInside(
			this MagickImage image,
			uint width,
			uint height,
			bool noIncrease,
			bool useMaskBackground)
		{
			ArgumentNullException.ThrowIfNull(image);
			var r1 = image.GetResizer();
			r1.ScaleInside(width, height, noIncrease);
			var clone1 = (MagickImage)image.CloneArea(
				new MagickGeometry(0, 0, image.Width, image.Height));
			clone1.Resize(r1.NewWidth, r1.NewHeight);
			if (useMaskBackground)
			{
				using var background = new MagickImage("pattern:CHECKERBOARD");
				clone1.Tile(background, CompositeOperator.DstOver);
			}
			return clone1;
		}


		/// <summary>
		/// Пропорционально масштабирует изображение так, чтобы оно полностью заполнило 
		/// указанную прямоугольную рамку снаружи (подготовка под последующее кадрирование).
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="width">
		/// Целевая ширина рамки кадрирования в пикселях.
		/// </param>
		/// <param name="height">
		/// Целевая высота рамки кадрирования в пикселях.
		/// </param>
		/// <returns>
		/// Новый объект измененного изображения, реализующий интерфейс <see cref="IMagickImage"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		public static IMagickImage GetResizeAround(
			this MagickImage image,
			uint width,
			uint height)
		{
			ArgumentNullException.ThrowIfNull(image);
			var r1 = image.GetResizer();
			r1.ScaleAround(width, height);
			var clone1 = (MagickImage)image.CloneArea(
				new MagickGeometry(0, 0, image.Width, image.Height));
			clone1.Resize(r1.NewWidth, r1.NewHeight);
			return clone1;
		}


		/// <summary>
		/// Масштабирует изображение нелинейным методом усреднения пропорций вокруг заданной стороны квадрата.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="side">
		/// Размер целевой стороны условного квадрата в пикселях.
		/// </param>
		/// <returns>
		/// Новый объект измененного изображения, реализующий интерфейс <see cref="IMagickImage"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		public static IMagickImage GetResizeAverage(
			this MagickImage image,
			uint side)
		{
			ArgumentNullException.ThrowIfNull(image);
			var r1 = image.GetResizer();
			r1.ScaleAverage(side);
			var clone1 = (MagickImage)image.CloneArea(
				new MagickGeometry(0, 0, image.Width, image.Height));
			clone1.Resize(r1.NewWidth, r1.NewHeight);
			return clone1;
		}


		/// <summary>
		/// Выполняет последовательные операции изменения геометрического размера изображения 
		/// и последующего точечного кадрирования (обрезки) по заданным координатам.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="width">
		/// Новая целевая ширина изображения перед операцией кадрирования в пикселях.
		/// </param>
		/// <param name="height">
		/// Новая целевая высота изображения перед операцией кадрирования в пикселях.
		/// </param>
		/// <param name="cropX">
		/// Стартовая координата обрезки по оси X (горизонтальное смещение от левого края).
		/// </param>
		/// <param name="cropY">
		/// Стартовая координата обрезки по оси Y (вертикальное смещение от верхнего края).
		/// </param>
		/// <param name="cropWidth">
		/// Итоговая ширина вырезаемой области (рамки кропа) в пикселях.
		/// </param>
		/// <param name="cropHeight">
		/// Итоговая высота вырезаемой области (рамки кропа) в пикселях.
		/// </param>
		/// <returns>
		/// Новый объект кадрированного изображения, реализующий интерфейс <see cref="IMagickImage"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		public static IMagickImage GetResizeAndCrop(
			this MagickImage image,
			uint width,
			uint height,
			uint cropX,
			uint cropY,
			uint cropWidth,
			uint cropHeight)
		{
			ArgumentNullException.ThrowIfNull(image);
			using var сlone1 = (MagickImage)image.CloneArea(
				new MagickGeometry(0, 0, image.Width, image.Height));
			сlone1.Resize(width, height);
			return сlone1.CloneArea(
				new MagickGeometry((int)cropX, (int)cropY, cropWidth, cropHeight));
		}


		/// <summary>
		/// Изменяет размер изображения так, чтобы оно заполнило рамку снаружи, 
		/// а затем выполняет кадрирование по строго заданным координатам.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="width">
		/// Целевая ширина рамки масштабирования в пикселях.
		/// </param>
		/// <param name="height">
		/// Целевая высота рамки масштабирования в пикселях.
		/// </param>
		/// <param name="cropX">
		/// Стартовая координата обрезки по оси X.
		/// </param>
		/// <param name="cropY">
		/// Стартовая координата обрезки по оси Y.
		/// </param>
		/// <param name="cropWidth">
		/// Ширина финального кадрированного изображения в пикселях.
		/// </param>
		/// <param name="cropHeight">
		/// Высота финального кадрированного изображения в пикселях.
		/// </param>
		/// <returns>
		/// Измененное и кадрированное изображение в виде <see cref="IMagickImage"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		public static IMagickImage GetResizeAroundAndCrop(
			this MagickImage image,
			uint width,
			uint height,
			uint cropX,
			uint cropY,
			uint cropWidth,
			uint cropHeight)
		{
			ArgumentNullException.ThrowIfNull(image);
			var r1 = image.GetResizer();
			r1.ScaleAround(width, height);
			return image.GetResizeAndCrop(
				r1.NewWidth, r1.NewHeight, cropX, cropY, cropWidth, cropHeight);
		}


		/// <summary>
		/// Изменяет размер изображения так, чтобы оно заполнило рамку снаружи, 
		/// а затем выполняет кадрирование (обрезку) с автоматическим расчетом стартовых координат 
		/// на основе переданных режимов смещения по вертикали и горизонтали.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="width">
		/// Целевая ширина рамки масштабирования в пикселях.
		/// </param>
		/// <param name="height">
		/// Целевая высота рамки масштабирования в пикселях.
		/// </param>
		/// <param name="cropWidth">
		/// Ширина финального кадрированного изображения в пикселях.
		/// </param>
		/// <param name="cropHeight">
		/// Высота финального кадрированного изображения in пикселях.
		/// </param>
		/// <param name="shiftVertical">
		/// Вариант фиксированного вертикального смещения рамки кропа (вверх, центр, вниз).
		/// </param>
		/// <param name="shiftHorizontal">
		/// Вариант фиксированного горизонтального смещения рамки кропа (влево, центр, вправо).
		/// </param>
		/// <returns>
		/// Измененное и кадрированное изображение в виде <see cref="IMagickImage"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		public static IMagickImage GetResizeAroundAndCrop(
			this MagickImage image,
			uint width,
			uint height,
			uint cropWidth,
			uint cropHeight,
			ImageShiftEnum shiftVertical,
			ImageShiftEnum shiftHorizontal)
		{
			ArgumentNullException.ThrowIfNull(image);
			var r1 = image.GetResizer();
			r1.ScaleAround(width, height);
			var cropX1 = ImageResizeHelper.GetCropStart(
				r1.NewWidth, cropWidth, shiftHorizontal);
			var cropY1 = ImageResizeHelper.GetCropStart(
				r1.NewHeight, cropHeight, shiftVertical);
			return image.GetResizeAndCrop(
				r1.NewWidth, r1.NewHeight, cropX1, cropY1, cropWidth, cropHeight);
		}


		/* methods */


		/// <summary>
		/// Пропорционально масштабирует изображение внутрь заданной прямоугольной рамки 
		/// и сохраняет полученный результат в файл на диске с указанием уровня качества.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="filename">
		/// Абсолютный или относительный путь к создаваемому файлу на диске.
		/// </param>
		/// <param name="width">
		/// Максимально допустимая ширина целевой рамки в пикселях.
		/// </param>
		/// <param name="height">
		/// Максимально допустимая высота целевой рамки в пикселях.
		/// </param>
		/// <param name="quality">
		/// Уровень сжатия/качества результирующего изображения (от 1 до 100).
		/// </param>
		/// <param name="noIncrease">
		/// Если установлено значение <see langword="true"/>, исходно маленькое изображение не будет увеличиваться.
		/// </param>
		/// <param name="useMaskBackground">
		/// Признак необходимости наложения шахматной текстуры под прозрачные области.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Вызывается, если путь <paramref name="filename"/> пуст или равен <see langword="null"/>.
		/// </exception>
		public static void SaveResizeInside(
			this MagickImage image,
			string filename,
			uint width,
			uint height,
			uint quality,
			bool noIncrease,
			bool useMaskBackground)
		{
			ArgumentNullException.ThrowIfNull(image);
			ArgumentException.ThrowIfNullOrEmpty(filename);
			using var copy1 = image.GetResizeInside(
				width, height, noIncrease, useMaskBackground);
			copy1.Quality = quality;
			copy1.Write(filename);
		}


		/// <summary>
		/// Пропорционально масштабирует изображение так, чтобы оно полностью заполнило указанную рамку снаружи, 
		/// и сохраняет полученный результат в файл на диске с указанием уровня качества.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="filename">
		/// Абсолютный или относительный путь к создаваемому файлу на диске.
		/// </param>
		/// <param name="width">
		/// Целевая ширина рамки в пикселях.
		/// </param>
		/// <param name="height">
		/// Целевая высота рамки в пикселях.
		/// </param>
		/// <param name="quality">
		/// Уровень сжатия/качества результирующего изображения (от 1 до 100).
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Вызывается, если путь <paramref name="filename"/> пуст или равен <see langword="null"/>.
		/// </exception>
		public static void SaveResizeAround(
			this MagickImage image,
			string filename,
			uint width,
			uint height,
			uint quality)
		{
			ArgumentNullException.ThrowIfNull(image);
			ArgumentException.ThrowIfNullOrEmpty(filename);
			using var copy1 = image.GetResizeAround(width, height);
			copy1.Quality = quality;
			copy1.Write(filename);
		}


		/// <summary>
		/// Масштабирует изображение методом усреднения пропорций вокруг заданной стороны квадрата 
		/// и сохраняет полученный результат в файл на диске с указанием уровня качества.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="filename">
		/// Абсолютный или относительный путь к создаваемому файлу на диске.
		/// </param>
		/// <param name="side">
		/// Размер целевой стороны условного квадрата в пикселях.
		/// </param>
		/// <param name="quality">
		/// Уровень сжатия/качества результирующего изображения (от 1 до 100).
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Вызывается, если путь <paramref name="filename"/> пуст или равен <see langword="null"/>.
		/// </exception>
		public static void SaveResizeAverage(
			this MagickImage image,
			string filename,
			uint side,
			uint quality)
		{
			ArgumentNullException.ThrowIfNull(image);
			ArgumentException.ThrowIfNullOrEmpty(filename);
			using var copy1 = image.GetResizeAverage(side);
			copy1.Quality = quality;
			copy1.Write(filename);
		}


		/// <summary>
		/// Изменяет размер изображения так, чтобы оно заполнило рамку снаружи, выполняет кадрирование 
		/// по строго заданным координатам и сохраняет итоговый результат в файл на диске.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="filename">
		/// Абсолютный или относительный путь к создаваемому файлу на диске.
		/// </param>
		/// <param name="width">
		/// Целевая ширина рамки масштабирования в пикселях.
		/// </param>
		/// <param name="height">
		/// Целевая высота рамки масштабирования в пикселях.
		/// </param>
		/// <param name="cropX">
		/// Стартовая координата обрезки по оси X.
		/// </param>
		/// <param name="cropY">
		/// Стартовая координата обрезки по оси Y.
		/// </param>
		/// <param name="cropWidth">
		/// Ширина финального кадрированного изображения в пикселях.
		/// </param>
		/// <param name="cropHeight">
		/// Высота финального кадрированного изображения в пикселях.
		/// </param>
		/// <param name="quality">
		/// Уровень сжатия/качества результирующего изображения (от 1 до 100).
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Вызывается, если путь <paramref name="filename"/> пуст или равен <see langword="null"/>.
		/// </exception>
		public static void SaveResizeAroundAndCrop(
			this MagickImage image,
			string filename,
			uint width,
			uint height,
			uint cropX,
			uint cropY,
			uint cropWidth,
			uint cropHeight,
			uint quality)
		{
			ArgumentNullException.ThrowIfNull(image);
			ArgumentException.ThrowIfNullOrEmpty(filename);
			using var copy1 = image.GetResizeAroundAndCrop(
				width, height, cropX, cropY, cropWidth, cropHeight);
			copy1.Quality = quality;
			copy1.Write(filename);
		}


		/// <summary>
		/// Изменяет размер изображения так, чтобы оно заполнило рамку снаружи, выполняет кадрирование 
		/// на основе режимов смещения и сохраняет итоговый результат в файл на диске.
		/// </summary>
		/// <param name="image">
		/// Исходное изображение Magick.NET.
		/// </param>
		/// <param name="filename">
		/// Абсолютный или относительный путь к создаваемому файлу на диске.
		/// </param>
		/// <param name="width">
		/// Целевая ширина рамки масштабирования в пикселях.
		/// </param>
		/// <param name="height">
		/// Целевая высота рамки масштабирования в пикселях.
		/// </param>
		/// <param name="cropWidth">
		/// Ширина финального кадрированного изображения в пикселях.
		/// </param>
		/// <param name="cropHeight">
		/// Высота финального кадрированного изображения в пикселях.
		/// </param>
		/// <param name="shiftVertical">
		/// Вариант фиксированного вертикального смещения рамки кропа.
		/// </param>
		/// <param name="shiftHorizontal">
		/// Вариант фиксированного горизонтального смещения рамки кропа.
		/// </param>
		/// <param name="quality">
		/// Уровень сжатия/качества результирующего изображения (от 1 до 100).
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="image"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Вызывается, если путь <paramref name="filename"/> пуст или равен <see langword="null"/>.
		/// </exception>
		public static void SaveResizeAroundAndCrop(
			this MagickImage image,
			string filename,
			uint width,
			uint height,
			uint cropWidth,
			uint cropHeight,
			ImageShiftEnum shiftVertical,
			ImageShiftEnum shiftHorizontal,
			uint quality)
		{
			ArgumentNullException.ThrowIfNull(image);
			ArgumentException.ThrowIfNullOrEmpty(filename);
			using var copy1 = image.GetResizeAroundAndCrop(
				width, height, cropWidth, cropHeight, shiftVertical, shiftHorizontal);
			copy1.Quality = quality;
			copy1.Write(filename);
		}


		/// <summary>
		/// Безопасно вычисляет перцептивный хэш изображения. 
		/// В случае повреждения структуры файла или невозможности рассчитать хэш, 
		/// возвращает null вместо генерации исключения.
		/// </summary>
		/// <param name="image">Исходное изображение Magick.NET.</param>
		/// <returns>Строковое представление визуального хэша или null.</returns>
		public static string? GetPerceptualHashSafe(
			this MagickImage image)
		{
			if (image == null)
				return null;
			try
			{
				var pHash1 = image.PerceptualHash();
				return pHash1?.ToString();
			}
			catch (Exception)
			{
				return null;
			}
		}


		/// <summary>
		/// Вычисляет степень визуального различия между двумя хэшами (расстояние Хэмминга).
		/// Чем ближе результат к 0, тем более похожи изображения.
		/// </summary>
		/// <param name="hash1">Перцептивный хэш первого изображения.</param>
		/// <param name="hash2">Перцептивный хэш второго изображения.</param>
		/// <returns>
		/// Числовое значение разницы. 
		/// 0 — полные дубликаты; 
		/// до 5–10 — картинки похожи (ресайз, вотермарка, сильное сжатие); 
		/// больше 15 — изображения визуально разные.
		/// </returns>
		public static double ComparePerceptualDifference(
			string hash1,
			string hash2)
		{
			if (string.IsNullOrEmpty(hash1) || string.IsNullOrEmpty(hash2))
				return double.MaxValue;
			try
			{
				var ph1 = new PerceptualHash(hash1);
				var ph2 = new PerceptualHash(hash2);
				return ph1.SumSquaredDistance(ph2);
			}
			catch (Exception)
			{
				return double.MaxValue;
			}
		}

	}

}
