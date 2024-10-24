using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour {

	public static PlayerController instance;

	private Animator anim;

	private string jump_Animation = "PlayerJump", change_Line_Animation = "ChangeLine", dash_Animation = "Dash";

	public GameObject
		player,
		shadow,
		supporter;

	public Vector3 
		first_PosOfPlayer,
		second_PosOfPlayer;

	[HideInInspector]
	public bool player_Died;

	[HideInInspector]
	public bool player_Jumped;

    [HideInInspector]
    public bool player_Dashed;

    public GameObject explosion;

	private SpriteRenderer player_Renderer;
	public Sprite TRex_Sprite, player_Sprite;

	private bool TRex_Trigger;
    private bool Supporter_Trigger;

	private float lastSummonedScore = 0f;


    private GameObject[] start_Effect;

	void Awake() {
		MakeInstance ();

		anim = player.GetComponent<Animator> ();

		player_Renderer = player.GetComponent<SpriteRenderer> ();

		start_Effect = GameObject.FindGameObjectsWithTag (MyTags.STAR_EFFECT);
	}

	void Start () {
		string path = "Sprites/Player/hero" + GameManager.instance.selected_Index + "_big";
		player_Sprite = Resources.Load<Sprite> (path);
		player_Renderer.sprite = player_Sprite;
	}

	void Update () {
		HandleChangeLine ();
		HandleJump ();
		HandleDash();
		SummonSupporter();
    }

	void MakeInstance() {
		if (instance == null) {
			instance = this;
		} else if (instance != null) {
			Destroy (gameObject);
		}
	}

	void HandleChangeLine() {

		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) {

			anim.Play (change_Line_Animation);
			transform.localPosition = second_PosOfPlayer;

			SoundManager.instance.PlayMoveLineSound ();

		} else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {

			anim.Play (change_Line_Animation);
			transform.localPosition = first_PosOfPlayer;

			SoundManager.instance.PlayMoveLineSound ();

		}

	}

	void HandleJump() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (!player_Jumped) {
				
				anim.Play (jump_Animation);
				player_Jumped = true;
				SoundManager.instance.PlayJumpSound ();
			}
		}
	}

    void HandleDash()
    {
		if (GameplayController.instance.Is30score)
		{
           
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if (!player_Dashed)
                {

                    anim.Play(dash_Animation);
                    player_Dashed = true;
                    SoundManager.instance.PlayMoveLineSound();
                }
            }
        }
    }

	void SummonSupporter()
	{
        float round = Mathf.Floor(GameplayController.instance.distance_Move);

		if (round % 30 == 0 && round != Mathf.Floor(lastSummonedScore) && round > 0)
		{
            lastSummonedScore = round;
            if (!Supporter_Trigger)
            {
                Supporter_Trigger = true;
                StartCoroutine(SupporterDuration());
            }
        }
		
       

    }
    void Die() {
		player_Died = true;
		player.SetActive (false);
		shadow.SetActive (false);

		GameplayController.instance.moveSpeed = 0f;
		GameplayController.instance.GameOver ();

		SoundManager.instance.PlayDeadSound ();
		SoundManager.instance.PlayGameOverClip ();
	}

	void DieWithObstacle(Collider2D target) {
		
        Die ();

		explosion.transform.position = target.transform.position;
		explosion.SetActive (true);
		target.gameObject.SetActive (false);

		SoundManager.instance.PlayDeadSound ();
	}

	IEnumerator TRexDuration() {
        SoundManager.instance.PlayPowerUpSound();

        yield return new WaitForSeconds (7f); // wait for 7 seconds
                                              // after 7 seconds, the TRex power up will be over
        if (TRex_Trigger) {
			TRex_Trigger = false;
            player_Renderer.sprite = player_Sprite;
        }

    }

    IEnumerator SupporterDuration()
    {

        SoundManager.instance.PlaySummonSound(); // anime sound
        GameObject newSupporter = Instantiate(supporter, new Vector3(0f,0f, 0f), Quaternion.identity);

        //set player is parent of supporter
        newSupporter.transform.SetParent(player.transform);
        newSupporter.transform.localPosition = new Vector3(1.43f, 1.03f, 0f);

        yield return new WaitForSeconds(10f); // wait for 10 seconds
                                             // after 7 seconds, the TRex power up will be over
        if (Supporter_Trigger)
        {
            Supporter_Trigger = false;
            Destroy(newSupporter);
            SoundManager.instance.PlaySkillAppearSound();
        }

    }

    void DestroyObstacle(Collider2D target) {
		explosion.transform.position = target.transform.position;
		explosion.SetActive (false); // turn off the explosion if its already turned on
		explosion.SetActive (true);

		target.gameObject.SetActive (false);

		SoundManager.instance.PlayDeadSound();

	}

	void OnTriggerEnter2D(Collider2D target) {
        if (target.tag == MyTags.OBSTACLE) {

			if (!TRex_Trigger) {
				DieWithObstacle (target);

			} else {
				DestroyObstacle (target);
			}

		}

		if (target.tag == MyTags.T_REX) {

			TRex_Trigger = true;
			player_Renderer.sprite = TRex_Sprite;
			target.gameObject.SetActive (false);


			StartCoroutine (TRexDuration());

		}

		if (target.tag == MyTags.STAR) {

			for (int i = 0; i < start_Effect.Length; i++) {
				if (!start_Effect [i].activeInHierarchy) {
					start_Effect [i].transform.position = target.transform.position;
					start_Effect [i].SetActive (true);
					break;
				}
			}

			target.gameObject.SetActive (false);
			SoundManager.instance.PlayCoinSound ();
            if (Supporter_Trigger)
            {
                GameplayController.instance.UpdateStarScoreDouble();
            }
            else
            {
                GameplayController.instance.UpdateStarScore();
            }
           
		}

	}

} // class



































