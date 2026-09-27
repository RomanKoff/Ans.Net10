// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для рекурсивного анализа дерева вложенных исключений <see cref="Exception.InnerException"/> и их строковых сообщений.
	/// </summary>
	public static partial class Exts_Exception
	{

		/* functions */


		/// <summary>
		/// Возвращает текстовое сообщение самого глубокого (первичного) исключения в цепочке <see cref="Exception.InnerException"/>.
		/// </summary>
		/// <param name="exception">Исходное исключение, выступающее началом дерева поиска.</param>
		/// <returns>Текст сообщения об ошибке <see cref="Exception.Message"/> корневого исключения.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetExceptionMessage(
			this Exception exception)
		{
			return (exception.InnerException == null)
				? exception.Message
				: exception.InnerException.GetExceptionMessage();
		}


		/// <summary>
		/// Рекурсивно проверяет, содержит ли сообщение текущего или любого вложенного исключения указанную подстроку. 
		/// Сравнение производится в регистронезависимом режиме.
		/// </summary>
		/// <param name="exception">Исходное исключение для проверки.</param>
		/// <param name="value">Искомая текстовая подстрока (например, имя системного сбоя).</param>
		/// <returns>
		/// <see langword="true"/>, если искомая подстрока найдена в свойстве <see cref="Exception.Message"/> 
		/// текущего или любого дочернего исключения; в противном случае — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TestContains(
			this Exception exception,
			string value)
		{
			if (string.IsNullOrEmpty(value))
				return false;
			if (exception.Message.Contains(value, StringComparison.OrdinalIgnoreCase))
				return true;
			return exception.InnerException != null
				&& exception.InnerException.TestContains(value);
		}


		/// <summary>
		/// Рекурсивно проверяет, начинается ли сообщение текущего или любого вложенного исключения с указанной подстроки. 
		/// Сравнение производится в регистронезависимом режиме.
		/// </summary>
		/// <param name="exception">Исходное исключение для проверки.</param>
		/// <param name="value">Искомая подстрока начала сообщения.</param>
		/// <returns>
		/// <see langword="true"/>, если свойство <see cref="Exception.Message"/> хотя бы одного исключения в цепочке 
		/// начинается с указанного текста; в противном случае — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TestStartsWith(
			this Exception exception,
			string value)
		{
			if (string.IsNullOrEmpty(value))
				return false;
			if (exception.Message.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				return true;
			return exception.InnerException != null
				&& exception.InnerException.TestStartsWith(value);
		}

	}

}
