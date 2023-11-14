using System.Collections.Generic;

public class LevelData
{
    public int Level
    {
        get;
        set;
    }

    public float HiveMoveIntervalSec
    {
        get;
        set;
    }

    public float HiveMoveIntervalFactor
    {
        get;
        set;
    }

    public float HiveShootIntervalSec
    {
        get;
        set;
    }

    public float HiveShootIntervalFactor
    {
        get;
        set;
    }

    public float BonusEnemySpawnTimeSec
    {
        get;
        set;
    }

    public List<string> EnemyList
    {
        get;
        set;
    }

    LevelData()
    {

    }

    public override string ToString()
    {
        string s = base.ToString();
        s += $" | Level: {Level}";
        s += $" | HiveMoveIntervalSec: {HiveMoveIntervalSec}";
        s += $" | HiveMoveFactor: {HiveMoveIntervalFactor}";
        s += $" | HiveShootIntervalSec: {HiveShootIntervalSec}";
        s += $" | HiveShootIntervalFactor: {HiveShootIntervalFactor}";
        s += $" | BonusEnemySpawnTimeSec: {BonusEnemySpawnTimeSec}";
        s += "\n" + PrintEnemyList();

        return s;
    }

    public string PrintEnemyList()
    {
        string s = "EnemyList:\n";

        foreach (string enemyLine in EnemyList)
        {
            s += enemyLine + "\n";
        }

        return s;
    }
}