// rev 2026-09-19

using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace Ans.Net10.Common.Services
{

	/// <summary>
	/// Интерфейс службы отправки электронных писем.
	/// </summary>
	public interface IMailerService
	{
		/// <summary>
		/// Асинхронно отправляет электронное письмо на основе переданной модели.
		/// </summary>
		/// <param name="message">Модель данных отправляемого сообщения.</param>
		/// <returns>Поток-задача <see cref="Task"/>, представляющая асинхронную операцию отправки.</returns>
		Task SendAsync(MailMessageModel message);
	}



	/// <summary>
	/// Интерфейс параметров конфигурации SMTP-клиента для службы отправки писем.
	/// </summary>
	public interface IMailerServiceOptions
	{
		/// <summary>Сетевой адрес или имя хоста SMTP-сервера.</summary>
		/// <value>Строка с адресом сервера.</value>
		string SmtpServer { get; }

		/// <summary>Сетевой порт SMTP-сервера для подключения.</summary>
		/// <value>Числовое значение порта.</value>
		int SmtpPort { get; }

		/// <summary>Признак использования защищенного SSL/TLS соединения при подключении.</summary>
		/// <value><see langword="true"/>, если требуется SSL; иначе — <see langword="false"/>.</value>
		bool SmtpUseSsl { get; }

		/// <summary>Имя пользователя (логин) для аутентификации на SMTP-сервере.</summary>
		/// <value>Строка с именем пользователя.</value>
		string SmtpUsername { get; }

		/// <summary>Пароль или токен приложения для аутентификации на SMTP-сервере.</summary>
		/// <value>Строка с паролем.</value>
		string SmtpPassword { get; }

		/// <summary>Адрес электронной почты отправителя, используемый по умолчанию.</summary>
		/// <value>Строка с email-адресом.</value>
		string DefaultFromAddress { get; }

		/// <summary>Отображаемое имя (заголовок) отправителя, используемое по умолчанию.</summary>
		/// <value>Строка с именем отправителя.</value>
		string DefaultFromTitle { get; }

		/// <summary>Почтовый адрес для принудительной отправки отладочных скрытых копий писем (CC).</summary>
		/// <value>Строка с email-адресом отладки.</value>
		string DebugCc { get; }
	}



	/// <summary>
	/// Заглушка службы отправки писем для использования в средах разработки или тестирования.
	/// Не выполняет реальную сетевую отправку.
	/// </summary>
	public class FakeMailerService
		: IMailerService
	{
		/// <inheritdoc />
		public Task SendAsync(
			MailMessageModel message)
		{
			return Task.CompletedTask;
		}
	}



	/// <summary>
	/// Реализация службы отправки писем через SMTP-протокол с использованием библиотеки MailKit.
	/// </summary>
	/// <param name="options">Параметры конфигурации подключения к SMTP-серверу.</param>
	public class AnsMailerService(
		IMailerServiceOptions options)
		: IMailerService
	{

		private readonly IMailerServiceOptions _options = options;


		/* functions */


		/// <summary>
		/// Создает объект адреса <see cref="MailboxAddress"/> с поддержкой UTF-8.
		/// </summary>
		/// <param name="title">Отображаемое имя адресата.</param>
		/// <param name="address">Электронный почтовый адрес.</param>
		/// <returns>Готовый объект адреса <see cref="MailboxAddress"/>.</returns>
		public static MailboxAddress GetMailboxAddress(
			string title,
			string address)
		{
			return new MailboxAddress(Encoding.UTF8, title, address);
		}


		/// <summary>
		/// Создает объект адреса <see cref="MailboxAddress"/>, где имя совпадает с электронным адресом.
		/// </summary>
		/// <param name="address">Электронный почтовый адрес.</param>
		/// <returns>Готовый объект адреса <see cref="MailboxAddress"/>.</returns>
		public static MailboxAddress GetMailboxAddress(
			string address)
		{
			return GetMailboxAddress(address, address);
		}


		/// <inheritdoc />
		public async Task SendAsync(
			MailMessageModel message)
		{
			var mimeMessage1 = new MimeMessage();
			mimeMessage1.From.Add(message.From ?? GetMailboxAddress(
				_options.DefaultFromTitle, _options.DefaultFromAddress));
			if (message.To != null)
				mimeMessage1.To.Add(message.To);
			if (message.Cc.Length > 0)
				mimeMessage1.Cc.AddRange(message.Cc);
			if (message.Bcc.Length > 0)
				mimeMessage1.Bcc.AddRange(message.Bcc);
			mimeMessage1.Subject = message.Subject;
			mimeMessage1.Body = new TextPart(TextFormat.Html)
			{
				Text = message.ContentHtml
			};
			using var client1 = new SmtpClient();
			client1.AuthenticationMechanisms.Remove("XOAUTH2");
			try
			{
				await client1.ConnectAsync(
					_options.SmtpServer,
					_options.SmtpPort,
					_options.SmtpUseSsl);
				await client1.AuthenticateAsync(
					_options.SmtpUsername,
					_options.SmtpPassword);
				await client1.SendAsync(mimeMessage1);
			}
			finally
			{
				if (client1.IsConnected)
					await client1.DisconnectAsync(true);
			}
		}

	}

}
