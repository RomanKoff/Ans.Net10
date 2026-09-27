// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class Exts_String
	{

		/// <summary>
		/// Возвращает левую часть строки до указанного разделительного символа, пропуская заданное количество вхождений с конца строки.
		/// </summary>
		/// <param name="instance">Исходная строка для сегментации. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Искомый символ-разделитель границы сегмента.</param>
		/// <param name="skip">Количество вхождений символа с конца строки, которые необходимо пропустить перед отсечением.</param>
		/// <returns>Левая часть строки до найденного символа разграничения или <see cref="string.Empty"/>, если символ не обнаружен.</returns>
		public static string GetLeftTo(
			this string? instance,
			char find,
			int skip = 0)
		{
			if (string.IsNullOrEmpty(instance) || skip < 0)
				return string.Empty;
			int currentMatch1 = 0;
			for (int i1 = instance.Length - 1; i1 >= 0; i1--)
				if (instance[i1] == find)
				{
					if (currentMatch1 == skip)
						return instance[..i1];
					currentMatch1++;
				}
			return string.Empty;
		}


		/// <summary>
		/// Возвращает левую часть строки до указанной разделительной подстроки, пропуская заданное количество вхождений с конца строки.
		/// </summary>
		/// <param name="instance">Исходная строка для сегментации. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Искомая подстрока-разделитель границы сегмента.</param>
		/// <param name="skip">Количество вхождений подстроки с конца строки, которые необходимо пропустить перед отсечением.</param>
		/// <returns>Левая часть строки до найденного маркера разграничения или <see cref="string.Empty"/>, если маркер не обнаружен.</returns>
		public static string GetLeftTo(
			this string? instance,
			string? find,
			int skip = 0)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(find) || skip < 0)
				return string.Empty;
			int currentMatch1 = 0;
			int searchIndex1 = instance.Length - 1;
			while (searchIndex1 >= 0)
			{
				int matchIndex1 = instance.LastIndexOf(find, searchIndex1, StringComparison.Ordinal);
				if (matchIndex1 == -1)
					return string.Empty;
				if (currentMatch1 == skip)
					return instance[..matchIndex1];
				currentMatch1++;
				searchIndex1 = matchIndex1 - 1;
			}
			return string.Empty;
		}


		/// <summary>
		/// Возвращает усеченную левую часть строки, состоящую из первых <paramref name="count"/> символов.
		/// </summary>
		/// <param name="instance">Исходная строка для обрезки. Допускает значение <see langword="null"/>.</param>
		/// <param name="count">Максимально требуемое количество символов слева.</param>
		/// <returns>Строка, содержащая символы из начала исходной строки, или <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetLeft(
			this string? instance,
			int count)
		{
			if (string.IsNullOrEmpty(instance) || count <= 0)
				return string.Empty;
			return count >= instance.Length ? instance : instance[..count];
		}


		/// <summary>
		/// Возвращает правую часть строки после указанного разделительного символа, пропуская заданное количество вхождений с конца строки.
		/// </summary>
		/// <param name="instance">Исходная строка для сегментации. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Искомый символ-разделитель границы сегмента.</param>
		/// <param name="skip">Количество вхождений символа с конца строки, которые необходимо пропустить перед отсечением.</param>
		/// <returns>Правая часть строки после найденного символа разграничения или <see cref="string.Empty"/>, если символ не обнаружен.</returns>
		public static string GetRightFrom(
			this string? instance,
			char find,
			int skip = 0)
		{
			if (string.IsNullOrEmpty(instance) || skip < 0)
				return string.Empty;
			int currentMatch1 = 0;
			for (int i1 = instance.Length - 1; i1 >= 0; i1--)
				if (instance[i1] == find)
				{
					if (currentMatch1 == skip)
						return instance[(i1 + 1)..];
					currentMatch1++;
				}
			return string.Empty;
		}


		/// <summary>
		/// Возвращает правую часть строки после указанной разделительной подстроки, пропуская заданное количество вхождений с конца строки.
		/// </summary>
		/// <param name="instance">Исходная строка для сегментации. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Искомая подстрока-разделитель границы сегмента.</param>
		/// <param name="skip">Количество вхождений подстроки с конца строки, которые необходимо пропустить перед отсечением.</param>
		/// <returns>Правая часть строки после найденного маркера разграничения или <see cref="string.Empty"/>, если маркер не обнаружен.</returns>
		public static string GetRightFrom(
			this string? instance,
			string? find,
			int skip = 0)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(find) || skip < 0)
				return string.Empty;
			int currentMatch1 = 0;
			int searchIndex1 = instance.Length - 1;
			while (searchIndex1 >= 0)
			{
				int matchIndex1 = instance.LastIndexOf(find, searchIndex1, StringComparison.Ordinal);
				if (matchIndex1 == -1)
					return string.Empty;
				if (currentMatch1 == skip)
					return instance[(matchIndex1 + find.Length)..];
				currentMatch1++;
				searchIndex1 = matchIndex1 - 1;
			}
			return string.Empty;
		}


		/// <summary>
		/// Возвращает усеченную правую часть строки, состоящую из последних <paramref name="count"/> символов.
		/// </summary>
		/// <param name="instance">Исходная строка для обрезки. Допускает значение <see langword="null"/>.</param>
		/// <param name="count">Максимально требуемое количество символов с конца строки.</param>
		/// <returns>Строка, содержащая символы из конца исходной строки, или <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRight(
			this string? instance,
			int count)
		{
			if (string.IsNullOrEmpty(instance) || count <= 0)
				return string.Empty;
			return count >= instance.Length ? instance : instance[^count..];
		}


		/// <summary>
		/// Возвращает правую часть строки, отрезая все, что находится до ПЕРВОГО вхождения символа слева (включая сам символ).
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Символ-разделитель, до которого отсекается левый край текста.</param>
		/// <returns>Оставшаяся правая часть строки после первого совпадения или <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRightSide(
			this string? instance,
			char find)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			int i1 = instance.IndexOf(find);
			return i1 == -1 ? string.Empty : instance[(i1 + 1)..];
		}


		/// <summary>
		/// Возвращает правую часть строки, отрезая все, что находится до ПЕРВОГО вхождения подстроки слева (включая саму подстроку).
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Подстрока-разделитель, до которой отсекается левый край текста.</param>
		/// <returns>Оставшаяся правая часть строки после первого совпадения подстроки или <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRightSide(
			this string? instance,
			string find)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(find))
				return string.Empty;
			int i1 = instance.IndexOf(find);
			return i1 == -1 ? string.Empty : instance[(i1 + find.Length)..];
		}


		/// <summary>
		/// Возвращает правую часть строки БЕЗ указанного количества символов слева (смещение точки начала строки).
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <param name="count">Количество символов, которое необходимо гарантированно отсечь с левого края.</param>
		/// <returns>Оставшаяся часть строки или <see cref="string.Empty"/>, если длина строки меньше или равна <paramref name="count"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRightSide(
			this string? instance,
			int count)
		{
			if (string.IsNullOrEmpty(instance) || count <= 0)
				return string.Empty;
			return instance.Length <= count ? string.Empty : instance[count..];
		}


		/// <summary>
		/// Возвращает левую часть строки, отрезая все, что находится после ПЕРВОГО вхождения символа справа (включая сам символ).
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Символ-разделитель, после которого отсекается правый край текста.</param>
		/// <returns>Оставшаяся левая часть строки до крайнего правого совпадения или <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetLeftSide(
			this string? instance,
			char find)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			int i1 = instance.LastIndexOf(find);
			return i1 == -1 ? string.Empty : instance[..i1];
		}


		/// <summary>
		/// Возвращает левую часть строки, отрезая все, что находится после ПЕРВОГО вхождения подстроки справа (включая саму подстроку).
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <param name="find">Подстрока-разделитель, после которой отсекается правый край текста.</param>
		/// <returns>Оставшаяся левая часть строки до крайнего правого совпадения подстроки или <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetLeftSide(
			this string? instance,
			string find)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(find))
				return string.Empty;
			int i1 = instance.LastIndexOf(find);
			return i1 == -1 ? string.Empty : instance[..i1];
		}


		/// <summary>
		/// Возвращает левую часть строки БЕЗ указанного количества символов справа (усечение хвоста строки).
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <param name="count">Количество символов, которое необходимо отсечь с правого края.</param>
		/// <returns>Оставшаяся часть строки или <see cref="string.Empty"/>, если длина строки меньше или равна <paramref name="count"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetLeftSide(
			this string? instance,
			int count)
		{
			if (string.IsNullOrEmpty(instance) || count <= 0)
				return string.Empty;
			return instance.Length <= count ? string.Empty : instance[0..^count];
		}


		/// <summary>
		/// Сокращает текст до указанной максимальной длины, интеллектуально перенося границу среза так, чтобы не разрывать слова на полуслове. 
		/// Если текст подвергся сокращению, в конец автоматически монтируется многоточие или кастомный маркер.
		/// </summary>
		/// <param name="instance">Исходный текст для сокращения. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxLength">Максимально допустимая суммарная длина результирующей строки, включая размер суффикса.</param>
		/// <param name="ellipsis">Строка-суффикс, добавляемая в конец при обрезке (например, многоточие). По умолчанию равен <c>"…"</c>.</param>
		/// <returns>Интеллектуально сокращенный текст с суффиксом, либо <see cref="string.Empty"/>.</returns>
		public static string GetCropToWords(
			this string? instance,
			int maxLength,
			string ellipsis = "…")
		{
			if (string.IsNullOrEmpty(instance) || maxLength <= 0)
				return string.Empty;
			if (instance.Length <= maxLength)
				return instance;
			int ellipsisLength1 = ellipsis?.Length ?? 0;
			int maxTextLength1 = maxLength - ellipsisLength1;
			if (maxTextLength1 <= 0)
				return instance[..maxLength];
			var span1 = instance.AsSpan();
			int cutIndex1 = maxTextLength1;
			if (char.IsWhiteSpace(span1[cutIndex1]) || char.IsPunctuation(span1[cutIndex1]))
				return string.Concat(span1[..cutIndex1].TrimEnd(), ellipsis);
			while (cutIndex1 > 0)
			{
				char ch1 = span1[cutIndex1 - 1];
				if (char.IsWhiteSpace(ch1) || char.IsPunctuation(ch1))
					break;
				cutIndex1--;
			}
			if (cutIndex1 == 0)
				cutIndex1 = maxTextLength1;
			return string.Concat(span1[..cutIndex1].TrimEnd(), ellipsis);
		}


		/// <summary>
		/// Извлекает подстроку из исходного текста с автоматической интеллектуальной подстановкой маски обрезки по краям результата.
		/// </summary>
		/// <param name="instance">Исходная строка. Допускает значение <see langword="null"/>.</param>
		/// <param name="startIndex">Начальный индекс извлечения подстроки.</param>
		/// <param name="length">Длина извлекаемого фрагмента текста.</param>
		/// <param name="beginCropMask">Маска, подставляемая в самое начало результата, если левый край исходного текста был усечен. Если равен <see langword="null"/> — используется <c>"…"</c>.</param>
		/// <param name="endCropMask">Маска, подставляемая в самый конец результата, если правый край исходного текста был усечен. Если равен <see langword="null"/> — совпадает с <paramref name="beginCropMask"/>.</param>
		/// <returns>Подстрока с масками по обрезанным краям, либо <see cref="string.Empty"/>.</returns>
		public static string GetCrop(
			this string? instance,
			int startIndex,
			int length,
			string? beginCropMask = null,
			string? endCropMask = null)
		{
			if (string.IsNullOrEmpty(instance) || length <= 0 || startIndex >= instance.Length)
				return string.Empty;
			if (startIndex < 0)
				startIndex = 0;
			int len1 = instance.Length;
			bool hasBeginCrop1 = startIndex > 0;
			int len2 = len1 - startIndex;
			bool hasEndCrop1 = len2 > length;
			int len3 = hasEndCrop1 ? length : len2;
			var s1 = instance.AsSpan(startIndex, len3);
			string maskStart1 = beginCropMask ?? "…";
			string maskEnd1 = endCropMask ?? beginCropMask ?? "…";
			return string.Concat(
				hasBeginCrop1 ? maskStart1 : string.Empty,
				s1,
				hasEndCrop1 ? maskEnd1 : string.Empty);
		}

	}

}
