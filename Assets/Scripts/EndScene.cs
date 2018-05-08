using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndScene : NetworkBehaviour {

	public void FinishGame() {
		CustomNetworkManager.singleton.StopClient();
		if (Network.isServer) CustomNetworkManager.singleton.StopServer();
	}
}
