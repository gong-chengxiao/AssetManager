namespace AssetManager.Contracts.Services;
public interface IDatabaseBackupService
{
    DateTime LastBackupTime
    {
        get;
    }
    Task<string> BackupDatabaseAsync(string backupFile, params string[] table);
    Task<string> BackupDatabaseWithRoutinesAsync(string backupFile, params string[] table);
    Task<DateTime> LoadTimeFromSettingsAsync();
    Task InitializeAsync();
}
