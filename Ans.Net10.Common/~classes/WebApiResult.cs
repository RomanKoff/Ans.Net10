// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Представляет конечный результат выполнения запроса к Web API, расширяющий базовый результат.
	/// </summary>
	/// <typeparam name="T">Тип ожидаемых полезных данных ответа.</typeparam>
	public class WebApiResult<T>
		: _WebResult_Base<T>
	{
	}

}
