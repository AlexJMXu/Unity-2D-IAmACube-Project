using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

	private const float TILE_OFFSET_X = 1f;
	private const float TILE_OFFSET_Y = 0.5f;

	[SerializeField] private Transform tileParent;
	[SerializeField] private GameObject tilePrefab;

	private int[] tileWeight = {1, 1, 1, 1, 1, 2, 3, 4};

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

	/* 
	 * Generate the actual playable area for the players.
	 * Done by randomly selecting an initial tile, and
	 * generating tiles from that through a loop
	 * until the maximum tile limit is reached.
	 */
	public void GenerateMap(Tile[] map, int min, int max) {
		int tileCount = 1;
		Stack<Tile> tileStack = new Stack<Tile>();

		Tile initialTile = map[Random.Range(0, map.Length)];
		tileStack.Push(initialTile);

		initialTile.spriteRenderer.enabled = true;
		initialTile.isActive = true;

		while (true) {
			if (tileStack.Count > 0) {
				Tile currentTile = tileStack.Pop();

				int weight = tileWeight[Random.Range(0, tileWeight.Length)];

				for (int i = 0; i < weight; i++) {
					Tile[] availableTiles = GetAvailableTiles(currentTile);

					if (availableTiles.Length > 0) {
						int selectTile = Random.Range(0, availableTiles.Length);
						availableTiles[selectTile].spriteRenderer.enabled = true;
						availableTiles[selectTile].isActive = true;
						tileStack.Push(availableTiles[selectTile]);

						tileCount++;

						if (tileCount == max) return;
					}
				}
			} else {
				break;
			}
		}
	}

	/* 
	 * Utility function to find available tiles to generate
	 * by checking which ones have not been "activated" yet
	 */
	private Tile[] GetAvailableTiles(Tile tile) {
		HashSet<Tile> tileSet = new HashSet<Tile>();

		if (tile.top != null && !tile.top.isActive) tileSet.Add(tile.top);
		if (tile.bottom != null && !tile.bottom.isActive) tileSet.Add(tile.bottom);
		if (tile.left != null && !tile.left.isActive) tileSet.Add(tile.left);
		if (tile.right != null && !tile.right.isActive) tileSet.Add(tile.right);

		return tileSet.ToArray();
	}
}
