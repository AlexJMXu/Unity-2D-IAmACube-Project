using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	// MOVEMENT VARIABLES
	private Vector3 startPos;
	private Vector3 targetPos;
	private float elapsedTime;
    public bool canMove;

	public bool onTile;

	// COMPONENTS
	private Player player;

	public bool isMoving;

	private MainManager mainManager;

	public GameObject pickupParticles;
	public GameObject friendshipParticles;

	public bool pressedReady = false;

	private AudioSource blipSound;
	private AudioSource buttonSound;

	void Start () {
		player = GetComponent<Player>();
        canMove = true;
		onTile = true;
		mainManager = MainManager.instance;
		blipSound = GameObject.Find ("BlipSound").GetComponent<AudioSource>();
		buttonSound = GameObject.Find ("ButtonSound").GetComponent<AudioSource> ();
	}
		
	void Update () {
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.Space) && !pressedReady) {
			CmdNextMap();
			pressedReady = true;
		}

		if (canMove) {
			if (player.playerState == Player.PlayerState.Moving)
				return;

			if (player.currentTile == null)
				return;

			if (player.currentTile.tileContains == Tile.TileContains.lockObj && player.currentTile.lockObject.activeSelf) {
				//Tile[] availableTiles = player.currentTile.
				//SetDestination (player.currentTile.top.associatedTransform.position, 0);

				// Lock Clash
				if (player.currentTile.top != null && player.currentTile.top.isActive && player.currentTile.top.tileContains != Tile.TileContains.lockObj && player.currentTile.top.tileType == Tile.TileType.corridorTile) {
					SetDestination (player.currentTile.top.associatedTransform.position, 0);
				} else if (player.currentTile.bottom != null && player.currentTile.bottom.isActive && player.currentTile.bottom.tileContains != Tile.TileContains.lockObj && player.currentTile.bottom.tileType == Tile.TileType.corridorTile){
					SetDestination (player.currentTile.bottom.associatedTransform.position, 1);
				} else if (player.currentTile.left != null && player.currentTile.left.isActive && player.currentTile.left.tileContains != Tile.TileContains.lockObj && player.currentTile.left.tileType == Tile.TileType.corridorTile){
					SetDestination (player.currentTile.left.associatedTransform.position, 2);
				} else if (player.currentTile.right != null && player.currentTile.right.isActive && player.currentTile.right.tileContains != Tile.TileContains.lockObj && player.currentTile.right.tileType == Tile.TileType.corridorTile){
					SetDestination (player.currentTile.right.associatedTransform.position, 3);
				}
			}

			// Check for friendship - seperated so can check GetKeyDown instead
			if ((Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) && player.currentTile.top != null && player.currentTile.top.isActive) {
				if (player.currentTile.top.isOccupied) { FriendshipForever(); }
			} else if ((Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) && player.currentTile.bottom != null && player.currentTile.bottom.isActive) {
				if (player.currentTile.bottom.isOccupied) { FriendshipForever(); }
			} else if ((Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) && player.currentTile.left != null && player.currentTile.left.isActive) {
				if (player.currentTile.left.isOccupied) { FriendshipForever(); }
			} else if ((Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) && player.currentTile.right != null && player.currentTile.right.isActive) {
				if (player.currentTile.right.isOccupied) { FriendshipForever(); }
			}

			if ((Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) && player.currentTile.top != null && player.currentTile.top.isActive) {
				if (player.currentTile.top.isOccupied) {return;}
				if (player.currentTile.top.tileContains == Tile.TileContains.lockObj && player.currentTile.top.lockObject.activeSelf) {return;}
				SetDestination (player.currentTile.top.associatedTransform.position, 0);
			} else if ((Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) && player.currentTile.bottom != null && player.currentTile.bottom.isActive) {
				if (player.currentTile.bottom.isOccupied) {return;}
				if (player.currentTile.bottom.tileContains == Tile.TileContains.lockObj && player.currentTile.bottom.lockObject.activeSelf) {return;}
				SetDestination (player.currentTile.bottom.associatedTransform.position, 1);
			} else if ((Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) && player.currentTile.left != null && player.currentTile.left.isActive) {
				if (player.currentTile.left.isOccupied) {return;}
				if (player.currentTile.left.tileContains == Tile.TileContains.lockObj && player.currentTile.left.lockObject.activeSelf) {return;}
				SetDestination (player.currentTile.left.associatedTransform.position, 2);
			} else if ((Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) && player.currentTile.right != null && player.currentTile.right.isActive) {
				if (player.currentTile.right.isOccupied) {return;}
				if (player.currentTile.right.tileContains == Tile.TileContains.lockObj && player.currentTile.right.lockObject.activeSelf) {return;}
				SetDestination (player.currentTile.right.associatedTransform.position, 3);
			}
		}

	}

	[Command]
	private void CmdNextMap() {
		RpcShowPlayerReady();
		mainManager.gameManager.NextMap();
	}

	[ClientRpc]
	private void RpcShowPlayerReady() {
		mainManager.gameManager.readyText[player.playerID].SetActive(true);
	}

	private void FriendshipForever() {
		CmdFriendshipForever();

	}

	[Command]
	private void CmdFriendshipForever() {
		RpcFriendshipForever();
	}

	[ClientRpc]
	private void RpcFriendshipForever() {
		StartCoroutine(FriendshipForeverCoroutine());
	}

	private IEnumerator FriendshipForeverCoroutine(){
		if (player.currentTile.top.isOccupied || player.currentTile.left.isOccupied || player.currentTile.right.isOccupied || player.currentTile.bottom.isOccupied ){
			GameObject particles = Instantiate (friendshipParticles, new Vector3(player.currentTile.transform.position.x, player.currentTile.transform.position.y + 1f , -3f), player.currentTile.transform.rotation);
			yield return Yielders.Get (2f);
			Destroy (particles, 2f);
		}

		yield return null;
	}

	void FixedUpdate () {
		Move();
	}

	private void Move() {
		// For arc
		float trajectoryHeight = 0.3f;

		if (player.playerState == Player.PlayerState.Idle) return;

		player.currentTile.isOccupied = true;

		if (elapsedTime >= 0.85f) {
			CheckPickup();
			player.currentTile.FadeTile ();
		}

        if (elapsedTime >= 1f) {
        	CheckIfJumpedOnButton();
        	player.playerState = Player.PlayerState.Idle;
        } else {

        	elapsedTime += Time.deltaTime*2.2f;

			// calculate straight-line lerp position:
        	player.associatedTransform.position = Vector3.Lerp(startPos, targetPos, elapsedTime );

			// add a value to Y, using Sine to give a curved trajectory in the Y direction
			player.associatedTransform.position += new Vector3(0f,trajectoryHeight * Mathf.Sin(Mathf.Clamp01(elapsedTime) * Mathf.PI),0f);

			// Keep y position = z position to simulate depth
        	player.associatedTransform.position = new Vector3(player.associatedTransform.position.x,
        													  player.associatedTransform.position.y,
        													  player.associatedTransform.position.y);
        }
	}

	private void SetDestination(Vector3 dest, int move) {
		CheckIfJumpedOffButton();
		player.currentTile.isOccupied = false;

		switch (move) {
			case 0:
				player.currentTile = player.currentTile.top;
				break;
			case 1:
				player.currentTile = player.currentTile.bottom;
				break;
			case 2:
				player.currentTile = player.currentTile.left;
				break;
			case 3:
				player.currentTile = player.currentTile.right;
				break;
			default:
				break;
		}

		isMoving = true;
		player.playerState = Player.PlayerState.Moving;
		startPos = player.associatedTransform.position;
		targetPos = dest;
		elapsedTime = 0f;

		CmdSetDestination(dest, move);
	}

	[Command]
	private void CmdSetDestination(Vector3 dest, int move) {
		RpcSetDestination(dest, move);
	}

	[ClientRpc]
	private void RpcSetDestination(Vector3 dest, int move) {
		CheckIfJumpedOffButton();
		player.currentTile.isOccupied = false;

		if (isLocalPlayer) return;

		switch (move) {
			case 0:
				player.currentTile = player.currentTile.top;
				break;
			case 1:
				player.currentTile = player.currentTile.bottom;
				break;
			case 2:
				player.currentTile = player.currentTile.left;
				break;
			case 3:
				player.currentTile = player.currentTile.right;
				break;
			default:
				break;
		}

		isMoving = true;
		player.playerState = Player.PlayerState.Moving;
		startPos = player.associatedTransform.position;
		targetPos = dest;
		elapsedTime = 0f;
	}

	private void CheckIfJumpedOnButton() {
		if (player.currentTile.tileContains == Tile.TileContains.button) {
			if (player.currentTile.room.roomType == Room.RoomType.RedButtonRoom) {
				buttonSound.Play ();
				foreach (var lockObj in TileManager.instance.redLockList) {
					lockObj.gameObject.SetActive (false);
				}
			}
			if (player.currentTile.room.roomType == Room.RoomType.BlueButtonRoom) {
				buttonSound.Play ();
				foreach (var lockObj in TileManager.instance.blueLockList) {
					lockObj.gameObject.SetActive (false);
				}
			}
			if (player.currentTile.room.roomType == Room.RoomType.GreenButtonRoom) {
				buttonSound.Play ();
				foreach (var lockObj in TileManager.instance.greenLockList) {
					lockObj.gameObject.SetActive (false);
				}
			}
		}
	}

	private void CheckIfJumpedOffButton() {
		if (player.currentTile.tileContains == Tile.TileContains.button) {
			if (player.currentTile.room.roomType == Room.RoomType.RedButtonRoom) {
				foreach (var lockObj in TileManager.instance.redLockList) {
					lockObj.gameObject.SetActive (true);
				}
			}
			if (player.currentTile.room.roomType == Room.RoomType.BlueButtonRoom) {
				foreach (var lockObj in TileManager.instance.blueLockList) {
					lockObj.gameObject.SetActive (true);
				}
			}
			if (player.currentTile.room.roomType == Room.RoomType.GreenButtonRoom) {
				foreach (var lockObj in TileManager.instance.greenLockList) {
					lockObj.gameObject.SetActive (true);
				}
			}
		}
	}

	private void CheckPickup() {
		if (player.currentTile.tileContains == Tile.TileContains.pickup) {
			Destroy (player.currentTile.pickupObject);
			player.currentTile.tileContains = Tile.TileContains.empty;

			if (isLocalPlayer) blipSound.Play ();

			GameObject particles = Instantiate (pickupParticles, new Vector3(player.currentTile.transform.position.x, player.currentTile.transform.position.y + 0.1f ,0f), player.currentTile.transform.rotation);
			Destroy(particles, 5f);

			player.GetPickup();
		}
	}

}
