// rev 2026-09-25

using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для работы с механизмами внедрения зависимостей (Dependency Injection).
	/// </summary>
	public static partial class Exts__di
	{

		/// <summary>
		/// Проверяет, зарегистрирована ли служба указанного типа в коллекции дескрипторов служб.
		/// </summary>
		/// <param name="services">Коллекция дескрипторов служб <see cref="IServiceCollection"/>.</param>
		/// <param name="type">Тип проверяемой службы.</param>
		/// <returns>
		/// <see langword="true"/>, если служба указанного типа <paramref name="type"/> зарегистрирована в контейнере; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasService(
			this IServiceCollection services,
			Type type)
		{
			return services.Any(
				x => x.ServiceType == type);
		}

	}

}
