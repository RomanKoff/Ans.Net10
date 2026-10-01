// rev 2026-010-01

using Ans.Net10.Common;
using Microsoft.EntityFrameworkCore;

namespace Ans.Net10.Psql
{

	/// <summary>
	/// Методы расширения для объектов <see cref="DbContext"/> и <see cref="DbSet{T}"/>, 
	/// автоматизирующие выполнение сырых SQL-команд, миграций и администрирование таблиц в PostgreSQL.
	/// </summary>
	public static partial class Exts_DbContext
	{

		/* methods */


		/// <summary>
		/// Переназначает права владения на все объекты СУБД, принадлежащие текущей роли (<c>CURRENT_ROLE</c>), 
		/// в пользу указанного нового владельца.
		/// </summary>
		/// <param name="context">Текущий контекст базы данных Entity Framework Core.</param>
		/// <param name="owner">Имя роли (пользователя) в PostgreSQL, которой передаются права владения.</param>
		public static void SqlReassignOwned(
			this DbContext context,
			string owner)
		{
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
			_ = context.Database.ExecuteSqlRaw(@$"
REASSIGN OWNED BY CURRENT_ROLE TO ""{owner}"";
");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
			_ = context.SaveChanges();
			Console.WriteLine($"[Ans.Net10.Psql] SqlReassignOwned(\"{owner}\")");
		}


		/// <summary>
		/// Изменяет владельца указанной таблицы в PostgreSQL.
		/// </summary>
		/// <param name="context">Текущий контекст базы данных Entity Framework Core.</param>
		/// <param name="table">Имя целевой таблицы базы данных.</param>
		/// <param name="owner">Имя новой роли-владельца таблицы.</param>
		public static void SqlAlterTableOwnerTo(
			this DbContext context,
			string table,
			string owner)
		{
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
			_ = context.Database.ExecuteSqlRaw(@$"
ALTER TABLE ""{table}"" OWNER TO ""{owner}"";
");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
			_ = context.SaveChanges();
			Console.WriteLine($"[Ans.Net10.Psql] SqlAlterTableOwnerTo(\"{table}\", \"{owner}\")");
		}


		/* functions */


		/// <summary>
		/// Полностью очищает указанную таблицу с помощью команды <c>DELETE</c> и принудительно 
		/// сбрасывает значение её автоинкрементного первичного ключа <c>Id</c> в начальное состояние (<c>1</c>).
		/// </summary>
		/// <param name="context">Текущий контекст базы данных Entity Framework Core.</param>
		/// <param name="table">Имя очищаемой таблицы базы данных.</param>
		/// <returns>Количество измененных записей в контексте после сохранения изменений.</returns>
		public static int SqlClearTable(
			this DbContext context,
			string table)
		{
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
			_ = context.Database.ExecuteSqlRaw(@$"
DELETE FROM ""{table}"";
SELECT setval(pg_get_serial_sequence('public.""{table}""', 'Id'), 1, 'f');
");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
			var count1 = context.SaveChanges();
			Console.WriteLine($"[Ans.Net10.Psql] SqlClearTable(\"{table}\")");
			return count1;
		}


		/// <summary>
		/// Извлекает физическое имя таблицы в базе данных PostgreSQL и связанный контекст <see cref="DbContext"/> 
		/// на основе метаданных текущего набора <see cref="DbSet{T}"/>. Поддерживает Nullable-контекст.
		/// </summary>
		/// <typeparam name="T">Тип доменной сущности, управляемой набором данных.</typeparam>
		/// <param name="dbSet">Текущий экземпляр набора данных.</param>
		/// <returns>Кортеж, содержащий строковое имя таблицы и ссылку на контекст базы данных. Элементы кортежа могут быть равны <see langword="null"/>.</returns>
		public static (string? TableName, DbContext? Context) GetTableNameAndContext<T>(
			this DbSet<T> dbSet)
			where T : class
		{
			var context1 = dbSet.GetDbContext();
			var type1 = context1?.Model.FindEntityType(typeof(T));
			return (type1?.GetTableName(), context1);
		}


		/// <summary>
		/// Очищает таблицу, связанную с типизированным набором <see cref="DbSet{T}"/>, со сбросом автоинкремента первичного ключа. 
		/// Производит безопасную валидацию метаданных на равенство <see langword="null"/>.
		/// </summary>
		/// <typeparam name="T">Тип доменной сущности.</typeparam>
		/// <param name="dbSet">Текущий экземпляр набора данных.</param>
		/// <returns>Результат выполнения операции очистки контекста, либо <c>-1</c>, если метаданные таблицы не найдены.</returns>
		public static int SqlClearTable<T>(
			this DbSet<T> dbSet)
			where T : class
		{
			var (name1, context1) = dbSet.GetTableNameAndContext();
			if (name1 == null || context1 == null)
				return -1;
			return context1.SqlClearTable(name1);
		}


		/// <summary>
		/// Вычисляет максимальное текущее значение первичного ключа <c>Id</c> в таблице типизированного набора 
		/// и синхронизирует с ним связанный автоинкрементный счетчик последовательности PostgreSQL.
		/// </summary>
		/// <typeparam name="T">Тип доменной сущности.</typeparam>
		/// <param name="dbSet">Текущий экземпляр набора данных.</param>
		/// <returns>Количество затронутых строк СУБД, либо <c>-1</c>, если контекст или имя таблицы равны <see langword="null"/>.</returns>
		public static int SerialSequenceSetMax<T>(
			this DbSet<T> dbSet)
			where T : class
		{
			var (name1, context1) = dbSet.GetTableNameAndContext();
			if (name1 == null || context1 == null)
				return -1;
			return context1.SerialSequenceSetMax(name1);
		}


		/// <summary>
		/// Выполняет порционную высокопроизводительную миграцию данных из внешнего источника в целевой набор <see cref="DbSet{TTarget}"/> 
		/// с периодическим сохранением транзакций в базу данных PostgreSQL.
		/// </summary>
		/// <typeparam name="TSource">Тип объектов источника данных.</typeparam>
		/// <typeparam name="TTarget">Тип доменных сущностей назначения.</typeparam>
		/// <param name="target">Целевой набор данных <see cref="DbSet{TTarget}"/>, в который выполняется вставка.</param>
		/// <param name="source">Исходная коллекция элементов для миграции.</param>
		/// <param name="newItem">Делегат функции маппинга, преобразующий объект источника в сущность назначения.</param>
		/// <param name="bufferCount">Размер порции (батча) записей, после достижения которого вызывается сохранение изменений на диске. По умолчанию: <c>999</c>.</param>
		/// <param name="funcDebug">Опциональный делегат для генерации и вывода отладочной информации по каждому обрабатываемому элементу.</param>
		/// <returns>Общее количество успешно мигрированных и сохраненных записей в базе данных.</returns>
		public static int Migration<TSource, TTarget>(
			this DbSet<TTarget> target,
			IEnumerable<TSource> source,
			Func<TSource, TTarget> newItem,
			int bufferCount = 999,
			Func<TSource, string>? funcDebug = null)
			where TSource : class
			where TTarget : class
		{
			var (name1, db1) = target.GetTableNameAndContext();
			if (name1 == null || db1 == null)
				return -1;
			if (source == null)
			{
				Console.WriteLine("the source is empty.");
				return -1;
			}
			int c1 = source switch
			{
				ICollection<TSource> c => c.Count,
				IReadOnlyCollection<TSource> rc => rc.Count,
				_ => source.Count()
			};
			if (c1 == 0)
			{
				Console.WriteLine("the source is empty.");
				return 0;
			}
			var count1 = 0;
			var i1 = bufferCount;
			Console.Write($"Migration [{name1}] — {c1}:");
			Console.CursorVisible = false;
			SuppConsole.CursorSavePos();
			foreach (var item1 in source)
			{
				if (funcDebug != null)
				{
					Console.WriteLine($"{name1}: {c1--} {funcDebug(item1)}  ");
					SuppConsole.CursorRestorePos();
				}
				var item2 = newItem(item1);
				target.Add(item2);
				if (--i1 < 1)
				{
					count1 += db1.SaveChanges();
					Console.Write(count1);
					SuppConsole.CursorRestorePos();
					i1 = bufferCount;
				}
			}
			if (i1 != bufferCount)
				count1 += db1.SaveChanges();
			db1.SerialSequenceSetMax($"{name1}");
			Console.WriteLine(SuppLangEn.GetPlural("{0} {1} added.", count1, "entity", "entities"));
			Console.CursorVisible = true;
			return count1;
		}


		/// <summary>
		/// Выполняет миграцию данных из внешнего источника методом <see cref="Migration"/> только в том случае, 
		/// если целевая таблица набора <see cref="DbSet{TTarget}"/> полностью пуста.
		/// </summary>
		/// <typeparam name="TSource">Тип объектов источника данных.</typeparam>
		/// <typeparam name="TTarget">Тип доменных сущностей назначения.</typeparam>
		/// <param name="target">Целевой набор данных <see cref="DbSet{TTarget}"/>.</param>
		/// <param name="source">Исходная коллекция элементов для миграции.</param>
		/// <param name="newItem">Делегат функции маппинга объектов.</param>
		/// <param name="bufferCount">Размер батча сохраняемых записей. По умолчанию: <c>999</c>.</param>
		/// <returns>Количество сохраненных записей либо <c>0</c>, если таблица уже содержала данные.</returns>
		public static int MigrationIfNotAny<TSource, TTarget>(
			this DbSet<TTarget> target,
			IEnumerable<TSource> source,
			Func<TSource, TTarget> newItem,
			int bufferCount = 999)
			where TSource : class
			where TTarget : class
		{
			if (target.Any())
			{
				var (name1, _) = target.GetTableNameAndContext();
				if (name1 == null)
					return -1;
				Console.Write($"Migration [{name1}] — ");
				Console.WriteLine(SuppLangEn.GetPlural(
					"already contains {0} {1}.", target.Count(), "entity", "entities"));
				return 0;
			}
			return target.Migration(source, newItem, bufferCount);
		}


		/// <summary>
		/// Выполняет сырой SQL-запрос, если переданная строка команды не является пустой или равной <see langword="null"/>.
		/// </summary>
		/// <param name="context">Текущий контекст базы данных Entity Framework Core.</param>
		/// <param name="sql">Строка сырого SQL-запроса.</param>
		/// <returns>Количество обработанных строк СУБД либо <c>0</c>, если запрос был пуст.</returns>
		public static int ExecuteSqlRawIfPresent(
			this DbContext context,
			string sql)
		{
			return string.IsNullOrEmpty(sql)
				? 0 : context.Database.ExecuteSqlRaw(sql);
		}


		/// <summary>
		/// Синхронизирует связанный автоинкрементный счетчик последовательности PostgreSQL
		/// с максимальным текущим значением столбца <c>Id</c>.
		/// </summary>
		/// <param name="context">Текущий контекст базы данных Entity Framework Core.</param>
		/// <param name="table">Имя целевой таблицы.</param>
		/// <returns>Количество затронутых строк СУБД.</returns>
		public static int SerialSequenceSetMax(
			this DbContext context,
			string table)
		{
			return context.Database.ExecuteSqlRaw(
				SuppSql.GetPsqlSerialSequence(table, "Id"));
		}


		/// <summary>
		/// Создает или обновляет в базе данных PostgreSQL системную PL/pgSQL функцию <c>func_dateupdate()</c>, 
		/// автоматически присваивающую столбцу <c>DateUpdate</c> текущее время сервера при срабатывании триггеров.
		/// </summary>
		/// <param name="context">Текущий контекст базы данных Entity Framework Core.</param>
		/// <returns>Результат выполнения команды СУБД.</returns>
		public static int CreateFunction_DateUpdate(
			this DbContext context)
		{
			const string sql1 = @"
CREATE OR REPLACE FUNCTION func_dateupdate()
RETURNS TRIGGER AS $$
BEGIN
   NEW.""DateUpdate"" = LOCALTIMESTAMP; 
   RETURN NEW;
END;
$$ language 'plpgsql';";
			return context.Database.ExecuteSqlRaw(sql1);
		}


		/// <summary>
		/// Создает или обновляет в базе данных PostgreSQL триггер автоматического обновления даты модификации записи 
		/// для указанной таблицы, срабатывающий перед выполнением операции <c>UPDATE</c>.
		/// </summary>
		/// <param name="context">Текущий контекст базы данных Entity Framework Core.</param>
		/// <param name="table">Имя таблицы, для которой создается триггер.</param>
		/// <returns>Результат выполнения команды СУБД.</returns>
		public static int CreateTrigger_DateUpdate(
			this DbContext context,
			string table)
		{
			string sql1 = @$"
CREATE OR REPLACE TRIGGER trigger_{table}_dateupdate
BEFORE UPDATE ON public.""{table}""
FOR EACH ROW EXECUTE PROCEDURE func_dateupdate();";
			return context.Database.ExecuteSqlRaw(sql1);
		}

	}

}