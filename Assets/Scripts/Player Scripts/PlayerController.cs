using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public static PlayerController instance;

	private Animator anim;

	private string jump_Animation = "PlayerJump", change_Line_Animation = "ChangeLine";

	public GameObject
		player,
		shadow;

	public Vector3 
		first_PosOfPlayer,
		second_PosOfPlayer;

	[HideInInspector]
	public bool player_Died;

	[HideInInspector]
	public bool player_Jumped;

	

	private SpriteRenderer player_Renderer;
	public Sprite player_Sprite;

	private bool TRex_Trigger;

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

			

		} else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {

			anim.Play (change_Line_Animation);
			transform.localPosition = first_PosOfPlayer;

			

		}

	}

	void HandleJump() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (!player_Jumped) {
				
				anim.Play (jump_Animation);
				player_Jumped = true;
				
			}
		}
	}

	
	

} // class



































