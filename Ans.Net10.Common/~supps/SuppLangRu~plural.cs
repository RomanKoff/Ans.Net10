// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class SuppLangRu
	{

		/// <summary>
		/// Возвращает грамматически корректную форму русского существительного в зависимости от числительного.
		/// </summary>
		/// <remarks>
		/// Алгоритм корректно обрабатывает отрицательные значения, а также большие числа типа <see cref="long"/>.
		/// </remarks>
		/// <param name="value">Количество объектов, на основе которого вычисляется флексия (окончание) [10].</param>
		/// <param name="form1">Форма слова в именительном падеже единственного числа для числительных, оканчивающихся на 1, кроме 11 (например, <c>"день"</c>, <c>"яблоко"</c>) [10].</param>
		/// <param name="form2to4">Форма слова в родительном падеже единственного числа для числительных, оканчивающихся на 2–4, кроме 12–14 (например, <c>"дня"</c>, <c>"яблока"</c>) [10].</param>
		/// <param name="form0and5to9and11to14">Форма слова в родительном падеже множественного числа для числительных, оканчивающихся на 0, 5–9, а также от 11 до 14 включительно (например, <c>"дней"</c>, <c>"яблок"</c>) [10].</param>
		/// <returns>Строка, содержащая существительное в правильной грамматической форме, соответствующей числу [10].</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetPlural(
			long value,
			string form1,
			string form2to4,
			string form0and5to9and11to14)
		{
			long absCount1 = Math.Abs(value);
			long remainder100 = absCount1 % 100;
			long remainder10 = absCount1 % 10;
			return remainder100 switch
			{
				>= 11 and <= 14 => form0and5to9and11to14,
				_ => remainder10 switch
				{
					1 => form1,
					>= 2 and <= 4 => form2to4,
					_ => form0and5to9and11to14
				}
			};
		}


		/// <summary>
		/// Возвращает грамматически корректную форму русского существительного в зависимости от числа, подставленную в указанный строковый шаблон.
		/// </summary>
		/// <param name="template">Шаблон форматирования (например, <c>"Осталось {0} {1}"</c>), где параметр <c>{0}</c> — число, а <c>{1}</c> — сформированное слово. Допускает значение <see langword="null"/> [10].</param>
		/// <param name="value">Количество объектов [10].</param>
		/// <param name="form1">Форма слова для числительных, оканчивающихся на 1 (кроме 11) [10].</param>
		/// <param name="form2to4">Форма слова для числительных, оканчивающихся на 2–4 (кроме 12–14) [10].</param>
		/// <param name="form0and5to9and11to14">Форма слова для числительных, оканчивающихся на 0, 5–9 и диапазона 11–14 [10].</param>
		/// <returns>Форматированная по шаблону строка, либо <see cref="string.Empty"/>, если строка шаблона пуста или состоит только из пробелов [10].</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetPlural(
			string? template,
			long value,
			string form1,
			string form2to4,
			string form0and5to9and11to14)
		{
			if (string.IsNullOrWhiteSpace(template))
				return string.Empty;
			var s1 = GetPlural(value, form1, form2to4, form0and5to9and11to14);
			return string.Format(template, value, s1);
		}


		/// <summary>
		/// Возвращает грамматически корректную форму возраста (числа лет) человека или объекта в формате "N год/года/лет".
		/// </summary>
		/// <param name="year">Количество полных лет [10].</param>
		/// <returns>Форматированная строка с числовым значением возраста и правильным существительным [10].</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetPluralAge(
			int year)
		{
			return GetPlural("{0} {1}", year, "год", "года", "лет");
		}

	}

}
