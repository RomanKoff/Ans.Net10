// rev 2026-10-21

using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	public partial class CurrentContext
	{

		/* methods */


		/// <summary>
		/// Устанавливает для ключа значение string
		/// </summary>
		public void SetKey(
			string key,
			string value)
		{
			HttpContext.Items[key] = value;
		}


		/// <summary>
		/// Устанавливает для ключа значение int
		/// </summary>
		public void SetKey(
			string key,
			int value)
		{
			HttpContext.Items[key] = value;
		}


		/// <summary>
		/// Устанавливает для ключа значение bool
		/// </summary>
		public void SetKey(
			string key,
			bool value)
		{
			HttpContext.Items[key] = value;
		}


		/// <summary>
		/// Устанавливает для ключа значение DateTime
		/// </summary>
		public void SetKey(
			string key,
			DateTime value)
		{
			HttpContext.Items[key] = value;
		}


		/// <summary>
		/// Устанавливает для ключа значение DateOnly
		/// </summary>
		public void SetKey(
			string key,
			DateOnly value)
		{
			HttpContext.Items[key] = value;
		}


		/// <summary>
		/// Устанавливает для ключа значение TimeOnly
		/// </summary>
		public void SetKey(
			string key,
			TimeOnly value)
		{
			HttpContext.Items[key] = value;
		}


		/// <summary>
		/// Устанавливает для ключа значение IHtmlContent
		/// </summary>
		public void SetKey(
			string key,
			Func<dynamic, IHtmlContent> value)
		{
			HttpContext.Items[key] = HttpContext.GetStringFromRazor(value);
		}


		/// <summary>
		/// Устанавливает для ключа значение 'on'
		/// </summary>
		public void SetKeyOn(
			string key)
		{
			SetKey(key, "on");
		}


		/// <summary>
		/// Устанавливает для ключа значение 'off'
		/// </summary>
		public void SetKeyOff(
			string key)
		{
			SetKey(key, "off");
		}


		/* functions */


		/// <summary>
		/// Проверяет, установлен ли указанный ключ в текстовое значение "on".
		/// </summary>
		/// <param name="key">Строковый ключ для проверки.</param>
		/// <returns><see langword="true"/>, если значение ключа равно "on"; иначе — <see langword="false"/>.</returns>
		public bool IsKeyOn(
			string key)
		{
			return HttpContext.Items[key] is string value1 && value1.Equals("on");
		}


		/// <summary>
		/// Проверяет, что указанный ключ не установлен в текстовое значение "off".
		/// </summary>
		/// <param name="key">Строковый ключ для проверки.</param>
		/// <returns><see langword="true"/>, если ключ отсутствует или его значение не равно "off"; иначе — <see langword="false"/>.</returns>
		public bool IsKeyNotOff(
			string key)
		{
			if (HttpContext.Items[key] is not { } value1)
				return true;
			return value1 is not string value2
				|| !value2.Equals("off", StringComparison.Ordinal);
		}


		/// <summary>
		/// Проверяет, что в контексте запроса установлен хотя бы один из указанных ключей.
		/// </summary>
		/// <param name="keys">Массив проверяемых строковых ключей.</param>
		/// <returns><see langword="true"/>, если найден хотя бы один ключ; иначе — <see langword="false"/>.</returns>
		public bool HasAnyKey(
			params string[] keys)
		{
			if (keys == null || keys.Length == 0)
				return false;
			for (int i1 = 0; i1 < keys.Length; i1++)
				if (HttpContext.Items.ContainsKey(keys[i1]))
					return true;
			return false;
		}


		/// <summary>
		/// Проверяет, что абсолютно все перечисленные ключи присутствуют в контексте запроса.
		/// </summary>
		/// <param name="keys">Массив проверяемых строковых ключей.</param>
		/// <returns><see langword="true"/>, если все до единого ключи найдены в контексте; иначе — <see langword="false"/>.</returns>
		public bool HasAllKeys(
			params string[] keys)
		{
			if (keys is null || keys.Length == 0)
				return false;
			foreach (var key1 in keys.AsSpan())
				if (!HttpContext.Items.ContainsKey(key1))
					return false;
			return true;
		}


		/// <summary>
		/// Возвращает значение ключа, приведенное к типу <see cref="string"/>.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Строковое значение ключа либо переданный <paramref name="defaultValue"/>.</returns>
		public string? GetKey(
			string key,
			string? defaultValue = null)
		{
			return HttpContext.Items[key] is string value1
				? value1
				: defaultValue;
		}


		/// <summary>
		/// Возвращает значение ключа, приведенное к типу <see cref="int"/>.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии или несовпадении типа.</param>
		/// <returns>Целочисленное значение ключа либо переданный <paramref name="defaultValue"/>.</returns>
		public int GetKeyInt(
			string key,
			int defaultValue = 0)
		{
			return HttpContext.Items[key] is int value1
				? value1
				: defaultValue;
		}


		/// <summary>
		/// Возвращает значение ключа, приведенное к логическому типу <see cref="bool"/>.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <returns><see langword="true"/>, если ключ содержит логическое значение <see langword="true"/>; иначе — <see langword="false"/>.</returns>
		public bool GetKeyBool(
			string key)
		{
			return HttpContext.Items[key] is bool value1 && value1;
		}


		/// <summary>
		/// Возвращает значение ключа, приведенное к типу <see cref="DateTime"/>.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Объект даты и времени либо <paramref name="defaultValue"/>.</returns>
		public DateTime? GetKeyDateTime(
			string key,
			DateTime? defaultValue = null)
		{
			return HttpContext.Items[key] is DateTime value1
				? value1
				: defaultValue;
		}


		/// <summary>
		/// Возвращает значение ключа, приведенное к типу <see cref="DateOnly"/>.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Объект даты либо <paramref name="defaultValue"/>.</returns>
		public DateOnly? GetKeyDateOnly(
			string key,
			DateOnly? defaultValue = null)
		{
			return HttpContext.Items[key] is DateOnly value1
				? value1
				: defaultValue;
		}


		/// <summary>
		/// Возвращает значение ключа, приведенное к типу <see cref="TimeOnly"/>.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Объект времени либо <paramref name="defaultValue"/>.</returns>
		public TimeOnly? GetKeyTimeOnly(
			string key,
			TimeOnly? defaultValue = null)
		{
			return HttpContext.Items[key] is TimeOnly value1
				? value1
				: defaultValue;
		}

	}

}
