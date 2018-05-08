using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class CustomNetworkManager : NetworkManager {

	public static CustomNetworkManager singleton;

	void Awake() {
		if (singleton == null) {
			singleton = this;
		} else {
			Destroy(gameObject);
		}
	}

	public override void OnClientConnect(NetworkConnection conn) {
        //base.OnClientConnect(conn);
    }

    public void SetMatchPrivate() {
    	if (matchMaker != null) {
    		matchMaker.SetMatchAttributes(matchInfo.networkId, false, matchInfo.domain, OnSetMatchAttributes);
    	}
    }
}
