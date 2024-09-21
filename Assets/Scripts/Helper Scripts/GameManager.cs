using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	private GameData gameData;

	[HideInInspector]
	public int starScore, score_Count, selected_Index;

	[HideInInspector]
	public bool[] heroes;

	[HideInInspector]
	public bool playSound = true;

	//private string data_Path = "GameData.dat";
	public string userName_Input;
	System.Object gameobject;
    private DatabaseReference dbreference;

    void Awake () {
		MakeSingleton ();
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;

        InitializeGameData();
	}

	void Start() {
    }
	
	void MakeSingleton() {
		if (instance != null) {
			Destroy (gameObject);
		} else if(instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject); // this will keep the GameManager object alive even when we load a new scene
        }
	}

	void InitializeGameData() {
		LoadGameData (); // load data from firebase

		

	}

	
	public void SaveGameData() {
		//string json = JsonUtility.ToJson(gameData);
		print("Save game");
        gameData = new GameData(userName_Input, starScore, score_Count, heroes, selected_Index);
        string json = JsonConvert.SerializeObject(gameData);

        try
        {
            print("Josn to save -> "+json);
            dbreference.Child("users").Child(userName_Input).SetValueAsync(json);
            print("Data saved");

        } catch(Exception e) {
			print("error");
            print(e.Message);
        } finally {
			
		}

	}

	
    public void LoadGameData() {
        
        StartCoroutine(LoadDataEnum());
        
    }
  
    IEnumerator LoadDataEnum()
	{
		print("LoadDataEnum");
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Error = (sender, args) =>
            {
                Debug.LogError("Error during deserialization: " + args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            }
        };
        var serverData = dbreference.Child("users").Child(userName_Input).GetValueAsync();
		yield return new WaitUntil(predicate: () => serverData.IsCompleted); // wait until the data is loaded

        DataSnapshot snapshot = serverData.Result;
		string jsonData = snapshot.GetRawJsonValue();

		if (jsonData != null && userName_Input != null)
		{
            print("jsonData: " + jsonData);
		}
		else
		{
			Debug.Log("jsonData: is null");
		}

        //handle convert json string to object
        var gamedataString = JsonConvert.DeserializeObject(jsonData).ToString();
		var gamedata2 = JsonConvert.DeserializeObject<GameData>(gamedataString);
        if (gamedata2 != null)
        {
            gameData = gamedata2;
            
            starScore = gameData.star_Score;
            score_Count = gameData.score_Count;
            heroes = gameData.heroes;
            selected_Index = gameData.selected_Index;
        }
        else
        {
            if (gameData == null)
            {
                // we are running our game for the first time (New player)
                // set up initial values
                			starScore = 0;

                // FOR TESTING ONLY REMOVE FOR PRODUCTION
                //starScore = 9999;

                score_Count = 0;
                selected_Index = 0;

                heroes = new bool[9];
                heroes[0] = true;

                for (int i = 1; i < heroes.Length; i++)
                {
                    heroes[i] = false;
                }


                gameobject = new { userName_Input, starScore, score_Count, heroes, selected_Index };
                gameData = new GameData(userName_Input, starScore, score_Count, heroes, selected_Index);


                SaveGameData();

            }
        }
    }

} // class




































