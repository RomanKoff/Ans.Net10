// rev 2026-11-22

using Microsoft.AspNetCore.Diagnostics;
using System.Text;

namespace Ans.Net10.Web.Compilation
{

    /// <summary>
    /// Представляет структурированную информацию об отдельной ошибке компиляции Razor-представления или C#-кода.
    /// </summary>
    /// <param name="FilePath">Виртуальный или физический путь к исходному файлу, в котором обнаружена ошибка.</param>
    /// <param name="Line">Номер строки с ошибкой (индексация с 1).</param>
    /// <param name="Column">Номер столбца (символа) с ошибкой (индексация с 1).</param>
    /// <param name="ErrorCode">Системный идентификатор ошибки компилятора (например, <c>"CS1002"</c> или <c>"RZ1001"</c>).</param>
    /// <param name="Message">Текстовое описание причины ошибки.</param>
    public sealed record AnsCompilationError(
        string FilePath,
        int Line,
        int Column,
        string ErrorCode,
        string Message)
    {
        /// <summary>
        /// Возвращает форматированную строку ошибки с указанием файла, номера строки, столбца и кода диагностики.
        /// </summary>
        /// <returns>Строковое представление ошибки вида <c>"File.cshtml (12,5): CS1002: ; expected"</c>.</returns>
        public override string ToString()
        {
            return $"{FilePath} ({Line},{Column}): {ErrorCode}: {Message}";
        }
    }



    /// <summary>
    /// Исключение, возникающее при обнаружении синтаксических или семантических ошибок 
    /// в процессе динамической компиляции Razor-представления.
    /// </summary>
    public sealed class AnsRazorCompilationException
        : Exception,
        ICompilationException
    {

        private readonly Lazy<IEnumerable<CompilationFailure>> _failures;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsRazorCompilationException"/>.
        /// </summary>
        /// <param name="viewPath">Виртуальный путь к компилируемому Razor-представлению.</param>
        /// <param name="errors">Коллекция обнаруженных ошибок компиляции.</param>
        /// <param name="generatedCode">Опциональный сгенерированный C#-код представления для диагностики.</param>
        /// <param name="sourceCode">Опциональный исходный текст Razor-представления (<c>.cshtml</c>) для отображения контекста строки.</param>
        public AnsRazorCompilationException(
            string viewPath,
            IReadOnlyList<AnsCompilationError> errors,
            string? generatedCode = null,
            string? sourceCode = null)
            : base(_buildDetailedMessage(viewPath, errors))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(viewPath);
            ArgumentNullException.ThrowIfNull(errors);

            ViewPath = viewPath;
            Errors = errors;
            GeneratedCode = generatedCode;
            SourceCode = sourceCode;
            _failures = new Lazy<IEnumerable<CompilationFailure>>(_buildFailures);
        }


        /* readonly properties */


        /// <summary>
        /// Возвращает виртуальный путь к представлению, в котором произошла ошибка компиляции.
        /// </summary>
        public string ViewPath { get; }


        /// <summary>
        /// Возвращает коллекцию всех ошибок, зафиксированных при компиляции.
        /// </summary>
        public IReadOnlyList<AnsCompilationError> Errors { get; }


        /// <summary>
        /// Возвращает исходный C#-код, сгенерированный движком Razor перед передачей в Roslyn, либо <see langword="null"/>.
        /// </summary>
        public string? GeneratedCode { get; }


        /// <summary>
        /// Возвращает исходный текст Razor-представления (<c>.cshtml</c>), либо <see langword="null"/>.
        /// </summary>
        public string? SourceCode { get; }


        /// <inheritdoc />
        public IEnumerable<CompilationFailure> CompilationFailures
            => _failures.Value;


        /* privates */


        private static string _buildDetailedMessage(
            string viewPath,
            IReadOnlyList<AnsCompilationError> errors)
        {
            if (errors is null || errors.Count == 0)
                return $"Ошибка компиляции Razor-представления '{viewPath}'.";

            var sb1 = new StringBuilder();
            sb1.AppendLine($"Ошибка компиляции Razor-представления '{viewPath}' (ошибок: {errors.Count}):");
            foreach (var err1 in errors)
                sb1.AppendLine($"  - {err1}");

            return sb1.ToString().TrimEnd();
        }


        private IEnumerable<CompilationFailure> _buildFailures()
        {
            var messages1 = Errors.Select(x => new DiagnosticMessage(
                message: $"{x.ErrorCode}: {x.Message}",
                formattedMessage: x.ToString(),
                filePath: x.FilePath,
                startLine: x.Line,
                startColumn: x.Column,
                endLine: x.Line,
                endColumn: x.Column + 1)).ToArray();

            return
            [
                new CompilationFailure(
                    sourceFilePath: ViewPath,
                    sourceFileContent: SourceCode ?? string.Empty,
                    compiledContent: GeneratedCode ?? string.Empty,
                    messages: messages1)
            ];
        }

    }

}
