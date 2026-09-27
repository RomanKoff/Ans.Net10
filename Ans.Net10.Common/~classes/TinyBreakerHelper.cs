// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Легковесный счетчик-прерыватель итераций для периодического выполнения кода внутри тяжелых циклов.
	/// </summary>
	/// <param name="step">Количество шагов (итераций) между срабатываниями триггера.</param>
	public class TinyBreakerHelper(
		int step)
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TinyBreakerHelper"/>
		/// со значением шага по умолчанию (1234).
		/// </summary>
		public TinyBreakerHelper()
			: this(1234)
		{
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает заданный шаг (интервал) срабатывания прерывателя.
		/// </summary>
		/// <value>
		/// Количество итераций, через которое метод <see cref="Next"/> возвращает <see langword="true"/>.
		/// </value>
		public int Step { get; } = step;


		/// <summary>
		/// Возвращает текущее количество накопленных итераций.
		/// </summary>
		/// <value>
		/// Текущий счетчик шагов от 0 до <see cref="Step"/> минус 1.
		/// </value>
		public int Current { get; private set; }


		/* functions */


		/// <summary>
		/// Фиксирует следующую итерацию. Возвращает <see langword="true"/>,
		/// если достигнут заданный шаг интервала.
		/// </summary>
		/// <remarks>
		/// При каждой фиксации внутренний счетчик <see cref="Current"/> увеличивается на 1. 
		/// При достижении значения <see cref="Step"/> счетчик автоматически сбрасывается в 0, 
		/// а метод возвращает значение <see langword="true"/>.
		/// </remarks>
		/// <returns>
		/// Значение <see langword="true"/>, если текущий интервал шагов пройден и достигнут лимит; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Next()
		{
			Current++;
			if (Current < Step)
				return false;
			Current = 0;
			return true;
		}

	}

}
