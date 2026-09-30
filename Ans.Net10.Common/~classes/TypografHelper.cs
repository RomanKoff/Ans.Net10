// rev 2026-09-28

using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Построитель-помощник для разбора текста и применения правил типографики к текстовым блокам, 
	/// изолируя HTML/XML-теги от обработки.
	/// </summary>
	public class TypografHelper
	{

		private readonly StringBuilder _result = new();


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TypografHelper"/> и запускает итеративный разбор текста.
		/// </summary>
		/// <param name="source">Исходный текст, содержащий HTML/XML разметку или чистый текст.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="source"/> равен <see langword="null"/>.</exception>
		public TypografHelper(
			string source)
		{
			ArgumentNullException.ThrowIfNull(source);
			Source = source;
			_parseText();
			Result = _result.ToString();
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает исходный текст.
		/// </summary>
		public string Source { get; }


		/// <summary>
		/// Возвращает результирующий оттипографленный текст.
		/// </summary>
		public string Result { get; }


		/* privates */


		/// <summary>
		/// Итеративно сканирует строку, разделяя ее на теги и контент для безопасной обработки.
		/// </summary>
		private void _parseText()
		{
			int current1 = 0;
			int length1 = Source.Length;
			while (current1 < length1)
			{
				int tagStart1 = Source.IndexOf('<', current1);
				if (tagStart1 == -1)
				{
					_result.Append(SuppTypograph.GetTypografElem(Source[current1..]));
					break;
				}
				if (tagStart1 > current1)
					_result.Append(SuppTypograph.GetTypografElem(Source[current1..tagStart1]));
				int tagEnd1 = Source.IndexOf('>', tagStart1);
				if (tagEnd1 == -1)
				{
					_result.Append(Source[tagStart1..]);
					break;
				}
				tagEnd1++;
				_result.Append(Source[tagStart1..tagEnd1]);
				current1 = tagEnd1;
			}
		}

	}

}
