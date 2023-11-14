using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

public sealed class LevelManager
{
	private static readonly LevelManager _instance = new LevelManager();
	public static LevelManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
    private static Dictionary<int, LevelData> _levelDataDict = null;

	static LevelManager()
	{
        _levelDataDict = new Dictionary<int, LevelData>();
	}
	
	private LevelManager()
	{

	}

    public static bool LoadLevelData()
    {
        string dataFilePath = Godot.ProjectSettings.GlobalizePath($"res://{Constants.LEVEL_DATA_FILE}");

        Debug.Print($"Attempting to load level data file: {dataFilePath}");

        if (!File.Exists(dataFilePath))
        {
            Debug.Print($"Error: Level data file {dataFilePath} does not exists!");
            return false;
        }

        try
        {
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                string jsonString = sr.ReadToEnd();

                var levelDatas = JsonConvert.DeserializeObject<IEnumerable<LevelData>>(jsonString);
                foreach (var levelData in levelDatas)
                {
                    _levelDataDict[levelData.Level] = levelData;
                }

                Debug.Print("Data loaded successfully");

                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Print($"Error occured while trying to load level data file");
            Debug.Print($"{e}");

            return false;
        }
    }

    public static LevelData GetLevelData(int level)
    {
        if (_levelDataDict.TryGetValue(level, out LevelData data))
        {
            return data;
        }

        return null;
    }
}