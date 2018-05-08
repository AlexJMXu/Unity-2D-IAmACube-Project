using UnityEngine;
using System;

// Translate data in the database into strings/values
public class DataTranslator : MonoBehaviour {

	private static string GAMES_TAG = "[GAMES]";

	public static string ValuesToData(int games) {
		return GAMES_TAG + games;
	}

	public static int DataToGames(string data) {
		return int.Parse(DataToValue(data, GAMES_TAG));
	}

	private static string DataToValue (string data, string tag) {
		string[] pieces = data.Split('/');
		for (int i = 0; i < pieces.Length; i++) {
			if (pieces[i].StartsWith(tag)) {
				return pieces[i].Substring(tag.Length);
			}
		}

		Debug.LogError(tag + " not found in data.");
		return "";
	}
}
