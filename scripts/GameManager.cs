using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

public sealed class GameManager
{
	private static readonly GameManager _instance = new GameManager();
	public static GameManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
    public static bool IsPaused
    {
        get;
        set;
    } = false;

    public static int Score
    {
        get;
        set;
    } = 0;
    public static int Highscore
    {
        get;
        set;
    } = 0;

	static GameManager()
	{
        
	}
	
	private GameManager()
	{

	}

	public static async Task<bool> SaveHighscoreData()
    {
        string filePath = Path.Combine(OS.GetUserDataDir(), Constants.HIGHSCORE_FILE);

        Debug.Print($"Attempting to save data to file: {filePath}");

        Dictionary<string, int> data = new Dictionary<string, int>();
        data["highscore"] = Highscore;

        string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);

        try
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                await sw.WriteAsync(jsonString);
                Debug.Print($"Data saved successfully");
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Print($"Error occured while trying to save file");
            Debug.Print($"Error: {e}");

            return false;
        }        
    }

    public static async Task<bool> LoadHighscoreData()
    {
        string filePath = Path.Combine(OS.GetUserDataDir(), Constants.HIGHSCORE_FILE);

        Debug.Print($"Attempting to load save file: {filePath}");

        if (!File.Exists(filePath))
        {
            await SaveHighscoreData();
            
            Debug.Print($"Warning: Save file {filePath} does not exist!");
            Debug.Print($"Creating highscore file with default values");
            
            return true;
        }

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string jsonString = sr.ReadToEnd();

                Dictionary<string, int> data = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonString);
                Highscore = data["highscore"];
            }

            Debug.Print("Data loaded successfully");

            return true;
        }
        catch (Exception e)
        {
            Debug.Print($"Error occured while trying to load file");
            Debug.Print($"Error: {e}");

            return false;
        }
    }

	public static async void SetHighscore()
	{
        if (Score > Highscore)
        {
            Highscore = Score;
            await SaveHighscoreData();
        }
	}
}