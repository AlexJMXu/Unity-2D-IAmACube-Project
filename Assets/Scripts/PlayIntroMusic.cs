using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayIntroMusic : MonoBehaviour {

	public static bool isIntroMusicPlaying;
	public static AudioSource introMusic;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(introMusic);
		if (!isIntroMusicPlaying) {
			AudioSource introMusic = this.GetComponent<AudioSource> ();
			introMusic.Play ();
			isIntroMusicPlaying = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
