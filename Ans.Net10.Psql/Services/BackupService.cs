using Ans.Net10.Common;
using System.Diagnostics;
using System.IO.Compression;

namespace Ans.Net10.Psql.Services
{

	/// <summary>
	/// Определяет контракт службы создания резервных копий баз данных PostgreSQL.
	/// </summary>
	public interface IBackupService
	{

		/// <summary>
		/// Инициализирует и запускает асинхронный процесс резервного копирования базы данных PostgreSQL.
		/// </summary>
		/// <param name="host">Сетевой адрес или хост сервера баз данных.</param>
		/// <param name="database">Имя целевой базы данных.</param>
		/// <param name="userName">Имя пользователя (роли) для подключения к СУБД.</param>
		/// <param name="password">Пароль пользователя для аутентификации.</param>
		/// <param name="port">Сетевой порт PostgreSQL сервера.</param>
		/// <returns>Поток-задача, представляющая асинхронную операцию резервного копирования.</returns>
		/// <exception cref="Exception">Генерируется при ошибках выполнения pg_dump или операциях с файлами буфера.</exception>
		Task InitBackup(
			string host,
			string database,
			string userName,
			string password,
			int port);
	}



	/// <summary>
	/// Реализация службы резервного копирования баз данных PostgreSQL
	/// с использованием внешней утилиты <c>pg_dump</c> 
	/// и последующим архивированием в формат ZIP.
	/// </summary>
	public class BackupService
		: IBackupService
	{

		/// <inheritdoc />
		public async Task InitBackup(
			string host,
			string database,
			string userName,
			string password,
			int port)
		{
			string tempFile1 = Path.GetTempFileName();
			string tempPath1 = Path.GetTempPath();
			string normalizedDbName1 = database.Replace(".", "_");
			string backupName1 = $"{host}-{normalizedDbName1}-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}".ToLowerInvariant();
			string zipFilePath1 = Path.Combine(tempPath1, $"{backupName1}.zip");
			try
			{
				var processStartInfo1 = new ProcessStartInfo
				{
					FileName = "pg_dump",
					Arguments = $"--file=\"{tempFile1}\" --format=t -n \"public\" --verbose --host={host} --port={port} --username={userName} \"{database}\"",
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};
				processStartInfo1.EnvironmentVariables.Add("PGPASSWORD", password);
				using (var process1 = Process.Start(processStartInfo1))
				{
					if (process1 == null)
						throw new Exception("[BackupService] Failed to start pg_dump process.");
					await process1.WaitForExitAsync();
					if (process1.ExitCode != 0)
					{
						string errorOutput1 = await process1.StandardError.ReadToEndAsync();
						throw new Exception(
							$"[Ans.Net10.Psql] [BackupService] pg_dump exited with code {process1.ExitCode}. Error: {errorOutput1}");
					}
				}
				if (!File.Exists(tempFile1))
					throw new Exception(
						"[Ans.Net10.Psql] [BackupService] Backup file does not exist.");
				using (var zip1 = ZipFile.Open(zipFilePath1, ZipArchiveMode.Create))
					zip1.CreateEntryFromFile(tempFile1, $"{backupName1}.backup");
				var backupArchive1 = new FileInfo(zipFilePath1);
			}
			catch (Exception ex)
			{
				throw new Exception(
					"[Ans.Net10.Psql] [BackupService] Error creating backup.", ex);
			}
			finally
			{
				SuppIO.DeleteFileIfExists(tempFile1);
				SuppIO.DeleteFileIfExists(zipFilePath1);
			}
		}

	}

}