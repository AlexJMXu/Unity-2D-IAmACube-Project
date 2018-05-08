using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class JoinGame : MonoBehaviour {

	List<GameObject> roomList = new List<GameObject>();

	[SerializeField] private Text status;
	[SerializeField] private GameObject roomListItemPrefab;
	[SerializeField] private Transform roomListParent;

	private CustomNetworkManager networkManager;

	private Coroutine refreshCoroutine;

	void Start() {
		networkManager = CustomNetworkManager.singleton;
		if (networkManager.matchMaker == null) {
			networkManager.StartMatchMaker();
		}

		RefreshRoomList();

		refreshCoroutine = StartCoroutine(RefreshRoomListCoroutine());
	}

	public void RefreshRoomList() {
		ClearRoomList();

		if (networkManager.matchMaker == null) {
			networkManager.StartMatchMaker();
		}

		status.text = "Loading...";
		networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
	}

	private IEnumerator RefreshRoomListCoroutine() {
		while(true) {
			yield return Yielders.Get(5f);
			RefreshRoomList();
		}
	}

	public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {
		status.text = "";
		if (!success || matchList == null) {
			status.text = "Couldn't get room list.";
			return;
		}

		foreach (MatchInfoSnapshot match in matchList) {
			GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
			_roomListItemGO.transform.SetParent(roomListParent, false);
			RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
			if (_roomListItem != null) {
				_roomListItem.Setup(match, JoinRoom);
			}

			roomList.Add(_roomListItemGO);
		}

		if (roomList.Count == 0) {
			status.text = "No rooms found.";
		}
	}

	private void ClearRoomList() {
		for (int i = 0; i < roomList.Count; i++) {
			Destroy(roomList[i]);
		}

		roomList.Clear();
	}

	public void JoinRoom(MatchInfoSnapshot _match) {
		networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
		if (refreshCoroutine != null) StopCoroutine(refreshCoroutine);
		StartCoroutine(WaitForJoin());
	}

	IEnumerator WaitForJoin() {
		ClearRoomList();

		int countdown = 10;
		while (countdown > 0) {
			status.text = "Joining... (" + countdown + ")";
			yield return Yielders.Get(1f);

			countdown--;	
		}

		// If it reaches here it means it failed to connect
		status.text = "Connection Timeout. Failed to connect.";
		yield return Yielders.Get(1f);

		MatchInfo matchInfo = networkManager.matchInfo;
		if (matchInfo != null) {
			networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
			networkManager.StopHost();
		}

		RefreshRoomList();
		refreshCoroutine = StartCoroutine(RefreshRoomListCoroutine());
	}

}
