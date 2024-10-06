using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;

	public AudioSource
		move_Audio_Source,
		jump_Audio_Source,
		clip_AudioSource, 
        background_Audio_Source,
		powerUp_Adudio_Source; // default: powerUp sound

    public AudioClip
		die_Clip,
		coin_Clip,
		game_Over_Clip;

	void Awake () {
        MakeInstance ();
	}

	void Start() {
        if (GameManager.instance.playSound) {
            background_Audio_Source.Play ();
		} else {
			background_Audio_Source.Stop ();
		}
	}
	
	void MakeInstance() {
		if (instance == null) {
			instance = this;
		} else if (instance != null) {
			Destroy (gameObject);
		}
	}

	public void PlayMoveLineSound() {
		move_Audio_Source.Play ();
	}

	public void PlayJumpSound() {
		jump_Audio_Source.Play ();
	}

	public void PlayDeadSound() {
        clip_AudioSource.clip = die_Clip;
        clip_AudioSource.Play ();
	}

	public void PlayPowerUpSound() {
        powerUp_Adudio_Source.Play ();
	}

	public void PlayCoinSound() {
		clip_AudioSource.clip = coin_Clip;
        clip_AudioSource.Play ();
	}

	public void PlayGameOverClip() {
		background_Audio_Source.Stop ();
		background_Audio_Source.clip = game_Over_Clip;
		background_Audio_Source.loop = false;
		background_Audio_Source.Play ();
	}

} // class








































