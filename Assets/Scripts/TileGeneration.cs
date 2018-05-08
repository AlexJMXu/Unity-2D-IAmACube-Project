using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour {

	public GameObject InitialTile;
	public GameObject TilePrefab;
	public GameObject EmptyObject;
	public GameObject PlayerPrefab;

	public float offset = 1f;
	public int tileLimit = 100;

	private int tileCount;
	private int numberOfSides;

	//private float tile_width = InitialTile.GetComponent<SpriteRenderer>().bounds.size.x;
	//private float tile_height = InitialTile.GetComponent<SpriteRenderer>().bounds.size.y;
	private float tile_width = 2;
	private float tile_height = 1;

	// Just for Weighted selection (here there is way more chance that each tile will only be connected to one other)
	private List<int> NumberOfSidesList = new List<int>{1,1,1,1,2,3,4};

	public static int xBound = 25;
	public static int yBound = 25;
	
	// Create an array of 9 rows and 7 Columns (odd numbers used so there can be a centre point)
	// This is just temporary. It can be made adjustable in the future
	[HideInInspector] public static GameObject[,] diamondGrid = new GameObject[xBound, yBound];

	// Array of xy index to use with the diamondGrid  diamondGrid[0,0] is top left corner, diamondGrid[8][6] is bottom right
	int[] diamondIndex = new int[2];

	// Use this for initialization
	void Start () {
		tileCount = 1;
		InitialTile = GameObject.Find ("IsometricTile");
		createDiamondGrid();
	}

	// Recursive Function
	void PlaceTilesAround(int[] diamondIndex){

		int directionNumber; // Number from 1-4 which represents direction, 1=right, 2=down, 3=left, 4=right, used to index directions array

		// How many will be placed around the selected tile, selected from the numberOfSidesList (weighted probability)
		// If 1 is chosen, the selected tile will be connected to one other tile, if 4 is selected, the tile will be completely surrounded
		numberOfSides = NumberOfSidesList[Random.Range(0,NumberOfSidesList.Count-1)];

		// Once we know how many tiles surround the selected tile, we need to also choose random directions to place them
		// FillDirectionList will fill the directions array with only directions which are possible
		// (directions will not be possible if they go out of grid bounds/ or if there is already a tile there)
		List<string> directions = new List<string>();
		directions = FillDirectionList(diamondIndex);


		for (int i = 0; i < numberOfSides; i++) {

			// If we've run out of possible directions/ have reached tile limit, stop the process
			if (directions.Count == 0)
				return;
			if (tileCount >= tileLimit) {
				return;
			}

			directionNumber = Random.Range (0, directions.Count); // Once we have direction list, just generate a random number to index it
			int[] newIndex = InstantiateAtDirection (diamondIndex, directions[directionNumber]); // Instantiate the tiles at that random direction

			// This is a recursive function, the instantiate function called above will return the next index
			PlaceTilesAround (newIndex);  
			// Remove directions already used for this selected tile (this for loop)
			directions.RemoveAt (directionNumber);
		}
	}
		
	// Check all directions around the tile
	// If there is already a tile there, or if direction is out of the grid bounds, then a new tile cannot be initialised there
	// Therefore, that direaction will not be added to the direction array (possible directions for new tiles to be initialised in)
	List<string> FillDirectionList(int[] indexArray){
		int x = indexArray [0];
		int y = indexArray [1];
		List<string> newDirections = new List<string>{};

		if ((x+1)<xBound && !diamondGrid [x+1, y].CompareTag ("Tile")){
			newDirections.Add ("Right");
		} if ((y+1)<yBound && !diamondGrid [x, y+1].CompareTag ("Tile")){
			newDirections.Add ("Down");
		} if ((x-1)>=0 && !diamondGrid [x-1,y].CompareTag ("Tile")){
			newDirections.Add ("Left");
		} if ((y-1)>=0 && !diamondGrid [x,y-1].CompareTag ("Tile")){
			newDirections.Add ("Up");
		}
		return newDirections;
	}

	int[] InstantiateAtDirection(int[] indexArray, string direction){
		int[] newIndex = new int[2];
		int x = indexArray [0];
		int y = indexArray [1];

		if (direction == "Right") {

			diamondGrid [x+1, y] = Instantiate (TilePrefab, new Vector3 (
				diamondGrid[x, y].transform.position.x+(tile_width / 2), 
				diamondGrid[x, y].transform.position.y+(tile_height / 2), 
				0), 
			Quaternion.identity);

			newIndex[0] = x+1;
			newIndex[1] = y;

		} else if (direction == "Down") {

			diamondGrid[x,y+1] = Instantiate (TilePrefab, new Vector3 (
				diamondGrid[x,y].transform.position.x+(tile_width / 2), 
				diamondGrid[x,y].transform.position.y-(tile_height / 2), 
				0), 
			Quaternion.identity);
			
			newIndex[0] = x;
			newIndex[1] = y+1;

		} else if (direction == "Left") {

			diamondGrid[x-1,y] = Instantiate (TilePrefab, new Vector3 (
				diamondGrid[x,y].transform.position.x-(tile_width / 2), 
				diamondGrid[x,y].transform.position.y-(tile_height / 2), 
				0), 
			Quaternion.identity);

			newIndex[0] = x-1;
			newIndex[1] = y;

		} else if (direction == "Up") {

			diamondGrid[x,y-1] = Instantiate (TilePrefab, new Vector3 (
				diamondGrid[x,y].transform.position.x-(tile_width / 2), 
				diamondGrid[x,y].transform.position.y+(tile_height / 2), 
				0), 
			Quaternion.identity);
		
			newIndex[0] = x;
			newIndex[1] = y-1;

		} else {
			Debug.Log ("Something Went Wrong in InstantiateDirection");
		}
		tileCount++;
		return newIndex;
	}
		
	void createDiamondGrid(){

		// Instantiate grid with empty game object to start with
		for (int i = 0; i < xBound; i++){
			for (int j = yBound-1; j >= 0; j--)  {// This ordering was used so tiles don't overlap?
				diamondGrid[i, j] = Instantiate(
					EmptyObject, 
					new Vector3(
						InitialTile.transform.position.x+(j * tile_width / 2) + (i * tile_width / 2), 
						InitialTile.transform.position.y+(i * tile_height / 2) - (j * tile_height / 2), 
						0), 
					Quaternion.identity);
			}
		}

		// Instantiate middle tile in the centre of the grid
		diamondGrid[(xBound-1)/2,(yBound-1)/2] = Instantiate(
					TilePrefab, 
					new Vector3(
						InitialTile.transform.position.x+(3 * tile_width / 2) + (4 * tile_width / 2), 
						InitialTile.transform.position.y+(4 * tile_height / 2) - (3 * tile_height / 2), 
						0), 
					Quaternion.identity);

		//Instantiate the player on that middle tile in the centre
		Instantiate(PlayerPrefab, new Vector3(
						diamondGrid[(xBound-1)/2,(yBound-1)/2].transform.position.x, 
						diamondGrid[(xBound-1)/2,(yBound-1)/2].transform.position.y+0.5f, 
						0), Quaternion.identity);


  		//PlaceTilesAround the centre tile - the centre tile is the starting point of this recursive function
		diamondIndex[0] = (xBound-1)/2;
		diamondIndex[1] = (yBound-1)/2;
		PlaceTilesAround(diamondIndex);
	}

	// Update is called once per frame
	void Update () {
		
	}


}
