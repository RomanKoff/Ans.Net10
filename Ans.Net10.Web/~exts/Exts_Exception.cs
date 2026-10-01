// rev 2026-010-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для класса <see cref="Exception"/>, обеспечивающие перехват 
	/// и обработку специфических исключений баз данных (MSSQL, PostgreSQL) на уровне валидации MVC.
	/// </summary>
	public static partial class Exts_Exception
	{

		/// <summary>
		/// Проверяет исключение на наличие ограничений уникальности MSSQL (UNIQUE KEY). 
		/// В случае совпадения добавляет соответствующую ошибку в контекст валидации.
		/// </summary>
		/// <param name="exception">Текущий экземпляр анализируемого исключения.</param>
		/// <param name="modelState">Словарь состояния модели MVC для записи ошибок валидации.</param>
		/// <param name="fieldName">Системное имя свойства (поля формы), для которого проверяется уникальность.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если исключение вызвано нарушением уникальности ключа; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="modelState"/> или <paramref name="fieldName"/> равен <see langword="null"/>.
		/// </exception>
		public static bool TestUpdateUniqueMSSQL(
			this Exception exception,
			ModelStateDictionary modelState,
			string fieldName)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			ArgumentNullException.ThrowIfNull(fieldName);
			if (!exception.TestContains("UNIQUE KEY"))
				return false;
			if (exception.TestContains($"_{fieldName}'."))
				modelState.AddModelError(
					fieldName, Common.Resources.Form.Text_RequiresAUniqueValue);
			else
				modelState.AddModelError(
					string.Empty, Common.Resources.Form.Text_SuchAnObjectExists);
			return true;
		}


		/// <summary>
		/// Проверяет исключение на наличие ошибки дублирования ключа в PostgreSQL (код psql 23505) 
		/// для конкретного поля формы.
		/// </summary>
		/// <remarks>
		/// Метод анализирует имя уникального индекса базы данных. Если имя индекса содержит 
		/// суффикс поля в формате <c>_fieldName"</c>, ошибка привязывается к конкретному инпуту.
		/// </remarks>
		/// <param name="exception">Текущий экземпляр анализируемого исключения.</param>
		/// <param name="modelState">Словарь состояния модели MVC для записи ошибок валидации.</param>
		/// <param name="fieldName">Системное имя свойства (поля формы), для которого проверяется уникальность.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если исключение вызвано нарушением уникальности psql; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="modelState"/> или <paramref name="fieldName"/> равен <see langword="null"/>.
		/// </exception>
		public static bool TestUpdateUniquePSQL(
			this Exception exception,
			ModelStateDictionary modelState,
			string fieldName)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			ArgumentNullException.ThrowIfNull(fieldName);
			if (!exception.TestStartsWith("23505: "))
				return false;
			// psql синтаксис ошибок уникальности обычно оканчивается на: violates unique constraint "IX_Name_FieldName"
			if (exception.TestContains($"_{fieldName}\""))
				modelState.AddModelError(
					fieldName, Common.Resources.Form.Text_RequiresAUniqueValue);
			else
				modelState.AddModelError(
					string.Empty, Common.Resources.Form.Text_SuchAnObjectExists);
			return true;
		}


		/// <summary>
		/// Проверяет исключение на наличие ошибки дублирования ключа в PostgreSQL (код psql 23505).
		/// </summary>
		/// <remarks>
		/// Обрабатывает кейсы вида: IX_TableName_MasterPtr_Ptr нарушает уникальное ограничение.
		/// </remarks>
		/// <param name="exception">Текущий экземпляр анализируемого исключения.</param>
		/// <param name="modelState">Словарь состояния модели MVC для записи ошибок валидации.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если зафиксирована ошибка дублирования ключа psql; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="modelState"/> равен <see langword="null"/>.</exception>
		public static bool TestDuplicateKeyPSQL(
			this Exception exception,
			ModelStateDictionary modelState)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			if (exception.TestStartsWith("23505: "))
			{
				modelState.AddModelError(
					string.Empty, Common.Resources.Form.Text_RequiresAUniqueValue);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Проверяет исключение на ограничение удаления внешней зависимости (код нарушений внешнего ключа psql 23503).
		/// </summary>
		/// <remarks>
		/// Используется для предотвращения удаления записей, на которые всё еще ссылаются другие объекты в БД.
		/// </remarks>
		/// <param name="exception">Текущий экземпляр анализируемого исключения.</param>
		/// <param name="modelState">Словарь состояния модели MVC для записи ошибок валидации.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если зафиксирована ошибка нарушения внешнего ключа при удалении/обновлении; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="modelState"/> равен <see langword="null"/>.</exception>
		public static bool TestDeleteReferencePSQL(
			this Exception exception,
			ModelStateDictionary modelState)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			if (exception.TestStartsWith("23503: "))
			{
				modelState.AddModelError(
					string.Empty, Common.Resources.Form.Text_ObjectContainsData);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Проверяет исключение на нарушение ограничений проверки PostgreSQL (CHECK constraint, код psql 23514).
		/// </summary>
		/// <remarks>
		/// Если имя ограничения содержит имя поля в формате <c>_fieldName</c>, ошибка привязывается к конкретному инпуту.
		/// </remarks>
		/// <param name="exception">Текущий экземпляр анализируемого исключения.</param>
		/// <param name="modelState">Словарь состояния модели MVC для записи ошибок валидации.</param>
		/// <param name="fieldName">Системное имя свойства (поля формы), для которого проверяется ограничение.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если зафиксирована ошибка нарушения CHECK constraint; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="modelState"/> или <paramref name="fieldName"/> равен <see langword="null"/>.
		/// </exception>
		public static bool TestCheckConstraintPSQL(
			this Exception exception,
			ModelStateDictionary modelState,
			string fieldName)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			ArgumentNullException.ThrowIfNull(fieldName);
			if (!exception.TestStartsWith("23514: "))
				return false;
			if (exception.TestContains($"_{fieldName}"))
				modelState.AddModelError(
					fieldName, Common.Resources.Form.Text_ValueDoesNotFit);
			else
				modelState.AddModelError(
					string.Empty, Common.Resources.Form.Text_ValueDoesNotFit);
			return true;
		}


		/// <summary>
		/// Проверяет исключение на наличие ошибок превышения длины строк (код psql 22001) 
		/// или неверного формата данных (код psql 22P02).
		/// </summary>
		/// <param name="exception">Текущий экземпляр анализируемого исключения.</param>
		/// <param name="modelState">Словарь состояния модели MVC для записи ошибок валидации.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если зафиксирована ошибка формата данных или переполнения; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="modelState"/> равен <see langword="null"/>.</exception>
		public static bool TestDataFormatErrorPSQL(
			this Exception exception,
			ModelStateDictionary modelState)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			if (exception.TestStartsWith("22001: ")
				|| exception.TestStartsWith("22P02: "))
			{
				modelState.AddModelError(
					string.Empty, Common.Resources.Form.Text_ValueDoesNotFit);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Проверяет исключение на нарушение ограничения NOT NULL в PostgreSQL (код psql 23502) 
		/// для конкретного поля формы.
		/// </summary>
		/// <param name="exception">Текущий экземпляр анализируемого исключения.</param>
		/// <param name="modelState">Словарь состояния модели MVC для записи ошибок валидации.</param>
		/// <param name="fieldName">Системное имя свойства (поля формы), для которого проверяется заполненность.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если зафиксирована ошибка пустого значения в NOT NULL колонке; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="modelState"/> или <paramref name="fieldName"/> равен <see langword="null"/>.
		/// </exception>
		public static bool TestNotNullViolationPSQL(
			this Exception exception,
			ModelStateDictionary modelState,
			string fieldName)
		{
			ArgumentNullException.ThrowIfNull(modelState);
			ArgumentNullException.ThrowIfNull(fieldName);
			if (!exception.TestStartsWith("23502: "))
				return false;
			// psql синтаксис: null value in column "FieldName" violates not-null constraint
			if (exception.TestContains($"column \"{fieldName}\""))
				modelState.AddModelError(
					fieldName, Common.Resources.Form.Text_ValueIsRequired);
			else
				modelState.AddModelError(
					string.Empty, Common.Resources.Form.Text_ValueIsRequired);
			return true;
		}

	}

}
