// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень вариантов ориентации графического объекта (изображения) на основе соотношения его сторон.
	/// </summary>
	public enum ImageOrientationEnum
		: int
	{
		/// <summary>
		/// Ориентация не определена или не может быть рассчитана.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Альбомная (горизонтальная) ориентация. Ширина больше высоты.
		/// </summary>
		Landscape = 1,

		/// <summary>
		/// Книжная (вертикальная) ориентация. Высота больше ширины.
		/// </summary>
		Portrait = 2,

		/// <summary>
		/// Квадратное изображение. Ширина равна высоте.
		/// </summary>
		Square = 3
	}



	/// <summary>
	/// Перечень вариантов фиксированного смещения рамки кадрирования (обрезки) относительно сторон изображения.
	/// </summary>
	public enum ImageShiftEnum
		: int
	{
		/// <summary>
		/// Расположение рамки строго по центру (смещение 50%).
		/// </summary>
		Center = 0,

		/// <summary>
		/// Расположение от самого начала стороны (левый или верхний край, смещение 0%).
		/// </summary>
		Start = 1,

		/// <summary>
		/// Смещение рамки ближе к началу стороны (смещение на 25%).
		/// </summary>
		StartMiddle = 2,

		/// <summary>
		/// Смещение рамки ближе к концу стороны (смещение на 75%).
		/// </summary>
		EndMiddle = 3,

		/// <summary>
		/// Расположение от самого конца стороны (правый или нижний край, смещение 100%).
		/// </summary>
		End = 4
	}



	/// <summary>
	/// Вспомогательный класс для расчета новых пропорций, коэффициентов масштабирования, 
	/// а также вычисления стартовых координат для операций кадрирования (обрезки) изображений.
	/// </summary>
	public class ImageResizeHelper
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ImageResizeHelper"/> на основе исходных геометрических размеров изображения.
		/// </summary>
		/// <remarks>
		/// Внутри конструктора автоматически рассчитывается исходный коэффициент пропорций <see cref="Ratio"/>, 
		/// определяется <see cref="Orientation"/> и выставляется признак квазиквадратности <see cref="IsNearSquare"/>.
		/// </remarks>
		/// <param name="width">Исходная ширина изображения в пикселях.</param>
		/// <param name="height">Исходная высота изображения в пикселях.</param>
		public ImageResizeHelper(
			uint width,
			uint height)
		{
			Width = width;
			Height = height;
			Ratio = (float)width / height;
			if (Width == Height)
			{
				IsNearSquare = true;
				Orientation = ImageOrientationEnum.Square;
			}
			else
			{
				IsNearSquare = Ratio < 1.1f && Ratio > 0.9f;
				Orientation = Ratio > 1.0f
					? ImageOrientationEnum.Landscape
					: ImageOrientationEnum.Portrait;
			}
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает исходную ширину изображения в пикселях.
		/// </summary>
		/// <value>Беззнаковое целое число (<see cref="uint"/>).</value>
		public uint Width { get; }


		/// <summary>
		/// Возвращает исходную высоту изображения в пикселях.
		/// </summary>
		/// <value>Беззнаковое целое число (<see cref="uint"/>).</value>
		public uint Height { get; }


		/// <summary>
		/// Возвращает соотношение сторон изображения, рассчитываемое как <c>Width / Height</c>.
		/// </summary>
		/// <value>Число с плавающей запятой одинарной точности (<see cref="float"/>).</value>
		public float Ratio { get; }


		/// <summary>
		/// Возвращает пространственную ориентацию графического объекта.
		/// </summary>
		/// <value>Одно из значений перечисления <see cref="ImageOrientationEnum"/>.</value>
		public ImageOrientationEnum Orientation { get; }


		/// <summary>
		/// Возвращает признак того, что изображение близко к квадратному формату.
		/// </summary>
		/// <remarks>
		/// Возвращает <see langword="true"/>, если коэффициент <see cref="Ratio"/> находится в пределах от <c>0.9f</c> до <c>1.1f</c>.
		/// </remarks>
		/// <value>Значение <see langword="true"/>, если формат близок к квадрату; в противном случае — <see langword="false"/>.</value>
		public bool IsNearSquare { get; }


		/// <summary>
		/// Возвращает рассчитанную новую ширину изображения в пикселях после применения методов масштабирования.
		/// </summary>
		/// <value>Новое беззнаковое целое значение ширины.</value>
		public uint NewWidth { get; private set; }


		/// <summary>
		/// Возвращает рассчитанную новую высоту изображения в пикселях после применения методов масштабирования.
		/// </summary>
		/// <value>Новое беззнаковое целое значение высоты.</value>
		public uint NewHeight { get; private set; }


		/* methods */


		/// <summary>
		/// Масштабирует размеры изображения по заданному линейному коэффициенту.
		/// </summary>
		/// <remarks>
		/// Результаты округляются до целого числа с помощью хелпера <c>SuppMath.RoundToUInt</c> и сохраняются в <see cref="NewWidth"/> и <see cref="NewHeight"/>.
		/// </remarks>
		/// <param name="ratio">Коэффициент масштабирования (например, <c>0.5f</c> для уменьшения размеров вдвое).</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Scale(
			float ratio)
		{
			NewWidth = SuppMath.RoundToUInt(Width * ratio);
			NewHeight = SuppMath.RoundToUInt(Height * ratio);
		}


		/// <summary>
		/// Пропорционально масштабирует изображение таким образом, чтобы оно гарантированно и полностью вписалось внутрь указанной прямоугольной рамки.
		/// </summary>
		/// <remarks>
		/// Вычисляется минимальный коэффициент масштабирования между осями <c>Width</c> и <c>Height</c> для сохранения исходных пропорций.
		/// </remarks>
		/// <param name="width">Максимально допустимая ширина целевой рамки.</param>
		/// <param name="height">Максимально допустимая высота целевой рамки.</param>
		/// <param name="noIncrease">
		/// Если установлено значение <see langword="true"/>, то исходно маленькое изображение не будет увеличиваться (апскейлиться), 
		/// если размеры целевой рамки больше размеров самого изображения.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ScaleInside(
			float width,
			float height,
			bool noIncrease)
		{
			float r1 = (noIncrease && width >= Width && height >= Height)
				? 1.0f : Math.Min(width / Width, height / Height);
			Scale(r1);
		}


		/// <summary>
		/// Пропорционально масштабирует изображение так, чтобы оно полностью заполнило указанную прямоугольную рамку снаружи (подготовка под кроп).
		/// </summary>
		/// <remarks>
		/// Вычисляется максимальный коэффициент масштабирования между осями. Часть изображения может выйти за пределы рамки по одной из сторон.
		/// </remarks>
		/// <param name="width">Целевая ширина рамки кадрирования.</param>
		/// <param name="height">Целевая высота рамки кадрирования.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ScaleAround(
			float width,
			float height)
		{
			float r1 = Math.Max(width / Width, height / Height);
			Scale(r1);
		}


		/// <summary>
		/// Масштабирует изображение нелинейным методом усреднения пропорций вокруг заданной стороны квадрата.
		/// </summary>
		/// <param name="side">Размер целевой стороны условного квадрата в пикселях.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ScaleAverage(
			uint side)
		{
			float w1 = side * (float)Width / Height;
			float h1 = side * (float)Height / Width;
			NewWidth = SuppMath.RoundToUInt((side + w1) / 2.0f);
			NewHeight = SuppMath.RoundToUInt((side + h1) / 2.0f);
		}


		/// <summary>
		/// Пропорционально масштабирует изображение до достижения заданной точной ширины. Высота рассчитывается автоматически.
		/// </summary>
		/// <param name="width">Целевая фиксированная ширина в пикселях.</param>
		/// <param name="noIncrease">
		/// Если установлено значение <see langword="true"/>, изображение не будет увеличиваться, если целевая ширина больше исходной <see cref="Width"/>.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ScaleToWidth(
			uint width,
			bool noIncrease)
		{
			float r1 = (noIncrease && width >= Width)
				? 1.0f : (float)width / Width;
			NewWidth = SuppMath.RoundToUInt(Width * r1);
			NewHeight = SuppMath.RoundToUInt(Height * r1);
		}


		/// <summary>
		/// Пропорционально масштабирует изображение до достижения заданной точной высоты. Ширина рассчитывается автоматически.
		/// </summary>
		/// <param name="height">Целевая фиксированная высота в пикселях.</param>
		/// <param name="noIncrease">
		/// Если установлено значение <see langword="true"/>, изображение не будет увеличиваться, если целевая высота больше исходной <see cref="Height"/>.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ScaleToHeight(
			uint height,
			bool noIncrease)
		{
			float r1 = (noIncrease && height >= Height)
				? 1.0f : (float)height / Height;
			NewWidth = SuppMath.RoundToUInt(Width * r1);
			NewHeight = SuppMath.RoundToUInt(Height * r1);
		}


		/* functions */


		/// <summary>
		/// Рассчитывает стартовую координату (в пикселях) по одной из осей для начала операции кадрирования (обрезки) на основе процентного смещения.
		/// </summary>
		/// <remarks>
		/// Значение <paramref name="percentageOfDisplacement"/> принудительно ограничивается диапазоном <c>[0..100]</c> с помощью <c>SuppMath.GetRestrict</c>.
		/// </remarks>
		/// <param name="length">Фактический текущий размер стороны изображения (ширина или высота).</param>
		/// <param name="crop">Требуемый целевой размер рамки обрезки для данной стороны.</param>
		/// <param name="percentageOfDisplacement">Процентное смещение рамки: <c>0</c> — от левого/верхнего края, <c>50</c> — по центру, <c>100</c> — от правого/нижнего края.</param>
		/// <returns>Беззнаковое целое число (<see cref="uint"/>), представляющее пиксель начала обрезки. Если <paramref name="length"/> меньше или равен <paramref name="crop"/>, возвращается <c>0</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint GetCropStart(
			uint length,
			uint crop,
			byte percentageOfDisplacement)
		{
			if (length <= crop)
				return 0;
			uint delta1 = length - crop;
			byte safePercent1 = SuppMath.GetRestrict(percentageOfDisplacement, (byte)0, (byte)100);
			return SuppMath.RoundToUInt(delta1 * (safePercent1 / 100.0f));
		}


		/// <summary>
		/// Рассчитывает стартовую координату (в пикселях) по одной из осей для начала операции кадрирования на основе предопределенного варианта смещения.
		/// </summary>
		/// <param name="length">Фактический текущий размер стороны изображения (ширина или высота).</param>
		/// <param name="crop">Требуемый целевой размер рамки обрезки для данной стороны.</param>
		/// <param name="shift">Вариант смещения рамки, выбранный из перечисления <see cref="ImageShiftEnum"/>.</param>
		/// <returns>Беззнаковое целое число (<see cref="uint"/>), представляющее пиксель начала обрезки. Если <paramref name="length"/> меньше или равен <paramref name="crop"/>, возвращается <c>0</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint GetCropStart(
			uint length,
			uint crop,
			ImageShiftEnum shift)
		{
			if (length <= crop)
				return 0;
			uint delta1 = length - crop;
			float percentage1 = shift switch
			{
				ImageShiftEnum.Start => 0.0f,
				ImageShiftEnum.StartMiddle => 0.25f,
				ImageShiftEnum.EndMiddle => 0.75f,
				ImageShiftEnum.End => 1.0f,
				_ => 0.5f // ImageShiftEnum.Center
			};
			return SuppMath.RoundToUInt(delta1 * percentage1);
		}


		/// <summary>
		/// Формирует строку состояния геометрических размеров графического объекта.
		/// </summary>
		/// <returns>Строка формата <c>"[ИсходнаяШирина:ИсходнаяВысота]->[НоваяШирина:НоваяВысота]"</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return $"[{Width}:{Height}]->[{NewWidth}:{NewHeight}]";
		}

	}

}
