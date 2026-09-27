// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Представляет отдельный элемент реестра (запись), поддерживающий иерархические уровни вложенности от 0 до 9, 
	/// маркеры текстовых меток и кастомную текстовую сериализацию/десериализацию.
	/// </summary>
	public class RegistryItem
	{

		/* consts */


		/// <summary>
		/// Строковый маркер для маскирования символа точки с запятой (<c>;</c>).
		/// </summary>
		public const string MASK_semicolon = "[x3B]";

		/// <summary>
		/// Строковый маркер для маскирования символа равенства (<c>=</c>).
		/// </summary>
		public const string MASK_equally = "[x3D]";

		/// <summary>
		/// Строковый маркер для маскирования символа решетки (<c>#</c>).
		/// </summary>
		public const string MASK_sharp = "[x23]";

		/// <summary>
		/// Строковый маркер для маскирования символа двоеточия (<c>:</c>).
		/// </summary>
		public const string MASK_colon = "[x3A]";


		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="RegistryItem"/> с параметрами по умолчанию.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RegistryItem()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RegistryItem"/> с явным указанием всех параметров метаданных.
		/// </summary>
		/// <param name="key">Уникальный строковый ключ элемента реестра.</param>
		/// <param name="value">Текстовое значение элемента.</param>
		/// <param name="level">Уровень вложенности элемента в иерархической структуре реестра (строго в диапазоне от 0 до 9).</param>
		/// <param name="isLabel">Флаг, указывающий, является ли данный элемент чисто текстовой меткой (заголовком группы).</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RegistryItem(
			string key,
			string value,
			int level,
			bool isLabel)
			: this()
		{
			Key = key;
			Value = value;
			Level = level;
			IsLabel = isLabel;
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RegistryItem"/>, выполняя немедленную десериализацию из строки специального формата.
		/// </summary>
		/// <param name="serialization">Строка сериализованных данных элемента формата <c>"key=[#][:level]value"</c>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RegistryItem(
			string serialization)
			: this()
		{
			FillFromString(serialization);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает уникальный ключ элемента реестра.
		/// </summary>
		/// <value>Строковое значение ключа. По умолчанию равно <see cref="string.Empty"/>.</value>
		public string Key { get; private set; } = string.Empty;


		/// <summary>
		/// Возвращает признак того, является ли данный элемент реестра меткой (заголовком подраздела).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если элемент является меткой; во всех остальных случаях — <see langword="false"/>.</value>
		public bool IsLabel { get; private set; }


		/// <summary>
		/// Возвращает уровень вложенности текущего элемента в иерархической структуре реестра.
		/// </summary>
		/// <value>Целочисленное значение уровня в диапазоне от 0 до 9.</value>
		public int Level { get; private set; }


		/* properties */


		/// <summary>
		/// Получает или задает строковое значение элемента реестра.
		/// </summary>
		/// <remarks>
		/// Защитный механизм свойства: если при установке передается значение <see langword="null"/> или пустая строка, 
		/// оно автоматически преобразуется в имя ключа, заключенное в фигурные скобки, например: <c>"{ИмяКлюча}"</c>.
		/// </remarks>
		/// <value>Строковое значение контента элемента.</value>
		public string Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => field = string.IsNullOrEmpty(value)
				? $"{{{Key}}}" : value;
		} = string.Empty;


		/* functions */


		/// <summary>
		/// Принудительно экранирует служебные управляющие символы (<c>;</c>, <c>=</c>, <c>#</c>, <c>:</c>) в строке данных, добавляя перед ними символ обратного слэша.
		/// </summary>
		/// <param name="data">Исходное необработанное строковое значение данных.</param>
		/// <returns>Строка данных с экранированными служебными символами. Если входная строка пуста, возвращается <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetDataEscapes(
			string data)
		{
			if (string.IsNullOrEmpty(data))
				return string.Empty;
			return data
				.Replace(";", "\\;")
				.Replace("=", "\\=")
				.Replace("#", "\\#")
				.Replace(":", "\\:");
		}


		/// <summary>
		/// Маскирует экранированные последовательности служебных символов, заменяя их на безопасные уникальные hex-маркеры.
		/// </summary>
		/// <param name="data">Строка данных, прошедшая этап предварительного экранирования.</param>
		/// <returns>Замаскированная строка данных, готовая к безопасному поиску разделителей в парсере.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetDataMasking(
			string data)
		{
			if (string.IsNullOrEmpty(data))
				return string.Empty;
			return data
				.Replace("\\;", MASK_semicolon)
				.Replace("\\=", MASK_equally)
				.Replace("\\#", MASK_sharp)
				.Replace("\\:", MASK_colon);
		}


		/// <summary>
		/// Демаскирует и восстанавливает ранее замаскированные hex-маркеры, заменяя их на оригинальные чистые служебные символы.
		/// </summary>
		/// <param name="data">Замаскированная строка данных после этапа синтаксического разбора.</param>
		/// <returns>Восстановленная строка с оригинальными неэкранированными служебными символами.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetDataRestores(
			string data)
		{
			if (string.IsNullOrEmpty(data))
				return string.Empty;
			return data
				.Replace(MASK_semicolon, ";")
				.Replace(MASK_equally, "=")
				.Replace(MASK_sharp, "#")
				.Replace(MASK_colon, ":");
		}


		/// <summary>
		/// Сериализует текущий элемент реестра в единую текстовую строку формата <c>"key=[#][:level]value"</c> с автоматическим экранированием служебных символов.
		/// </summary>
		/// <returns>Итоговая строка сериализованных данных элемента реестра, готовая для сохранения в файлы конфигурации.</returns>
		public override string ToString()
		{
			var key1 = GetDataEscapes(Key);
			var value1 = GetDataEscapes(Value);
			var label1 = IsLabel.Make("#");
			var level1 = (Level is > 0 and < 10).Make($":{Level}");
			return $"{key1}={label1}{level1}{value1}";
		}


		/* methods */


		/// <summary>
		/// Полностью заполняет и перезаписывает свойства текущего элемента реестра на основе переданной сериализованной строки.
		/// </summary>
		/// <remarks>
		/// Разбор строки выполняется на основе маскирования разделителей, изоляции ключа по первому вхождению символа <c>'='</c> 
		/// и последующего вычисления флагов типа и уровня вложенности.
		/// </remarks>
		/// <param name="serialization">Строка данных формата <c>"key=[#][:level]value"</c>.</param>
		/// <exception cref="ArgumentException">
		/// Вызывается в следующих случаях: 
		/// <list type="bullet">
		/// <item><description>Переданная строка <paramref name="serialization"/> пуста, равна <see langword="null"/> или состоит только из пробелов.</description></item>
		/// <item><description>В строке полностью отсутствует обязательный символ разделителя ключа и значения (<c>'='</c>).</description></item>
		/// </list>
		/// </exception>
		public void FillFromString(
			string serialization)
		{
			if (string.IsNullOrWhiteSpace(serialization))
				throw new ArgumentException(
					"[Ans.Net10.Common] Строка сериализации не может быть пустой.",
					nameof(serialization));
			var s1 = GetDataMasking(serialization);
			var i1 = s1.IndexOf('=');
			if (i1 < 1)
				throw new ArgumentException(
					"[Ans.Net10.Common] Недопустимый формат строки сериализации элемента реестра. Отсутствует символ '='.",
					nameof(serialization));
			Key = GetDataRestores(s1[..i1]);
			var s2 = s1[(i1 + 1)..];
			if (s2.Length > 0 && s2[0] == '#')
			{
				IsLabel = true;
				s2 = s2[1..];
			}
			if (s2.Length > 1 && s2[0] == ':' && char.IsDigit(s2[1]))
			{
				Level = s2[1].ToString().ToInt(0);
				s2 = s2[2..];
			}
			else if (s2.Length > 0 && s2[0] == ':')
				s2 = s2[1..];
			Value = GetDataRestores(s2);
		}

	}

}
