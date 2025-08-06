using System;
using System.IO;
using BepInEx;
using CartInventory.DTO;
using HarmonyLib;
using Newtonsoft.Json;

namespace CartInventory;

public class SaveManager
{
    private string _currentSaveId = "default";
    public SaveData Data { get; private set; } = new();

    public void UpdateSaveId()
    {
        try
        {
            if (StatsManager.instance != null)
            {
                var str = (string)AccessTools.Field(typeof(StatsManager), "saveFileCurrent")
                    .GetValue(StatsManager.instance);
                if (!string.IsNullOrEmpty(str))
                {
                    if (str.EndsWith(".sav"))
                        str = str.Substring(0, str.Length - 4);
                    _currentSaveId = str;
                    return;
                }
            }

            if (!string.IsNullOrEmpty(_currentSaveId))
                return;
            _currentSaveId = "default";
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(_currentSaveId))
                return;
            _currentSaveId = "default";
        }
    }

    public string GetCurrentSaveFilePath()
    {
        return Path.Combine(Path.Combine(Paths.ConfigPath, "CartInventory"), _currentSaveId + ".json");
    }

    public void Save()
    {
        if (SemiFunc.IsMultiplayer())
            return;
        var path = GetCurrentSaveFilePath();
        if (!File.Exists(path))
        {
            var directoryName = Path.GetDirectoryName(path);
            if (directoryName != null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        var content = JsonConvert.SerializeObject(Data, Formatting.Indented);
        File.WriteAllText(path, content);
    }

    public void Load()
    {
        if (SemiFunc.IsMultiplayer())
            return;
        var path = GetCurrentSaveFilePath();
        if (File.Exists(path))
        {
            var fileData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));
            if (fileData != null)
            {
                Data = fileData;
            }
        }
    }

    public void Delete()
    {
        if (SemiFunc.IsMultiplayer())
            return;
        Data.TrackDollars = 0;
        var path = GetCurrentSaveFilePath();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}