using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class TimerScript : NetworkBehaviour {

	[SyncVar] private float timer = 180.0f;
	private float timerOrigin = 180.0f;
	private float min = 2;
	public GameObject textObject;
	private Text timerText;

	//public GameObject overScreen;

	public GameObject fader;

	// Use this for initialization
	void Start () {
		timer = timerOrigin;
		timerText = textObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		// Will count down every second
		timer -= Time.deltaTime;
		//timerText.text = "8:" + (Mathf.Round(timer)).ToString() + "am";
		timerText.text = "Time: " + min +":"+ (Mathf.Round(timer)).ToString(); 

		/*
		if (min == 0 && timer <= 0f) {
			StartCoroutine (FadeOut ());
		}

		if (timer <= 0f) {
			//timer = timerOrigin;
			//fader.GetComponent<FadeManager> ().Fade (true, 2);
			//timerText.text = "TIME OVER";
			//overScreen.SetActive(true);
			//SceneManager.LoadScene("Over");
			//SceneManager.LoadScene("EndScene");

			//StartCoroutine (FadeOut ());
			min--;
			timer = timerOrigin;
		}*/

		int minutes = Mathf.FloorToInt(timer / 60F);
		int seconds = Mathf.FloorToInt(timer - minutes * 60);
		string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

		//GUI.Label(new Rect(10,10,250,100), minutes + ":" + seconds);
		timerText.text = niceTime; 

		if (minutes == 0 && seconds <= 2f) {
			StartCoroutine (FadeOut ());
		}

		/*
		if (timer <= 0f) {
			//timer = timerOrigin;
			//fader.GetComponent<FadeManager> ().Fade (true, 2);
			//timerText.text = "TIME OVER";
			//overScreen.SetActive(true);
			//SceneManager.LoadScene("Over");
			//SceneManager.LoadScene("EndScene");

			StartCoroutine (FadeOut ());
		}*/
	}

	IEnumerator FadeOut(){
		fader.GetComponent<FadeManager> ().Fade (true, 2);

		yield return Yielders.Get (2);
		timerText.text = "TIME OVER";
		
		SceneManager.LoadScene("EndScene");
	}
}
