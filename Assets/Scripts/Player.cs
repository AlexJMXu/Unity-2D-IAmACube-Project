using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

	public int playerID;

	// STATE
	public enum PlayerState { Idle, Moving };
	[HideInInspector] public PlayerState playerState;

	// VARIABLES
	public Tile currentTile;

	// COMPONENTS
	[SerializeField] public Animator animator;
	[SerializeField] public Transform associatedTransform;
	[SerializeField] public SpriteRenderer spriteRenderer;

	public GameObject ValueText;
	public Canvas theCanvas;

	private PlayerController playerController;

	private MainManager mainManager;
	private TileManager tileManager;

	public bool isLocal;

	void Awake() {
		playerController = GetComponent<PlayerController>();

		spriteRenderer.enabled = false;
	}

	void Start() {
		mainManager = MainManager.instance;
		tileManager = TileManager.instance;
		//ValueText = GameObject.Find ("pickupValText");
		//ValueText.SetActive (false);

		theCanvas = GameObject.Find ("Canvas").GetComponent<Canvas>();

		//ValueText.transform.SetParent (theCanvas.transform, false);

		if (!isServer) mainManager.gameManager.DisableStartButton();
		if (isLocalPlayer) isLocal = true;
	}

	public void Spawn (int playerID) {
		CmdSpawn(playerID);
	}

	[Command]
	private void CmdSpawn(int playerID) {
		RpcSpawn(playerID);
	}

	[ClientRpc]
	private void RpcSpawn(int playerID) {
		if (isLocalPlayer) mainManager.gameManager.StartTimer();

		this.playerID = playerID;
		spriteRenderer.enabled = true;

		Tile randomSpawnPoint = mainManager.tileManager.GetRandomSpawnPoint();
		// Do not spawn the player on lucks/ buttons/ pickups etc
		while (randomSpawnPoint.tileContains != Tile.TileContains.empty) {
			randomSpawnPoint = mainManager.tileManager.GetRandomSpawnPoint();
		}

		currentTile = randomSpawnPoint;
		currentTile.isOccupied = true;

		if (isLocalPlayer) playerController.enabled = true;
		associatedTransform.position = currentTile.associatedTransform.position;
	}

	public void Respawn() {
		CmdRespawn();
	}

	[Command]
	private void CmdRespawn() {
		RpcRespawn();
	}

	[ClientRpc]
	private void RpcRespawn() {
		playerState = Player.PlayerState.Idle;

		currentTile.isOccupied = false;

		Tile randomSpawnPoint = mainManager.tileManager.GetRandomSpawnPoint();
		// Do not spawn the player on lucks/ buttons/ pickups etc
		while (randomSpawnPoint.tileContains != Tile.TileContains.empty) {
			randomSpawnPoint = mainManager.tileManager.GetRandomSpawnPoint();
		}

		currentTile = randomSpawnPoint;
		currentTile.isOccupied = true;

		associatedTransform.position = currentTile.associatedTransform.position;
		
		playerController.pressedReady = false;
	}

	public void GetPickup() {

		mainManager.gameManager.pickupCount++;
		mainManager.gameManager.pickupCounterText.text = "Pickup Counter: " + mainManager.gameManager.pickupCount + " / " + tileManager.numberOfPickups;

		if (mainManager.gameManager.pickupCount == tileManager.numberOfPickups) { 
			mainManager.gameManager.pickupCounterText.text = "HURRAH! \\(^O^)/";
		}

		mainManager.gameManager.scoreValue += Mathf.RoundToInt((Mathf.Pow(mainManager.gameManager.pickupCount, 2f) * 10f));

		if (isLocalPlayer) {
			GameObject theText = Instantiate (ValueText, theCanvas.transform.position, theCanvas.transform.rotation);
			theText.transform.SetParent (theCanvas.transform, false);
			Debug.Log (Mathf.RoundToInt ((Mathf.Pow (mainManager.gameManager.pickupCount, 2f) * 10f)));
			int value = Mathf.RoundToInt ((Mathf.Pow (mainManager.gameManager.pickupCount, 2f) * 10f));
			theText.GetComponent<Text> ().text = "+" + value;
			//yield return Yielders.Get (1);
			Destroy (theText, 0.5f);
		}

		mainManager.gameManager.scoreCounterText.text = "Score: " + mainManager.gameManager.scoreValue;
	}

}
