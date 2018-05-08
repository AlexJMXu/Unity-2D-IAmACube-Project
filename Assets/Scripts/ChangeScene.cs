using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeTheScene(string name){
		SceneManager.LoadScene(name, LoadSceneMode.Single);
	}

	public void Quit(){
		Application.Quit ();
		Debug.Log ("Quit!");
	}
}
