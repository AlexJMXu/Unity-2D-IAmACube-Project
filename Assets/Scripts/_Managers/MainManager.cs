using UnityEngine;

public class MainManager : MonoBehaviour {

	public static MainManager instance;

	public GameManager gameManager;
	public TileManager tileManager;
	public PlayerManager playerManager;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}
}
