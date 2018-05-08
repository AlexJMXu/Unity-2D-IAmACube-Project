using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField] private Behaviour[] componentsToDisable;

	[SerializeField] private string remoteLayerName = "RemotePlayer";

	void Start() {
		if (!isLocalPlayer) {
			DisableComponents();
		}
	}

	public override void OnStartClient() {
		base.OnStartClient();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();

		GameManager.RegisterPlayer(_netID, _player);
	}

	private void DisableComponents() {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable[i].enabled = false;
		}
	}

	void OnDisable() {
		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();

		GameManager.UnregisterPlayer(_netID, _player);
		if (isServer) CustomNetworkManager.singleton.SetMatchPrivate();

		//CustomNetworkManager.singleton.StopClient();

		//if (isServer) CustomNetworkManager.singleton.StopServer();

		if (GetComponent<Player>().currentTile != null) GetComponent<Player>().currentTile.isOccupied = false;
	}

	void OnDestroy() {
		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();

		GameManager.UnregisterPlayer(_netID, _player);
		if (isServer) CustomNetworkManager.singleton.SetMatchPrivate();

		//CustomNetworkManager.singleton.StopClient();

		//if (isServer) CustomNetworkManager.singleton.StopServer();

		if (GetComponent<Player>().currentTile != null) GetComponent<Player>().currentTile.isOccupied = false;
	}
}
