// rev 2026-09-26

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class Exts_String
	{

		/// <summary>
		/// Преобразует регистр строки, делая её первую букву заглавной (заглавный символ начала предложения).
		/// </summary>
		/// <param name="instance">Исходная строка для преобразования. Допускает значение <see langword="null"/>.</param>
		/// <param name="forcedToLower">Принудительно перевести все остальные символы строки, кроме первого, в нижний регистр.</param>
		/// <param name="cultureInfo">Информация о культуре и языковых стандартах. Если равен <see langword="null"/>, используется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Результирующая строка с первой заглавной буквой, либо <see cref="string.Empty"/>, если исходная строка пуста.</returns>
		public static string GetAsFirstUpper(
			this string? instance,
			bool forcedToLower = false,
			CultureInfo? cultureInfo = null)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			var cultureInfo1 = cultureInfo ?? CultureInfo.InvariantCulture;
			if (instance.Length == 1)
				return char.ToUpper(instance[0], cultureInfo1).ToString();
			return string.Create(
				instance.Length,
				(Src: instance, ForcedToLower: forcedToLower, Culture: cultureInfo1),
				(span1, state1) =>
				{
					var src1 = state1.Src;
					span1[0] = char.ToUpper(src1[0], state1.Culture);
					if (state1.ForcedToLower)
						src1.AsSpan(1).ToLower(span1[1..], state1.Culture);
					else
						src1.AsSpan(1).CopyTo(span1[1..]);
				});
		}


		/// <summary>
		/// Преобразует регистр строки, делая её первую букву строчной (например, для camelCase форматирования идентификаторов).
		/// </summary>
		/// <param name="instance">Исходная строка для преобразования. Допускает значение <see langword="null"/>.</param>
		/// <param name="forcedToUpper">Принудительно перевести все остальные символы строки, кроме первого, в верхний регистр.</param>
		/// <param name="cultureInfo">Информация о культуре и языковых стандартах. Если равен <see langword="null"/>, используется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Результирующая строка с первой строчной буквой, либо <see cref="string.Empty"/>, если исходная строка пуста.</returns>
		public static string GetAsFirstLower(
			this string? instance,
			bool forcedToUpper = false,
			CultureInfo? cultureInfo = null)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			var cultureInfo1 = cultureInfo ?? CultureInfo.InvariantCulture;
			if (instance.Length == 1)
				return char.ToLower(instance[0], cultureInfo1).ToString();
			return string.Create(
				instance.Length,
				(Src: instance, ForcedToUpper: forcedToUpper, Culture: cultureInfo1),
				(span1, state1) =>
				{
					var src1 = state1.Src;
					span1[0] = char.ToLower(src1[0], state1.Culture);
					if (state1.ForcedToUpper)
						src1.AsSpan(1).ToUpper(span1[1..], state1.Culture);
					else
						src1.AsSpan(1).CopyTo(span1[1..]);
				});
		}


		/// <summary>
		/// Преобразует регистр строки, начиная каждое слово с заглавной буквы (Title Case форматирование заголовков).
		/// </summary>
		/// <param name="instance">Исходная строка для преобразования. Допускает значение <see langword="null"/>.</param>
		/// <param name="forcedToLower">Принудительно перевести всю строку в нижний регистр перед применением форматирования заголовков.</param>
		/// <param name="cultureInfo">Информация о культуре и языковых стандартах. Если равен <see langword="null"/>, используется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Строка в регистре Title Case, либо <see cref="string.Empty"/>, если исходная строка пуста.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetAsTitleCase(
			this string? instance,
			bool forcedToLower = false,
			CultureInfo? cultureInfo = null)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			var cultureInfo1 = cultureInfo ?? CultureInfo.InvariantCulture;
			var s1 = forcedToLower
				? instance.ToLower(cultureInfo) : instance;
			return cultureInfo1.TextInfo.ToTitleCase(s1);
		}

	}

}
