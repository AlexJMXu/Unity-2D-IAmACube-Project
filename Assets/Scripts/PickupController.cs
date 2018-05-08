using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour {

	void Update () {
		// Want to make the pickups spin
		// Rotation needs to be smooth and framerate independant - That's why multiplying by Time.deltaTime
		transform.Rotate(new Vector3(15,30,45) * Time.deltaTime);
	}
}
