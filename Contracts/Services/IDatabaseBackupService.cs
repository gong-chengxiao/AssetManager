namespace AssetManager.Contracts.Services;
public interface IDatabaseBackupService
{
    DateTime LastBackupTime
    {
        get;
    }
    Task<string> BackupDatabaseAsync(string backupFile, string externArgs, params string[] table);
    Task<DateTime> LoadTimeFromSettingsAsync();
    Task InitializeAsync();
}
