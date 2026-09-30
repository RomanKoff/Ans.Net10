// rev 2026-09-29

namespace Ans.Net10.Web
{

	/// <summary>
	/// Унифицированная модель результата выполнения операции Web API (DTO).
	/// </summary>
	/// <remarks>
	/// Используется для возврата стандартизированных ответов из контроллеров или конечных точек API,
	/// содержащих статус операции, текстовое описание и связанные параметры.
	/// </remarks>
	public class ApiResultModel
	{

		/* consts */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="ApiResultModel"/>.
		/// </summary>
		/// <remarks>
		/// Данный конструктор необходим для работы встроенных механизмов десериализации JSON/XML.
		/// </remarks>
		public ApiResultModel()
		{
			Result = string.Empty;
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ApiResultModel"/> с указанием итогового результата.
		/// </summary>
		/// <param name="result">Строковое представление результата или статуса операции.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="result"/> равен <see langword="null"/>.
		/// </exception>
		public ApiResultModel(
			string result)
			: this()
		{
			ArgumentNullException.ThrowIfNull(result);
			Result = result;
		}


		/* properties */


		/// <summary>
		/// Получает или задает строковое значение результата или статуса выполненной операции.
		/// </summary>
		/// <value>
		/// По умолчанию инициализируется как <see cref="string.Empty"/>. Не должно принимать значение <see langword="null"/>.
		/// </value>
		/// <example><c>"success"</c>, <c>"error"</c>, <c>"42"</c></example>
		public string Result { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает заголовок, краткое описание или сообщение, сопровождающее результат.
		/// </summary>
		/// <value>
		/// Текст сообщения или <see langword="null"/>, если описание отсутствует.
		/// </value>
		public string? Title { get; set; }


		/// <summary>
		/// Получает или задает дополнительные параметры, метаданные или контекст ответа в строковом виде.
		/// </summary>
		/// <value>
		/// Сериализованная строка параметров или <see langword="null"/>, если дополнительные данные отсутствуют.
		/// </value>
		public string? Params { get; set; }

	}

}
