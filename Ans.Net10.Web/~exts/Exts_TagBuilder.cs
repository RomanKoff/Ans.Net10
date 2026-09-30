// rev 2026-09-28

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="TagBuilder"/>, обеспечивающие удобную и безопасную 
	/// манипуляцию HTML-атрибутами, CSS-классами и инлайновыми стилями тегов.
	/// </summary>
	public static partial class Exts_TagBuilder
	{

		/// <summary>
		/// Расширяет значение существующего HTML-атрибута тега, добавляя к нему новое значение через разделитель, 
		/// либо создает атрибут, если он отсутствовал.
		/// </summary>
		/// <param name="tag">Текущий построитель тега.</param>
		/// <param name="name">Системное имя HTML-атрибута (например, <c>"data-id"</c>).</param>
		/// <param name="value">Добавляемое значение атрибута.</param>
		/// <param name="separator">Строка-разделитель между старым и новым значением (например, пробел или точка с запятой).</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если <paramref name="tag"/>, <paramref name="name"/>, <paramref name="value"/> или <paramref name="separator"/> равны <see langword="null"/>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ExpandAttribute(
			this TagBuilder tag,
			string name,
			string value,
			string separator)
		{
			ArgumentNullException.ThrowIfNull(tag);
			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentNullException.ThrowIfNull(value);
			ArgumentNullException.ThrowIfNull(separator);
			if (tag.Attributes.TryGetValue(name, out var current1) && !string.IsNullOrEmpty(current1))
				tag.Attributes[name] = string.Concat(current1, separator, value);
			else
				tag.Attributes[name] = value;
		}


		/// <summary>
		/// Добавляет CSS-класс к тегу. Если у тега уже есть классы, новый добавляется через пробел.
		/// </summary>
		/// <param name="tag">Текущий построитель тега.</param>
		/// <param name="value">Имя добавляемого CSS-класса (или несколько классов через пробел).</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="tag"/> или <paramref name="value"/> равны <see langword="null"/>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ExpandClassAttribute(
			this TagBuilder tag,
			string value)
		{
			tag.ExpandAttribute("class", value, " ");
		}


		/// <summary>
		/// Добавляет инлайновый CSS-стиль к атрибуту <c>style</c> тега. Если стили уже заданы, новый добавляется через точку с запятой.
		/// </summary>
		/// <param name="tag">Текущий построитель тега.</param>
		/// <param name="value">Строка добавляемого CSS-стиля (например, <c>"display:none"</c>).</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="tag"/> или <paramref name="value"/> равны <see langword="null"/>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ExpandStyleAttribute(
			this TagBuilder tag,
			string value)
		{
			tag.ExpandAttribute("style", value, ";");
		}

	}

}
