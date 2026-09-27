// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ans.Net10.Common.Json
{

	/// <summary>
	/// Конвертер для гибкого чтения логических значений (bool)
	/// из различных типов JSON-токенов (True, False, String, Number).
	/// </summary>
	public class BoolConverter
		: JsonConverter<bool>
	{

		/* functions */


		/// <inheritdoc />
		public override bool Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			return reader.TokenType switch
			{
				JsonTokenType.True => true,
				JsonTokenType.False => false,
				JsonTokenType.String => bool.TryParse(reader.GetString(), out var b1)
					? b1 : (int.TryParse(reader.GetString(), out var i1) && i1 != 0),
				JsonTokenType.Number => reader.TryGetInt64(out long l1)
					? l1 != 0 : reader.TryGetDouble(out double d1) && d1 != 0,
				_ => throw new JsonException(
					$"[Ans.Net10.Common] Unsupported token for conversion to a Boolean value: {reader.TokenType}"),
			};
		}


		/* methods */


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Write(
			Utf8JsonWriter writer,
			bool value,
			JsonSerializerOptions options)
		{
			writer.WriteBooleanValue(value);
		}

	}

}
