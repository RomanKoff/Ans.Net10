using Ans.Net10.Common;
using ImageMagick;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Images
{

	/// <summary>
	/// Предоставляет механизмы для управления жизненным циклом графического объекта Magick.NET, 
	/// чтения его EXIF-метаданных и подготовки к операциям модификации.
	/// </summary>
	public class ImageHelper
		: IDisposable
	{

		private bool _disposedValue;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ImageHelper"/> на основе 
		/// метаданных переданного объекта информации о файле.
		/// </summary>
		/// <param name="info">
		/// Объект информации о медиафайле <see cref="MediaFileInfo"/>.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="info"/> равен <see langword="null"/>.
		/// </exception>
		public ImageHelper(
			MediaFileInfo info)
		{
			ArgumentNullException.ThrowIfNull(info);
			Info = info;
			if (Info.IsWebImage)
			{
				Image = new MagickImage(info.FileInfo.FullName);
				if (Info.IsJpeg)
					Exif = Image.GetExifProfile();
			}
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает метаданные и физические характеристики анализируемого файла.
		/// </summary>
		public MediaFileInfo Info { get; }


		/// <summary>
		/// Возвращает живой графический объект движка ImageMagick. 
		/// Равен <see langword="null"/>, если файл не является валидным веб-изображением.
		/// </summary>
		public MagickImage? Image { get; }


		/// <summary>
		/// Возвращает профиль EXIF-метаданных изображения. 
		/// Равен <see langword="null"/>, если у изображения отсутствуют метаданные или формат их не поддерживает.
		/// </summary>
		public IExifProfile? Exif { get; }


		/// <summary>
		/// Возвращает строковое представление тега пространственной ориентации из EXIF-метаданных файла.
		/// </summary>
		public string? ExifOrientation
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Exif?.GetValue(ExifTag.Orientation)?.ToString();
		}


		/// <summary>
		/// Получает или задает строку параметров или инструкций для динамической модификации изображения.
		/// </summary>
		public string? Modify { get; set; }


		/// <summary>
		/// Получает или задает режим фиксированного смещения рамки кадрирования, 
		/// применяемый по умолчанию при автоматическом кропе.
		/// </summary>
		public ImageShiftEnum ShiftDefault { get; set; }


		/* methods */


		/// <summary>
		/// Освобождает управляемые (и при необходимости неуправляемые) ресурсы, используемые объектом.
		/// </summary>
		/// <param name="disposing">
		/// Значение <see langword="true"/> для освобождения как управляемых, так и неуправляемых ресурсов; 
		/// значение <see langword="false"/> для освобождения только неуправляемых ресурсов.
		/// </param>
		protected virtual void Dispose(
			bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
					Image?.Dispose();
				_disposedValue = true;
			}
		}


		/// <summary>
		/// Выполняет очистку ресурсов, закрытие графических контекстов движка и деаллокацию памяти.
		/// </summary>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

	}

}
