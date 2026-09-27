// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	public static partial class SuppLangRu
	{

		/// <summary>
		/// Возвращает значение числа типа <see cref="long"/> в текстовой форме (прописью) на русском языке.
		/// </summary>
		/// <remarks>
		/// Алгоритм поддерживает корректное разложение триад до квинтиллионов включительно, 
		/// учитывает знаки для отрицательных чисел, системные граничные состояния (<see cref="long.MinValue"/>), 
		/// а также правила женского рода для разряда тысяч (например, <c>"одна тысяча"</c>, <c>"две тысячи"</c>).
		/// </remarks>
		/// <param name="number">Исходное целое число для преобразования прописью.</param>
		/// <returns>Строка с полным текстом числа прописью в нижнем регистре.</returns>
		public static string GetNumberToText(
			long number)
		{
			if (number == 0)
				return "ноль";
			if (number == long.MinValue)
				return "минус девять квинтиллионов двести двадцать три квадриллиона триста семьдесят два триллиона тридцать шесть миллиардов восемьсот пятьдесят четыре миллиона семьсот семьдесят пять тысяч восемьсот восемь";
			if (number < 0)
				return $"минус {GetNumberToText(Math.Abs(number))}";
			var sb1 = new StringBuilder();
			long quintillions1 = number / 1_000_000_000_000_000_000;
			long quadrillions1 = (number % 1_000_000_000_000_000_000) / 1_000_000_000_000_000;
			long trillions1 = (number % 1_000_000_000_000_000) / 1_000_000_000_000;
			long billions1 = (number % 1_000_000_000_000) / 1_000_000_000;
			long millions1 = (number % 1_000_000_000) / 1_000_000;
			long thousands1 = (number % 1_000_000) / 1_000;
			long remainder1 = number % 1_000;
			if (quintillions1 > 0)
			{
				_appendTriad(sb1, quintillions1, isFeminine: false);
				sb1
					.Append(' ')
					.Append(GetPlural(quintillions1, "квинтиллион", "квинтиллиона", "квинтиллионов"));
			}
			if (quadrillions1 > 0)
			{
				if (sb1.Length > 0)
					sb1.Append(' ');
				_appendTriad(sb1, quadrillions1, isFeminine: false);
				sb1
					.Append(' ')
					.Append(GetPlural(quadrillions1, "квадриллион", "квадриллиона", "квадриллионов"));
			}
			if (trillions1 > 0)
			{
				if (sb1.Length > 0)
					sb1.Append(' ');
				_appendTriad(sb1, trillions1, isFeminine: false);
				sb1
					.Append(' ')
					.Append(GetPlural(trillions1, "триллион", "триллиона", "триллионов"));
			}
			if (billions1 > 0)
			{
				if (sb1.Length > 0)
					sb1.Append(' ');
				_appendTriad(sb1, billions1, isFeminine: false);
				sb1
					.Append(' ')
					.Append(GetPlural(billions1, "миллиард", "миллиарда", "миллиардов"));
			}
			if (millions1 > 0)
			{
				if (sb1.Length > 0)
					sb1.Append(' ');
				_appendTriad(sb1, millions1, isFeminine: false);
				sb1
					.Append(' ')
					.Append(GetPlural(millions1, "миллион", "миллиона", "миллионов"));
			}
			if (thousands1 > 0)
			{
				if (sb1.Length > 0)
					sb1.Append(' ');
				_appendTriad(sb1, thousands1, isFeminine: true); // Используем женский род (одна/две тысячи)
				sb1
					.Append(' ')
					.Append(GetPlural(thousands1, "тысяча", "тысячи", "тысяч"));
			}
			if (remainder1 > 0)
			{
				if (sb1.Length > 0)
					sb1.Append(' ');
				_appendTriad(sb1, remainder1, isFeminine: false);
			}
			return sb1.ToString();
		}


		/// <summary>
		/// Формирует бухгалтерскую финансовую пропись денежной суммы на русском языке на основе типа <see cref="decimal"/>.
		/// </summary>
		/// <remarks>
		/// Сумма автоматически округляется до копеек (2 знака после запятой) по банковскому правилу (округление к ближайшему четному). 
		/// Первая буква итоговой строки всегда переводится в верхний регистр для корректного отображения в финансовых документах и актах.
		/// </remarks>
		/// <param name="amount">Исходная денежная сумма (тип <see cref="decimal"/>).</param>
		/// <returns>Форматированная бухгалтерская строка вида <c>"Сто двадцать пять руб. 56 коп."</c>.</returns>
		public static string GetCurrencyToText(
			decimal amount)
		{
			decimal roundedAmount1 = Math.Round(amount, 2, MidpointRounding.ToEven);
			long rubles1 = (long)Math.Truncate(roundedAmount1);
			int kopecks1 = (int)Math.Abs((roundedAmount1 - rubles1) * 100);
			var rublesText1 = GetNumberToText(rubles1);
			if (string.IsNullOrEmpty(rublesText1))
				return $"Ноль руб. {kopecks1:D2} коп.";
			return string.Create(rublesText1.Length, rublesText1, (span, state) =>
			{
				state.AsSpan().CopyTo(span);
				span[0] = char.ToUpperInvariant(span[0]);
			}) + $" руб. {kopecks1:D2} коп.";
		}


		/* privates */


		private static readonly string[] _UNITS = [
			string.Empty, "один", "два", "три", "четыре",
			"пять", "шесть", "семь", "восемь", "девять" ];

		private static readonly string[] _TEENS = [
			"десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать",
			"пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" ];

		private static readonly string[] _TENS = [
			string.Empty, string.Empty, "двадцать", "тридцать", "сорок",
			"пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" ];

		private static readonly string[] _HUNDREDS = [
			string.Empty, "сто", "двести", "триста", "четыреста",
			"пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" ];



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void _appendTriad(
			StringBuilder sb,
			long num,
			bool isFeminine)
		{
			long h1 = num / 100;
			long t1 = (num % 100) / 10;
			long u1 = num % 10;

			if (h1 > 0)
				sb.Append(_HUNDREDS[h1]);

			if (t1 == 1)
			{
				if (h1 > 0) sb.Append(' ');
				sb.Append(_TEENS[u1]);
				return;
			}

			if (t1 > 1)
			{
				if (h1 > 0) sb.Append(' ');
				sb.Append(_TENS[t1]);
			}

			if (u1 > 0)
			{
				if (h1 > 0 || t1 > 0) sb.Append(' ');
				var s1 = (isFeminine, u1) switch
				{
					(true, 1) => "одна",
					(true, 2) => "две",
					_ => _UNITS[u1]
				};
				sb.Append(s1);
			}
		}

	}

}
