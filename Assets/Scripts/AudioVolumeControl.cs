using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeControl : MonoBehaviour {

	public Slider musicVolumeSlider;
	public Slider sfxVolumeSlider;

	//public AudioSource volumeAudio;
	//public AudioSource volumeAudio;

	/*
	public void VolumeController(){
		volumeSlider.value = volumeAudio.volume;
	}*/

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayIntroMusic.introMusic != null) {
			musicVolumeSlider.value = PlayIntroMusic.introMusic.volume;
			PlayIntroMusic.introMusic.volume = musicVolumeSlider.value;
		}
	}
}
