// rev 2026-09-30

using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace Ans.Net10.Web
{

	/*
	Пример использования:
	Загрузит, автоматически вызовет .Test() на валидацию и сохранит в кэш.
	var webOptions = AppSettingsFactory.GetOptions<LibWebOptions>(builder.Configuration);
	*/



	/// <summary>
	/// Интерфейс для строго типизированных параметров конфигурации приложения.
	/// </summary>
	public interface IAppSettings
	{
		/// <summary>
		/// Получает системное имя секции в файле конфигурации.
		/// </summary>
		string SectionName { get; }

		/// <summary>
		/// Выполняет валидацию параметров конфигурации после их загрузки.
		/// </summary>
		void Test();
	}



	/// <summary>
	/// Базовый класс для реализации строго типизированных параметров конфигурации,
	/// загружаемых из файла настроек <c>appsettings.json</c>.
	/// </summary>
	public abstract class _AppSettings_Base
		: IAppSettings
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_AppSettings_Base"/> 
		/// с указанием системного имени секции конфигурации.
		/// </summary>
		/// <param name="sectionName">Системное имя секции в файле настроек.</param>
		protected _AppSettings_Base(
			string sectionName)
		{
			SectionName = sectionName;
		}


		/* virtuals */


		/// <inheritdoc />
		public virtual void Test() { }


		/* readonly properties */


		/// <inheritdoc />
		public string SectionName { get; }


		/* functions */


		/// <summary>
		/// Формирует и возвращает исключение, информирующее об отсутствии обязательного параметра.
		/// </summary>
		/// <param name="paramName">Системное имя пропущенного параметра.</param>
		/// <returns>Новый экземпляр исключения <see cref="Exception"/>.</returns>
		/// <exception cref="ArgumentException">Вызывается, если <paramref name="paramName"/> пуст.</exception>
		public Exception GetExceptionParamRequired(
			string paramName)
		{
			ArgumentException.ThrowIfNullOrEmpty(paramName);
			return new Exception(
				$"[appsettings.json/{SectionName}/{paramName}] is required!");
		}

	}



	/// <summary>
	/// Предоставляет методы для потокобезопасной загрузки, валидации и кэширования 
	/// параметров конфигурации из <see cref="IConfiguration"/>.
	/// </summary>
	public static class AppSettingsFactory
	{

		private static readonly ConcurrentDictionary<string, IAppSettings> _cache = new();


		/* functions */


		/// <summary>
		/// Атомарно загружает секцию конфигурации, приводит её к типу <typeparamref name="T"/>, 
		/// выполняет встроенную валидацию и кэширует результат.
		/// </summary>
		/// <typeparam name="T">Тип класса параметров, реализующий <see cref="IAppSettings"/> и имеющий конструктор по умолчанию.</typeparam>
		/// <param name="configuration">Архитектурный интерфейс конфигурации .NET.</param>
		/// <returns>Полностью инициализированный и проверенный экземпляр класса конфигурации.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="configuration"/> равен <see langword="null"/>.</exception>
		/// <exception cref="Exception">Вызывается, если целевая секция отсутствует в файле настроек или не прошла валидацию.</exception>
		public static T GetOptions<T>(
			IConfiguration configuration)
			where T : class,
			IAppSettings, new()
		{
			ArgumentNullException.ThrowIfNull(configuration);
			var instanceForName1 = new T();
			string key1 = instanceForName1.SectionName;
			var options1 = _cache.GetOrAdd(key1, _ =>
			{
				var boundOptions1 = configuration.GetSection(key1).Get<T>()
					?? throw new Exception(
						$"[appsettings.json/{key1}] section is missing or invalid!");
				boundOptions1.Test();
				return boundOptions1;
			});
			return (T)options1;
		}

	}

}
