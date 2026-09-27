// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ans.Net10.Common.Json
{

	/// <summary>
	/// Конвертер для автоматического преобразования числовых и логических значений из JSON
	/// в строковое представление при десериализации.
	/// </summary>
	public class AutoNumberToStringConverter
		: JsonConverter<string>
	{

		/* functions */


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool CanConvert(
			Type typeToConvert)
		{
			return typeof(string) == typeToConvert;
		}


		/// <inheritdoc />
		public override string? Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			return reader.TokenType switch
			{
				JsonTokenType.String => reader.GetString(),
				JsonTokenType.Number => reader.TryGetInt64(out long l1)
					? l1.ToString()
					: reader.GetDouble().ToString(),
				JsonTokenType.True => "True",
				JsonTokenType.False => "False",
				JsonTokenType.Null => null,
				_ => throw new JsonException(
					$"[Ans.Net10.Common] Unsupported token for conversion to a string: {reader.TokenType}")
			};
		}


		/* methods */


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(
			Utf8JsonWriter writer,
			string value,
			JsonSerializerOptions options)
		{
			writer.WriteStringValue(value);
		}

	}

}
