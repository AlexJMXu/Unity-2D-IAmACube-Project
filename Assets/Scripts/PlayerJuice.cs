// !!!!!!!!!!!!!!!!!!!!!!!
// NOTE: I (Anita) was just testing something here - don't use this script anywhere
// !!!!!!!!!!!!!!!!!!!!!!!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJuice : MonoBehaviour {

	private bool touchingPlatform = true;
	private GameObject currentPlatform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag ("Tile")) {
			//Debug.Log ("Hello");
			//StartCoroutine (PushDownTile (other.gameObject));
			touchingPlatform = true;
			currentPlatform = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.CompareTag ("Tile")) {
			//Debug.Log ("Hello");
			//StartCoroutine (PushDownTile (other.gameObject));
			touchingPlatform = false;
		}
	}

	IEnumerator MoveDown(){
		if (touchingPlatform == true) {
			//Debug.Log ("Before Moving" + gameObject.transform.position);

			gameObject.transform.position += new Vector3 (0, -0.3f, 0);
			currentPlatform.transform.position += new Vector3 (0, -0.3f, 0);

			//Debug.Log ("After Down Moving" + gameObject.transform.position);

			yield return new WaitForSeconds (0.1f);

			gameObject.transform.position += new Vector3 (0, 0.3f, 0);
			currentPlatform.transform.position += new Vector3 (0, 0.3f, 0);

			//Debug.Log ("After Up Moving" + gameObject.transform.position);
		}
	}
}
