using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

	// STATE
	public enum ButtonState { Pressed, NotPressed };
	[HideInInspector] public ButtonState buttonState;

	// VARIABLES
	public Tile currentTile;

	// COMPONENTS
	[SerializeField] public Transform associatedTransform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Spawn (Tile tile) {
		currentTile = tile;

		associatedTransform.position = tile.associatedTransform.position;
	}
}