using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour {

	public Text usernameText;

	void Start() {
		if (UserAccountManager.isLoggedIn)
			usernameText.text = UserAccountManager.LoggedIn_Username;
	}

	public void Logout() {
		if (UserAccountManager.isLoggedIn)
			UserAccountManager.instance.Logout();
	}
}
