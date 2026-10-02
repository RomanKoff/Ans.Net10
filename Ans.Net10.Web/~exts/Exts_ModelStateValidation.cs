// rev 2026-10-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Предоставляет статические методы расширения для словаря состояний моделей <see cref="ModelStateDictionary"/>, 
	/// упрощающие выполнение сложной бизнес-валидации полей форм ввода с использованием скомпилированных регулярных выражений ядра Common.
	/// </summary>
	public static partial class Exts_ModelStateValidation
	{

		/// <summary>
		/// Выполняет строгую проверку строкового значения поля на соответствие каноническому системному имени переменной в стиле языка C#.
		/// </summary>
		/// <param name="modelState">Словарь состояний текущей валидации MVC-модели.</param>
		/// <param name="fieldName">Системное программное имя свойства (поля формы), для которого выполняется проверка.</param>
		/// <param name="value">Анализируемое строковое значение переменной, полученное из пользовательского ввода.</param>
		/// <returns>Значение <see langword="true"/>, если строка успешно прошла валидацию; в противном случае — <see langword="false"/> (ошибка автоматически пишется в ModelState).</returns>
		public static bool ValidateVarnameStrict(
			this ModelStateDictionary modelState,
			string fieldName,
			string? value)
		{
			if (string.IsNullOrEmpty(value) || !SuppRegex.G_REGEX_VARNAME_STRICT().IsMatch(value))
			{
				modelState.AddModelError(
					fieldName,
					Common.Resources.Form.Text_ValueDoesNotFit);
				return false;
			}
			return true;
		}


		/// <summary>
		/// Выполняет комплексную строгую валидацию адреса электронной почты на соответствие международным стандартам именования ящиков.
		/// </summary>
		/// <param name="modelState">Словарь состояний текущей валидации MVC-модели.</param>
		/// <param name="fieldName">Системное программное имя свойства (поля формы), для которого выполняется проверка.</param>
		/// <param name="value">Анализируемое строковое значение email-адреса.</param>
		/// <returns>Значение <see langword="true"/>, если адрес корректен; в противном случае — <see langword="false"/>.</returns>
		public static bool ValidateEmailStrict(
			this ModelStateDictionary modelState,
			string fieldName,
			string? value)
		{
			if (string.IsNullOrEmpty(value) || !SuppRegex.G_REGEX_EMAIL_STRICT().IsMatch(value))
			{
				modelState.AddModelError(
					fieldName,
					Common.Resources.Form.Text_ValueDoesNotFit);
				return false;
			}
			return true;
		}


		/// <summary>
		/// Выполняет строгую валидацию имени объекта на вхождение только разрешенных безопасных символов (строчные латинские буквы, цифры, дефис, точка).
		/// </summary>
		/// <param name="modelState">Словарь состояний текущей валидации MVC-модели.</param>
		/// <param name="fieldName">Системное программное имя свойства (поля формы).</param>
		/// <param name="value">Анализируемое строковое значение имени.</param>
		/// <returns>Значение <see langword="true"/>, если имя безопасно и валидно; в противном случае — <see langword="false"/>.</returns>
		public static bool ValidateNameStrict(
			this ModelStateDictionary modelState,
			string fieldName,
			string? value)
		{
			if (string.IsNullOrEmpty(value) || !SuppRegex.G_REGEX_NAME_STRICT().IsMatch(value))
			{
				modelState.AddModelError(
					fieldName,
					Common.Resources.Form.Text_ValueDoesNotFit);
				return false;
			}
			return true;
		}


		/// <summary>
		/// Выполняет валидацию строки на соответствие формату пути внутренних иерархических идентификаторов ресурсов (например, "root/subnode/resource").
		/// </summary>
		/// <param name="modelState">Словарь состояний текущей валидации MVC-модели.</param>
		/// <param name="fieldName">Системное программное имя свойства (поля формы).</param>
		/// <param name="value">Анализируемая строка пути.</param>
		/// <returns>Значение <see langword="true"/>, если структура пути верна; в противном случае — <see langword="false"/>.</returns>
		public static bool ValidateIpathStrict(
			this ModelStateDictionary modelState,
			string fieldName,
			string? value)
		{
			if (string.IsNullOrEmpty(value) || !SuppRegex.G_REGEX_IPATH_STRICT().IsMatch(value))
			{
				modelState.AddModelError(
					fieldName,
					Common.Resources.Form.Text_ValueDoesNotFit);
				return false;
			}
			return true;
		}


		/// <summary>
		/// Выполняет поиск и валидацию структуры стандартного сетевого адреса протокола IPv4.
		/// </summary>
		/// <param name="modelState">Словарь состояний текущей валидации MVC-модели.</param>
		/// <param name="fieldName">Системное программное имя свойства (поля формы).</param>
		/// <param name="value">Анализируемая строка IP-адреса.</param>
		/// <returns>Значение <see langword="true"/>, если строка является валидным IPv4 адресом; в противном случае — <see langword="false"/>.</returns>
		public static bool ValidateIp4(
			this ModelStateDictionary modelState,
			string fieldName,
			string? value)
		{
			if (string.IsNullOrEmpty(value) || !SuppRegex.G_REGEX_IP4().IsMatch(value))
			{
				modelState.AddModelError(
					fieldName,
					Common.Resources.Form.Text_ValueDoesNotFit);
				return false;
			}
			return true;
		}

	}

}
