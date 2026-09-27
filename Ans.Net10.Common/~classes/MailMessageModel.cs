// rev 2026-09-25

using MimeKit;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Модель данных, представляющая полную структуру и метаданные электронного письма
	/// для последующей отправки через сервисы на базе библиотек MimeKit и MailKit.
	/// </summary>
	public class MailMessageModel
	{

		/// <summary>
		/// Получает или задает электронный адрес и отображаемое имя отправителя письма.
		/// </summary>
		/// <value>
		/// Объект <see cref="MailboxAddress"/>, содержащий email-адрес и имя отправителя, 
		/// или <see langword="null"/>, если отправитель должен быть определен автоматически конфигурацией почтового сервиса.
		/// </value>
		public MailboxAddress? From { get; set; }


		/// <summary>
		/// Получает или задает электронный адрес и отображаемое имя главного (прямого) получателя письма.
		/// </summary>
		/// <value>
		/// Объект <see cref="MailboxAddress"/>, содержащий email-адрес и имя получателя, 
		/// или <see langword="null"/>, если адрес назначения еще не задан.
		/// </value>
		public MailboxAddress? To { get; set; }


		/// <summary>
		/// Получает или задает массив адресов получателей копии письма (Carbon Copy, CC).
		/// </summary>
		/// <value>
		/// Массив объектов <see cref="MailboxAddress"/>. По умолчанию инициализируется как пустой массив. 
		/// Свойство гарантированно не возвращает <see langword="null"/>.
		/// </value>
		public MailboxAddress[] Cc { get; set; } = [];


		/// <summary>
		/// Получает или задает массив адресов получателей скрытой копии письма (Blind Carbon Copy, BCC).
		/// </summary>
		/// <value>
		/// Массив объектов <see cref="MailboxAddress"/>. По умолчанию инициализируется как пустой массив. 
		/// Свойство гарантированно не возвращает <see langword="null"/>.
		/// </value>
		public MailboxAddress[] Bcc { get; set; } = [];


		/// <summary>
		/// Получает или задает тему (заголовок) электронного письма.
		/// </summary>
		/// <value>
		/// Строка текста, содержащая тему письма. По умолчанию равна <see cref="string.Empty"/>. 
		/// Не должна принимать значение <see langword="null"/>.
		/// </value>
		public string Subject { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает основное содержимое (тело) электронного письма в формате разметки HTML.
		/// </summary>
		/// <value>
		/// Строка, содержащая HTML-код тела сообщения. По умолчанию равна <see cref="string.Empty"/>. 
		/// Не должна принимать значение <see langword="null"/>.
		/// </value>
		public string ContentHtml { get; set; } = string.Empty;

	}

}
