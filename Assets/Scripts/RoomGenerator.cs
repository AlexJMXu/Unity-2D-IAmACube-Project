using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {

	private const float TILE_OFFSET_X = 1f;
	private const float TILE_OFFSET_Y = 0.5f;

	[SerializeField] private Transform tileParent;
	[SerializeField] private GameObject tilePrefab;

	[SerializeField] private GameObject redLockPrefab;
	[SerializeField] private GameObject blueLockPrefab;
	[SerializeField] private GameObject greenLockPrefab;
	[SerializeField] private GameObject redButtonPrefab;
	[SerializeField] private GameObject blueButtonPrefab;
	[SerializeField] private GameObject greenButtonPrefab;

	private int numberOfRooms = 10;

	Stack<Tile> tileStack;

	private MainManager mainManager;

	void Start () {
		mainManager = MainManager.instance;
	}

	/* 
	 * Generate tiles based on the GRID_SIZE, all tiles will have
	 * the sprite renderers disabled until they are used to generate
	 * the actual playable map area
	 */
	public Tile[] GenerateTiles(Tile[] map, int GRID_SIZE) {
		Vector3 startPos;

		map = new Tile[GRID_SIZE * GRID_SIZE];

		int count = 0;
		for (int i = 0; i < GRID_SIZE; i++) {
			startPos = new Vector3(((-GRID_SIZE) * TILE_OFFSET_X) + (i * TILE_OFFSET_X) + 1, ((GRID_SIZE/(GRID_SIZE/2)) * TILE_OFFSET_Y) - (i * TILE_OFFSET_Y) - 1, 0);
			for (int j = 0; j < GRID_SIZE; j++) {
				Vector3 pos = startPos + new Vector3(j * TILE_OFFSET_X, j * TILE_OFFSET_Y, 0);

				GameObject go = (GameObject) Instantiate (tilePrefab, pos, Quaternion.identity);

				Tile tile = go.GetComponent<Tile>();
				tile.associatedTransform.name = (count).ToString();
				tile.associatedTransform.parent = tileParent;
				tile.id = count;
				tile.tileContains = Tile.TileContains.empty;
				map[count] = tile;

				count++;
			}
		}

		return SetTileConnections(GRID_SIZE, map);
	}

	/* 
	 * Set which tiles are connected to each other, using the array
	 * storing all the tiles. This will make it easier to get adjacent
	 * tiles through code later.
	 */
	public Tile[] SetTileConnections(int GRID_SIZE, Tile[] map) {
		for (int i = 0; i < map.Length; i++) {
			if (i >= GRID_SIZE) map[i].top = map[i - GRID_SIZE];
			if (i < (GRID_SIZE * GRID_SIZE) - GRID_SIZE) map[i].bottom = map[i + GRID_SIZE];
			if (i % GRID_SIZE != 0) map[i].left = map[i - 1];
			if (i % GRID_SIZE != GRID_SIZE-1) map[i].right = map[i + 1];
		}

		return map;
	}

	/* Outdated Comment 
	 * Generate the actual playable area for the players.
	 * Done by randomly selecting an initial tile, and
	 * generating tiles from that through a loop
	 * until the maximum tile limit is reached.
	 */
	public void GenerateMap(Tile[] map, int min, int max) {
		numberOfRooms = 10;

		tileStack = new Stack<Tile>();

		// ROOM
		for (int k = 0; k < numberOfRooms; k++) {
			// Set room location

			// Centre tile of room
			Tile initialTile = map[Random.Range(0, map.Length)];

			// If the tile is already active, keep looking for more.
			while (initialTile.isActive) {
				initialTile = map[Random.Range(0, map.Length)];
			}

			tileStack.Push(initialTile);

			if (tileStack.Count > 0) {
				Tile currentTile = tileStack.Pop();
				produceRoom (currentTile, max);
			}
		}

		ProduceCorridors ();
		ProduceLocks ();
		ProduceButtons ();
	}

	void ProduceCorridors(){
		for (int i = 0; i < mainManager.tileManager.roomList.Count; i++) {
			if (i == mainManager.tileManager.roomList.Count - 1) {
				ConnectByCorridor (mainManager.tileManager.roomList [i], mainManager.tileManager.roomList [0]);
			} else {
				ConnectByCorridor (mainManager.tileManager.roomList[i], mainManager.tileManager.roomList[i+1]);
			}	
		}
	}

	void ProduceLocks(){
		foreach (var room in mainManager.tileManager.roomList){
			Tile[] tileBoundary;
			tileBoundary = room.GetCorridorBoundaryTiles ();

			foreach (var tile in tileBoundary) {
				if (tile.tileContains == Tile.TileContains.lockObj)	continue;  // If tile already has lock, don't place another one here
				if (room.roomType == Room.RoomType.RedLockRoom) {
					GameObject lockObj = (GameObject) Instantiate (redLockPrefab, tile.transform.position, Quaternion.identity);
					mainManager.tileManager.redLockList.Add (lockObj);
					tile.lockObject = lockObj;
					tile.tileContains = Tile.TileContains.lockObj;
				}
				if (room.roomType == Room.RoomType.BlueLockRoom) {
					GameObject lockObj = (GameObject) Instantiate (blueLockPrefab, tile.transform.position, Quaternion.identity);
					mainManager.tileManager.blueLockList.Add (lockObj);
					tile.lockObject = lockObj;
					tile.tileContains = Tile.TileContains.lockObj;
				}
				if (room.roomType == Room.RoomType.GreenLockRoom) {
					GameObject lockObj = (GameObject) Instantiate (greenLockPrefab, tile.transform.position, Quaternion.identity);
					mainManager.tileManager.greenLockList.Add (lockObj);
					tile.lockObject = lockObj;
					tile.tileContains = Tile.TileContains.lockObj;
				}
				tile.room = room;
			}
		}
	}

	void ProduceButtons(){
		foreach (var room in mainManager.tileManager.roomList){
			Tile[] roomTiles;
			roomTiles = room.GetRoomTiles ();

			// Choose a random tile in the room // todo - make this better
			Tile randomTile = mainManager.tileManager.GetRandomActiveTile();
			while (!roomTiles.Contains (randomTile)) {
				randomTile = mainManager.tileManager.GetRandomActiveTile();
			}
				
			if (room.roomType == Room.RoomType.RedButtonRoom) {
				GameObject buttonObj = (GameObject) Instantiate (redButtonPrefab, randomTile.transform.position, Quaternion.identity);
				//redLockList.Add (buttonObj);
				randomTile.tileContains = Tile.TileContains.button;
				randomTile.buttonObject = buttonObj;
			}
			if (room.roomType == Room.RoomType.BlueButtonRoom) {
				GameObject buttonObj = (GameObject) Instantiate (blueButtonPrefab, randomTile.transform.position, Quaternion.identity);
				//blueLockList.Add (buttonObj);
				randomTile.tileContains = Tile.TileContains.button;
				randomTile.buttonObject = buttonObj;
			}
			if (room.roomType == Room.RoomType.GreenButtonRoom) {
				GameObject buttonObj = (GameObject) Instantiate (greenButtonPrefab, randomTile.transform.position, Quaternion.identity);
				//greenLockList.Add (buttonObj);
				randomTile.tileContains = Tile.TileContains.button;
				randomTile.buttonObject = buttonObj;
			}
			randomTile.room = room;
		}
	}

	void CreateCorridorTile (Tile headTile){
		if (headTile.tileType == Tile.TileType.roomTile) {
			return;
		}

		headTile.spriteRenderer.enabled = true;
		//headTile.spriteRenderer.color = Color.magenta;
		headTile.spriteRenderer.color = new Color(136/255f, 160/255f, 24/255f, 1);
		headTile.isActive = true;
		tileStack.Push(headTile);
		headTile.tileType = Tile.TileType.corridorTile;
	}

	void ConnectByCorridor(Room room1, Room room2){
		Tile headTile = room1.referenceTile;

		// HORIZONTAL CONNECTION
		if (room1.referenceTile.getTileX () < room2.referenceTile.getTileX ()) { // Move right

			// Ignore tiles until end of room
			for (int i = 0;  i < ((room1.xSize-1)/2)+1; i++){
				headTile = headTile.right;
			}

			while (headTile.getTileX() < room2.referenceTile.getTileX ()) {
				if (headTile == null) {break;}
				CreateCorridorTile (headTile);
				headTile = headTile.right;
				if (headTile == null) {break;}
			}
				
		}	else if (room1.referenceTile.getTileX () > room2.referenceTile.getTileX ()) { // Move Left
			
			for (int i = 0;  i < ((room1.xSize-1)/2)+1; i++){
				headTile = headTile.left;
			}

			while (headTile.getTileX() > room2.referenceTile.getTileX ()) {
				if (headTile == null) {break;}
				CreateCorridorTile (headTile);
				headTile = headTile.left;
				if (headTile == null) {break;}
			}
		}

		// VERTICAL CONNECTION
		if (room1.referenceTile.getTileY () < room2.referenceTile.getTileY ()) { 

			while (headTile.getTileY() < room2.referenceTile.getTileY () - (room2.ySize-1)/2) {
				if (headTile == null) {break;}
				CreateCorridorTile (headTile);
				headTile = headTile.bottom;
				if (headTile == null) {break;}
			}

		}	else if (room1.referenceTile.getTileY () > room2.referenceTile.getTileY () ) { 
			
			while (headTile.getTileY() > room2.referenceTile.getTileY () + (room2.ySize-1)/2) {
				if (headTile == null) {break;}
				CreateCorridorTile (headTile);
				headTile = headTile.top;
				if (headTile == null) {break;}
			}
		}
	}
		

	// Produce a room around a tile
	void produceRoom(Tile currentTile, int max){

		// Add the room to the list of rooms in the scene - currently the room uses the centre point as the reference tile
		Room newRoom = new Room();
		newRoom.referenceTile = currentTile;

		// How many sides is the room going to have? - randomly chosen between a range, each number should be odd so ref tile is in centre
		int[] possibleRoomSizes = new int[]{3,5};

		newRoom.xSize = possibleRoomSizes[Random.Range (0,2)];;
		newRoom.ySize = possibleRoomSizes[Random.Range (0,2)];

		Tile[] availableTiles = newRoom.GetAvailableTiles();

		// Check if room can be placed here first - avoid overlapping rooms + rooms which go out of boundary
		if (newRoom.hitsNull || newRoom.hitsAnotherRoom ||newRoom.referenceTile.getTileX() % 2 != 0 || newRoom.referenceTile.getTileY() % 2 != 0) {
			numberOfRooms++;
			return;
		}

		SetRoomType (newRoom);

		// Check if any rooms have nearby X or Ys - that would be awful!!
		for (int i = 0; i < (mainManager.tileManager.map.Length); i++) {

			if (mainManager.tileManager.map [i].getTileX () == newRoom.referenceTile.getTileX () + (((newRoom.xSize - 1) / 2) + 1)
			    || mainManager.tileManager.map [i].getTileX () == newRoom.referenceTile.getTileX () - (((newRoom.xSize - 1) / 2) + 1)
			    || mainManager.tileManager.map [i].getTileY () == newRoom.referenceTile.getTileY () + (((newRoom.ySize - 1) / 2) + 1)
			    || mainManager.tileManager.map [i].getTileY () == newRoom.referenceTile.getTileY () - (((newRoom.ySize - 1) / 2) + 1)) {

				if (mainManager.tileManager.map [i].isActive) {
					numberOfRooms++; 
					return;
				}
			}
		}
			

		// Place the room
		for (int i = 0; i < availableTiles.Length; i++) {
			availableTiles[i].spriteRenderer.enabled = true;
			availableTiles[i].isActive = true;
			availableTiles[i].tileType = Tile.TileType.roomTile;
			tileStack.Push(availableTiles[i]);
		}

		mainManager.tileManager.roomList.Add (newRoom);
	}

	void SetRoomType(Room roomBeingSet){
		bool anyRoomRL = false; bool anyRoomRB = false;
		bool anyRoomBL = false; bool anyRoomBB = false;
		bool anyRoomGL = false; bool anyRoomGB = false;

		foreach (Room room in mainManager.tileManager.roomList) {
			if (room.roomType == Room.RoomType.RedLockRoom)  	anyRoomRL = true;
			if (room.roomType == Room.RoomType.RedButtonRoom)  	anyRoomRB = true;
			if (room.roomType == Room.RoomType.BlueLockRoom)  	anyRoomBL = true;
			if (room.roomType == Room.RoomType.BlueButtonRoom)	anyRoomBB = true;
			if (room.roomType == Room.RoomType.GreenLockRoom)	anyRoomGL = true;
			if (room.roomType == Room.RoomType.GreenButtonRoom)	anyRoomGB = true;
		}
			
		if (anyRoomRL == false) {
			roomBeingSet.roomType = Room.RoomType.RedLockRoom;
		} else if (anyRoomRB == false) {
			roomBeingSet.roomType = Room.RoomType.RedButtonRoom;
		} else if (anyRoomBL == false) {
			roomBeingSet.roomType = Room.RoomType.BlueLockRoom;
		} else if (anyRoomBB == false) {
			roomBeingSet.roomType = Room.RoomType.BlueButtonRoom;
		} else if (anyRoomGL == false) {
			roomBeingSet.roomType = Room.RoomType.GreenLockRoom;
		} else if (anyRoomGB == false) {
			roomBeingSet.roomType = Room.RoomType.GreenButtonRoom;
		} else {
			roomBeingSet.roomType = Room.RoomType.Empty;
		}
	}

}
