using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScore : MonoBehaviour {

	private Text scoreText;

	private MainManager mainManager;

	public GameObject fader;

	// Use this for initialization
	void Start () {
		//CustomNetworkManager.singleton.StopServer();
		mainManager = MainManager.instance;
		scoreText = this.GetComponent<Text> ();
		fader.GetComponent<FadeManager> ().Fade (false, 2);
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text = "Team Score: " + mainManager.gameManager.scoreValue;
	}
}
