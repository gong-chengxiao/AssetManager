﻿using AssetManager.Contracts.Services;
using AssetManager.Core.Helpers;
using AssetManager.Core.Models;
using MySqlConnector;
using System.Diagnostics;

namespace AssetManager.Services;
public class DatabaseBackupService : IDatabaseBackupService
{
    private string SK_LastBackupTime = AppSettings.SK_LastBackupTime;
    private readonly ILocalSettingsService _localSettingsService;

    public DateTime LastBackupTime { get; set; } = DateTime.MinValue;

    public DatabaseBackupService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task<string> BackupDatabaseAsync(string backupFile, params string[] table)
    {
        var command = $"mysqldump " +
            $"-h {AppSettings.Host} " +
            $"-P {AppSettings.Port} " +
            $"-u {AppSettings.DbUserName} " +
            $"-p {AppSettings.DbPassWord} " +
            $"{AppSettings.DbName} --tables {string.Join(" ", table)} " +
            $"> {backupFile}";

        // 在命令行中执行command
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            await process.WaitForExitAsync();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            await SaveTimeInSettingsAsync(DateTime.Now);

            await Task.CompletedTask;
            return output;
        }
        catch { throw; }
        
    }
    public async Task InitializeAsync()
    {
        LastBackupTime = await LoadTimeFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task<DateTime> LoadTimeFromSettingsAsync()
    {
        var lastBackupTimeString = await _localSettingsService.ReadSettingAsync<string>(SK_LastBackupTime);

        return lastBackupTimeString == null ? DateTime.MinValue : DateTime.Parse(lastBackupTimeString);
    }
    private async Task SaveTimeInSettingsAsync(DateTime backuptime)
    {
        await _localSettingsService.SaveSettingAsync(SK_LastBackupTime, backuptime.ToString());
    }
}
