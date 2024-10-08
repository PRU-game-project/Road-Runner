﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour {

	public static GameplayController instance;

	public float moveSpeed, distance_Factor = 1f;
	public float distance_Move;
	private bool gameJustStarted;
	public bool Is30score = false;

    public GameObject obstacles_Obj;
	public GameObject[] obstacle_List;

	[HideInInspector]
	public bool obstacles_Is_Active;

	private string Coroutine_Method_Name = "SpawnObstacles";

	private Text score_Text;
	private Text star_Score_Text;

	private int star_Score_Count, score_Count;

	public GameObject pause_Panel;
	public Animator pause_Anim;

	public GameObject gameOver_Panel;
	public Animator gameOver_Anim;

	public Text final_Score_Text, best_Score_Text, final_Star_Score_Text;

	void Awake () {
		MakeInstance ();

		score_Text = GameObject.Find ("ScoreText").GetComponent<Text> ();
		star_Score_Text = GameObject.Find ("StarText").GetComponent<Text> ();
	}

	void Start() {
		gameJustStarted = true;

		GetObstacles ();
		StartCoroutine (Coroutine_Method_Name); // this function will be called every 0.6 seconds

    }

	void Update () {
		MoveCamera ();
	}

	void MakeInstance() {
		if (instance == null) {
			instance = this;

		} else if (instance != null) {
			Destroy (gameObject);
		}
	}

	void MoveCamera() {

		if (gameJustStarted) {

			if (!PlayerController.instance.player_Died) {
				// check if player is alive
				if (moveSpeed < 12.0f) {
					moveSpeed += Time.deltaTime * 5.0f;

				} else {
					moveSpeed = 12f;
					gameJustStarted = false;
				}
			}
		}

		// check if player is alive
		if(!PlayerController.instance.player_Died) {
			Camera.main.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
			UpdateDistance ();
		}

	}

	void UpdateDistance() {
		distance_Move += Time.deltaTime * distance_Factor;
		float round = Mathf.Round (distance_Move);

		// COUNT AND SHOW THE SCORE
		score_Count = (int)round; // save the score when the player dies
		score_Text.text = round.ToString ();
        if (round >= 30.0f && round < 60.0f) {
			Is30score = true;
			moveSpeed = 14f;

		} else if (round >= 60)
        { // every 60 points increase the speed by 2
            moveSpeed += 1f * Time.deltaTime;
			Debug.Log("Move Speed: " + moveSpeed);
        }
    }

	void GetObstacles() {
		obstacle_List = new GameObject[obstacles_Obj.transform.childCount];

		for (int i = 0; i < obstacle_List.Length; i++) {
			obstacle_List [i] = 
				obstacles_Obj.GetComponentsInChildren<ObstacleHolder> (true) [i].gameObject;
		}
	}

    // this coroutine function will be called every 0.6 seconds by Start()
    IEnumerator SpawnObstacles() {

		while (true) {

			if (!PlayerController.instance.player_Died) {

				if (!obstacles_Is_Active) {

					if (Random.value <= 0.85f)
                    { // 85% chance of spawning an obstacle

                        int randomIndex = 0;

						do {

                            // get a random index
                            randomIndex = Random.Range(0, obstacle_List.Length);

						} while(obstacle_List[randomIndex].activeInHierarchy); // stop random when the obstacle is not active

                        // set the obstacle to active
                        obstacle_List[randomIndex].SetActive (true);
						obstacles_Is_Active = true;

					}

				}

			}

			yield return new WaitForSeconds (0.6f);
		}
	}

	public void UpdateStarScore() {
		star_Score_Count++;
		star_Score_Text.text = star_Score_Count.ToString ();
	}

    public void UpdateStarScoreDouble()
    {
        star_Score_Count+=2;
        star_Score_Text.text = star_Score_Count.ToString();
    }

    public void PauseGame() {
		Time.timeScale = 0f;
		pause_Panel.SetActive (true);
		pause_Anim.Play ("SlideIn");
	}

	public void ResumeGame() {
		pause_Anim.Play ("SlideOut");
	}

	public void RestartGame() {
		Time.timeScale = 1f;
		SceneManager.LoadScene ("Gameplay");
	}

	public void HomeButton() {
		Time.timeScale = 1f;
		SceneManager.LoadScene ("MainMenu");
	}

	public void GameOver() {
		Time.timeScale = 0f;
		gameOver_Panel.SetActive (true);
		gameOver_Anim.Play ("SlideIn");

		final_Score_Text.text = score_Count.ToString ();
		final_Star_Score_Text.text = star_Score_Count.ToString ();

		if (GameManager.instance.score_Count < score_Count) {
			GameManager.instance.score_Count = score_Count;
		}

		best_Score_Text.text = GameManager.instance.score_Count.ToString ();

		GameManager.instance.starScore += star_Score_Count;

		GameManager.instance.SaveGameData();

	}

} // class























































