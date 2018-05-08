using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	private Dictionary<int, Player> players = new Dictionary<int, Player>();

	public Player GetPlayer(int id) {
		return players[id];
	}

	public void AddPlayer(int id, Player player) {
		players.Add(id, player);
	}
}
