# Динамическая компиляция кода и представлений (Ans.Net10.Web.Compilation)

Подсистема динамической компиляции библиотеки `Ans.Net10.Web` обеспечивает рантайм-сборку исходных `.cs`-файлов бизнес-логики (`App_Code`) и Razor-представлений (`.cshtml`) на базе `RazorProjectEngine` и компилятора Roslyn (`CSharpCompilation`), полностью заменяя стандартный пакет `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`.



## 1. Архитектура и ключевые компоненты

| Компонент | Назначение |
| :--- | :--- |
| `AnsMetadataReferenceProvider` | Потокобезопасный реестр метаданных (`MetadataReference`), объединяющий базовые сборки платформы (`TRUSTED_PLATFORM_ASSEMBLIES` + `AppDomain`) и динамически скомпилированную сборку `App_Code`. |
| `AnsAppCodeCompiler` | Рантайм-компилятор `.cs`-файлов из каталога `App_Code` в изолированный выгружаемый `AssemblyLoadContext` с отслеживанием изменений через `IFileProvider`. |
| `AnsRazorViewCompiler` | Реализация `IViewCompiler`, выполняющая ленивую компиляцию `.cshtml`-файлов, кэширование дескрипторов и автоматическую инвалидацию при изменении исходников. |
| `AnsRazorViewCompilerProvider` | Провайдер `IViewCompilerProvider`, подменяющий стандартный компилятор MVC в DI-контейнере. |
| `AnsRazorCompilationException` | Исключение, реализующее `ICompilationException` для отображения точных строк ошибок в стандартной странице разработчика ASP.NET Core. |



## 2. Инициализация при старте приложения

1. **Регистрация сервисов (`LibWebStartup.Add_AnsNet10Web`):** При наличии секции `DynamicCompilation` в конфигурации в DI регистрируются синглтоны `AnsMetadataReferenceProvider`, `AnsRazorViewCompiler`, `AnsAppCodeCompiler`, а стандартный `IViewCompilerProvider` заменяется на `AnsRazorViewCompilerProvider`.
2. **Первичная сборка (`LibWebStartup.Use_AnsNet10WebAsync`):** До начала обработки входящих HTTP-запросов вызывается `AnsAppCodeCompiler.CompileAsync(force: true)`, гарантирующий готовность моделей и хелперов из `App_Code`.



## 3. Алгоритм компиляции каталога `App_Code`

1. **Отслеживание изменений:** В конструкторе `AnsAppCodeCompiler` регистрируется подписка `ChangeToken.OnChange` на маску `{AppCodePath}/**/*.cs`. При любом изменении файла устанавливается атомарный флаг `_isDirty = 1`.
2. **Ленивая пересборка (`EnsureCompiledAsync`):** Перед компиляцией любого Razor-представления `AnsRazorViewCompiler` проверяет состояние `App_Code`. Если `_isDirty == 0`, проверка завершается мгновенно без блокировок.
3. **Синхронизация и чтение файлов:** При наличии изменений захватывается `SemaphoreSlim`. Все `.cs`-файлы рекурсивно считываются с защитой от временной блокировки процессами IDE (`_readFileWithRetryAsync` — до 4 попыток с прогрессивной задержкой).
4. **Компиляция Roslyn и горячая подмена контекста:**
   - Исходные файлы парсятся с `LanguageVersion.Latest` и `NullableContextOptions.Enable`.
   - `CSharpCompilation` генерирует в памяти DLL со встроенными отладочными символами (`DebugInformationFormat.Embedded`).
   - При наличии ошибок компиляции сохраняется предыдущая рабочая сборка и выбрасывается `AnsRazorCompilationException`.
   - При успехе новая сборка загружается в коллекционный `AssemblyLoadContext(isCollectible: true)`, ссылки обновляются в `AnsMetadataReferenceProvider`, кэш представлений сбрасывается (`ResetCache()`), а старый контекст выгружается (`Unload()`).



## 4. Алгоритм компиляции Razor-представлений (`.cshtml`)

1. **Индексация предкомпилированных RCL-представлений:** При создании `AnsRazorViewCompiler` и при каждом сбросе кэша (`ResetCache()`) из `ApplicationPartManager` извлекаются встроенные дескрипторы представлений (`ViewsFeature`) и индексируются в `FrozenDictionary`. Если на диске по тому же виртуальному пути существует физический `.cshtml`-файл, он имеет приоритет над RCL.
2. **Проверка актуальности кэша (`CompileAsync`):**
   - Сначала выполняется вызов `AnsAppCodeCompiler.EnsureCompiledAsync()` для гарантии актуальности моделей.
   - Выполняется быстрая проверка записи в `ConcurrentDictionary` по нормализованному виртуальному пути (`_isCacheValid` сверяет временные метки `LastWriteTimeUtc` самого файла и всех иерархических `_ViewImports.cshtml`, а для RCL — отсутствие физического файла на диске).
   - При промахе кэша захватывается гранулярная блокировка `SemaphoreSlim` для конкретного пути (double-checked locking), чтобы исключить дублирующую компиляцию при параллельных запросах.
3. **Кодогенерация Razor и компиляция Roslyn (в пуле потоков):**
   - `RazorProjectEngine` (конфигурация `"MVC-3.0"`, `RazorLanguageVersion.Latest`) с подключенными пространствами имен `Ans.Net10.Common` / `Ans.Net10.Web` и TagHelpers из всех `ApplicationParts` транслирует `.cshtml` в C#-код.
   - Синтаксическое дерево парсится с виртуальным именем `{PhysicalPath}.g.cs`, а исходные байты `.cshtml` прикрепляются через `EmbeddedText` с алгоритмом `SourceHashAlgorithm.Sha256`.
   - `CSharpCompilation` собирает in-memory сборку с `DebugInformationFormat.Embedded` и загружает её в коллекционный `AssemblyLoadContext`.
4. **Инвалидация и отладка:**
   - К `CompiledViewDescriptor.ExpirationTokens` привязываются общий токен сброса `_resetTokenSource`, наблюдатель за самим `.cshtml`-файлом, а также наблюдатели за всеми `_ViewImports.cshtml` и `_ViewStart.cshtml` вверх по иерархии директорий.
   - Благодаря встроенному PDB и совпадению SHA256-хеша `EmbeddedText` с файлом на диске, отладчик Visual Studio при возникновении исключений сразу открывает нужную строку `.cshtml`-файла без предупреждений о расхождении исходников.
