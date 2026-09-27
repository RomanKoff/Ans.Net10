// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class SuppLangRu
	{

		/// <summary>
		/// Возвращает случайную полноценную текстовую панграмму на русском языке, покрывающую все буквы алфавита.
		/// </summary>
		/// <returns>Строка, содержащая случайный полный текстовый семпл.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetSample()
			=> _Consts.GetRandomSampleRu();


		/// <summary>
		/// Возвращает случайную укороченную фразу (средний семпл) на русском языке.
		/// </summary>
		/// <returns>Строка, содержащая случайный средний текстовый семпл.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetSampleSmall()
			=> _Consts.GetRandomSampleSmallRu();


		/// <summary>
		/// Возвращает случайное короткое словосочетание (минимальный текстовый семпл) на русском языке.
		/// </summary>
		/// <returns>Строка, содержащая случайный минимальный текстовый семпл.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetSampleSmaller()
			=> _Consts.GetRandomSampleSmallerRu();

	}

}
