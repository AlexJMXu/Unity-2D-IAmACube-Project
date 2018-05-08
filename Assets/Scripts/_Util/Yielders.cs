using System.Collections.Generic;
using UnityEngine;
 
public static class Yielders {

	private static Dictionary<float, WaitForSeconds> timeInterval = new Dictionary<float, WaitForSeconds>();
	private static Dictionary<float, WaitForSecondsRealtime> timeIntervalRealTime = new Dictionary<float, WaitForSecondsRealtime>();

	public static WaitForSeconds Get(float seconds) {
		if (!timeInterval.ContainsKey(seconds)) {
			timeInterval.Add(seconds, new WaitForSeconds(seconds));
		}
		
		return timeInterval[seconds];
	}

	public static WaitForSecondsRealtime GetRealTime(float seconds) {
		if (!timeIntervalRealTime.ContainsKey(seconds)) {
			timeIntervalRealTime.Add(seconds, new WaitForSecondsRealtime(seconds));
		}
		
		return timeIntervalRealTime[seconds];
	}
}
