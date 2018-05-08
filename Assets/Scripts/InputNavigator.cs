using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputNavigator : MonoBehaviour {
	EventSystem system;
	Selectable next = null;

	// Use this for initialization
	void Start () {
		system = EventSystem.current;
	}

	// Update is called once per frame
	public void Update() {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			Selectable nextOne = null;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				nextOne = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();

				if (nextOne == null) return;
				else next = nextOne;

			} else {
				nextOne = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

				if (nextOne == null) return;
				else next = nextOne;
			}

			if (next != null) {
				InputField inputfield = next.GetComponent<InputField>();

				if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));  // If it's an input field, also set the text caret

				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			}

		}
	}
}