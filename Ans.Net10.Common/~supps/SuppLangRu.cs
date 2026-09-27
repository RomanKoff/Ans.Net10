// rev 2026-09-26

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень основных грамматических падежей русского языка.
	/// </summary>
	public enum WordCasesRuEnum
	{
		/// <summary>
		/// Именительный падеж (Кто? Что?).
		/// </summary>
		Nominative,

		/// <summary>
		/// Родительный падеж (Кого? Чего?).
		/// </summary>
		Genitive,

		/// <summary>
		/// Дательный падеж (Кому? Чему?).
		/// </summary>
		Dative,

		/// <summary>
		/// Винительный падеж (Кого? Что?).
		/// </summary>
		Accusative,

		/// <summary>
		/// Творительный падеж (Кем? Чем?).
		/// </summary>
		Instrumental,

		/// <summary>
		/// Предложный падеж (О ком? О чём?).
		/// </summary>
		Prepositional
	}



	/// <summary>
	/// Вспомогательный класс для работы с русским языком, включая определение пола, морфологию и плюрализацию.
	/// </summary>
	public static partial class SuppLangRu
	{

		/// <summary>
		/// Возвращает вычисленный биологический пол человека на основе экспресс-анализа характерных суффиксов и окончаний его отчества, фамилии или национальных маркеров ФИО.
		/// </summary>
		/// <param name="fullname">Строка, содержащая полный текст ФИО, фамилию с инициалами, отдельное отчество или восточные именные маркеры (оглы/кызы).</param>
		/// <returns>
		/// Определенное значение из перечисления <see cref="GenderEnum"/>, либо <see cref="GenderEnum.NotSpecified"/>, если по имеющимся признакам пол установить не удалось.
		/// </returns>
		/// <example>
		/// <code>
		/// SuppLangRu.GetGender("Пушкин Александр Сергеевич"); // Возвращает GenderEnum.Male
		/// SuppLangRu.GetGender("Эфендиев Эльчин Ильяс оглы"); // Возвращает GenderEnum.Male
		/// SuppLangRu.GetGender("Ахмедова Лейла кызы");         // Возвращает GenderEnum.Female
		/// </code>
		/// </example>
		public static GenderEnum GetGender(
			string fullname)
		{
			if (string.IsNullOrWhiteSpace(fullname))
				return GenderEnum.NotSpecified;
			var span1 = fullname.AsSpan().Trim();
			if (span1.EndsWith("ич", StringComparison.OrdinalIgnoreCase))
				return GenderEnum.Male;
			if (span1.EndsWith("на", StringComparison.OrdinalIgnoreCase))
				return GenderEnum.Female;
			if (span1.EndsWith("глы", StringComparison.OrdinalIgnoreCase))
				return GenderEnum.Male;
			if (span1.EndsWith("ызы", StringComparison.OrdinalIgnoreCase))
				return GenderEnum.Female;
			return GenderEnum.NotSpecified;
		}

	}

}
