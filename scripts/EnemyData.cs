using System.Collections.Generic;

public class EnemyData
{
    public int Id
    {
        get;
        set;
    }

    public int Points
    {
        get;
        set;
    }

    public int Hp
    {
        get;
        set;
    }

    EnemyData()
    {

    }

    public override string ToString()
    {
        return $"{base.ToString()} | Id: {Id} | Points: {Points} | Hp: {Hp}";
    }
}