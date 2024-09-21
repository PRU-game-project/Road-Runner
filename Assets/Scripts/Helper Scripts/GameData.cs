using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//[Serializable]
public class GameData
{
    public string name_User { get; set; }
    public int star_Score { get; set; }
    public int score_Count { get; set; }
    public bool[] heroes { get; set; }
    public int selected_Index { get; set; }

    // Constructor
    public GameData(string name_User, int star_Score, int score_Count, bool[] heroes, int selected_Index)
    {
        this.name_User = name_User;
        this.star_Score = star_Score;
        this.score_Count = score_Count;
        this.heroes = heroes;
        this.selected_Index = selected_Index;
    }
}


































