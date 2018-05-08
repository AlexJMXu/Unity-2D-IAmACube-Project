using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

	public Text gamesCount;

	// Use this for initialization
	void Start () {
		if (UserAccountManager.isLoggedIn) {
			UserAccountManager.instance.GetData(OnReceivedData);
		}
	}
	
	void OnReceivedData(string data) {
		if (gamesCount == null) return;

		gamesCount.text = DataTranslator.DataToGames(data).ToString() + " GAMES PLAYED";
	}
}
