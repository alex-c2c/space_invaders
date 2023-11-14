using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

public sealed class EnemyManager
{
	private static readonly EnemyManager _instance = new EnemyManager();
	public static EnemyManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
    private static Dictionary<int, EnemyData> _enemyDataDict = null;

	static EnemyManager()
	{
        _enemyDataDict = new Dictionary<int, EnemyData>();
	}
	
	private EnemyManager()
	{

	}

    public static bool LoadEnemyData()
    {
        string dataFilePath = Godot.ProjectSettings.GlobalizePath($"res://{Constants.ENEMY_DATA_FILE}");

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

                var enemyDatas = JsonConvert.DeserializeObject<IEnumerable<EnemyData>>(jsonString);
                foreach (var enemyData in enemyDatas)
                {
                    _enemyDataDict[enemyData.Id] = enemyData;
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

    public static EnemyData GetEnemyData(int id)
    {
        if (_enemyDataDict.TryGetValue(id, out EnemyData data))
        {
            return data;
        }

        return null;
    }
}