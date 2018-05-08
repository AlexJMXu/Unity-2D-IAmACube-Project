using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TileManager : NetworkBehaviour {

	public static TileManager instance;

	public GameObject fader;

	// 25 x 25 map size
	public static int GRID_SIZE = 30;
	private const int MINIMUM_MAP_SIZE = 50;
	private const int MAXIMUM_MAP_SIZE = 200;

	[HideInInspector] public Tile[] map;

	[SyncVar] private int randomSeed;

	[SerializeField] private GameObject pickupPrefab;
	public float numberOfPickups = 20;

	private MainManager mainManager;
	private RoomGenerator tileGenerator;

	public List<Tile> listOfNonLockTiles = new List<Tile>();
	public List<Room> roomList = new List<Room>();
	//public static List<Block> roomList = new List<Block>();
	public List<GameObject> redLockList = new List<GameObject>();
	public List<GameObject> blueLockList = new List<GameObject> ();
	public List<GameObject> greenLockList = new List<GameObject> ();

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}

		tileGenerator = GetComponent<RoomGenerator>();
	}

	void Start() {
		mainManager = MainManager.instance;
	}

	public void GenerateMap() {
		if (isServer) {
			randomSeed = Random.Range(0, 999);
			GenerateMapForAllPlayers(randomSeed);
		}
	}

	public void GenerateMapForAllPlayers(int seed) {
		CmdGenerateMapForAllPlayers(seed);
	}

	[Command]
	private void CmdGenerateMapForAllPlayers(int seed) {
		RpcGenerateMapForAllPlayers(seed);
	}

	[ClientRpc]
	private void RpcGenerateMapForAllPlayers(int seed) {
		mainManager.gameManager.DisableStartButtonAndText();

		Random.InitState(seed);

		map = tileGenerator.GenerateTiles(map, GRID_SIZE);

		tileGenerator.GenerateMap(map, MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

		PlacePickups();

		foreach (Room room in roomList) {
			if (room.roomType != Room.RoomType.RedLockRoom && room.roomType != Room.RoomType.BlueLockRoom && room.roomType != Room.RoomType.GreenLockRoom) {
				Tile[] roomTiles = room.GetRoomTiles ();
				foreach (Tile tile in roomTiles) {
					listOfNonLockTiles.Add (tile);
				}
			}
		}
	}

	public void NextMap() {
		CmdNextMap();
	}

	[Command]
	private void CmdNextMap() {
		RpcNextMap();
	}

	[ClientRpc]
	private void RpcNextMap() {

		fader.GetComponent<FadeManager> ().Fade (true, 0);

		mainManager.gameManager.SetAllReadyTextOff();

		listOfNonLockTiles.Clear();
		roomList.Clear();
		redLockList.Clear();
		blueLockList.Clear();
		greenLockList.Clear();

		for (int i = 0; i < map.Length; i++) {
			map[i].ForceStopFading();
			map[i].spriteRenderer.enabled = false;
			map[i].spriteRenderer.color = map[i].roomColor;

			if (map[i].pickupObject != null) Destroy(map[i].pickupObject);
			if (map[i].lockObject != null) Destroy(map[i].lockObject);
			if (map[i].buttonObject != null) Destroy(map[i].buttonObject);

			map[i].tileType = Tile.TileType.other;
			map[i].tileContains = Tile.TileContains.empty;
			map[i].room = null;

			map[i].pickupObject = null;
			map[i].lockObject = null;
			map[i].buttonObject = null;

			map[i].isActive = false;
			map[i].isOccupied = false;
			map[i].isFading = false;
		}

		tileGenerator.GenerateMap(map, MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

		PlacePickups();

		foreach (Room room in roomList) {
			if (room.roomType != Room.RoomType.RedLockRoom && room.roomType != Room.RoomType.BlueLockRoom && room.roomType != Room.RoomType.GreenLockRoom) {
				Tile[] roomTiles = room.GetRoomTiles ();
				foreach (Tile tile in roomTiles) {
					listOfNonLockTiles.Add (tile);
				}
			}
		}

		mainManager.gameManager.pickupCount = 0;
		mainManager.gameManager.pickupCounterText.text = "Pickup Counter: " + mainManager.gameManager.pickupCount + " / " + numberOfPickups;

		LayerScript.layerNumber++;

		fader.GetComponent<FadeManager> ().Fade (false, 2);
	}

	public Tile GetRandomActiveTile() {
		while (true) {
			int rand = Random.Range(0, map.Length);

			if (map[rand].isActive) return map[rand];
		}
	}

	public Tile GetRandomSpawnPoint() {
		while (true) {
			int rand = Random.Range(0, listOfNonLockTiles.Count);

			if (listOfNonLockTiles[rand].isActive && !listOfNonLockTiles[rand].isOccupied) return listOfNonLockTiles[rand];
		}
	}

	public void PlacePickups() {
		List<Tile> listOfTilesForPickups = new List<Tile>();

		for (int i = 0; i < numberOfPickups; i++) {
			Tile randomSpawnPoint = mainManager.tileManager.GetRandomActiveTile();
			// Do not spawn the pickups on locks/ buttons etc
			while (randomSpawnPoint.tileContains != Tile.TileContains.empty) {
				randomSpawnPoint = mainManager.tileManager.GetRandomActiveTile();
			}
			
			listOfTilesForPickups.Add (randomSpawnPoint);
			randomSpawnPoint.tileContains = Tile.TileContains.pickup;
			randomSpawnPoint.pickupObject = (GameObject) Instantiate (pickupPrefab, listOfTilesForPickups[i].transform.position, Quaternion.identity);
		}
	}
}
