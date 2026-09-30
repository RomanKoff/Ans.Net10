using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Web.Services
{

	/*
	 *	LibStartup.Add_AnsNet10Web()
	 *		builder.Services.AddScoped<IViewRenderService, AnsViewRenderService>();
     */



	/// <summary>
	/// Интерфейс сервиса для изолированного рендеринга представлений (Views) 
	/// и частичных представлений (Partials) движка Razor напрямую в строковые буферы.
	/// </summary>
	public interface IViewRenderService
	{
		/// <summary>
		/// Создает и возвращает базовый автономный контекст действия MVC.
		/// </summary>
		ActionContext GetActionContext();

		/// <summary>
		/// Ищет полное представление Razor в указанном контексте действия.
		/// </summary>
		ViewEngineResult GetViewEngineResult(ActionContext actionContext, string viewName);

		/// <summary>
		/// Ищет полное представление Razor, используя автономный контекст по умолчанию.
		/// </summary>
		ViewEngineResult GetViewEngineResult(string viewName);

		/// <summary>
		/// Ищет частичное представление Razor (Partial View) по его имени.
		/// </summary>
		ViewEngineResult GetPartialEngineResult(string viewName);

		/// <summary>
		/// Асинхронно компилирует и рендерит полное представление Razor в строку с передачей модели данных.
		/// </summary>
		Task<string> RenderViewToStringAsync(string viewName, object? model);

		/// <summary>
		/// Асинхронно компилирует и рендерит частичное представление Razor в строку с передачей модели данных.
		/// </summary>
		Task<string> RenderPartialToStringAsync(string viewName, object? model);
	}



	/// <summary>
	/// Реализация сервиса компиляции и рендеринга Razor-шаблонов в текстовые строки.
	/// </summary>
	/// <remarks>
	/// Рекомендуется регистрировать в контейнере зависимостей как Scoped:
	/// <c>builder.Services.AddScoped&lt;IViewRenderService, AnsViewRenderService&gt;();</c>
	/// </remarks>
	public class AnsViewRenderService(
		IRazorViewEngine razorViewEngine,
		ITempDataProvider tempDataProvider,
		IServiceProvider serviceProvider)
		: IViewRenderService
	{

		/* functions */


		/// <inheritdoc />
		public ActionContext GetActionContext()
		{
			return new ActionContext(
				new DefaultHttpContext { RequestServices = serviceProvider },
				new RouteData(),
				new ActionDescriptor());
		}


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ViewEngineResult GetViewEngineResult(
			ActionContext actionContext,
			string viewName)
		{
			ArgumentNullException.ThrowIfNull(actionContext);
			ArgumentException.ThrowIfNullOrEmpty(viewName);
			return razorViewEngine.FindView(actionContext, viewName, isMainPage: false);
		}


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ViewEngineResult GetViewEngineResult(
			string viewName)
		{
			return GetViewEngineResult(GetActionContext(), viewName);
		}


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ViewEngineResult GetPartialEngineResult(
			string viewName)
		{
			ArgumentException.ThrowIfNullOrEmpty(viewName);
			return razorViewEngine.GetView(executingFilePath: viewName, viewPath: viewName, isMainPage: false);
		}


		/// <inheritdoc />
		public async Task<string> RenderViewToStringAsync(
			string viewName,
			object? model)
		{
			ArgumentException.ThrowIfNullOrEmpty(viewName);
			var actionContext1 = GetActionContext();
			var result1 = GetViewEngineResult(actionContext1, viewName);
			if (result1.View == null)
				throw new InvalidOperationException(
					$"[Ans.Net10.Web] \"{viewName}\" does not match any available Razor view.");
			return await ExecuteRenderAsync(actionContext1, result1.View, model);
		}


		/// <inheritdoc />
		public async Task<string> RenderPartialToStringAsync(
			string viewName,
			object? model)
		{
			ArgumentException.ThrowIfNullOrEmpty(viewName);
			var actionContext1 = GetActionContext();
			var result1 = GetPartialEngineResult(viewName);
			if (result1.View == null)
				throw new InvalidOperationException(
					$"[Ans.Net10.Web] \"{viewName}\" does not match any available Razor partial view.");
			return await ExecuteRenderAsync(actionContext1, result1.View, model);
		}


		/// <summary>
		/// Исполняет непосредственный запуск движка рендеринга Razor в выделенный текстовый поток StreamWriter.
		/// </summary>
		private async Task<string> ExecuteRenderAsync(
			ActionContext actionContext,
			IView view,
			object? model)
		{
			var viewData1 = new ViewDataDictionary(
				new EmptyModelMetadataProvider(),
				new ModelStateDictionary())
			{
				Model = model
			};
			using var writer1 = new StringWriter();
			var viewContext1 = new ViewContext(
				actionContext,
				view,
				viewData1,
				new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
				writer1,
				new HtmlHelperOptions());
			await view.RenderAsync(viewContext1);
			return writer1.ToString();
		}

	}

}
