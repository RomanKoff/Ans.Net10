// rev 2026-010-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для работы с метаданными моделей (<see cref="ModelExpression"/>, <see cref="ModelMetadata"/>) 
	/// и словарями состояния валидации (<see cref="ModelStateDictionary"/>).
	/// </summary>
	public static partial class Exts__model
	{

		/* functions */


		/// <summary>
		/// Извлекает строковое значение из выражения модели, адаптированное и безопасное для вывода в веб-интерфейсах.
		/// </summary>
		/// <param name="expression">Текущий экземпляр выражения модели.</param>
		/// <returns>Строковое веб-представление значения модели, либо <see langword="null"/>, если модель не инициализирована.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="expression"/> равен <see langword="null"/>.</exception>
		public static string? GetModelValueString(
			this ModelExpression expression)
		{
			ArgumentNullException.ThrowIfNull(expression);

			var value1 = expression.Model;
			if (value1 == null)
				return null;
			return SuppValues.GetStringForWeb(value1);
		}


		/// <summary>
		/// Возвращает кастомный атрибут ограничения максимальной длины <see cref="MaxLengthAttribute"/>, 
		/// примененный к свойству модели.
		/// </summary>
		/// <param name="data">Метаданные исследуемого свойства модели.</param>
		/// <returns>Экземпляр атрибута <see cref="MaxLengthAttribute"/> или <see langword="null"/>, если ограничение отсутствует.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="data"/> равен <see langword="null"/>.</exception>
		public static MaxLengthAttribute? GetMaxLengthAttribute(
			this ModelMetadata data)
		{
			ArgumentNullException.ThrowIfNull(data);
			return data.ValidatorMetadata
				.OfType<MaxLengthAttribute>()
				.FirstOrDefault();
		}


		/// <summary>
		/// Возвращает кастомный атрибут проверки регулярного выражения <see cref="RegularExpressionAttribute"/>, 
		/// примененный к свойству модели.
		/// </summary>
		/// <param name="data">Метаданные исследуемого свойства модели.</param>
		/// <returns>Экземпляр атрибута <see cref="RegularExpressionAttribute"/> или <see langword="null"/>, если атрибут отсутствует.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="data"/> равен <see langword="null"/>.</exception>
		public static RegularExpressionAttribute? GetRegularExpressionAttribute(
			this ModelMetadata data)
		{
			ArgumentNullException.ThrowIfNull(data);
			return data.ValidatorMetadata
				.OfType<RegularExpressionAttribute>()
				.FirstOrDefault();
		}


		/// <summary>
		/// Возвращает кастомный атрибут валидации числового или строкового диапазона <see cref="RangeAttribute"/>, 
		/// примененный к свойству модели.
		/// </summary>
		/// <param name="data">Метаданные исследуемого свойства модели.</param>
		/// <returns>Экземпляр атрибута <see cref="RangeAttribute"/> или <see langword="null"/>, если ограничение диапазона отсутствует.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="data"/> равен <see langword="null"/>.</exception>
		public static RangeAttribute? GetRangeAttribute(
			this ModelMetadata data)
		{
			ArgumentNullException.ThrowIfNull(data);
			return data.ValidatorMetadata
				.OfType<RangeAttribute>()
				.FirstOrDefault();
		}


		/// <summary>
		/// Возвращает кастомный атрибут обязательности заполнения поля <see cref="RequiredAttribute"/>, 
		/// примененный к свойству модели.
		/// </summary>
		/// <param name="data">Метаданные исследуемого свойства модели.</param>
		/// <returns>Экземпляр атрибута <see cref="RequiredAttribute"/> или <see langword="null"/>, если поле не является обязательным.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="data"/> равен <see langword="null"/>.</exception>
		public static RequiredAttribute? GetRequiredAttribute(
			this ModelMetadata data)
		{
			ArgumentNullException.ThrowIfNull(data);
			return data.ValidatorMetadata
				.OfType<RequiredAttribute>()
				.FirstOrDefault();
		}


		/// <summary>
		/// Возвращает плоский массив строк со списком глобальных ошибок валидации модели (ошибок, не привязанных к конкретным инпутам формы).
		/// </summary>
		/// <param name="modelState">Словарь состояния текущей валидации MVC.</param>
		/// <returns>Массив текстовых сообщений об ошибках. Если ошибки отсутствуют, возвращает пустой массив.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="modelState"/> равен <see langword="null"/>.</exception>
		public static string[] GetModelErrors(
			this ModelStateDictionary modelState)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			return _getErrors(modelState, x => string.IsNullOrEmpty(x.Key));
		}


		/// <summary>
		/// Возвращает плоский массив строк со списком ошибок валидации для конкретного именованного поля формы.
		/// </summary>
		/// <param name="modelState">Словарь состояния текущей валидации MVC.</param>
		/// <param name="name">Системное имя проверяемого свойства (поля модели).</param>
		/// <returns>Массив текстовых сообщений об ошибках для указанного поля. Если ошибки отсутствуют, возвращает пустой массив.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="modelState"/> или <paramref name="name"/> равен <see langword="null"/>.</exception>
		public static string[] GetFieldErrors(
			this ModelStateDictionary modelState,
			string name)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			ArgumentNullException.ThrowIfNull(name);
			return _getErrors(modelState, x => x.Key == name);
		}


		/* privates */


		private static string[] _getErrors(
			ModelStateDictionary modelState,
			Func<KeyValuePair<string, ModelStateEntry?>, bool> func)
		{
			if (modelState.IsValid)
				return [];
			return [.. modelState
				.Where(func)
				.SelectMany(x => x.Value!.Errors)
				.Select(x => x.ErrorMessage)
				.Where(msg => !string.IsNullOrEmpty(msg))];
		}

	}

}
