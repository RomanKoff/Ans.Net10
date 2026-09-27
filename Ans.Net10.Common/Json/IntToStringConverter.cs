// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ans.Net10.Common.Json
{

	/// <summary>
	/// Конвертер для десериализации целочисленных значений (int),
	/// переданных в JSON в виде строк, и их сериализации обратно в строки.
	/// </summary>
	public class IntToStringConverter
		: JsonConverter<int>
	{

		/* functions */


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool CanConvert(
			Type typeToConvert)
		{
			return typeof(int) == typeToConvert;
		}


		/// <inheritdoc />
		public override int Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.Number)
				return reader.GetInt32();
			if (reader.TokenType == JsonTokenType.String)
				return reader.GetString().ToInt(0);
			throw new JsonException(
				$"[Ans.Net10.Common] A number or string was expected, but a token was received: {reader.TokenType}");
		}


		/* methods */


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(
			Utf8JsonWriter writer,
			int value,
			JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}

	}

}
