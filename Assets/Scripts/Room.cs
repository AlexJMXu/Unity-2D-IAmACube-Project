using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room {

	// STATE

	public enum RoomType{
		RedButtonRoom,
		BlueButtonRoom,
		GreenButtonRoom,
		RedLockRoom,
		BlueLockRoom,
		GreenLockRoom,
		Empty
	};

	// VARIABLES
	public Tile referenceTile;
	public int xSize;
	public int ySize;

	public bool hitsNull = false;
	public bool hitsAnotherRoom = false;

	public RoomType roomType;

	// COMPONENTS
	//[SerializeField] public Transform associatedTransform;


	public Tile[] GetAvailableTiles() {
		HashSet<Tile> tileSet = new HashSet<Tile> ();

		tileSet.Add (referenceTile);

		//Add Tiles Above ReferenceTile (same x)
		Tile currentTile = referenceTile;
		for (int i = 0; i < (((ySize - 1)) / 2); i++) {
			
			if (currentTile.top == null || currentTile.top.top == null) {
				this.hitsNull = true;
				return tileSet.ToArray ();
			}

			if (currentTile.top.isActive || currentTile.top.top.isActive) {
				this.hitsAnotherRoom = true;
				return tileSet.ToArray ();
			}

			tileSet.Add (currentTile.top);
			currentTile = currentTile.top;
		}
			
		// Add Tiles Below ReferenceTile (same x)
		currentTile = referenceTile;
		for (int i = 0; i < (((ySize - 1)) / 2); i++) {
			
			if (currentTile.bottom == null || currentTile.bottom.bottom == null) {
				this.hitsNull = true;
				return tileSet.ToArray ();
			}

			if (currentTile.bottom.isActive || currentTile.bottom.bottom.isActive) {
				this.hitsAnotherRoom = true;
				return tileSet.ToArray ();
			}

			tileSet.Add (currentTile.bottom);
			currentTile = currentTile.bottom;
		}

		// Add all tiles to left, for each tile to the left, add the ones above it and below it as well
		currentTile = referenceTile;
		for (int i = 0; i < (((xSize - 1)) / 2); i++) {
			
			if (currentTile.left == null || currentTile.left.left == null) {
				this.hitsNull = true;
				return tileSet.ToArray ();
			}

			if (currentTile.left.isActive || currentTile.left.left.isActive) {
				this.hitsAnotherRoom = true;
				return tileSet.ToArray ();
			}


			tileSet.Add (currentTile.left);
			currentTile = currentTile.left;
			Tile currentVTile = currentTile;

			for (int j = 0; j < (((ySize - 1)) / 2); j++) {

				if (currentVTile.top.isActive || currentVTile.top.top.isActive) {
					this.hitsAnotherRoom = true;
					return tileSet.ToArray ();
				}

				tileSet.Add (currentVTile.top);
				currentVTile = currentVTile.top;
			}

			currentVTile = currentTile;
			for (int k = 0; k < (((ySize - 1)) / 2); k++) {

				if (currentVTile.bottom.isActive || currentVTile.bottom.bottom.isActive) {
					this.hitsAnotherRoom = true;
					return tileSet.ToArray ();
				}

				tileSet.Add (currentVTile.bottom);
				currentVTile = currentVTile.bottom;
			}
		}

		// Now the same for the tiles on the right
		currentTile = referenceTile;
		for (int i = 0; i < (((xSize - 1) / 2)); i++) {

			if (currentTile.right == null || currentTile.right.right == null) {
				this.hitsNull = true;
				return tileSet.ToArray ();
			}

			if (currentTile.right.isActive || currentTile.right.right.isActive) {
				this.hitsAnotherRoom = true;
				return tileSet.ToArray ();
			}

			tileSet.Add (currentTile.right);
			currentTile = currentTile.right;
			Tile currentVTile = currentTile;

			for (int j = 0; j < (((ySize - 1) / 2)); j++) {
				
				if (currentVTile.top.isActive || currentVTile.top.top.isActive) {
					this.hitsAnotherRoom = true;
					return tileSet.ToArray ();
				}

				tileSet.Add (currentVTile.top);
				currentVTile = currentVTile.top;
			}

			currentVTile = currentTile;
			for (int k = 0; k < (((ySize - 1) / 2)); k++) {

				if (currentVTile.bottom.isActive || currentVTile.bottom.bottom.isActive) {
					this.hitsAnotherRoom = true;
					return tileSet.ToArray ();
				}

				tileSet.Add (currentVTile.bottom);
				currentVTile = currentVTile.bottom;
			}
		}
		return tileSet.ToArray ();
	}



	public Tile[] GetRoomTiles() {
		HashSet<Tile> tileSet = new HashSet<Tile> ();

		tileSet.Add (referenceTile);

		//Add Tiles Above ReferenceTile (same x)
		Tile currentTile = referenceTile;
		for (int i = 0; i < (((ySize - 1)) / 2); i++) {
			currentTile = currentTile.top;
			tileSet.Add (currentTile);
		}

		// Add Tiles Below ReferenceTile (same x)
		currentTile = referenceTile;
		for (int i = 0; i < (((ySize - 1)) / 2); i++) {
			currentTile = currentTile.bottom;
			tileSet.Add (currentTile);
		}

		// Add all tiles to left, for each tile to the left, add the ones above it and below it as well
		currentTile = referenceTile;
		for (int i = 0; i < (((xSize - 1)) / 2); i++) {
			currentTile = currentTile.left;
			tileSet.Add (currentTile);
			Tile currentVTile = currentTile;

			for (int j = 0; j < (((ySize - 1)) / 2); j++) {
				currentVTile = currentVTile.top;
				tileSet.Add (currentVTile);
			}

			currentVTile = currentTile;
			for (int k = 0; k < (((ySize - 1)) / 2); k++) {
				currentVTile = currentVTile.bottom;
				tileSet.Add (currentVTile);
			}
		}

		// Now the same for the tiles on the right
		currentTile = referenceTile;
		for (int i = 0; i < (((xSize - 1) / 2)); i++) {
			currentTile = currentTile.right;
			tileSet.Add (currentTile);

			Tile currentVTile = currentTile;

			for (int j = 0; j < (((ySize - 1) / 2)); j++) {
				currentVTile = currentVTile.top;
				tileSet.Add (currentVTile);
			}

			currentVTile = currentTile;
			for (int k = 0; k < (((ySize - 1) / 2)); k++) {
				currentVTile = currentVTile.bottom;
				tileSet.Add (currentVTile);
			}
		}
		return tileSet.ToArray ();
	}


	public Tile[] GetCorridorBoundaryTiles(){

		HashSet<Tile> tileSet = new HashSet<Tile> ();

		Tile currentTile = this.referenceTile;
		//Move current Tile to bottomleft corner of BOUNDARY 
		for (int i = 0; i < ((this.xSize - 1) / 2)+1; i++) {
			currentTile = currentTile.left;
		}
		for (int i = 0; i < ((this.ySize - 1) / 2)+1; i++) {
			currentTile = currentTile.bottom;
		}

		// Check the boundary tiles for active corridor tiles ~ will need to lock them

		// Check tiles above ~ just check for active
		for (int i = 0; i < (this.ySize) + 1; i++) {
			currentTile = currentTile.top;
			if (currentTile.isActive) {
				tileSet.Add (currentTile);
			} 
		}
		// Check tiles right
		for (int i = 0; i < (this.xSize) + 1; i++) {
			currentTile = currentTile.right;
			if (currentTile.isActive) {
				tileSet.Add (currentTile);
			}
		}
		// Check tiles below
		for (int i = 0; i < (this.ySize) + 1; i++) {
			currentTile = currentTile.bottom;
			if (currentTile.isActive) {
				tileSet.Add (currentTile);
			}
		}
		// Check tiles left
		for (int i = 0; i < (this.xSize) + 1; i++) {
			currentTile = currentTile.left;
			if (currentTile.isActive) {
				tileSet.Add (currentTile);
			}
		}

		return tileSet.ToArray ();
	}

}
