using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	private MainManager mainManager;

	[SerializeField] private GameObject startGameButton;
	[SerializeField] private GameObject waitText;
	[SerializeField] private GameObject timerText;

	public int pickupCount;
	public int scoreValue;
	public Text pickupCounterText;
	public Text scoreCounterText;

	[SerializeField] public GameObject[] readyText;

	private int playerReadyCount;

	public bool gameStarted;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Start () {
		mainManager = MainManager.instance;

		startGameButton.SetActive(true);

		playerReadyCount = 0;

		pickupCounterText.text = "Pickup Counter: " + pickupCount + " / " + mainManager.tileManager.numberOfPickups;
		scoreCounterText.text = "Score: " + scoreValue;
	}

	public void DisableStartButton() {
		startGameButton.SetActive(false);
		waitText.SetActive(true);
	}

	public void DisableStartButtonAndText() {
		startGameButton.SetActive(false);
		waitText.SetActive(false);
	}

	public void StartGame() {
		CustomNetworkManager.singleton.SetMatchPrivate();

		mainManager.tileManager.GenerateMap();

		Player[] players = GetAllPlayers();
		for (int i = 0; i < players.Length; i++) {
			players[i].Spawn(i);
		}
	}

	public void NextMap() {
		playerReadyCount++;

		if (playerReadyCount >= GetAllPlayers().Length) {
			playerReadyCount = 0;
			//NetworkManager.singleton.ServerChangeScene("GameScene");
			mainManager.tileManager.NextMap();

			Player[] players = GetAllPlayers();
			for (int i = 0; i < players.Length; i++) {
				players[i].Respawn();
			}
		}
	}

	public void SetAllReadyTextOff() {
		for (int i = 0; i < readyText.Length; i++) {
			readyText[i].SetActive(false);
		}
	}

	public void StartTimer() {
		timerText.SetActive(true);
		SetAllReadyTextOff();
		gameStarted = true;
	}

	public void ExitGame() {
		CustomNetworkManager.singleton.StopClient();
		if (Network.isServer) CustomNetworkManager.singleton.StopServer();

		//SceneManager.LoadScene("LobbyScene");
	}
	
	/*
	 * NETWORKING CODE *
	 */
	private const string PLAYER_ID_PREFIX = "Player ";

	private static Dictionary<string, Player> players = new Dictionary<string, Player>();

	public static void RegisterPlayer(string _netID, Player _player) {
		MainManager mainManager = MainManager.instance;

		if (mainManager.gameManager.gameStarted) return;

		string _playerID = PLAYER_ID_PREFIX + _netID;
		Debug.Log(_playerID + " Connected");
		players.Add(_playerID, _player);
		_player.transform.name = _playerID;

		for (int i = 0; i < GetAllPlayers().Length; i++) {
			mainManager.gameManager.readyText[i].SetActive(true);
		}
	}

	public static void UnregisterPlayer(string _netID, Player _player) {
		MainManager mainManager = MainManager.instance;

		string _playerID = PLAYER_ID_PREFIX + _netID;
		Debug.Log(_playerID + " Disconnected");
		players.Remove(_playerID);

		if (mainManager.gameManager.gameStarted) return;

		mainManager.gameManager.SetAllReadyTextOff();

		for (int i = 0; i < GetAllPlayers().Length; i++) {
			mainManager.gameManager.readyText[i].SetActive(true);
		}
	}

	public static Player GetPlayer(string _playerID) {
		return players[_playerID];
	}

	public static Player[] GetAllPlayers() {
		return players.Values.ToArray();
	}
		
	/*
	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log(player);
	}*/
}
