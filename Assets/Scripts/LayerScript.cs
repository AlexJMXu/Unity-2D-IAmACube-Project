using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerScript : MonoBehaviour {

	public static int layerNumber;

	private Text layerText;

	// Use this for initialization
	void Start () {
		layerNumber = 1;
		layerText = this.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		layerText.text = "Layer: "+layerNumber;
	}
}
