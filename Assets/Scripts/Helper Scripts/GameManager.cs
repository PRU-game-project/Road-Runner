using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private GameData gameData;
    [HideInInspector]
    public bool isOnline = true;
    private string data_Path = "GameData.dat";

    [HideInInspector]
    public int starScore, score_Count, selected_Index;

    [HideInInspector]
    public int starScore2, score_Count2, selected_Index2;



    [HideInInspector]
    public bool[] heroes;

    [HideInInspector]
    public bool[] heroes2;


    [HideInInspector]
    public bool playSound = true;


    //private string data_Path = "GameData.dat";
    public string userName_Input;
    System.Object gameobject;
    private DatabaseReference dbreference;

    void Awake()
    {
        MakeSingleton();
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        checkStatus();
        InitializeGameData();
    }

    void Start()
    {
        if (!isOnline)
        {
            print(Application.persistentDataPath + data_Path);
        }
    }

    void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // this will keep the GameManager object alive even when we load a new scene
        }
    }

    void InitializeGameData()
    {
        if (isOnline)
        {
            LoadGameData(); //online
        }
        else
        {
            LoadLocalGameData();
            if (gameData == null)
            {
                // we are running our game for the first time
                // set up initial values
                //			starScore = 0;

                // FOR TESTING ONLY REMOVE FOR PRODUCTION
                starScore = 9000;

                score_Count = 0;
                selected_Index = 0;

                heroes = new bool[9];
                heroes[0] = true;

                for (int i = 1; i < heroes.Length; i++)
                {
                    heroes[i] = false;
                }

                gameData = new GameData();

                gameData.StarScore = starScore;
                gameData.ScoreCount = score_Count;
                gameData.Heroes = heroes;
                gameData.SelectedIndex = selected_Index;

                SaveGameDataLocal();

            }
        }
    }

    public void checkStatus()
    {
        // if dbreference is null, we are offline
        if (dbreference == null)
        {
            isOnline = false;
        }
        else
        {
            isOnline = true;
        }
    }

    public void SaveGameData()
    {
        print("Save game");
        SaveGameDataLocal();
        gameData = new GameData(userName_Input, starScore, score_Count, heroes, selected_Index);
        string json = JsonConvert.SerializeObject(gameData);

        try
        {
            print("Josn to save -> " + json);
            dbreference.Child("users").Child(userName_Input).SetValueAsync(json);
            print("Data saved");

        }
        catch (Exception e)
        {
            print("Error save data to firebase");
            print(e.Message);
        }
        finally
        {

        }

    }
    public void SaveGameDataLocal()
    {
        Debug.Log("Save Local GameData");
        FileStream file = null;

        try
        {

            BinaryFormatter bf = new BinaryFormatter();

            file = File.Create(Application.persistentDataPath + data_Path);

            if (gameData != null)
            {

                gameData.Heroes = heroes;
                gameData.StarScore = starScore;
                gameData.ScoreCount = score_Count;
                gameData.SelectedIndex = selected_Index;

                bf.Serialize(file, gameData);
            }
            print(Application.persistentDataPath + data_Path);
        }
        catch (Exception e)
        {
            Debug.Log("Error Saving GameData: " + e.Message);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    public void LoadLocalGameData()
    {
        Debug.Log("Load Local GameData");
        FileStream file = null;

        try
        {

            BinaryFormatter bf = new BinaryFormatter();

            file = File.Open(Application.persistentDataPath + data_Path, FileMode.Open);

            gameData = (GameData)bf.Deserialize(file);

            if (gameData != null)
            {
                starScore = gameData.StarScore;
                score_Count = gameData.ScoreCount;
                heroes = gameData.Heroes;
                selected_Index = gameData.SelectedIndex;
            }

        }
        catch (Exception e)
        {
            Debug.Log("Error Loading GameData: " + e.Message);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }
    public void LoadLocalGameDataCompare()
    {
        Debug.Log("Load Local GameData To Compare");
        FileStream file = null;

        try
        {

            BinaryFormatter bf = new BinaryFormatter();

            file = File.Open(Application.persistentDataPath + data_Path, FileMode.Open);

            gameData = (GameData)bf.Deserialize(file);

            if (gameData != null)
            {
                starScore2 = gameData.StarScore;
                score_Count2 = gameData.ScoreCount;
                heroes2 = gameData.Heroes;
                selected_Index2 = gameData.SelectedIndex;
            }

        }
        catch (Exception e)
        {
            Debug.Log("Not have Gamedata Local");
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    public void LoadGameData()
    {
        //StartCoroutine(LoadDataEnum());
        LoadGameDataAsync();
    }


    public async void LoadGameDataAsync() // Thay đổi từ LoadGameData sang LoadGameDataAsync
    {
        print("LoadGameDataAsync");

        var serverData = await dbreference.Child("users").Child(userName_Input).GetValueAsync();

        DataSnapshot snapshot = serverData;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null && userName_Input != null)
        {
            print("jsonData: " + jsonData);
            //compare data local
            LoadLocalGameDataCompare();
            //if the data is different, we will update data follow the local data
            if (starScore != starScore2 || score_Count != score_Count2 || selected_Index != selected_Index2)
            {
                starScore = starScore2;
                score_Count = score_Count2;
                selected_Index = selected_Index2;
                heroes = heroes2;
                SaveGameData();
            }
        }
        else
        {
            Debug.Log("jsonData: is null");
            //load local data to gamedata
            LoadLocalGameData();
            // if gamedata is still null, then we are running the game for the first time
            if (gameData == null)
            {
                // First time running the game (New player)
                starScore = 0;
                score_Count = 0;
                selected_Index = 0;
                heroes = new bool[9];
                heroes[0] = true;

                for (int i = 1; i < heroes.Length; i++)
                {
                    heroes[i] = false;
                }

                gameData = new GameData(userName_Input, starScore, score_Count, heroes, selected_Index);
                SaveGameData();
            }
            else
            {
                SaveGameData(); // save local data to firebase
            }
        }

        // Handle convert json string to object
        var gamedataString = JsonConvert.DeserializeObject(jsonData)?.ToString();
        var gameDataConverted = JsonConvert.DeserializeObject<GameData>(gamedataString);

        if (gameDataConverted != null)
        {
            gameData = gameDataConverted;
            starScore = gameData.star_Score;
            score_Count = gameData.score_Count;
            heroes = gameData.heroes;
            selected_Index = gameData.selected_Index;
        }
        else
        {
            if (gameData == null)
            {
                // First time running the game (New player)
                starScore = 0;
                score_Count = 0;
                selected_Index = 0;
                heroes = new bool[9];
                heroes[0] = true;

                for (int i = 1; i < heroes.Length; i++)
                {
                    heroes[i] = false;
                }

                gameData = new GameData(userName_Input, starScore, score_Count, heroes, selected_Index);
                SaveGameData();
                SaveGameDataLocal();
            }
        }
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
