// rev 2026-09-26

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ans.Net10.Common.Json
{

	/// <summary>
	/// Фабрика конвертеров для автоматического преобразования
	/// строковых значений из JSON в числовые типы данных.
	/// </summary>
	public class AutoStringToNumberConverterFactory
		: JsonConverterFactory
	{

		/* functions */


		/// <inheritdoc />
		public override bool CanConvert(
			Type typeToConvert)
		{
			return Type.GetTypeCode(typeToConvert) switch
			{
				TypeCode.Byte or
				TypeCode.SByte or
				TypeCode.UInt16 or
				TypeCode.UInt32 or
				TypeCode.UInt64 or
				TypeCode.Int16 or
				TypeCode.Int32 or
				TypeCode.Int64 or
				TypeCode.Decimal or
				TypeCode.Double or
				TypeCode.Single => true,
				_ => false,
			};
		}


		/// <inheritdoc />
		public override JsonConverter CreateConverter(
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			var converterType1 = typeof(AutoStringToNumberConverter<>)
				.MakeGenericType(typeToConvert);
			return (JsonConverter)Activator
				.CreateInstance(converterType1)!;
		}





		/* nested classes */


		private sealed class AutoStringToNumberConverter<T>
			: JsonConverter<T>
			where T : struct, IConvertible
		{

			/* functions */


			public override T Read(
				ref Utf8JsonReader reader,
				Type typeToConvert,
				JsonSerializerOptions options)
			{
				if (reader.TokenType == JsonTokenType.Number)
					return (T)Convert.ChangeType(reader.GetDouble(), typeof(T), CultureInfo.InvariantCulture);
				if (reader.TokenType == JsonTokenType.String)
				{
					var s1 = reader.GetString();
					if (double.TryParse(s1, CultureInfo.InvariantCulture, out var d1))
						return (T)Convert.ChangeType(d1, typeof(T), CultureInfo.InvariantCulture);
				}
				throw new JsonException(
					$"[Ans.Net10.Common] Failed to convert the value of type {reader.TokenType} to the numeric type {typeof(T).Name}");
			}


			/* methods */


			public override void Write(
				Utf8JsonWriter writer,
				T value,
				JsonSerializerOptions options)
			{
				var d1 = value.ToDouble(CultureInfo.InvariantCulture);
				writer.WriteNumberValue(d1);
			}

		}

	}

}
