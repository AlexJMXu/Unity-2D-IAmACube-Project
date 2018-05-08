using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour {

	public enum TileType{
		other,
		roomTile,
		corridorTile
	}

	public enum TileContains{
		empty,
		lockObj,
		button,
		pickup
	}

	public TileType tileType;
	public TileContains tileContains;
	public Room room;

	public GameObject pickupObject;
	public GameObject lockObject;
	public GameObject buttonObject;

	public int id;
	public bool isActive;
	public bool isOccupied; //Contains (another) player
	public bool isFading;

	public Tile top;
	public Tile bottom;
	public Tile left;
	public Tile right;

	public Transform associatedTransform;
	public SpriteRenderer spriteRenderer;

	public Color roomColor = Color.white;
	public Color corridorColor = new Color(136/255f, 160/255f, 24/255f, 1);

	public Coroutine fadingCoroutine = null;

	public int getTileX(){
		return id % TileManager.GRID_SIZE;
	}

	public int getTileY(){
		return id / TileManager.GRID_SIZE;
	}


	public void FadeTile ()
	{
		if (isFading)
			StopCoroutine (fadingCoroutine);

		fadingCoroutine = StartCoroutine(ColorTile ());
	}

	public void ForceStopFading() {
		if (fadingCoroutine != null) StopCoroutine(fadingCoroutine);
	}

	IEnumerator ColorTile(){
		isFading = true;

		Color originalColor = this.spriteRenderer.color;
		if (tileType == Tile.TileType.roomTile) {
			originalColor = roomColor;
		} else if (tileType == Tile.TileType.corridorTile) {
			originalColor = corridorColor;
		}

		this.spriteRenderer.color = Color.red;

		float duration = 3f;
		float startingTime = Time.time;
		float endTime = startingTime + duration;

		do
		{
			float t = (Time.time - startingTime) / duration;
			this.spriteRenderer.color = Color.Lerp(Color.red, originalColor, t);
			yield return null;
		} while (Time.time <= endTime);

		isFading = false;
	}

	/*
	public Tile[] GetSurroundingActiveTiles(){
		HashSet<Tile> tileSet = new HashSet<Tile> ();
			
		if (this.top != null && this.top.isActive && this.top.tileContains != Tile.TileContains.lockObj)
			tileSet.Add (this.top);
		if (this.bottom != null && this.bottom.isActive && this.bottom.tileContains != Tile.TileContains.lockObj)
			tileSet.Add (this.bottom);
		if (this.left != null && this.left.isActive && this.left.tileContains != Tile.TileContains.lockObj)
			tileSet.Add (this.left);
		if (this.right != null && this.right.isActive && this.right.tileContains != Tile.TileContains.lockObj)
			tileSet.Add (this.right);

		return tileSet.ToArray ();
	}*/

}
