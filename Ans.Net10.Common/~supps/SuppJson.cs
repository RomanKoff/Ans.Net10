// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для высокопроизводительной работы с JSON 
	/// (классический подход на основе рефлексии и статический компиляторный Source Generation).
	/// </summary>
	public static class SuppJson
	{

		// --- Дополнительная информация ---
		//
		// // Сюда пишем все типы, которые будем превращать в JSON или читать из него.
		// [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
		// [JsonSerializable(typeof(Person))]
		// [JsonSerializable(typeof(List<Person>))] // Если нужно работать со списками
		// public partial class MyProjectJsonContext : JsonSerializerContext { }

		// // Передаем объект и метаданные типа: AppJsonContext.Default.Person
		// string jsonText = SuppJson.GetJsonStringFromObjectGen(originalPerson, MyProjectJsonContext.Default.Person);


		/* consts */


		/// <summary>
		/// Глобальные параметры сериализации JSON-данных по умолчанию.
		/// </summary>
		/// <value>
		/// Предустановленный экземпляр <see cref="JsonSerializerOptions"/> с игнорированием 
		/// свойств со значением <see langword="null"/>, регистронезависимым поиском ключей и безопасным экранированием символов.
		/// </value>
		public static readonly JsonSerializerOptions DEFAULT_JSON_SERIALIZER_OPTIONS = new()
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			PropertyNameCaseInsensitive = true,
			//WriteIndented = true, // красивая печать
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};


		/// <summary>
		/// Глобальные низкоуровневые параметры записи JSON-потоков по умолчанию.
		/// </summary>
		/// <value>
		/// Предустановленный экземпляр <see cref="JsonWriterOptions"/> с оптимизированным безопасным экранированием спецсимволов.
		/// </value>
		public static readonly JsonWriterOptions DEFAULT_JSON_WRITER_OPTIONS = new()
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		};


		/* functions */


		/// <summary>
		/// Десериализует объект из текстовой строки JSON.
		/// </summary>
		/// <typeparam name="T">Тип результирующего целевого объекта.</typeparam>
		/// <param name="json">Исходная строка JSON-документа.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы (<see langword="null"/>), применяются настройки по умолчанию <see cref="DEFAULT_JSON_SERIALIZER_OPTIONS"/>.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или системное значение <see langword="default"/>, если строка пуста или равна <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetObjectFromJsonString<T>(
			string json,
			JsonSerializerOptions? options = null)
		{
			if (string.IsNullOrEmpty(json))
				return default;
			return JsonSerializer.Deserialize<T>(
				json, options ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// Десериализует объект из сырого массива байт JSON в кодировке UTF-8.
		/// </summary>
		/// <typeparam name="T">Тип результирующего целевого объекта.</typeparam>
		/// <param name="utf8Json">Исходный байтовый массив в кодировке UTF-8.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetObjectFromJsonBytes<T>(
			byte[] utf8Json,
			JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Deserialize<T>(
				utf8Json, options ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// Десериализует объект из неизменяемой readonly-последовательности байт памяти JSON (UTF-8) без лишних аллокаций.
		/// </summary>
		/// <typeparam name="T">Тип результирующего целевого объекта.</typeparam>
		/// <param name="utf8Json">Входящий высокопроизводительный срез байт памяти <see cref="ReadOnlySpan{Byte}"/>.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetObjectFromJsonSpan<T>(
			ReadOnlySpan<byte> utf8Json,
			JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Deserialize<T>(
				utf8Json, options ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// Десериализует объект из синхронного базового потока данных <see cref="Stream"/>.
		/// </summary>
		/// <typeparam name="T">Тип результирующего целевого объекта.</typeparam>
		/// <param name="stream">Входящий бинарный поток данных.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetObjectFromJsonStream<T>(
			Stream stream,
			JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Deserialize<T>(
				stream, options ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// Асинхронно десериализует объект из входящего потока данных JSON.
		/// </summary>
		/// <typeparam name="T">Тип результирующего целевого объекта.</typeparam>
		/// <param name="stream">Входящий асинхронный поток данных.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Структура <see cref="ValueTask{T}"/>, содержащая десериализованный объект типа <typeparamref name="T"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<T?> GetObjectFromJsonStreamAsync<T>(
			Stream stream,
			JsonSerializerOptions? options = null)
		{
			return JsonSerializer.DeserializeAsync<T>(
				stream, options ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// Десериализует объект напрямую из физического JSON-файла на диске.
		/// </summary>
		/// <typeparam name="T">Тип результирующего целевого объекта.</typeparam>
		/// <param name="filename">Полный или относительный путь к файлу конфигурации или данных JSON на диске.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или <see langword="null"/>.</returns>
		public static T? GetObjectFromJsonFile<T>(
			string filename,
			JsonSerializerOptions? options = null)
		{
			using var stream1 = new FileStream(
				filename, FileMode.Open, FileAccess.Read);
			return GetObjectFromJsonStream<T>(
				stream1, options);
		}


		/// <summary>
		/// Асинхронно десериализует объект напрямую из физического JSON-файла с диска в неблокирующем режиме.
		/// </summary>
		/// <typeparam name="T">Тип результирующего целевого объекта.</typeparam>
		/// <param name="filename">Полный или относительный путь к файлу JSON на диске.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Поток-задача <see cref="Task{T}"/>, содержащая десериализованный объект типа <typeparamref name="T"/>.</returns>
		public static async Task<T?> GetObjectFromJsonFileAsync<T>(
			string filename,
			JsonSerializerOptions? options = null)
		{
			using var stream1 = new FileStream(
				filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
			return await GetObjectFromJsonStreamAsync<T>(
				stream1, options);
		}


		/// <summary>
		/// Сериализует переданный объект в обычную текстовую строку JSON.
		/// </summary>
		/// <param name="obj">Экземпляр объекта любой структуры для сериализации.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Текстовая строка, содержащая JSON-представление переданного объекта.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetJsonStringFromObject(
			object obj,
			JsonSerializerOptions? options = null)
		{
			return JsonSerializer.Serialize(
				obj, options ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// Сериализует объект в массив байт JSON в высокопроизводительной кодировке UTF-8.
		/// </summary>
		/// <param name="obj">Экземпляр объекта для сериализации.</param>
		/// <param name="options">Кастомные параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <returns>Массив байт в кодировке UTF-8, готовый для передачи по сети или записи.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] GetJsonBytesFromObject(
			object obj,
			JsonSerializerOptions? options = null)
		{
			return JsonSerializer.SerializeToUtf8Bytes(
				obj, options ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// (Source Gen) Десериализует объект из текстовой строки JSON без использования тяжелой рантайм-рефлексии (Reflection).
		/// </summary>
		/// <typeparam name="T">Тип десериализуемого объекта.</typeparam>
		/// <param name="json">Исходная строка JSON-документа.</param>
		/// <param name="jsonTypeInfo">Инфраструктурные метаданные типа со сценарием Source Generation, сгенерированные компилятором на этапе сборки проекта.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetObjectFromJsonStringGen<T>(
			string json,
			JsonTypeInfo<T> jsonTypeInfo)
		{
			return JsonSerializer.Deserialize(
				json, jsonTypeInfo);
		}


		/// <summary>
		/// (Source Gen) Десериализует объект из массива байт JSON (UTF-8) без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип десериализуемого объекта.</typeparam>
		/// <param name="utf8Json">Исходный массив байт в кодировке UTF-8.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetObjectFromJsonBytesGen<T>(
			byte[] utf8Json,
			JsonTypeInfo<T> jsonTypeInfo)
		{
			return JsonSerializer.Deserialize(
				utf8Json, jsonTypeInfo);
		}


		/// <summary>
		/// (Source Gen) Десериализует объект из высокопроизводительного readonly-среза байт памяти без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип десериализуемого объекта.</typeparam>
		/// <param name="utf8Json">Входящий срез байт памяти <see cref="ReadOnlySpan{Byte}"/> в кодировке UTF-8.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <returns>Десериализованный объект типа <typeparamref name="T"/> или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetObjectFromJsonSpanGen<T>(
			ReadOnlySpan<byte> utf8Json,
			JsonTypeInfo<T> jsonTypeInfo)
		{
			return JsonSerializer.Deserialize(
				utf8Json, jsonTypeInfo);
		}


		/// <summary>
		/// (Source Gen) Асинхронно десериализует объект из потока данных JSON без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип десериализуемого объекта.</typeparam>
		/// <param name="stream">Входящий асинхронный поток данных.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <returns>Структура-задача <see cref="ValueTask{T}"/>, содержащая десериализованный объект типа <typeparamref name="T"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ValueTask<T?> GetObjectFromJsonStreamGenAsync<T>(
			Stream stream,
			JsonTypeInfo<T> jsonTypeInfo)
		{
			return JsonSerializer.DeserializeAsync(
				stream, jsonTypeInfo);
		}


		/// <summary>
		/// (Source Gen) Асинхронно десериализует объект напрямую из файла JSON на диске без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип десериализуемого объекта.</typeparam>
		/// <param name="filename">Полный или относительный путь к файлу на диске.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <returns>Поток-задача <see cref="Task{T}"/>, содержащая десериализованный объект типа <typeparamref name="T"/>.</returns>
		public static async Task<T?> GetObjectFromJsonFileGenAsync<T>(
			string filename,
			JsonTypeInfo<T> jsonTypeInfo)
		{
			using var stream1 = new FileStream(
				filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
			return await GetObjectFromJsonStreamGenAsync(
				stream1, jsonTypeInfo);
		}


		/// <summary>
		/// (Source Gen) Сериализует строго типизированный объект в строку JSON без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип сериализуемого объекта.</typeparam>
		/// <param name="obj">Экземпляр объекта для сериализации.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <returns>Текстовая строка, содержащая быстро скомпилированное JSON-представление объекта.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetJsonStringFromObjectGen<T>(
			T obj,
			JsonTypeInfo<T> jsonTypeInfo)
		{
			return JsonSerializer.Serialize(
				obj, jsonTypeInfo);
		}


		/// <summary>
		/// (Source Gen) Сериализует строго типизированный объект в массив байт UTF-8 без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип сериализуемого объекта.</typeparam>
		/// <param name="obj">Экземпляр объекта для сериализации.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <returns>Массив байт в кодировке UTF-8, сгенерированный без аллокаций на анализ метаданных.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] GetJsonBytesFromObjectGen<T>(
			T obj,
			JsonTypeInfo<T> jsonTypeInfo)
		{
			return JsonSerializer.SerializeToUtf8Bytes(
				obj, jsonTypeInfo);
		}


		/// <summary>
		/// Записывает сериализованный объект напрямую в синхронный поток данных <see cref="Stream"/> с помощью экономичного писателя <see cref="Utf8JsonWriter"/>.
		/// </summary>
		/// <param name="obj">Экземпляр объекта для записи.</param>
		/// <param name="stream">Целевой бинарный поток данных для записи.</param>
		/// <param name="serializerOptions">Параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <param name="writerOptions">Параметры низкоуровневой конфигурации писателя (экранирование, отступы).</param>
		public static void WriteObjectToJsonStream(
			object obj,
			Stream stream,
			JsonSerializerOptions? serializerOptions = null,
			JsonWriterOptions writerOptions = default)
		{
			var encoder1 = writerOptions.Encoder == null
				? DEFAULT_JSON_WRITER_OPTIONS : writerOptions;
			using var writer1 = new Utf8JsonWriter(
				stream, encoder1);
			JsonSerializer.Serialize(
				writer1, obj, serializerOptions ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
		}


		/// <summary>
		/// Асинхронно записывает сериализованный объект напрямую в поток данных <see cref="Stream"/> с помощью писателя <see cref="Utf8JsonWriter"/>.
		/// </summary>
		/// <param name="obj">Экземпляр объекта для записи.</param>
		/// <param name="stream">Целевой асинхронный поток для записи.</param>
		/// <param name="serializerOptions">Параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <param name="writerOptions">Параметры низкоуровневой конфигурации писателя.</param>
		/// <returns>Объект-задача <see cref="Task"/>, представляющий асинхронную операцию записи и сброса буферов.</returns>
		public static async Task WriteObjectToJsonStreamAsync(
			object obj,
			Stream stream,
			JsonSerializerOptions? serializerOptions = null,
			JsonWriterOptions writerOptions = default)
		{
			var encoder1 = writerOptions.Encoder == null
				? DEFAULT_JSON_WRITER_OPTIONS : writerOptions;
			await using var writer1 = new Utf8JsonWriter(
				stream, encoder1);
			JsonSerializer.Serialize(
				writer1, obj, serializerOptions ?? DEFAULT_JSON_SERIALIZER_OPTIONS);
			await writer1.FlushAsync();
		}


		/// <summary>
		/// Сериализует и физически сохраняет объект в файл формата JSON на диск.
		/// </summary>
		/// <param name="obj">Экземпляр объекта для сохранения.</param>
		/// <param name="filename">Полный или относительный путь к создаваемому/перезаписываемому файлу на диске.</param>
		/// <param name="options">Параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <param name="writerOptions">Параметры низкоуровневой конфигурации писателя.</param>
		public static void SaveObjectToJsonFile(
			object obj,
			string filename,
			JsonSerializerOptions? options = null,
			JsonWriterOptions writerOptions = default)
		{
			using var stream1 = new FileStream(
				filename, FileMode.Create, FileAccess.Write);
			WriteObjectToJsonStream(
				obj, stream1, options, writerOptions);
		}


		/// <summary>
		/// Асинхронно сериализует и сохраняет объект в файл формата JSON на диск в неблокирующем потоки режиме.
		/// </summary>
		/// <param name="obj">Экземпляр объекта для сохранения.</param>
		/// <param name="filename">Полный или относительный путь к создаваемому файлу на диске.</param>
		/// <param name="options">Параметры сериализации. Если не заданы, применяются настройки по умолчанию.</param>
		/// <param name="writerOptions">Параметры низкоуровневой конфигурации писателя.</param>
		/// <returns>Объект-задача <see cref="Task"/>, представляющий асинхронную операцию записи на устройство ввода-вывода.</returns>
		public static async Task SaveObjectToJsonFileAsync(
			object obj,
			string filename,
			JsonSerializerOptions? options = null,
			JsonWriterOptions writerOptions = default)
		{
			using var stream1 = new FileStream(
				filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
			await WriteObjectToJsonStreamAsync(
				obj, stream1, options, writerOptions);
		}


		/// <summary>
		/// (Source Gen) Асинхронно записывает строго типизированный объект в поток данных JSON без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип сериализуемого объекта.</typeparam>
		/// <param name="obj">Экземпляр объекта для записи.</param>
		/// <param name="stream">Целевой асинхронный поток для записи.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <param name="writerOptions">Параметры низкоуровневой конфигурации писателя.</param>
		/// <returns>Объект-задача <see cref="Task"/>, представляющий асинхронную операцию записи.</returns>
		public static async Task WriteObjectToJsonStreamGenAsync<T>(
			T obj,
			Stream stream,
			JsonTypeInfo<T> jsonTypeInfo,
			JsonWriterOptions writerOptions = default)
		{
			var encoder1 = writerOptions.Encoder == null
				? DEFAULT_JSON_WRITER_OPTIONS : writerOptions;
			await using var writer1 = new Utf8JsonWriter(
				stream, encoder1);
			JsonSerializer.Serialize(
				writer1, obj, jsonTypeInfo);
			await writer1.FlushAsync();
		}


		/// <summary>
		/// (Source Gen) Асинхронно сохраняет строго типизированный объект в файл JSON на диск без использования рантайм-рефлексии.
		/// </summary>
		/// <typeparam name="T">Тип сериализуемого объекта.</typeparam>
		/// <param name="obj">Экземпляр объекта для сохранения.</param>
		/// <param name="filename">Полный или относительный путь к создаваемому файлу на диске.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <param name="writerOptions">Параметры низкоуровневой конфигурации писателя.</param>
		/// <returns>Объект-задача <see cref="Task"/>, представляющий асинхронную операцию сохранения.</returns>
		public static async Task SaveObjectToJsonFileGenAsync<T>(
			T obj,
			string filename,
			JsonTypeInfo<T> jsonTypeInfo,
			JsonWriterOptions writerOptions = default)
		{
			using var stream1 = new FileStream(
				filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
			await WriteObjectToJsonStreamGenAsync(
				obj, stream1, jsonTypeInfo, writerOptions);
		}

	}

}
